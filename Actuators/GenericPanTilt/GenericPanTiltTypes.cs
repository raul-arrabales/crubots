using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Ccr.Core;
using Microsoft.Dss.Core.Attributes;
using Microsoft.Dss.ServiceModel.Dssp;
using Microsoft.Dss.ServiceModel.DsspServiceBase;
using W3C.Soap;

namespace Cera.Generic.PanTilt
{
    [DisplayName("Generic Service Contract for PAN TILT Actuator")]
    [Description("This is a generic contract without an actual service implementation. It is used for referencing pan tilt actuators in the CERA code.")]
    [DssServiceDescription("http://www.conscious-robots.com/raul/machine-consciousness.html")]
    public sealed class Contract
    {
        [DataMember]
        public const string Identifier = "http://www.conscious-robots.com/2009/11/genericpantilt.html";
    }
    
    [DataContract]
    [DisplayName("Generic Pan Tilt Actuator State")]
    [Description("Specifies the state of the generic Pan Tilt Actuator")]
    public class GenericPanTiltState
    {
        private int _camId;

        [DataMember]
        [Description("Specifies the ID of the webcam device being controlled")]
        [DisplayName("Webcam ID")]
        public int CamID
        {
            get { return _camId; }
            set { _camId = value; }
        }
    }

    /// <summary>
    /// Generic Pan Tilt Operations
    /// </summary>
    [ServicePort]
    public class GenericPanTiltOperations : 
        PortSet<DsspDefaultLookup, DsspDefaultDrop, Get, Subscribe, PanTiltOperation>
    {
    }


    /// <summary>
    /// Pan Tilt Operation Request
    /// </summary>
    [DataContract]
    public class PanTiltOperationRequest
    {
        public enum OpType
        {
            MoveUp,     // Move the camera up
            MoveDown,   // Move the camera down
            MoveLeft,   // Move the camera left
            MoveRight,  // Move the camera right
            Reset       // Reset
        }

        // Type of operation
        private OpType _type;

        // Public accessor for operation type
        [DataMember]
        public OpType OperationType
        {
            get { return _type; }
            set { _type = value; }
        }

        public PanTiltOperationRequest()
        {
        }

        /// <summary>
        /// Pan Tilt Operation Request Contructor
        /// </summary>
        /// <param name="ot">Operation Type</param>
        public PanTiltOperationRequest(OpType ot)
        {
            _type = ot;
        }
    }


    /// <summary>
    /// Pan Tilt Operation
    /// </summary>
    public class PanTiltOperation : Update<PanTiltOperationRequest, PortSet<DefaultUpdateResponseType, Fault>>
    {
        public PanTiltOperation()
            : base(new PanTiltOperationRequest())
        {
        }

        public PanTiltOperation(PanTiltOperationRequest.OpType OpType)
            : base(new PanTiltOperationRequest(OpType))
        {
        }
    }

    public class Get : Get<GetRequestType, PortSet<GenericPanTiltState, Fault>>
    {
        public Get()
        {
        }

        public Get(GetRequestType body)
            : base(body)
        {
        }

        public Get(GetRequestType body, PortSet<GenericPanTiltState, Fault> responsePort)
            : base(body, responsePort)
        {
        }
    }

    /// <summary>
    /// PanTiltCam subscribe operation
    /// </summary>
    public class Subscribe : Subscribe<SubscribeRequestType, PortSet<SubscribeResponseType, Fault>>
    {
        /// <summary>
        /// Creates a new instance of Subscribe
        /// </summary>
        public Subscribe()
        {
        }

        /// <summary>
        /// Creates a new instance of Subscribe
        /// </summary>
        /// <param name="body">the request message body</param>
        public Subscribe(SubscribeRequestType body)
            : base(body)
        {
        }

        /// <summary>
        /// Creates a new instance of Subscribe
        /// </summary>
        /// <param name="body">the request message body</param>
        /// <param name="responsePort">the response port for the request</param>
        public Subscribe(SubscribeRequestType body, PortSet<SubscribeResponseType, Fault> responsePort)
            : base(body, responsePort)
        {
        }
    }
}


