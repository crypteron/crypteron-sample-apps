﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Crypteron.SampleApps.EF6.DbFirst
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class PlainDbContext : DbContext
    {
        public PlainDbContext()
            : base("name=PlainDbContext")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<User> Users { get; set; }
    
        public virtual ObjectResult<User> usp_SearchUserByName(string usernamePrefix)
        {
            var usernamePrefixParameter = usernamePrefix != null ?
                new ObjectParameter("UsernamePrefix", usernamePrefix) :
                new ObjectParameter("UsernamePrefix", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<User>("usp_SearchUserByName", usernamePrefixParameter);
        }
    
        public virtual ObjectResult<User> usp_SearchUserByName(string usernamePrefix, MergeOption mergeOption)
        {
            var usernamePrefixParameter = usernamePrefix != null ?
                new ObjectParameter("UsernamePrefix", usernamePrefix) :
                new ObjectParameter("UsernamePrefix", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<User>("usp_SearchUserByName", mergeOption, usernamePrefixParameter);
        }
    }
}
