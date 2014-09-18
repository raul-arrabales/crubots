//------------------------------------------------------------------------------
// Part of MSRS Sample Code - Modified by Raul Arrabales - 27 Jul 07
//
// SIMULATED PIONEER SONAR SERVICE
//
// CRANIUM - SIMULATED PIONEER SONAR SENSOR
// This service provides access to a simulated P3DX frontal Sonar Array. 
// It uses the LRF raycasting to simulate Sonar transducers. 
//
// As suggested by Kyle - MSFT: 
// "you can make a reasonable simulated sonar sensor by doing something similar 
// to the simulated laser rangefinder.  Instead of casting hundreds of rays in a 
// scanline pattern like the laser rangefinder does, cast a handful of rays in a 
// cone that matches the aperture of the sonar sensor you want to simulate.  In 
// your code, look at the distance results returned by each ray and then set the 
// sonar return value to be the closest intersection.  
// 
// This code is provided AS IS. No warranty is provided for any purpose. 
// Use it at your own risk. 
// Please notify bugs, suggestions to: raul@conscious-robots.com
//
//  $File: SimulatedSonarTypes.cs $ $Revision: 6 $
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using Microsoft.Ccr.Core;
using W3C.Soap;
using Microsoft.Dss.ServiceModel.Dssp;
using sicklrf = Microsoft.Robotics.Services.Sensors.SickLRF.Proxy;
// Raul - SONAR Generic Contract
using pxsonar = Microsoft.Robotics.Services.Sonar.Proxy;

namespace Cranium.Simulation.Sensors
{
    
    public static class Contract
    {
        public const string Identifier = "http://www.conscious-robots.com/2009/09/simulatedpioneersonar.html";
    }



}
