//------------------------------------------------------------------------------
//
// CRANIUM Simulated Pioneer 3DX GPS SERVICE
//
// CRANIUM
//
// 2010 Raul Arrabales (raul@conscious-robots.com)
//
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
using Microsoft.Robotics.PhysicalModel;


using System.IO;
using System.Drawing.Imaging;
using System.Drawing;

namespace Cranium.Simulation.Sensors.SimulatedPioneerGPS
{
    /// <summary>
    /// Simulated Pioneer GPS sensor entity
    /// </summary>
    [DataContract]
    [CLSCompliant(true)]
    [EditorAttribute(typeof(GenericObjectEditor), typeof(System.Drawing.Design.UITypeEditor))]    
    public class PioneerGPSEntity : VisualEntity
    {
        private BoxShape _shape;

        [DataMember]
        public BoxShape Shape
        {
            get { return _shape; }
            set { _shape = value; }        
        }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public PioneerGPSEntity()
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
                // GPS sensor dimensions and relative position
                _shape = new BoxShape(new BoxShapeProperties(
                    "GPS Sensor", 
                    0.01f, 
                    new Pose(new Vector3(0,0.8f,0)), 
                    new Vector3(0.01f, 0.01f, 0.01f)));

                State.PhysicsPrimitives.Add(_shape);
                base.Initialize(device, physicsEngine);
                if (Parent == null)
                {
                    throw new Exception("GPS Sensor entity must be a child of another entity.");
                }
                CreateAndInsertPhysicsEntity(physicsEngine);
                Flags |= VisualEntityProperties.DisableRendering;
                AddShapeToPhysicsEntity(_shape, new VisualEntityMesh(device, 0.01f, 0.01f));       

            }
            catch (Exception ex)
            {
                HasBeenInitialized = false;
                InitError = ex.ToString();
            }
        }

    }
}
