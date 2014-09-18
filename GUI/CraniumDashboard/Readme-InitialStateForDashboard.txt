Setting up an InitialState Partner for the SimpleDashboard


These instructions outline the steps required to add a saved state to the
SimpleDashboard service. They assume you already know how to use Visual
Studio and are familiar with C#.

Open the SimpleDashboard in Visual Studio.

Open the fil SimpleDashboard.cs.

Find the state variable, which is called _state, and is somewhere
near the top of the service class. (In old code it was common to use
_state as the name and a type of StateType. However, newer services
often use servicenameState as the name and/or type.)

In the SimpleDashboard.cs it will look like this:

class SimpleDashboardService : DsspServiceBase
{
    // shared access to state is protected by the interleave pattern
    // when we activate the handlers
    StateType _state = new StateType();


You cannot create a new State instance this way if you want to use an
InitalState Partner. This is because the partner creates the State.
Unfortunately, if the config file does not exist then it actually sets
the State to null, so your code will crash later unless you change it.
(See below.)


Add a filename (URI) for the name of the config file:

    private const string InitialStateUri = "SimpleDashboard.Config.xml";

NOTE:
This file will be created in the store directory when the state is saved.


Add an attribute to the State and change its initial value to null:

    [InitialStatePartner(Optional = true, ServiceUri = InitialStateUri)]
    StateType _state = null;


You have to decide what information you want to include in the State.
Most of this is probably in the form of global variables inside the
service class. You need to cut these out and paste them into the State
class. State classes are usually in a file called servicenameTypes.cs
or servicenameState.cs.

For SimpleDashboard, it is SimpleDashboardState.cs and the class looks
like the following:

    [DataContract]
    public class StateType
    {
        [DataMember]
        public bool Log;
        [DataMember]
        public string LogFile;
    }

Notice that there are already a couple of fields in the State. We will
need to make sure that these are implemented properly to use them in the
initialization process.

For instance, if Log is true, then we need to open the LogFile immediately
on startup. In the existing code however, it will only be opened if the user
clicks on the browse button and selects a new log file. So some code changes
might be required to support an initial state in an existing service.

We want to add two more fields:
        [DataMember]
        public string Machine;
        [DataMember]
        public ushort Port;

The [DataMember] attribute indicates that this field is serializable,
i.e. it will be included in the saved State.


Now go back to SimpleDashboard.cs and find the Start() method. (It is in the
Startup region.)

At the beginning of the Start method insert code to create the State if it does
not exist and initialize it with the default values. It will only be null if
there was no config file. If you want to write robust code, you should also
validate the contents of the State because the user might have put garbage
into some of the fields in the config file!

protected override void Start()
{
    // Create a default State if no config file exists
    if (_state == null)
    {
        _state = new StateType();
        _state.Log = false;
        _state.LogFile = "";
        _state.Machine = "";
        _state.Port = 0;
    }
...


Because the SimpleDashboard uses a Windows Form, we have to somehow get the
State information to the Form so that it can initialize itself. There is a
routine called CreateForm(). (It is in the WinForms interaction region.)
This passes the event port to the form already, so we can simply add another
parameter, the State, as follows:

    System.Windows.Forms.Form CreateForm()
    {
        // Create the Form and pass it the State
        return new DriveControl(_eventsPort, _state);
    }       



In the Form itself, DriveControl.cs, we need to modify the constructor to
accept the new parameter. The code currently looks like this:

    partial class DriveControl : Form
    {
        DriveControlEvents _eventsPort;

        public DriveControl(DriveControlEvents EventsPort)
        {
            _eventsPort = EventsPort;

            InitializeComponent();
            txtPort.ValidatingType = typeof(ushort);
        }

    ...


Change the code for the constructor to the following:

        public DriveControl(DriveControlEvents EventsPort, StateType state)
        {
            _eventsPort = EventsPort;

            InitializeComponent();
            txtPort.ValidatingType = typeof(ushort);

            // Restore log file settings
            txtLogFile.Text = state.LogFile;
            if (state.Log)
            {
                if (state.LogFile != "")
                {
                    chkLog.Checked = true;
                    // Initialize the log file
                    // Copied code from the Checkbox Click handler
                    txtLogFile.Enabled = !chkLog.Checked;
                    btnBrowse.Enabled = !chkLog.Checked;
                    _eventsPort.Post(new OnLogSetting(this, chkLog.Checked, txtLogFile.Text));
                }
                else
                    chkLog.Checked = false;
            }
            else
                chkLog.Checked = false;

            // Set up the connection parameters
            txtMachine.Text = state.Machine;
            if (state.Port != 0)
                txtPort.Text = state.Port.ToString();
        }


The new code accepts the State as a parameter. It sets the UI components
based on the values in the State, and if necessary it also initializes the
log file. Note, however, that it does not automatically connect. It just
fills in the textboxes. This is because it would be a waste of time if
you wanted to connect to a different DSS host.


The next step in hacking the code is to make sure that the State is saved,
i.e. written out to an XML config file. You can decide when you want this
to happen, but it probably makes sense to do it whenever the Connect button
is pressed or a new log file is opened. So we need to modify two Click event
handlers.

Unfortunately, the State resides in the main service, not in the Windows
Form, so we have to create a new Request type and send a request to save
the state -- you can't do it directly from inside the Form code.

There is already code for OnLogSetting (which was called above in the
initialization if the log file needed to be opened) so we can use this
as a pattern for the new message type.

Locate the OnLogSetting class which is near the bottom of DriveControl.cs.
Insert the following new class:

    class OnConnectSetting : DriveControlEvent
    {
        string _machine;
        ushort _port;

        public string Machine
        {
            get { return _machine; }
            set { _machine = value; }
        }

        public ushort Port
        {
            get { return _port; }
            set { _port = value; }
        }

        public OnConnectSetting(DriveControl form, string machine, ushort port)
            : base(form)
        {
            _machine = machine;
            _port = port;
        }
    }


What we have done is create a new message type that can be sent from the
Form back to the main service.

Now we need to add the new type to the PortSet. Locate the DriveControlEvents
class a little higher up in the file:

    class DriveControlEvents 
        : PortSet<
            OnLoad,
            OnClosed,
            OnChangeJoystick,
            OnConnect,
    ...

You will see that it contains an OnLogSetting. Add another port to the list
called OnConnectSetting, i.e. the new one we just created.

Still in DriveControl.cs, locate the code for btnConnect_Click(). This
handles button clicks on the Connect button.

Just before the bottom of the routine, insert an extra line to send the new
connection parameters to the main service using the OnConnectSetting port that
we have just created:
        ...

        UriBuilder builder = new UriBuilder(Schemes.DsspTcp, machine, port, ServicePaths.InstanceDirectory);

        // Pass the connection settings back and save the state
        _eventsPort.Post(new OnConnectSetting(this, machine, port));

        _eventsPort.Post(new OnConnect(this, builder.ToString()));
    }


The fun is not over yet! Now we can send a message, but it has nowhere to go!
We need a handler in the service to process the OnConnectSetting messages.


In SimpleDashboard.cs, find the region called Drive Control Event Handlers.
(You can just select OnConnectHandler from the Members drop-down list if you
want to. It is in this region of the code.)

Add the following handler:

    void OnConnectSettingHandler(OnConnectSetting onConnectSetting)
    {
        _state.Machine = onConnectSetting.Machine;
        _state.Port = onConnectSetting.Port;
	SaveState(_state);
    }

This is a very simple handler that partially updates the State. Then it
uses the SaveState() method to write out an XML file.

Why not just use a Replace request instead? After all, there is already
a ReplaceHandler(). Basically because log settings are set separately, and
Replace does not save the State, and it is a good exercise to create a new
message type anyway.

Now go back to the Start() method and look for the code that sets up the
handlers for all of the message types:

    Activate(Arbiter.Interleave(
        new TeardownReceiverGroup
        (
            Arbiter.Receive<DsspDefaultDrop>(false, _mainPort, DropHandler)
        ),
        new ExclusiveReceiverGroup
        (
            Arbiter.ReceiveWithIterator<Replace>(true, _mainPort, ReplaceHandler),
            Arbiter.ReceiveWithIterator<OnLoad>(true, _eventsPort, OnLoadHandler),
            Arbiter.Receive<OnClosed>(true, _eventsPort, OnClosedHandler),
            Arbiter.ReceiveWithIterator<OnChangeJoystick>(true, _eventsPort, OnChangeJoystickHandler),
            Arbiter.Receive<OnLogSetting>(true, _eventsPort, OnLogSettingHandler),
            // Add a handler for the connection parameters
            Arbiter.Receive<OnConnectSetting>(true, _eventsPort, OnConnectSettingHandler)
                ),
    ...

You need to add the line for the OnConnectSettingHandler just below the
OnLogSetting. It is not important what order the handlers are listed in,
but it is important that this particular handler is placed in the
ExclusiveReceiverGroup because it modifies the State. Therefore it must
have exclusive access so that two messages are not processed at the
same time resulting in conflicting changes to the State. (In our case here,
this is actually not so important, but in general it is.)


But wait, there's more! We still have not have not saved the State if the
log settings are changed. This is easy. Modify the OnLogSettingHandler()
to save the State:

    void OnLogSettingHandler(OnLogSetting onLogSetting)
    {
        _state.Log = onLogSetting.Log;
        _state.LogFile = onLogSetting.File;
        SaveState(_state);
    ...

This might not be the best idea because the log filename might be invalid
and opening the file might fail. You can look through the code and find
a better place to put the call to SaveState().



Now compile and run your program.

The first time it is run, there will not be a saved state (config) file.
So all the values will default. You won't see any difference.

Connect to a service and maybe open a log file. Then close down the
Dashboard.

Start the Dashboard again, and magically you should see the saved connection
and log parameters reappear!

Now that you have the infrastructure in place, you can add more data
to the State and save it if you find yourself repeatedly entering the
same information into the Dashboard.


NOTES:
When you want to set up a log file, it must be located in a subdirectory of
the store. Just create a "logs" directory in the file browse dialog for
example, then open your log file in there.

Also, the config file is saved into the store directory by default. You
can add a subdirectory to the URI for the config file if you want to keep
it separate from the other files in the store, but you can't choose an
arbitrary path on the computer because this would open a giant security
hole!

And finally, there is batch file, startdashboard.bat, included with the
program in the ZIP file. You need to copy this to the root directory of
your Microsoft Robotics Studio installation. Then you can easily run the
Dashboard. Notice that this batch file uses different port numbers from
the ones normally used in the tutorials. This is deliberate so that you
can have a Dashboard running separately from other services. However, you
can also add the Dashboard to the manifest for another service, e.g. the
Maze Simulator, and have it run inside the same DSS host.


Trevor Taylor
22-Sep-2006
