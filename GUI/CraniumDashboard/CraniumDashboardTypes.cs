//-----------------------------------------------------------------------
//  This file was part of the Microsoft Robotics Studio Code Samples.
// 
//  Copyright (C) Microsoft Corporation.  All rights reserved.
//
//  $File: ControlPanelTypes.cs $ $Revision: 11 $
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.Dss.Core.Attributes;

using Microsoft.Ccr.Core;
using Microsoft.Dss.ServiceModel.Dssp;

using dssp = Microsoft.Dss.ServiceModel.Dssp;

namespace Cranium.Controls
{

    /// <summary>
    /// DSS Contract for Dashboard
    /// </summary>
    static class Contract
    {
        /// <summary>
        /// The DSS Namespace for Dashboard
        /// </summary>
        public const string Identifier = "http://www.conscious-robots.com/2007/09/craniumdashboard.html";
    }

    /// <summary>
    /// The Dashboard Operations Port
    /// </summary>
    public class DashboardOperations : PortSet<DsspDefaultLookup, DsspDefaultDrop, Get, Replace>
    {
    }

    /// <summary>
    /// DSS Get Definition for Dashboard 
    /// </summary>
    public class Get : Get<dssp.GetRequestType, PortSet<StateType, W3C.Soap.Fault>>
    {
        /// <summary>
        /// Default DSS Get Constructor
        /// </summary>
        public Get()
        {
        }

        /// <summary>
        /// DSS GetRequestType Constructor
        /// </summary>
        /// <param name="body"></param>
        public Get(dssp.GetRequestType body)
            : base(body)
        {
        }

    }

    /// <summary>
    /// DSS Replace Definition for Dashboard 
    /// </summary>
    public class Replace : Replace<StateType, PortSet<dssp.DefaultReplaceResponseType, W3C.Soap.Fault>>
    {
        /// <summary>
        /// Default DSS Get Constructor
        /// </summary>
        public Replace()
        {
        }

        /// <summary>
        /// DSS Dashboard StateType Constructor
        /// </summary>
        /// <param name="body"></param>
        public Replace(StateType body)
            : base(body)
        {
        }       
    }




    #region WinForms communication

    public class FromWinformEvents : Port<FromWinformMsg>
    {
    }

    public class FromWinformMsg
    {
        public enum MsgEnum
        {
            Loaded,
            Drag,
            Zoom
        }

        private string[] _parameters;
        public string[] Parameters
        {
            get { return _parameters; }
            set { _parameters = value; }
        }

        private MsgEnum _command;
        public MsgEnum Command
        {
            get { return _command; }
            set { _command = value; }
        }

        private object _object;
        public object Object
        {
            get { return _object; }
            set { _object = value; }
        }

        public FromWinformMsg(MsgEnum command, string[] parameters)
        {
            _command = command;
            _parameters = parameters;
        }
        public FromWinformMsg(MsgEnum command, string[] parameters, object objectParam)
        {
            _command = command;
            _parameters = parameters;
            _object = objectParam;
        }
    }
    #endregion



}
