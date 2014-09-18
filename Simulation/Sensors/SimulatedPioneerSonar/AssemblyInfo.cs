//-----------------------------------------------------------------------
//  This file is part of the Microsoft Robotics Studio Code Samples.
// 
//  Copyright (C) Microsoft Corporation.  All rights reserved.
//
//  $File: AssemblyInfo.cs $ $Revision: 2 $
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using attributes = Microsoft.Dss.Core.Attributes;
using interopservices = System.Runtime.InteropServices;
using System.Reflection;

[assembly: attributes.ServiceDeclaration(attributes.DssServiceDeclaration.ServiceBehavior)]
[assembly: interopservices.ComVisible(false)]
[assembly: CLSCompliant(true)]

[assembly: AssemblyTitleAttribute("Simulated Sonar")]
[assembly: AssemblyCopyrightAttribute("(c) 2007. Raul Arrabales")]
[assembly: AssemblyDescriptionAttribute("Simulated Sonar")]
[assembly: AssemblyCompanyAttribute("www.Conscious-Robots.com")]
[assembly: AssemblyProductAttribute("CRANIUM")]
