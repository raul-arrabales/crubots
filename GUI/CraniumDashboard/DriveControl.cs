//-----------------------------------------------------------------------
//  This file was part of the Microsoft Robotics Studio Code Samples.
// 
//  Copyright (C) Microsoft Corporation.  All rights reserved.
//
//  $File: DriveControl.cs $ $Revision: 9 $
//
// Modifications by Trevor Taylor
// Queensland University of Technology
//
// Modifications by Raul Arrabales
// University Carlos III of Madrid
//
//
// Version 1
// Incorporate changes to allow saving of settings so you don't have
// to re-enter connection and logging parameters every time you run
// the program.
//
// Version 2 - 29-Sep-2006
// Several changes. Unfortunately, changes to the actual form are
// difficult to record. Te GUI has been modified by the addition
// of a menu and some slight re-arranging of the controls to make
// room for the menu.
//
// Version 3 - 10-Oct-2006
// Update to October CTP.
// Change name to Dashboard because it is no longer Simple!
// Also, this avoids a name conflict with the real SimpleDashboard.
// At this stage, the two different code-bases are too hard to keep
// in sync anymore.
// Add the window location to the Options so that it will start up
// in the same place every time.
// Slight modification to the Dead Zone code for smooth take-up
// rather than a sudden jump in speeds from zero to the starting
// value.
//
// Version 4 - 15-Nov-2006
// Updated for November CTP.
// Added an About Box.
//
// Version 5 - 15-Dec-2006
// Updated for Version 1.0 of MSRS (December 2006)
//
// Version 6 - 24-Jan-2007
// Updated with code supplied by Ben Axelrod to colour-code any
// obstacles directly in front of the robot in the LRF display
//
// Version 7 - May-2007
// Added an option to display a map instead of the 3D view for
// the LRF
// Added buttons for RotateDegrees and DriveDistance
//
// Version 8 - Jun-2007
// Added a WebCam Viewer
//
// Version 9 - Aug-2007 (ControlPanel branch of Dashboard).
// Added SONAR support
// 
//
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;
using Microsoft.Ccr.Core;
// TT - Changed in Oct CTP
//using joystick = Microsoft.Robotics.Services.Samples.Drivers.Joystick.Proxy;
// TT - Changed again for V1.0
// Delete reference to Joystick and replace with GameController
//using joystick = Microsoft.Robotics.Services.Sample.Joystick.Proxy;
using joystick = Microsoft.Robotics.Services.GameController.Proxy;
using drive = Microsoft.Robotics.Services.Drive.Proxy;
using arm = Microsoft.Robotics.Services.ArticulatedArm.Proxy;
// TT Jun-2007
using webcam = Microsoft.Robotics.Services.WebCam.Proxy;
using sicklrf = Microsoft.Robotics.Services.Sensors.SickLRF.Proxy;
using cs = Microsoft.Dss.Services.Constructor;
using Microsoft.Dss.ServiceModel.Dssp;
using Microsoft.Dss.ServiceModel.DsspServiceBase;
using System.Drawing.Drawing2D;
using System.IO;
using Microsoft.Dss.Core;
using Microsoft.Robotics.Simulation.Physics.Proxy;

using Microsoft.Robotics.Simulation.Engine;
using Microsoft.Robotics.PhysicalModel;

// Raul - Generic contract for Sonar
using pxsonar = Microsoft.Robotics.Services.Sonar.Proxy;

using pxPanTilt = Cera.Generic.PanTilt.Proxy;

using pxGPS = Cranium.Simulation.Sensors.SimulatedPioneerGPS.Proxy;

// Raul - Generic contract for bumper
using pxbumper = Microsoft.Robotics.Services.ContactSensor.Proxy;


using System.Runtime.InteropServices;


namespace Cranium.Controls
{
    partial class DriveControl : Form
    {
        DriveControlEvents _eventsPort;


        #region Raul - Sept 2007 - Form Images

        // Main camera image
        private Bitmap _cameraImage;

        public Bitmap CameraImage
        {
            get { return _cameraImage; }
            set
            {
                _cameraImage = value;

                Image old = picCamImage.Image;
                picCamImage.Image = value;

                if (old != null)
                {
                    old.Dispose();
                }
            }
        }


        #endregion


        // TT - New variables for the option settings and
        // reformatting of the form
        GUIOptions options;
        int LRFPanelBaseY, LRFPanelHeight;
        int FormBaseHeight;

        // TT - Make this a variable
        // Height of the LRF image bitmap
        // NOTE: The width of the picturebox must be at least
        // twice this height so that the map will display correctly
        int LRFImageHeight = 100;


        // TT - Change the parameters for Form constructor
        public DriveControl(DriveControlEvents EventsPort, StateType state)
        {
            _eventsPort = EventsPort;

            InitializeComponent();
            simulImage.MouseMove += new MouseEventHandler(simulImage_MouseMove);
            simulImage.MouseDown += new MouseEventHandler(simulImage_MouseDown);
            _eventsPort.PostUnknownType(new OnSimulLoaded(this, this));


            txtPort.ValidatingType = typeof(ushort);

            // TT - Add initialization code based on the saved State
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
                    _eventsPort.PostUnknownType(new OnLogSetting(this, chkLog.Checked, txtLogFile.Text));
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

            // TT - Version 2 added all the options below
            options = new GUIOptions();
            options = state.Options;

            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(options.WindowStartX, options.WindowStartY);

            // The dead zone can't be negative, but it can be zero
            options.DeadZoneX = Math.Abs(state.Options.DeadZoneX);
            options.DeadZoneY = Math.Abs(state.Options.DeadZoneY);

            // Just in case the scale factors have not been initialized
            if (state.Options.TranslateScaleFactor == 0)
                options.TranslateScaleFactor = 1.0;
            else
                options.TranslateScaleFactor = state.Options.TranslateScaleFactor;
            if (state.Options.RotateScaleFactor == 0)
                options.RotateScaleFactor = 0.5;
            else
                options.RotateScaleFactor = state.Options.RotateScaleFactor;

            LRFPanelBaseY = groupBoxLRF.Top;
            LRFPanelHeight = LRFPanelBaseY;
            FormBaseHeight = this.Height - LRFPanelHeight;
            ReformatForm();
        }


        Vector2 _mouseDown;
        void simulImage_MouseDown(object sender, MouseEventArgs e)
        {
            _mouseDown = new Vector2(e.X, e.Y);
        }

        void simulImage_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _eventsPort.PostUnknownType(new OnSimulDrag(null, new Vector2(e.X - _mouseDown.X, e.Y - _mouseDown.Y)));
                _mouseDown = new Vector2(e.X, e.Y);
            }
            else if (e.Button == MouseButtons.Right)
            {
                _eventsPort.PostUnknownType(new OnSimulZoom(null, new Vector2(e.X - _mouseDown.X, e.Y - _mouseDown.Y)));
                _mouseDown = new Vector2(e.X, e.Y);
            }
        }


        private void ReformatForm()
        {
            int height = FormBaseHeight;

            groupBoxLRF.Top = LRFPanelBaseY;
            groupBoxLRF.Visible = true;
            height += LRFPanelHeight;
            this.Height = height;
        }

        private void cbJoystick_SelectedIndexChanged(object sender, EventArgs e)
        {
            IEnumerable<joystick.Controller> list = cbJoystick.Tag as IEnumerable<joystick.Controller>;

            if (list != null)
            {
                if (cbJoystick.SelectedIndex >= 0)
                {
                    int index = 0;
                    foreach (joystick.Controller controller in list)
                    {
                        if (index == cbJoystick.SelectedIndex)
                        {
                            OnChangeJoystick change = new OnChangeJoystick(this);
                            change.Joystick = controller;

                            _eventsPort.PostUnknownType(change);

                            return;
                        }
                        index++;
                    }
                }
            }
        }

/*        
        private void cbJoystick_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbJoystick.Tag is joystick.JoystickInstance[])
            {
                joystick.JoystickInstance[] list = (joystick.JoystickInstance[])cbJoystick.Tag;

                if (cbJoystick.SelectedIndex >= 0 && 
                    cbJoystick.SelectedIndex < list.Length)
                {
                    OnChangeJoystick change = new OnChangeJoystick(this);
                    change.Joystick = list[cbJoystick.SelectedIndex];

                    _eventsPort.Post(change);
                }
            }
        }
*/

        private void DriveControl_Load(object sender, EventArgs e)
        {
            _eventsPort.PostUnknownType(new OnLoad(this));
        }

        private void DriveControl_FormClosed(object sender, FormClosedEventArgs e)
        {
            _eventsPort.PostUnknownType(new OnClosed(this));
        }

        public void ReplaceJoystickList(IEnumerable<joystick.Controller> controllers)
        {
            cbJoystick.BeginUpdate();
            try
            {
                cbJoystick.Items.Clear();
                foreach (joystick.Controller controller in controllers)
                {
                    int index = cbJoystick.Items.Add(controller.InstanceName);
                    if (controller.Current == true)
                    {
                        cbJoystick.SelectedIndex = index;
                    }
                }
                cbJoystick.Tag = controllers;
            }
            finally
            {
                cbJoystick.EndUpdate();
            }
        }

/*        
        public void ReplaceJoystickList(joystick.StateType data)
        {
            cbJoystick.BeginUpdate();
            try
            {
                cbJoystick.Items.Clear();
                foreach (joystick.JoystickInstance instance in data.Available)
                {
                    cbJoystick.Items.Add(instance.Name);
                    if (data.Current != null &&
                        instance.Guid == data.Current.Guid)
                    {
                        cbJoystick.SelectedIndex = cbJoystick.Items.Count - 1;
                    }
                }
                cbJoystick.Tag = data.Available;
            }
            finally
            {
                cbJoystick.EndUpdate();
            }
        }
*/

        public void UpdateJoystickAxes(joystick.Axes axes)
        {
            int x = axes.X;
            int y = -axes.Y;

            lblX.Text = x.ToString();
            lblY.Text = y.ToString();
            lblZ.Text = axes.Z.ToString();

            DrawJoystick(x, y);

            /*
            if (!chkStop.Checked)
            {
                int left;
                int right;

                if (chkDrive.Checked == true)
                {
                    if (y > -100)
                    {
                        left = y + axes.X / 4;
                        right = y - axes.X / 4;
                    }
                    else
                    {
                        left = y - axes.X / 4;
                        right = y + axes.X / 4;
                    }
                }
                else
                {
                    left = right = 0;
                }
                _eventsPort.Post(new OnMove(this, left, right));
            }
             */

            // TT - Version 2 - New code
            if (!chkStop.Checked)
            {
                double left;
                double right;

                if (chkDrive.Checked == true)
                {
                    //double x, y;
                    //this is the raw length of the vector
                    double magnitude = Math.Sqrt(x * x + y * y);

                    //x = data.X;
                    //y = data.Y;
                    // Check for the "dead zone"
                    // TT - Version 3
                    // Added some code so that the speed values would
                    // not suddenly jump after leaving the Dead Zone
                    if (Math.Abs(x) < options.DeadZoneX)
                        x = 0;
                    else
                    {
                        // Subtract off the dead zone value so that the
                        // coord starts from zero
                        if (x > 0)
                            x -= (int) options.DeadZoneX;
                        else
                            x += (int) options.DeadZoneX;
                    }
                    if (Math.Abs(y) < options.DeadZoneY)
                        y = 0;
                    else
                    {
                        if (y > 0)
                            y -= (int) options.DeadZoneY;
                        else
                            y += (int) options.DeadZoneY;
                    }

                    if (x == 0 && y == 0)
                    {
                        // Totally dead in the middle!
                        left = right = 0;
                    }
                    else
                    {
                        //angle of the vector
                        double theta = Math.Atan2(y, x);

                        //this is the maximum magnitude for a given angle
                        //                    double maxMag;
                        double scaledMag = 1.0;
                        /*
                        // Sorry Ben, I did not understand why you did this
                        // and the 1000 cancels out anyway if you look
                        // carefully at the code.
                        if (Math.Abs(data.X) > Math.Abs(data.Y))
                            scaledMag = magnitude * Math.Abs(Math.Cos(theta));
    //                        maxMag = Math.Abs(1000 / Math.Cos(theta));
                        else
                            scaledMag = magnitude * Math.Abs(Math.Sin(theta));
    //                        maxMag = Math.Abs(1000 / Math.Sin(theta));
                        */
                        //a scaled down magnitude according to above
                        //                    double scaledMag = magnitude * 1000 / maxMag;
                        scaledMag = magnitude;
                        //decompose the vector into motor components
                        // What is the significance of 150? The cross-hairs
                        // cross over at 0, so this just meant that the
                        // cross-over point was slightly below the center
                        // of the yoke ...
                        // NOTE: There is a peculiar problem that if you
                        // try to rotate the robot on the spot, and you
                        // are not careful to keep the cursor on the
                        // horizontal center line, then the robot might
                        // suddenly start rotating in the OPPOSITE direction!
                        // This is a quirk of the maths. It happens if the
                        // cursor moves outside the dead zone on the bottom
                        // (negative) side. This might be the reason for the
                        // -150 in the original code, i.e. to try to avoid
                        // this problem.
                        //if (data.Y > -150)
                        if (y >= 0)
                        {
                            left = scaledMag * options.TranslateScaleFactor * Math.Sin(theta) + scaledMag * options.RotateScaleFactor * Math.Cos(theta);
                            right = scaledMag * options.TranslateScaleFactor * Math.Sin(theta) - scaledMag * options.RotateScaleFactor * Math.Cos(theta);
                        }
                        else
                        {
                            left = scaledMag * options.TranslateScaleFactor * Math.Sin(theta) - scaledMag * options.RotateScaleFactor * Math.Cos(theta);
                            right = scaledMag * options.TranslateScaleFactor * Math.Sin(theta) + scaledMag * options.RotateScaleFactor * Math.Cos(theta);
                        }
                    }
                }
                else
                {
                    left = right = 0;
                }

                //cap at 1000
                left = Math.Min(left, 1000);
                right = Math.Min(right, 1000);
                left = Math.Max(left, -1000);
                right = Math.Max(right, -1000);
                // Quick and dirty way to display results for debugging -
                // Uncomment the two lines below
                //                Console.WriteLine("Joy: " + data.X + ", " + data.Y
                //                            + " => " + left + ", " + right);
                _eventsPort.PostUnknownType(new OnMove(this, (int)Math.Round(left), (int)Math.Round(right)));
            }
            // End of changes

        }

        public void UpdateJoystickButtons(joystick.Buttons buttons)
        {
            if (buttons.Pressed != null && buttons.Pressed.Count > 0)
            {
                string[] buttonString = buttons.Pressed.ConvertAll<string>(
                    delegate(bool button)
                    {
                        return button ? "X" : "O";
                    }
                ).ToArray();

                lblButtons.Text = string.Join(" ", buttonString);

                if (chkStop.Checked && buttons.Pressed.Count > 2)
                {
                    if (buttons.Pressed[2] == true)
                    {
                        chkStop.Checked = false;
                    }
                }
                else if (buttons.Pressed[1] == true && buttons.Pressed.Count > 1)
                {
                    chkStop.Checked = true;
                }

                if (buttons.Pressed[0] != chkDrive.Checked)
                {
                    chkDrive.Checked = buttons.Pressed[0];
                }
            }

        }

/*
        public void UpdateJoystick_old(joystick.StateType data)
        {
            lblX.Text = data.X.ToString();
            lblY.Text = data.Y.ToString();
            lblZ.Text = data.Z.ToString();


            DrawJoystick(data.X, data.Y);

            if (data.Buttons != null)
            {
                string[] buttons =
                    Array.ConvertAll<bool, string>(data.Buttons,
                        delegate(bool button)
                        {
                            return button ? "X" : "O";
                        });

                lblButtons.Text = string.Join(" ", buttons);

                if (chkStop.Checked)
                {
                    if (data.Buttons[2] == true)
                    {
                        chkStop.Checked = false;
                    }
                }
                else if (data.Buttons[1] == true)
                {
                    chkStop.Checked = true;
                }
                if (data.Buttons[0] != chkDrive.Checked)
                {
                    chkDrive.Checked = data.Buttons[0];
                }
            }

            // TT - Version 2 - Start of changes from Ben Axelrod
            // Remove the old code and incorporate new code
//            if (!chkStop.Checked)
//            {
//                int left;
//                int right;
//
//                if (chkDrive.Checked == true)
//                {
//                    if (data.Y > -100)
//                    {
//                        left = data.Y + data.X / 4;
//                        right = data.Y - data.X / 4;
//                    }
//                    else
//                    {
//                        left = data.Y - data.X / 4;
//                        right = data.Y + data.X / 4;
//                    }
//                }
//                else
//                {
//                    left = right = 0;
//                }
//                _eventsPort.Post(new OnMove(this, left, right));
//            }

            // TT - Version 2 - New code
            if (!chkStop.Checked)
            {
                double left;
                double right;

                if (chkDrive.Checked == true)
                {
                    double x, y;
                    //this is the raw length of the vector
                    double magnitude = Math.Sqrt(data.Y * data.Y + data.X * data.X);

                    x = data.X;
                    y = data.Y;
                    // Check for the "dead zone"
                    if (Math.Abs(x) < options.DeadZoneX)
                        x = 0;
                    if (Math.Abs(y) < options.DeadZoneY)
                        y = 0;

                    if (x == 0 && y == 0)
                    {
                        // Totally dead in the middle!
                        left = right = 0;
                    }
                    else
                    {
                        //angle of the vector
                        double theta = Math.Atan2(y, x);

                        //this is the maximum magnitude for a given angle
                        //                    double maxMag;
                        double scaledMag = 1.0;

                        // Sorry Ben, I did not understand why you did this
                        // and the 1000 cancels out anyway if you look
                        // carefully at the code.
//                        if (Math.Abs(data.X) > Math.Abs(data.Y))
//                            scaledMag = magnitude * Math.Abs(Math.Cos(theta));
//                        maxMag = Math.Abs(1000 / Math.Cos(theta));
//                        else
//                            scaledMag = magnitude * Math.Abs(Math.Sin(theta));
//                        maxMag = Math.Abs(1000 / Math.Sin(theta));

                        //a scaled down magnitude according to above
                        //                    double scaledMag = magnitude * 1000 / maxMag;
                        scaledMag = magnitude;
                        //decompose the vector into motor components
                        // What is the significance of 150? The cross-hairs
                        // cross over at 0, so this just meant that the
                        // cross-over point was slightly below the center
                        // of the yoke ...
                        //if (data.Y > -150)
                        if (y > 0)
                        {
                            left = scaledMag * options.TranslateScaleFactor * Math.Sin(theta) + scaledMag * options.RotateScaleFactor * Math.Cos(theta);
                            right = scaledMag * options.TranslateScaleFactor * Math.Sin(theta) - scaledMag * options.RotateScaleFactor * Math.Cos(theta);
                        }
                        else
                        {
                            left = scaledMag * options.TranslateScaleFactor * Math.Sin(theta) - scaledMag * options.RotateScaleFactor * Math.Cos(theta);
                            right = scaledMag * options.TranslateScaleFactor * Math.Sin(theta) + scaledMag * options.RotateScaleFactor * Math.Cos(theta);
                        }
                    }
                }
                else
                {
                    left = right = 0;
                }

                //cap at 1000
                left = Math.Min(left, 1000);
                right = Math.Min(right, 1000);
                left = Math.Max(left, -1000);
                right = Math.Max(right, -1000);
                // Quick and dirty way to display results for debugging
//                Console.WriteLine("Joy: " + data.X + ", " + data.Y
//                            + " => " + left + ", " + right);
                _eventsPort.Post(new OnMove(this, (int)Math.Round(left), (int)Math.Round(right)));
            }
            // End of changes
        }
*/

        private void DrawJoystick(int x, int y)
        {
            Bitmap bmp = new Bitmap(picJoystick.Width, picJoystick.Height);
            Graphics g = Graphics.FromImage(bmp);

            int width = bmp.Width - 1;
            int height = bmp.Height - 1;

            g.Clear(Color.Transparent);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            GraphicsPath path = new GraphicsPath();
            path.AddEllipse(0, 0, width, height);

            PathGradientBrush pathBrush = new PathGradientBrush(path);
            pathBrush.CenterPoint = new PointF(width / 3f, height / 3f);
            pathBrush.CenterColor = Color.White;
            pathBrush.SurroundColors = new Color[] { Color.LightGray };
            
            g.FillPath(pathBrush, path);
            g.DrawPath(Pens.Black, path);

            int partial = y * height / 2200;
            if (partial > 0)
            {
                g.DrawArc(Pens.Black,
                    0,
                    height / 2 - partial,
                    width,
                    2 * partial,
                    180,
                    180);
            }
            else if (partial == 0)
            {
                g.DrawLine(Pens.Black, 0, height / 2, width, height / 2);
            }
            else
            {
                g.DrawArc(Pens.Black,
                    0,
                    height / 2 + partial,
                    width,
                    -2 * partial,
                    0,
                    180);
            }

            partial = x * width / 2200;
            if (partial > 0)
            {
                g.DrawArc(Pens.Black,
                    width / 2 - partial,
                    0,
                    2 * partial,
                    height,
                    270,
                    180);
            }
            else if (partial == 0)
            {
                g.DrawLine(Pens.Black, width / 2, 0, width / 2, height);
            }
            else
            {
                g.DrawArc(Pens.Black,
                    width / 2 + partial,
                    0,
                    -2 * partial,
                    height,
                    90,
                    180);
            }

            picJoystick.Image = bmp;
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            string machine = txtMachine.Text;
            
            if (machine.Length == 0)
            {
                txtMachine.Focus();
                return;
            }

            object obj = txtPort.ValidateText();

            if (obj == null)
            {
                obj = (ushort)0;
            }

            ushort port = (ushort)obj;

            UriBuilder builder = new UriBuilder(Schemes.DsspTcp, machine, port, ServicePaths.InstanceDirectory);

            // TT - Add a message to save the connection parameters
            _eventsPort.PostUnknownType(new OnConnectSetting(this, machine, port));

            _eventsPort.PostUnknownType(new OnConnect(this, builder.ToString()));
        }

        public void ReplaceDirectoryList(ServiceInfoType[] list)
        {
            listDirectory.BeginUpdate();
            try
            {
                listDirectory.Tag = list;
                listDirectory.Items.Clear();

                if (list.Length > 0)
                {
                    UriBuilder node = new UriBuilder(list[0].Service);
                    node.Path = null;
                    lblNode.Text = node.Host + ":" + node.Port;
                    txtMachine.Text = node.Host;
                    txtPort.Text = node.Port.ToString();

                    linkDirectory.Enabled = true;
                }
                else
                {
                    lblNode.Text = string.Empty;
                    linkDirectory.Enabled = false;
                }

                foreach (ServiceInfoType info in list)
                {
                    if (info.Contract == sicklrf.Contract.Identifier ||
                        info.Contract == drive.Contract.Identifier ||
                        info.Contract == arm.Contract.Identifier ||
                        info.Contract == pxsonar.Contract.Identifier ||
                        info.Contract == pxbumper.Contract.Identifier ||
                        info.Contract == pxPanTilt.Contract.Identifier ||
                        info.Contract == webcam.Contract.Identifier ||
                        info.Contract == pxGPS.Contract.Identifier )
                        // info.Contract == vision.Contract.Identifier) 

                    {
                        Uri serviceUri = new Uri(info.Service);
                        listDirectory.Items.Add(serviceUri.AbsolutePath);
                    }
                }

                if (ServiceByContract(sicklrf.Contract.Identifier) == null)
                {
                    btnStartLRF.Enabled = true;
                    btnConnectLRF.Enabled = false;
                }
                else
                {
                    btnStartLRF.Enabled = false;
                    btnConnectLRF.Enabled = true;
                }


                if (ServiceByContract(pxsonar.Contract.Identifier) == null)
                {
                    btnStartSonar.Enabled = true;
                    btnConnectSonar.Enabled = false;
                }
                else
                {
                    btnStartSonar.Enabled = false;
                    btnConnectSonar.Enabled = true;
                }

                if (ServiceByContract(webcam.Contract.Identifier) == null)
                {
                    btnCamConnect.Enabled = false;
                    btnCamDisconnect.Enabled = false;
                }
                else
                {
                    btnCamConnect.Enabled = true;                    
                }
                /*
                if (ServiceByContract(vision.Contract.Identifier) == null)
                {
                    btnConnectVision.Enabled = false;
                    btnDisconnectVision.Enabled = false;
                }
                 
                else
                {
                    btnConnectVision.Enabled = true;
                    btnDisconnectVision.Enabled = false;
                }
                */
            }
            finally
            {
                listDirectory.EndUpdate();
            }
        }

        private void linkDirectory_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (listDirectory.Tag is ServiceInfoType[])
            {
                ServiceInfoType[] list = (ServiceInfoType[])listDirectory.Tag;

                if (list.Length > 0)
                {
                    UriBuilder node;

                    if (list[0].AliasList != null &&
                        list[0].AliasList.Count > 0)
                    {
                        node = new UriBuilder(list[0].AliasList[0]);
                    }
                    else
                    {
                        node = new UriBuilder(list[0].Service);
                    }

                    node.Path = "directory";
                    System.Diagnostics.Process.Start(node.ToString());
                }
            }
        }

        string ServiceByContract(string contract)
        {
            if (listDirectory.Tag is ServiceInfoType[])
            {
                ServiceInfoType[] list = (ServiceInfoType[])listDirectory.Tag;

                foreach (ServiceInfoType service in list)
                {
                    if (service.Contract == contract)
                    {
                        return service.Service;
                    }
                }
            }
            return null;
        }

        private void btnStartLRF_Click(object sender, EventArgs e)
        {
            OnStartService start = new OnStartService(this, sicklrf.Contract.Identifier);
            start.Constructor = ServiceByContract(cs.Contract.Identifier);

            if (start.Constructor != null)
            {
                _eventsPort.PostUnknownType(start);
            }
        }

        private void btnConnectLRF_Click(object sender, EventArgs e)
        {
            string lrf = ServiceByContract(sicklrf.Contract.Identifier);
            if (lrf != null)
            {
                _eventsPort.PostUnknownType(new OnConnectSickLRF(this, lrf));
            }
        }

        public void StartedSickLRF()
        {
            btnConnect_Click(this, EventArgs.Empty);
        }

        private void chkStop_CheckedChanged(object sender, EventArgs e)
        {
            if (chkStop.Checked)
            {
                _eventsPort.PostUnknownType(new OnEStop(this));
            }
        }

        DateTime _lastLaser = DateTime.Now;

        // TT May-2007 - Draw an occupancy grid style map using the LRF data
        private Bitmap DrawMap(sicklrf.State stateType)
        {
            double angle;
            int range;

            Bitmap bmp = new Bitmap(stateType.DistanceMeasurements.Length, LRFImageHeight);
            Graphics g = Graphics.FromImage(bmp);
            g.Clear(Color.LightGray);

            // Loop invariants
            double startAngle = stateType.AngularRange / 2.0;
            // AngularResolution is zero! This is a bug that I have
            // reported and it should be fixed in V1.5. In the meantime,
            // calculate the value.
            double angleIncrement = (double)stateType.AngularRange / (double)stateType.DistanceMeasurements.Length;
            double rx, ry;
            int ix, iy;
//            Pen whitePen = new Pen(Color.White);
//            Pen blackPen = new Pen(Color.Black, 2);
//            blackPen.EndCap = LineCap.Round;

            // First draw the free space
            // Note that this code draws the free space using a series
            // of white "rays" from the LRF. The result is some small
            // "slivers" that are not filled in between the rays.
            // Some people fill in the whole area outlined by the
            // hit points on the assumption that if two rays are close
            // together then there is probably nothing in between.
            // Strictly speaking this is not correct because the LRF
            // tells you nothing about the spaces between rays.
            for (int x = 0; x < stateType.DistanceMeasurements.Length; x++)
            {
                range = stateType.DistanceMeasurements[x];
                // NOTE: Scans are backwards, i.e. right to left
                angle = (startAngle - x * angleIncrement) * Math.PI / 180;
                // The Simulated LRF returns zero if there is no hit
                // This is not the way that a real LRF works
                if (range <= 0)
                    range = (int) options.MaxLRFRange;
                rx = range * Math.Cos(angle);
                ry = range * Math.Sin(angle);
                ix = (int)(ry * LRFImageHeight / options.MaxLRFRange) + bmp.Width / 2;
                iy = bmp.Height - (int)(rx * LRFImageHeight / options.MaxLRFRange);
                // NOTE: This code relies on the fact that the DrawLine
                // method will clip at the edges of the bitmap!
                g.DrawLine(Pens.White, bmp.Width / 2, bmp.Height - 1, ix, iy);
            }

            // Now draw the obstacles at the points of the laser hits
            for (int x = 0; x < stateType.DistanceMeasurements.Length; x++)
            {
                range = stateType.DistanceMeasurements[x];
                // NOTE: Scans are backwards, i.e. right to left
                angle = (startAngle - x * angleIncrement) * Math.PI / 180;
                // The Simulated LRF returns zero if there is no hit
                // This is not the way that a real LRF works
                if (range <= 0)
                    range = (int)options.MaxLRFRange;
                rx = range * Math.Cos(angle);
                ry = range * Math.Sin(angle);
                ix = (int)(ry * LRFImageHeight / options.MaxLRFRange) + bmp.Width / 2;
                iy = bmp.Height - (int)(rx * LRFImageHeight / options.MaxLRFRange);
                if (range < options.MaxLRFRange)
                {
                    /*
                    // We need a small dot at the end of the laser ray
                    // However, drawing a point with DrawLine does not work!
                    // So make it a little longer
                    int ix2, iy2;
                    ix2 = ix;
                    if (ix2 > bmp.Width / 2)
                        ix2 = ix - 1;
                    if (ix2 < bmp.Width / 2)
                        ix2 = ix + 1;
                    iy2 = iy;
                    if (iy2 < bmp.Height / 2)
                        iy2 = iy + 1;
                    g.DrawLine(blackPen, ix, iy, ix2, iy2);
                    */

                    // Put a single pixel at the end of the ray
                    // This does not give a nice dark boundary like
                    // drawing a short line, but it is more correct
                    // because technically the laser only gives data
                    // at discrete angles and you can't say anything
                    // about the area in between rays
                    // NOTE: We have to check the bounds of the image
                    // for SetPixel -- it does not clip
                    if (ix >=0 && ix < bmp.Width &&
                        iy >=0 && iy < bmp.Height)
                        bmp.SetPixel(ix, iy, Color.Black);
                }
            }

            // Return the image
            return bmp;
        }


        #region Raul - Sept 2007 - Draw Vision Stuff

        /*
         
        DateTime lastFace = new DateTime(1999, 1, 1);
        DateTime lastLeftHand = new DateTime(1999, 1, 1);
        DateTime lastRightHand = new DateTime(1999, 1, 1);
        DateTime lastObject = new DateTime(1999, 1, 1);


        public void DrawMotionImage(Bitmap image)
        {
            this.MotionImage = image;
            
            TimeSpan span = DateTime.Now - lastFace;
            if (span.Seconds > 1)
            {
                picVisionFace.Visible = false;
            }
            
            span = DateTime.Now - lastLeftHand;
            if (span.Seconds > 1)
            {
                picLeftHand.Visible = false;
            }

            span = DateTime.Now - lastRightHand;
            if (span.Seconds > 1)
            {
                picRightHand.Visible = false;
            }

            span = DateTime.Now - lastObject;
            if (span.Seconds > 1)
            {
                picVisionObject.Visible = false;
            }

        }

        public void DrawFace(vision.FaceResult face)
        {
            if (face.HeadFound)
            {
                picVisionFace.Visible = true;
                lastFace = DateTime.Now;
            }        
        }


        public void DrawHand(vision.HandGestureResult hand)
        {
            if (hand.LeftHandGestureFound)
            {
                if (hand.LeftHandGestureFound)
                {
                    picLeftHand.Visible = true;
                    lastLeftHand = DateTime.Now;
                }
                if ( hand.RightHandGestureFound)
                {
                    picRightHand.Visible = true;
                    lastRightHand = DateTime.Now;
                }
            }
        }

        public void DrawObject(vision.ObjectResult vobject)
        {
            if (vobject.ObjectFound)
            {
                picVisionObject.Visible = true;
                lastObject = DateTime.Now;
            }
        }
        */
        #endregion

        // Raul - Aug 2007 - Draw an occupancy grid style map using the SONAR data
        private Bitmap DrawSonarMap(pxsonar.SonarState stateType)
        {
            // Graphic scale factor (milimeters to pixels)
            const int ScaleFactor = 40;
            const int bottomMargin = 10;

            float scaled_maxRange = options.SonarRange / ScaleFactor;
            float half_cone = options.SonarTransducerAngularRange / 2;

            // Set the dimension of the local map image
            Bitmap bmp = new Bitmap(
                (options.SonarRange * 2) / ScaleFactor,   // Width
                (options.SonarRange / ScaleFactor) );     // Height          

            Graphics g = Graphics.FromImage(bmp);
            g.Clear(Color.LightGray);

            // This is only valid for a single 8 transducer array so far
            // !!!!
            if (stateType.DistanceMeasurements.Length > 8)
            {              
                return bmp;
            }

            // Draw the free space. Same as noted by Trevor in DrawLRFMap.
            // "slivers" that are not filled in between the cones.
            double angle;
            double cone_angles;
            double scaled_range;
            double rx, ry;
            int ix, iy;            
            float x3=0, y3=0, x4=0, y4=0;
            Pen fatPen = new Pen(Color.White, 2.0f);            

            // For each Sonar transducer
            for (int i = 0; i < stateType.DistanceMeasurements.Length; i++)
            {                
                scaled_range = stateType.DistanceMeasurements[i] / ScaleFactor;

                angle = options.SonarRadians[i];

                // Draw current cone
                for (float j = -half_cone; j <= half_cone; j+=1.0f)
                {
                    cone_angles = options.SonarRadians[i] + j * Math.PI / 180;
                    rx = scaled_range * Math.Cos(cone_angles);
                    ry = scaled_range * Math.Sin(cone_angles);

                    ix = bmp.Width - bmp.Width / 2 - (int)(ry * scaled_maxRange / scaled_maxRange);
                    iy = bmp.Height - bottomMargin - (int)(rx * scaled_maxRange / scaled_maxRange);

                    // get the borders of this cone
                    if (j == -half_cone) // center of the cone
                    {
                        x3 = ix;
                        y3 = iy;
                    }
                    if (j == half_cone)
                    {
                        x4 = ix;
                        y4 = iy;
                    }
                    
                    g.DrawLine(fatPen,
                        bmp.Width / 2,
                        bmp.Height - bottomMargin,
                        ix,
                        iy);

                }

                // Draw obstacles at the points of Sonar hits
                if (scaled_range < scaled_maxRange)
                {
                    g.DrawLine(Pens.Black, x3, y3, x4, y4);
                }


            }    
                   
            // Return the image
            return bmp;
        }



        // TT - Draw the pseudo 3D view including Ben's code to show
        // the width of the robot
        private Bitmap Draw3DView(sicklrf.State stateType)
        {
            // TT - Some minor rearrangement of Ben's code
            int robotWidth = (int)options.RobotWidth; // mm
            double angle, obsThresh;
            Color col;

            Bitmap bmp = new Bitmap(stateType.DistanceMeasurements.Length, LRFImageHeight);
            Graphics g = Graphics.FromImage(bmp);
            g.Clear(Color.LightGray);

            int half = bmp.Height / 2;

            for (int x = 0; x < stateType.DistanceMeasurements.Length; x++)
            {
                int range = stateType.DistanceMeasurements[x];
                if (range > 0 && range < options.MaxLRFRange)
                {
                    int h = bmp.Height * 500 / stateType.DistanceMeasurements[x];
                    if (h < 0)
                    {
                        h = 0;
                    }
                    // TT - Ignore the robot width if it is not set
                    if (robotWidth > 0)
                    {
                        angle = x * Math.PI * stateType.AngularRange /
                                stateType.DistanceMeasurements.Length / 180; // radians
                        obsThresh = Math.Abs(robotWidth / (2 * Math.Cos(angle)));
                    }
                    else
                        obsThresh = 0.0;

                    if (range < obsThresh)
                        col = LinearColor(Color.DarkRed, Color.LightGray, 0, (int)options.MaxLRFRange, range);
                    else
                        col = LinearColor(Color.DarkBlue, Color.LightGray, 0, (int)options.MaxLRFRange, range);

                    g.DrawLine(new Pen(col), bmp.Width - x, half - h, bmp.Width - x, half + h);
                }
            }

            // Return the image
            return bmp;
        }



        // Raul - August 2007. Draw SONAR ring readings
        private Bitmap DrawSonarRing(pxsonar.SonarState stateType)
        {
            // Graphic scale factor (milimeters to pixels)
            // This is appropiate for 8 sonar transducers (just one sonar array)!
            const int ScaleFactor = 40;
            const int bottomMargin = 10;
            float half_cone_rads = (float)((options.SonarTransducerAngularRange / 2) * Math.PI) / 180;
            Color col;

            Bitmap bmp = new Bitmap( 
                (options.SonarRange * 2)/ScaleFactor,                  // Width
                (options.SonarRange/ScaleFactor) + bottomMargin );     // Height           

            Graphics g = Graphics.FromImage(bmp);
            g.Clear(Color.White);

            float x1 = bmp.Width / 2;
            float y1 = bmp.Height - bottomMargin;


            if (stateType.DistanceMeasurements.Length != options.SonarRadians.Length)
            {
                // This code needs to be improved. Graphical representation depends on Sonar Array arrangement.
            }
            else
            {
                lblSonarS0.Text = String.Format("S0: {0}", (int)stateType.DistanceMeasurements[0]);
                lblSonarS1.Text = String.Format("S1: {0}", (int)stateType.DistanceMeasurements[1]);
                lblSonarS2.Text = String.Format("S2: {0}", (int)stateType.DistanceMeasurements[2]);
                lblSonarS3.Text = String.Format("S3: {0}", (int)stateType.DistanceMeasurements[3]);
                lblSonarS4.Text = String.Format("S4: {0}", (int)stateType.DistanceMeasurements[4]);
                lblSonarS5.Text = String.Format("S5: {0}", (int)stateType.DistanceMeasurements[5]);
                lblSonarS6.Text = String.Format("S6: {0}", (int)stateType.DistanceMeasurements[6]);
                lblSonarS7.Text = String.Format("S7: {0}", (int)stateType.DistanceMeasurements[7]);

                for (int i = 0; i < stateType.DistanceMeasurements.Length; i++)
                {
                    int range = (int)stateType.DistanceMeasurements[i];

                    if (range > 0 && range <= options.SonarRange)
                    {
                        float scaled_range = (float)range / ScaleFactor;
                        col = LinearColor(Color.DarkBlue, Color.LightGray, 0, (int)options.SonarRange, range);
                        Pen myPen = new Pen(col);
                        
                        // X = scaled_range * Sin( transducer_orientation )
                        float x2 = bmp.Width - x1 - scaled_range * (float)Math.Sin(options.SonarRadians[i]);

                        // Y = scaled_range * Cos (transduCer_orientation)                                                        
                        float y2 = y1 - (scaled_range * (float)Math.Cos(options.SonarRadians[i]));

                        float m3 = scaled_range * (float)Math.Cos(half_cone_rads); // Side of the Cone = range * Cos(8 degrees)
         
                        float y3 = y1 - (m3 * (float)Math.Cos(options.SonarRadians[i] - half_cone_rads));
                        float x3 = x1 - (m3 * (float)Math.Sin(options.SonarRadians[i] - half_cone_rads));

                        float y4 = y1 - (m3 * (float)Math.Cos(options.SonarRadians[i] + half_cone_rads));
                        float x4 = x1 - (m3 * (float)Math.Sin(options.SonarRadians[i] + half_cone_rads));

                        // Raul - Here I draw all the lines
                        // You can comment out the lines you don't want to have in the Control Panel representation

                        // Draw the 2D cone bisector
                        g.DrawLine(new Pen(Color.OrangeRed), 
                            x1,      // I consider all rays coming out from a single point.
                            y1,      // and from the bottom of the image.
                            x2, 
                            y2);

                        // Draw right frontal range
                        g.DrawLine(myPen, x2, y2, x3, y3);

                        // Draw left frontal range
                        g.DrawLine(myPen, x2, y2, x4, y4);

                        // Draw right side of the cone
                        g.DrawLine(myPen, x1, y1, x3, y3);

                        // Draw left side of the cone
                        g.DrawLine(myPen, x1, y1, x4, y4);
                    }
                }
            }
            // Return the image
            return bmp;
        }




// TT Nov-2006 - Changed for new CTP
//        public void ReplaceLaserData(sicklrf.StateType stateType)
        public void ReplaceLaserData(sicklrf.State stateType)
        {
            if (stateType.TimeStamp < _lastLaser)
            {
                return;
            }
            _lastLaser = stateType.TimeStamp;
            TimeSpan delay = DateTime.Now - stateType.TimeStamp;
            lblDelay.Text = delay.ToString();

            // TT - Changes supplied by Ben Axelrod to highlight
            // any objects immediately in front of the robot made
            // the following code redundant
            /*
            for (int x = 0; x < stateType.DistanceMeasurements.Length; x++)
            {
                int range = stateType.DistanceMeasurements[x];
                if (range > 0 && range < 8192)
                {
                    int h = bmp.Height * 500 / stateType.DistanceMeasurements[x];
                    if (h < 0)
                    {
                        h = 0;
                    }
                    Color col = LinearColor(Color.DarkBlue, Color.LightGray, 0, 8192, range);
                    g.DrawLine(new Pen(col), bmp.Width - x, half - h, bmp.Width - x, half + h);
                }
            }
            */

            // TT - May-2007
            // Added an option to display a map instead of 3D view
            if (options.DisplayMap)
            {
                // Display a top-down map using the colour convention
                // for an occupancy grid
                picLRF.Image = DrawMap(stateType);
            }
            else
            {
                // Display the simulated 3D view
                picLRF.Image = Draw3DView(stateType);
            }

            if (btnConnectLRF.Enabled)
            {
                btnConnectLRF.Enabled = false;
            }
            if (!btnDisconnect.Enabled)
            {
                btnDisconnect.Enabled = true;
            }
        }

        // Raul - Bumpers Update code
        #region Bumpers Update

        /// <summary>
        /// Update the UI with the state of bumpers
        /// </summary>
        /// <param name="bumpersState"></param>
        public void ReplaceBumpersData(pxbumper.ContactSensorArrayState bumpersState)
        {
            foreach (pxbumper.ContactSensor bumper in bumpersState.Sensors)
            {
                UpdateBumperButton(bumper.Name, bumper.Pressed);
            }
        }

        /// <summary>
        /// Update the UI with the estat of a single bumper
        /// </summary>
        /// <param name="bumperState"></param>
        public void UpdateBumperData(pxbumper.ContactSensor bumperState)
        {
            UpdateBumperButton(bumperState.Name, bumperState.Pressed);
        }

        /// <summary>
        /// Updates the UI button corresponding to the specified bumper.
        /// </summary>
        /// <param name="bumperName"></param>
        /// <param name="pressed"></param>
        private void UpdateBumperButton(string bumperName, bool pressed)
        {
            Color buttonColor;
            if (pressed)
            {
                buttonColor = Color.Red;
            }
            else
            {
                buttonColor = Color.Green;
            }
            
            if (bumperName.Contains("b10"))
            {
                btn_b10.BackColor = buttonColor;
            }
            else if (bumperName.Contains("b11"))
            {
                btn_b11.BackColor = buttonColor; ;
            }
            else if (bumperName.Contains("b12"))
            {
                btn_b12.BackColor = buttonColor; ;
            }
            else if (bumperName.Contains("b13"))
            {
                btn_b13.BackColor = buttonColor;
            }
            else if (bumperName.Contains("b1"))
            {
                btn_b1.BackColor = buttonColor;
            }
            else if (bumperName.Contains("b2"))
            {
                btn_b2.BackColor = buttonColor;
            }
            else if (bumperName.Contains("b3"))
            {
                btn_b3.BackColor = buttonColor;
            }
            else if (bumperName.Contains("b4"))
            {
                btn_b4.BackColor = buttonColor;
            }
            else if (bumperName.Contains("b5"))
            {
                btn_b5.BackColor = buttonColor;
            }
            else if (bumperName.Contains("b9"))
            {
                btn_b9.BackColor = buttonColor;
            }
        }


        #endregion


        // Raul - Replace GPS Data

        public void ReplaceGPSData(pxGPS.SimulatedPioneerGPSState gpsState)
        {
            lblGPSX.Text = String.Format("{0:n}", gpsState.X);
            lblGPSY.Text = String.Format("{0:n}", gpsState.Y);
            lblGPSZ.Text = String.Format("{0:n}", gpsState.Z);
            lblGPSAngle.Text = String.Format("{0}", gpsState.Theta);
            lblGPStime.Text = gpsState.TimeStamp.ToLongTimeString() + " " + gpsState.TimeStamp.Millisecond;            
        }
        
         
        // Raul - Replace Sonar Data

        DateTime _lastSonar = DateTime.Now;

        public void ReplaceSonarData(pxsonar.SonarState stateType)
        {            
            if (stateType.TimeStamp < _lastSonar)
            {
                return;
            }
            _lastSonar = stateType.TimeStamp;
            TimeSpan delay = DateTime.Now - stateType.TimeStamp;
            lblSonarDelay.Text = delay.ToString();

            // Raul - Sept 2007
            if (options.DisplaySonarMap)
            {
                picSonar.Image = DrawSonarMap(stateType);
            }
            else
            {
                picSonar.Image = DrawSonarRing(stateType);
            }

            if (btnConnectSonar.Enabled)
            {
                btnConnectSonar.Enabled = false;
            }
            if (!btnDisconnectSonar.Enabled)
            {
                btnDisconnectSonar.Enabled = true;
            }
        }


        private Color LinearColor(Color nearColor, Color farColor, int nearLimit, int farLimit, int value)
        {
            if (value <= nearLimit)
            {
                return nearColor;
            }
            else if (value >= farLimit)
            {
                return farColor;
            }

            int span = farLimit - nearLimit;
            int pos = value - nearLimit;

            int r = (nearColor.R * (span - pos) + farColor.R * pos) / span;
            int g = (nearColor.G * (span - pos) + farColor.G * pos) / span;
            int b = (nearColor.B * (span - pos) + farColor.B * pos) / span;

            return Color.FromArgb(r, g, b);
        }


       
        


        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            // Raul - SickLRF commented out!!
            // _eventsPort.Post(new OnDisconnectSickLRF(this));
            btnConnectLRF.Enabled = true;
            btnDisconnect.Enabled = false;
        }

        DateTime _lastMotor = DateTime.Now;

        public void UpdateMotorData(drive.DriveDifferentialTwoWheelState data)
        {
            if (data.TimeStamp > _lastMotor)
            {
                _lastMotor = data.TimeStamp;
                TimeSpan lag = DateTime.Now - data.TimeStamp;

            }
        }

        private void chkLog_CheckedChanged(object sender, EventArgs e)
        {
            txtLogFile.Enabled = !chkLog.Checked;
            btnBrowse.Enabled = !chkLog.Checked;
            _eventsPort.PostUnknownType(new OnLogSetting(this, chkLog.Checked, txtLogFile.Text));
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            saveFileDialog.InitialDirectory = LayoutPaths.RootDir + LayoutPaths.StoreDir;
            saveFileDialog.FileName = txtLogFile.Text;
            if (DialogResult.OK == saveFileDialog.ShowDialog(this))
            {
                txtLogFile.Text = saveFileDialog.FileName;
            }
        }

        public void ErrorLogging(Exception ex)
        {
            MessageBox.Show(this, 
                "Exception thrown by logging:\n" + ex.Message, 
                Text, 
                MessageBoxButtons.OK, 
                MessageBoxIcon.Exclamation
            );

            chkLog.Checked = false;
            txtLogFile.Enabled = true;
            btnBrowse.Enabled = true;
        }


        private void picJoystick_MouseLeave(object sender, EventArgs e)
        {
            UpdateJoystickButtons(new joystick.Buttons());
            UpdateJoystickAxes(new joystick.Axes());
        }

        private void picJoystick_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int x, y;
                x = Math.Min(picJoystick.Width, Math.Max(e.X, 0));
                y = Math.Min(picJoystick.Height, Math.Max(e.Y, 0));

                x = x * 2000 / picJoystick.Width - 1000;
                y = y * 2000 / picJoystick.Height - 1000;

                joystick.Axes axes = new joystick.Axes();
                axes.X = x;
                axes.Y = y;

                UpdateJoystickAxes(axes);
            }
        }

        private void picJoystick_MouseUp(object sender, MouseEventArgs e)
        {
            picJoystick_MouseLeave(sender, e);
        }

/*        
        private void picJoystick_MouseLeave(object sender, EventArgs e)
        {
            joystick.StateType dummy = new joystick.StateType();

            UpdateJoystick(dummy);
        }

        private void picJoystick_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                int x, y;
                x = Math.Min(picJoystick.Width, Math.Max(e.X, 0));
                y = Math.Min(picJoystick.Height, Math.Max(e.Y, 0));

                x = x * 2000 / picJoystick.Width - 1000;
                y = 1000 - y * 2000 / picJoystick.Height;

                joystick.StateType dummy = new joystick.StateType();
                dummy.X = x;
                dummy.Y = y;

                UpdateJoystick(dummy);
            }
        }

*/
        public void PerformedRoundTrip(bool roundTrip)
        {
            string title = roundTrip ? "Remote Drive Control" : "Remote Drive Control - Connection Down";

            if (Text != title)
            {
                Text = title;
            }
        }

        private void listDirectory_DoubleClick(object sender, EventArgs e)
        {
            ServiceInfoType[] list = listDirectory.Tag as ServiceInfoType[];

            if (list != null &&
                listDirectory.SelectedIndex >= 0 &&
                listDirectory.SelectedIndex < list.Length)
            {
                ServiceInfoType info = FindServiceInfoFromServicePath((string)listDirectory.SelectedItem);
                if (info == null)
                    return;
                if (info.Contract == drive.Contract.Identifier)
                {
                    _eventsPort.PostUnknownType(new OnConnectMotor(this, info.Service));
                }
                else if (info.Contract == sicklrf.Contract.Identifier)
                {
                    _eventsPort.PostUnknownType(new OnConnectSickLRF(this, info.Service));
                }
                else if (info.Contract == arm.Contract.Identifier)
                {
                    _eventsPort.PostUnknownType(new OnConnectArticulatedArm(this, info.Service));
                }
                    // TT - Webcam support
                else if (info.Contract == webcam.Contract.Identifier)
                {
                    _eventsPort.PostUnknownType(new OnConnectWebCam(this, info.Service));
                }
                    // Raul - Sonar Support
                else if (info.Contract == pxsonar.Contract.Identifier)
                {
                    _eventsPort.PostUnknownType(new OnConnectSonar(this, info.Service));
                }
                    // Raul - Bumpers Support
                else if (info.Contract == pxbumper.Contract.Identifier)
                {
                    _eventsPort.PostUnknownType(new OnConnectBumpers(this, info.Service));
                }
                    // Raul - GPS
                else if (info.Contract == pxGPS.Contract.Identifier)
                {
                    _eventsPort.PostUnknownType(new OnConnectGPS(this, info.Service));
                }
                else if (info.Contract == pxPanTilt.Contract.Identifier)
                {
                    _eventsPort.PostUnknownType(new OnConnectPanTilt(this, info.Service));
                    btnDown.Enabled = true;
                    btnUp.Enabled = true;
                    btnLeft.Enabled = true;
                    btnReset.Enabled = true;
                    btnRight.Enabled = true;
                }

            }
        }

        ServiceInfoType FindServiceInfoFromServicePath(string path)
        {
            ServiceInfoType[] list = (ServiceInfoType[])listDirectory.Tag;

            UriBuilder builder = new UriBuilder(list[0].Service);
            builder.Path = path;

            string uri = builder.ToString();

            return FindServiceInfoFromServiceUri(uri);
        }

        ServiceInfoType FindServiceInfoFromServiceUri(string uri)
        {
            ServiceInfoType[] list = (ServiceInfoType[])listDirectory.Tag;

            foreach (ServiceInfoType si in list)
            {
                if (si.Service == uri)
                    return si;
            }
            return null;
        }

        private void saveFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            string path = Path.GetFullPath(saveFileDialog.FileName);
            if (!path.StartsWith(saveFileDialog.InitialDirectory))
            {
                MessageBox.Show("Log file must be in a subdirectory of the store", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Cancel = true;
            }
        }



        private void saveSettingsMenuItem_Click(object sender, EventArgs e)
        {
            OnOptionSettings opt = new OnOptionSettings(this, options);
            opt.Options.WindowStartX = this.Location.X;
            opt.Options.WindowStartY = this.Location.Y;
            _eventsPort.PostUnknownType(opt);
        }

        private void optionsMenuItem_Click(object sender, EventArgs e)
        {
            GUIOptions opt = options;
            Options optDialog = new Options(ref opt);
            DialogResult result = optDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                options = opt;
                ReformatForm();
            }
        }

        private void exitMenuItem_Click(object sender, EventArgs e)
        {
            _eventsPort.PostUnknownType(new OnClosed(this));
            this.Close();
        }

        // TT Nov 2006 - Added an About Box to make it easier to
        // figure out which version is being used
        private void aboutMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox about = new AboutBox();
            about.ShowDialog();
        }

        // TT May-2007 - Added buttons for DriveDistance and RotateDegrees
        private void btnLeft_Click(object sender, EventArgs e)
        {
            _eventsPort.PostUnknownType(new OnRotate(this, options.RotateAngle, options.MotionSpeed));
        }

        private void btnRight_Click(object sender, EventArgs e)
        {
            _eventsPort.PostUnknownType(new OnRotate(this, -options.RotateAngle, options.MotionSpeed));
        }

        private void btnForward_Click(object sender, EventArgs e)
        {
            _eventsPort.PostUnknownType(new OnTranslate(this, options.DriveDistance/1000, options.MotionSpeed));
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            _eventsPort.PostUnknownType(new OnEStop(this));
        }

        private void btnReverse_Click(object sender, EventArgs e)
        {
            _eventsPort.PostUnknownType(new OnTranslate(this, - options.DriveDistance / 1000, options.MotionSpeed));
        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        // Raul - Disconnect Sonar
        private void btnDisconnectSonar_Click(object sender, EventArgs e)
        {
            _eventsPort.PostUnknownType(new OnDisconnectSonar(this));
            btnConnectSonar.Enabled = true;
            btnDisconnectSonar.Enabled = false;
        }

        private void btnConnectSonar_Click(object sender, EventArgs e)
        {         
            string sonar = ServiceByContract(pxsonar.Contract.Identifier);
            if (sonar != null)
            {
                _eventsPort.PostUnknownType(new OnConnectSonar(this, sonar));
            }
        }

        private void btnStartSonar_Click(object sender, EventArgs e)
        {
            OnStartService start = new OnStartService(this, pxsonar.Contract.Identifier);
            start.Constructor = ServiceByContract(cs.Contract.Identifier);

            if (start.Constructor != null)
            {
                _eventsPort.PostUnknownType(start);
            }        
        }

        // Raul - Sept 2007
        private void btnCamConnect_Click(object sender, EventArgs e)
        {
            string  webcamStr = ServiceByContract(webcam.Contract.Identifier);
            if (webcamStr != null)
            {
                _eventsPort.PostUnknownType(new OnConnectWebCam(this, webcamStr));
                // Can't disconnect from camera, so I disabled the disconnect button 
                btnCamDisconnect.Enabled = false;
                btnCamConnect.Enabled = false;
            }
        }

        // Raul - Sept 2007
        private void btnCamDisconnect_Click(object sender, EventArgs e)
        {
            _eventsPort.PostUnknownType(new OnDisconnectWebCam(this));
            btnCamConnect.Enabled = true;
            btnCamDisconnect.Enabled = false;
        }

        private void listDirectory_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        // Raul - Sept 2007
        /*
        private void btnConnectVision_Click(object sender, EventArgs e)
        {
            string visionStr = ServiceByContract(vision.Contract.Identifier);
            if (visionStr != null)
            {
                _eventsPort.PostUnknownType(new OnConnectVision(this, visionStr));
                btnDisconnectVision.Enabled = true;
                btnConnectVision.Enabled = false;
            }
        }

        // Raul - Sept 2007
        private void btnDisconnectVision_Click(object sender, EventArgs e)
        {
            _eventsPort.PostUnknownType(new OnDisconnectVision(this));
            btnConnectVision.Enabled = true;
            btnDisconnectVision.Enabled = false;        
        }
         */

        private void label14_Click(object sender, EventArgs e)
        {

        }

        private void picVisionFace_Click(object sender, EventArgs e)
        {

        }

        private void groupBox3_Enter_1(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void label20_Click(object sender, EventArgs e)
        {

        }

        private void label23_Click(object sender, EventArgs e)
        {

        }


        #region Embedded Simulator UI

        public void SetHeadless(bool headless)
        {
            checkBoxHideUI.Checked = headless;
            this.Refresh();
        }

        public void SetCameraImage(Bitmap bmp)
        {
            simulImage.Image = bmp;
            simulImage.Invalidate();
        }


        #endregion

        private void checkBoxHideUI_CheckedChanged(object sender, EventArgs e)
        {
            SimulatorConfiguration config = new SimulatorConfiguration(true);
            config.Headless = checkBoxHideUI.Checked;
            SimulationEngine.GlobalInstancePort.Update(config);
        }

        private void chkDrive_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void btnUp_Click(object sender, EventArgs e)
        {            
            _eventsPort.PostUnknownType(new OnPTMove(this, 
                pxPanTilt.PanTiltOperationRequest.OpType.MoveUp));
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            _eventsPort.PostUnknownType(new OnPTMove(this,
                pxPanTilt.PanTiltOperationRequest.OpType.MoveDown));
        }

        private void btnLeft_Click_1(object sender, EventArgs e)
        {
            _eventsPort.PostUnknownType(new OnPTMove(this,
                pxPanTilt.PanTiltOperationRequest.OpType.MoveLeft));
        }

        private void btnRight_Click_1(object sender, EventArgs e)
        {
            _eventsPort.PostUnknownType(new OnPTMove(this,
                    pxPanTilt.PanTiltOperationRequest.OpType.MoveRight));
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            _eventsPort.PostUnknownType(new OnPTMove(this,
                pxPanTilt.PanTiltOperationRequest.OpType.Reset));
        }

        private void groupBox6_Enter(object sender, EventArgs e)
        {

        }

    }



    // Raul - Aug 2007 - Changed from PortSet<> to dynamic runtime PortSet type in order to 
    // support more than 20 ports
    public class DriveControlEvents : PortSet
    {
        public DriveControlEvents()
            : base(
                typeof(OnLoad),
                typeof(OnClosed),
                typeof(OnChangeJoystick),
                typeof(OnConnect),
                typeof(OnConnectMotor),
                typeof(OnConnectSonar),
                typeof(OnConnectGPS),
                typeof(OnConnectBumpers),
                typeof(OnConnectSickLRF),
                typeof(OnConnectArticulatedArm),
                typeof(OnConnectWebCam),
                typeof(OnConnectVision),
                typeof(OnDisconnectVision),
                typeof(OnStartService),
                typeof(OnMove),
                typeof(OnPTMove),
                typeof(OnRotate),
                typeof(OnTranslate),
                typeof(OnEStop),
                typeof(OnApplyJointParameters),
                typeof(OnDisconnectSonar),
                typeof(OnDisconnectGPS),
                typeof(OnDisconnectBumpers),
                typeof(OnDisconnectSickLRF),
                typeof(OnDisconnectWebCam),
                typeof(OnConnectPanTilt),
                typeof(OnLogSetting),
                typeof(OnQueryFrame),
                typeof(OnConnectSetting),
                typeof(OnOptionSettings),
                typeof(OnSimulLoaded),
                typeof(OnSimulDrag),
                typeof(OnSimulZoom))
            {
            }
    }

    

    class DriveControlEvent
    {
        private DriveControl _driveControl;

        public DriveControl DriveControl
        {
            get { return _driveControl; }
            set { _driveControl = value; }
        }

        public DriveControlEvent(DriveControl driveControl)
        {
            _driveControl = driveControl;
        }
    }


    class OnSimulLoaded : DriveControlEvent
    {
        private object _object;
        public object Object
        {
            get { return _object; }
            set { _object = value; }
        }

        public OnSimulLoaded(DriveControl form, object objectParam)
            :base(form)
        {
            _object = objectParam;
        }
    }

    class OnSimulDrag : DriveControlEvent
    {
        private object _object;
        public object Object
        {
            get { return _object; }
            set { _object = value; }
        }

        public OnSimulDrag(DriveControl form, object objectParam)
            : base(form)
        {
            _object = objectParam;
        }
    }

    class OnSimulZoom : DriveControlEvent
    {
        private object _object;
        public object Object
        {
            get { return _object; }
            set { _object = value; }
        }

        public OnSimulZoom(DriveControl form, object objectParam)
            : base(form)
        {
            _object = objectParam;
        }
    }

    class OnLoad : DriveControlEvent
    {
        public OnLoad(DriveControl form)
            : base(form)
        {
        }
    }

    class OnConnect : DriveControlEvent
    {
        string _service;

        public string Service
        {
            get { return _service; }
            set { _service = value; }
        }

        public OnConnect(DriveControl form, string service)
            : base(form)
        {
            _service = service;
        }
    }

    class OnConnectMotor : OnConnect
    {
        public OnConnectMotor(DriveControl form, string service)
            : base(form, service)
        {
        }
    }

    class OnConnectSickLRF : OnConnect
    {
        public OnConnectSickLRF(DriveControl form, string service)
            : base(form, service)
        {
        }
    }

    // Raul - OnConnectSonar
    class OnConnectSonar : OnConnect
    {
        public OnConnectSonar(DriveControl form, string service)
            : base(form, service)
        {
        }
    }

    class OnConnectGPS : OnConnect
    {
        public OnConnectGPS(DriveControl form, string service)
            : base(form, service)
        {
        }
    }

        
    class OnConnectBumpers : OnConnect
    {
        public OnConnectBumpers(DriveControl form, string service)
            : base(form, service)
        {
        }
    }

    class OnConnectArticulatedArm : OnConnect
    {
        public OnConnectArticulatedArm(DriveControl form, string service)
            : base(form, service)
        {
        }
    }

    class OnConnectSimulatedArm : OnConnect
    {
        public OnConnectSimulatedArm(DriveControl form, string service)
            : base(form, service)
        {
        }
    }

    class OnStartService : DriveControlEvent
    {
        string _contract;
        string _constructor;

        public string Contract
        {
            get { return _contract; }
            set { _contract = value; }
        }

        public string Constructor
        {
            get { return _constructor; }
            set { _constructor = value; }
        }


        public OnStartService(DriveControl form, string contract)
            : base(form)
        {
            _contract = contract;
        }
    }

    class OnClosed : DriveControlEvent
    {
        public OnClosed(DriveControl form)
            : base(form)
        {
        }
    }

    class OnChangeJoystick : DriveControlEvent
    {
        joystick.Controller _joystick;

        public joystick.Controller Joystick
        {
            get { return _joystick; }
            set { _joystick = value; }
        }

        public OnChangeJoystick(DriveControl form)
            : base(form)
        {
        }
    }

/*
        class OnChangeJoystick : DriveControlEvent
        {
            joystick.JoystickInstance _joystick;

            public joystick.JoystickInstance Joystick
            {
                get { return _joystick; }
                set { _joystick = value; }
            }

            public OnChangeJoystick(DriveControl form)
                : base(form)
            {
            }
        }
    */


    class OnPTMove : DriveControlEvent
    {
        pxPanTilt.PanTiltOperationRequest.OpType _ptcommand;

        public pxPanTilt.PanTiltOperationRequest.OpType PTCommand
        {
            get { return _ptcommand; }
            set { _ptcommand = value; }
        }

        public OnPTMove(DriveControl form, pxPanTilt.PanTiltOperationRequest.OpType cmd)
            : base(form)
        {
            _ptcommand = cmd; 
        }
    }


    class OnMove : DriveControlEvent
    {
        int _left;

        public int Left
        {
            get { return _left; }
            set { _left = value; }
        }

        int _right;

        public int Right
        {
            get { return _right; }
            set { _right = value; }
        }

        public OnMove(DriveControl form, int left, int right)
            : base(form)
        {
            // Changes from Ben Axelrod to let the robot run free!
            // Fly my beauty, fly!!!
            _left = left;// *750 / 1250;
            _right = right;// *750 / 1250;
            // End of Ben's changes
        }
    }

    class OnRotate : DriveControlEvent
    {
        double _angle;

        public double Angle
        {
            get { return _angle; }
            set { _angle = value; }
        }

        double _power;

        public double Power
        {
            get { return _power; }
            set { _power = value; }
        }

        public OnRotate(DriveControl form, double angle, double power)
            : base(form)
        {
            _angle = angle;
            _power = power;
        }
    }

    class OnTranslate : DriveControlEvent
    {
        double _distance;

        public double Distance
        {
            get { return _distance; }
            set { _distance = value; }
        }

        double _power;

        public double Power
        {
            get { return _power; }
            set { _power = value; }
        }

        public OnTranslate(DriveControl form, double distance, double power)
            : base(form)
        {
            _distance = distance;
            _power = power;
        }
    }

    class OnEStop : DriveControlEvent
    {
        public OnEStop(DriveControl form)
            : base(form)
        {
        }
    }

    class OnSynchronizeArms : DriveControlEvent
    {
        public OnSynchronizeArms(DriveControl form)
            : base(form)
        {
        }
    }

    class OnApplyJointParameters : DriveControlEvent
    {
        int _angle;

        public int Angle
        {
            get { return _angle; }
            set { _angle = value; }
        }

        string _jointName;

        public string JointName
        {
            get { return _jointName; }
            set { _jointName = value; }
        }

        public OnApplyJointParameters(DriveControl form, int angle, string name)
            : base(form)
        {
            _angle = angle;
            _jointName = name;
        }
    }

    class OnDisconnectSickLRF : DriveControlEvent
    {
        public OnDisconnectSickLRF(DriveControl form)
            : base(form)
        {
        }
    }


    // Raul - OnDisconnectSonar
    class OnDisconnectSonar : DriveControlEvent
    {
        public OnDisconnectSonar(DriveControl form)
            : base(form)
        {
        }
    }

    // Raul - GPS Disconnect
    class OnDisconnectGPS : DriveControlEvent
    {
        public OnDisconnectGPS(DriveControl form)
            : base(form)
        {
        }
    }


    // Raul - OnDisconnectBumpers
    class OnDisconnectBumpers : DriveControlEvent
    {
        public OnDisconnectBumpers(DriveControl form)
            : base(form)
        {
        }
    }


    class OnLogSetting : DriveControlEvent
    {
        bool _log;
        string _file;

        public bool Log
        {
            get { return _log; }
            set { _log = value; }
        }

        public string File
        {
            get { return _file; }
            set { _file = value; }
        }

        public OnLogSetting(DriveControl form, bool log, string file)
            : base(form)
        {
            _log = log;
            _file = file;
        }
    }

    // TT - Add a new message type for connection parameters
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

    // TT - Version 2 - Add a new message type for option settings
    class OnOptionSettings : DriveControlEvent
    {
        public GUIOptions Options;

        public OnOptionSettings(DriveControl form)
            : base(form)
        {
            Options = new GUIOptions();
        }
        public OnOptionSettings(DriveControl form, GUIOptions opt)
            : base(form)
        {
            Options = new GUIOptions();
            Options = opt;
        }
    }

    // TT - Webcam support
    class OnConnectWebCam : OnConnect
    {
        public OnConnectWebCam(DriveControl form, string service)
            : base(form, service)
        {
        }
    }

    class OnConnectPanTilt : OnConnect
    {
        public OnConnectPanTilt(DriveControl form, string service)
            : base(form, service)
        {
        }
    }

    class OnDisconnectWebCam : DriveControlEvent
    {
        public OnDisconnectWebCam(DriveControl form)
            : base(form)
        {
        }
    }

    // Raul - Sept 2007 - Cera Vision Support
    class OnConnectVision : OnConnect
    {
        public OnConnectVision(DriveControl form, string service)
            : base(form, service)
        {
        }
    }

    // Raul - Sept 2007
    class OnDisconnectVision : DriveControlEvent
    {
        public OnDisconnectVision(DriveControl form)
            : base(form)
        {
        }
    }


    class OnQueryFrame : DriveControlEvent
    {
        public OnQueryFrame(DriveControl form)
            : base(form)
        {
        }
    }


}