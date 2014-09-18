
SimulatedPionnerSonar is distributed under the terms of GNU GPL (see license.txt).
CRANIUM (c) 2007-2009. Raúl Arrabales Moreno. (raul at conscious-robots.com)


//------------------------------------------------------------------------------
// Part of MRDS Sample Code - Modified by Raul Arrabales - 27 Jul 07 / Sep 09.
//
// SIMULATED PIONEER SONAR SERVICE
//
// CRANIUM - SIMULATED PIONEER SONAR SENSOR
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
//  $File: SimulatedSonar.cs $ $Revision: 6 $
//------------------------------------------------------------------------------

INSTRUCTIONS

This ZIP file contains the source code and Visual Studio project.
Extract files to MRDS Home path. The directory 
packages\crubots\Simulation\Sensors\SimulatedPioneerSonar
will be created. It contains the source code for the simulated SONAR Service.


This code it is designed to work with MRDS 2008 R2.

Download the ZIP file and unzip it into your MRDS directory.
Note that this is assumed to be:
C:\Documents and Settings\[UserName]\Microsoft Robotics Dev Studio 2008 R2

When you unzip the file, it creates one project in the
packages\crubots\simulation\sensors directory under your MSRS installation:

 The folder SimulatedPioneerSonar will contain the source code.

If you want to compile the projects yourself, then open
the project and do a rebuild (see the note below first!):

 SimulatedPioneerSonar\SimulatedPioneerSonar.sln


*** IMPORTANT NOTE: ***

In order to have the project references working for your particular settings,
you will need to run DssProjectMigration.exe. For instance (from the MRDS
command prompt):

 bin\DssProjectMigration.exe packages\crubots\Simulation\Sensors\SimulatedPioneerSonar

The output should look like this:

-------------------------
C:\Documents and Settings\Administrador\Microsoft Robotics Dev Studio 2008 R2>bi
n\DssProjectMigration.exe packages\crubots\Simulation\Sensors\SimulatedPioneerSo
nar
*   Searching Directory: C:\Documents and Settings\Administrador\Microsoft
    Robotics Dev Studio 2008
    R2\packages\crubots\Simulation\Sensors\SimulatedPioneerSonar
*     Updating project C:\Documents and Settings\Administrador\Microsoft
      Robotics Dev Studio 2008
      R2\packages\crubots\Simulation\Sensors\SimulatedPioneerSonar\SimulatedPion

      eerSonar.csproj ...
*     Updating project C:\Documents and Settings\Administrador\Microsoft
      Robotics Dev Studio 2008
      R2\packages\crubots\Simulation\Sensors\SimulatedPioneerSonar\SimulatedPion

      eerSonar.csproj.user ...
-------------------------


Now you can rebuild the code and run it using the Visual Studio debugger.


________________________________________________________________
Raúl Arrabales Moreno           
http://www.Conscious-Robots.com/raul/


______________________________________________________________________________

HISTORY

revision 6:
	27th Sep 2009
	Migrated to MRDS 2008 R2. Changed name to SimulatedPioneerSonar
	(MRDS comes now with a SimulatedSonar Service).

update5:
	(14th Nov 2007)
	Fixed a bug in the service state initialization and storage.

update4:

	(21st Sep 2007)
	SimulatedSonar Update 4 is available for download. Sonar transducer cone 
	aperture has been changed to 15 degrees to match the real angular aperture 
	of Pioneer 3 DX Sonar transducers. Therefore, total angular range of the 
	sonar ring is 195 degrees (180 plus two cone halves). Note that there are 
	blind spots between sonar transducers.

beta3: 
	(7th Aug 2007) A SonarChangeThreshold variable has been added with a 
	default value of 100.0. Subscriber services are only notified when the
	change in the readings is greater than this threshold. This reduces the
	number of notification received by the subscribers.

beta2:
	Removed SickLRF project from the SimulatedSonar solution. 
	Installation path changed to <MSRS>\Apps\UC3M\SimulatedSonar

beta1:
	First version. Still testing.