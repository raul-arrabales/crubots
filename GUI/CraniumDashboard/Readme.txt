

The Cranium Dashboard service is a version of the Dashboard service 
by Trevor Taylor in which a control for representing the input 
of Sonar has been added. 


// verion 10 - Sep 2009. (MRDS 2008 R2). 
// Updated by Raul Arrbales.

// Version 9 - Aug-2007 (MSRS V1.5) ControlPanel branch of Dashboard
// Added SONAR reading visualization. 
// Updated by Raul Arrabales.


This Dashboard Service provides access to a graphical representation 
of a simulated (or real) frontal SONAR array. The simulated Sonar service uses 
Robotics Studio Simulator raycasting to simulate SONAR transducers 
Using this service you are able to test Simulated Sonar distance measurement in 
the MRDS Visual Environment. See the Simulated Sonar service description for details 
about how measurements are aquired.

More information at:

http://www.conscious-robots.com/en/robotics-studio/robotics-studio-services/control-panel-service-dashb.html 
_____________________________________________________________


CraniumDashboard Serive Readme.

This file contains information about the Cranium Dashboard for MRDS,
including installation instructions. Updated for the MRDS 2008 R2
release (Sep 2009).


Quick Start Instructions

Download the ZIP file and unzip it into your MRDS root directory.
This should create a directory called:
<MRDS>\packages\crubots\GUI\CraniumDashboard

where <MRDS> is the root of your MRDS installation, e.g.
C:\Documents and Settings\[User]\Microsoft Robotics Dev Studio 2008 R2

You need to recompile all of the services. To do this, open
a MRDS command prompt. Then enter the command:

bin\DssProjectMigration.exe packages\crubots\GUI\CraniumDashboard


The output should look like this:

-------------------------
C:\Documents and Settings\Administrador\Microsoft Robotics Dev Studio 2008 R2>bi
n\DssProjectMigration.exe packages\crubots\GUI\CraniumDashboard
*   Searching Directory: C:\Documents and Settings\Administrador\Microsoft
    Robotics Dev Studio 2008 R2\packages\crubots\GUI\CraniumDashboard
*     Updating project C:\Documents and Settings\Administrador\Microsoft
      Robotics Dev Studio 2008
      R2\packages\crubots\GUI\CraniumDashboard\CraniumDashboard.csproj ...
*     Updating project C:\Documents and Settings\Administrador\Microsoft
      Robotics Dev Studio 2008
      R2\packages\crubots\GUI\CraniumDashboard\CraniumDashboard.csproj.user ...
--------------------------

Then the service can be built using for instance Visual Studio IDE.


_____________________________________________________________


Original Readme.txt from Trevor Taylor:
---------------------------------------

Modified Dashboard - Updated for MSRS V1.5

This code is a modified version of the SimpleDashboard that
is supplied with the Microsoft Robotics Studio.

It has several additional features:

1. It remembers settings!
2. You can customise the way that the "trackball" control works.
3. You can display the LRF in a top-down view.
4. It can display a WebCam window.
5. Motion Control buttons that use RotateDegrees and
DriveDistance to control the robot.

I have tested this code using a USB Joystick and it works OK.

It also works with a real web camera or a simulated camera.

The Motion Control buttons rely on functions that are new
in V1.5 for the Simulated Differential Drive. I provide my
own implementation of this service.


Saved Settings

Implicit values:
Window X and Y Position
Connection parameters
Log settings

The following are in the Options dialog:
Dead Zone X and Y
Translate Scale Factor
Rotate Scale Factor
Show LRF
Show Articulated Arm

New with V1.5:
Robot width in 3D LRF view
Display Map (top-down LRF view)
Max Range for LRF
Motion Control buttons with corresponding option settings



Known Problems

When you try to rotate the robot on the spot, you might find
that it starts to rotate in the wrong direction. This is a
"feature" of the way that the speeds are calculated for the
left and right motors. If this happens it will be because
you have moved the cursor below the horizontal line in the
trackball. Move up above the horizontal and it will fix
itself.

The File\Exit menu option closes the Dashboard window but does
not cause DssHost to exit. There is no clean method for doing
this in MSRS yet. (Refer to the Discussion Forum). This will be
corrected once V1.0 of MSRS has shipped.



Trevor Taylor
T.Taylor@qut.edu.au
July 2007
