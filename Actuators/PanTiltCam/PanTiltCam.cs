using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Ccr.Core;
using Microsoft.Dss.Core.Attributes;
using Microsoft.Dss.ServiceModel.Dssp;
using Microsoft.Dss.Core.DsspHttp;
using Microsoft.Dss.ServiceModel.DsspServiceBase;
using W3C.Soap;
using submgr = Microsoft.Dss.Services.SubscriptionManager;

using pxPT = Cera.Generic.PanTilt.Proxy;

// Needed to load logitech move dll
using System.Runtime.InteropServices;


namespace Cera.SensoryMotor.PanTilt
{
    [Contract(Contract.Identifier)]
    [AlternateContract(pxPT.Contract.Identifier)]
    [DisplayName("PanTiltCam")]
    [Description("PanTiltCam service. Provides access to Logitech WebCam Pan Tilt motors")]
    [DssServiceDescription("http://www.conscious-robots.com/raul/machine-consciousness.html")]
    class PanTiltCamService : DsspServiceBase
    {
        /// <summary>
        /// Service state
        /// </summary>
        [ServiceState]
        pxPT.GenericPanTiltState _state = new pxPT.GenericPanTiltState();

        /// <summary>
        /// Main service port
        /// </summary>
        [ServicePort("/PanTiltCam", AllowMultipleInstances = true)]
        pxPT.GenericPanTiltOperations _mainPort = new pxPT.GenericPanTiltOperations();

        [SubscriptionManagerPartner]
        submgr.SubscriptionManagerPort _submgrPort = new submgr.SubscriptionManagerPort();

        // Load Logitech driver operations
        [DllImport("logimove.dll")]
        static extern void ReloadDevices();
        //Reloads the list of system devices to operate on.

        [DllImport("logimove.dll")]
        static extern int GetDeviceCount();
        //Retrieves the number of system devices to operate on.

        [DllImport("logimove.dll")]
        static extern IntPtr GetDevicePath(int iDeviceIndex);
        //Retrieves the path of a device. iDeviceIndex is in range of 0 to GetDeviceCount()-1.

        [DllImport("logimove.dll")]
        static extern int OpenDevice(IntPtr DevicePath);
        //Opens the device with specified path, and returns a handle to it.
        //If failed, returns 0.
        //Note: HANDLE is a 32-bit integer.

        [DllImport("logimove.dll")]
        static extern int CloseDevice(int hDevice);
        //Closes an opened device, by its handle. Returns whether operation succeeded or not.

        [DllImport("logimove.dll")]
        static extern void MoveLeft(int hDevice);
        //Pans camera, specified by device handle, to the left.

        [DllImport("logimove.dll")]
        static extern void MoveRight(int hDevice);
        //Pans camera, specified by device handle, to the right.

        [DllImport("logimove.dll")]
        static extern void MoveTop(int hDevice);
        //Tilts camera, specified by device handle, up.

        [DllImport("logimove.dll")]
        static extern void MoveBottom(int hDevice);
        //Tilts camera, specified by device handle, down.

        [DllImport("logimove.dll")]
        static extern void MoveHome(int hDevice);
        //Moves the camera to its home position.

        [DllImport("logimove.dll")]
        static extern void Move(int hDevice, int iPosition);
        //* MOVE_HOME = 0x00000000
        //* MOVE_LEFT = 0x0000FF80
        //* MOVE_RIGHT = 0x00000080
        //* MOVE_TOP = 0xFF800000
        //* MOVE_BOTTOM = 0x00800000



        /// <summary>
        /// Service constructor
        /// </summary>
        public PanTiltCamService(DsspServiceCreationPort creationPort)
            : base(creationPort)
        {
        }

        /// <summary>
        /// Service start
        /// </summary>
        protected override void Start()
        {
            int NumCams = GetDeviceCount();
            LogInfo("Number of cameras found: " + NumCams);

            // Look for a camera 
            IntPtr path = IntPtr.Zero; 
            for (int i = 0; i < NumCams; i++)
            {
                path = GetDevicePath(i);
                LogInfo("Camera found: " + path.ToString());
            }

            // Open the right device here
            path = GetDevicePath(6);
            _state.CamID = OpenDevice(path);

            if (_state.CamID != 0)
            {
                LogInfo("Using Camera Device ID: " + _state.CamID);
            }
            else
            {
                LogError("Camera device not found. Pan Tilt Service will not work.");
            }

            base.Start();
        }

        /// <summary>
        /// Handles Subscribe messages
        /// </summary>
        /// <param name="subscribe">the subscribe request</param>
        [ServiceHandler]
        public void SubscribeHandler(pxPT.Subscribe subscribe)
        {
            SubscribeHelper(_submgrPort, subscribe.Body, subscribe.ResponsePort);
        }


        /// <summary>
        /// Pan Tilt Operation Handler
        /// </summary>
        /// <param name="op"></param>
        [ServiceHandler(ServiceHandlerBehavior.Exclusive)]
        public IEnumerator<ITask> PanTiltOperationHandler(pxPT.PanTiltOperation op)
        {
            pxPT.PanTiltOperationRequest req = op.Body;

            LogInfo("Received request: " + req.OperationType.ToString());

            switch (req.OperationType)
            {
                case pxPT.PanTiltOperationRequest.OpType.MoveDown:                   
                        MoveBottom(_state.CamID);
                        break;
                case pxPT.PanTiltOperationRequest.OpType.MoveLeft:                    
                        MoveLeft(_state.CamID);
                        break;
                case pxPT.PanTiltOperationRequest.OpType.MoveRight:                    
                        MoveRight(_state.CamID);
                        break;
                case pxPT.PanTiltOperationRequest.OpType.MoveUp:                    
                        MoveTop(_state.CamID);
                        break;
                case pxPT.PanTiltOperationRequest.OpType.Reset:
                        MoveHome(_state.CamID);
                        break;
            }

            op.ResponsePort.Post(DefaultUpdateResponseType.Instance);
            yield break;
        }

    
    }
}


