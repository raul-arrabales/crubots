namespace Cranium.Controls
{
    partial class Options
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.grpTrackball = new System.Windows.Forms.GroupBox();
            this.txtRotateScaleFactor = new System.Windows.Forms.TextBox();
            this.txtTranslateScaleFactor = new System.Windows.Forms.TextBox();
            this.txtDeadZoneY = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtDeadZoneX = new System.Windows.Forms.TextBox();
            this.grpMotion = new System.Windows.Forms.GroupBox();
            this.txtDriveDistance = new System.Windows.Forms.TextBox();
            this.txtRotateAngle = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.txtMotionSpeed = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.grpLRF = new System.Windows.Forms.GroupBox();
            this.txtMaxLRFRange = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.chkDisplayMap = new System.Windows.Forms.CheckBox();
            this.txtRobotWidth = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.chkShowLRF = new System.Windows.Forms.CheckBox();
            this.grpArm = new System.Windows.Forms.GroupBox();
            this.chkShowArm = new System.Windows.Forms.CheckBox();
            this.grpCamera = new System.Windows.Forms.GroupBox();
            this.txtCameraInterval = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.grBoxSonarOptions = new System.Windows.Forms.GroupBox();
            this.chkBoxDisplaySonarMap = new System.Windows.Forms.CheckBox();
            this.grpTrackball.SuspendLayout();
            this.grpMotion.SuspendLayout();
            this.grpLRF.SuspendLayout();
            this.grpArm.SuspendLayout();
            this.grpCamera.SuspendLayout();
            this.grBoxSonarOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(250, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Set the Option values. Remember to Save Settings.";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(164, 407);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(64, 26);
            this.btnOK.TabIndex = 16;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(275, 407);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(64, 26);
            this.btnCancel.TabIndex = 17;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // grpTrackball
            // 
            this.grpTrackball.Controls.Add(this.txtRotateScaleFactor);
            this.grpTrackball.Controls.Add(this.txtTranslateScaleFactor);
            this.grpTrackball.Controls.Add(this.txtDeadZoneY);
            this.grpTrackball.Controls.Add(this.label5);
            this.grpTrackball.Controls.Add(this.label4);
            this.grpTrackball.Controls.Add(this.label3);
            this.grpTrackball.Controls.Add(this.label2);
            this.grpTrackball.Controls.Add(this.txtDeadZoneX);
            this.grpTrackball.Location = new System.Drawing.Point(20, 31);
            this.grpTrackball.Name = "grpTrackball";
            this.grpTrackball.Size = new System.Drawing.Size(230, 140);
            this.grpTrackball.TabIndex = 18;
            this.grpTrackball.TabStop = false;
            this.grpTrackball.Text = "Trackball";
            // 
            // txtRotateScaleFactor
            // 
            this.txtRotateScaleFactor.Location = new System.Drawing.Point(145, 110);
            this.txtRotateScaleFactor.Name = "txtRotateScaleFactor";
            this.txtRotateScaleFactor.Size = new System.Drawing.Size(66, 20);
            this.txtRotateScaleFactor.TabIndex = 16;
            this.txtRotateScaleFactor.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtTranslateScaleFactor
            // 
            this.txtTranslateScaleFactor.Location = new System.Drawing.Point(145, 81);
            this.txtTranslateScaleFactor.Name = "txtTranslateScaleFactor";
            this.txtTranslateScaleFactor.Size = new System.Drawing.Size(66, 20);
            this.txtTranslateScaleFactor.TabIndex = 14;
            this.txtTranslateScaleFactor.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtDeadZoneY
            // 
            this.txtDeadZoneY.Location = new System.Drawing.Point(145, 52);
            this.txtDeadZoneY.Name = "txtDeadZoneY";
            this.txtDeadZoneY.Size = new System.Drawing.Size(66, 20);
            this.txtDeadZoneY.TabIndex = 12;
            this.txtDeadZoneY.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(14, 114);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(105, 13);
            this.label5.TabIndex = 15;
            this.label5.Text = "Rotate Scale Factor:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 85);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(117, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "Translate Scale Factor:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 56);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(74, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Dead Zone Y:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Dead Zone X:";
            // 
            // txtDeadZoneX
            // 
            this.txtDeadZoneX.Location = new System.Drawing.Point(145, 23);
            this.txtDeadZoneX.Name = "txtDeadZoneX";
            this.txtDeadZoneX.Size = new System.Drawing.Size(66, 20);
            this.txtDeadZoneX.TabIndex = 10;
            this.txtDeadZoneX.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // grpMotion
            // 
            this.grpMotion.Controls.Add(this.txtDriveDistance);
            this.grpMotion.Controls.Add(this.txtRotateAngle);
            this.grpMotion.Controls.Add(this.label10);
            this.grpMotion.Controls.Add(this.label9);
            this.grpMotion.Controls.Add(this.txtMotionSpeed);
            this.grpMotion.Controls.Add(this.label8);
            this.grpMotion.Location = new System.Drawing.Point(258, 31);
            this.grpMotion.Name = "grpMotion";
            this.grpMotion.Size = new System.Drawing.Size(230, 140);
            this.grpMotion.TabIndex = 19;
            this.grpMotion.TabStop = false;
            this.grpMotion.Text = "Motion Commands";
            // 
            // txtDriveDistance
            // 
            this.txtDriveDistance.Location = new System.Drawing.Point(149, 52);
            this.txtDriveDistance.Name = "txtDriveDistance";
            this.txtDriveDistance.Size = new System.Drawing.Size(66, 20);
            this.txtDriveDistance.TabIndex = 5;
            this.txtDriveDistance.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtRotateAngle
            // 
            this.txtRotateAngle.Location = new System.Drawing.Point(149, 81);
            this.txtRotateAngle.Name = "txtRotateAngle";
            this.txtRotateAngle.Size = new System.Drawing.Size(66, 20);
            this.txtRotateAngle.TabIndex = 4;
            this.txtRotateAngle.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(14, 56);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(105, 13);
            this.label10.TabIndex = 3;
            this.label10.Text = "Drive Distance (mm):";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(14, 84);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(119, 13);
            this.label9.TabIndex = 2;
            this.label9.Text = "Rotate Angle (degrees):";
            // 
            // txtMotionSpeed
            // 
            this.txtMotionSpeed.Location = new System.Drawing.Point(150, 23);
            this.txtMotionSpeed.Name = "txtMotionSpeed";
            this.txtMotionSpeed.Size = new System.Drawing.Size(66, 20);
            this.txtMotionSpeed.TabIndex = 1;
            this.txtMotionSpeed.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(14, 27);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(88, 13);
            this.label8.TabIndex = 0;
            this.label8.Text = "Speed (mm/sec):";
            // 
            // grpLRF
            // 
            this.grpLRF.Controls.Add(this.txtMaxLRFRange);
            this.grpLRF.Controls.Add(this.label7);
            this.grpLRF.Controls.Add(this.chkDisplayMap);
            this.grpLRF.Controls.Add(this.txtRobotWidth);
            this.grpLRF.Controls.Add(this.label6);
            this.grpLRF.Controls.Add(this.chkShowLRF);
            this.grpLRF.Location = new System.Drawing.Point(20, 176);
            this.grpLRF.Name = "grpLRF";
            this.grpLRF.Size = new System.Drawing.Size(230, 120);
            this.grpLRF.TabIndex = 20;
            this.grpLRF.TabStop = false;
            this.grpLRF.Text = "Laser Range Finder";
            // 
            // txtMaxLRFRange
            // 
            this.txtMaxLRFRange.Location = new System.Drawing.Point(157, 93);
            this.txtMaxLRFRange.Name = "txtMaxLRFRange";
            this.txtMaxLRFRange.Size = new System.Drawing.Size(66, 20);
            this.txtMaxLRFRange.TabIndex = 20;
            this.txtMaxLRFRange.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(18, 96);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(119, 13);
            this.label7.TabIndex = 19;
            this.label7.Text = "Max Laser Range (mm):";
            // 
            // chkDisplayMap
            // 
            this.chkDisplayMap.AutoSize = true;
            this.chkDisplayMap.Location = new System.Drawing.Point(21, 71);
            this.chkDisplayMap.Name = "chkDisplayMap";
            this.chkDisplayMap.Size = new System.Drawing.Size(84, 17);
            this.chkDisplayMap.TabIndex = 18;
            this.chkDisplayMap.Text = "Display Map";
            this.chkDisplayMap.UseVisualStyleBackColor = true;
            // 
            // txtRobotWidth
            // 
            this.txtRobotWidth.Location = new System.Drawing.Point(157, 42);
            this.txtRobotWidth.Name = "txtRobotWidth";
            this.txtRobotWidth.Size = new System.Drawing.Size(66, 20);
            this.txtRobotWidth.TabIndex = 17;
            this.txtRobotWidth.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(18, 45);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(133, 13);
            this.label6.TabIndex = 16;
            this.label6.Text = "Robot Width for LRF (mm):";
            // 
            // chkShowLRF
            // 
            this.chkShowLRF.AutoSize = true;
            this.chkShowLRF.Location = new System.Drawing.Point(21, 24);
            this.chkShowLRF.Name = "chkShowLRF";
            this.chkShowLRF.Size = new System.Drawing.Size(149, 17);
            this.chkShowLRF.TabIndex = 15;
            this.chkShowLRF.Text = "Show Laser Range Finder";
            this.chkShowLRF.UseVisualStyleBackColor = true;
            // 
            // grpArm
            // 
            this.grpArm.Controls.Add(this.chkShowArm);
            this.grpArm.Location = new System.Drawing.Point(258, 176);
            this.grpArm.Name = "grpArm";
            this.grpArm.Size = new System.Drawing.Size(230, 58);
            this.grpArm.TabIndex = 21;
            this.grpArm.TabStop = false;
            this.grpArm.Text = "Articulated Arm";
            // 
            // chkShowArm
            // 
            this.chkShowArm.AutoSize = true;
            this.chkShowArm.Location = new System.Drawing.Point(17, 23);
            this.chkShowArm.Name = "chkShowArm";
            this.chkShowArm.Size = new System.Drawing.Size(127, 17);
            this.chkShowArm.TabIndex = 16;
            this.chkShowArm.Text = "Show Articulated Arm";
            this.chkShowArm.UseVisualStyleBackColor = true;
            // 
            // grpCamera
            // 
            this.grpCamera.Controls.Add(this.txtCameraInterval);
            this.grpCamera.Controls.Add(this.label11);
            this.grpCamera.Location = new System.Drawing.Point(258, 240);
            this.grpCamera.Name = "grpCamera";
            this.grpCamera.Size = new System.Drawing.Size(229, 55);
            this.grpCamera.TabIndex = 22;
            this.grpCamera.TabStop = false;
            this.grpCamera.Text = "WebCam Viewer";
            // 
            // txtCameraInterval
            // 
            this.txtCameraInterval.Location = new System.Drawing.Point(152, 23);
            this.txtCameraInterval.Name = "txtCameraInterval";
            this.txtCameraInterval.Size = new System.Drawing.Size(63, 20);
            this.txtCameraInterval.TabIndex = 1;
            this.txtCameraInterval.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(14, 26);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(105, 13);
            this.label11.TabIndex = 0;
            this.label11.Text = "Update Interval (ms):";
            // 
            // grBoxSonarOptions
            // 
            this.grBoxSonarOptions.Controls.Add(this.chkBoxDisplaySonarMap);
            this.grBoxSonarOptions.Location = new System.Drawing.Point(20, 307);
            this.grBoxSonarOptions.Name = "grBoxSonarOptions";
            this.grBoxSonarOptions.Size = new System.Drawing.Size(229, 94);
            this.grBoxSonarOptions.TabIndex = 23;
            this.grBoxSonarOptions.TabStop = false;
            this.grBoxSonarOptions.Text = "SONAR";
            this.grBoxSonarOptions.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // chkBoxDisplaySonarMap
            // 
            this.chkBoxDisplaySonarMap.AutoSize = true;
            this.chkBoxDisplaySonarMap.Location = new System.Drawing.Point(21, 28);
            this.chkBoxDisplaySonarMap.Name = "chkBoxDisplaySonarMap";
            this.chkBoxDisplaySonarMap.Size = new System.Drawing.Size(115, 17);
            this.chkBoxDisplaySonarMap.TabIndex = 16;
            this.chkBoxDisplaySonarMap.Text = "Display Sonar Map";
            this.chkBoxDisplaySonarMap.UseVisualStyleBackColor = true;
            // 
            // Options
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(510, 440);
            this.Controls.Add(this.grBoxSonarOptions);
            this.Controls.Add(this.grpCamera);
            this.Controls.Add(this.grpArm);
            this.Controls.Add(this.grpLRF);
            this.Controls.Add(this.grpMotion);
            this.Controls.Add(this.grpTrackball);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.label1);
            this.Name = "Options";
            this.Text = "Options";
            this.Load += new System.EventHandler(this.Options_Load);
            this.grpTrackball.ResumeLayout(false);
            this.grpTrackball.PerformLayout();
            this.grpMotion.ResumeLayout(false);
            this.grpMotion.PerformLayout();
            this.grpLRF.ResumeLayout(false);
            this.grpLRF.PerformLayout();
            this.grpArm.ResumeLayout(false);
            this.grpArm.PerformLayout();
            this.grpCamera.ResumeLayout(false);
            this.grpCamera.PerformLayout();
            this.grBoxSonarOptions.ResumeLayout(false);
            this.grBoxSonarOptions.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox grpTrackball;
        private System.Windows.Forms.TextBox txtRotateScaleFactor;
        private System.Windows.Forms.TextBox txtTranslateScaleFactor;
        private System.Windows.Forms.TextBox txtDeadZoneY;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtDeadZoneX;
        private System.Windows.Forms.GroupBox grpMotion;
        private System.Windows.Forms.TextBox txtMotionSpeed;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.GroupBox grpLRF;
        private System.Windows.Forms.TextBox txtMaxLRFRange;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox chkDisplayMap;
        private System.Windows.Forms.TextBox txtRobotWidth;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox chkShowLRF;
        private System.Windows.Forms.TextBox txtDriveDistance;
        private System.Windows.Forms.TextBox txtRotateAngle;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.GroupBox grpArm;
        private System.Windows.Forms.CheckBox chkShowArm;
        private System.Windows.Forms.GroupBox grpCamera;
        private System.Windows.Forms.TextBox txtCameraInterval;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.GroupBox grBoxSonarOptions;
        private System.Windows.Forms.CheckBox chkBoxDisplaySonarMap;
    }
}