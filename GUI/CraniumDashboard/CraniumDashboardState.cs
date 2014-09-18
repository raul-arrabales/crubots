//-----------------------------------------------------------------------
//  This file was part of the Microsoft Robotics Studio Code Samples.
// 
//  Copyright (C) Microsoft Corporation.  All rights reserved.
//
//  $File: ControlaPanelState.cs $ $Revision: 11 $
//
// May-2007:
// Added options for displaying a map
//
// Jun-2007:
// Added a webcam viewer
//
// Aug-2007:
// Cranium ControlPanel branch of Dashboard.
//
// Nov-2007 Raul:
// Removed webcam viewer
//
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.Dss.Core.Attributes;
using drive = Microsoft.Robotics.Services.Drive.Proxy;
// TT - Changed in Oct CTP
//using sicklrf = Microsoft.Robotics.Services.Samples.Drivers.SickLRF.Proxy;
// using sicklrf = Microsoft.Robotics.Services.Sensors.SickLRF.Proxy;


namespace Cranium.Controls
{
    /// <summary>
    /// Dashboard StateType
    /// </summary>
    [DataContract]
    public class StateType
    {
        [DataMember]
        public bool Log;
        [DataMember]
        public string LogFile;
        // TT - Add new data members for the connection parameters
        [DataMember]
        public string Machine;
        [DataMember]
        public ushort Port;
        [DataMember]
        public GUIOptions Options;
    }

    // TT - Version 2 - New class that is an "options bag" for the
    // option settings. An instance is added to the State above.
    [DataContract]
    public class GUIOptions
    {
        // Initial position of the Window
        [DataMember]
        public int WindowStartX;
        [DataMember]
        public int WindowStartY;

        // Dead Zone parameters
        // The "deadzone" is a region where the movement of the
        // joystick has no effect. The implementation here snaps
        // the x or y coordinate to zero when it is within the
        // DeadZoneX or Y range. This allows the robot to drive
        // dead ahead, or to rotate perfectly. The old code did
        // not have an exact center and so the wheel power could
        // never be balanced!
        // For my Lego NXT, I use values of 80 for each of these.
        // This amounts to only a few pixels on the screen (because
        // the "yoke" range is +/- 1000). If you set it too high,
        // you won't get any movement! However, you can set it to
        // zero if you don't want this feature.
        [DataMember]
        public double DeadZoneX;
        [DataMember]
        public double DeadZoneY;
        // Scale Factors
        // Because different robots have different drive
        // characteristics, and their users have different reaction
        // times and/or preferences, there are now two parameters
        // that affect the scaling of the drive power. The Translate
        // Scale Factor adjusts the forward/backward take-up rate,
        // i.e. moving the joystick forwards or backwards (which is
        // up or down on the "yoke" on the screen).
        // The Rotate Scale Factor adjusts the rate for the the
        // left/right (side to side) movements to control the speed
        // of rotation.
        // Usually you want different scale factors for these two
        // types of motions because it is hard to control turns if
        // the robot spins at the same speed that it uses to drives
        // forwards.
        // For my Lego NXT, I use values of 0.8 for Translate and
        // 0.2 for Rotate.
        //
        // NOTE: The results of the motor power calculations are
        // limited to +/- 1000, which translates to +/- 1.0 when
        // sent to the differentialdrive service. This means that
        // if you set Translate to 2.0 for instance, then it will
        // max out halfway between the center and the maximum
        // (top) of the joystick travel. This makes the speed much
        // more responsive to joystick movements. Conversely, you
        // can set a value of 0.5 and then the robot will only ever
        // reach half of its possible drive speed (approximately).
        [DataMember]
        public double TranslateScaleFactor;
        [DataMember]
        public double RotateScaleFactor;
        [DataMember]
        public bool ShowLRF;
        [DataMember]
        public bool ShowArm;

        // Added to support colour-coding of the LRF as per the code
        // supplied by Ben Axelrod
        [DataMember]
        public double RobotWidth;

        // TT - May-2007 Version 6
        // Added to support displaying a top-down map
        [DataMember]
        public bool DisplayMap;
        [DataMember]
        public double MaxLRFRange;
        // TT - May-2007 Version 7
        // Values for use with RotateDegrees and DriveDistance
        // These are really intended for testing, not for actual use
        [DataMember]
        public double MotionSpeed;
        [DataMember]
        public double DriveDistance;
        [DataMember]
        public double RotateAngle;

        // TT - Jun-2007 Version 8
        [DataMember]
        public int CameraInterval;
        
        // Raul - Aug-2007 Version 9
        [DataMember]
        public int SonarRange;

        [DataMember]
        public double[] SonarRadians;

        [DataMember]
        public float SonarTransducerAngularRange;

        [DataMember]
        public bool DisplaySonarMap;

    }

}
