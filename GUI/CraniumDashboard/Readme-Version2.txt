Update to SimpleDashboard - Mark II

Following a suggestion from Ben Axelrod, I have modified the
Simple Dashboard (yet again!)

This time I did not keep detailed notes of the changes, and
unfortunately these modifications cannot easily be tracked
without a source code management system. This means that the
code has reached the point where it will start to be a
separate generation from the Microsoft code :-(

I think the MSRS community will rapidly tire of "yet another
variation on <insert your service name here>", so this will
soon become just a toy for me to play with.

In summary, here are the changes:

1. Add a menu to the DriveControl form to allow options to be
set and saved.

2. Add a new class to hold a "bag" of option settings. This is
where any future options should be placed. It is held inside
the service state as a single field, which makes it a little
easier to manage. It still serializes OK. Have a look in
SimpleDashboardState.cs.

3. Add a new message type to pass back the option settings to
the main service and request that they be saved.

4. Incorporate some changes to the "joystick" control so that
you can adjust the speed to suit your robot and/or your reaction
time! (I'm getting old, so slower is better for me.) These are
based on the code posted by Ben Axelrod, but I have added a
"dead zone" so that the robot will actually drive straight and
spin perfectly on the spot. Also added adjustable scale factors.
I'm still not 100% happy with the result, but I can now control
my new Lego NXT much better than with the original version.

5. Add some code to reflow the controls on the form so that you
don't have to have the Laser Range Finder or Articulated Arm if
you are not using them. (Can somebody look at the code and tell
me how to do this better? I tried autosize on the form, but that
does not work if you simple hide the panels.) This makes the
basic panel much more compact and easier to fit on the screen.


Also included in the ZIP is a file called runlego.bat. You can
place this into the root directory of your MSRS installation and
run it to start up both the Lego NXT service and the Dashboard.
This makes is quite simple to get going with the NXT.


The following comments have been extracted from the code. They
explain how the new option parameters work.

Dead Zone parameters

The "deadzone" is a region where the movement of the
joystick has no effect. The implementation here snaps
the x or y coordinate to zero when it is within the
DeadZoneX or Y range. This allows the robot to drive
dead ahead, or to rotate perfectly. The old code did
not have an exact center and so the wheel power could
never be balanced!

For my Lego NXT, I use values of 80 for each of these.
This amounts to only a few pixels on the screen (because
the "yoke" range is +/- 1000). If you set it too high,
you won't get any movement! However, you can set it to
zero if you don't want this feature.


Scale Factors

Because different robots have different drive
characteristics, and their users have different reaction
times and/or preferences, there are now two parameters
that affect the scaling of the drive power.

The Translate Scale Factor adjusts the forward/backward
take-up rate, i.e. moving the joystick forwards or backwards
(which is up or down on the "yoke" on the screen).

The Rotate Scale Factor adjusts the rate for the the
left/right (side to side) movements to control the speed
of rotation.

Usually you want different scale factors for these two
types of motions because it is hard to control turns if
the robot spins at the same speed that it uses to drives
forwards.

For my Lego NXT, I use values of 0.8 for Translate and
0.2 for Rotate.

NOTE: The results of the motor power calculations are
limited to +/- 1000, which translates to +/- 1.0 when
sent to the differentialdrive service. This means that
if you set Translate to 2.0 for instance, then it will
max out halfway between the center and the maximum
(top) of the joystick travel. This makes the speed much
more responsive to joystick movements, at least over
half of the range (the rest of the range is useless).
Conversely, you can set a value of 0.5 and then the robot
will only ever reach half of its possible drive speed
(approximately - depends on the robot).


Enjoy!

Comments and suggestions are welcome.

Trevor Taylor
T.Taylor@qut.edu.au
29th September, 2006
