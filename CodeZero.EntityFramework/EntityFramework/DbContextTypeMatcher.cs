//  <copyright file="DbContextTypeMatcher.cs" project="CodeZero.EntityFramework" solution="CodeZero">
//      Copyright (c) 2018 CodeZero Framework.  All rights reserved.
//  </copyright>
//  <author>Nasr Aldin M.</author>
//  <email>nasr2ldin@gmail.com</email>
//  <website>https://nasraldin.com/codezero</website>
//  <github>https://nasraldin.github.io/CodeZero</github>
//  <date>01/01/2018 01:00 AM</date>
using CodeZero.Domain.Uow;

namespace CodeZero.EntityFramework
{
    public class DbContextTypeMatcher : DbContextTypeMatcher<CodeZeroDbContext>
    {
        public DbContextTypeMatcher(ICurrentUnitOfWorkProvider currentUnitOfWorkProvider) 
            : base(currentUnitOfWorkProvider)
        {
        }
    }
}