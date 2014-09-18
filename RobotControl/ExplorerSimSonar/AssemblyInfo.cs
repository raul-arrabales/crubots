//-----------------------------------------------------------------------
//  This file is part of the Microsoft Robotics Studio Code Samples.
// 
//  Copyright (C) Microsoft Corporation.  All rights reserved.
//
//  $File: AssemblyInfo.cs $ $Revision: 2 $
//-----------------------------------------------------------------------

using System;
using System.Reflection;
using Microsoft.Dss.Core;
using Microsoft.Dss.Core.Attributes;

[assembly: ServiceDeclaration(DssServiceDeclaration.ServiceBehavior)]
[assembly: AssemblyTitle("ExplorerSim Service")]
[assembly: AssemblyProduct("Explorer Service")]
[assembly: AssemblyVersion("1.0")]
[assembly: AssemblyCopyright("Written by Trevor Taylor based on code from Microsoft (Jul 2007)")]
[assembly: AssemblyDescription("Simulated Pioneer robot wandering in a maze-like environment using LRF. Built for MSRS V1.5. This version is freely available.")]
[assembly: AssemblyCompany("Queensland University of Technology")]
