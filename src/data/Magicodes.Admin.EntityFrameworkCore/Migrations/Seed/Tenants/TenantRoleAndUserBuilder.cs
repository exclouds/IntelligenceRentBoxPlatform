using System.Linq;
using Abp;
using Abp.Authorization;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.MultiTenancy;
using Abp.Notifications;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Magicodes.Admin.Authorization;
using Magicodes.Admin.Authorization.Roles;
using Magicodes.Admin.Authorization.Users;
using Magicodes.Admin.EntityFrameworkCore;
using Magicodes.Admin.Notifications;
using Magicodes.Admin.Core.Custom.Authorization;

namespace Magicodes.Admin.Migrations.Seed.Tenants
{
	public class TenantRoleAndUserBuilder
	{
		private readonly AdminDbContext _context;
		private readonly int _tenantId;

		public TenantRoleAndUserBuilder(AdminDbContext context, int tenantId)
		{
			_context = context;
			_tenantId = tenantId;
		}

		public void Create()
		{
			CreateRolesAndUsers();
		}

		private void CreateRolesAndUsers()
		{
			//管理员角色

			var adminRole = _context.Roles.IgnoreQueryFilters().FirstOrDefault(r => r.TenantId == _tenantId && r.Name == StaticRoleNames.Tenants.Admin);
			if (adminRole == null)
			{
				adminRole = _context.Roles.Add(new Role(_tenantId, StaticRoleNames.Tenants.Admin, StaticRoleNames.Tenants.Admin) { IsStatic = true }).Entity;
				_context.SaveChanges();
			}

			//基础用户角色

			var userRole = _context.Roles.IgnoreQueryFilters().FirstOrDefault(r => r.TenantId == _tenantId && r.Name == StaticRoleNames.Tenants.User);
			if (userRole == null)
			{
				userRole = _context.Roles.Add(new Role(_tenantId, StaticRoleNames.Tenants.User, StaticRoleNames.Tenants.User) { IsStatic = true, IsDefault = true }).Entity;
				_context.SaveChanges();
			}
			//平台新增用户角色
			var noneRole = _context.Roles.IgnoreQueryFilters().FirstOrDefault(r => r.TenantId == _tenantId && r.Name == StaticRoleNames.Tenants.None);
			if (noneRole == null)
			{
				noneRole = _context.Roles.Add(new Role(_tenantId, StaticRoleNames.Tenants.None, StaticRoleNames.Tenants.None) { IsStatic = true, IsDefault = true }).Entity;
				_context.SaveChanges();
			}
			// Grant all permissions to admin role

			var grantedPermissions = _context.Permissions.IgnoreQueryFilters()
				.OfType<RolePermissionSetting>()
				.Where(p => p.TenantId == _tenantId && p.RoleId == userRole.Id)
				.Select(p => p.Name)
				.ToList();

			var permissions = PermissionFinder
				.GetAllPermissions(
					new AppAuthorizationProvider(true),
					new AppCustomAuthorizationProvider(true)
				)
				.Where(p => p.MultiTenancySides.HasFlag(MultiTenancySides.Tenant) &&
							!grantedPermissions.Contains(p.Name))
				.ToList();

			if (permissions.Any())
			{
				_context.Permissions.AddRange(
					permissions.Select(permission => new RolePermissionSetting
					{
						TenantId = _tenantId,
						Name = permission.Name,
						IsGranted = true,
						RoleId = userRole.Id
					})
				);
				_context.SaveChanges();
			}

			//admin user

			var adminUser = _context.Users.IgnoreQueryFilters().FirstOrDefault(u => u.TenantId == _tenantId && u.UserName == AbpUserBase.AdminUserName);
			if (adminUser == null)
			{
				adminUser = User.CreateTenantAdminUser(_tenantId, "admin@exclouds.com.cn");
				adminUser.Password = new PasswordHasher<User>(new OptionsWrapper<PasswordHasherOptions>(new PasswordHasherOptions())).HashPassword(adminUser, "123456");
				adminUser.IsEmailConfirmed = true;
				//TODO:为了兼容新版UI先将改为false
				adminUser.ShouldChangePasswordOnNextLogin = false;
				adminUser.IsActive = true;
                adminUser.Surname = "exclouds";

                _context.Users.Add(adminUser);
				_context.SaveChanges();

				//Assign Admin role to admin user
				_context.UserRoles.Add(new UserRole(_tenantId, adminUser.Id, adminRole.Id));
				_context.SaveChanges();

				//User account of admin user
				if (_tenantId == 1)
				{
					_context.UserAccounts.Add(new UserAccount
					{
						TenantId = _tenantId,
						UserId = adminUser.Id,
						UserName = AbpUserBase.AdminUserName,
						EmailAddress = adminUser.EmailAddress
					});
					_context.SaveChanges();
				}

				//Notification subscription
				_context.NotificationSubscriptions.Add(new NotificationSubscriptionInfo(SequentialGuidGenerator.Instance.Create(), _tenantId, adminUser.Id, AppNotificationNames.NewUserRegistered));
				_context.SaveChanges();
			}
		}
	}
}
