//------------------------------------------------------------------------------
//
// CRANIUM Simulated Pioneer 3DX GPS SERVICE
//
// CRANIUM
//
// 2010 Raul Arrabales (raul@conscious-robots.com)
// Based on Microsoft's Simulated GPS service
//
//
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Ccr.Core;
using Microsoft.Dss.Core;
using Microsoft.Dss.Core.Attributes;
using Microsoft.Dss.ServiceModel.Dssp;
using Microsoft.Dss.ServiceModel.DsspServiceBase;
using W3C.Soap;
using submgr = Microsoft.Dss.Services.SubscriptionManager;
using simengine = Microsoft.Robotics.Simulation.Engine;
using dssp = Microsoft.Dss.ServiceModel.Dssp;

namespace Cranium.Simulation.Sensors.SimulatedPioneerGPS
{
    [Contract(Contract.Identifier)]
    [DisplayName("SimulatedPioneerGPS")]
    [Description("SimulatedPioneerGPS service (Basic X,Y,Z coordinates GPS sensor for the simulator)")]
    class SimulatedPioneerGPSService : DsspServiceBase
    {
        // Simulation variables
        simengine.VisualEntity _entity;
        simengine.SimulationEnginePort _notificationTarget;

        [ServiceState]
        SimulatedPioneerGPSState _state = new SimulatedPioneerGPSState();

        [ServicePort("/SimulatedPioneerGPS", AllowMultipleInstances = true)]
        SimulatedPioneerGPSOperations _mainPort = new SimulatedPioneerGPSOperations();

        [SubscriptionManagerPartner]
        submgr.SubscriptionManagerPort _submgrPort = new submgr.SubscriptionManagerPort();

        public SimulatedPioneerGPSService(DsspServiceCreationPort creationPort)
            : base(creationPort)
        {
        }

        protected override void Start()
        {

            _notificationTarget = new simengine.SimulationEnginePort();
            simengine.SimulationEngine.GlobalInstancePort.Subscribe(ServiceInfo.PartnerList, _notificationTarget);

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

            // start notification method
            SpawnIterator<DateTime>(DateTime.Now, CheckForStateChange);
        }


        /// <summary>
        /// Check the hardware value 5 times per second and send a notification if it has changed.
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        private IEnumerator<ITask> CheckForStateChange(DateTime timeout)
        {
            while (true)
            {
                if (_entity != null)
                {
                    try
                    {
                        // Get parent entity position 
                        _state.X = _entity.Parent.State.Pose.Position.X;
                        _state.Y = _entity.Parent.State.Pose.Position.Y;
                        _state.Z = _entity.Parent.State.Pose.Position.Z;
                        _state.Theta = _entity.Parent.Rotation.Y; //  *Math.PI / 180; // Orientation in rads.
                        _state.TimeStamp = DateTime.Now;
                        base.SendNotification<Replace>(_submgrPort, _state);
                    }
                    catch
                    {
                    }
                }
                yield return Arbiter.Receive(false, TimeoutPort(200), delegate { });
            }
        }


        /// <summary>
        /// Called the first time the simulation engine tells us about our entity.  Activate
        /// the other handlers and insert ourselves into the service directory.
        /// </summary>
        /// <param name="ins"></param>
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
            _entity = (PioneerGPSEntity)ins.Body;
            _entity.ServiceContract = Contract.Identifier;

            CreateDefaultState();
        }

        private void CreateDefaultState()
        {
            _state.X = 0;
            _state.Y = 0;
            _state.Z = 0;
            _state.Theta = 0;
            _state.TimeStamp = DateTime.Now;
        }

        void DeleteEntityNotificationHandler(simengine.DeleteSimulationEntity del)
        {
            _entity = null;
        }

        /// <summary>
        /// Get Handler
        /// </summary>
        /// <param name="get"></param>
        /// <returns></returns>
        [ServiceHandler(ServiceHandlerBehavior.Concurrent)]
        public virtual IEnumerator<ITask> GetHandler(Get get)
        {
            // the state is updated automatically
            get.ResponsePort.Post(_state);
            yield break;
        }

        /// <summary>
        /// Replace Handler
        /// </summary>
        /// <param name="replace"></param>
        /// <returns></returns>
        [ServiceHandler(ServiceHandlerBehavior.Exclusive)]
        public virtual IEnumerator<ITask> ReplaceHandler(Replace replace)
        {
            _state = replace.Body;
            replace.ResponsePort.Post(DefaultReplaceResponseType.Instance);
            yield break;
        }


        /// <summary>
        /// Subscribe Handler
        /// </summary>
        /// <param name="subscribe"></param>
        /// <returns></returns>
        [ServiceHandler(ServiceHandlerBehavior.Concurrent)]
        public virtual IEnumerator<ITask> SubscribeHandler(Subscribe subscribe)
        {
            SubscribeHelper(_submgrPort, subscribe.Body, subscribe.ResponsePort);
            yield break;
        }
    }

}


