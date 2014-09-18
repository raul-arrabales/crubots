using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Ccr.Core;
using Microsoft.Dss.Core.Attributes;
using Microsoft.Dss.ServiceModel.Dssp;
using Microsoft.Dss.Core.DsspHttp;
using Microsoft.Dss.ServiceModel.DsspServiceBase;
using W3C.Soap;

namespace Cera.SensoryMotor.PanTilt
{
    /// <summary>
    /// PanTiltCam contract class
    /// </summary>
    public sealed class Contract
    {
        /// <summary>
        /// DSS contract identifer for PanTiltCam
        /// </summary>
        [DataMember]
        public const string Identifier = "http://www.conscious-robots.com/2009/10/pantiltcam.html";
    }


}


