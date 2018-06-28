﻿//  <copyright file="AuditingStoreExtensions.cs" project="CodeZero" solution="CodeZero">
//      Copyright (c) 2018 CodeZero Framework.  All rights reserved.
//  </copyright>
//  <author>Nasr Aldin M.</author>
//  <email>nasr2ldin@gmail.com</email>
//  <website>https://nasraldin.com/codezero</website>
//  <github>https://nasraldin.github.io/CodeZero</github>
//  <date>01/01/2018 01:00 AM</date>
using CodeZero.Threading;

namespace CodeZero.Auditing
{
    /// <summary>
    /// Extension methods for <see cref="IAuditingStore"/>.
    /// </summary>
    public static class AuditingStoreExtensions
    {
        /// <summary>
        /// Should save audits to a persistent store.
        /// </summary>
        /// <param name="auditingStore">Auditing store</param>
        /// <param name="auditInfo">Audit informations</param>
        public static void Save(this IAuditingStore auditingStore, AuditInfo auditInfo)
        {
            AsyncHelper.RunSync(() => auditingStore.SaveAsync(auditInfo));
        }
    }
}