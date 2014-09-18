//------------------------------------------------------------------------------
//
// CRANIUM Simulated Pioneer 3DX Bumper SERVICE
//
// CRANIUM
//
// Copyright (c) 2007-2009 Raul Arrabales (raul@conscious-robots.com)
//
//  $File: PioneerBumperEntity.cs $ $Revision: 2 $
//
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using xna = Microsoft.Xna.Framework;
using xnagrfx = Microsoft.Xna.Framework.Graphics;
using xnaprof = Microsoft.Robotics.Simulation.MeshLoader;
using Microsoft.Robotics.Simulation;
using Microsoft.Robotics.Simulation.Engine;
using Microsoft.Robotics.Simulation.Physics;
using Microsoft.Dss.Core.Attributes;
using Microsoft.Dss.Core;
using Microsoft.Ccr.Core;
using Microsoft.Robotics.Simulation.Physics;
using Microsoft.Robotics.PhysicalModel;

using Microsoft.Robotics.Simulation.Engine;

using System.IO;
using System.Drawing.Imaging;
using System.Drawing;

namespace ConsciousRobots.Cranium.Simulation.Sensors.Bumper
{

    /// <summary>
    /// Models an array of Pioneer 3DX Contact Sensors
    /// </summary>
    [DataContract]
    [CLSCompliant(true)]
    [EditorAttribute(typeof(GenericObjectEditor), typeof(System.Drawing.Design.UITypeEditor))]
    public class PioneerBumperEntity : VisualEntity
    {
        Port<EntityContactNotification> _notifications = new Port<EntityContactNotification>();

        // Total number of bumper segments
        private int _segments = 10; // We have 5 front contact sensors and 5 rear contact sensors

        // contact sensor shapes
        private BoxShape[] _shapes;


        /// <summary>
        /// Bumper panel shapes
        /// </summary>
        [DataMember]
        public BoxShape[] Shapes
        {
            get { return _shapes; }
            set { _shapes = value; }
        }


        /// <summary>
        /// Default constructor
        /// </summary>
        public PioneerBumperEntity() 
        {             
        }


        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="device"></param>
        /// <param name="physicsEngine"></param>
        public override void Initialize(xnagrfx.GraphicsDevice device, PhysicsEngine physicsEngine)
        {
            try
            {
                InitError = string.Empty;

                // Create simulated P3DX front and rear bumper arrays
                _shapes = new BoxShape[_segments]; 

                // Bumper panel dimensions
                Vector3 segmentDimensions = new Vector3( 0.075f, 0.025f, 0.01f); // 10 cm width, 2.5 cm wide, 1.0 cm deep

                // Bumper names
                string[] bumperName = new string[] { "b9/rear", "b10/rear", "b11/rear", "b12/rear", "b13/rear",
                                                     "b1/front", "b2/front", "b3/front", "b4/front", "b5/front" 
                                                      };
                // Bumper panel angles
                float[] bumperAngle = new float[] { (float)-(38.0f * Math.PI) / 180,  // b1 is at -52 degrees.
                                                    (float)-(19.0f * Math.PI) / 180,  // b2 is at -19 degrees.
                                                    (float)(0.0f * Math.PI) / 180,   // b3 is centered front.
                                                    (float)(19.0f * Math.PI) / 180,  // b4 is at 19 degrees.
                                                    (float)(38.0f * Math.PI) / 180,  // b5 is at 52 degrees.
                                                    (float)(142.0f * Math.PI) / 180, // b9 is at 128 degrees.
                                                    (float)(161.0f * Math.PI) / 180, // b10 is at 161 degrees.
                                                    (float)(180.0f * Math.PI) / 180, // b11 is centered rear.
                                                    (float)-(161.0f * Math.PI) / 180, // b12 is at -162 degrees.
                                                    (float)-(142.0f * Math.PI) / 180 }; // b13 is at -128 degrees.
                                                    
                
                // P3DX Bumper segment poses
                Vector3[] bumperPose = new Vector3[_segments];   


                // Add frontal and rear bumper shapes
                for (int segment = 0; segment < _segments; segment++)
                {
                    // P3DX Bumper segment pose
                    bumperPose[segment] = new Vector3((float)(-0.25f * Math.Sin(bumperAngle[segment])),  // X
                                                      0.05f,                                             // Y
                                                      (float)(0.25f * Math.Cos(bumperAngle[segment])));  // Z

                    // Create current segment:
                    BoxShape bumper = new BoxShape(
                        new BoxShapeProperties(
                            bumperName[segment], // segment name
                            0.001f, // segment mass
                            new Pose(bumperPose[segment],          // segment position
                                     Quaternion.FromAxisAngle(0, 1, 0, -bumperAngle[segment])), // segment orientation
                            segmentDimensions)); // segment dimensions                    

                    bumper.BoxState.Material = new MaterialProperties("P3DX chassis", 0.0f, 0.25f, 0.5f);
                    bumper.Parent = this;
                    bumper.BoxState.EnableContactNotifications = true;

                    bumper.State.DiffuseColor = new Vector4(0.1f, 0.1f, 0.1f, 0.1f);

                    _shapes[segment] = bumper;

                }

                foreach (BoxShape b in _shapes)
                {
                    State.PhysicsPrimitives.Add(b);                    
                }

                base.Initialize(device, physicsEngine);

                if (Parent == null)
                    throw new Exception("This entity must be a child of another entity.");

                CreateAndInsertPhysicsEntity(physicsEngine);

                Flags |= VisualEntityProperties.DisableRendering;

                foreach (BoxShape shape in _shapes)
                    AddShapeToPhysicsEntity(shape, null);
            }
            catch (Exception ex)
            {
                HasBeenInitialized = false;
                InitError = ex.ToString();
            }
        }

        /// <summary>
        /// Adds a notification port to the list of subscriptions that get notified when the bumper shapes
        /// collide in the physics world
        /// </summary>
        /// <param name="notificationTarget"></param>
        public void Subscribe(Port<EntityContactNotification> notificationTarget)
        {
            // the parent has the physics entity create (we are just part of that entity)
            // so subscribe there.
            PhysicsEntity.SubscribeForContacts(notificationTarget);
        }
    }

}
