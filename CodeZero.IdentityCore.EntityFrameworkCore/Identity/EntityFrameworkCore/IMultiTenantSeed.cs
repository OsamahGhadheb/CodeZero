﻿//  <copyright file="IMultiTenantSeed.cs" project="CodeZero.IdentityCore.EntityFrameworkCore" solution="CodeZero">
//      Copyright (c) 2018 CodeZero Framework.  All rights reserved.
//  </copyright>
//  <author>Nasr Aldin M.</author>
//  <email>nasr2ldin@gmail.com</email>
//  <website>https://nasraldin.com/codezero</website>
//  <github>https://nasraldin.github.io/CodeZero</github>
//  <date>01/01/2018 01:00 AM</date>
using CodeZero.MultiTenancy;

namespace CodeZero.Identity.EntityFrameworkCore
{
    public interface IMultiTenantSeed
    {
        CodeZeroTenantBase Tenant { get; set; }
    }
}