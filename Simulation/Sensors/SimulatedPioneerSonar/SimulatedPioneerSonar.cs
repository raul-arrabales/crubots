//------------------------------------------------------------------------------
//
// SIMULATED PIONEER SONAR SERVICE
//
// CRANIUM - SIMULATED SONAR SENSOR
// This service provides access to a simulated P3DX frontal Sonar Array. 
// It uses the LRF raycasting to simulate Sonar transducers. 
//
// As suggested by Kyle - MSFT: 
// "you can make a reasonable simulated sonar sensor by doing something similar 
// to the simulated laser rangefinder.  Instead of casting hundreds of rays in a 
// scanline pattern like the laser rangefinder does, cast a handful of rays in a 
// cone that matches the aperture of the sonar sensor you want to simulate.  In 
// your code, look at the distance results returned by each ray and then set the 
// sonar return value to be the closest intersection.  
// 
// This code is provided AS IS. No warranty is provided for any purpose. 
// Use it at your own risk. 
// Please notify bugs, suggestions to: raul@conscious-robots.com
//
//  $File: SimulatedSonar.cs $ $Revision: update 6 $
//------------------------------------------------------------------------------



using System;
using System.Collections.Generic;
using Microsoft.Ccr.Core;
using Microsoft.Dss.Core;
using Microsoft.Dss.Core.Attributes;
using permissions = System.Security.Permissions;
using svcbase = Microsoft.Dss.ServiceModel.DsspServiceBase;
using dssp = Microsoft.Dss.ServiceModel.Dssp;

// Raul - SONAR Generic Contract
// Raul - NOTE that this service simulates just ONE SONAR TRANSDUCER - NOT A SONAR RING!
// Raul - To simulate the P3DX Sonar ring, eight Sonar transducers have to be added in the simulation.
using pxsonar = Microsoft.Robotics.Services.Sonar.Proxy;

using submgr = Microsoft.Dss.Services.SubscriptionManager;
using Microsoft.Robotics.Simulation;
using Microsoft.Robotics.Simulation.Engine;
using Microsoft.Robotics.Simulation.Physics;
using Microsoft.Robotics.PhysicalModel;
using System.ComponentModel;
using Microsoft.Dss.Core.DsspHttp;
using System.Net;


using xna = Microsoft.Xna.Framework;
using xnagrfx = Microsoft.Xna.Framework.Graphics;
using xnaprof = Microsoft.Robotics.Simulation.MeshLoader;

namespace Cranium.Simulation.Sensors
{

    /// <summary>
    /// Models a SONAR using physics raycasting to determine impact points
    /// </summary>
    [DataContract]
    [CLSCompliant(true)]
    [EditorAttribute(typeof(GenericObjectEditor), typeof(System.Drawing.Design.UITypeEditor))]
    public class SonarEntity : VisualEntity
    {
        CachedEffectParameter _timeAttenuationHandle;

        Shape _particlePlane;
        float _elapsedSinceLastScan;
        float _appTime;
        Port<RaycastResult> _raycastResultsPort;
        RaycastResult _lastResults;
        /// <summary>
        /// We scan 4 times a second
        /// </summary>
        const float SCAN_INTERVAL = 0.250f;
        const float IMPACT_SPHERE_RADIUS = 0.02f;

        RaycastProperties _raycastProperties;
        /// <summary>
        /// Raycast configuration
        /// </summary>
        public RaycastProperties RaycastProperties
        {
            get { return _raycastProperties; }
            set { _raycastProperties = value; }
        }

        BoxShape _sonarBox;

        /// <summary>
        /// Geometric representation of sonar physical sensor
        /// </summary>
        [DataMember]
        [Description("The geometry representation of the sonar.")]
        public BoxShape SonarBox
        {
            get { return _sonarBox; }
            set { _sonarBox = value; }
        }

        Port<RaycastResult> _serviceNotification;

        /// <summary>
        /// Default constructor
        /// </summary>
        public SonarEntity() 
        { 
        }

        /// <summary>
        /// Initialization constructor
        /// </summary>
        /// <param name="localPose"></param>
        public SonarEntity(Pose localPose)
        {
            // create a new instance of the sonar pose so we dont re-use the raycast reference
            // That reference will be updated regularly
            BoxShapeProperties box = new BoxShapeProperties("Sonar", 0.5f,
                localPose,
                new Vector3(0.1f, 0.1f, 0.1f));
            _sonarBox = new BoxShape(box);
            State.Assets.Effect = "LaserRangeFinder.fx";
        }

        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="device"></param>
        /// <param name="physicsEngine"></param>
        public override void Initialize(xnagrfx.GraphicsDevice device, PhysicsEngine physicsEngine)
        {
            try
            {
                InitError = string.Empty;
                // set flag so rendering engine renders us last
                Flags |= VisualEntityProperties.UsesAlphaBlending;

                // creates effect, loads meshes, etc
                base.Initialize(device, physicsEngine);

                HeightFieldShapeProperties hf = new HeightFieldShapeProperties("height field", 2, 0.02f, 2, 0.02f, 0, 0, 1, 1);
                hf.HeightSamples = new HeightFieldSample[hf.RowCount * hf.ColumnCount];
                for (int i = 0; i < hf.HeightSamples.Length; i++)
                    hf.HeightSamples[i] = new HeightFieldSample();

                _particlePlane = new Shape(hf);
                _particlePlane.State.Name = "sonar impact plane";

                VisualEntityMesh sonarMesh = null;

                // we render on our own only the laser impact points. The laser Box is rendered as part of the parent.
                int index = Meshes.Count;
                Meshes.Add(SimulationEngine.ResourceCache.CreateMesh(device, _particlePlane.State));
                Meshes[0].Textures[0] = SimulationEngine.ResourceCache.CreateTextureFromFile(device, "particle.bmp");

                // we have a custom effect, with an additional global parameter. Get handle to it here
                if (Effect != null)
                    _timeAttenuationHandle = Effect.GetParameter("timeAttenuation");

                World = xna.Matrix.Identity;
                if (Meshes.Count > 1)
                    sonarMesh = Meshes[0];

                if (Parent == null)
                    throw new Exception("This entity must be a child of another entity.");

                Parent.AddShapeToPhysicsEntity(_sonarBox, sonarMesh);
            }
            catch (Exception ex)
            {
                HasBeenInitialized = false;
                InitError = ex.ToString();
            }
        }

        /// <summary>
        /// Frame update
        /// </summary>
        /// <param name="update"></param>
        public override void Update(FrameUpdate update)
        {
            if (_raycastProperties == null)
            {
                base.Update(update);
                return;
            }

            _appTime = (float)update.ApplicationTime;

            // assume pose of parent
            if (Parent != null)
                State.Pose = Parent.State.Pose;

            _elapsedSinceLastScan += (float)update.ElapsedTime;
            // only retrieve raycast results every SCAN_INTERVAL.
            // For entities that are compute intenisve, you should consider giving them
            // their own task queue so they dont flood a shared queue
            if (_elapsedSinceLastScan > SCAN_INTERVAL)
            {
                _elapsedSinceLastScan = 0;
                // the LRF looks towards the negative Z axis (towards the user), not the positive Z axis
                // which is the default orientation. So we have to rotate its orientation by 180 degrees

                _raycastProperties.OriginPose.Orientation = TypeConversion.FromXNA(
                    TypeConversion.ToXNA(State.Pose.Orientation) * xna.Quaternion.CreateFromAxisAngle(new xna.Vector3(0, 1, 0), (float)Math.PI));

                // to calculate the position of the origin of the raycast, we must first rotate the LocalPose position
                // of the raycast (an offset from the origin of the parent entity) by the orientation of the parent entity.
                // The origin of the raycast is then this rotated offset added to the parent position.
                xna.Matrix parentOrientation = xna.Matrix.CreateFromQuaternion(TypeConversion.ToXNA(State.Pose.Orientation));
                xna.Vector3 localOffset = xna.Vector3.Transform(TypeConversion.ToXNA(_sonarBox.State.LocalPose.Position), parentOrientation);

                _raycastProperties.OriginPose.Position = State.Pose.Position + TypeConversion.FromXNA(localOffset);
                _raycastResultsPort = PhysicsEngine.Raycast2D(_raycastProperties);
                _raycastResultsPort.Test(out _lastResults);
                if (_serviceNotification != null && _lastResults != null)
                {
                    _serviceNotification.Post(_lastResults);
                }
            }

            base.Update(update);
        }

        /// <summary>
        /// Frame render
        /// </summary>
        public override void Render(RenderMode renderMode, MatrixTransforms transforms, CameraEntity currentCamera)
        {
            if ((int)(Flags & VisualEntityProperties.DisableRendering) > 0)
                return;

            if (_lastResults != null)
                RenderResults(renderMode, transforms, currentCamera);
        }

        void RenderResults(RenderMode renderMode, MatrixTransforms transforms, CameraEntity currentCamera)
        {
            _timeAttenuationHandle.SetValue(new xna.Vector4(100 * (float)Math.Cos(_appTime * (1.0f / SCAN_INTERVAL)), 0, 0, 1));

            // render impact points as a quad
            xna.Matrix inverseViewRotation = currentCamera.ViewMatrix;
            inverseViewRotation.M41 = inverseViewRotation.M42 = inverseViewRotation.M43 = 0;
            xna.Matrix.Invert(ref inverseViewRotation, out inverseViewRotation);
            xna.Matrix localTransform = xna.Matrix.CreateFromAxisAngle(new xna.Vector3(1, 0, 0), (float)-Math.PI / 2) * inverseViewRotation;

            SimulationEngine.GlobalInstance.Device.RenderState.DepthBufferWriteEnable = false;
            for (int i = 0; i < _lastResults.ImpactPoints.Count; i++)
            {
                xna.Vector3 pos = new xna.Vector3(_lastResults.ImpactPoints[i].Position.X,
                    _lastResults.ImpactPoints[i].Position.Y,
                    _lastResults.ImpactPoints[i].Position.Z);

                xna.Vector3 resultDir = pos - Position;
                resultDir.Normalize();

                localTransform.Translation = pos - .02f * resultDir;
                transforms.World = localTransform;

                base.Render(renderMode, transforms, Meshes[0]);
            }
            SimulationEngine.GlobalInstance.Device.RenderState.DepthBufferWriteEnable = true;
        }

        /// <summary>
        /// Registers a port for queueing raycast results from the physics engine
        /// </summary>
        /// <param name="notificationTarget"></param>
        public void Register(Port<RaycastResult> notificationTarget)
        {
            if (notificationTarget == null)
                throw new ArgumentNullException("notificationTarget");
            if (_serviceNotification != null)
                throw new InvalidOperationException("A notification target is already registered");
            _serviceNotification = notificationTarget;
        }
    }



    /// <summary>
    /// Provides access to a simulated SONAR contract 
    /// using physics raycasting and the LaserRangeFinderEntity
    /// </summary>
    [DisplayName("Simulated SONAR")]
    [Description("Provides access to a simulated SONAR.\n(Uses the Generic Sonar contract.)")]
    [AlternateContract(pxsonar.Contract.Identifier)]
    // Raul - [AlternateContract(sicklrf.Contract.Identifier)]
    [Contract(Contract.Identifier)]    
    // Raul - public class SimulatedLRFService : svcbase.DsspServiceBase
    public class SimulatedPioneerSonar : svcbase.DsspServiceBase
    {
        // Raul - Service Initial state partner file and initially set to null.
        public const string InitialStateUri = ServicePaths.MountPoint + @"/packages/crubots/Config/SimulatedPioneerSonar.Config.xml";
        [InitialStatePartner(Optional = true, ServiceUri = InitialStateUri)]
        private pxsonar.SonarState _state = null;

        #region Simulation Variables
        SonarEntity _entity;
        SimulationEnginePort _notificationTarget;
        PhysicsEngine _physicsEngine;
        Port<RaycastResult> _raycastResults = new Port<RaycastResult>();
        #endregion


        /// <summary>
        /// Number of SONAR devices in the simulated SONAR Ring.
        /// </summary>
        public const int SonarArrayLength = 8;

        /// <summary>
        /// This threshold value established whether or not to notify a Simulated SONAR Replace.
        /// </summary>
        public const double SonarChangeThreshold = 100.0;


        // Raul - The P3DX Sonar Range can be 0.10 m to 4m. 
        // const float LASER_RANGE = 8f;
        const float SONAR_RANGE = 4000f;

        /// <summary>
        /// This array hold the former reading.
        /// </summary>
        private double[] formerDistanceMeasurements;


        [ServicePort("/SimulatedSonar", AllowMultipleInstances = true)]
        private pxsonar.SonarOperations _mainPort = new pxsonar.SonarOperations();

        // Raul - Subscription Manager
        [Partner("SubMgr", Contract = submgr.Contract.Identifier, CreationPolicy = PartnerCreationPolicy.CreateAlways)]
        submgr.SubscriptionManagerPort _subMgrPort = new submgr.SubscriptionManagerPort();

        // Raul - Default constructor
        public SimulatedPioneerSonar(dssp.DsspServiceCreationPort creationPort) : 
                base(creationPort)
        {
			
        }


        protected override void Start()
        {
            _physicsEngine = PhysicsEngine.GlobalInstance;
            _notificationTarget = new SimulationEnginePort();

            // PartnerType.Service is the entity instance name. 
            SimulationEngine.GlobalInstancePort.Subscribe(ServiceInfo.PartnerList, _notificationTarget);

            LogInfo("Starting Simulated Sonar");

            // Raul - If state cannot be read from file create a default one
            if (_state == null)
            {
                _state = CreateDefaultState();
            }
            else // Use the state saved in the file, but don't forget to allocate memory for arrays.
            {
                _state.DistanceMeasurements = new double[SonarArrayLength];
                formerDistanceMeasurements = new double[SonarArrayLength];
            }

            // dont start listening to DSSP operations, other than drop, until notification of entity
            Activate(new Interleave(
                new TeardownReceiverGroup
                (
                    Arbiter.Receive<InsertSimulationEntity>(false, _notificationTarget, InsertEntityNotificationHandlerFirstTime),
                    Arbiter.Receive<dssp.DsspDefaultDrop>(false, _mainPort, DefaultDropHandler)
                ),
                new ExclusiveReceiverGroup(),
                new ConcurrentReceiverGroup()
            ));

            // Publish the service to the local Node Directory
            // DirectoryInsert();
        }



        /// <summary>
        /// Create default service state
        /// </summary>
        pxsonar.SonarState CreateDefaultState()
        {
            pxsonar.SonarState temp_state = new pxsonar.SonarState();

            // Allocation for Arcos Sensor readings.
            temp_state.DistanceMeasurements = new double[SonarArrayLength];
            formerDistanceMeasurements = new double[SonarArrayLength];

            temp_state.MaxDistance = 4000;
            temp_state.HardwareIdentifier = 0;
            temp_state.TimeStamp = DateTime.Now; 

            // Initialize Sonar readings.
            for (int i = 0; i < SonarArrayLength; i++)
            {
                temp_state.DistanceMeasurements[i] = 0.0;
                formerDistanceMeasurements[i] = 0.0;
            }

            // Raul - P3DX front ring sonar is 180 degress. But lateral transducers are
            // Raul - centered at 90 degrees, so I consider: 196 degrees.
            // Raul - I considered the aperture of one transducer is 16 degrees.
            temp_state.AngularRange = 196;  // 180 plus two halfs of lateral transducers.

            // Raul - Angle increment for sonar transducer rays.
            temp_state.AngularResolution = 1.0; // let's generate one ray per degree.

            // Save state in case it doesn't exist.
            SaveState(temp_state);

            return temp_state;
        }


        void DeleteEntityNotificationHandler(DeleteSimulationEntity del)
        {
            _entity = null;
        }

        void InsertEntityNotificationHandlerFirstTime(InsertSimulationEntity ins)
        {
            InsertEntityNotificationHandler(ins);
            base.Start();
            MainPortInterleave.CombineWith(
                new Interleave(
                    new TeardownReceiverGroup(),
                    new ExclusiveReceiverGroup(
                        Arbiter.Receive<InsertSimulationEntity>(true, _notificationTarget, InsertEntityNotificationHandler),
                        Arbiter.Receive<DeleteSimulationEntity>(true, _notificationTarget, DeleteEntityNotificationHandler)
                    ),
                    new ConcurrentReceiverGroup()
                )
            );
        }

        void InsertEntityNotificationHandler(InsertSimulationEntity ins)
        {
            _entity = (SonarEntity)ins.Body;
            _entity.ServiceContract = Contract.Identifier;

            // CreateDefaultState();

            RaycastProperties raycastProperties = new RaycastProperties();            
            raycastProperties.StartAngle = (float) -_state.AngularRange / 2.0f;
            raycastProperties.EndAngle = (float) _state.AngularRange / 2.0f;
            raycastProperties.AngleIncrement = (float)_state.AngularResolution;
            // raycastProperties.Range = LASER_RANGE;
            raycastProperties.Range = SONAR_RANGE;
            // Raul - This should be the pose of the sonar transducer.
            // Raul - I consider 8 Sonar transducers rays in the P3DX frontal ring
            // Raul - coming from a common central point.
            raycastProperties.OriginPose = new Pose();

            _entity.RaycastProperties = raycastProperties;
            _entity.Register(_raycastResults);

            // attach handler to raycast results port
            Activate(Arbiter.Receive(true, _raycastResults, RaycastResultsHandler));
        }

        private void RaycastResultsHandler(RaycastResult result)
        {
            // we just receive ray cast information from physics. Currently we just use
            // the distance measurement for each impact point reported. However, our simulation
            // engine also provides you the material properties so you can decide here to simulate
            // scattering, reflections, noise etc.
            
            // Raul - Note the closest intersection.
            double minDistance = SONAR_RANGE; 

            // Raul - The P3DX Frontal Sonar Ring have 8 SONAR transducers:
            // Raul - From left to right: S7, S6, S5, S4, S3, S2, S1, and S0.
            // Raul - Each one has an aperture of 15 degress, and their orientations are:
            // Raul - -90, -50, -30, -10, +10, +30, +50, +90 degrees respectively.
 
            // Raul - Get all distance measurements
            double[] distances = new double[SonarArrayLength];
            for (int i=0; i < SonarArrayLength; i++)
            {
                distances[i] = SONAR_RANGE;
            }

            // Raul - This code obviously needs to be rewritten:
            foreach (RaycastImpactPoint pt in result.ImpactPoints)
            {
                // LogInfo("Point:" + pt.ReadingIndex + " v:" + pt.Position.W);
                // Rays corresponding to transducer S7
                if (pt.ReadingIndex >= 0 && pt.ReadingIndex <= 15)
                {
                    if (distances[7] > pt.Position.W * 1000f)
                    {
                        // Get closest intersection in S7
                        distances[7] = (float)pt.Position.W * 1000f;
                    }
                }

                // Rays corresponding to transducer S6
                if (pt.ReadingIndex >= 40 && pt.ReadingIndex <= 55)
                {
                    // the distance to the impact has been pre-calculted from the origin 
                    // and it's in the fourth element of the vector
                    if (distances[6] > pt.Position.W * 1000f)
                    {
                        // Get closest intersection in S6
                        distances[6] = (float)pt.Position.W * 1000f;
                    }
                }

                // Rays corresponding to transducer S5
                if (pt.ReadingIndex >= 60 && pt.ReadingIndex <= 75)
                {
                    // the distance to the impact has been pre-calculted from the origin 
                    // and it's in the fourth element of the vector
                    if (distances[5] > pt.Position.W * 1000f)
                    {
                        // Get closest intersection in S5
                        distances[5] = (float)pt.Position.W * 1000f;
                    }
                }

                // Rays corresponding to transducer S4
                if (pt.ReadingIndex >= 80 && pt.ReadingIndex <= 95)
                {
                    // the distance to the impact has been pre-calculted from the origin 
                    // and it's in the fourth element of the vector
                    if (distances[4] > pt.Position.W * 1000f)
                    {
                        // Get closest intersection in S4
                        distances[4] = (float)pt.Position.W * 1000f;
                    }
                }

                // Rays corresponding to transducer S3
                if (pt.ReadingIndex >= 100 && pt.ReadingIndex <= 115)
                {
                    // the distance to the impact has been pre-calculted from the origin 
                    // and it's in the fourth element of the vector
                    if (distances[3] > pt.Position.W * 1000f)
                    {
                        // Get closest intersection in S3
                        distances[3] = (float)pt.Position.W * 1000f;
                    }
                }

                // Rays corresponding to transducer S2
                if (pt.ReadingIndex >= 120 && pt.ReadingIndex <= 135)
                {
                    // the distance to the impact has been pre-calculted from the origin 
                    // and it's in the fourth element of the vector
                    if (distances[2] > pt.Position.W * 1000f)
                    {
                        // Get closest intersection in S2
                        distances[2] = (float)pt.Position.W * 1000f;
                    }
                }

                // Rays corresponding to transducer S1
                if (pt.ReadingIndex >= 140 && pt.ReadingIndex <= 155)
                {
                    // the distance to the impact has been pre-calculted from the origin 
                    // and it's in the fourth element of the vector
                    if (distances[1] > pt.Position.W * 1000f)
                    {
                        // Get closest intersection in S1
                        distances[1] = (float)pt.Position.W * 1000f;
                    }
                }

                // Rays corresponding to transducer S0
                if (pt.ReadingIndex >= 180 && pt.ReadingIndex <= 195)
                {
                    // the distance to the impact has been pre-calculted from the origin 
                    // and it's in the fourth element of the vector
                    if (distances[0] > pt.Position.W * 1000f)
                    {
                        // Get closest intersection in S0
                        distances[0] = (float)pt.Position.W * 1000f;
                    }
                }

            }



            // Raul - Get minimum distance
            for (int i = 0; i < SonarArrayLength; i++ )
            {
                if (minDistance > distances[i])
                {
                    minDistance = distances[i];
                }
            }


            // Raul - Build Sonar State
            pxsonar.SonarState latestResults = new pxsonar.SonarState();


            // Distance between former reading and current reading is calculated
            double distance = 0.0;

            for (int i = 0; i < SonarArrayLength; i++ )
            {
                _state.DistanceMeasurements[i] = distances[i];

                // Calculate distance for each SONAR device
                distance += Math.Abs(_state.DistanceMeasurements[i] - formerDistanceMeasurements[i]);

                // Store former reading for subsequent notifications
                formerDistanceMeasurements[i] = _state.DistanceMeasurements[i];
            }

            // ONLY NOTIFY Significative chanes in sonar readings
            if (distance > SonarChangeThreshold)
            {
                // I am using SonarState.DistanceMeasurement to store the delta 
                // between last and current readings.
                _state.DistanceMeasurement = distance;


                _state.MaxDistance = minDistance; // Select the closest intersection.
                _state.AngularRange = (int)Math.Abs(_entity.RaycastProperties.EndAngle - _entity.RaycastProperties.StartAngle);
                _state.AngularResolution = _entity.RaycastProperties.AngleIncrement;
                _state.TimeStamp = DateTime.Now;

                // send replace message to self            
                pxsonar.Replace replace = new pxsonar.Replace();

                // for perf reasons dont set response port, we are just talking to ourself anyway
                replace.ResponsePort = null;
                replace.Body = _state;
                _mainPort.Post(replace);
            }
        }

        [ServiceHandler(ServiceHandlerBehavior.Concurrent)]
        public IEnumerator<ITask> GetHandler(pxsonar.Get get)
        {
            get.ResponsePort.Post(_state);
            yield break;
        }

        [ServiceHandler(ServiceHandlerBehavior.Concurrent)]
        public IEnumerator<ITask> HttpGetHandler(HttpGet get)
        {
            get.ResponsePort.Post(new HttpResponseType(HttpStatusCode.OK, _state));
            yield break;
        }

        [ServiceHandler(ServiceHandlerBehavior.Exclusive)]
        public IEnumerator<ITask> ReplaceHandler(pxsonar.Replace replace)
        {
            _state = replace.Body;
            if (replace.ResponsePort != null)
                replace.ResponsePort.Post(dssp.DefaultReplaceResponseType.Instance);

            // issue notification
            _subMgrPort.Post(new submgr.Submit(_state, dssp.DsspActions.ReplaceRequest));
            yield break;
        }

        [ServiceHandler(ServiceHandlerBehavior.Exclusive)]
        public IEnumerator<ITask> SubscribeHandler(pxsonar.Subscribe subscribe)
        {
            yield return Arbiter.Choice(
                SubscribeHelper(_subMgrPort, subscribe.Body, subscribe.ResponsePort),
                delegate(SuccessResult success)
                {
                    _subMgrPort.Post(new submgr.Submit(
                        subscribe.Body.Subscriber, dssp.DsspActions.ReplaceRequest, _state, null));
                },
                delegate(Exception ex) { LogError(ex); }
            );
        }

        [ServiceHandler(ServiceHandlerBehavior.Exclusive)]
        public IEnumerator<ITask> ReliableSubscribeHandler(pxsonar.ReliableSubscribe subscribe)
        {
            yield return Arbiter.Choice(
                SubscribeHelper(_subMgrPort, subscribe.Body, subscribe.ResponsePort),
                delegate(SuccessResult success)
                {
                    _subMgrPort.Post(new submgr.Submit(
                        subscribe.Body.Subscriber, dssp.DsspActions.ReplaceRequest, _state, null));
                },
                delegate(Exception ex) { LogError(ex); }
            );
        }
    }
}
