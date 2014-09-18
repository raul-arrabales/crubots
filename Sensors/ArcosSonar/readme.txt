
ArcosSonar is distributed under the terms of GNU GPL (see license.txt).
CRANIUM (c) 2007-2009. Raúl Arrabales Moreno. (raul at conscious-robots.com)

ArcosSonar is a small Robotics Studio Service (DSS Service) that allows
you to access the Pioneer 3 DX Robot frontal SONAR array readings.
The zip file contains the source code and Visual Studio project. 

Decompress the contents of the file under MSRS home directory
and build it using Visual Studio.

Please, notify bugs and comments.
____________________________________________________________


ArcosSonar Readme

This file contains information about the ArcosSonar 
service for MRDS, including installation instructions. This code
it is designed to work with MRDS 2008 R2 and Pioneer 3DX robot.


Quick Start Instructions

Download the ZIP file and unzip it into your MRDS directory.
Note that this is assumed to be:
C:\Documents and Settings\[UserName]\Microsoft Robotics Dev Studio 2008 R2

When you unzip the file, it creates one project in the
packages\crubots\sensors directory under your MSRS installation:

 The folder ArcosSonar will contain the source code.

If you want to compile the projects yourself, then open
the project and do a rebuild (see the note below first!):

 ArcosSonar\ArcosSonar.sln


*** IMPORTANT NOTE: ***

In order to have the project references working for your particular settings,
you will need to run DssProjectMigration.exe. For instance (from the MRDS
command prompt):

 bin\DssProjectMigration.exe packages\crubots\Sensors\ArcosSonar

The output should look like this:

-------------------------
C:\Documents and Settings\Administrador\Microsoft Robotics Dev Studio 2008 R2>bi
n\DssProjectMigration.exe packages\crubots\Sensors\ArcosSonar
*   Searching Directory: C:\Documents and Settings\Administrador\Microsoft
    Robotics Dev Studio 2008 R2\packages\crubots\Sensors\ArcosSonar
*     Updating project C:\Documents and Settings\Administrador\Microsoft
      Robotics Dev Studio 2008
      R2\packages\crubots\Sensors\ArcosSonar\ArcosSonar.csproj ...
*     Updating project C:\Documents and Settings\Administrador\Microsoft
      Robotics Dev Studio 2008
      R2\packages\crubots\Sensors\ArcosSonar\ArcosSonar.csproj.user ...
-------------------------


Now you can rebuild the code and run it using the Visual Studio debugger.

________________________________________________________________
Raúl Arrabales Moreno           
http://www.Conscious-Robots.com/raul/


____________________________________________________________

HISTORY

rev8:
	27th Sep 2009. Migrated to MRDS 2008 R2.
	Service directory has been changed to
	packages\crubots\Sensors\ArcosSonar

beta7:
	14-Aug-2007. ReliableSubscribe operation implemented.

beta6:
	Service directory has been changed to Apps\UC3M\ArcosSonar		

beta5:
	Service ported to Microsoft Robotics Studio 1.5.

beta4:
	Replace notifications are only sent to subscriber services when a given threshold is
	reached in the difference from former SONAR reading. This prevents a flood of messsages,
	which don't really provide new information.

beta3:
	The arcos code service now implements the generic contract for sensor.
	This means that the code that subscribes to this service doesn't need to be changed if the
	underlaying hardware changes.

beta2: 
	ArcosSonarState now contains Sonar readings in a List, so it can be processed much more easily.
	Waiting for feedback from Microsoft about how to implement the generic contract for SONAR.

beta1: 
	Initial version. Not fully tested. Developed for Microsoft Robotics Studio 1.5 May 2007 CTP.
	It is not implementing the generic contract for SONAR. Instead it just defines a minimal interface
	to get SONAR readings from a concrete robot: the Pioneer 3 DX.




Visit www.conscious-robots.com for manuals, API details and more information. 