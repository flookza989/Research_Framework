﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Research_Framework.ApplicationDB
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class ResearchDBEntities : DbContext
    {
        public ResearchDBEntities()
            : base("name=ResearchDBEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<branch> branches { get; set; }
        public virtual DbSet<faculty> faculties { get; set; }
        public virtual DbSet<log> logs { get; set; }
        public virtual DbSet<process> processes { get; set; }
        public virtual DbSet<process_path> process_path { get; set; }
        public virtual DbSet<research> researches { get; set; }
        public virtual DbSet<research_member> research_member { get; set; }
        public virtual DbSet<user> users { get; set; }
        public virtual DbSet<View_process> View_process { get; set; }
        public virtual DbSet<View_research> View_research { get; set; }
        public virtual DbSet<View_research_member> View_research_member { get; set; }
        public virtual DbSet<View_user> View_user { get; set; }
    }
}
