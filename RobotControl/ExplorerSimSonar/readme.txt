
ExplorerSimSonar Revision 3 Readme

Raul Arrabales - Nov 2008

Please, see original readme below for more details.

This is a modified version of ExplorerSim in
which support for Sonar has been added.

Note all code changes has been flagged as "// Raul - ..."

Please, report bugs to raul at conscious-robots.com


HISTORY

Revision 3:
	(28th Nov 2008)
	Migrated to Robotics Developer Studio 2008.

Revision 2:
	(14th Nov 2007)
	Added a delay of 1 second before initial mapping
	to avoid sonar scans before maze walls are in place.

Revision 1:
	Version of ExplorerSim adapted to Simulated Sonar





________________________________________________________________

Original ExplorerSim Readme

This file contains information about the ExplorerSim for MSRS,
including installation instructions. It is for the official
release of Version 1.5.


Quick Start Instructions

Download the ZIP file and unzip it into your MSRS directory.
Note that this is assumed to be:
C:\Microsoft Robotics Studio (1.5)

When you unzip the file, it creates five projects in the
Apps\QUT directory under your MSRS installation:
Dashboard
DifferentialDriveTT
ExplorerSim
Intro
MazeSimulator

Some batch files and shortcuts are copied into the MSRS root
directory.

Now open a MSRS DOS Command Prompt window and at the DOS
prompt type the following command:

RebuildQUTApps

(DOS commands are not case-sensitive, but it looks better
in mixed upper and lower case. There are no spaces in the
command -- it is the name of a batch file.)

This will recompile the code for all of the services.
It also writes texture files into the store\media
directory.

If you want to compile the projects yourself, then open
each of the projects in turn in the following order and
do a rebuild:
Apps\QUT\DifferentialDriveTT\SimulatedDifferentialDriveTT.sln
Apps\QUT\Dashboard\Dashboard.sln
Apps\QUT\MazeSimulator\MazeSimulator.sln
Apps\QUT\Intro\Intro.sln
Apps\QUT\ExplorerSim\ExplorerSim.sln


When the rebuild has completed, type the following at
the DOS prompt:

RunExplorerSim

You can also run the Maze Simulator without starting the
Explorer by typing:

RunMazeSimulator

I have supplied three shortcuts that can be used instead
of the .bat files. However, if your copy of MSRS is NOT
installed on the C: drive (it automatically selected that
drive for me when I installed it), or it is not in the
default folder you will have to edit the properties of
the shortcuts and change the "Target" and "Start In"
properties to use the correct directory.


IMPORTANT NOTE:

If your MSRS is not on C: or in the default location
then you will need to change the following Project Property
settings in each of the projects:
Change Output Path in Build
Change Post-Build Events in Build Events
Change Start External Program and Working Directory in Debug
Change the Reference Paths

All you probably need to do is change C: to your drive
letter, unless you also installed into it with a different
directory name, and then it is more complicated.

Now you can rebuild the code, and when it has finished you
can run it using the debugger.


Trevor Taylor
Faculty of IT, QUT
July 2007 