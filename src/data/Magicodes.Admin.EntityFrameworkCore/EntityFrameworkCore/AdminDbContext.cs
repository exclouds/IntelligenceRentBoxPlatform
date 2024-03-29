﻿using Abp.IdentityServer4;
using Abp.Zero.EntityFrameworkCore;
using Magicodes.Admin.Attachments;
using Magicodes.Admin.Authorization.OpenId;
using Microsoft.EntityFrameworkCore;
using Magicodes.Admin.Authorization.Roles;
using Magicodes.Admin.Authorization.Users;
using Magicodes.Admin.Chat;
using Magicodes.Admin.Contents;
using Magicodes.Admin.Editions;
using Magicodes.Admin.Friendships;
using Magicodes.Admin.MultiTenancy;
using Magicodes.Admin.MultiTenancy.Payments;
using Magicodes.Admin.Storage;
using Magicodes.Admin.Core.Custom.Business;

namespace Magicodes.Admin.EntityFrameworkCore
{
    public partial class AdminDbContext : AbpZeroDbContext<Tenant, Role, User, AdminDbContext>, IAbpPersistedGrantDbContext
	{
        /* Define an IDbSet for each entity of the application */

        public virtual DbSet<BinaryObject> BinaryObjects { get; set; }
        public virtual DbSet<PersistedGrantEntity> PersistedGrants { get; set; }
        public virtual DbSet<Friendship> Friendships { get; set; }

        public virtual DbSet<ChatMessage> ChatMessages { get; set; }

        public virtual DbSet<SubscribableEdition> SubscribableEditions { get; set; }

        public virtual DbSet<SubscriptionPayment> SubscriptionPayments { get; set; }
        

        public virtual DbSet<AttachmentInfo> AttachmentInfos { get; set; }

        public virtual DbSet<ObjectAttachmentInfo> ObjectAttachmentInfos { get; set; }

        public virtual DbSet<ArticleInfo> ArticleInfos { get; set; }

        public virtual DbSet<ArticleSourceInfo> ArticleSourceInfos { get; set; }

        public virtual DbSet<ArticleTagInfo> ArticleTagInfos { get; set; }

        public virtual DbSet<ColumnInfo> ColumnInfos { get; set; }

        public virtual DbSet<AppUserOpenId> AppUserOpenIds { get; set; }


        

        public AdminDbContext(DbContextOptions<AdminDbContext> options)
			: base(options)
		{

		}

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //    modelBuilder.Entity<BinaryObject>(b =>
        //    {
        //        b.HasIndex(e => new { e.TenantId });
        //    });

        //    modelBuilder.Entity<ChatMessage>(b =>
        //    {
        //        b.HasIndex(e => new { e.TenantId, e.UserId, e.ReadState });
        //        b.HasIndex(e => new { e.TenantId, e.TargetUserId, e.ReadState });
        //        b.HasIndex(e => new { e.TargetTenantId, e.TargetUserId, e.ReadState });
        //        b.HasIndex(e => new { e.TargetTenantId, e.UserId, e.ReadState });
        //    });

        //    modelBuilder.Entity<Friendship>(b =>
        //    {
        //        b.HasIndex(e => new { e.TenantId, e.UserId });
        //        b.HasIndex(e => new { e.TenantId, e.FriendUserId });
        //        b.HasIndex(e => new { e.FriendTenantId, e.UserId });
        //        b.HasIndex(e => new { e.FriendTenantId, e.FriendUserId });
        //    });

        //    modelBuilder.Entity<Tenant>(b =>
        //    {
        //        b.HasIndex(e => new { e.SubscriptionEndDateUtc });
        //        b.HasIndex(e => new { e.CreationTime });
        //    });

        //    modelBuilder.Entity<SubscriptionPayment>(b =>
        //    {
        //        b.HasIndex(e => new { e.Status, e.CreationTime });
        //        b.HasIndex(e => new { e.PaymentId, e.Gateway });
        //    });

        //    modelBuilder.ConfigurePersistedGrantEntity();
        //}
    }
}
