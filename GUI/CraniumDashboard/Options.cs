// Options.cs
//
// This file is derived from the Microsoft Robotics Studio Code Samples
// for the SimpleDashboard.
//
// Written by Trevor Taylor, 29-Sep-2006
// Last Updated: May-2007
//
// Portions by Raul Arrabales, Aug-2007
//
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Cranium.Controls
{
    public partial class Options : Form
    {
        // Local copy of a reference to the Option settings
        // See DashboardState.cs for the definition of
        // the GUIOptions class.
        GUIOptions opt;

        // Constructor takes a reference to the options so that
        // we can return the results of changes made by the user
        public Options(ref GUIOptions options)
        {
            InitializeComponent();
            opt = options;
        }

        // Display all of the current option settings when the
        // form is loaded
        private void Options_Load(object sender, EventArgs e)
        {
            txtDeadZoneX.Text = opt.DeadZoneX.ToString();
            txtDeadZoneY.Text = opt.DeadZoneY.ToString();
            txtTranslateScaleFactor.Text = opt.TranslateScaleFactor.ToString();
            txtRotateScaleFactor.Text = opt.RotateScaleFactor.ToString();
            chkShowArm.Checked = opt.ShowArm;
            chkShowLRF.Checked = opt.ShowLRF;
            txtRobotWidth.Text = opt.RobotWidth.ToString();
            // Version 7
            chkDisplayMap.Checked = opt.DisplayMap;
            txtMaxLRFRange.Text = opt.MaxLRFRange.ToString();
            txtMotionSpeed.Text = opt.MotionSpeed.ToString();
            txtDriveDistance.Text = opt.DriveDistance.ToString();
            txtRotateAngle.Text = opt.RotateAngle.ToString();
            txtCameraInterval.Text = opt.CameraInterval.ToString();

            // Raul - Sept 2007
            chkBoxDisplaySonarMap.Checked = opt.DisplaySonarMap;
        }

        // User clicked on OK
        private void btnOK_Click(object sender, EventArgs e)
        {
            double dX, dY, trans, rot, robwid, maxrange;
            double speed, distance, angle;
            int interval;
            string err = "";

            // Set some defaults
            // This is just to keep the compiler happy - the values
            // will be overwritten, but the compiler does not understand
            // the try/catch blocks and says the variables are not
            // assigned a value ...
            dX = dY = 80;
            trans = 1.0;
            rot = 0.5;
            robwid = 0.0;
            maxrange = 8192;
            speed = 100;
            distance = 300;
            angle = 45;
            interval = 250;

            try
            {
                dX = Math.Abs(double.Parse(txtDeadZoneX.Text));
            }
            catch
            {
                err += "Enter a number for Dead Zone X\n";
            }
            try
            {
                dY = Math.Abs(double.Parse(txtDeadZoneY.Text));
            }
            catch
            {
                err += "Enter a number for Dead Zone X\n";
            }
            try
            {
                trans = double.Parse(txtTranslateScaleFactor.Text);
            }
            catch
            {
                err += "Enter a number for TranslateScaleFactor\n";
            }
            try
            {
                rot = double.Parse(txtRotateScaleFactor.Text);
            }
            catch
            {
                err += "Enter a number for RotateScaleFactor\n";
            }
            try
            {
                robwid = double.Parse(txtRobotWidth.Text);
            }
            catch
            {
                err += "Enter a number for RobotWidth (in mm)\n";
            }
            try
            {
                maxrange = double.Parse(txtMaxLRFRange.Text);
            }
            catch
            {
                err += "Enter a number for Maximum Laser Range (in mm)\n";
            }

            // TT - Version 7
            try
            {
                speed = double.Parse(txtMotionSpeed.Text);
                if (speed <= 10)
                    err += "Motion Speed must be greater than 10\n";
                if (speed > 1000)
                    err += "Motion Speed must be 1000 or less\n";
            }
            catch
            {
                err += "Enter a number for Motion Speed (in mm/sec)\n";
            }
            try
            {
                distance = double.Parse(txtDriveDistance.Text);
            }
            catch
            {
                err += "Enter a number for Drive Distance (in mm)\n";
            }
            try
            {
                angle = double.Parse(txtRotateAngle.Text);
                if (angle > 359)
                    err += "Rotate Angle must be less than 360\n";
            }
            catch
            {
                err += "Enter a number for Rotate Angle (in degrees)\n";
            }
            try
            {
                interval = int.Parse(txtCameraInterval.Text);
                if (interval <= 0)
                    err += "Interval must be > 0\n";
            }
            catch
            {
                err += "Enter a number for Interval (in milliseconds)\n";
            }

            // TT - Version 3
            // Allow negative values
            // This might seem strange, but it allows the axes to be
            // flipped on the trackball which some people might find
            // more convenient.
            if (trans == 0)
                err += "Translate Scale Factor must not be zero\n";
            if (rot == 0)
                err += "Rotate Scale Factor must not be zero\n";

            // If any of the tests above generated an error message,
            // then display it now and don't make the changes
            if (err != "")
            {
                MessageBox.Show(err, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            // Set the new values
            opt.DeadZoneX = dX;
            opt.DeadZoneY = dY;
            opt.TranslateScaleFactor = trans;
            opt.RotateScaleFactor = rot;

            opt.ShowArm = chkShowArm.Checked;
            opt.ShowLRF = chkShowLRF.Checked;

            opt.RobotWidth = robwid;

            // Version 6
            opt.DisplayMap = chkDisplayMap.Checked;
            opt.MaxLRFRange = maxrange;
            opt.MotionSpeed = speed;
            opt.DriveDistance = distance;
            opt.RotateAngle = angle;
            opt.CameraInterval = interval;

            // Raul - Sept 2007
            opt.DisplaySonarMap = chkBoxDisplaySonarMap.Checked;

            // Set our return result
            this.DialogResult = DialogResult.OK;
            // Close down and return to caller
            this.Close();
        }

        // User clicked on Cancel
        private void btnCancel_Click(object sender, EventArgs e)
        {
            // Just set the result and die quietly
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

    }
}