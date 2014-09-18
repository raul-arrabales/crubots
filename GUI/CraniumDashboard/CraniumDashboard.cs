//-----------------------------------------------------------------------
// This file was part of the Microsoft Robotics Studio Code Samples.
// 
// Copyright (C) Microsoft Corporation.  All rights reserved.
// Portions Copyright by Trevor Taylor.
// Portions Copyright by Raul Arrabales.
//
//  $File: CraniumDashboard.cs $ $Revision: 11 $
//
// Modifications by Trevor Taylor
// Faculty of IT
// Queensland University of Technology
//
// CRANIUM Modifications by Raul Arrabales
// University Carlos III of Madrid
// www.Conscious-Robots.com
// (CRANIUM ControlPanel is heavily based on Trevor Taylor's Dashboard service)
//
// Version 1
// Incorporate changes to allow saving of settings so you don't have
// to re-enter connection and logging parameters every time you run
// the program.
//
// Version 2
// Add an "option bag" to hold various option settings that can then
// be saved into the state (config) file.
//
// Version 3
// Add initial x,y location for the window so that it will always
// start up in the same place and not wander around the screen!
// Minor changes to the way the dead zone was handled to make the
// robot speed changes smoother.
// Allow the scale factors to be negative (but not zero!). This
// might seem strange, but it allows the axes to be flipped on the
// "joystick" control for those people who want to invert them.
//
// Version 4
// Updated for the November 2006 CTP
// I just can't give up my Dashboard! I hate having to re-enter
// the connection details every time I use the Simple Dashboard,
// and my version is more compact.
// Only a couple of minor changes were required and no new
// functionality was added except for an About Box.
//
// Version 5 (for V1.0 of MSRS)
// Joystick has changed to GameController which meant that several
// areas of the code had to be updated. Had to change the references
// as well.
// Removed reference to the simulated Lynx arm because it no longer
// exists. (Not sure why it was there in the first place.)
// Minor changes in OnLogSettingHandler().
//
// Version 6 - 24-Jan-2007
// Updated with code supplied by Ben Axelrod to colour-code any
// obstacles directly in front of the robot in the LRF display
//
// Version 7 - May-2007
// Added the ability to display a map for the LRF instead of the
// simulated 3D view. Then I found out that Ben had done this too!
// Added buttons to support DriveDistance and RotateDegrees
// (Actually intended for testing, not serious use)
//
// Version 8 - Jun-2007 (May CTP of V1.5)
// Added WebCam Viewer, which I have wanted to do for months!
// It was a real pain in the neck due to bugs in the simulated
// webcam, but I finally got it working. It even works with a
// real webcam!
//
// Jul-2007:
// Verified with released version of V1.5
//
// Version 9 - Aug-2007 (MSRS V1.5) ControlPanel branch of Dashboard
// Added SONAR reading visualization. 
//
// Version 10 - Sept 2007 (MSRS V.1.5) Cranium Dashboard
//
// Version 11 - Nov 2007 - Raul Arrabales.
// Portions of code rewritten to remove webcam processing and vision services.
// I want to generate a brach of code only for Sonar input, neglecting the robot
// vision capabilities for the time being. 
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.Dss.Core.Attributes;

using W3C.Soap;
using Microsoft.Ccr.Core;
using Microsoft.Ccr.Adapters.WinForms;
using Microsoft.Dss.Core;

using System.Drawing;
using System.Drawing.Imaging;

using Microsoft.Dss.ServiceModel.Dssp;
using Microsoft.Dss.ServiceModel.DsspServiceBase;
using Microsoft.Dss.Services.Serializer;
using Microsoft.Robotics.Simulation.Physics.Proxy;
using Microsoft.Robotics.Simulation.Proxy;
using Microsoft.Robotics.Simulation.Engine;
using Microsoft.Robotics.Simulation;
using engineproxy = Microsoft.Robotics.Simulation.Engine.Proxy;
using Microsoft.Robotics.Simulation.Physics;
using Microsoft.Robotics.PhysicalModel;

using System.IO;
using System.Xml;
using System.Xml.Serialization;


using xnaTypes = Microsoft.Xna.Framework;

using arm = Microsoft.Robotics.Services.ArticulatedArm.Proxy;
using cs = Microsoft.Dss.Services.Constructor;
using drive = Microsoft.Robotics.Services.Drive.Proxy;
using ds = Microsoft.Dss.Services.Directory;
using fs = Microsoft.Dss.Services.FileStore;

using game = Microsoft.Robotics.Services.GameController.Proxy;

// Raul - Using Sonar instead of LRF 
using sicklrf = Microsoft.Robotics.Services.Sensors.SickLRF.Proxy;
using submgr = Microsoft.Dss.Services.SubscriptionManager;
using dssp = Microsoft.Dss.ServiceModel.Dssp;

// using Microsoft.Robotics.PhysicalModel.Proxy;
using System.ComponentModel;


// Raul - Sonar generic contract
using pxsonar = Microsoft.Robotics.Services.Sonar.Proxy;

// Camera Pan Tilt service
using pxPanTilt = Cera.Generic.PanTilt.Proxy;

// Simulated GPS
using pxGPS = Cranium.Simulation.Sensors.SimulatedPioneerGPS.Proxy;

// Bumper Generic Contract
using pxbumper = Microsoft.Robotics.Services.ContactSensor.Proxy;

// Camera generic contract
using cam = Microsoft.Robotics.Services.WebCam.Proxy;




namespace Cranium.Controls
{

    /// <summary>
    /// ControlPanel Service
    /// </summary>
    [DisplayName("Cranium Dashboard")]
    [Description("Service with a WinForm UI for interacting with DSS sensor and actuator services")]
    [Contract(Contract.Identifier)]
    class DashboardService : DsspServiceBase
    {
        // TT - Added for config file
        // Jul-2007 - Need to specify the directory in V1.5
        private const string InitialStateUri = ServicePaths.MountPoint + @"/packages/crubots/config/CraniumDashboard.Config.xml";

        // shared access to state is protected by the interleave pattern
        // when we activate the handlers
        // TT - Added the InitialStatePartner attribute and changed
        // _state to be null
        [InitialStatePartner(Optional = true, ServiceUri = InitialStateUri)]
        StateType _state = null;

        [ServicePort("/craniumdashboard", AllowMultipleInstances = true)]
        DashboardOperations _mainPort = new DashboardOperations();

        // TT Dec-2006 - Updated for V1.0
        [Partner("GameController", Contract = game.Contract.Identifier, CreationPolicy = PartnerCreationPolicy.UseExistingOrCreate)]
        game.GameControllerOperations _gameControllerPort = new game.GameControllerOperations();
        game.GameControllerOperations _gameControllerNotify = new game.GameControllerOperations();
        Port<Shutdown> _gameControllerShutdown = new Port<Shutdown>();

        DriveControl _driveControl;
        DriveControlEvents _eventsPort = new DriveControlEvents();


        // Embedded Simulation window
        SimulationEnginePort _notificationTarget;
        CameraEntity _observer;
        Port<Bitmap> _bitmapPort;
        SimulatorConfiguration _defaultConfig = null;

        #region Startup
        /// <summary>
        /// DashboardService Default DSS Constuctor
        /// </summary>
        /// <param name="pCreate"></param>
        public DashboardService(DsspServiceCreationPort pCreate) : base(pCreate)
        {

        }

        /// <summary>
        /// Entry Point for the Dashboard Service
        /// </summary>
        protected override void Start()
        {
            // TT - Added code to create a default State if no
            // config file exists
            if (_state == null)
            {
                _state = new StateType();
                _state.Log = false;
                _state.LogFile = "";
                _state.Machine = "";
                _state.Port = 0;
            }

            // TT - Version 2 - The options "bag"
            // This is tacky, but we need to set the default values
            // in case there is no existing config.xml file
            if (_state.Options == null)
            {
                _state.Options = new GUIOptions();
                _state.Options.DeadZoneX = 80;
                _state.Options.DeadZoneY = 80;
                _state.Options.TranslateScaleFactor = 1.0;
                _state.Options.RotateScaleFactor = 0.5;
                _state.Options.ShowLRF = false;
                _state.Options.ShowArm = false;
                _state.Options.DisplayMap = false;

                // Raul - Sept 2007
                _state.Options.DisplaySonarMap = false;

                // Updated in later versions with more options
                // These values are in mm
                _state.Options.RobotWidth = 300;
                _state.Options.MaxLRFRange = 8192;
                _state.Options.DriveDistance = 300;
                // Speed is in mm/sec???
                _state.Options.MotionSpeed = 100;
                // Angle is in degrees
                _state.Options.RotateAngle = 45;
                // Camera update interval in milliseconds
                // Note that this is only required for the
                // simulated webcam because it does not provide
                // updates when you subscribe
                _state.Options.CameraInterval = 250;

                // Raul - Version 9
                _state.Options.SonarRange = 4000;
                _state.Options.SonarTransducerAngularRange = 15.0f;

                // Raul - Sonar transducer positions.
                // This is only valid for a single sonar array of 8 transducers!!
                _state.Options.SonarRadians = new double[8];
                // Orientations of the P3DX frontal sonar transducers
                _state.Options.SonarRadians[0] = (Math.PI * 90) / 180;
                _state.Options.SonarRadians[1] = (Math.PI * 50) / 180;
                _state.Options.SonarRadians[2] = (Math.PI * 30) / 180;
                _state.Options.SonarRadians[3] = (Math.PI * 10) / 180;
                _state.Options.SonarRadians[4] = -(Math.PI * 10) / 180;
                _state.Options.SonarRadians[5] = -(Math.PI * 30) / 180;
                _state.Options.SonarRadians[6] = -(Math.PI * 50) / 180;
                _state.Options.SonarRadians[7] = -(Math.PI * 90) / 180;

            }
            if (_state.Options.CameraInterval < 100)
                _state.Options.CameraInterval = 100;

            // Handlers that need write or exclusive access to state go under
            // the exclusive group. Handlers that need read or shared access, and can be
            // concurrent to other readers, go to the concurrent group.
            // Other internal ports can be included in interleave so you can coordinate
            // intermediate computation with top level handlers.
            Activate(Arbiter.Interleave(
                new TeardownReceiverGroup
                (
                    Arbiter.Receive<DsspDefaultDrop>(false, _mainPort, DropHandler)
                ),
                new ExclusiveReceiverGroup
                (
                    Arbiter.ReceiveWithIterator<Replace>(true, _mainPort, ReplaceHandler),
                    Arbiter.ReceiveWithIteratorFromPortSet<OnLoad>(true, _eventsPort, OnLoadHandler),
                    Arbiter.ReceiveFromPortSet<OnClosed>(true, _eventsPort, OnClosedHandler),
                    Arbiter.ReceiveWithIteratorFromPortSet<OnChangeJoystick>(true, _eventsPort, OnChangeJoystickHandler),
                    Arbiter.ReceiveFromPortSet<OnLogSetting>(true, _eventsPort, OnLogSettingHandler),
                    // TT - Added this handler for Connection parameters
                    Arbiter.ReceiveFromPortSet<OnConnectSetting>(true, _eventsPort, OnConnectSettingHandler),
                    // TT - Added this handler for Options
                    Arbiter.ReceiveFromPortSet<OnOptionSettings>(true, _eventsPort, OnOptionSettingsHandler),

                    
                    Arbiter.ReceiveWithIterator<cam.UpdateFrame>(true, _webCamNotify, CameraUpdateFrameHandler),
                    
                    Arbiter.ReceiveWithIteratorFromPortSet<OnConnectPanTilt>(true, _eventsPort, OnConnectPanTiltHandler)
                    

                    // Raul - Sept. Added this handler to disconnect cam - doesn't work yet
                    // Arbiter.ReceiveFromPortSet<OnDisconnectWebCam>(true, _eventsPort, OnDisconnectWebCamHandler),
                    // Raul - Handler for Cera Vision
                    // Arbiter.ReceiveWithIteratorFromPortSet<OnConnectVision>(true, _eventsPort, OnConnectVisionHandler),
                    // Arbiter.ReceiveFromPortSet<OnDisconnectVision>(true, _eventsPort, OnDisconnectVisionHandler)                   
                ),
                new ConcurrentReceiverGroup
                (
                    Arbiter.Receive<DsspDefaultLookup>(true,_mainPort,DefaultLookupHandler),
                    Arbiter.ReceiveWithIterator<Get>(true, _mainPort, GetHandler),
                    
                    // TT Dec-2006 - Updated for V1.0
                    Arbiter.ReceiveWithIterator<game.Replace>(true, _gameControllerNotify, JoystickReplaceHandler),
                    Arbiter.ReceiveWithIterator<sicklrf.Replace>(true, _laserNotify, OnLaserReplaceHandler),
                    

                    Arbiter.ReceiveWithIteratorFromPortSet<OnConnect>(true, _eventsPort, OnConnectHandler),
                    
                    Arbiter.ReceiveWithIterator<drive.Update>(true, _driveNotify, OnDriveUpdateNotificationHandler),
                    Arbiter.ReceiveWithIteratorFromPortSet<OnConnectMotor>(true, _eventsPort, OnConnectMotorHandler),
                    Arbiter.ReceiveWithIteratorFromPortSet<OnMove>(true, _eventsPort, OnMoveHandler),
                    
                    // TT May-2007 - Added Rotate and Translate
                    Arbiter.ReceiveWithIteratorFromPortSet<OnRotate>(true, _eventsPort, OnRotateHandler),
                    Arbiter.ReceiveWithIteratorFromPortSet<OnTranslate>(true, _eventsPort, OnTranslateHandler),
                    Arbiter.ReceiveWithIteratorFromPortSet<OnEStop>(true, _eventsPort, OnEStopHandler),
                    
                    // Raul - LRF
                    Arbiter.ReceiveWithIteratorFromPortSet<OnStartService>(true, _eventsPort, OnStartServiceHandler),
                    Arbiter.ReceiveWithIteratorFromPortSet<OnConnectSickLRF>(true, _eventsPort, OnConnectSickLRFHandler),
                    Arbiter.ReceiveFromPortSet<OnDisconnectSickLRF>(true, _eventsPort, OnDisconnectSickLRFHandler),

                    // Raul - Sonar 
                    Arbiter.ReceiveWithIteratorFromPortSet<OnConnectSonar>(true, _eventsPort, OnConnectSonarHandler),
                    Arbiter.ReceiveFromPortSet<OnDisconnectSonar>(true, _eventsPort, OnDisconnectSonarHandler),
                    Arbiter.ReceiveWithIterator<pxsonar.Replace>(true, _sonarNotify, OnSonarReplaceHandler),

                    // Raul - GPS
                    Arbiter.ReceiveWithIteratorFromPortSet<OnConnectGPS>(true, _eventsPort, OnConnectGPSHandler),
                    Arbiter.ReceiveFromPortSet<OnDisconnectGPS>(true, _eventsPort, OnDisconnectGPSHandler),
                    Arbiter.ReceiveWithIterator<pxGPS.Replace>(true, _gpsNotify, OnGPSReplaceHandler),

                    // Raul - Bumpers
                    Arbiter.ReceiveWithIteratorFromPortSet<OnConnectBumpers>(true, _eventsPort, OnConnectBumpersHandler),
                    Arbiter.ReceiveFromPortSet<OnDisconnectBumpers>(true, _eventsPort, OnDisconnectBumpersHandler),
                    Arbiter.ReceiveWithIterator<pxbumper.Replace>(true, _bumpersNotify, OnBumpersReplaceHandler),
                    Arbiter.ReceiveWithIterator<pxbumper.Update>(true, _bumpersNotify, OnBumperUpdateHandler),

                    Arbiter.ReceiveWithIteratorFromPortSet<OnConnectWebCam>(true, _eventsPort, OnConnectWebCamHandler),
                    Arbiter.ReceiveWithIteratorFromPortSet<OnPTMove>(true, _eventsPort, OnPanTiltMoveHandler)
                )
            ));

            DirectoryInsert();

            _notificationTarget = new SimulationEnginePort();
            EntitySubscribeRequestType req = new EntitySubscribeRequestType();
            req.Name = "PursuitCam";
            SimulationEngine.GlobalInstancePort.Subscribe(req, _notificationTarget);

            // Set the Simulator camera view and resolution
            UpdateCameraView view = new UpdateCameraView(new CameraView());
            view.Body.EyePosition = new Vector3(1, 5, 1);
            view.Body.LookAtPoint = new Vector3(0, 0, 0);
            view.Body.XResolution = 640;
            view.Body.YResolution = 480;
            SimulationEngine.GlobalInstancePort.Post(view);

            // get the current simulator configuration
            _defaultConfig = new SimulatorConfiguration(true);

            Activate(Arbiter.Choice(SimulationEngine.GlobalInstancePort.Query(_defaultConfig),
                delegate(SimulatorConfiguration config)
                {
                    _defaultConfig = config;
                    if (_driveControl != null)
                        WinFormsServicePort.FormInvoke(delegate() { _driveControl.SetHeadless(config.Headless); });
                },
                delegate(W3C.Soap.Fault fault)
                {
                }
            ));

            // Add the winform message handler to the interleave
            Activate(Arbiter.Interleave(
                new TeardownReceiverGroup(),
                new ExclusiveReceiverGroup
                (
                    Arbiter.Receive<InsertSimulationEntity>(false, _notificationTarget, InsertEntityNotificationHandlerFirstTime),
                    Arbiter.ReceiveFromPortSet<OnSimulLoaded>(true, _eventsPort, OnSimulLoadedHandler),
                    Arbiter.ReceiveFromPortSet<OnSimulDrag>(true, _eventsPort, OnSimulDragHandler),
                    Arbiter.ReceiveFromPortSet<OnSimulZoom>(true, _eventsPort, OnSimulZoomHandler)
                ),
                new ConcurrentReceiverGroup()
            ));


            WinFormsServicePort.Post(new RunForm(CreateForm));
        }

        #endregion

        #region WinForms interaction

        System.Windows.Forms.Form CreateForm()
        {
            // TT - Modify this call to pass the state to the Form
            return new DriveControl(_eventsPort, _state);
        }

        void InsertEntityNotificationHandlerFirstTime(InsertSimulationEntity ins)
        {
            _bitmapPort = new Port<Bitmap>();

            InsertEntityNotificationHandler(ins);
            Activate(Arbiter.Interleave(
                    new TeardownReceiverGroup
                    (
                    ),
                    new ExclusiveReceiverGroup(
                        Arbiter.ReceiveFromPortSet<OnSimulLoaded>(true, _eventsPort, OnSimulLoadedHandler),
                        Arbiter.ReceiveFromPortSet<OnSimulDrag>(true, _eventsPort, OnSimulDragHandler),
                        Arbiter.ReceiveFromPortSet<OnSimulZoom>(true, _eventsPort, OnSimulZoomHandler),
                        Arbiter.Receive<InsertSimulationEntity>(true, _notificationTarget, InsertEntityNotificationHandler),
                        Arbiter.Receive<DeleteSimulationEntity>(true, _notificationTarget, DeleteEntityNotificationHandler),
                        Arbiter.Receive<Bitmap>(true, _bitmapPort, UpdateCameraImage)
                    ),
                    new ConcurrentReceiverGroup()
                ));
        }

        void InsertEntityNotificationHandler(InsertSimulationEntity ins)
        {
            _observer = (CameraEntity)ins.Body;
            _observer.Subscribe(_bitmapPort);
        }
        void DeleteEntityNotificationHandler(DeleteSimulationEntity del)
        {
            _observer = null;
        }


        public Bitmap ResizeBitmap(Bitmap b, int nWidth, int nHeight) { Bitmap result = new Bitmap(nWidth, nHeight); using (Graphics g = Graphics.FromImage((Image)result))    g.DrawImage(b, 0, 0, nWidth, nHeight); return result; }

        void UpdateCameraImage(Bitmap bmp)
        {

            if (bmp != null)
            {
                // Resize image
                Bitmap small = ResizeBitmap(bmp, 320, 240);

                WinFormsServicePort.Post(new FormInvoke(delegate() { _driveControl.SetCameraImage(small); }));
            }
        }


        void OnSimulLoadedHandler(OnSimulLoaded msg)
        {
            _driveControl = (DriveControl)msg.Object; 
            WinFormsServicePort.FormInvoke( delegate() {_driveControl.SetHeadless(_defaultConfig.Headless); });
        }


        void OnSimulDragHandler(OnSimulDrag msg)
        {
            if (_observer != null)
            {
                Vector2 drag = (Vector2)msg.Object;
                xnaTypes.Vector3 view = _observer.LookAt - _observer.Location;
                view.Normalize();
                xnaTypes.Vector3 up = new xnaTypes.Vector3(0, 1, 0);
                float dot = xnaTypes.Vector3.Dot(view, up);
                if (Math.Abs(dot) > 0.99)
                {
                    up += new xnaTypes.Vector3(0.1f, 0, 0);
                    up.Normalize();
                }
                xnaTypes.Vector3 right = xnaTypes.Vector3.Cross(view, up);
                view = xnaTypes.Vector3.Multiply(view, 10f);
                view = xnaTypes.Vector3.Transform(view, xnaTypes.Matrix.CreateFromAxisAngle(up, (float)(-drag.X * Math.PI / 500)));
                view = xnaTypes.Vector3.Transform(view, xnaTypes.Matrix.CreateFromAxisAngle(right, (float)(-drag.Y * Math.PI / 500)));

                _observer.LookAt = _observer.Location + view;
            }
        }


        void OnSimulZoomHandler(OnSimulZoom msg)
        {
            if (_observer != null)
            {
                Vector2 drag = (Vector2)msg.Object;
                xnaTypes.Vector3 view = _observer.LookAt - _observer.Location;
                view.Normalize();
                view = xnaTypes.Vector3.Multiply(view, drag.X * 0.1f);
                _observer.LookAt += view;
                _observer.Location += view;
            }
        }
        
        
        


        #endregion

        #region DSS Handlers

        /// <summary>
        /// Get Handler returns Dashboard State.
        /// </summary>
        /// <remarks>
        /// We declare this handler as an iterator so we can easily do
        /// sequential, logically blocking receives, without the need
        /// of nested Activate calls
        /// </remarks>
        /// <param name="get"></param>
        IEnumerator<ITask> GetHandler(Get get)
        {
            get.ResponsePort.Post(_state);
            yield break;
        }

        /// <summary>
        /// Replace Handler sets Dashboard State
        /// </summary>
        /// <param name="replace"></param>
        IEnumerator<ITask> ReplaceHandler(Replace replace)
        {
            _state = replace.Body;
            replace.ResponsePort.Post(dssp.DefaultReplaceResponseType.Instance);
            yield break;
        }

        /// <summary>
        /// Drop Handler shuts down Dashboard
        /// </summary>
        /// <param name="drop"></param>
        void DropHandler(DsspDefaultDrop drop)
        {
            SpawnIterator(drop, DropIterator);
        }

        IEnumerator<ITask> DropIterator(DsspDefaultDrop drop)
        {
            LogInfo("Starting Drop");

            /*
            // Close the WebCam Form
            if (_cameraForm != null)
            {
                WebCamForm cam = _cameraForm;
                _cameraForm = null;

                WinFormsServicePort.FormInvoke(
                    delegate()
                    {
                        if (!cam.IsDisposed)
                        {
                            cam.Dispose();
                        }
                    }
                );
            }
            */

            // Close the Dashboard Form
            if (_driveControl != null)
            {
                DriveControl drive = _driveControl;
                _driveControl = null;

                WinFormsServicePort.FormInvoke(
                    delegate()
                    {
                        if (!drive.IsDisposed)
                        {
                            drive.Dispose();
                        }
                    }
                );
            }


            if (_laserShutdown != null)
            {
                yield return PerformShutdown(ref _laserShutdown);
            }

            if (_motorShutdown != null)
            {
                yield return PerformShutdown(ref _motorShutdown);
            }

            base.DefaultDropHandler(drop);
        }

        Choice PerformShutdown(ref Port<Shutdown> port)
        {
            Shutdown shutdown = new Shutdown();
            port.Post(shutdown);
            port = null;

            return Arbiter.Choice(
                shutdown.ResultPort,
                delegate(SuccessResult success) { },
                delegate(Exception e)
                {
                    LogError(e);
                }
            );
        }

        #endregion

        #region Joystick Operations

        Choice EnumerateJoysticks()
        {
            return Arbiter.Choice(
                _gameControllerPort.GetControllers(new game.GetControllersRequest()),
                delegate(game.GetControllersResponse response)
                {
                    WinFormsServicePort.FormInvoke(
                        delegate()
                        {
                            _driveControl.ReplaceJoystickList(response.Controllers);
                        }
                    );
                },
                delegate(Fault fault)
                {
                    LogError(fault);
                }
            );
        }
        /*
        Choice EnumerateJoysticks()
        {
            return Arbiter.Choice(
                _joystickPort.GetControllers(new joystick.GetControllersRequest()),
                delegate(joystick.GetControllersResponse response)
                {
                    WinFormsServicePort.FormInvoke(
                        delegate()
                        {
                            _driveControl.ReplaceJoystickList(response.Controllers);
                        }
                    );
                },
                delegate(Fault fault)
                {
                    LogError(fault);
                }
            );
        }
        */
        // TT Dec-2006 - Updated for V1.0
        Choice SubscribeToJoystick()
        {
            return Arbiter.Choice(
                _gameControllerPort.Subscribe(_gameControllerNotify),
                delegate(SubscribeResponseType response) { },
                delegate(Fault fault) { LogError(fault); }
            );

        }

        // TT Dec-2006 - Updated for V1.0
        IEnumerator<ITask> JoystickReplaceHandler(game.Replace replace)
        {
            if (_driveControl != null)
            {
                WinFormsServicePort.FormInvoke(
                    delegate()
                    {
                        _driveControl.UpdateJoystickButtons(replace.Body.Buttons);
                        _driveControl.UpdateJoystickAxes(replace.Body.Axes);
                    }
                );
            }

            yield break;
        }

        IEnumerator<ITask> JoystickUpdateAxesHandler(game.UpdateAxes update)
        {
            if (_driveControl != null)
            {
                WinFormsServicePort.FormInvoke(
                    delegate()
                    {
                        _driveControl.UpdateJoystickAxes(update.Body);
                    }
                );
            }
            yield break;
        }

        IEnumerator<ITask> JoystickUpdateButtonsHandler(game.UpdateButtons update)
        {
            if (_driveControl != null)
            {
                WinFormsServicePort.FormInvoke(
                    delegate()
                    {
                        _driveControl.UpdateJoystickButtons(update.Body);
                    }
                );
            }
            yield break;
        }

        IEnumerator<ITask> OnChangeJoystickHandler(OnChangeJoystick onChangeJoystick)
        {
            if (onChangeJoystick.DriveControl == _driveControl)
            {
                // TT Dec-2006 - Copied from V1.0
//                yield return Arbiter.Choice(
//                    _joystickPort.ChangeController(onChangeJoystick.Joystick),
                Activate(Arbiter.Choice(
                    _gameControllerPort.ChangeController(onChangeJoystick.Joystick),
                    delegate(DefaultUpdateResponseType response)
                    {
                        LogInfo("Changed Joystick");
                    },
                    delegate(Fault f)
                    {
                        LogError(null, "Unable to change Joystick", f);
                    })
                );
            }
            // TT Dec-2006 - Copied from V1.0
            yield break;
        }

        #endregion

        #region Drive Control Event Handlers

        IEnumerator<ITask> OnLoadHandler(OnLoad onLoad)
        {
            _driveControl = onLoad.DriveControl;

            LogInfo("Loaded Form");

            yield return EnumerateJoysticks();

            yield return SubscribeToJoystick();
        }


        void OnClosedHandler(OnClosed onClosed)
        {
            if (onClosed.DriveControl == _driveControl)
            {
                LogInfo("Form Closed");

                _mainPort.Post(new DsspDefaultDrop(DropRequestType.Instance));
            }
        }

        // TT - New handler for Connection parameter setting/saving
        void OnConnectSettingHandler(OnConnectSetting onConnectSetting)
        {
            _state.Machine = onConnectSetting.Machine;
            _state.Port = onConnectSetting.Port;
            SaveState(_state);
        }

        // TT - New handler for Option setting/saving
        void OnOptionSettingsHandler(OnOptionSettings Opt)
        {
            _state.Options = Opt.Options;
            SaveState(_state);
        }


        IEnumerator<ITask> OnConnectHandler(OnConnect onConnect)
        {
            if (onConnect.DriveControl == _driveControl)
            {
                UriBuilder builder = new UriBuilder(onConnect.Service);
                builder.Scheme = new Uri(ServiceInfo.Service).Scheme;

                ds.DirectoryPort port = ServiceForwarder<ds.DirectoryPort>(builder.Uri);
                // TT Nov-2006 - Changed for new CTP
                // ds.Get get = new ds.Get(GetRequestType.Instance);
                ds.Get get = new ds.Get();

                port.Post(get);
                ServiceInfoType[] list = null;

                yield return Arbiter.Choice(get.ResponsePort,
                    delegate(ds.GetResponseType response)
                    {
                        list = response.RecordList;
                    },
                    delegate(Fault fault)
                    {
                        list = new ServiceInfoType[0];
                        LogError(fault);
                    }
                );

                WinFormsServicePort.FormInvoke(
                    delegate()
                    {
                        _driveControl.ReplaceDirectoryList(list);
                    }
                );
            }
        }

        IEnumerator<ITask> OnStartServiceHandler(OnStartService onStartService)
        {
            if (onStartService.DriveControl == _driveControl &&
                onStartService.Constructor != null)
            {
                cs.ConstructorPort port = ServiceForwarder<cs.ConstructorPort>(onStartService.Constructor);

                ServiceInfoType request = new ServiceInfoType(onStartService.Contract);
                cs.Create create = new cs.Create(request);

                port.Post(create);

                string service = null;

                yield return Arbiter.Choice(
                    create.ResponsePort,
                    delegate(CreateResponse response)
                    {
                        service = response.Service;
                    },
                    delegate(Fault fault)
                    {
                        LogError(fault);
                    }
                );


                if (service == null)
                {
                    yield break;
                }

                WinFormsServicePort.FormInvoke(
                    delegate()
                    {
                        _driveControl.StartedSickLRF();
                    }
                );
            }
        }

        #endregion

        #region PanTilt operations
        pxPanTilt.GenericPanTiltOperations _pantiltPort = null;

        IEnumerator<ITask> OnConnectPanTiltHandler(OnConnectPanTilt onConnectPanTilt)
        {
            _pantiltPort = ServiceForwarder<pxPanTilt.GenericPanTiltOperations>(onConnectPanTilt.Service);


            // Send a reset
            pxPanTilt.PanTiltOperationRequest resetReq =
                new pxPanTilt.PanTiltOperationRequest();
            resetReq.OperationType = pxPanTilt.PanTiltOperationRequest.OpType.Reset;

            pxPanTilt.PanTiltOperation resetOp = new pxPanTilt.PanTiltOperation(resetReq);


            _pantiltPort.Post(resetOp);

            yield return Arbiter.Choice(
                        resetOp.ResponsePort,
                        delegate(DefaultUpdateResponseType response)
                        {
                            LogInfo("Reset Command Suceeded");
                        },
                        delegate(Fault f)
                        {
                            LogError(f);
                        }
            );
        }

        IEnumerator<ITask> OnPanTiltMoveHandler(OnPTMove ptm)
        {
            LogInfo("OnPanTiltMoveHandler");

            pxPanTilt.PanTiltOperationRequest ptreq =
                new pxPanTilt.PanTiltOperationRequest();
            ptreq.OperationType = ptm.PTCommand;
            pxPanTilt.PanTiltOperation ptop = new pxPanTilt.PanTiltOperation(ptreq);

            LogInfo("Sending request to PanTilt: " + ptm.PTCommand.ToString()); 

            _pantiltPort.Post(ptop);

            yield return Arbiter.Choice(
                        ptop.ResponsePort,
                        delegate(DefaultUpdateResponseType response)                         
                        {
                            LogInfo("PT Command Succeeded");
                        },
                        delegate(Fault f)
                        {
                             LogError(f);
                        }
            );

        }

        #endregion 

        #region Motor operations

        drive.DriveOperations _drivePort = null;
        drive.DriveOperations _driveNotify = new drive.DriveOperations();
        Port<Shutdown> _motorShutdown = null;

        IEnumerator<ITask> OnConnectMotorHandler(OnConnectMotor onConnectMotor)
        {
            if (onConnectMotor.DriveControl == _driveControl)
            {
                drive.EnableDriveRequest request = new drive.EnableDriveRequest();

                if (_drivePort != null)
                {
                    yield return Arbiter.Choice(
                        _drivePort.EnableDrive(request),
                        delegate(DefaultUpdateResponseType response)                         
                        {
                            
                        },
                        delegate(Fault f)
                        {
                            LogError(f);
                        }
                    );

                    if (_motorShutdown != null)
                    {
                        yield return PerformShutdown(ref _motorShutdown);
                    }
                }

                _drivePort = ServiceForwarder<drive.DriveOperations>(onConnectMotor.Service);
                _motorShutdown = new Port<Shutdown>();

                request.Enable = true;

                yield return Arbiter.Choice(
                    _drivePort.EnableDrive(request),
                    delegate(DefaultUpdateResponseType response) { },
                    delegate(Fault f)
                    {
                        LogError(f);
                    }
                );

                drive.ReliableSubscribe subscribe = new drive.ReliableSubscribe(
                    new ReliableSubscribeRequestType(10)
                );
                subscribe.NotificationPort = _driveNotify;
                subscribe.NotificationShutdownPort = _motorShutdown;

                _drivePort.Post(subscribe);

                yield return Arbiter.Choice(
                    subscribe.ResponsePort,
                    delegate(SubscribeResponseType response)
                    {
                        LogInfo("Subscribed to " + onConnectMotor.Service);
                    },
                    delegate(Fault fault)
                    {
                        _motorShutdown = null;
                        LogError(fault);
                    }
                );
            }
        }

        IEnumerator<ITask> OnDriveUpdateNotificationHandler(drive.Update notification)
        {
            if (_driveControl != null)
            {
                WinFormsServicePort.FormInvoke(
                    delegate()
                    {
                        _driveControl.UpdateMotorData(notification.Body);
                    }
                );
            }

            LogObject(notification.Body);
            yield break;
        }

        // This scale factor is applied to power settings.
        // Perhaps it it so convert from mm to m/sec.
        // However, the main point is that actual power values are
        // quite small.
        double MOTOR_POWER_SCALE_FACTOR = 0.001;

        IEnumerator<ITask> OnMoveHandler(OnMove onMove)
        {
            if (onMove.DriveControl == _driveControl && _drivePort != null)
            {
                drive.SetDrivePowerRequest request = new drive.SetDrivePowerRequest();
                request.LeftWheelPower = (double)onMove.Left * MOTOR_POWER_SCALE_FACTOR;
                request.RightWheelPower = (double)onMove.Right * MOTOR_POWER_SCALE_FACTOR;

                yield return Arbiter.Choice(
                    _drivePort.SetDrivePower(request),
                    delegate(DefaultUpdateResponseType response) { },
                    delegate(Fault f)
                    {
                        LogError(f);
                    }
                );
            }
        }

        IEnumerator<ITask> OnRotateHandler(OnRotate onRotate)
        {
            if (onRotate.DriveControl == _driveControl && _drivePort != null)
            {
                drive.RotateDegreesRequest request = new drive.RotateDegreesRequest();
                request.Degrees = onRotate.Angle;
                request.Power = (double)onRotate.Power * MOTOR_POWER_SCALE_FACTOR;

                yield return Arbiter.Choice(
                    _drivePort.RotateDegrees(request),
                    delegate(DefaultUpdateResponseType response) { },
                    delegate(Fault f)
                    {
                        LogError(f);
                    }
                );
            }
        }

        IEnumerator<ITask> OnTranslateHandler(OnTranslate onTranslate)
        {
            if (onTranslate.DriveControl == _driveControl && _drivePort != null)
            {
                drive.DriveDistanceRequest request = new drive.DriveDistanceRequest();
                request.Distance = onTranslate.Distance;
                request.Power = (double)onTranslate.Power * MOTOR_POWER_SCALE_FACTOR;

                yield return Arbiter.Choice(
                    _drivePort.DriveDistance(request),
                    delegate(DefaultUpdateResponseType response) { },
                    delegate(Fault f)
                    {
                        LogError(f);
                    }
                );
            }
        }

        IEnumerator<ITask> OnEStopHandler(OnEStop onEStop)
        {
            if (onEStop.DriveControl == _driveControl && _drivePort != null)
            {
                LogInfo("Requesting EStop");
                drive.AllStopRequest request = new drive.AllStopRequest();

                yield return Arbiter.Choice(
                    _drivePort.AllStop(request),
                    delegate(DefaultUpdateResponseType response) { },
                    delegate(Fault f)
                    {
                        LogError(f);
                    }
                );
            }
        }

        #endregion


        #region Laser Range Finder operations
 
        sicklrf.SickLRFOperations _laserPort;
        sicklrf.SickLRFOperations _laserNotify = new sicklrf.SickLRFOperations();
        Port<Shutdown> _laserShutdown = null;

        IEnumerator<ITask> OnConnectSickLRFHandler(OnConnectSickLRF onConnectSickLRF)
        {
            if (onConnectSickLRF.DriveControl != _driveControl)
                yield break;
            _laserPort = ServiceForwarder<sicklrf.SickLRFOperations>(onConnectSickLRF.Service);
            _laserShutdown = new Port<Shutdown>();

            sicklrf.ReliableSubscribe subscribe = new sicklrf.ReliableSubscribe(
                new ReliableSubscribeRequestType(5)
            );
            subscribe.NotificationPort = _laserNotify;
            subscribe.NotificationShutdownPort = _laserShutdown;

            _laserPort.Post(subscribe);

            yield return Arbiter.Choice(
                subscribe.ResponsePort,
                delegate(SubscribeResponseType response)
                {
                    LogInfo("Subscribed to " + onConnectSickLRF.Service);
                },
                delegate(Fault fault)
                {
                    _laserShutdown = null;
                    LogError(fault);
                }
            );
        }

        IEnumerator<ITask> OnLaserReplaceHandler(sicklrf.Replace replace)
        {
            if (_driveControl != null)
            {
                WinFormsServicePort.FormInvoke(
                    delegate()
                    {
                        _driveControl.ReplaceLaserData(replace.Body);
                    }
                );
            }

            LogObject(replace.Body);
            yield break;
        }

        void OnDisconnectSickLRFHandler(OnDisconnectSickLRF onDisconnectSickLRF)
        {
            PerformShutdown(ref _laserShutdown);
        }
     
        #endregion
         
        #region Raul - Sept 2007 - Cera Vision Operations

        /*
        Port<Shutdown> _visionShutdown = null;        

        IEnumerator<ITask> OnConnectVisionHandler(OnConnectVision onConnectVision)
        {
            if (onConnectVision.DriveControl != _driveControl)
                yield break;
            _visionPort = ServiceForwarder<vision.CeraVisionOperations>(onConnectVision.Service);
            _visionShutdown = new Port<Shutdown>();

            vision.Subscribe subscribe = new vision.Subscribe();

            subscribe.NotificationPort = _visionNotify;
            subscribe.NotificationShutdownPort = _visionShutdown;

            _visionPort.Post(subscribe);

            yield return Arbiter.Choice(
                subscribe.ResponsePort,
                delegate(SubscribeResponseType response)
                {
                    LogInfo("Subscribed to " + onConnectVision.Service);
                },
                delegate(Fault fault)
                {
                    _visionShutdown = null;
                    LogError(fault);
                }
            );
        }


        void OnDisconnectVisionHandler(OnDisconnectVision onDisconnectVision)
        {
            PerformShutdown(ref _visionShutdown);
        }


        IEnumerator<ITask> OnVisionMotionNotificationHandler(vision.NotifyMotionImage notification)
        {
            if (_driveControl != null)
            {
                WinFormsServicePort.FormInvoke(
                    delegate()
                    {
                        vision.ImageResult motionBmp = notification.Body;
                        _driveControl.DrawMotionImage( motionBmp.BitmapImage );                        
                    }
                );
            }
            yield break;
        }


        IEnumerator<ITask> OnVisionFaceNotificationHandler(vision.NotifyFaceDetection notification)
        {
            if (_driveControl != null)
            {
                WinFormsServicePort.FormInvoke(
                    delegate()
                    {
                        vision.FaceResult face = notification.Body;                        
                        _driveControl.DrawFace( face );
                    }
                );
            }
            yield break;
        }

        IEnumerator<ITask> OnVisionHandNotificationHandler(vision.NotifyHandGestureDetection notification)
        {
            if (_driveControl != null)
            {
                WinFormsServicePort.FormInvoke(
                    delegate()
                    {
                        vision.HandGestureResult hand = notification.Body;
                        _driveControl.DrawHand(hand);
                    }
                );
            }
            yield break;
        }


        IEnumerator<ITask> OnVisionObjectNotificationHandler(vision.NotifyObjectDetection notification)
        {
            if (_driveControl != null)
            {
                WinFormsServicePort.FormInvoke(
                    delegate()
                    {
                        vision.ObjectResult vobject = notification.Body;
                        _driveControl.DrawObject( vobject );
                    }
                );
            }
            yield break;
        }

        */ 

        #endregion


        // Raul - Sonar Operations
        #region SONAR operations

        pxsonar.SonarOperations _sonarPort;
        pxsonar.SonarOperations _sonarNotify = new pxsonar.SonarOperations();
        Port<Shutdown> _sonarShutdown = null;


        IEnumerator<ITask> OnConnectSonarHandler(OnConnectSonar onConnectSonar)
        {
            if (onConnectSonar.DriveControl != _driveControl)
                yield break;
            _sonarPort = ServiceForwarder<pxsonar.SonarOperations>(onConnectSonar.Service);
            _sonarShutdown = new Port<Shutdown>();

            pxsonar.ReliableSubscribe subscribe = new pxsonar.ReliableSubscribe(
                new ReliableSubscribeRequestType(5)
            );

            subscribe.NotificationPort = _sonarNotify;
            subscribe.NotificationShutdownPort = _sonarShutdown;

            _sonarPort.Post(subscribe);

            yield return Arbiter.Choice(
                subscribe.ResponsePort,
                delegate(SubscribeResponseType response)
                {
                    LogInfo("Subscribed to " + onConnectSonar.Service);
                },
                delegate(Fault fault)
                {
                    _sonarShutdown = null;
                    LogError(fault);
                }
            );
        }


        IEnumerator<ITask> OnSonarReplaceHandler(pxsonar.Replace replace)
        {
            if (_driveControl != null)
            {
                WinFormsServicePort.FormInvoke(
                    delegate()
                    {
                        // LogInfo("Llamando a ReplaceSonarData");
                        _driveControl.ReplaceSonarData(replace.Body);
                    }
                );
            }

            // LogObject(replace.Body);
            yield break;
        }

        void OnDisconnectSonarHandler(OnDisconnectSonar onDisconnectSonar)
        {
            PerformShutdown(ref _sonarShutdown);
        }

        #endregion


        // Raul - GPS Operations
        #region GPS Operations

        pxGPS.SimulatedPioneerGPSOperations _gpsPort;
        pxGPS.SimulatedPioneerGPSOperations _gpsNotify = new pxGPS.SimulatedPioneerGPSOperations();
        Port<Shutdown> _gpsShutdown = null;

        /// <summary>
        /// GPS Connection Handler
        /// </summary>
        /// <param name="onConnectGPS"></param>
        /// <returns></returns>
        IEnumerator<ITask> OnConnectGPSHandler(OnConnectGPS onConnectGPS)
        {
            LogInfo("On Connect GPS");
            if (onConnectGPS.DriveControl != _driveControl)
                yield break;

            _gpsPort = ServiceForwarder<pxGPS.SimulatedPioneerGPSOperations>(onConnectGPS.Service);
            _gpsShutdown = new Port<Shutdown>();

            pxGPS.Subscribe subscribeOp = new pxGPS.Subscribe();

            subscribeOp.NotificationPort = _gpsNotify;
            subscribeOp.NotificationShutdownPort = _gpsShutdown;

            _gpsPort.Post(subscribeOp);

            yield return Arbiter.Choice(
                subscribeOp.ResponsePort,
                delegate(SubscribeResponseType response)
                {
                    LogInfo("Subscribed to " + onConnectGPS.Service);
                },
                delegate(Fault fault)
                {
                    _gpsShutdown = null;
                    LogError(fault);
                }
            );
        }

        /// <summary>
        /// GPS Replace Notification Handler
        /// </summary>
        /// <param name="replace"></param>
        /// <returns></returns>
        IEnumerator<ITask> OnGPSReplaceHandler(pxGPS.Replace replace)
        {
            if (_driveControl != null)
            {
                WinFormsServicePort.FormInvoke(
                    delegate()
                    {                        
                        _driveControl.ReplaceGPSData(replace.Body);
                    }
                );
            }            
            yield break;
        }

        /// <summary>
        /// Disconnect GPS Handler
        /// </summary>
        /// <param name="onDisconnectGPS"></param>
        void OnDisconnectGPSHandler(OnDisconnectGPS onDisconnectGPS)
        {
            PerformShutdown(ref _gpsShutdown);
        }


        #endregion

        // Raul - Bumpers Operations
        #region BUMPERS operations

        // Bumper Ports
        pxbumper.ContactSensorArrayOperations _bumpersPort;
        pxbumper.ContactSensorArrayOperations _bumpersNotify = new pxbumper.ContactSensorArrayOperations();
        Port<Shutdown> _bumpersShutdown = null;

        /// <summary>
        /// Handles Bumper service connection request
        /// </summary>
        /// <param name="onConnectBumpers">Bumper connection request</param>
        /// <returns></returns>
        IEnumerator<ITask> OnConnectBumpersHandler(OnConnectBumpers onConnectBumpers)
        {
            if (onConnectBumpers.DriveControl != _driveControl)
                yield break;
            _bumpersPort = ServiceForwarder<pxbumper.ContactSensorArrayOperations>(onConnectBumpers.Service);
            _bumpersShutdown = new Port<Shutdown>();

            pxbumper.ReliableSubscribe subscribe = new pxbumper.ReliableSubscribe(
                new ReliableSubscribeRequestType(5)
            );

            subscribe.NotificationPort = _bumpersNotify;
            subscribe.NotificationShutdownPort = _bumpersShutdown;

            _bumpersPort.Post(subscribe);

            yield return Arbiter.Choice(
                subscribe.ResponsePort,
                delegate(SubscribeResponseType response)
                {
                    LogInfo("Subscribed to " + onConnectBumpers.Service);
                },
                delegate(Fault fault)
                {
                    _bumpersShutdown = null;
                    LogError(fault);
                }
            );
        }

        /// <summary>
        /// Handlers Bumpers Replace notification
        /// </summary>
        /// <param name="replace">Bumpers replace notification</param>
        /// <returns></returns>
        IEnumerator<ITask> OnBumpersReplaceHandler(pxbumper.Replace replace)
        {
            if (_driveControl != null)
            {
                WinFormsServicePort.FormInvoke(
                    delegate()
                    {
                        _driveControl.ReplaceBumpersData(replace.Body);
                    }
                );
            }

            LogObject(replace.Body);
            yield break;
        }

        /// <summary>
        /// Handles bumper update notification
        /// </summary>
        /// <param name="update">bumper update notification</param>
        /// <returns></returns>
        IEnumerator<ITask> OnBumperUpdateHandler(pxbumper.Update update)
        {
            if (_driveControl != null)
            {
                WinFormsServicePort.FormInvoke(
                    delegate()
                    {
                        _driveControl.UpdateBumperData(update.Body);
                    }
                );
            }
            yield break;
        }

        /// <summary>
        /// Handles Bumper disconnect
        /// </summary>
        /// <param name="onDisconnectBumper"></param>
        void OnDisconnectBumpersHandler(OnDisconnectBumpers onDisconnectBumpers)
        {
            PerformShutdown(ref _bumpersShutdown);
        }

        #endregion




        #region Logging operations

        fs.FileStorePort _fileStorePort = null;
        object _fspLock = new object();

        void OnLogSettingHandler(OnLogSetting onLogSetting)
        {
            _state.Log = onLogSetting.Log;
            _state.LogFile = onLogSetting.File;
            // TT - Save the state on changes to Log settings
            SaveState(_state);

            if (_state.Log)
            {
                try
                {
                    Uri file = new Uri(_state.LogFile);

                    // TT Dec-2006 - Not required in V1.0
                    //fs.FileStoreConstructorPort fsConstructor = (fs.FileStoreConstructorPort)
                    //    Environment.InternalServicePortTable[Contracts.FileStoreConstructor];


                    fs.FileStoreCreate fsCreate = new fs.FileStoreCreate(file, new fs.FileStorePort());
                    // TT Dec-2006 - Update for V1.0
                    // fsConstructor.Post(fsCreate);
                    FileStoreConstructorPort.Post(fsCreate);
                    Activate(
                        Arbiter.Choice(
                            fsCreate.ResultPort,
                            delegate(fs.FileStorePort fsp)
                            {
                                LogInfo("Started Logging");
                                lock (_fspLock)
                                {
                                    _fileStorePort = fsp;
                                }
                            },
                            delegate(Exception ex)
                            {
                                WinFormsServicePort.FormInvoke(delegate()
                                    {
                                        _driveControl.ErrorLogging(ex);
                                    }
                                );

                            }
                        )
                    );
                }
                catch (Exception e)
                {
                    WinFormsServicePort.FormInvoke(delegate()
                        {
                            _driveControl.ErrorLogging(e);
                        }
                    );
                }
            }
            else if (_fileStorePort != null)
            {
                LogInfo("Stop Logging");
                lock(_fspLock)
                {
                    fs.FileStorePort fsp = _fileStorePort;

                    LogInfo("Flush Log");
                    fsp.Post(new fs.Flush());

                    Activate(
                        Arbiter.Receive(false, TimeoutPort(1000),
                            delegate(DateTime signal)
                            {
                                LogInfo("Stop Log");
                                fsp.Post(new Shutdown());
                            }
                        )
                    );
                    
                    _fileStorePort = null;
                }
            }
        }

        void LogObject(object data)
        {
            lock (_fspLock)
            {
                if (_state.Log &&
                    _fileStorePort != null)
                {
                    _fileStorePort.Post(new fs.WriteObject(data));
                }
            }
        }

        #endregion

        // Webcam sensor management
        #region Camera
        
        cam.WebCamOperations _webCamPort;
        cam.WebCamOperations _webCamNotify = new cam.WebCamOperations();

        IEnumerator<ITask> OnConnectWebCamHandler(OnConnectWebCam Opt)
        {
            Fault fault = null;
            SubscribeResponseType s;
            String camera = Opt.Service;

            _webCamPort = ServiceForwarder<cam.WebCamOperations>(camera);


            //cam.Subscribe subscribe = new cam.Subscribe();
            //subscribe.NotificationPort = _webCamNotify;


            yield return Arbiter.Choice(
                _webCamPort.Subscribe(_webCamNotify),
                //                subscribe.ResponsePort,
                delegate(SubscribeResponseType success)
                { s = success; },
                delegate(Fault f)
                {
                    fault = f;
                }
            );

            if (fault != null)
            {
                LogError(null, "Failed to subscribe to webcam", fault);
                yield break;
            }
        }       


        // Handler for new frames from the camera
        IEnumerator<ITask> CameraUpdateFrameHandler(cam.UpdateFrame update)
        {
            cam.QueryFrameResponse frame = null;
            Fault fault = null;

            //cam.QueryFrame query = new cam.QueryFrame();
            //_webCamPort.Post(query);

            yield return Arbiter.Choice(
                _webCamPort.QueryFrame(new cam.QueryFrameRequest()),
//                query.ResponsePort,
                delegate(cam.QueryFrameResponse success)
                {
                    frame = success;
                },
                delegate(Fault f)
                {
                    fault = f;
                }
            );

            if (fault != null)
            {
                LogError(null, "Failed to get frame from camera", fault);
                yield break;
            }

            Bitmap bmp = MakeBitmap(frame.Size.Width, frame.Size.Height, frame.Frame);
            
            DisplayImage(bmp);

            yield break;
        }


        Bitmap MakeBitmap(int width, int height, byte[] imageData)
        {
            // NOTE: This code implicitly assumes that the width is a multiple
            // of four bytes because Bitmaps have to be longword aligned.
            // We really should look at bmp.Stride to see if there is any padding.
            // However, the width and height come from the webcam and most cameras
            // have resolutions that are multiples of four.

            Bitmap bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);

            BitmapData data = bmp.LockBits(
                new Rectangle(0, 0, bmp.Width, bmp.Height),
                ImageLockMode.WriteOnly,
                PixelFormat.Format24bppRgb
            );

            System.Runtime.InteropServices.Marshal.Copy(imageData, 0, data.Scan0, imageData.Length);

            bmp.UnlockBits(data);

            return bmp;
        }


        // Display an image in the WebCam Form
        bool DisplayImage(Bitmap bmp)
        {
            Fault fault = null;
            
            FormInvoke setImage = new FormInvoke(
                delegate()
                {
                    _driveControl.CameraImage = bmp;
                }
            );

            WinFormsServicePort.Post(setImage);

            Arbiter.Choice(
                setImage.ResultPort,
                delegate(EmptyValue success) { },
                delegate(Exception e)
                {
                    fault = Fault.FromException(e);
                }
            );

            if (fault != null)
            {
                LogError(null, "Unable to set camera image on form", fault);
                return false;
            }
            else
            {
                // LogInfo("New camera frame");
                return true;
            }
        }
        

        #endregion

    }

}

