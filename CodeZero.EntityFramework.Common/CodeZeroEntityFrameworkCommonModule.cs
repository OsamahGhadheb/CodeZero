﻿//  <copyright file="CodeZeroEntityFrameworkCommonModule.cs" project="CodeZero.EntityFramework.Common" solution="CodeZero">
//      Copyright (c) 2018 CodeZero Framework.  All rights reserved.
//  </copyright>
//  <author>Nasr Aldin M.</author>
//  <email>nasr2ldin@gmail.com</email>
//  <website>https://nasraldin.com/codezero</website>
//  <github>https://nasraldin.github.io/CodeZero</github>
//  <date>01/01/2018 01:00 AM</date>
using CodeZero.Modules;
using CodeZero.Reflection.Extensions;

namespace CodeZero.EntityFramework
{
    [DependsOn(typeof(CodeZeroKernelModule))]
    public class CodeZeroEntityFrameworkCommonModule : CodeZeroModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(CodeZeroEntityFrameworkCommonModule).GetAssembly());
        }
    }
}
