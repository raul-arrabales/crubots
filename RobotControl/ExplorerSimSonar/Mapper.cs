//-----------------------------------------------------------------------
//  Mapper.cs - Code to construct an Occupancy Grid Map
//  This file is part of the Explorer Simulation example 
//  Copyright (C) 2007 by Trevor Taylor, QUT.
//  This code is freely available for non-commercial use.
//
//  $File: Mapper.cs $ $Revision: 1 $
//
// The code to trace rays through the map was taken from
// Andrew Howard's Simple Mapping Utilities (pmap):
// http://www-robotics.usc.edu/~ahoward/pmap/index.html
// Refer to the omap.c module.
// pmap is available under a GNU GPL, which might therefore
// mean that ExplorerSim also falls under this licence.
// However, the amount of code used is minimal.
//
// NOTES:
// This is not a good example of code design. It is intended
// simply to demonstrate some principles. For instance, the Map
// should be a separate class. Also it makes some strong
// assumptions about the sensor, which in this case is a LRF.
//
// Updates using Bayes Rule are not yet implemented.
//
// Sometimes the robot stops abruptly and this causes it to tilt
// forwards. The laser then scans the floor. This puts bad info
// into the map.
//
// If you fiddle with the Simulator window (zooming, resizing, etc.)
// this can impact on the simulation. Consequently there might
// be some data drawn into the map with a pose that is slightly
// wrong. This is not a big deal.
// 
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

using Microsoft.Dss.Core.Attributes;


namespace Microsoft.Robotics.Services.ExplorerSim
{
    /// <summary>
    /// Modes (methods) for producing a map 
    /// </summary>
    [DataContract]
    public enum MapMode
    {
        Overwrite,      // Just write new data (free or occupied) into the map
        Counting,       // Count the number of times a cell is seen free (or occ)
        BayesRule       // Use Bayes Rule to update cell values
    }

    // Values for the occupancy grid
    // Because the values are stored as a single byte,
    // they range from 0 to 255. However, there are
    // thresholds for occupied/vacant cells because
    // it is not necessary for the probabilities to
    // reach 255 or 0 respectively.
    // The range is arbitrarily divided into three sections:
    // Occupied =   0 ->  64
    // Unknown  =  65 -> 191
    // Vacant   = 192 -> 255
    // This conforms to the convention that is commonly used for
    // occupancy grids in the literature where:
    // Black (Occupied) represents obstacles, 
    // Grey  (Unknown) is unknown, and
    // White (Free) is free (clear or empty) space
    // For convenience, two other values (and aliases) are also
    // defined, although they might be a little confusing:
    // OBSTACLE = MAP_UNKNOWN_LOW is the UPPER bound for an
    // obstacle, or the LOWER bound for unknown space
    // MAP_VACANT = MAP_UNKNOWN_HIGH is the LOWER bound for free
    // (vacant) space, or the UPPER bound for unknown space
    public enum MapValues
    {
        OCCUPIED = 0,
        OBSTACLE = 64,
        // Value for 50% probabilty of occupied, i.e. unknown space
        UNKNOWN = 128,
        VACANT = 192,
        FREE = 255,

        // Range for Unknown (synonyms for OBSTACLE and VACANT)
        UNKNOWN_LOW = 64,
        UNKNOWN_HIGH = 192
    }


    /// <summary>
    /// Mapper - Class for performing mapping
    /// </summary>
    class Mapper
    {
        /// <summary>
        /// Drawing mode for the map
        /// </summary>
        public MapMode mode = MapMode.Overwrite;

        // Number of range points in each scan
        private int _num_ranges;

        // Maximum accepted range
        private double _range_max;

        // Start angle for scans
        private double _range_start;

        // Angular resolution of scans
        private double _range_step;

        // Grid resolution and dimensions
        private double _grid_res;
        private int _grid_sx, _grid_sy, _grid_size;

        // Conversion factor for range scans
        // Scans are in millimeters, but we work in meters
        private const int _range_factor = 1000;

        // The actual map itself is a 2D array of bytes        
        private byte[,] _mapData = null;


        // Raul - Flag for sonar data.
        private bool _useSonarData;


        // Raul - Sonar transducer orientation for P3DX frontal sonar Ring
        private double[] sonarRadians;
        private int[] sonarDegrees;



        public byte[,] GetMapData()
        {
            return _mapData;
        }

        // The local map that is used during updates with Bayes Rule
        private byte[,] _localMapData = null;

        // This was an attempt to encapsulate the map image,
        // but there are problems with threading and the GDI
        private static Bitmap _mapImage = null;
        private static Graphics g = null;

        public Bitmap GetMapImage()
        {
            return _mapImage;
        }




        // Raul - Init Mapper for Sonar data
        /// <summary>
        /// Init - Set parameters and do the necessary allocations
        /// </summary>
        /// <remarks>
        /// NOTE: This method must be called before any others!
        /// </remarks>
        /// <param name="useSonarData">Flag to indicate sonar data</param>
        /// <param name="num_ranges">Number of scans</param>
        /// <param name="range_max">Max range</param>
        /// <param name="range_start">Start angle for scan</param>
        /// <param name="range_step">Increment between scans</param>
        /// <param name="width">Width of map in meters</param>
        /// <param name="height">Height of map in meters</param>
        /// <param name="grid_res">Resolution of map in meters (grid size)</param>
        /// <returns></returns>
        public Bitmap Init(bool useSonarData, int num_ranges, double range_max,
                   double range_start, double range_step,
                   double width, double height, double grid_res)
        {
            // Record range settings
            _num_ranges = num_ranges;
            _range_max = range_max;
            _range_start = range_start;
            _range_step = range_step;

            // Raul - Indicates if ranges come from a Sonar instead of LRF.
            _useSonarData = useSonarData;

            // Sonar transducers orientation
            if (useSonarData)
            {
                sonarRadians = new double[8];
                sonarRadians[0] = (Math.PI * 90) / 180;
                sonarRadians[1] = (Math.PI * 50) / 180;
                sonarRadians[2] = (Math.PI * 30) / 180;
                sonarRadians[3] = (Math.PI * 10) / 180;
                sonarRadians[4] = -(Math.PI * 10) / 180;
                sonarRadians[5] = -(Math.PI * 30) / 180;
                sonarRadians[6] = -(Math.PI * 50) / 180;
                sonarRadians[7] = -(Math.PI * 90) / 180;

                sonarDegrees = new int[8];
                sonarDegrees[0] = 90;
                sonarDegrees[1] = 50;
                sonarDegrees[2] = 30;
                sonarDegrees[3] = 10;
                sonarDegrees[4] = -10;
                sonarDegrees[5] = -30;
                sonarDegrees[6] = -50;
                sonarDegrees[7] = -90;

            }
            
            // Allocate space for grid
            _grid_res = grid_res;
            _grid_sx = (int)Math.Ceiling(width / _grid_res);
            // Ensure that the X axis has a multiple of 4 pixels,
            // i.e. it will be longword aligned when the bitmap
            // image is created
            if ((_grid_sx % 4) != 0)
            {
                _grid_sx += 4 - (_grid_sx % 4);
            }
            _grid_sy = (int)Math.Ceiling(height / _grid_res);
            _grid_size = _grid_sx * _grid_sy;
            // Allocate memory for the maps
            _mapData = new byte[_grid_sx, _grid_sy];
            _localMapData = new byte[_grid_sx, _grid_sy];

            Bitmap _mapImage = new Bitmap(_grid_sx, _grid_sy, PixelFormat.Format24bppRgb);
            g = Graphics.FromImage(_mapImage);
            g.Clear(Color.LightGray);

            // Clear() could probably be called as a convenience

            return _mapImage;
        }


        /// <summary>
        /// Clear() - Erase the map
        /// </summary>
        public void Clear()
        {
            int i, j;
            for (j=0; j<_grid_sy; j++)
                for (i=0; i<_grid_sx; i++)
                    _mapData[i, j] = (byte) MapValues.UNKNOWN;

            return;
        }

        private void ClearLocal()
        {
            int i, j;
            for (j = 0; j < _grid_sy; j++)
                for (i = 0; i < _grid_sx; i++)
                    _localMapData[i, j] = (byte) MapValues.UNKNOWN;

            return;
        }

        // Convert from real-world coord to map coord
        private int MapGridX(double x)
        {
            return (int)Math.Floor(x / _grid_res) + _grid_sx / 2;
        }

        private int MapGridY(double y)
        {
            return (int)Math.Floor(y / _grid_res) + _grid_sy / 2;
        }

        // Check that map coords are inside the array
        private bool MapGridValid(int x, int y)
        {
            return ((x >= 0) && (x < _grid_sx) &&
                    (y >= 0) && (y < _grid_sy));
        }


        /// <summary>
        /// Add - Adds a new laser range scan to the map
        /// </summary>
        /// <param name="x">Robot position</param>
        /// <param name="y">Robot position</param>
        /// <param name="theta">Robot orientation</param>
        /// <param name="num_ranges">Number of range values (redundant)</param>
        /// <param name="ranges">Array of range values in millimeters</param>
        public void Add(float x, float y, float theta, int num_ranges, int[] ranges)
        {
            // Raul - Call the sonar customized methos if using Sonar
            if (_useSonarData)
            {
                AddSonarScan(x, y, theta, num_ranges, ranges);
                return;
            }

            int i, j;
            double r, dx, dy, dr;
            //vector2_t a, b, p;
            double ax, ay, bx, by, px, py;
            int step_count;
            double step_size;
            //int nx, ny, nindex, occ;
            int nx, ny, occ;
            bool maxed;
            double angle;

            // Make sure we hit every grid cell
            step_size = _grid_res;

            //a = pose.pos;
            ax = x;
            ay = y;

            // Erase the local map if using Bayes Rule
            if (mode == MapMode.BayesRule)
                ClearLocal();

            // Ray-trace the grid
            for (i = 0; i < num_ranges; i++)
            {
                r = (double)ranges[i] / _range_factor;
                // There is a fudge here due to a bug in MSRS that returns
                // zero instead of max range for a laser miss
                if (r >= _range_max || r == 0)
                {
                    r = _range_max;
                    maxed = true;
                }
                else
                    maxed = false;

                // Compute range end-point
                // NOTE: Scans are backwards, i.e. right to left
                angle = -_range_start + i * _range_step;
                // Now adjust for the robot's orientation
                angle += theta;
                // Convert to Radians
                angle = angle * Math.PI / 180;
                // Calculate the actual coords
                // Note that r is relative to the robot
                bx = r * Math.Cos(angle) + ax;
                by = r * Math.Sin(angle) + ay;

                // Compute line parameters
                dx = bx - ax;
                dy = by - ay;
                dr = Math.Sqrt(dx * dx + dy * dy);

                step_count = (int)Math.Floor(dr / step_size) + 2;
                dx /= (step_count - 1);
                dy /= (step_count - 1);

                // Just to keep the compiler happy ...
                occ = (int) MapValues.UNKNOWN;

                // Walk the line and update the grid
                for (j = 0; j < step_count; j++)
                {
                    px = ax + dx * j;
                    py = ay + dy * j;

                    nx = MapGridX(px);
                    ny = MapGridY(py);

                    if (MapGridValid(nx, ny))
                    {
                        switch (mode)
                        {
                            case MapMode.Counting:
                            default:
                                occ = (int)_mapData[nx, ny] + 1;
                                if (occ > 255)
                                    occ = 255;
                                break;

                            case MapMode.Overwrite:
                                occ = 255;
                                break;

                            case MapMode.BayesRule:
                                occ = (int) MapValues.VACANT;
                                break;
                        }

                        if (mode == MapMode.BayesRule)
                            _localMapData[nx, ny] = (byte)occ;
                        else
                            _mapData[nx, ny] = (byte)occ;
                    }
                }

                // Place an obstacle at the end of the ray if the scan
                // was not maxed out (a "miss" returns the max range value)
                if (!maxed)
                {
                    px = ax + dx * step_count;
                    py = ay + dy * step_count;

                    nx = MapGridX(px);
                    ny = MapGridY(py);

                    if (MapGridValid(nx, ny))
                    {
                        switch (mode)
                        {
                            case MapMode.Counting:
                            default:
                                occ = (int)_mapData[nx, ny] - 1;
                                if (occ < 0)
                                    occ = 0;
                                break;

                            case MapMode.Overwrite:
                                occ = 0;
                                break;

                            case MapMode.BayesRule:
                                occ = (int) MapValues.OBSTACLE;
                                break;
                        }

                        if (mode == MapMode.BayesRule)
                            _localMapData[nx, ny] = (byte)occ;
                        else
                            _mapData[nx, ny] = (byte)occ;
                    }
                }
            }

            if (mode == MapMode.BayesRule)
                UpdateMapBayes();

            return;
        }


        // Raul - Add sonar scan
        /// <summary>
        /// Add - Adds a new sonar range scan to the map
        /// </summary>
        /// <param name="x">Robot position</param>
        /// <param name="y">Robot position</param>
        /// <param name="theta">Robot orientation</param>
        /// <param name="num_ranges">Number of range values (redundant)</param>
        /// <param name="ranges">Array of range values in millimeters</param>
        public void AddSonarScan(float x, float y, float theta, int num_ranges, int[] ranges)
        {
            int i, j;
            double r, dx, dy, dr;
            //vector2_t a, b, p;
            double ax, ay, bx, by, px, py;
            int step_count;
            double step_size;
            //int nx, ny, nindex, occ;
            int nx, ny, occ;
            bool maxed;
            double angle;
            // Raul - sonar transducer centered angle
            double sonarBisectorAngle;

            // Make sure we hit every grid cell
            step_size = _grid_res;

            //a = pose.pos;
            ax = x;
            ay = y;

            // Erase the local map if using Bayes Rule
            if (mode == MapMode.BayesRule)
                ClearLocal();

            // Ray-trace the grid
            // Raul - For each transducer
            for (i = 0; i < num_ranges; i++)
            {
                r = (double)ranges[i] / _range_factor;
                // There is a fudge here due to a bug in MSRS that returns
                // zero instead of max range for a laser miss
                if (r >= _range_max || r == 0)
                {
                    r = _range_max;
                    maxed = true;
                }
                else
                    maxed = false;

                // Compute range end-point
                // NOTE: Scans are backwards, i.e. right to left


                // Raul - P3DX Sonar
                sonarBisectorAngle = sonarDegrees[i]; // Sonar transducer 
                sonarBisectorAngle = sonarBisectorAngle + theta;              


                // Raul - For each degree in the sonar transduce cone
                // Raul - Cone span is 15 degrees.
                for (double deg =-8; deg < 8; deg+=0.5)
                {

                    // Raul - Convert angle to radians
                    // angle = (angle + deg) * Math.PI / 180;
                    angle = ((sonarBisectorAngle+deg) * Math.PI) / 180;


                    // Calculate the actual coords
                    // Note that r is relative to the robot
                    bx = r * Math.Cos(angle) + ax;
                    by = r * Math.Sin(angle) + ay;

                    // Compute line parameters
                    dx = bx - ax;
                    dy = by - ay;
                    dr = Math.Sqrt(dx * dx + dy * dy);

                    step_count = (int)Math.Floor(dr / step_size) + 2;
                    dx /= (step_count - 1);
                    dy /= (step_count - 1);

                    // Just to keep the compiler happy ...
                    occ = (int)MapValues.UNKNOWN;

                    // Walk the line and update the grid
                    for (j = 0; j < step_count; j++)
                    {
                        px = ax + dx * j;
                        py = ay + dy * j;

                        nx = MapGridX(px);
                        ny = MapGridY(py);

                        if (MapGridValid(nx, ny))
                        {
                            switch (mode)
                            {
                                case MapMode.Counting:
                                default:
                                    occ = (int)_mapData[nx, ny] + 1;
                                    if (occ > 255)
                                        occ = 255;
                                    break;

                                case MapMode.Overwrite:
                                    occ = 255;
                                    break;

                                case MapMode.BayesRule:
                                    occ = (int)MapValues.VACANT;
                                    break;
                            }

                            if (mode == MapMode.BayesRule)
                                _localMapData[nx, ny] = (byte)occ;
                            else
                                _mapData[nx, ny] = (byte)occ;
                        }
                    }

                
                    // Place an obstacle at the end of the ray if the scan
                    // was not maxed out (a "miss" returns the max range value)
                    if (!maxed && deg==0)
                    {
                        px = ax + dx * step_count;
                        py = ay + dy * step_count;

                        nx = MapGridX(px);
                        ny = MapGridY(py);
                        
                        if (MapGridValid(nx, ny))
                        {
                            switch (mode)
                            {
                                case MapMode.Counting:
                                default:
                                    occ = (int)_mapData[nx, ny] - 1;
                                    if (occ < 0)
                                        occ = 0;
                                    break;

                                case MapMode.Overwrite:
                                    occ = 0;
                                    break;

                                case MapMode.BayesRule:
                                    occ = (int)MapValues.OBSTACLE;
                                    break;
                            }

                            if (mode == MapMode.BayesRule)
                                _localMapData[nx, ny] = (byte)occ;
                            else
                                _mapData[nx, ny] = (byte)occ;
                        }
                         
                    }
                 }
            }

            if (mode == MapMode.BayesRule)
                UpdateMapBayes();

            return;
        }





        // Update the map using Bayes Rule
        private void UpdateMapBayes()
        {
	        int lx, ly;			// Coords in local map
	        int gx, gy;			// Coords in global map
	        //int rx, ry;			// Robot x,y on the global image
	        //int cx, cy;			// Robot x,y on the local map (Centre of map)
	        int localWidth, localHeight;
	        int globalWidth, globalHeight;
	        float p, occ, occOld, occNew;

            // Maybe the dimensions will be different one day ...
            localWidth = _grid_sx;
            localHeight = _grid_sy;
            globalWidth = _grid_sx;
            globalHeight = _grid_sy;

            for (ly = 0; ly < localHeight; ly++)
            {
                // Not used now
                //gy = ry + (ly - cy);
                gy = ly;
                // Protection in case we run off the edges of the
                // global map, which can happen!
                if ((gy >= 0) && (gy < globalHeight))
                {
                    for (lx = 0; lx < localWidth; lx++)
                    {
                        // Only update if the new probability is NOT
                        // equal to UNKNOWN, i.e. we have some new knowledge
                        // about this cell
                        if (_localMapData[lx,ly] != (byte)MapValues.UNKNOWN)
                        {
                            // occNew is the new probability of NOT being occupied
                            // (This is because the map is inverted as far as
                            // probability is concerned in order to match the
                            // standard convention for colours)
                            occNew = (float)_localMapData[lx,ly];
                            // We have to check for the extreme values to
                            // avoid problems in the calculations later
                            if (occNew == 0.0f)
                                occNew = 1.0f;
                            if (occNew > 254.0f)
                                occNew = 254.0f;
                            occNew = occNew / 255.0f;

                            // Not used now
                            //gx = rx + (lx - cx);
                            gx = lx;
                            // Insurance against stepping off the map ...
                            if ((gx >= 0) && (gx < globalWidth))
                            {
                                // Calculate the current (old) probability
                                occOld = (float)_mapData[gx, gy] / 255.0f;

                                p = occOld * occNew / ((1.0f - occOld) * (1.0f - occNew));
                                occ = 255.0f * (p / (1.0f + p));
                                // Force it back into the required range of 1-254
                                if (occ < 1.0f)
                                    occ = 1.0f;
                                if (occ > 254.0f)
                                    occ = 254.0f;

                                // Save the new probability
                                _mapData[gx, gy] = (byte)occ;
                            }
                        }
                    }
                }
            }
            return;
        }


        /// <summary>
        /// MapToImage - Convert the map to an image
        /// </summary>
        /// <remarks>
        /// Creates a bitmap from the map array
        /// NOTE: The bitmap will be 24-bit RGB which is the most common format.
        /// The bitmap will be greyscale, as is usual for an occpancy grid,
        /// with Black = Occupied, White = Free.
        /// However, coloured information can be written into it by the
        /// caller, e.g. a path for the robot, after the bitmap is returned.
        /// </remarks>
        /// <returns>Bitmap containing the map (24-bit RGB)</returns>
        public Bitmap MapToImage()
        {
            byte[] data = new byte[_grid_size * 3];
            int i, j, k;
            // Bitmaps are flipped in the Y direction, so process the
            // scanlines backwards
            k = 0;
            for (j = _grid_sy-1; j >= 0; j--)
            {
                for (i = 0; i < _grid_sx; i++)
                {
                    // Insert the grid cell value for R, G and B
                    // This will make it greyscale
                    data[k++] = _mapData[i, j];
                    data[k++] = _mapData[i, j];
                    data[k++] = _mapData[i, j];
                }
            }
            return (MakeBitmap(_grid_sx, _grid_sy, data));
        }


        /// <summary>
        /// MakeBitmap - Makes a Bitmap from raw data
        /// </summary>
        /// <param name="width">Bitmap Width</param>
        /// <param name="height">Bitmap Height</param>
        /// <param name="imageData">Raw image data in RGB format
        /// (Bitmaps are BGR with image flipped in Y direction)</param>
        /// <returns>Bitmap image (24-bit RGB)</returns>
        public Bitmap MakeBitmap(int width, int height, byte[] imageData)
        {
            // NOTE: This code implicitly assumes that the width is a multiple
            // of four bytes because Bitmaps have to be longword aligned.
            // We really should look at bmp.Stride to see if there is any padding.
            // However, the width and height come from the webcam and most cameras
            // have resolutions that are multiples of four.

            Bitmap bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);

            BitmapData data = bmp.LockBits(
                new Rectangle(0, 0, bmp.Width, bmp.Height),
                ImageLockMode.WriteOnly,
                PixelFormat.Format24bppRgb
            );

            Marshal.Copy(imageData, 0, data.Scan0, imageData.Length);

            bmp.UnlockBits(data);

            return bmp;
        }


        // Draw an occupancy grid style map using LRF data
        // NOTE: This uses basic GDI calls. It was used for
        // proof-of-concept, but is not used anymore.
        /// <summary>
        /// DrawMap - Draws an occupancy grid from LRF data
        /// </summary>
        /// <param name="bmp">Bitmap to draw into</param>
        /// <param name="X">X-Position in meters</param>
        /// <param name="Y">Y-Position in meters</param>
        /// <param name="Theta">Orientation in degrees</param>
        /// <param name="AngularRange">Field of View of the LRF</param>
        /// <param name="DistanceMeasurements">Array of range scans in millimeters</param>
        public void DrawMap(Bitmap bmp, float X, float Y, float Theta,
                        int AngularRange, int[] DistanceMeasurements)
        {
            double angle;
            int range;
            double rx, ry;
            int startX, startY;
            int endX, endY;

            // Avoid nasty surprises
            //if (_mapImage == null || g == null)
            //    return;
            //Bitmap _mapImage = new Bitmap(_grid_sx, _grid_sy, PixelFormat.Format24bppRgb);
            g = Graphics.FromImage(bmp);
            //g.Clear(Color.LightGray);

            // Loop invariants
            double startAngle = AngularRange / 2.0;
            // AngularResolution is zero! This is a bug that I have
            // reported and it should be fixed in V1.5. In the meantime,
            // calculate the value.
            double angleIncrement = (double)AngularRange / (double)DistanceMeasurements.Length;
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
            startX = (int)(X / _grid_res) + bmp.Width / 2;
            startY = bmp.Height / 2 - (int)(Y / _grid_res);
            for (int x = 0; x < DistanceMeasurements.Length; x++)
            {
                range = DistanceMeasurements[x];
                // NOTE: Scans are backwards, i.e. right to left
                angle = -startAngle + x * angleIncrement;
                // Now adjust for the robot's orientation
                angle += Theta;
                // Convert to Radians
                angle = angle * Math.PI / 180;

                // The Simulated LRF returns zero if there is no hit
                // This is not the way that a real LRF works
                if (range <= 0)
                    range = (int)(_range_max * _range_factor);

                rx = range * Math.Cos(angle) / _range_factor;
                ry = range * Math.Sin(angle) / _range_factor;
                endX = startX + (int)(rx / _grid_res);
                endY = startY - (int)(ry / _grid_res);
                // NOTE: This code relies on the fact that the DrawLine
                // method will clip at the edges of the bitmap!
                g.DrawLine(Pens.White, startX, startY, endX, endY);
            }

            // Now draw the obstacles at the points of the laser hits.
            // NOTE: It might seem inefficient to do this as two loops,
            // but this way the obstacles can overwrite the free space
            // in the case of roundoff errors.
            for (int x = 0; x < DistanceMeasurements.Length; x++)
            {
                range = DistanceMeasurements[x];
                // NOTE: Scans are backwards, i.e. right to left
                angle = -startAngle + x * angleIncrement;
                // Now adjust for the robot's orientation
                angle += Theta;
                // Convert to Radians
                angle = angle * Math.PI / 180;
                // The Simulated LRF returns zero if there is no hit
                // This is not the way that a real LRF works
                if (range <= 0)
                    range = (int)(_range_max * _range_factor);

                // Only draw the obstacle if the range is not maxed out
                // because this indicates a "miss"
                if (range < (int)(_range_max * _range_factor))
                {
                    rx = range * Math.Cos(angle) / _range_factor;
                    ry = range * Math.Sin(angle) / _range_factor;
                    endX = startX + (int)(rx / _grid_res);
                    endY = startY - (int)(ry / _grid_res);
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
                    if (endX >= 0 && endX < bmp.Width &&
                        endY >= 0 && endY < bmp.Height)
                        bmp.SetPixel(endX, endY, Color.Black);
                }
            }
        }

    }
}
