
SimulatedPioneerBumper Service - Revision 2 Readme

Copyright (c) Raul Arrabales - Dec 2007, Sep 2009.

Please, report bugs to raul at conscious-robots.com

________________________________________________________________

HISTORY

Revision 2:
	(27th Sep 2009)
	Service migrated to MRDS 2008 R2. 

Revision 1:
	(11th Dec 2007)
	Initial version of the Pioneer 3DX Simulated bumper.
	Both frontal and rear bumper arrays are simulated.


________________________________________________________________

SimulatedPioneerBumper Readme

This file contains information about the SimulatedPioneerBumper 
service for MRDS, including installation instructions. This code
it is designed to work with MRDS 2008 R2.


Quick Start Instructions

Download the ZIP file and unzip it into your MRDS directory.
Note that this is assumed to be:
C:\Documents and Settings\[UserName]\Microsoft Robotics Dev Studio 2008 R2

When you unzip the file, it creates one project in the
packages\crubots\simulation\sensors directory under your MSRS installation:

 The folder SimulatedPioneerBumper will contain the source code.

If you want to compile the projects yourself, then open
the project and do a rebuild (see the note below first!):

 SimulatedPioneerBumper\SimulatedPioneerBumper.sln


*** IMPORTANT NOTE: ***

In order to have the project references working for your particular settings,
you will need to run DssProjectMigration.exe. For instance (from the MRDS
command prompt):

 bin\DssProjectMigration.exe packages\crubots\simulation\sensors\SimulatedPioneerBumper

The output should look like this:

-------------------------
C:\Documents and Settings\Administrador\Microsoft Robotics Dev Studio 2008 R2>bi
n\DssProjectMigration.exe packages\crubots\simulation\sensors\SimulatedPioneerBu
mper
*   Searching Directory: C:\Documents and Settings\Administrador\Microsoft
    Robotics Dev Studio 2008
    R2\packages\crubots\simulation\sensors\SimulatedPioneerBumper
*     Updating project C:\Documents and Settings\Administrador\Microsoft
      Robotics Dev Studio 2008
      R2\packages\crubots\simulation\sensors\SimulatedPioneerBumper\SimulatedPio

      neerBumper.csproj ...
*     Updating project C:\Documents and Settings\Administrador\Microsoft
      Robotics Dev Studio 2008
      R2\packages\crubots\simulation\sensors\SimulatedPioneerBumper\SimulatedPio

      neerBumper.csproj.user ...
-------------------------


Now you can rebuild the code and run it using the Visual Studio debugger.


________________________________________________________________
Raúl Arrabales Moreno           
http://www.Conscious-Robots.com/raul/

