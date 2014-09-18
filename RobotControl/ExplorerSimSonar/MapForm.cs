//-----------------------------------------------------------------------
//  This file is part of the Explorer Simulation example 
//  Copyright (C) 2007 by Trevor Taylor, QUT.
//  This code is freely available for non-commercial use.
//
//  Form for displaying a Map
//
//  $File: MapForm.cs $ $Revision: 1 $
//
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;


namespace Microsoft.Robotics.Services.ExplorerSim
{
    public partial class MapForm : Form
    {
        // Class for the Windows Form
        public MapForm()
        {
            InitializeComponent();
        }

        // Local copy of the image
        private Bitmap _MapImage;

        public Bitmap MapImage
        {
            get { return _MapImage; }
            set
            {
                _MapImage = value;

                Image old = picMap.Image;
                picMap.Image = value;

                // Took out this code
                // If the same bitmap is used and simply updated,
                // then we don't want to dispose it!!!
                /*
                if (old != null)
                {
                    old.Dispose();
                }
                */
            }
        }
    }
}
