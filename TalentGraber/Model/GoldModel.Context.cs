﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Macrodeek.Model
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using Macrodeek.MacrodeekCommon;
    
    public partial class GoldModelContainer : DbContext
    {
        public GoldModelContainer()
            : base(AppSettingsProvider.GetSetting("GoldModelContainer"))
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<GithubRepo> GithubRepoes { get; set; }
        public virtual DbSet<TalentCandidate> TalentCandidates { get; set; }
        public virtual DbSet<GithubAccount> GithubAccounts { get; set; }
        public virtual DbSet<TalentGraberUser> TalentGraberUsers { get; set; }
        public virtual DbSet<UserToken> UserTokens { get; set; }
        public virtual DbSet<ContributerToRepo> ContributerToRepoes { get; set; }
    }
}
