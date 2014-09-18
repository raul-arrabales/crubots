//-----------------------------------------------------------------------
//  This file is part of the Explorer Simulation example 
//  Copyright (C) 2007 by Trevor Taylor, QUT.
//  This code is freely available for non-commercial use.
//
//  $File: ExplorerSimSonar.cs $ $Revision: 4 $
//  Raul Arrabales - Renamed from ExplorerSim to ExplorerSonarSim and added
//  Support for Sonar.
//
//
// TT Jun-2007:
// Original Explorer service was modified so that it will work with
// the MazeSimulator.
// It seems that the simulated Pioneer does not behave the same as
// a real Pioneer because some of the values were way off.
//
// NOTE: This code requires my updated version of the Simulated
// Differential Drive because it gets the pose of the robot to draw
// the map. This is cheating, but it is intended for teaching, not
// to work with actual robots.
//
// TT Jul-2007:
// Some minor changes for the final release of MSRS V1.5.
// Changed mapping to use ray tracing from omap.c instead of drawing
// into a bitmap.
//
// Raul Sept-2007:
// Added support for Sonar.
//
// Raul Nov-2008:
// Migrated to MRDS 2008.
//
// Raul Jan 2010
// Migrated to MRDS 2008 R2 and .NET 3.5, and adapted to CRUBOT services
// Using Simulated GPS to get position.
//
// Known Bugs:
// The robot sometimes gets stuck oscillating from side to side.
// This is a bug somewhere in the code that I have not tried to
// diagnose. It was in the original Microsoft code.
// If this happens, just use the Dashboard to move the robot and
// it should break out of the cycle.
//
// Occassionally the program will crash with an access violation.
// This happens because the GDI uses a Single Threaded Apartment
// (STA) model and it gets confused if two threads access the same
// graphics structures. The recommendation from Microsoft is to do
// all GDI stuff in a WinForm. However, it works most of the time
// as it is so I have not bothered.
//
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;

using W3C.Soap;
using Microsoft.Ccr.Core;
using Microsoft.Dss.Core;
using Microsoft.Dss.Core.Attributes;
using Microsoft.Dss.ServiceModel.Dssp;
using Microsoft.Dss.Services.Serializer;
using Microsoft.Dss.ServiceModel.DsspServiceBase;

using bumper = Microsoft.Robotics.Services.ContactSensor.Proxy;
using drive = Microsoft.Robotics.Services.Drive.Proxy;

// Raul - Added support for Sonar
using sicklrf = Microsoft.Robotics.Services.Sensors.SickLRF.Proxy;
using pxsonar = Microsoft.Robotics.Services.Sonar.Proxy;

using dssp = Microsoft.Dss.ServiceModel.Dssp;
using System.ComponentModel;

using System.Drawing;
using System.Windows.Forms;
using Microsoft.Ccr.Adapters.WinForms;

using webcam = Microsoft.Robotics.Services.WebCam.Proxy;

// Raul - Simulated GPS
using pxGPS = Cranium.Simulation.Sensors.SimulatedPioneerGPS.Proxy;


namespace Microsoft.Robotics.Services.ExplorerSim
{
    [DisplayName("ExplorerSimSonar")]
    [Description("Provides access simple wanderer behaviour for a robot made up of a differential drive, bumpers, and LRF OR a frontal SONAR ring.")]
    [Contract(Contract.Identifier)]
    class ExplorerSimSonar : DsspServiceBase
    {
        #region constants
        // The explorer uses there constants to evaluate open space 
        // and adjust speed and rotation of the differential drive.
        // Change those values to match your robot.
        // You could expose them on the explorers state type and make the
        // service configurable.

        /// <summary>
        /// Amount to backup when hitting an obstacle.
        /// </summary>
        const int BackupDistance = -300; // mm

        /// <summary>
        /// If an obstacle comes within thisdistance the robot stops moving.
        /// </summary>
        const int ObstacleDistance = 500; // mm

        /// <summary>
        /// If the robot is mapping and has this much open space ahead it stops mapping
        /// and enters the space.
        /// </summary>
        const int SafeDistance = 2000; // mm

        /// <summary>
        /// The width of the corridor that must be safe in order to go from mapping to moving.
        /// </summary>
        const int CorridorWidthMapping = 350; // mm

        /// <summary>
        /// The minimum free distance that is required to drive with max. velocity.
        /// </summary>
        const int FreeDistance = 3000; // mm

        /// <summary>
        /// If the free space is at least this distance the robot operates at 1/2 max. velocity otherwise 
        /// the robot slows down to 1/4 of max. vel.
        /// </summary>
        const int AwareOfObstacleDistance = 1500; // mm

        /// <summary>
        /// The max. velocity with which to move.
        /// </summary>
// TT Jun-2007
// A value of 1000 is way too fast in simulation
//        const int MaximumForwardVelocity = 1000; // mm/sec
        const int MaximumForwardVelocity = 600; // mm/sec

        /// <summary>
        /// The with of the corridor in which obstacles effect velocity.
        /// </summary>
        const int CorridorWidthMoving = 500; // mm

        /// <summary>
        /// If no laser data is received within this time the robot stops.
        /// </summary>
        const int WatchdogTimeout = 500; // msec

        /// <summary>
        /// Interval between timer notifications.
        /// </summary>
        const int WatchdogInterval = 100; // msec
        #endregion

        #region main port and state

        // TT - Added for config file
        // Jul-2007 - Need to add the directory in V1.5
        // Raul - Alternative config file for this Sonar version
        private const string InitialStateUri = ServicePaths.MountPoint + @"/packages/crubots/config/ExplorerSimSonar.Config.xml";

        // TT - Added the InitialStatePartner attribute and changed
        // _state to be null. This allows settings to be read from a
        // config file.
        [InitialStatePartner(Optional = true, ServiceUri = InitialStateUri)]
        ExplorerSimSonarState _state = null;

        [ServicePort("/explorersimsonar")]
        ExplorerSimSonarOperations _mainPort = new ExplorerSimSonarOperations();
        #endregion

        #region partners
        #region bumper partner

        [Partner("Bumper", Contract = bumper.Contract.Identifier, CreationPolicy = PartnerCreationPolicy.UseExisting)]
        bumper.ContactSensorArrayOperations _bumperPort = new bumper.ContactSensorArrayOperations();
        bumper.ContactSensorArrayOperations _bumperNotify = new bumper.ContactSensorArrayOperations();
        #endregion

        #region drive partner
        [Partner("Drive", Contract = drive.Contract.Identifier, CreationPolicy = PartnerCreationPolicy.UseExisting)]
        drive.DriveOperations _drivePort = new drive.DriveOperations();
        drive.DriveOperations _driveNotify = new drive.DriveOperations();
        #endregion

        #region sonar partner
        // Raul - sonar partner
        [Partner("Sonar", Contract = pxsonar.Contract.Identifier, CreationPolicy = PartnerCreationPolicy.UseExisting)]
        pxsonar.SonarOperations _sonarPort = new pxsonar.SonarOperations();
        pxsonar.SonarOperations _sonarNotify = new pxsonar.SonarOperations();
        #endregion

        #region GPS partner
        // Raul - Sim GPS Support 
        [Partner("GPS", Contract = pxGPS.Contract.Identifier, CreationPolicy = PartnerCreationPolicy.UseExisting)]
        pxGPS.SimulatedPioneerGPSOperations _gpsPort = new pxGPS.SimulatedPioneerGPSOperations();
        pxGPS.SimulatedPioneerGPSOperations _gpsNotify = new pxGPS.SimulatedPioneerGPSOperations();

        #endregion 


        #endregion


        public ExplorerSimSonar(DsspServiceCreationPort create)
            : base(create)
        {
            // Nothing required here   
        }

        protected override void Start()
        {
            base.Start();

            // TT - Make sure that we have a state in case the config file
            // is missing
            if (_state == null)
            {
                CreateState();
            }
            else
            {
                // Record Current Timestamp
                _state.InitTimeStamp = DateTime.Now;
            }


            #region request handler setup
            Activate(
                Arbiter.Interleave(
                    new TeardownReceiverGroup(
                        Arbiter.Receive<DsspDefaultDrop>(false, _mainPort, DropHandler)
                    ),
                    new ExclusiveReceiverGroup(
                        // Arbiter.Receive<LaserRangeFinderResetUpdate>(true, _mainPort, LaserRangeFinderResetUpdateHandler),
                        // Arbiter.Receive<LaserRangeFinderUpdate>(true, _mainPort, LaserRangeFinderUpdateHandler),
                        // Raul - Sonar Update Handler
                        Arbiter.Receive<SonarUpdate>(true, _mainPort, SonarUpdateHandler),
                        Arbiter.Receive<BumpersUpdate>(true, _mainPort, BumpersUpdateHandler),
                        Arbiter.Receive<BumperUpdate>(true, _mainPort, BumperUpdateHandler),
                        Arbiter.Receive<GPSUpdate>(true, _mainPort, GPSUpdateHandler),
                        Arbiter.Receive<DriveUpdate>(true, _mainPort, DriveUpdateHandler),
                        Arbiter.Receive<WatchDogUpdate>(true, _mainPort, WatchDogUpdateHandler)
                    ),
                    new ConcurrentReceiverGroup(
                        Arbiter.Receive<Get>(true, _mainPort, GetHandler),
                        Arbiter.Receive<dssp.DsspDefaultLookup>(true, _mainPort, DefaultLookupHandler)
                    )
                )
            );
            #endregion

            #region notification handler setup
            Activate(
                Arbiter.Interleave(
                    new TeardownReceiverGroup(),
                    new ExclusiveReceiverGroup(
                    ),
                    new ConcurrentReceiverGroup(
                        // Arbiter.Receive<sicklrf.Reset>(true, _laserNotify, LaserResetNotification),
                        // Raul - Sonar replace notification handler
                        Arbiter.ReceiveWithIterator<pxsonar.Replace>(true, _sonarNotify, SonarReplaceNotification),
                        Arbiter.Receive<drive.Update>(true, _driveNotify, DriveUpdateNotification),
                        Arbiter.Receive<bumper.Replace>(true, _bumperNotify, BumperReplaceNotification),
                        Arbiter.Receive<pxGPS.Replace>(true, _gpsNotify, GPSNotificationHandler),
                        Arbiter.Receive<bumper.Update>(true, _bumperNotify, BumperUpdateNotification)
                    )
                )
            );

            // We cannot replicate the activation of laser notifications because the 
            // handler uses Test() to skip old laser notifications.


            if (_state.UseSonar)
            {
                Activate(
                    Arbiter.ReceiveWithIterator<pxsonar.Replace>(false, _sonarNotify, SonarReplaceNotification)
                );
            }

            #endregion

            // Start watchdog timer
            _mainPort.Post(new WatchDogUpdate(new WatchDogUpdateRequest(DateTime.Now)));

            // Create Subscriptions
            _bumperPort.Subscribe(_bumperNotify);
            _drivePort.Subscribe(_driveNotify);
            _gpsPort.Subscribe(_gpsNotify);

            // Raul - Subscribe depending on configuration

            if (_state.UseSonar)
            {
                _sonarPort.Subscribe(_sonarNotify);
            }
             

        }


        // Create the default state
        void CreateState()
        {
            _state = new ExplorerSimSonarState();

            // Current Timestamp
            _state.InitTimeStamp = DateTime.Now;

            // Map dimensions in meters
            _state.MapWidth = 48;
            _state.MapHeight = 36;
            // Map resolution in meters
            _state.MapResolution = 0.05;
            // Maximum range in meters
            _state.MapMaxRange = 7.99;

            _state.BayesVacant = (byte)MapValues.VACANT;
            _state.BayesObstacle = (byte)MapValues.OBSTACLE;

            // Raul - Use Sonar and LRF flags
            _state.UseLRF = false;
            _state.UseSonar = true; 

            // Raul - Max range for sonar.
            if (_state.UseSonar)
            {
                _state.MapMaxRange = 6.0;
            }


            // Raul - Sonar array length
            _state.SonarTransducers = 8;

            // Raul - Sonar transducer orientation
            _state.SonarRadians = new double[_state.SonarTransducers];

            // Orientations of the P3DX frontal sonar transducers
            _state.SonarRadians[0] = (Math.PI * 90) / 180;
            _state.SonarRadians[1] = (Math.PI * 50) / 180;
            _state.SonarRadians[2] = (Math.PI * 30) / 180;
            _state.SonarRadians[3] = (Math.PI * 10) / 180;
            _state.SonarRadians[4] = -(Math.PI * 10) / 180;
            _state.SonarRadians[5] = -(Math.PI * 30) / 180;
            _state.SonarRadians[6] = -(Math.PI * 50) / 180;
            _state.SonarRadians[7] = -(Math.PI * 90) / 180;

            // Save the state now as a convenience to the user
            LogInfo("Creating Default State for ExplorerSimSonar");
            SaveState(_state);

        }

        #region DSS operation handlers (Get, Drop)
        public void DropHandler(DsspDefaultDrop drop)
        {
            // Currently it is not possible to activate a handler that returns
            // IEnumerator<ITask> in a teardown group. Thus we have to spawn the iterator ourselves
            // to acheive the same effect.
            // This issue will be addressed in upcomming releases.
            SpawnIterator(drop, DoDrop);
        }

        IEnumerator<ITask> DoDrop(DsspDefaultDrop drop)
        {
            if (_mapForm != null)
            {
                MapForm map = _mapForm;
                _mapForm = null;

                WinFormsServicePort.FormInvoke(
                    delegate()
                    {
                        if (!map.IsDisposed)
                        {
                            map.Close();
                            map.Dispose();
                        }
                    }
                );
            }

            if (_state.IsActive)
            {
                LogInfo("ExplorerSimSonar service is being dropped while moving, Requesting Stop.");

                yield return Arbiter.Choice(
                    StopMoving(),
                    delegate(DefaultUpdateResponseType response) { },
                    delegate(Fault fault) { }
                );

                yield return Arbiter.Choice(
                    DisableMotor(),
                    delegate(DefaultUpdateResponseType response) { },
                    delegate(Fault fault) { }
                );
            }

            LogInfo("Dropping ExplorerSimSonar.");

            base.DefaultDropHandler(drop);
        }

        void GetHandler(Get get)
        {
            get.ResponsePort.Post(_state);
        }
        #endregion

        #region Watch dog timer handlers
        void WatchDogUpdateHandler(WatchDogUpdate update)
        {
            // Raul - If using LRF
            if (_state.UseLRF)
            {
                TimeSpan sinceLaser = update.Body.TimeStamp - _state.MostRecentLaser;

                if (sinceLaser.TotalMilliseconds >= WatchdogTimeout && !_state.IsUnknown)
                {
                    LogInfo("Stop requested, last laser data seen at " + _state.MostRecentLaser);
                    StopMoving();
                    _state.LogicalState = LogicalState.Unknown;
                }
            }

            // Raul - If using Sonar
            if (_state.UseSonar)
            {
                TimeSpan sinceSonar = update.Body.TimeStamp - _state.MostRecentSonar;

                //if (sinceSonar.TotalMilliseconds >= WatchdogTimeout && !_state.IsUnknown)
                //{
                //    LogInfo("Stop requested, last sonar data seen at " + _state.MostRecentSonar);
                //    StopMoving();
                //    _state.LogicalState = LogicalState.Unknown;
                //}
            }

            if (_state.SonarData != null)
            {
                if (_state.DrawMap && _state.X != 0 && _state.Y != 0)
                {                    
                    GetMostRecentSonarNotification(_state.SonarData);
                    SpawnIterator(_state.SonarData.AngularRange, _state.SonarData.DistanceMeasurements, UpdateMap);
                }

                double distance = FindNearestObstacleInCorridor(_state.SonarData, CorridorWidthMapping, 45);

                // AvoidCollision and EnterOpenSpace have precedence over
                // all other state transitions and are thus handled first.
                AvoidCollision(distance);
                EnterOpenSpace(distance);

                UpdateLogicalState(_state.SonarData, distance);
            }

            Activate(
               Arbiter.Receive(
                   false,
                   TimeoutPort(WatchdogInterval),
                   delegate(DateTime ts)
                   {
                       _mainPort.Post(new WatchDogUpdate(new WatchDogUpdateRequest(ts)));
                   }
               )
            );

            update.ResponsePort.Post(DefaultUpdateResponseType.Instance);
        }
        #endregion

        #region GPS handlers

        /// <summary>
        /// Handles GPS Replace notifications from GPS service
        /// </summary>
        /// <param name="replace"></param>
        void GPSNotificationHandler(pxGPS.Replace replace)
        {
            _mainPort.Post(new GPSUpdate(replace.Body));
        }

        /// <summary>
        /// Handles GPS updates
        /// </summary>
        /// <param name="update"></param>
        void GPSUpdateHandler(GPSUpdate update)
        {
            // Update robot position (GPS provides sim coordinates, not 2d coords).
            _state.X = -(float)update.Body.X;
            _state.Y = (float)update.Body.Z;
            _state.Theta = (float)(update.Body.Theta + 180 + 90);
        }

        #endregion

        #region  Bumper handlers
        /// <summary>
        /// Handles Replace notifications from the Bumper partner
        /// </summary>
        /// <remarks>Posts a <typeparamref name="BumpersUpdate"/> to itself.</remarks>
        /// <param name="replace">notification</param>
        void BumperReplaceNotification(bumper.Replace replace)
        {
            _mainPort.Post(new BumpersUpdate(replace.Body));
        }

        /// <summary>
        /// Handles Update notification from the Bumper partner
        /// </summary>
        /// <remarks>Posts a <typeparamref name="BumperUpdate"/> to itself.</remarks>
        /// <param name="update">notification</param>
        void BumperUpdateNotification(bumper.Update update)
        {
            _mainPort.Post(new BumperUpdate(update.Body));
        }

        /// <summary>
        /// Handles the <typeparamref name="BumpersUpdate"/> request.
        /// </summary>
        /// <param name="update">request</param>
        void BumpersUpdateHandler(BumpersUpdate update)
        {
            if (_state.IsMoving && BumpersPressed(update.Body))
            {
                Bumped();
            }
            update.ResponsePort.Post(DefaultUpdateResponseType.Instance);
        }

        /// <summary>
        /// Handles the <typeparamref name="BumperUpdate"/> request.
        /// </summary>
        /// <param name="update">request</param>
        void BumperUpdateHandler(BumperUpdate update)
        {
            if (_state.IsMoving && update.Body.Pressed)
            {
                Bumped();
            }
            update.ResponsePort.Post(DefaultUpdateResponseType.Instance);
        }

        /// <summary>
        /// Stops the robot. If the robot was going forward it backs up.
        /// </summary>
        private void Bumped()
        {
            if (_state.Velocity <= 0.0)
            {
                LogInfo("Rear and/or Front bumper pressed, Stopping");
                // either a rear bumper or both front and rear
                // bumpers are pressed. STOP!
                StopTurning();
                StopMoving();

                _state.LogicalState = LogicalState.Unknown;
                _state.Countdown = 3;
            }
            else
            {
                LogInfo("Front bumper pressed, Backing up by " + (-BackupDistance) + "mm");
                // only a front bumper is pressed.
                // move back <BackupDistance> mm;
                StopTurning();
                Translate(BackupDistance);

                _state.LogicalState = LogicalState.Unknown;
                _state.Countdown = 5;
            }
        }

        /// <summary>
        /// Checks whether at least one of the contact sensors is pressed.
        /// </summary>
        /// <param name="bumpers"><code>true</code> if at least one bumper in <paramref name="bumpers"/> is pressed, otherwise <code>false</code></param>
        /// <returns></returns>
        private bool BumpersPressed(bumper.ContactSensorArrayState bumpers)
        {
            if (bumpers.Sensors == null)
            {
                return false;
            }
            foreach (bumper.ContactSensor s in bumpers.Sensors)
            {
                if (s.Pressed)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region Drive handlers
        /// <summary>
        /// Handles Update notification from the Drive partner
        /// </summary>
        /// <remarks>Posts a <typeparamref name="DriveUpdate"/> request to itself.</remarks>
        /// <param name="update">notification</param>
        void DriveUpdateNotification(drive.Update update)
        {
            _mainPort.Post(new DriveUpdate(update.Body));
        }

        /// <summary>
        /// Handles DriveUpdate request
        /// </summary>
        /// <param name="update">request</param>
        void DriveUpdateHandler(DriveUpdate update)
        {
            DriveStateUpdate(update.Body);
            /*
            float w = update.Body.LeftWheel.MotorState.Pose.Orientation.W;
            _state.DriveState = update.Body;
            _state.Velocity = (VelocityFromWheel(update.Body.LeftWheel) + VelocityFromWheel(update.Body.RightWheel)) / 2;
            // Make sure that the pose is up to date
            // NOTE: This is a FUDGE! Also, it only gets updated when something
            // changes, not continuously, so it is usually OUT OF DATE!
            _state.X = (update.Body.LeftWheel.MotorState.Pose.Position.X +
                        update.Body.RightWheel.MotorState.Pose.Position.X) / 2;
            _state.Y = (update.Body.LeftWheel.MotorState.Pose.Position.Z +
                        update.Body.RightWheel.MotorState.Pose.Position.Z) / 2;
            _state.Theta = (float) (Math.Acos(w)*2*180/Math.PI);
            if (update.Body.LeftWheel.MotorState.Pose.Orientation.Y < 0)
                _state.Theta = -_state.Theta;
             */
            update.ResponsePort.Post(DefaultUpdateResponseType.Instance);
        }

        // Update our internal state using a drive state
        void DriveStateUpdate(drive.DriveDifferentialTwoWheelState s)
        {
            float w = s.LeftWheel.MotorState.Pose.Orientation.W;
            _state.DriveState = s;
            _state.Velocity = (VelocityFromWheel(s.LeftWheel) + VelocityFromWheel(s.RightWheel)) / 2;
            // Make sure that the pose is up to date
            // NOTE: This is a FUDGE! Also, it only gets updated when something
            // changes, not continuously, so it is usually OUT OF DATE!
            _state.X = (s.LeftWheel.MotorState.Pose.Position.X +
                        s.RightWheel.MotorState.Pose.Position.X) / 2;
            _state.Y = (s.LeftWheel.MotorState.Pose.Position.Z +
                        s.RightWheel.MotorState.Pose.Position.Z) / 2;
            _state.Theta = (float)(Math.Acos(w) * 2 * 180 / Math.PI);
            if (s.LeftWheel.MotorState.Pose.Orientation.Y < 0)
                _state.Theta = -_state.Theta;
        }

        /// <summary>
        /// Computes the wheel velocity in mm/s.
        /// </summary>
        /// <param name="wheel">wheel</param>
        /// <returns>velocity</returns>
        private int VelocityFromWheel(Microsoft.Robotics.Services.Motor.Proxy.WheeledMotorState wheel)
        {
            if (wheel == null)
            {
                return 0;
            }
            return (int)(1000 * wheel.WheelSpeed); // meters to millimeters
        }
        #endregion

        #region Laser handlers



        /// <summary>
        /// Handles the <typeparamref name="LaserRangeFinderUpdate"/> request.
        /// </summary>
        /// <param name="update">request</param>
        void LaserRangeFinderUpdateHandler(LaserRangeFinderUpdate update)
        {
            sicklrf.State laserData = update.Body;
            _state.MostRecentLaser = laserData.TimeStamp;

            if (_state.DrawMap)
                SpawnIterator(laserData.AngularRange, laserData.DistanceMeasurements, UpdateMap);

            int distance = FindNearestObstacleInCorridor(laserData, CorridorWidthMapping, 45);

            // AvoidCollision and EnterOpenSpace have precedence over
            // all other state transitions and are thus handled first.
            AvoidCollision(distance);
            EnterOpenSpace(distance);

            UpdateLogicalState(laserData, distance);

            update.ResponsePort.Post(DefaultUpdateResponseType.Instance);
        }

        #endregion 

        #region Sonar Handlers

        // Raul - Sonar replace notification
        /// <summary>
        /// Handles Replace notifications from the Sonar partner
        /// </summary>
        /// <remarks>Posts a <typeparamref name="LaserRangeFinderUpdate"/> to itself.</remarks>
        /// <param name="replace">notification</param>
        /// <returns>task enumerator</returns>
        IEnumerator<ITask> SonarReplaceNotification(pxsonar.Replace replace)
        {
            // When this handler is called a couple of notifications may
            // have piled up. We only want the most recent one.
            pxsonar.SonarState sonarData = replace.Body;

            SonarUpdate request = new SonarUpdate(sonarData);

            _mainPort.Post(request);

            yield return Arbiter.Choice(
                request.ResponsePort,
                delegate(DefaultUpdateResponseType response) { },
                delegate(Fault fault) { }
            );

            // Skip messages that have been queued up in the meantime.
            // The notification that are lingering are out of data by now.
            GetMostRecentSonarNotification(sonarData);


            // Reactivate the handler.
            Activate(
                Arbiter.ReceiveWithIterator<pxsonar.Replace>(false, _sonarNotify, SonarReplaceNotification)
            );
        }

        // Raul - Sonar update handler
        /// <summary>
        /// Handles the <typeparamref name="LaserRangeFinderUpdate"/> request.
        /// </summary>
        /// <param name="update">request</param>
        void SonarUpdateHandler(SonarUpdate update)
        {
            pxsonar.SonarState sonarData = update.Body;

            _state.SonarData = sonarData; 

            _state.MostRecentSonar = sonarData.TimeStamp;

            // Raul - Allow one second, so the maze is completely built. Otherwise the sonar seems to go though the walls.
            TimeSpan sinceInit = _state.MostRecentSonar - _state.InitTimeStamp;

            if (sinceInit.Seconds > 4)
            {
                if (_state.DrawMap && _state.X != 0 && _state.Y != 0)
                {
                    SpawnIterator(sonarData.AngularRange, sonarData.DistanceMeasurements, UpdateMap);
                }

                double distance = FindNearestObstacleInCorridor(sonarData, CorridorWidthMapping, 45);

                // AvoidCollision and EnterOpenSpace have precedence over
                // all other state transitions and are thus handled first.
                AvoidCollision(distance);
                EnterOpenSpace(distance);

                UpdateLogicalState(sonarData, distance);
            }
 
            update.ResponsePort.Post(DefaultUpdateResponseType.Instance);
        }

        #endregion


        /// <summary>
        /// If the robot is mapping and there is sufficient open space directly ahead, enter this space.
        /// </summary>
        /// <param name="distance"></param>
        private void EnterOpenSpace(double distance)
        {
            if (distance > SafeDistance && _state.IsMapping)
            {
                // We are mapping but can see plenty of free space ahead.
                // The robot should go into this space.

                StopTurning();
                _state.LogicalState = LogicalState.FreeForwards;
                // TT changed this value
                //_state.Countdown = 4;
                _state.Countdown = 10;
            }
        }

        /// <summary>
        /// If the robot is moving and an obstacle is too close, stop and map the environment for a way around it.
        /// </summary>
        /// <param name="distance"></param>
        private void AvoidCollision(double distance)
        {
            if (distance < ObstacleDistance && _state.IsMoving)
            {
                //
                // We are moving and there is something less than <LaserObstacleDistance>
                // millimeters from the center of the robot. STOP.
                //

                StopMoving();
                _state.LogicalState = LogicalState.Unknown;
                _state.Countdown = 0;
            }
        }


        /// <summary>
        /// Transitions to the most appropriate state.
        /// </summary>
        /// <param name="laserData">most     recently sensed laser range data</param>
        /// <param name="distance">closest obstacle in corridor ahead</param>
        private void UpdateLogicalState(sicklrf.State laserData, int distance)
        {
            if (_state.Countdown > 0)
            {
                _state.Countdown--;
            }
            else if (_state.IsUnknown)
            {
                StartMapping(laserData, distance);
            }
            else if (_state.IsMoving)
            {
                Move(laserData, distance);
            }
            else if (_state.IsMapping)
            {
                Map(laserData, distance);
            }
        }

        // Raul - UpdateLogicalState for Sonar
        /// <summary>
        /// Transitions to the most appropriate state.
        /// </summary>
        /// <param name="sonarData">most recently sensed sonar range data</param>
        /// <param name="distance">closest obstacle in corridor ahead</param>
        private void UpdateLogicalState(pxsonar.SonarState sonarData, double distance)
        {
            if (_state.Countdown > 0)
            {
                _state.Countdown--;
            }
            else if (_state.IsUnknown)
            {
                StartMapping(sonarData, distance);
            }
            else if (_state.IsMoving)
            {
                Move(sonarData, distance);
            }
            else if (_state.IsMapping)
            {
                Map(sonarData, distance);
            }
        }


        /// <summary>
        /// Implements the "Moving" meta state.
        /// </summary>
        /// <param name="laserData">most recently sensed laser range data</param>
        /// <param name="distance">closest obstacle in corridor ahead</param>
        private void Move(sicklrf.State laserData, int distance)
        {
            switch (_state.LogicalState)
            {
                case LogicalState.AdjustHeading:
                    AdjustHeading();
                    break;
                case LogicalState.FreeForwards:
                    AdjustVelocity(laserData, distance);
                    break;
                default:
                    LogInfo("ExplorerSim.Move() called in illegal state");
                    break;
            }
        }

        // Raul - Move for Sonar data
        /// <summary>
        /// Implements the "Moving" meta state.
        /// </summary>
        /// <param name="sonarData">most recently sensed sonar range data</param>
        /// <param name="distance">closest obstacle in corridor ahead</param>
        private void Move(pxsonar.SonarState sonarData, double distance)
        {
            switch (_state.LogicalState)
            {
                case LogicalState.AdjustHeading:
                    AdjustHeading();
                    break;
                case LogicalState.FreeForwards:
                    AdjustVelocity(sonarData, distance);
                    break;
                default:
                    LogInfo("ExplorerSimSonar.Move() called in illegal state");
                    break;
            }
        }


        /// <summary>
        /// Implements the "Mapping" meta state.
        /// </summary>
        /// <param name="laserData">most recently sensed laser range data</param>
        /// <param name="distance">closest obstacle in corridor ahead</param>
        private void Map(sicklrf.State laserData, int distance)
        {
            switch (_state.LogicalState)
            {
                case LogicalState.RandomTurn:
                    LogInfo("Random Turn");         // TT
                    RandomTurn();
                    break;
                case LogicalState.MapSurroundings:
                    _state.Mapped = true;
                    LogInfo("Turning 180 deg to map");
                    Turn(180);

                    _state.LogicalState = LogicalState.MapSouth;
                    _state.Countdown = 15;
                    break;
                case LogicalState.MapSouth:
                    LogInfo("Mapping the View South");
                    _state.South = laserData;
                    Turn(180);

                    _state.LogicalState = LogicalState.MapNorth;
                    _state.Countdown = 15;
                    break;
                case LogicalState.MapNorth:
                    LogInfo("Mapping the View North");
                    _state.NewHeading = FindBestComposite(_state.South, laserData);
                    LogInfo("Map suggest turn: " + _state.NewHeading);
                    _state.South = null;
                    _state.LogicalState = LogicalState.AdjustHeading;
                    break;
                default:
                    LogInfo("ExplorerSim.Map() called in illegal state");
                    break;
            }
        }


        // Raul - Map with Sonar data
        /// <summary>
        /// Implements the "Mapping" meta state.
        /// </summary>
        /// <param name="sonarData">most recently sensed sonar range data</param>
        /// <param name="distance">closest obstacle in corridor ahead</param>
        private void Map(pxsonar.SonarState sonarData, double distance)
        {
            switch (_state.LogicalState)
            {
                case LogicalState.RandomTurn:
                    LogInfo("Random Turn");         // TT
                    RandomTurn();
                    break;
                case LogicalState.MapSurroundings:
                    _state.Mapped = true;
                    LogInfo("Turning 180 deg to map");
                    Turn(180);

                    _state.LogicalState = LogicalState.MapSouth;
                    _state.Countdown = 15;
                    break;
                case LogicalState.MapSouth:
                    LogInfo("Mapping the View South");
                    _state.SouthSonar = sonarData;
                    Turn(180);

                    _state.LogicalState = LogicalState.MapNorth;
                    _state.Countdown = 15;
                    break;
                case LogicalState.MapNorth:
                    LogInfo("Mapping the View North");
                    _state.NewHeading = FindBestComposite(_state.SouthSonar, sonarData);
                    LogInfo("Map suggest turn: " + _state.NewHeading);
                    _state.South = null;
                    _state.LogicalState = LogicalState.AdjustHeading;
                    break;
                default:
                    LogInfo("ExplorerSimSonar.Map() called in illegal state");
                    break;
            }
        }



        /// <summary>
        /// Adjusts the velocity based on environment.
        /// </summary>
        /// <param name="laserData">most recently sensed laser range data</param>
        /// <param name="distance">closest obstacle in corridor ahead</param>
        private void AdjustVelocity(sicklrf.State laserData, int distance)
        {
            _state.Mapped = false;
            // Why does AdjustVelocity call Turn() ???
            // This does not seem to make any sense, and the robot gets
            // stuck in oscillations to the left and right
            int test = FindBestFrom(laserData, 0, _state.Velocity / 10, CorridorWidthMoving);

            if (distance > FreeDistance)
            {
                MoveForward(MaximumForwardVelocity);

                //if (Math.Abs(test) < 10)
                if (Math.Abs(test) < 10)
                {
                    Turn(test / 2);
                    // TT Jun-2007 - Changed to slow down
                    _state.Countdown = Math.Abs((test / 2) / 5);
                }
            }
            else if (distance > AwareOfObstacleDistance)
            {
                MoveForward(MaximumForwardVelocity / 2);

                //if (Math.Abs(test) < 45)
                if (Math.Abs(test) < 45)
                {
                    Turn(test / 2);
                    // TT Jun-2007 - Changed
                    _state.Countdown = Math.Abs((test / 2) / 5);
                }
            }
            else
            {
                MoveForward(MaximumForwardVelocity / 4);

                if (Math.Abs(test) < 60)
                {
                    Turn(test);
                    // TT Jun-2007 - Changed
                    //_state.Countdown = Math.Abs(test / 10);
                    _state.Countdown = 5;
                }
            }
        }


        // Raul - Adjust velocity for sonarData
        /// <summary>
        /// Adjusts the velocity based on environment.
        /// </summary>
        /// <param name="sonarData">most recently sensed sonar range data</param>
        /// <param name="distance">closest obstacle in corridor ahead</param>
        private void AdjustVelocity(pxsonar.SonarState sonarData, double distance)
        {
            _state.Mapped = false;
            // Why does AdjustVelocity call Turn() ???
            // This does not seem to make any sense, and the robot gets
            // stuck in oscillations to the left and right
            double test = FindBestFrom(sonarData, 0, _state.Velocity / 10, CorridorWidthMoving);

            if (distance > FreeDistance)
            {
                MoveForward(MaximumForwardVelocity);

                //if (Math.Abs(test) < 10)
                if (Math.Abs(test) < 10)
                {
                    Turn((int)test / 2);
                    // TT Jun-2007 - Changed to slow down
                    _state.Countdown = (int)Math.Abs((test / 2) / 5);
                }
            }
            else if (distance > AwareOfObstacleDistance)
            {
                MoveForward(MaximumForwardVelocity / 2);

                //if (Math.Abs(test) < 45)
                if (Math.Abs(test) < 45)
                {
                    Turn((int)test / 2);
                    // TT Jun-2007 - Changed
                    _state.Countdown = (int)Math.Abs((test / 2) / 5);
                }
            }
            else
            {
                MoveForward(MaximumForwardVelocity / 4);

                if (Math.Abs(test) < 60)
                {
                    Turn((int)test);
                    // TT Jun-2007 - Changed
                    //_state.Countdown = Math.Abs(test / 10);
                    _state.Countdown = 5;
                }
            }
        }

        /// <summary>
        /// Implements the "AdjustHeading" state.
        /// </summary>
        private void AdjustHeading()
        {
            LogInfo("Step Turning to: " + _state.NewHeading);
            Turn(_state.NewHeading);

            _state.LogicalState = LogicalState.FreeForwards;
            _state.Countdown = Math.Abs(_state.NewHeading / 10);
        }

        /// <summary>
        /// Implements the "RandomTurn" state.
        /// </summary>
        private void RandomTurn()
        {
            _state.NewHeading = new Random().Next(-115, 115);
            LogInfo("Start Turning (random) to: " + _state.NewHeading);
            Turn(_state.NewHeading);

            _state.LogicalState = LogicalState.Unknown;
            _state.Countdown = 2 + Math.Abs(_state.NewHeading / 10);
        }


        /// <summary>
        /// Transitions to "Mapping" meta state or "AdjustHeading" state depending on
        /// environment.
        /// </summary>
        /// <param name="laserData">most recently sensed laser range data</param>
        /// <param name="distance">closest obstacle in corridor ahead</param>
        private void StartMapping(sicklrf.State laserData, int distance)
        {
            StopMoving();

            if (distance < ObstacleDistance)
            {
                if (_state.Mapped)
                {
                    // We have been mapping before but do not seem to
                    // have found anything.
                    _state.LogicalState = LogicalState.RandomTurn;
                }
                else
                {
                    _state.LogicalState = LogicalState.MapSurroundings;
                }
            }
            else
            {
                int step = Math.Min(ObstacleDistance, distance - CorridorWidthMapping);
                // find the best angle from step mm in front of
                // our current position
                _state.NewHeading = FindBestFrom(laserData, 0, step, CorridorWidthMapping);

                LogInfo("Step: " + step + " Turn: " + _state.NewHeading);
                Translate(step);

                _state.LogicalState = LogicalState.AdjustHeading;
                _state.Countdown = step / 50 + Math.Abs(_state.NewHeading / 10);
            }
        }

        // Raul - Start mapping for Sonar data
        /// <summary>
        /// Transitions to "Mapping" meta state or "AdjustHeading" state depending on
        /// environment.
        /// </summary>
        /// <param name="sonarData">most recently sensed sonar range data</param>
        /// <param name="distance">closest obstacle in corridor ahead</param>
        private void StartMapping(pxsonar.SonarState sonarData, double distance)
        {
            StopMoving();

            if (distance < ObstacleDistance)
            {
                if (_state.Mapped)
                {
                    // We have been mapping before but do not seem to
                    // have found anything.
                    _state.LogicalState = LogicalState.RandomTurn;
                }
                else
                {
                    _state.LogicalState = LogicalState.MapSurroundings;
                }
            }
            else
            {
                double step = Math.Min(ObstacleDistance, distance - CorridorWidthMapping);
                // find the best angle from step mm in front of
                // our current position
                _state.NewHeading = FindBestFrom(sonarData, 0, step, CorridorWidthMapping);

                LogInfo("Step: " + step + " Turn: " + _state.NewHeading);
                Translate((int)step);

                _state.LogicalState = LogicalState.AdjustHeading;
                _state.Countdown = (int)step / 50 + Math.Abs(_state.NewHeading / 10);
            }
        }


        /// <summary>
        /// Gets the most recent laser notification. Older notifications are dropped.
        /// </summary>
        /// <param name="laserData">last known laser data</param>
        /// <returns>most recent laser data</returns>
        private pxsonar.SonarState GetMostRecentSonarNotification(pxsonar.SonarState sonarData)
        {
            pxsonar.Replace testReplace;

            // _laserNotify is a PortSet<>, P3 represents IPort<sicklrf.Replace> that
            // the portset contains
            int count = _sonarNotify.P3.ItemCount - 1;

            for (int i = 0; i < count; i++)
            {
                testReplace = _sonarNotify.Test<pxsonar.Replace>();
                if (testReplace.Body.TimeStamp > sonarData.TimeStamp)
                {
                    sonarData = testReplace.Body;
                }
            }
           
            if (count > 0)
            {
                LogInfo(string.Format("Dropped {0} sonar readings (sonar start)", count));
            }
            return sonarData;
        }



        /// <summary>
        /// Handle the <typeparamref name="LaserRangeFinderResetUpdate"/> request.
        /// </summary>
        /// <remarks>Stops the robot.</remarks>
        /// <param name="update">request</param>
        void LaserRangeFinderResetUpdateHandler(LaserRangeFinderResetUpdate update)
        {
            if (_state.LogicalState != LogicalState.Unknown)
            {
                LogInfo("Stop requested: laser reported reset");
                StopMoving();

                _state.LogicalState = LogicalState.Unknown;
                _state.Countdown = 0;
            }
            update.ResponsePort.Post(DefaultUpdateResponseType.Instance);
        }


        /// <summary>
        /// Respresent a laser range finder reading
        /// </summary>
        class RangeData
        {
            /// <summary>
            /// Creates a new instance.
            /// </summary>
            /// <param name="distance">measured distance</param>
            /// <param name="heading">heading in degrees</param>
            public RangeData(double distance, double heading)
            {
                _distance = distance;
                _heading = heading;
            }

            double _distance;
            double _heading;

            /// <summary>
            /// Gets the distance in milimeters.
            /// </summary>
            public double Distance
            {
                get { return _distance; }
            }

            /// <summary>
            /// Gets the heading in degrees.
            /// </summary>
            public double Heading
            {
                get { return _heading; }
            }

            /// <summary>
            /// Comparer to sort instances by distance.
            /// </summary>
            /// <param name="first">first reading</param>
            /// <param name="second">second reading</param>
            /// <returns>a value less than 0 if  <paramref name="first"/> is closer than <paramref name="second"/>, 0 if both have the same distance, a value greater 0 otherwise</returns>
            static public int ByDistance(RangeData first, RangeData second)
            {
                return first._distance.CompareTo(second._distance);
            }
        }


       /// <summary>
        /// Finds the best free corridor (maximum free space ahead) in a 360 degree scan.
        /// </summary>
        /// <param name="south">the backward half of the scan</param>
        /// <param name="north">the forward half of the scan</param>
        /// <returns>best heading in degrees</returns>
        private int FindBestComposite(sicklrf.State south, sicklrf.State north)
        {
            sicklrf.State composite = new sicklrf.State();

            composite.DistanceMeasurements = new int[720];

            for (int i = 0; i < 720; i++)
            {
                if (i < 180)
                {
                    composite.DistanceMeasurements[i] = south.DistanceMeasurements[i + 180];
                }
                else if (i < 540)
                {
                    composite.DistanceMeasurements[i] = north.DistanceMeasurements[i - 180];
                }
                else
                {
                    composite.DistanceMeasurements[i] = south.DistanceMeasurements[i - 540];
                }
            }

            composite.AngularResolution = 0.5;
            composite.AngularRange = 360;
            composite.Units = north.Units;

            return FindBestFrom(composite, 0, 0, CorridorWidthMoving);
        }



        // Raul - Find best composite for Sonar Data.
        /// <summary>
        /// Finds the best free corridor (maximum free space ahead) in a 360 degree scan.
        /// </summary>
        /// <param name="south">the backward half of the scan</param>
        /// <param name="north">the forward half of the scan</param>
        /// <returns>best heading in degrees</returns>
        private int FindBestComposite(pxsonar.SonarState south, pxsonar.SonarState north)
        {
            pxsonar.SonarState composite = new pxsonar.SonarState();

            // Raul - I am assuming just one 8 transducer frontal sonar ring.
            // Raul - That means just 8 measuments per 180 degrees. 
            // Raul - Therefore a 360 scan has SonarTranducers * 2 measurements.
            composite.DistanceMeasurements = new double[_state.SonarTransducers*2];

            for (int i = 0; i < _state.SonarTransducers*2; i++)
            {
                if (i < _state.SonarTransducers)
                {
                    composite.DistanceMeasurements[i] = south.DistanceMeasurements[i];
                }
                else
                {
                    composite.DistanceMeasurements[i] = north.DistanceMeasurements[i-_state.SonarTransducers];
                }
            }

            composite.AngularResolution = 22.5;
            composite.AngularRange = 360;
            // composite.Units = north.Units;

            return FindBestFrom(composite, 0, 0, CorridorWidthMoving);
        }



        /// <summary>
        /// Finds the best heading in a 180 degree laser scan
        /// </summary>
        /// <param name="laserData">laser scan</param>
        /// <param name="dx">horizontal offset</param>
        /// <param name="dy">vertical offset</param>
        /// <param name="width">width of corridor that must be free</param>
        /// <returns>best heading in degrees</returns>
        private int FindBestFrom(sicklrf.State laserData, int dx, int dy, int width)
        {
            int count = laserData.DistanceMeasurements.Length;
            double span = Math.PI * laserData.AngularRange / 180.0;
            int result;

            List<RangeData> ranges = new List<RangeData>();

            for (int i = 0; i < count; i++)
            {
                int range = laserData.DistanceMeasurements[i];
                double angle = span * i / count - span / 2.0;

                double x = range * Math.Sin(angle) - dx;
                double y = range * Math.Cos(angle) - dy;

                angle = Math.Atan2(-x, y);
                range = (int)Math.Sqrt(x * x + y * y);

                ranges.Add(new RangeData(range, angle));
            }

            ranges.Sort(RangeData.ByDistance);

            for (int i = 0; i < ranges.Count; i++)
            {
                RangeData curr = ranges[i];

                double delta = Math.Atan2(width, curr.Distance);
                double low = curr.Heading - delta;
                double high = curr.Heading + delta;

                for (int j = i + 1; j < ranges.Count; j++)
                {
                    if (ranges[j].Heading > low &&
                        ranges[j].Heading < high)
                    {
                        ranges.RemoveAt(j);
                        j--;
                    }

                }
            }

            ranges.Reverse();

            int bestDistance = (int)ranges[0].Distance;
            double bestHeading = ranges[0].Heading;
            Random rand = new Random();

            for (int i = 0; i < ranges.Count; i++)
            {
                if (ranges[i].Distance < bestDistance)
                {
                    break;
                }
                if (rand.Next(i + 1) == 0)
                {
                    bestHeading = ranges[i].Heading;
                }
            }

            // TT - Log the value
            result = -(int)Math.Round(180 * bestHeading / Math.PI);
            LogInfo("Find Best Heading: " + result);

            return result;
        }


        // Raul - FindBestFrom for Sonar data
        /// <summary>
        /// Finds the best heading in a 180 degree sonar scan
        /// </summary>
        /// <param name="sonarData">sonar scan</param>
        /// <param name="dx">horizontal offset</param>
        /// <param name="dy">vertical offset</param>
        /// <param name="width">width of corridor that must be free</param>
        /// <returns>best heading in degrees</returns>
        private int FindBestFrom(pxsonar.SonarState sonarData, double dx, double dy, double width)
        {
            int count = sonarData.DistanceMeasurements.Length;
            // double span = Math.PI * sonarData.AngularRange / 180.0;
            int result;

            List<RangeData> ranges = new List<RangeData>();

            for (int i = 0; i < count; i++)
            {
                double range = sonarData.DistanceMeasurements[i];
                double angle; // Raul - angle = span * i / count - span / 2.0;

                if (i < _state.SonarTransducers)
                {
                    angle = _state.SonarRadians[i];
                }
                else
                {
                    angle = -_state.SonarRadians[i-_state.SonarTransducers];
                }

                double x = range * Math.Sin(angle) - dx;
                double y = range * Math.Cos(angle) - dy;

                angle = Math.Atan2(-x, y);
                range = (int)Math.Sqrt(x * x + y * y);

                ranges.Add(new RangeData(range, angle));
            }

            ranges.Sort(RangeData.ByDistance);

            for (int i = 0; i < ranges.Count; i++)
            {
                RangeData curr = ranges[i];

                double delta = Math.Atan2(width, curr.Distance);
                double low = curr.Heading - delta;
                double high = curr.Heading + delta;

                for (int j = i + 1; j < ranges.Count; j++)
                {
                    if (ranges[j].Heading > low &&
                        ranges[j].Heading < high)
                    {
                        ranges.RemoveAt(j);
                        j--;
                    }

                }
            }

            ranges.Reverse();

            double bestDistance = ranges[0].Distance;
            double bestHeading = ranges[0].Heading;
            Random rand = new Random();

            for (int i = 0; i < ranges.Count; i++)
            {
                if (ranges[i].Distance < bestDistance)
                {
                    break;
                }
                if (rand.Next(i + 1) == 0)
                {
                    bestHeading = ranges[i].Heading;
                }
            }

            // TT - Log the value
            result = -(int)Math.Round(180 * bestHeading / Math.PI);
            LogInfo("Find Best Heading: " + result);

            return result;
        }



        /// <summary>
        /// Finds closest obstacle in a corridor.
        /// </summary>
        /// <param name="laserData">laser scan</param>
        /// <param name="width">corridor width</param>
        /// <param name="fov">field of view in degrees</param>
        /// <returns>distance to the closest obstacle</returns>
        private int FindNearestObstacleInCorridor(sicklrf.State laserData, int width, int fov)
        {
            int index;
            int best = 8192;
            // TT - There is a timing issue during startup whereby the
            // LRF actually does not supply any data! Just ignore it.
            if (laserData == null || laserData.DistanceMeasurements == null)
                return 0;
            int count = laserData.DistanceMeasurements.Length;
            double rangeLow = -laserData.AngularRange / 2.0;
            double rangeHigh = laserData.AngularRange / 2.0;
            double span = laserData.AngularRange;

            for (index = 0; index < count; index++)
            {
                double angle = rangeLow + (span * index) / count;
                if (Math.Abs(angle) < fov)
                {
                    angle = angle * Math.PI / 180;

                    int range = laserData.DistanceMeasurements[index];
                    // TT - The simulated LRF returns zero if there is
                    // no hit. It should return max range. I reported
                    // this and it should be fixed in V1.5.
                    if (range == 0)
                        range = 8192;
                    int x = (int)(range * Math.Sin(angle));
                    int y = (int)(range * Math.Cos(angle));

                    if (Math.Abs(x) < width)
                    {
                        if (range < best)
                        {
                            best = range;
                        }
                    }
                }
            }

            // TT - Log the value
            //LogInfo("Nearest Obstacle: " + best);
            return best;
        }



        // Raul - Finds closest obstacle in a corridor for sonar data
        /// <summary>
        /// Finds closest obstacle in a corridor.
        /// </summary>
        /// <param name="sonarData">sonar scan</param>
        /// <param name="width">corridor width</param>
        /// <param name="fov">field of view in degrees</param>
        /// <returns>distance to the closest obstacle</returns>
        private double FindNearestObstacleInCorridor(pxsonar.SonarState sonarData, int width, int fov)
        {
            int index;
            double best = 8192;
            // TT - There is a timing issue during startup whereby the
            // LRF actually does not supply any data! Just ignore it.
            if (sonarData == null || sonarData.DistanceMeasurements == null)
                return 0;
            int count = sonarData.DistanceMeasurements.Length;
            double rangeLow = -sonarData.AngularRange / 2.0;
            double rangeHigh = sonarData.AngularRange / 2.0;
            double span = sonarData.AngularRange;

            for (index = 0; index < count; index++)
            {
                double angle = rangeLow + (span * index) / count;
                if (Math.Abs(angle) < fov)
                {
                    angle = angle * Math.PI / 180;

                    double range = sonarData.DistanceMeasurements[index];

                    int x = (int)(range * Math.Sin(angle));
                    int y = (int)(range * Math.Cos(angle));

                    if (Math.Abs(x) < width)
                    {
                        if (range < best)
                        {
                            best = range;
                        }
                    }
                }
            }

            // TT - Log the value
            //LogInfo("Nearest Obstacle: " + best);
            return best;
        }
    

        #region Drive helper method

        /// <summary>
        /// Sets the forward velocity of the drive.
        /// </summary>
        /// <param name="speed">velocity in mm/s</param>
        /// <returns>response port</returns>
        private PortSet<DefaultUpdateResponseType, Fault> MoveForward(int speed)
        {
            LogInfo(string.Format("MoveForward speed={0}", speed));
            if ((_state.DriveState == null || !_state.DriveState.IsEnabled) && speed != 0)
            {
                EnableMotor();
            }

            drive.SetDriveSpeedRequest request = new drive.SetDriveSpeedRequest();

            request.LeftWheelSpeed = (double)speed / 1000.0; // millimeters to meters 
            request.RightWheelSpeed = (double)speed / 1000.0; // millimeters to meters 

            return _drivePort.SetDriveSpeed(request);
        }

        /// <summary>
        /// Turns the drive relative to its current heading.
        /// </summary>
        /// <param name="angle">angle in degrees</param>
        /// <returns>response port</returns>
        PortSet<DefaultUpdateResponseType, Fault> Turn(int angle)
        {
            angle = -angle;
            if (_state.DriveState == null || !_state.DriveState.IsEnabled)
            {
                EnableMotor();
            }

            return _drivePort.RotateDegrees(angle, 0.2f);
        }

        /// <summary>
        /// Moves the drive forward for the specified distance.
        /// </summary>
        /// <param name="step">distance in mm</param>
        /// <returns>response port</returns>
        PortSet<DefaultUpdateResponseType, Fault> Translate(int step)
        {
            if (_state.DriveState == null || !_state.DriveState.IsEnabled)
            {
                EnableMotor();
            }

            drive.DriveDistanceRequest request = new drive.DriveDistanceRequest();
            request.Distance = (double)step / 1000.0; // millimeters to meters

            return _drivePort.DriveDistance(request);
        }

        /// <summary>
        /// Sets the velocity of the drive to 0.
        /// </summary>
        /// <returns></returns>
        PortSet<DefaultUpdateResponseType, Fault> StopMoving()
        {
            return MoveForward(0);
        }

        /// <summary>
        /// Sets the turning velocity to 0.
        /// </summary>
        /// <returns>response port</returns>
        PortSet<DefaultUpdateResponseType, Fault> StopTurning()
        {
            return Turn(0);
        }

        /// <summary>
        /// Enables the drive
        /// </summary>
        /// <returns>response port</returns>
        PortSet<DefaultUpdateResponseType, Fault> EnableMotor()
        {
            return EnableMotor(true);
        }

        /// <summary>
        /// Disables the drive
        /// </summary>
        /// <returns>repsonse port</returns>
        PortSet<DefaultUpdateResponseType, Fault> DisableMotor()
        {
            return EnableMotor(false);
        }

        /// <summary>
        /// Sets the drives enabled state.
        /// </summary>
        /// <param name="enable">new enables state</param>
        /// <returns>response port</returns>
        PortSet<DefaultUpdateResponseType, Fault> EnableMotor(bool enable)
        {
            drive.EnableDriveRequest request = new drive.EnableDriveRequest();
            request.Enable = enable;

            return _drivePort.EnableDrive(request);
        }
        #endregion

        #region Mapping

        // This region contains all of the mapping code

        // This is the map object
        private static Mapper _map = null;
        // The Windows Form for displaying the map
        MapForm _mapForm = null;
        // A bitmap used for displaying the map
        Bitmap _mapBitmap;

        /// <summary>
        /// Update the map when new laser data arrives
        /// </summary>
        /// <param name="AngularRange"></param>
        /// <param name="DistanceMeasurements"></param>
        /// <returns></returns>
        IEnumerator<ITask> UpdateMap(int AngularRange, int[] DistanceMeasurements)
        {
            // Insurance in case the LRF is not behaving ...
            if (DistanceMeasurements == null)
                yield break;

            // Create a new map if we don't have one
            if (_map == null)
            {
                double startAngle = AngularRange / 2.0;
                // AngularResolution is zero for the simulated LRF!
                // This is a bug that I have reported and it should be
                // fixed in V1.5. In the meantime, calculate the value.
                double angleIncrement = (double)AngularRange / (double)DistanceMeasurements.Length;
                // Create the mapper object
                _map = new Mapper();
                // Set the drawing mode
                _map.mode = _state.DrawingMode;

                // Raul - For Laser
                if (_state.UseLRF)
                {
                    // Set the parameters and allocate the necessary memory
                    _mapBitmap = _map.Init(false, DistanceMeasurements.Length, _state.MapMaxRange,
                                    startAngle, angleIncrement,
                                    _state.MapWidth, _state.MapHeight,
                                    _state.MapResolution);
                }
                // Raul - Different parameters for Sonar data (P3DX sonar transducers orientation)
                else if (_state.UseSonar)
                {
                    
                    // Set the parameters and allocate the necessary memory
                    _mapBitmap = _map.Init(true, DistanceMeasurements.Length, _state.MapMaxRange,
                                    startAngle, angleIncrement,
                                    _state.MapWidth, _state.MapHeight,
                                    _state.MapResolution);
                }


                // Clear the map initially (could be moved to Alloc)
                _map.Clear();
            }

            // Make sure that we have a Windows Form to display the map
            if (_mapForm == null)
                MakeForm();

            // Get the current pose from the Differential Drive
            //drive.Get GetRequest = new drive.Get();
            //_drivePort.Post(GetRequest);
            // NOTE: Using yield return means that the code will not
            // continue until the drive state has been returned
            //yield return Arbiter.Choice(
            //    GetRequest.ResponsePort,
            //    DriveStateUpdate,
            //    delegate(Fault fault) { }
            //);

            // Draw the map now using the new LRF data and the robot's pose
            // Note that DrawMap used GDI primitives to draw into a bitmap
            // This is one way to do it, but it is not very flexible
            /*
            _map.DrawMap(_mapBitmap, -_state.X, _state.Y, (_state.Theta - 90),
                        AngularRange, DistanceMeasurements);
            DisplayImage(_mapBitmap);
            */

            // Add new laser data to the existing map
            _map.Add(-_state.X, _state.Y, (_state.Theta - 90),
                        DistanceMeasurements.Length, DistanceMeasurements);
            // Display the new map by requesting a bitmap image and then
            // passing it to the code inside the Windows Form
            DisplayImage(_map.MapToImage());

        }


        /// <summary>
        /// Update the map when new laser data arrives
        /// </summary>
        /// <param name="AngularRange"></param>
        /// <param name="DistanceMeasurements"></param>
        /// <returns></returns>
        IEnumerator<ITask> UpdateMap(double AngularRange, double[] DistanceMeasurements)
        {
            // LogInfo("updating map..:");

            // Insurance in case the LRF is not behaving ...
            if (DistanceMeasurements == null)
                yield break;

            if (_state.X == 0 && _state.Y == 0)
                yield break; 

            // Create a new map if we don't have one
            if (_map == null)
            {
                double startAngle = AngularRange / 2.0;
                // AngularResolution is zero for the simulated LRF!
                // This is a bug that I have reported and it should be
                // fixed in V1.5. In the meantime, calculate the value.
                double angleIncrement = (double)AngularRange / (double)DistanceMeasurements.Length;
                // Create the mapper object
                _map = new Mapper();
                // Set the drawing mode
                _map.mode = _state.DrawingMode;
                // Set the parameters and allocate the necessary memory
                _mapBitmap = _map.Init(_state.UseSonar, DistanceMeasurements.Length, _state.MapMaxRange,
                                startAngle, angleIncrement,
                                _state.MapWidth, _state.MapHeight,
                                _state.MapResolution);
                // Clear the map initially (could be moved to Alloc)
                _map.Clear();
            }

            // Make sure that we have a Windows Form to display the map
            if (_mapForm == null)
                MakeForm();

            //// Get the current pose from the Differential Drive
            //drive.Get GetRequest = new drive.Get();
            //_drivePort.Post(GetRequest);
            //// NOTE: Using yield return means that the code will not
            //// continue until the drive state has been returned
            //yield return Arbiter.Choice(
            //    GetRequest.ResponsePort,
            //    DriveStateUpdate,
            //    delegate(Fault fault) { }
            //);

            // Draw the map now using the new LRF data and the robot's pose
            // Note that DrawMap used GDI primitives to draw into a bitmap
            // This is one way to do it, but it is not very flexible
            /*
            _map.DrawMap(_mapBitmap, -_state.X, _state.Y, (_state.Theta - 90),
                        AngularRange, DistanceMeasurements);
            DisplayImage(_mapBitmap);
            */

            // Add new laser data to the existing map
            int[] distances = new int[DistanceMeasurements.Length];
            for (int i = 0; i < distances.Length; i++)
            {
                distances[i] = (int)DistanceMeasurements[i];
            }
            _map.Add(_state.X, _state.Y, _state.Theta,
                        DistanceMeasurements.Length, distances);
            // Display the new map by requesting a bitmap image and then
            // passing it to the code inside the Windows Form
            DisplayImage(_map.MapToImage());

        }


        // Create and display the Windows Form containing the map
        bool MakeForm()
        {
            Fault fault = null;

            RunForm runForm = new RunForm(CreateMapForm);

            WinFormsServicePort.Post(runForm);

            Arbiter.Choice(
                runForm.pResult,
                delegate(SuccessResult success) { },
                delegate(Exception e)
                {
                    fault = Fault.FromException(e);
                }
            );

            if (fault != null)
            {
                LogError(null, "Failed to create Map window", fault);
                return false;
            }
            else
                return true;
        }


        // The actual form creation needs to be called by RunForm
        Form CreateMapForm()
        {
            _mapForm = new MapForm();
            return _mapForm;
        }


        // Display a bitmap in the map window
        bool DisplayImage(Bitmap bmp)
        {
            Fault fault = null;

            FormInvoke setImage = new FormInvoke(
                delegate()
                {
                    if (bmp != null)
                        _mapForm.MapImage = bmp;
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