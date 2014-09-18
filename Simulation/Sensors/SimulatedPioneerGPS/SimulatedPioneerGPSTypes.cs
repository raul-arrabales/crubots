using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Ccr.Core;
using Microsoft.Dss.Core.Attributes;
using Microsoft.Dss.ServiceModel.Dssp;
using Microsoft.Dss.ServiceModel.DsspServiceBase;
using W3C.Soap;

namespace Cranium.Simulation.Sensors.SimulatedPioneerGPS
{
    public sealed class Contract
    {
        [DataMember]
        public const string Identifier = "http://www.conscious-robots.com/2010/01/simulatedpioneergps.html";
    }

    [DataContract]
    public class SimulatedPioneerGPSState
    {
        /// <summary>
        /// X component
        /// </summary>
        [DataMember]
        public double X { get; set; }

        /// <summary>
        /// Y component
        /// </summary>
        [DataMember]
        public double Y { get; set; }

        /// <summary>
        /// Z component
        /// </summary>
        [DataMember]
        public double Z { get; set; }

        /// <summary>
        /// Orientation
        /// </summary>
        [DataMember]
        public double Theta { get; set; }

        /// <summary>
        /// Timestamp of this sample
        /// </summary>
        [DataMember(Order = -1, XmlOmitDefaultValue = true)]
        [Description("Indicates the timestamp of the sensor reading.")]
        [DefaultValue(typeof(DateTime), "0001-01-01T00:00:00")]
        public DateTime TimeStamp { get; set; }
    }

    [ServicePort]
    public class SimulatedPioneerGPSOperations : PortSet<DsspDefaultLookup, DsspDefaultDrop, Get, Subscribe, Replace>
    {
    }

    public class Get : Get<GetRequestType, PortSet<SimulatedPioneerGPSState, Fault>>
    {
        public Get()
        {
        }

        public Get(GetRequestType body)
            : base(body)
        {
        }

        public Get(GetRequestType body, PortSet<SimulatedPioneerGPSState, Fault> responsePort)
            : base(body, responsePort)
        {
        }
    }

    /// <summary>
    /// GPSSensor replace operation
    /// </summary>
    public class Replace : Replace<SimulatedPioneerGPSState, PortSet<DefaultReplaceResponseType, Fault>>
    {
    }

    public class Subscribe : Subscribe<SubscribeRequestType, PortSet<SubscribeResponseType, Fault>>
    {
        public Subscribe()
        {
        }

        public Subscribe(SubscribeRequestType body)
            : base(body)
        {
        }

        public Subscribe(SubscribeRequestType body, PortSet<SubscribeResponseType, Fault> responsePort)
            : base(body, responsePort)
        {
        }
    }
}


