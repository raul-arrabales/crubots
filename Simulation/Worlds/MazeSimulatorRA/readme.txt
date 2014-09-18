Maze Simulator Readme (RA version)

This file contains information about the Maze Simulator for MSRS,
including installation instructions. Updated for the MRDS 2008 R2
release (Sep 2009).

August 2007 - RA version. Based on a former version by Trevor
Taylor, Update by Raul Arrabales.
MazeSimulatorRA is a slightly modified version of MazeSimulator
that adds a frontal SONAR array in the P3DX robot.


Quick Start Instructions

Download the ZIP file and unzip it into your MRDS root directory.
This should create a directory called:
<MRDS>\packages\crubots\Simulation\Worlds\MazeSimulatorRA

where <MRDS> is the root of your MRDS installation, e.g.
C:\Documents and Settings\[User]\Microsoft Robotics Dev Studio 2008 R2

You need to recompile all of the services. To do this, open
a MRDS command prompt. Then enter the command:

bin\DssProjectMigration.exe packages\crubots\Simulation\Worlds\MazeSimulatorRA


The output should look like this:

-------------------------
C:\Documents and Settings\Administrador\Microsoft Robotics Dev Studio 2008 R2>bi
n\DssProjectMigration.exe packages\crubots\Simulation\Worlds\MazeSimulatorRA
*   Searching Directory: C:\Documents and Settings\Administrador\Microsoft
    Robotics Dev Studio 2008
    R2\packages\crubots\Simulation\Worlds\MazeSimulatorRA
*     Updating project C:\Documents and Settings\Administrador\Microsoft
      Robotics Dev Studio 2008
      R2\packages\crubots\Simulation\Worlds\MazeSimulatorRA\MazeSimulatorRA.cspr

      oj ...
*     Updating project C:\Documents and Settings\Administrador\Microsoft
      Robotics Dev Studio 2008
      R2\packages\crubots\Simulation\Worlds\MazeSimulatorRA\MazeSimulatorRA.cspr

      oj.user ...
--------------------------

Then the service can be built using for instance Visual Studio IDE.


of Apps\QUT\MazeSimulator\QUT_Textures to the directory
<MSRS>\store\media\QUT_Textures. If this is not done, you
will end up with invisible walls (although they are still
there and you can bump into them!)

There is a configuration file, MazeSimulator.Config.xml,
in the Apps\QUT\Config directory. This file uses a sample
maze with a variety of differently coloured walls and
blocks.

If you want to run the program straight away, you can
open an MSRS DOS command prompt window and type:

RunMazeSimulator

This runs a batch file that starts the simulator.

(DOS commands are not case-sensitive, but it looks better
in mixed upper and lower case.)

There is also a shortcut in the MSRS root directory.

NOTE:
The manifest for the Maze Simulator is set up to use my
version of the Dashboard. This has several additional
features compared to the Simple Dashboard provided by
Microsoft. However, it will still work with the Microsoft
Dashboard.


Troubleshooting

Q. When I run the batch file it says "Invalid manifest identifier".
What's wrong?

A. You are running the batch file from the wrong directory.
Consequently dsshost can't find the manifest! Make sure that
you are in the MRS root directory before you run the batch
file. (You should have copied the batch file here when you
installed the Maze Simulator.) Note that the MRS root has to
be the Working Directory in the Debug settings in Visual Studio
Project Properties as well or the debugger won't start up.


Q. What is a manifest anyway?

A. A manifest is an XML file that describes the services that
are required to run an application. You can open it and look
at it -- there is nothing magic about a manifest. However,
it allows applications to be associated with different
services at runtime without changing the code. For instance,
you could change from a real robot to the simulator or vice
versa, provided they both implemented the same types of services.


Q. The Microsoft documentation talks about the config directory.
Do I have to put my manifest in there?

A. No. The command line that you use for dsshost specifies
where the manifest is. Putting it into config is only for
convenience. Also, you should note that the config files
must reside in Apps\QUT\Config, not in samples\config.


Q. When I run the program there is a message in the DOS window
that says: "No physics hardware present, using software physics".
What is wrong?

A. Nothing. This just means that the simulator could not
find a hardware acceleration board for the physics engine.
If you can see the simulation then you can ignore the message.
On the other hand, if your simulation window is blank then
you do not have a suitable graphics card, and this is a problem!


Q. There are no walls! But I can see the laser beams.
What the ...?

A. The bitmap texture files must be copied from the
MSRS\Apps\QUT\MazeSimulator\QUT_Textures directory into
MRS\store\media\QUT_Textures. This should have been done
when you ran RebuildQUTApps.cmd.
You have invisible walls! They are still there -- just
try driving through one. You will also see the laser hits!


Q. I clicked on Connect in the Dashboard, but there is no
list of services. Why?

A. The most likely reason is that you made a typing mistake
in the port number. It is 50001, not 50000. Or maybe you
made a typo in the machine name. You should use localhost.
Unfortunately, the Dashboard just ignores failed connections
and does not tell you.


Q. I have connected in the Dashboard and selected the service,
but the robot does not move when I try to drive it. What's up?

A. Maybe you double-clicked on the Laser Range Finder service
instead of the Differential Drive. Is there a display in the
Laser Range Finder panel and the Disconnect button is enabled?
Just double-click on the Differential Drive service and try
again. You should see the Lag updating. Also, you have to
click on the Drive button before you can make the robot move.


Q. Can I drive with a joystick?

A. I don't know. Do you have a licence?
This is getting silly ... The Dashboard supports DirectX
devices, so if you have a joystick then go for it! I have
tested it using a Logitech Attack 3 USB joystick and it
works fine.


Q. Why do you use port 50000?

A. It's in the Microsoft examples :-)

Seriously, you can use any available port. However, it is highly
recommended that you use a value above 32768 because many of the
ports with low numbers are "well known ports" and serve special
purposes such as FTP, e-mail, web server, etc.


Q. Why is this so complicated?

A. Ask Microsoft :-)



Tips

When dsshost is running, you can examine a lot of details using
a web browser by simply going to the port that was specified in
the command line.

For example, if you used the batch file included with this code
you would browse to:
http://localhost:50000

I strongly recommend that you become familiar with all of the
information available from the dsshost page! What is happening
is that dsshost acts like a web server while it is running.

Become familiar with the Simulator controls. It is hard to
navigate around with the mouse and it takes some practice.


--------------------------------------------------------------------------

Original Readme from Ben Axelrod

This service creates a maze or simple grid world in the Microsoft 
Robotics Studio simulation environment.  It uses an black and white
image to generate the maze.  Every pixel will become one cell of 
the maze.  There are many maze generators online.  These two are 
pretty good: 

http://www.delorie.com/game-room/mazes/genmaze.cgi
http://www.billsgames.com/mazegenerator/

There are several parameters for building the maze:

 * WallBoxSize: size of the wall cubes (in meters). Should be slightly less than GridSpacing.
 * GridSpacing: Maze grid size (in meters).  Adjust these parameters according to the size of your robot.
 * RobotStartCellRow, RobotStartCellCol: Place the robot at these coordinates (upper left corner is (0,0))
 * Maze: The image file and path relative to C:/Microsoft Robotics Studio.  
	This image should be black and white with one pixel per cell.  It can be of format .GIF .BMP or .PNG
 * OptimizeBlocks: Will merge adjacent blocks into 1 long block.  This can significantly improve performance.

For questions and comments, please post to the Robotics Studio Newsgroup.

http://www.microsoft.com/communities/newsgroups/en-us/default.aspx?dg=microsoft.public.msroboticsstudio

--------------------------------------------------------------------------

Updated Version to Set Heights using Colors
Trevor Taylor, QUT
24-Aug-2006

First a few instructions:

When you unZIP the file you need to decide where to put it. I suggest
it goes under the MRS root directory in samples/MazeSimulator.
That's where I put it.

If your MRS installation is NOT on the D: drive (mine is), then you
will have to edit all of the relevant settings in the Project settings
in Visual Studio or it will not build.

The following Project Property settings might have to be changed:
Change Output path in Build
Change Post-Build Events in Build Events
Change Start External Program and Working Directory in Debug
Change Reference Paths
All you need to do (probably) is change D: to C:

Also, you must start Visual Studio from the MRS command prompt using
the devenv command. Don't just double-click on the Solution file in
Windows Explorer. If you do, then then post-build step will not work.
(There are some environment variables that need to be defined.)

If you use the path I suggested for the files, then you can start the
simulator using the following command in an MRS DOS window. (It's here
so you can copy and paste it!)

dsshost.exe -port:50000 -tcpport:50001 -manifest:"samples/MazeSimulator/MazeSimulator.manifest.xml"

NOTE: Your current directory when you run this command must be the
root of the MRS installation. In my case this is:
D:\Microsoft Robotics Studio (September 2006)
The manifest path is relative to here!

You can of course just start the program by running it from inside
Visual Studio if you want to. That's easier, but you might want to
play with it later without starting VS.

To use the Dashboard that starts up automatically, you need to
connect to localhost and port 50001.


How do the colors work?

It's actually quite simple. When the program starts it reads the bitmap
image. Then it uses the color of the top-left pixel as the background,
i.e. the floor. This does mean that your walls can't start in the
top-left, or they become floor and vice versa! However, this is not a
problem. Just put a border around your maze.

The colors in the bitmap are mapped to the basic 8 colors:
Black, Red, Green, Yellow, Blue, Magenta, Cyan, White
I also added Grey. OK, so that's 9 colors ...

The mapping allows you to be imprecise in picking the actual colors.
Look at my examples: ModelSmall.gif and ModelLarge.bmp. Also notice
that you don't have to use BMP files.

With the exception of the background color, all the colors are then
mapped to their corresponding heights in a simple lookup table 
(a byte array). You can change the values to suit yourself.

I have improved the optimization strategy so that the code now builds
large rectangular regions before drawing walls. The more walls you
have, the slower the simulator runs. There is a very heavy penalty
for this, and even with what I consider to be simple mazes my frame
rate drops below 10fps.

Note that when merging wall blocks together, they will not be merged
if they are different heights. Notice that I did NOT say different
colors! Two colors can have the same height.


Final Notes

In the original version that I downloaded there was no gravity and
I could not turn it on. That problem seems to have disappeared.

However, the original version did not make the walls very heavy and
you could easily push them out of the way. So I have made the walls
have infinite mass. Try pushing that you little robot!

You will notice that my sample mazes have diagonal walls. This is
deliberate. However, they generate a heap of separate wall blocks.
One day maybe I will have time to look for these too and optimize
them as well ...

I also decided to center the maze in the world and drop the robot
right into the middle of it. For some mazes, the robot might end up
inside a wall. You can change the code and recompile. That's why you
have the source!

Like the original author, I prefer that you post questions to the
Robotics Studio newsgroup. I'm not trying to remain anonymous, I
just don't have the time to answer a heap of e-mails.

Enjoy!


--------------------------------------------------------------------------

Update 2

Added code to display boxes using colors. Much more pretty!

However, to get this to work you need to copy the BMP files
from the images subdirectory under the Maze Simulator to your
store\media directory in your MRS installation. If you don't
do this, all the walls will disappear and it will look as though
the maze does not exist, except that the LRF scans will appear
in "thin air"! The robot will still bump into the walls :-)


--------------------------------------------------------------------------

Update 3
19-Sep-2006

Updated for the September 2006 CTP.

Used the new features for color of simple objects.

Exposed most of the simulation parameters through the initial
state (config file).


--------------------------------------------------------------------------

Update 4
10-Oct-2006

Updated for the October 2006 CTP. See also Notes-OctoberCTP.txt

For some reason (probably the available free space), MSRS installed
itself on the C: drive this time instead of D: (after three times on
the D: drive in the past). Therefore, note carefully that some of
the details above now have an INCORRECT drive letter.

Increased the number of colours to 16 and changed the colour mapping
code accordingly.

Added MassMap to allow the mass of objects to be changed.

Added UseSphere flags so that spherical objects can be used.

Updated the State to contain all the new info.


--------------------------------------------------------------------------

Update 5
8-Nov-2006

Updated for the November 2006 CTP.

Ran the Migration tool.
Recompiled under the new CTP.
Only one minor change to the code to fix a compilation error.
Checked that it still worked.
Updated the documentation.


--------------------------------------------------------------------------

Update 6
15-Dec-2006

Updated for the Version 1.0 release of MSRS

Ran the Migration tool.
Fixed a couple of problems. In particular, simulation entity
names must not begin with a slash or they will not be created
correctly.
Added a RobotType to the State and also copied the code for
the Lego NXT from Simulation Tutorial 2 so that it is now
possible to create either a Pioneer 3DX or Lego NXT.
Updated the documentation.


--------------------------------------------------------------------------

Updates 7 and 8
18-Jul-2007

Updated for the Version 1.5 release of MSRS

Ran the DssProjectMigration tool.
No major problems (except during with the May CTP,
which was fixed before release).
The default location for config files was changed in V1.5,
so I have moved it to Apps\QUT\Config.

Note that the lighting has changed in V1.5.
Also there seems to be a problem with the collision detection.
The simulated Pioneer robot actually moves inside walls!


--------------------------------------------------------------------------

Updated - RA Version
02-Aug-2007

Updated from MazeSimulator update 8.
August 2007 - RA version.
Update by Raul Arrabales.
MazeSimulatorRA is a slightly modified version of MazeSimulator
that adds a frontal SONAR array instead of LRF in the P3DX robot.


--------------------------------------------------------------------------

Update 9
14-Aug-2007

Updated from MazeSimulator update 8.
August 2007 - Followed Trevor suggestion to avoid having branches.
Update by Raul Arrabales.
MazeSimulatorRA is a slightly modified version of MazeSimulator
that adds the option for a frontal SONAR array instead of LRF in 
the P3DX robot.

Latest changes are:

MazeSimulatorRaTypes.cs:

        // Raul - Aug 2007 - Allow selection between LRF or SONAR.
        // (or both? open to include options for more sensors).
        [DataMember]
        public bool UseLRF;

        [DataMember]
        public bool UseSonar;


MazeSimulatorRa.cs:

            // Raul - Aug 2007 - Indicate what sensors are being used
            _state.UseLRF = false;
            _state.UseSonar = true; 


