//------------------------------------------------------------------------------
//
// ARCOS SONAR SERVICE
//
// CRANIUM - Pioneer 3 DX Hardware Services - ARCOS SONAR
// This service provides access to P3DX frontal Sonar Array. 
// It subscribes to Microsoft ARCOS CORE Service. It offers subscriptions. 
// 
// This code is provided AS IS. No warranty is provided for any purpose. 
// Use it at your own risk. 
// Please notify bugs, suggestions to: raul@conscious-robots.com
//
//  $File: ArcosSonar.cs $ $Revision: 8 $
//------------------------------------------------------------------------------


using Microsoft.Ccr.Core;
using Microsoft.Dss.Core;
using Microsoft.Dss.Core.Attributes;
using Microsoft.Dss.ServiceModel.Dssp;
using Microsoft.Dss.ServiceModel.DsspServiceBase;
using System;
using System.Collections.Generic;
using dssphttp = Microsoft.Dss.Core.DsspHttp;
using dssp = Microsoft.Dss.ServiceModel.Dssp;

// SONAR Generic Contract
using pxsonar = Microsoft.Robotics.Services.Sonar.Proxy;

using xml = System.Xml;

// Alias for ARCOS Core proxy.
using ArcosCore = Microsoft.Robotics.Services.MobileRobots.Arcos.Proxy;

// Alias for Subscription Manager.
using submgr = Microsoft.Dss.Services.SubscriptionManager; 


namespace Cranium.Arcos
{
    
    /// <summary>
    /// CRANIUM Arcos Sonar Service
    /// </summary>
    [Contract(Contract.Identifier)]
    [AlternateContract("http://schemas.microsoft.com/robotics/2006/06/sonar.html")]
    public class ArcosSonar : DsspServiceBase
    {
        /// <summary>
        /// Number of SONAR devices in the SONAR Ring.
        /// </summary>
        public const int SonarArrayLength = 8;

        /// <summary>
        /// This threshold value established whether or not to notify a SONAR Replace.
        /// </summary>
        public const double SonarChangeThreshold = 100.0;

        /// <summary>
        /// This array hold the former reading.
        /// </summary>
        private double[] formerDistanceMeasurements;

        /// <summary>
        /// _state: SONAR State
        /// </summary>
        private pxsonar.SonarState _state = new Microsoft.Robotics.Services.Sonar.Proxy.SonarState();
                
        /// <summary>
        /// _main Port
        /// </summary>
        [ServicePort("/arcossonar", AllowMultipleInstances=false)]
        private pxsonar.SonarOperations _mainPort = new Microsoft.Robotics.Services.Sonar.Proxy.SonarOperations();


        // Define the Arcos Core Service Partner
        [Partner("ArcosCore", Contract = ArcosCore.Contract.Identifier,
            CreationPolicy = PartnerCreationPolicy.UseExistingOrCreate)]
        ArcosCore.ArcosCoreOperations _arcosCorePort = new ArcosCore.ArcosCoreOperations();
        ArcosCore.ArcosCoreOperations _arcosCoreNotify = new ArcosCore.ArcosCoreOperations();


        // Define the Subscription Manager Partner
        [Partner("SubMgr", Contract = submgr.Contract.Identifier,
            CreationPolicy = PartnerCreationPolicy.CreateAlways)]
        private submgr.SubscriptionManagerPort _submgrPort = new submgr.SubscriptionManagerPort();         
                
        
        /// <summary>
        /// Default Service Constructor
        /// </summary>
        public ArcosSonar(DsspServiceCreationPort creationPort) : 
                base(creationPort)
        {
        }


        /// <summary>
        /// Service Start
        /// </summary>
        protected override void Start()
        {
			base.Start();
			// Add service specific initialization here.

            // Allocation for Arcos Sensor readings.
            _state.DistanceMeasurements = new double[SonarArrayLength];
            formerDistanceMeasurements = new double[SonarArrayLength];

            for (int i = 0; i < SonarArrayLength; i++)
            {
                _state.DistanceMeasurements[i] = 0.0;
                formerDistanceMeasurements[1] = 0.0;
            }


            // Subscribe to ArcosCore
            _arcosCorePort.Subscribe(_arcosCoreNotify);

            // Include this service in the directory
            // DirectoryInsert();

            // Add Receiver tasks for the notification messages comming from ArcosCore partner
            Activate<ITask>(
                Arbiter.Receive<ArcosCore.Replace>(true, _arcosCoreNotify, NotifyReplaceHandler)
            );

        }


        /// <summary>
        /// Retrieves Sonar data from Arcos Core and update the state
        /// </summary>
        /// <param name="replace">Replace message</param>
        private void NotifyReplaceHandler(ArcosCore.Replace replace)
        {
            List<ArcosCore.SonarReadingData> _sonarDataList = null;
            // Get SONAR data
            try
            {
                _sonarDataList = replace.Body.Information.Sonar;
            }
            catch (NullReferenceException e)
            {
                LogInfo(e);
                return;
            }

            // Update state
            _state.TimeStamp = replace.Body.Information.TimeStamp;
            try
            {
                // Build a trace for debugging
                // string trace = "ArcosSonar: ";

                // Distance between former reading and current reading is calculated
                double distance = 0.0;

                for (int i = 0; i < SonarArrayLength; i++)
                {
                    _state.DistanceMeasurements[i] = (double)_sonarDataList[i].Reading;
                    // trace = trace + _state.DistanceMeasurements[i] + " ";

                    // Calculate distance for each SONAR device
                    distance += Math.Abs(_state.DistanceMeasurements[i] - formerDistanceMeasurements[i]);

                    // Store former reading for subsequent notifications
                    formerDistanceMeasurements[i] = _state.DistanceMeasurements[i];
                }

                // I am using SonarState.DistanceMeasurement to store the delta 
                // between last and current readings.
                _state.DistanceMeasurement = distance;

                // Values for other members of SonarState
                _state.HardwareIdentifier = 0;
                _state.AngularRange = 195.0;
                _state.AngularResolution = 15.0;  // Raul - Not sure what should be populated here.
                _state.MaxDistance = 600.0;               
                

                // trace = trace + " Dist: " + distance;
                // LogInfo(trace);

                // Notify new sonar readings
                // Only if the readings have changed more than a given threshold.
                if (distance > SonarChangeThreshold)
                {
                    base.SendNotification<Replace>(_submgrPort, _state);
                }
                else
                {
                    // LogInfo("Not significant change. Subs are not notified");
                }

            }
            catch (NullReferenceException e)
            {
                LogInfo("ArcosSonar: Null Reference");
            }
            catch (ArgumentOutOfRangeException e)
            {
                LogInfo("ArcosSonar: Argument out of range");
            }
        }


        /// <summary>
        /// Get Handler
        /// </summary>
        /// <param name="get"></param>
        /// <returns></returns>
        [ServiceHandler(ServiceHandlerBehavior.Concurrent)]
        public virtual IEnumerator<ITask> GetHandler(pxsonar.Get get)
        {
            get.ResponsePort.Post(_state);
            yield break;
        }


        /// <summary>
        /// Replace Handler
        /// </summary>
        /// <param name="replace"></param>
        /// <returns></returns>
        [ServiceHandler(ServiceHandlerBehavior.Exclusive)]
        public virtual IEnumerator<ITask> ReplaceHandler(pxsonar.Replace replace)
        {
            _state = replace.Body;
            replace.ResponsePort.Post(DefaultReplaceResponseType.Instance);
            yield break;
        }


        /// <summary>
        /// Subscribe Handler. Manages subscription requests - Subscribe messages.
        /// </summary>
        /// <param name="subscribe"></param>
        /// <returns></returns>
        [ServiceHandler(ServiceHandlerBehavior.Exclusive)]
        public virtual IEnumerator<ITask> SubscribeHandler(pxsonar.Subscribe subscribe)
        {
            SubscribeRequestType request = subscribe.Body;
            LogInfo("Subscribe request from: " + request.Subscriber);

            // Use the Subscription Manager to insert the new subscriber
            yield return Arbiter.Choice(
                SubscribeHelper(_submgrPort, request, subscribe.ResponsePort),
                    delegate(SuccessResult success)
                    {
                        base.SendNotification<Replace>(_submgrPort, request.Subscriber, _state);
                    },
                    delegate(Exception e)
                    {
                        LogError(null, "Subscribe failed", e);
                    }
            );          
            yield break;
        }



        /// <summary>
        /// ReliableSubscribe Handler
        /// </summary>
        /// <param name="subscribe"></param>
        /// <returns></returns>
        [ServiceHandler(ServiceHandlerBehavior.Exclusive)]
        public virtual IEnumerator<ITask> ReliableSubscribeHandler(pxsonar.ReliableSubscribe subscribe)
        {
            yield return Arbiter.Choice(
                SubscribeHelper(_submgrPort, subscribe.Body, subscribe.ResponsePort),
                delegate(SuccessResult success)
                {
                    _submgrPort.Post(new submgr.Submit(
                        subscribe.Body.Subscriber, dssp.DsspActions.ReplaceRequest, _state, null));
                },
                delegate(Exception ex) { LogError(ex); }
            ); ;
        }



        /// <summary>
        /// HttpGet Handler
        /// </summary>
        /// <param name="get"></param>
        /// <returns></returns>
        [ServiceHandler(ServiceHandlerBehavior.Concurrent)]
        public virtual IEnumerator<ITask> HttpGetHandler(Microsoft.Dss.Core.DsspHttp.HttpGet get)
        {
            get.ResponsePort.Post(new dssphttp.HttpResponseType(_state));
            yield break;
        }
    }
}
