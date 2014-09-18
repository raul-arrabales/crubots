//-----------------------------------------------------------------------
//  This file is part of the Microsoft Robotics Studio Code Samples.
// 
//  Copyright (C) Microsoft Corporation.  All rights reserved.
//
//  $File: DriveControl.Designer.cs $ $Revision: 9 $
//-----------------------------------------------------------------------

namespace Cranium.Controls
{
    partial class DriveControl
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
            System.Windows.Forms.Label label1;
            System.Windows.Forms.Label label5;
            System.Windows.Forms.Label label6;
            System.Windows.Forms.Label label7;
            System.Windows.Forms.Label label2;
            System.Windows.Forms.Label label4;
            System.Windows.Forms.Label label3;
            this.cbJoystick = new System.Windows.Forms.ComboBox();
            this.lblX = new System.Windows.Forms.Label();
            this.lblY = new System.Windows.Forms.Label();
            this.lblZ = new System.Windows.Forms.Label();
            this.lblButtons = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkStop = new System.Windows.Forms.CheckBox();
            this.chkDrive = new System.Windows.Forms.CheckBox();
            this.picJoystick = new System.Windows.Forms.PictureBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.linkDirectory = new System.Windows.Forms.LinkLabel();
            this.lblNode = new System.Windows.Forms.Label();
            this.listDirectory = new System.Windows.Forms.ListBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.txtPort = new System.Windows.Forms.MaskedTextBox();
            this.txtMachine = new System.Windows.Forms.TextBox();
            this.groupBoxLRF = new System.Windows.Forms.GroupBox();
            this.btnDisconnect = new System.Windows.Forms.Button();
            this.lblDelay = new System.Windows.Forms.Label();
            this.btnConnectLRF = new System.Windows.Forms.Button();
            this.btnStartLRF = new System.Windows.Forms.Button();
            this.picLRF = new System.Windows.Forms.PictureBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label12 = new System.Windows.Forms.Label();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.txtLogFile = new System.Windows.Forms.TextBox();
            this.chkLog = new System.Windows.Forms.CheckBox();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.MainMenu = new System.Windows.Forms.MenuStrip();
            this.FileMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.saveSettingsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolsMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.HelpMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SonarGroupBox = new System.Windows.Forms.GroupBox();
            this.lblSonarS7 = new System.Windows.Forms.Label();
            this.lblSonarS6 = new System.Windows.Forms.Label();
            this.lblSonarS5 = new System.Windows.Forms.Label();
            this.lblSonarS4 = new System.Windows.Forms.Label();
            this.lblSonarS3 = new System.Windows.Forms.Label();
            this.lblSonarS2 = new System.Windows.Forms.Label();
            this.lblSonarS1 = new System.Windows.Forms.Label();
            this.lblSonarS0 = new System.Windows.Forms.Label();
            this.btnStartSonar = new System.Windows.Forms.Button();
            this.picSonar = new System.Windows.Forms.PictureBox();
            this.btnDisconnectSonar = new System.Windows.Forms.Button();
            this.btnConnectSonar = new System.Windows.Forms.Button();
            this.lblSonarDelay = new System.Windows.Forms.Label();
            this.grBoxSonarMap = new System.Windows.Forms.GroupBox();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnRight = new System.Windows.Forms.Button();
            this.btnLeft = new System.Windows.Forms.Button();
            this.btnDown = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.btnCamDisconnect = new System.Windows.Forms.Button();
            this.btnCamConnect = new System.Windows.Forms.Button();
            this.picCamImage = new System.Windows.Forms.PictureBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btn_b13 = new System.Windows.Forms.Button();
            this.btn_b12 = new System.Windows.Forms.Button();
            this.btn_b9 = new System.Windows.Forms.Button();
            this.btn_b10 = new System.Windows.Forms.Button();
            this.btn_b11 = new System.Windows.Forms.Button();
            this.label19 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.btn_b5 = new System.Windows.Forms.Button();
            this.btn_b1 = new System.Windows.Forms.Button();
            this.btn_b4 = new System.Windows.Forms.Button();
            this.btn_b2 = new System.Windows.Forms.Button();
            this.btn_b3 = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.simulImage = new System.Windows.Forms.PictureBox();
            this.checkBoxHideUI = new System.Windows.Forms.CheckBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.lblGPSZ = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.lblGPStime = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.lblGPSY = new System.Windows.Forms.Label();
            this.lblGPSX = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.lblGPSAngle = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            label6 = new System.Windows.Forms.Label();
            label7 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picJoystick)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBoxLRF.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLRF)).BeginInit();
            this.groupBox5.SuspendLayout();
            this.MainMenu.SuspendLayout();
            this.SonarGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picSonar)).BeginInit();
            this.grBoxSonarMap.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picCamImage)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.simulImage)).BeginInit();
            this.groupBox6.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(8, 31);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(55, 17);
            label1.TabIndex = 1;
            label1.Text = "Device:";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(45, 62);
            label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(21, 17);
            label5.TabIndex = 5;
            label5.Text = "X:";
            label5.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(45, 82);
            label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(21, 17);
            label6.TabIndex = 6;
            label6.Text = "Y:";
            label6.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(45, 103);
            label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(21, 17);
            label7.TabIndex = 7;
            label7.Text = "Z:";
            label7.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(8, 124);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(60, 17);
            label2.TabIndex = 8;
            label2.Text = "Buttons:";
            label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(9, 62);
            label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(38, 17);
            label4.TabIndex = 1;
            label4.Text = "Port:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(9, 25);
            label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(65, 17);
            label3.TabIndex = 0;
            label3.Text = "Machine:";
            // 
            // cbJoystick
            // 
            this.cbJoystick.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbJoystick.FormattingEnabled = true;
            this.cbJoystick.Location = new System.Drawing.Point(80, 27);
            this.cbJoystick.Margin = new System.Windows.Forms.Padding(4);
            this.cbJoystick.Name = "cbJoystick";
            this.cbJoystick.Size = new System.Drawing.Size(171, 24);
            this.cbJoystick.TabIndex = 0;
            this.cbJoystick.SelectedIndexChanged += new System.EventHandler(this.cbJoystick_SelectedIndexChanged);
            // 
            // lblX
            // 
            this.lblX.Location = new System.Drawing.Point(80, 62);
            this.lblX.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblX.Name = "lblX";
            this.lblX.Size = new System.Drawing.Size(47, 16);
            this.lblX.TabIndex = 2;
            this.lblX.Text = "0";
            this.lblX.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblY
            // 
            this.lblY.Location = new System.Drawing.Point(80, 82);
            this.lblY.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblY.Name = "lblY";
            this.lblY.Size = new System.Drawing.Size(47, 16);
            this.lblY.TabIndex = 3;
            this.lblY.Text = "0";
            this.lblY.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblZ
            // 
            this.lblZ.Location = new System.Drawing.Point(80, 103);
            this.lblZ.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblZ.Name = "lblZ";
            this.lblZ.Size = new System.Drawing.Size(47, 16);
            this.lblZ.TabIndex = 4;
            this.lblZ.Text = "0";
            this.lblZ.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblButtons
            // 
            this.lblButtons.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblButtons.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblButtons.Location = new System.Drawing.Point(84, 124);
            this.lblButtons.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblButtons.Name = "lblButtons";
            this.lblButtons.Size = new System.Drawing.Size(168, 16);
            this.lblButtons.TabIndex = 9;
            this.lblButtons.Text = "O";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkStop);
            this.groupBox1.Controls.Add(this.chkDrive);
            this.groupBox1.Controls.Add(this.picJoystick);
            this.groupBox1.Controls.Add(label1);
            this.groupBox1.Controls.Add(this.lblButtons);
            this.groupBox1.Controls.Add(this.cbJoystick);
            this.groupBox1.Controls.Add(label2);
            this.groupBox1.Controls.Add(this.lblX);
            this.groupBox1.Controls.Add(label7);
            this.groupBox1.Controls.Add(this.lblY);
            this.groupBox1.Controls.Add(label6);
            this.groupBox1.Controls.Add(this.lblZ);
            this.groupBox1.Controls.Add(label5);
            this.groupBox1.Location = new System.Drawing.Point(16, 33);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(260, 186);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Direct Input Device";
            // 
            // chkStop
            // 
            this.chkStop.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkStop.Location = new System.Drawing.Point(149, 144);
            this.chkStop.Margin = new System.Windows.Forms.Padding(4);
            this.chkStop.Name = "chkStop";
            this.chkStop.Size = new System.Drawing.Size(103, 30);
            this.chkStop.TabIndex = 12;
            this.chkStop.Text = "Stop";
            this.chkStop.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkStop.UseVisualStyleBackColor = true;
            this.chkStop.CheckedChanged += new System.EventHandler(this.chkStop_CheckedChanged);
            // 
            // chkDrive
            // 
            this.chkDrive.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkDrive.Location = new System.Drawing.Point(12, 144);
            this.chkDrive.Margin = new System.Windows.Forms.Padding(4);
            this.chkDrive.Name = "chkDrive";
            this.chkDrive.Size = new System.Drawing.Size(101, 30);
            this.chkDrive.TabIndex = 11;
            this.chkDrive.Text = "Drive";
            this.chkDrive.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chkDrive.UseVisualStyleBackColor = true;
            this.chkDrive.CheckedChanged += new System.EventHandler(this.chkDrive_CheckedChanged);
            // 
            // picJoystick
            // 
            this.picJoystick.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.picJoystick.Location = new System.Drawing.Point(172, 60);
            this.picJoystick.Margin = new System.Windows.Forms.Padding(4);
            this.picJoystick.Name = "picJoystick";
            this.picJoystick.Size = new System.Drawing.Size(65, 60);
            this.picJoystick.TabIndex = 10;
            this.picJoystick.TabStop = false;
            this.picJoystick.MouseLeave += new System.EventHandler(this.picJoystick_MouseLeave);
            this.picJoystick.MouseMove += new System.Windows.Forms.MouseEventHandler(this.picJoystick_MouseMove);
            this.picJoystick.MouseUp += new System.Windows.Forms.MouseEventHandler(this.picJoystick_MouseUp);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.linkDirectory);
            this.groupBox2.Controls.Add(this.lblNode);
            this.groupBox2.Controls.Add(this.listDirectory);
            this.groupBox2.Controls.Add(this.btnConnect);
            this.groupBox2.Controls.Add(this.txtPort);
            this.groupBox2.Controls.Add(this.txtMachine);
            this.groupBox2.Controls.Add(label4);
            this.groupBox2.Controls.Add(label3);
            this.groupBox2.Location = new System.Drawing.Point(284, 33);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox2.Size = new System.Drawing.Size(231, 279);
            this.groupBox2.TabIndex = 11;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Remote Node";
            // 
            // linkDirectory
            // 
            this.linkDirectory.AutoSize = true;
            this.linkDirectory.Enabled = false;
            this.linkDirectory.LinkArea = new System.Windows.Forms.LinkArea(8, 9);
            this.linkDirectory.Location = new System.Drawing.Point(9, 92);
            this.linkDirectory.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.linkDirectory.Name = "linkDirectory";
            this.linkDirectory.Size = new System.Drawing.Size(110, 20);
            this.linkDirectory.TabIndex = 8;
            this.linkDirectory.TabStop = true;
            this.linkDirectory.Text = "Service Directory:";
            this.linkDirectory.UseCompatibleTextRendering = true;
            this.linkDirectory.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkDirectory_LinkClicked);
            // 
            // lblNode
            // 
            this.lblNode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNode.AutoEllipsis = true;
            this.lblNode.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblNode.Location = new System.Drawing.Point(13, 118);
            this.lblNode.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblNode.Name = "lblNode";
            this.lblNode.Size = new System.Drawing.Size(205, 25);
            this.lblNode.TabIndex = 7;
            // 
            // listDirectory
            // 
            this.listDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listDirectory.FormattingEnabled = true;
            this.listDirectory.ItemHeight = 16;
            this.listDirectory.Location = new System.Drawing.Point(13, 162);
            this.listDirectory.Margin = new System.Windows.Forms.Padding(4);
            this.listDirectory.Name = "listDirectory";
            this.listDirectory.Size = new System.Drawing.Size(208, 100);
            this.listDirectory.TabIndex = 5;
            this.listDirectory.SelectedIndexChanged += new System.EventHandler(this.listDirectory_SelectedIndexChanged);
            this.listDirectory.DoubleClick += new System.EventHandler(this.listDirectory_DoubleClick);
            // 
            // btnConnect
            // 
            this.btnConnect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConnect.Location = new System.Drawing.Point(149, 52);
            this.btnConnect.Margin = new System.Windows.Forms.Padding(4);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(73, 28);
            this.btnConnect.TabIndex = 4;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(85, 54);
            this.txtPort.Margin = new System.Windows.Forms.Padding(4);
            this.txtPort.Mask = "99999";
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(55, 22);
            this.txtPort.TabIndex = 3;
            // 
            // txtMachine
            // 
            this.txtMachine.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMachine.Location = new System.Drawing.Point(85, 21);
            this.txtMachine.Margin = new System.Windows.Forms.Padding(4);
            this.txtMachine.Name = "txtMachine";
            this.txtMachine.Size = new System.Drawing.Size(136, 22);
            this.txtMachine.TabIndex = 2;
            // 
            // groupBoxLRF
            // 
            this.groupBoxLRF.Controls.Add(this.btnDisconnect);
            this.groupBoxLRF.Controls.Add(this.lblDelay);
            this.groupBoxLRF.Controls.Add(this.btnConnectLRF);
            this.groupBoxLRF.Controls.Add(this.btnStartLRF);
            this.groupBoxLRF.Controls.Add(this.picLRF);
            this.groupBoxLRF.Location = new System.Drawing.Point(16, 320);
            this.groupBoxLRF.Margin = new System.Windows.Forms.Padding(4);
            this.groupBoxLRF.Name = "groupBoxLRF";
            this.groupBoxLRF.Padding = new System.Windows.Forms.Padding(4);
            this.groupBoxLRF.Size = new System.Drawing.Size(499, 215);
            this.groupBoxLRF.TabIndex = 12;
            this.groupBoxLRF.TabStop = false;
            this.groupBoxLRF.Text = "Laser Range Finder";
            // 
            // btnDisconnect
            // 
            this.btnDisconnect.Enabled = false;
            this.btnDisconnect.Location = new System.Drawing.Point(224, 23);
            this.btnDisconnect.Margin = new System.Windows.Forms.Padding(4);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new System.Drawing.Size(100, 28);
            this.btnDisconnect.TabIndex = 4;
            this.btnDisconnect.Text = "Disconnect";
            this.btnDisconnect.UseVisualStyleBackColor = true;
            this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
            // 
            // lblDelay
            // 
            this.lblDelay.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDelay.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDelay.Location = new System.Drawing.Point(349, 28);
            this.lblDelay.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDelay.Name = "lblDelay";
            this.lblDelay.Size = new System.Drawing.Size(137, 22);
            this.lblDelay.TabIndex = 3;
            this.lblDelay.Text = "0";
            // 
            // btnConnectLRF
            // 
            this.btnConnectLRF.Enabled = false;
            this.btnConnectLRF.Location = new System.Drawing.Point(116, 23);
            this.btnConnectLRF.Margin = new System.Windows.Forms.Padding(4);
            this.btnConnectLRF.Name = "btnConnectLRF";
            this.btnConnectLRF.Size = new System.Drawing.Size(100, 28);
            this.btnConnectLRF.TabIndex = 2;
            this.btnConnectLRF.Text = "Connect";
            this.btnConnectLRF.UseVisualStyleBackColor = true;
            this.btnConnectLRF.Click += new System.EventHandler(this.btnConnectLRF_Click);
            // 
            // btnStartLRF
            // 
            this.btnStartLRF.Enabled = false;
            this.btnStartLRF.Location = new System.Drawing.Point(8, 22);
            this.btnStartLRF.Margin = new System.Windows.Forms.Padding(4);
            this.btnStartLRF.Name = "btnStartLRF";
            this.btnStartLRF.Size = new System.Drawing.Size(100, 28);
            this.btnStartLRF.TabIndex = 1;
            this.btnStartLRF.Text = "Start";
            this.btnStartLRF.UseVisualStyleBackColor = true;
            this.btnStartLRF.Click += new System.EventHandler(this.btnStartLRF_Click);
            // 
            // picLRF
            // 
            this.picLRF.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.picLRF.Location = new System.Drawing.Point(9, 59);
            this.picLRF.Margin = new System.Windows.Forms.Padding(4);
            this.picLRF.Name = "picLRF";
            this.picLRF.Size = new System.Drawing.Size(481, 149);
            this.picLRF.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picLRF.TabIndex = 0;
            this.picLRF.TabStop = false;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.label12);
            this.groupBox5.Controls.Add(this.btnBrowse);
            this.groupBox5.Controls.Add(this.txtLogFile);
            this.groupBox5.Controls.Add(this.chkLog);
            this.groupBox5.Location = new System.Drawing.Point(16, 226);
            this.groupBox5.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox5.Size = new System.Drawing.Size(260, 86);
            this.groupBox5.TabIndex = 14;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Logging";
            // 
            // label12
            // 
            this.label12.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(141, 25);
            this.label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(105, 22);
            this.label12.TabIndex = 18;
            this.label12.Text = "0";
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(187, 52);
            this.btnBrowse.Margin = new System.Windows.Forms.Padding(4);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(36, 28);
            this.btnBrowse.TabIndex = 2;
            this.btnBrowse.Text = "...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // txtLogFile
            // 
            this.txtLogFile.Location = new System.Drawing.Point(8, 53);
            this.txtLogFile.Margin = new System.Windows.Forms.Padding(4);
            this.txtLogFile.Name = "txtLogFile";
            this.txtLogFile.Size = new System.Drawing.Size(169, 22);
            this.txtLogFile.TabIndex = 1;
            // 
            // chkLog
            // 
            this.chkLog.AutoSize = true;
            this.chkLog.Location = new System.Drawing.Point(8, 23);
            this.chkLog.Margin = new System.Windows.Forms.Padding(4);
            this.chkLog.Name = "chkLog";
            this.chkLog.Size = new System.Drawing.Size(122, 21);
            this.chkLog.TabIndex = 0;
            this.chkLog.Text = "Log Messages";
            this.chkLog.UseVisualStyleBackColor = true;
            this.chkLog.CheckedChanged += new System.EventHandler(this.chkLog_CheckedChanged);
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.Filter = "Xml log file|*.log;*.xml|All files|*.*";
            this.saveFileDialog.Title = "Log File";
            this.saveFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.saveFileDialog_FileOk);
            // 
            // MainMenu
            // 
            this.MainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FileMenu,
            this.ToolsMenu,
            this.HelpMenu});
            this.MainMenu.Location = new System.Drawing.Point(0, 0);
            this.MainMenu.Name = "MainMenu";
            this.MainMenu.Padding = new System.Windows.Forms.Padding(8, 2, 0, 2);
            this.MainMenu.Size = new System.Drawing.Size(1219, 29);
            this.MainMenu.TabIndex = 16;
            this.MainMenu.Text = "menuStrip1";
            // 
            // FileMenu
            // 
            this.FileMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveSettingsMenuItem,
            this.exitMenuItem});
            this.FileMenu.Name = "FileMenu";
            this.FileMenu.Size = new System.Drawing.Size(48, 25);
            this.FileMenu.Text = "&File";
            // 
            // saveSettingsMenuItem
            // 
            this.saveSettingsMenuItem.Name = "saveSettingsMenuItem";
            this.saveSettingsMenuItem.Size = new System.Drawing.Size(212, 26);
            this.saveSettingsMenuItem.Text = "&Save Settings...";
            this.saveSettingsMenuItem.Click += new System.EventHandler(this.saveSettingsMenuItem_Click);
            // 
            // exitMenuItem
            // 
            this.exitMenuItem.Name = "exitMenuItem";
            this.exitMenuItem.Size = new System.Drawing.Size(212, 26);
            this.exitMenuItem.Text = "E&xit";
            this.exitMenuItem.Click += new System.EventHandler(this.exitMenuItem_Click);
            // 
            // ToolsMenu
            // 
            this.ToolsMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optionsMenuItem});
            this.ToolsMenu.Name = "ToolsMenu";
            this.ToolsMenu.Size = new System.Drawing.Size(62, 25);
            this.ToolsMenu.Text = "&Tools";
            // 
            // optionsMenuItem
            // 
            this.optionsMenuItem.Name = "optionsMenuItem";
            this.optionsMenuItem.Size = new System.Drawing.Size(154, 26);
            this.optionsMenuItem.Text = "&Options";
            this.optionsMenuItem.Click += new System.EventHandler(this.optionsMenuItem_Click);
            // 
            // HelpMenu
            // 
            this.HelpMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutMenuItem});
            this.HelpMenu.Name = "HelpMenu";
            this.HelpMenu.Size = new System.Drawing.Size(55, 25);
            this.HelpMenu.Text = "Help";
            // 
            // aboutMenuItem
            // 
            this.aboutMenuItem.Name = "aboutMenuItem";
            this.aboutMenuItem.Size = new System.Drawing.Size(161, 26);
            this.aboutMenuItem.Text = "About ...";
            this.aboutMenuItem.Click += new System.EventHandler(this.aboutMenuItem_Click);
            // 
            // SonarGroupBox
            // 
            this.SonarGroupBox.Controls.Add(this.lblSonarS7);
            this.SonarGroupBox.Controls.Add(this.lblSonarS6);
            this.SonarGroupBox.Controls.Add(this.lblSonarS5);
            this.SonarGroupBox.Controls.Add(this.lblSonarS4);
            this.SonarGroupBox.Controls.Add(this.lblSonarS3);
            this.SonarGroupBox.Controls.Add(this.lblSonarS2);
            this.SonarGroupBox.Controls.Add(this.lblSonarS1);
            this.SonarGroupBox.Controls.Add(this.lblSonarS0);
            this.SonarGroupBox.Controls.Add(this.btnStartSonar);
            this.SonarGroupBox.Controls.Add(this.picSonar);
            this.SonarGroupBox.Controls.Add(this.btnDisconnectSonar);
            this.SonarGroupBox.Controls.Add(this.btnConnectSonar);
            this.SonarGroupBox.Controls.Add(this.lblSonarDelay);
            this.SonarGroupBox.Location = new System.Drawing.Point(525, 33);
            this.SonarGroupBox.Margin = new System.Windows.Forms.Padding(4);
            this.SonarGroupBox.Name = "SonarGroupBox";
            this.SonarGroupBox.Padding = new System.Windows.Forms.Padding(4);
            this.SonarGroupBox.Size = new System.Drawing.Size(433, 271);
            this.SonarGroupBox.TabIndex = 17;
            this.SonarGroupBox.TabStop = false;
            this.SonarGroupBox.Text = "SONAR";
            this.SonarGroupBox.Enter += new System.EventHandler(this.groupBox3_Enter);
            // 
            // lblSonarS7
            // 
            this.lblSonarS7.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSonarS7.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSonarS7.Location = new System.Drawing.Point(317, 222);
            this.lblSonarS7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSonarS7.Name = "lblSonarS7";
            this.lblSonarS7.Size = new System.Drawing.Size(92, 22);
            this.lblSonarS7.TabIndex = 16;
            this.lblSonarS7.Text = "0";
            // 
            // lblSonarS6
            // 
            this.lblSonarS6.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSonarS6.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSonarS6.Location = new System.Drawing.Point(317, 198);
            this.lblSonarS6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSonarS6.Name = "lblSonarS6";
            this.lblSonarS6.Size = new System.Drawing.Size(92, 22);
            this.lblSonarS6.TabIndex = 15;
            this.lblSonarS6.Text = "0";
            // 
            // lblSonarS5
            // 
            this.lblSonarS5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSonarS5.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSonarS5.Location = new System.Drawing.Point(317, 175);
            this.lblSonarS5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSonarS5.Name = "lblSonarS5";
            this.lblSonarS5.Size = new System.Drawing.Size(92, 22);
            this.lblSonarS5.TabIndex = 14;
            this.lblSonarS5.Text = "0";
            // 
            // lblSonarS4
            // 
            this.lblSonarS4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSonarS4.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSonarS4.Location = new System.Drawing.Point(317, 151);
            this.lblSonarS4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSonarS4.Name = "lblSonarS4";
            this.lblSonarS4.Size = new System.Drawing.Size(92, 22);
            this.lblSonarS4.TabIndex = 13;
            this.lblSonarS4.Text = "0";
            // 
            // lblSonarS3
            // 
            this.lblSonarS3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSonarS3.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSonarS3.Location = new System.Drawing.Point(317, 128);
            this.lblSonarS3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSonarS3.Name = "lblSonarS3";
            this.lblSonarS3.Size = new System.Drawing.Size(92, 22);
            this.lblSonarS3.TabIndex = 12;
            this.lblSonarS3.Text = "0";
            // 
            // lblSonarS2
            // 
            this.lblSonarS2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSonarS2.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSonarS2.Location = new System.Drawing.Point(317, 105);
            this.lblSonarS2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSonarS2.Name = "lblSonarS2";
            this.lblSonarS2.Size = new System.Drawing.Size(92, 22);
            this.lblSonarS2.TabIndex = 11;
            this.lblSonarS2.Text = "0";
            // 
            // lblSonarS1
            // 
            this.lblSonarS1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSonarS1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSonarS1.Location = new System.Drawing.Point(317, 79);
            this.lblSonarS1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSonarS1.Name = "lblSonarS1";
            this.lblSonarS1.Size = new System.Drawing.Size(92, 22);
            this.lblSonarS1.TabIndex = 10;
            this.lblSonarS1.Text = "0";
            // 
            // lblSonarS0
            // 
            this.lblSonarS0.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSonarS0.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSonarS0.Location = new System.Drawing.Point(317, 57);
            this.lblSonarS0.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSonarS0.Name = "lblSonarS0";
            this.lblSonarS0.Size = new System.Drawing.Size(92, 22);
            this.lblSonarS0.TabIndex = 9;
            this.lblSonarS0.Text = "0";
            // 
            // btnStartSonar
            // 
            this.btnStartSonar.Enabled = false;
            this.btnStartSonar.Location = new System.Drawing.Point(8, 21);
            this.btnStartSonar.Margin = new System.Windows.Forms.Padding(4);
            this.btnStartSonar.Name = "btnStartSonar";
            this.btnStartSonar.Size = new System.Drawing.Size(65, 28);
            this.btnStartSonar.TabIndex = 8;
            this.btnStartSonar.Text = "Start";
            this.btnStartSonar.UseVisualStyleBackColor = true;
            this.btnStartSonar.Click += new System.EventHandler(this.btnStartSonar_Click);
            // 
            // picSonar
            // 
            this.picSonar.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.picSonar.Location = new System.Drawing.Point(8, 57);
            this.picSonar.Margin = new System.Windows.Forms.Padding(4);
            this.picSonar.Name = "picSonar";
            this.picSonar.Size = new System.Drawing.Size(301, 190);
            this.picSonar.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picSonar.TabIndex = 7;
            this.picSonar.TabStop = false;
            // 
            // btnDisconnectSonar
            // 
            this.btnDisconnectSonar.Enabled = false;
            this.btnDisconnectSonar.Location = new System.Drawing.Point(189, 21);
            this.btnDisconnectSonar.Margin = new System.Windows.Forms.Padding(4);
            this.btnDisconnectSonar.Name = "btnDisconnectSonar";
            this.btnDisconnectSonar.Size = new System.Drawing.Size(100, 28);
            this.btnDisconnectSonar.TabIndex = 6;
            this.btnDisconnectSonar.Text = "Disconnect";
            this.btnDisconnectSonar.UseVisualStyleBackColor = true;
            this.btnDisconnectSonar.Click += new System.EventHandler(this.btnDisconnectSonar_Click);
            // 
            // btnConnectSonar
            // 
            this.btnConnectSonar.Enabled = false;
            this.btnConnectSonar.Location = new System.Drawing.Point(81, 21);
            this.btnConnectSonar.Margin = new System.Windows.Forms.Padding(4);
            this.btnConnectSonar.Name = "btnConnectSonar";
            this.btnConnectSonar.Size = new System.Drawing.Size(100, 28);
            this.btnConnectSonar.TabIndex = 5;
            this.btnConnectSonar.Text = "Connect";
            this.btnConnectSonar.UseVisualStyleBackColor = true;
            this.btnConnectSonar.Click += new System.EventHandler(this.btnConnectSonar_Click);
            // 
            // lblSonarDelay
            // 
            this.lblSonarDelay.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSonarDelay.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSonarDelay.Location = new System.Drawing.Point(297, 25);
            this.lblSonarDelay.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSonarDelay.Name = "lblSonarDelay";
            this.lblSonarDelay.Size = new System.Drawing.Size(124, 22);
            this.lblSonarDelay.TabIndex = 4;
            this.lblSonarDelay.Text = "0";
            // 
            // grBoxSonarMap
            // 
            this.grBoxSonarMap.Controls.Add(this.btnReset);
            this.grBoxSonarMap.Controls.Add(this.btnRight);
            this.grBoxSonarMap.Controls.Add(this.btnLeft);
            this.grBoxSonarMap.Controls.Add(this.btnDown);
            this.grBoxSonarMap.Controls.Add(this.btnUp);
            this.grBoxSonarMap.Controls.Add(this.btnCamDisconnect);
            this.grBoxSonarMap.Controls.Add(this.btnCamConnect);
            this.grBoxSonarMap.Controls.Add(this.picCamImage);
            this.grBoxSonarMap.Location = new System.Drawing.Point(967, 33);
            this.grBoxSonarMap.Margin = new System.Windows.Forms.Padding(4);
            this.grBoxSonarMap.Name = "grBoxSonarMap";
            this.grBoxSonarMap.Padding = new System.Windows.Forms.Padding(4);
            this.grBoxSonarMap.Size = new System.Drawing.Size(225, 271);
            this.grBoxSonarMap.TabIndex = 18;
            this.grBoxSonarMap.TabStop = false;
            this.grBoxSonarMap.Text = "Onboard Camera";
            // 
            // btnReset
            // 
            this.btnReset.Enabled = false;
            this.btnReset.Location = new System.Drawing.Point(103, 242);
            this.btnReset.Margin = new System.Windows.Forms.Padding(4);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(105, 28);
            this.btnReset.TabIndex = 15;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnRight
            // 
            this.btnRight.Enabled = false;
            this.btnRight.Location = new System.Drawing.Point(155, 209);
            this.btnRight.Margin = new System.Windows.Forms.Padding(4);
            this.btnRight.Name = "btnRight";
            this.btnRight.Size = new System.Drawing.Size(53, 28);
            this.btnRight.TabIndex = 14;
            this.btnRight.Text = "Right";
            this.btnRight.UseVisualStyleBackColor = true;
            this.btnRight.Click += new System.EventHandler(this.btnRight_Click_1);
            // 
            // btnLeft
            // 
            this.btnLeft.Enabled = false;
            this.btnLeft.Location = new System.Drawing.Point(103, 209);
            this.btnLeft.Margin = new System.Windows.Forms.Padding(4);
            this.btnLeft.Name = "btnLeft";
            this.btnLeft.Size = new System.Drawing.Size(44, 28);
            this.btnLeft.TabIndex = 13;
            this.btnLeft.Text = "Left";
            this.btnLeft.UseVisualStyleBackColor = true;
            this.btnLeft.Click += new System.EventHandler(this.btnLeft_Click_1);
            // 
            // btnDown
            // 
            this.btnDown.Enabled = false;
            this.btnDown.Location = new System.Drawing.Point(21, 242);
            this.btnDown.Margin = new System.Windows.Forms.Padding(4);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(59, 28);
            this.btnDown.TabIndex = 12;
            this.btnDown.Text = "Down";
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // btnUp
            // 
            this.btnUp.Enabled = false;
            this.btnUp.Location = new System.Drawing.Point(21, 209);
            this.btnUp.Margin = new System.Windows.Forms.Padding(4);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(59, 28);
            this.btnUp.TabIndex = 11;
            this.btnUp.Text = "Up";
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // btnCamDisconnect
            // 
            this.btnCamDisconnect.Enabled = false;
            this.btnCamDisconnect.Location = new System.Drawing.Point(116, 20);
            this.btnCamDisconnect.Margin = new System.Windows.Forms.Padding(4);
            this.btnCamDisconnect.Name = "btnCamDisconnect";
            this.btnCamDisconnect.Size = new System.Drawing.Size(100, 28);
            this.btnCamDisconnect.TabIndex = 10;
            this.btnCamDisconnect.Text = "Disconnect";
            this.btnCamDisconnect.UseVisualStyleBackColor = true;
            this.btnCamDisconnect.Click += new System.EventHandler(this.btnCamDisconnect_Click);
            // 
            // btnCamConnect
            // 
            this.btnCamConnect.Enabled = false;
            this.btnCamConnect.Location = new System.Drawing.Point(8, 18);
            this.btnCamConnect.Margin = new System.Windows.Forms.Padding(4);
            this.btnCamConnect.Name = "btnCamConnect";
            this.btnCamConnect.Size = new System.Drawing.Size(100, 28);
            this.btnCamConnect.TabIndex = 9;
            this.btnCamConnect.Text = "Connect";
            this.btnCamConnect.UseVisualStyleBackColor = true;
            this.btnCamConnect.Click += new System.EventHandler(this.btnCamConnect_Click);
            // 
            // picCamImage
            // 
            this.picCamImage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.picCamImage.Location = new System.Drawing.Point(7, 60);
            this.picCamImage.Margin = new System.Windows.Forms.Padding(4);
            this.picCamImage.Name = "picCamImage";
            this.picCamImage.Size = new System.Drawing.Size(213, 148);
            this.picCamImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picCamImage.TabIndex = 8;
            this.picCamImage.TabStop = false;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btn_b13);
            this.groupBox3.Controls.Add(this.btn_b12);
            this.groupBox3.Controls.Add(this.btn_b9);
            this.groupBox3.Controls.Add(this.btn_b10);
            this.groupBox3.Controls.Add(this.btn_b11);
            this.groupBox3.Controls.Add(this.label19);
            this.groupBox3.Controls.Add(this.label18);
            this.groupBox3.Controls.Add(this.btn_b5);
            this.groupBox3.Controls.Add(this.btn_b1);
            this.groupBox3.Controls.Add(this.btn_b4);
            this.groupBox3.Controls.Add(this.btn_b2);
            this.groupBox3.Controls.Add(this.btn_b3);
            this.groupBox3.Location = new System.Drawing.Point(16, 543);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox3.Size = new System.Drawing.Size(499, 167);
            this.groupBox3.TabIndex = 20;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Bumpers";
            this.groupBox3.Enter += new System.EventHandler(this.groupBox3_Enter_1);
            // 
            // btn_b13
            // 
            this.btn_b13.Location = new System.Drawing.Point(119, 98);
            this.btn_b13.Margin = new System.Windows.Forms.Padding(4);
            this.btn_b13.Name = "btn_b13";
            this.btn_b13.Size = new System.Drawing.Size(48, 26);
            this.btn_b13.TabIndex = 11;
            this.btn_b13.Text = "b13";
            this.btn_b13.UseVisualStyleBackColor = true;
            // 
            // btn_b12
            // 
            this.btn_b12.Location = new System.Drawing.Point(175, 112);
            this.btn_b12.Margin = new System.Windows.Forms.Padding(4);
            this.btn_b12.Name = "btn_b12";
            this.btn_b12.Size = new System.Drawing.Size(48, 26);
            this.btn_b12.TabIndex = 10;
            this.btn_b12.Text = "b12";
            this.btn_b12.UseVisualStyleBackColor = true;
            // 
            // btn_b9
            // 
            this.btn_b9.Location = new System.Drawing.Point(345, 98);
            this.btn_b9.Margin = new System.Windows.Forms.Padding(4);
            this.btn_b9.Name = "btn_b9";
            this.btn_b9.Size = new System.Drawing.Size(48, 26);
            this.btn_b9.TabIndex = 9;
            this.btn_b9.Text = "b9";
            this.btn_b9.UseVisualStyleBackColor = true;
            // 
            // btn_b10
            // 
            this.btn_b10.Location = new System.Drawing.Point(289, 112);
            this.btn_b10.Margin = new System.Windows.Forms.Padding(4);
            this.btn_b10.Name = "btn_b10";
            this.btn_b10.Size = new System.Drawing.Size(48, 26);
            this.btn_b10.TabIndex = 8;
            this.btn_b10.Text = "b10";
            this.btn_b10.UseVisualStyleBackColor = true;
            // 
            // btn_b11
            // 
            this.btn_b11.Location = new System.Drawing.Point(231, 127);
            this.btn_b11.Margin = new System.Windows.Forms.Padding(4);
            this.btn_b11.Name = "btn_b11";
            this.btn_b11.Size = new System.Drawing.Size(48, 26);
            this.btn_b11.TabIndex = 7;
            this.btn_b11.Text = "b11";
            this.btn_b11.UseVisualStyleBackColor = true;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(8, 98);
            this.label19.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(99, 17);
            this.label19.TabIndex = 6;
            this.label19.Text = "Rear Bumpers";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(11, 46);
            this.label18.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(101, 17);
            this.label18.TabIndex = 5;
            this.label18.Text = "Front Bumpers";
            // 
            // btn_b5
            // 
            this.btn_b5.Location = new System.Drawing.Point(345, 50);
            this.btn_b5.Margin = new System.Windows.Forms.Padding(4);
            this.btn_b5.Name = "btn_b5";
            this.btn_b5.Size = new System.Drawing.Size(48, 26);
            this.btn_b5.TabIndex = 4;
            this.btn_b5.Text = "b5";
            this.btn_b5.UseVisualStyleBackColor = true;
            // 
            // btn_b1
            // 
            this.btn_b1.Location = new System.Drawing.Point(119, 52);
            this.btn_b1.Margin = new System.Windows.Forms.Padding(4);
            this.btn_b1.Name = "btn_b1";
            this.btn_b1.Size = new System.Drawing.Size(48, 26);
            this.btn_b1.TabIndex = 3;
            this.btn_b1.Text = "b1";
            this.btn_b1.UseVisualStyleBackColor = true;
            // 
            // btn_b4
            // 
            this.btn_b4.Location = new System.Drawing.Point(289, 36);
            this.btn_b4.Margin = new System.Windows.Forms.Padding(4);
            this.btn_b4.Name = "btn_b4";
            this.btn_b4.Size = new System.Drawing.Size(48, 26);
            this.btn_b4.TabIndex = 2;
            this.btn_b4.Text = "b4";
            this.btn_b4.UseVisualStyleBackColor = true;
            // 
            // btn_b2
            // 
            this.btn_b2.Location = new System.Drawing.Point(175, 36);
            this.btn_b2.Margin = new System.Windows.Forms.Padding(4);
            this.btn_b2.Name = "btn_b2";
            this.btn_b2.Size = new System.Drawing.Size(48, 26);
            this.btn_b2.TabIndex = 1;
            this.btn_b2.Text = "b2";
            this.btn_b2.UseVisualStyleBackColor = true;
            // 
            // btn_b3
            // 
            this.btn_b3.Location = new System.Drawing.Point(231, 23);
            this.btn_b3.Margin = new System.Windows.Forms.Padding(4);
            this.btn_b3.Name = "btn_b3";
            this.btn_b3.Size = new System.Drawing.Size(48, 26);
            this.btn_b3.TabIndex = 0;
            this.btn_b3.Text = "b3";
            this.btn_b3.UseVisualStyleBackColor = true;
            this.btn_b3.Click += new System.EventHandler(this.button1_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.simulImage);
            this.groupBox4.Controls.Add(this.checkBoxHideUI);
            this.groupBox4.Location = new System.Drawing.Point(525, 320);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox4.Size = new System.Drawing.Size(433, 390);
            this.groupBox4.TabIndex = 21;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Simulation - Pursuit Camera";
            // 
            // simulImage
            // 
            this.simulImage.Location = new System.Drawing.Point(5, 63);
            this.simulImage.Margin = new System.Windows.Forms.Padding(4);
            this.simulImage.Name = "simulImage";
            this.simulImage.Size = new System.Drawing.Size(427, 295);
            this.simulImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.simulImage.TabIndex = 1;
            this.simulImage.TabStop = false;
            // 
            // checkBoxHideUI
            // 
            this.checkBoxHideUI.AutoSize = true;
            this.checkBoxHideUI.Location = new System.Drawing.Point(17, 31);
            this.checkBoxHideUI.Margin = new System.Windows.Forms.Padding(4);
            this.checkBoxHideUI.Name = "checkBoxHideUI";
            this.checkBoxHideUI.Size = new System.Drawing.Size(130, 21);
            this.checkBoxHideUI.TabIndex = 0;
            this.checkBoxHideUI.Text = "Hide external UI";
            this.checkBoxHideUI.UseVisualStyleBackColor = true;
            this.checkBoxHideUI.CheckedChanged += new System.EventHandler(this.checkBoxHideUI_CheckedChanged);
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.lblGPSAngle);
            this.groupBox6.Controls.Add(this.label13);
            this.groupBox6.Controls.Add(this.lblGPSZ);
            this.groupBox6.Controls.Add(this.label11);
            this.groupBox6.Controls.Add(this.lblGPStime);
            this.groupBox6.Controls.Add(this.label10);
            this.groupBox6.Controls.Add(this.lblGPSY);
            this.groupBox6.Controls.Add(this.lblGPSX);
            this.groupBox6.Controls.Add(this.label9);
            this.groupBox6.Controls.Add(this.label8);
            this.groupBox6.Location = new System.Drawing.Point(971, 320);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(221, 178);
            this.groupBox6.TabIndex = 22;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "GPS Position";
            this.groupBox6.Enter += new System.EventHandler(this.groupBox6_Enter);
            // 
            // lblGPSZ
            // 
            this.lblGPSZ.AutoSize = true;
            this.lblGPSZ.Location = new System.Drawing.Point(44, 59);
            this.lblGPSZ.Name = "lblGPSZ";
            this.lblGPSZ.Size = new System.Drawing.Size(16, 17);
            this.lblGPSZ.TabIndex = 7;
            this.lblGPSZ.Text = "0";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(14, 58);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(21, 17);
            this.label11.TabIndex = 6;
            this.label11.Text = "Z:";
            // 
            // lblGPStime
            // 
            this.lblGPStime.AutoSize = true;
            this.lblGPStime.Location = new System.Drawing.Point(103, 143);
            this.lblGPStime.Name = "lblGPStime";
            this.lblGPStime.Size = new System.Drawing.Size(16, 17);
            this.lblGPStime.TabIndex = 5;
            this.lblGPStime.Text = "0";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(14, 143);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(83, 17);
            this.label10.TabIndex = 4;
            this.label10.Text = "TimeStamp:";
            // 
            // lblGPSY
            // 
            this.lblGPSY.AutoSize = true;
            this.lblGPSY.Location = new System.Drawing.Point(44, 86);
            this.lblGPSY.Name = "lblGPSY";
            this.lblGPSY.Size = new System.Drawing.Size(16, 17);
            this.lblGPSY.TabIndex = 3;
            this.lblGPSY.Text = "0";
            // 
            // lblGPSX
            // 
            this.lblGPSX.AutoSize = true;
            this.lblGPSX.Location = new System.Drawing.Point(44, 31);
            this.lblGPSX.MinimumSize = new System.Drawing.Size(4, 0);
            this.lblGPSX.Name = "lblGPSX";
            this.lblGPSX.Size = new System.Drawing.Size(16, 17);
            this.lblGPSX.TabIndex = 2;
            this.lblGPSX.Text = "0";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(14, 86);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(21, 17);
            this.label9.TabIndex = 1;
            this.label9.Text = "Y:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(14, 31);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(21, 17);
            this.label8.TabIndex = 0;
            this.label8.Text = "X:";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(14, 116);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(48, 17);
            this.label13.TabIndex = 8;
            this.label13.Text = "Angle:";
            // 
            // lblGPSAngle
            // 
            this.lblGPSAngle.AutoSize = true;
            this.lblGPSAngle.Location = new System.Drawing.Point(68, 116);
            this.lblGPSAngle.Name = "lblGPSAngle";
            this.lblGPSAngle.Size = new System.Drawing.Size(16, 17);
            this.lblGPSAngle.TabIndex = 9;
            this.lblGPSAngle.Text = "0";
            // 
            // DriveControl
            // 
            this.AcceptButton = this.btnConnect;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(1219, 741);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.grBoxSonarMap);
            this.Controls.Add(this.SonarGroupBox);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBoxLRF);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.MainMenu);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(1225, 771);
            this.Name = "DriveControl";
            this.Text = "CRANIUM Dashboard - www.Conscious-Robots.com";
            this.Load += new System.EventHandler(this.DriveControl_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.DriveControl_FormClosed);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picJoystick)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBoxLRF.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picLRF)).EndInit();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.MainMenu.ResumeLayout(false);
            this.MainMenu.PerformLayout();
            this.SonarGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picSonar)).EndInit();
            this.grBoxSonarMap.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picCamImage)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.simulImage)).EndInit();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbJoystick;
        private System.Windows.Forms.Label lblX;
        private System.Windows.Forms.Label lblY;
        private System.Windows.Forms.Label lblZ;
        private System.Windows.Forms.Label lblButtons;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListBox listDirectory;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.MaskedTextBox txtPort;
        private System.Windows.Forms.TextBox txtMachine;
        private System.Windows.Forms.PictureBox picJoystick;
        private System.Windows.Forms.GroupBox groupBoxLRF;
        private System.Windows.Forms.Button btnConnectLRF;
        private System.Windows.Forms.Button btnStartLRF;
        private System.Windows.Forms.PictureBox picLRF;
        private System.Windows.Forms.CheckBox chkStop;
        private System.Windows.Forms.CheckBox chkDrive;
        private System.Windows.Forms.Button btnDisconnect;
        private System.Windows.Forms.Label lblDelay;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.TextBox txtLogFile;
        private System.Windows.Forms.CheckBox chkLog;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.Label lblNode;
        private System.Windows.Forms.LinkLabel linkDirectory;
        private System.Windows.Forms.MenuStrip MainMenu;
        private System.Windows.Forms.ToolStripMenuItem FileMenu;
        private System.Windows.Forms.ToolStripMenuItem saveSettingsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ToolsMenu;
        private System.Windows.Forms.ToolStripMenuItem optionsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem HelpMenu;
        private System.Windows.Forms.ToolStripMenuItem aboutMenuItem;
        private System.Windows.Forms.GroupBox SonarGroupBox;
        private System.Windows.Forms.Label lblSonarDelay;
        private System.Windows.Forms.Button btnDisconnectSonar;
        private System.Windows.Forms.Button btnConnectSonar;
        private System.Windows.Forms.PictureBox picSonar;
        private System.Windows.Forms.Button btnStartSonar;
        private System.Windows.Forms.Label lblSonarS0;
        private System.Windows.Forms.Label lblSonarS1;
        private System.Windows.Forms.Label lblSonarS4;
        private System.Windows.Forms.Label lblSonarS3;
        private System.Windows.Forms.Label lblSonarS2;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label lblSonarS7;
        private System.Windows.Forms.Label lblSonarS6;
        private System.Windows.Forms.Label lblSonarS5;
        private System.Windows.Forms.GroupBox grBoxSonarMap;
        private System.Windows.Forms.PictureBox picCamImage;
        private System.Windows.Forms.Button btnCamDisconnect;
        private System.Windows.Forms.Button btnCamConnect;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btn_b3;
        private System.Windows.Forms.Button btn_b4;
        private System.Windows.Forms.Button btn_b2;
        private System.Windows.Forms.Button btn_b5;
        private System.Windows.Forms.Button btn_b1;
        private System.Windows.Forms.Button btn_b13;
        private System.Windows.Forms.Button btn_b12;
        private System.Windows.Forms.Button btn_b9;
        private System.Windows.Forms.Button btn_b10;
        private System.Windows.Forms.Button btn_b11;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.CheckBox checkBoxHideUI;
        private System.Windows.Forms.PictureBox simulImage;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnRight;
        private System.Windows.Forms.Button btnLeft;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label lblGPSX;
        private System.Windows.Forms.Label lblGPSY;
        private System.Windows.Forms.Label lblGPStime;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label lblGPSZ;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label lblGPSAngle;
        private System.Windows.Forms.Label label13;
    }
}