﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LINQHomeWork
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class SzstApplicationContext : DbContext
    {
        public SzstApplicationContext()
            : base("name=SzstApplicationContext")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<C__MigrationHistory> C__MigrationHistory { get; set; }
        public virtual DbSet<Answer> Answers { get; set; }
        public virtual DbSet<Application> Applications { get; set; }
        public virtual DbSet<Grade> Grades { get; set; }
        public virtual DbSet<Remark> Remarks { get; set; }
        public virtual DbSet<User> Users { get; set; }
    }
}
