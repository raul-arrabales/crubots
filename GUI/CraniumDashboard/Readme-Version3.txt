Update to SimpleDashboard - Mark III

The code is now too different from the Microsoft SimpleDashboard
to keep maintaining it so I have given it a different name, i.e.
Dashboard. There is a fair bit of "dead code" that has been
commented out now. I really should clean it up ...

I could not resist making even more changes:
When you save the settings it now remembers the window position
so that it will start up again in the same place.

Also, you can now enter negative values for the Translate and
Rotate Scale Factors. This allows you to flip the axes on the
"joystick" control. For some people this might feel more natural.

The other changes were mostly related to changes in some of the
namespaces.

I tried to fix a small bug were the robot would suddenly start
rotating in the opposite direction if you moved the cursor
slightly above or below the horizontal on the trackball. It
is not completely eliminated, but it happens less often.

Note that the Exit item in the File menu does not close down
the instance of DSShost. It will close the Dashboard window,
and actually stops the service. However, the other services
(Joystick, etc) and DSShost keep running, so you still have
to manually kill it. (Type Ctrl-C if you ran it from DOS, or
stop the debugger if you are running from Visual Studio.)
This is annoying, but it has already been noted in the MSRS
Discussion Forum.

Comments and suggestions are (still) welcome.

Trevor Taylor
T.Taylor@qut.edu.au
24th October, 2006
