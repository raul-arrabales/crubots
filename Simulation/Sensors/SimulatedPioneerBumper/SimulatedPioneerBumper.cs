//------------------------------------------------------------------------------
//
// CRANIUM Simulated Pioneer 3DX Bumper SERVICE
//
// CRANIUM
//
// Copyright (c) 2007 Raul Arrabales (raul@conscious-robots.com)
//
//  $File: SimulatedPioneerBumper.cs $ $Revision: beta1 $
//
//------------------------------------------------------------------------------

using Microsoft.Ccr.Core;
using Microsoft.Dss.Core;
using Microsoft.Dss.Core.Attributes;
using Microsoft.Dss.ServiceModel.Dssp;
using Microsoft.Dss.Core.DsspHttp;
using Microsoft.Dss.ServiceModel.DsspServiceBase;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using System.Net;

using dssp = Microsoft.Dss.ServiceModel.Dssp;

// Contact Sensor Generic contract proxy
using pxContactSensor = Microsoft.Robotics.Services.ContactSensor.Proxy;

// Simulation Engine
using simengine = Microsoft.Robotics.Simulation.Engine;

// Simulation Physics
using physics = Microsoft.Robotics.Simulation.Physics;

// Subscription Manager
using submgr = Microsoft.Dss.Services.SubscriptionManager;


namespace ConsciousRobots.Cranium.Simulation.Sensors.Bumper
{
    
    /// <summary>
    /// Implementation class for SimulatedPioneerBumper
    /// </summary>
    [DisplayName("SimulatedPioneerBumper")]
    [Description("The SimulatedPioneerBumper Service provides access to a simulated contact sensor array used as P3DX bumpers.\n(Uses the Generic Contact Sensor contract.")]
    [AlternateContract(pxContactSensor.Contract.Identifier)]
    [Contract(Contract.Identifier)]
    public class SimulatedPioneerBumperService : DsspServiceBase
    {
        // Simulated bumper array entity
        private PioneerBumperEntity _entity;

        // Simulation engine port
        private simengine.SimulationEnginePort _notificationTarget;

        // Service State
        private pxContactSensor.ContactSensorArrayState _state = new pxContactSensor.ContactSensorArrayState();

        // Entity Contact Notifications port
        private Port<physics.EntityContactNotification> _contactNotificationPort = new Port<physics.EntityContactNotification>();

        // Shapes to sensor table
        private Dictionary<physics.Shape, pxContactSensor.ContactSensor> _bumperShapeToSensorTable;


        // Main service port
        [ServicePort("/simulatedpioneerbumper", AllowMultipleInstances = true)]
        private pxContactSensor.ContactSensorArrayOperations _mainPort = new pxContactSensor.ContactSensorArrayOperations();


        // Subscription Manager partner
        [Partner("SubMgr", Contract = submgr.Contract.Identifier, CreationPolicy = PartnerCreationPolicy.CreateAlways)]
        submgr.SubscriptionManagerPort _subMgrPort = new submgr.SubscriptionManagerPort();


        /// <summary>
        /// Default Service Constructor
        /// </summary>
        public SimulatedPioneerBumperService(DsspServiceCreationPort creationPort) : 
                base(creationPort)
        {
        }


        /// <summary>
        /// Service Start
        /// </summary>
        protected override void Start()
        {			
			// Get the notification target
            _notificationTarget = new simengine.SimulationEnginePort();

            // PartnerType.Service is the entity instance name
            simengine.SimulationEngine.GlobalInstancePort.Subscribe(
                ServiceInfo.PartnerList,
                _notificationTarget);

            // dont start listening to DSSP operations, other than drop, until notification of entity
            Activate(new Interleave(
                new TeardownReceiverGroup
                (
                    Arbiter.Receive<simengine.InsertSimulationEntity>(false, _notificationTarget, InsertEntityNotificationHandlerFirstTime),
                    Arbiter.Receive<dssp.DsspDefaultDrop>(false, _mainPort, DefaultDropHandler)
                ),
                new ExclusiveReceiverGroup(),
                new ConcurrentReceiverGroup()
            ));


        }


        private void CreateDefaultState()
        {
            // default state has no contact sensors
            _state.Sensors = new List<pxContactSensor.ContactSensor>();
            _bumperShapeToSensorTable = new Dictionary<physics.Shape, pxContactSensor.ContactSensor>();
        }

        void DeleteEntityNotificationHandler(simengine.DeleteSimulationEntity del)
        {
            _entity = null;
        }

        void InsertEntityNotificationHandlerFirstTime(simengine.InsertSimulationEntity ins)
        {
            InsertEntityNotificationHandler(ins);
            base.Start();
            MainPortInterleave.CombineWith(
                new Interleave(
                    new TeardownReceiverGroup(),
                    new ExclusiveReceiverGroup(
                        Arbiter.Receive<simengine.InsertSimulationEntity>(true, _notificationTarget, InsertEntityNotificationHandler),
                        Arbiter.Receive<simengine.DeleteSimulationEntity>(true, _notificationTarget, DeleteEntityNotificationHandler)
                    ),
                    new ConcurrentReceiverGroup()
                )
            );
        }

        void InsertEntityNotificationHandler(simengine.InsertSimulationEntity ins)
        {
            // _entity = (simengine.BumperArrayEntity)ins.Body;
            _entity = (PioneerBumperEntity)ins.Body;
            _entity.ServiceContract = Contract.Identifier;

            // reinitialize the state
            CreateDefaultState();

            pxContactSensor.ContactSensor cs = null;

            // The simulation bumper service uses a simple heuristic to assign contact sensors, to physics shapes:
            // Half the sensors go infront, the other half in the rear. Assume front sensors are first in the list
            // In the for loop below we create a lookup table that matches simulation shapes with sensors. When
            // the physics engine notifies us with a contact, it provides the shape that came into contact. We need to
            // translate that to sensor. In the future we might add an object Context field to shapes to make this easier
            for (int i = 0; i < _entity.Shapes.Length; i++)
            {
                cs = new pxContactSensor.ContactSensor();
                cs.Name = _entity.Shapes[i].State.Name;
                cs.HardwareIdentifier = i;
                _state.Sensors.Add(cs);
                _bumperShapeToSensorTable.Add((physics.BoxShape)_entity.Shapes[i], cs);
            }

            // subscribe to bumper simulation entity for contact notifications
            _entity.Subscribe(_contactNotificationPort);
            // Activate a handler on the notification port, it will run when contacts occur in simulation
            Activate(Arbiter.Receive(false, _contactNotificationPort, PhysicsContactNotificationHandler));
        }

        void PhysicsContactNotificationHandler(physics.EntityContactNotification contact)
        {
            foreach (physics.ShapeContact sc in contact.ShapeContacts)
            {
                // look up shape involved in collision and check its one of the bumper shapes
                pxContactSensor.ContactSensor s;
                if (!_bumperShapeToSensorTable.TryGetValue(sc.LocalShape, out s))
                    continue;
                if (contact.Stage == physics.ContactNotificationStage.Started)
                    s.Pressed = true;
                else if (contact.Stage == physics.ContactNotificationStage.Finished)
                    s.Pressed = false;
                s.TimeStamp = DateTime.Now;
                // notification for individual sensor
                _subMgrPort.Post(new submgr.Submit(s, dssp.DsspActions.UpdateRequest));
            }

            // send notification
            _subMgrPort.Post(new submgr.Submit(_state, dssp.DsspActions.ReplaceRequest));

            // reactivate notification handler
            Activate(Arbiter.Receive(false, _contactNotificationPort, PhysicsContactNotificationHandler));
        }


        /// <summary>
        /// Get Handler
        /// </summary>
        /// <param name="get"></param>
        /// <returns></returns>
        [ServiceHandler(ServiceHandlerBehavior.Concurrent)]
        public IEnumerator<ITask> GetHandler(pxContactSensor.Get get)
        {
            get.ResponsePort.Post(_state);
            yield break;
        }


        [ServiceHandler(ServiceHandlerBehavior.Concurrent)]
        public IEnumerator<ITask> GetHandler(HttpGet get)
        {
            get.ResponsePort.Post(new HttpResponseType(HttpStatusCode.OK, _state));
            yield break;
        }

        [ServiceHandler(ServiceHandlerBehavior.Exclusive)]
        public IEnumerator<ITask> ReplaceHandler(pxContactSensor.Replace replace)
        {
            _state = replace.Body;
            replace.ResponsePort.Post(dssp.DefaultReplaceResponseType.Instance);
            _subMgrPort.Post(new submgr.Submit(_state, dssp.DsspActions.ReplaceRequest));
            yield break;
        }

        [ServiceHandler(ServiceHandlerBehavior.Exclusive)]
        public IEnumerator<ITask> SubscribeHandler(pxContactSensor.Subscribe subscribe)
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
        public IEnumerator<ITask> ReliableSubscribeHandler(pxContactSensor.ReliableSubscribe subscribe)
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
