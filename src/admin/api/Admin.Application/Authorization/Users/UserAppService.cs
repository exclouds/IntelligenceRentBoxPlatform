using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Configuration;
using Abp.Authorization;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.AutoMapper;
using Abp.Dapper.Repositories;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.Notifications;
using Abp.Organizations;
using Abp.Runtime.Session;
using Abp.Timing;
using Abp.UI;
using Abp.Zero.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Magicodes.Admin.Authorization.Permissions;
using Magicodes.Admin.Authorization.Permissions.Dto;
using Magicodes.Admin.Authorization.Roles;
using Magicodes.Admin.Authorization.Users.Dto;
using Magicodes.Admin.Authorization.Users.Exporting;
using Magicodes.Admin.Dto;
using Magicodes.Admin.Notifications;
using Magicodes.Admin.Url;
using Magicodes.Admin.Organizations.Dto;
using Abp.Runtime.Caching;
using Microsoft.AspNetCore.Mvc;
using Magicodes.Admin.Core.Custom.Basis;

namespace Magicodes.Admin.Authorization.Users
{
    [AbpAuthorize(AppPermissions.Pages_Administration_Users)]
    public class UserAppService : AdminAppServiceBase, IUserAppService
    {
        public IAppUrlService AppUrlService { get; set; }

        private readonly RoleManager _roleManager;
        private readonly IUserEmailer _userEmailer;
        private readonly IUserListExcelExporter _userListExcelExporter;
        private readonly INotificationSubscriptionManager _notificationSubscriptionManager;
        private readonly IAppNotifier _appNotifier;
        private readonly IRepository<RolePermissionSetting, long> _rolePermissionRepository;
        private readonly IRepository<UserPermissionSetting, long> _userPermissionRepository;
        private readonly IRepository<UserRole, long> _userRoleRepository;
        private readonly IUserPolicy _userPolicy;
        private readonly IEnumerable<IPasswordValidator<User>> _passwordValidators;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IRepository<OrganizationUnit, long> _organizationUnitRepository;
        private readonly IRepository<Department, int> _departmentRepository;
        private readonly IRepository<UserOrganizationUnit, long> _userOrganizationUnitRepository;

        private readonly IDapperRepository<UserRole, long> _userroleDapperRepository;
        private readonly IRepository<User, long> _userRepository;
        private readonly LogInManager _logInManager;

        private readonly ICacheManager _cacheManager;
        public UserAppService(
            RoleManager roleManager,
            IUserEmailer userEmailer,
            IUserListExcelExporter userListExcelExporter,
            INotificationSubscriptionManager notificationSubscriptionManager,
            IAppNotifier appNotifier,
            IRepository<RolePermissionSetting, long> rolePermissionRepository,
            IRepository<UserPermissionSetting, long> userPermissionRepository,
            IRepository<UserRole, long> userRoleRepository,
            IUserPolicy userPolicy,
            IEnumerable<IPasswordValidator<User>> passwordValidators,
            IPasswordHasher<User> passwordHasher,
            IRepository<OrganizationUnit, long> organizationUnitRepository,
            IRepository<Department, int> departmentRepository
                , IRepository<UserOrganizationUnit, long> userOrganizationUnitRepository
            , IDapperRepository<UserRole, long> userroleDapperRepository
            , IRepository<User, long> userRepository
            , LogInManager logInManager
            , ICacheManager cacheManager)
        {
            _roleManager = roleManager;
            _userEmailer = userEmailer;
            _userListExcelExporter = userListExcelExporter;
            _notificationSubscriptionManager = notificationSubscriptionManager;
            _appNotifier = appNotifier;
            _rolePermissionRepository = rolePermissionRepository;
            _userPermissionRepository = userPermissionRepository;
            _userRoleRepository = userRoleRepository;
            _userPolicy = userPolicy;
            _passwordValidators = passwordValidators;
            _passwordHasher = passwordHasher;
            _organizationUnitRepository = organizationUnitRepository;

            _userOrganizationUnitRepository = userOrganizationUnitRepository;
            _departmentRepository = departmentRepository;
            _userroleDapperRepository = userroleDapperRepository;

            AppUrlService = NullAppUrlService.Instance;
            _userRepository = userRepository;
            _logInManager = logInManager;
            _cacheManager = cacheManager;
        }

        #region 系统方法
        /// <summary>
        /// 获取平台用户
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<UserListExtionDto>> GetAllUsers(GetUsersInput input)
        {

            var query = from user in _userRepository.GetAll()
                       .WhereIf(input.Role != null && input.Role.Count > 0, u => u.Roles.Any(r => input.Role.Contains(r.RoleId)))
                       .WhereIf(input.Organization != null && input.Organization.Count > 0, u => input.Organization.Contains(u.DeptCode))
                        .WhereIf(!input.code.IsNullOrEmpty(), u => input.code == u.OrganizationCode)
                       .WhereIf(
                           !input.Filter.IsNullOrWhiteSpace(),
                           u =>
                               (u.Name.Contains(input.Filter) ||
                               u.Surname.Contains(input.Filter) ||
                               u.UserName.Contains(input.Filter) ||
                               u.EmailAddress.Contains(input.Filter))
                               && !u.UserNature

                       )
                        select new UserListExtionDto
                        {
                            Id = user.Id,
                            Name = user.Name,
                            UserName = user.UserName,
                            EmailAddress = user.EmailAddress,
                            PhoneNumber = user.PhoneNumber,
                            TelNumber = user.TelNumber,
                            Sex = user.Sex,
                            companyWorkNo = user.OrganizationCode,
                            Dpts = user.DeptCode,
                            LastLoginTime = user.LastLoginTime,
                            IsLockoutEnabled = user.IsLockoutEnabled,
                            IsAdmin = user.IsAdmin,
                            CreationTime = user.CreationTime
                        };

            var userCount = await query.CountAsync();
            var users = query
            .OrderBy(input.Sorting)
            .PageBy(input)
            .ToList();


            var roles = await _userRoleRepository.GetAll().ToListAsync();
            var coms = _organizationUnitRepository.GetAll().ToList();
            var deps = _departmentRepository.GetAll().ToList();
            foreach (var user in users)
            {
                string rolename = "";
                //取角色名称
                foreach (var a in roles.FindAll(p => p.UserId == user.Id))
                {
                    rolename += (await _roleManager.GetRoleByIdAsync(a.RoleId)).DisplayName + ",";
                }
                user.Roles = rolename.Trim(',');

                var comlist = coms.Where(p => ("," + user.companyWorkNo + ",").Contains("," + p.Code + ",")).Select(p => p.DisplayName).ToList();
                var dep = deps.Where(p => ("," + user.companyWorkNo + ",").Contains("," + p.Code + ",") && p.DepCode == user.Dpts).Select(p => p.DepName).FirstOrDefault();
                user.Dpts = dep;
                user.surname = string.Join(",", comlist);
            }
            return new PagedResultDto<UserListExtionDto>(userCount, users);



        }

        /// <summary>
        /// 获取客户端用户
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<UserListDto>> GetUsers(GetUsersInput input)
        {
            var query = UserManager.Users
                .WhereIf(
                    !input.Filter.IsNullOrWhiteSpace(),
                    u =>
                        (u.Name.Contains(input.Filter) ||
                        u.Surname.Contains(input.Filter) ||
                        u.UserName.Contains(input.Filter) ||
                        u.EmailAddress.Contains(input.Filter))
                        && u.UserNature
                );

            if (input.Permission != null && input.Permission.Count > 0)
            {
                query = from user in query
                        join ur in _userRoleRepository.GetAll() on user.Id equals ur.UserId into urJoined
                        from ur in urJoined.DefaultIfEmpty()
                        join up in _userPermissionRepository.GetAll() on new { UserId = user.Id } equals new { up.UserId } into upJoined
                        from up in upJoined.DefaultIfEmpty()
                        join rp in _rolePermissionRepository.GetAll() on new { ur.RoleId } equals new { rp.RoleId } into rpJoined
                        from rp in rpJoined.DefaultIfEmpty()
                        where (up != null && up.IsGranted || up == null && rp != null) && (input.Permission.Contains(up.Name) || input.Permission.Contains(rp.Name))
                        group user by user
                        into userGrouped
                        select userGrouped.Key;
            }

            var userCount = await query.CountAsync();

            var users = query
                .OrderBy(input.Sorting)
                .PageBy(input)
                .ToList();

            var userListDtos = ObjectMapper.Map<List<UserListDto>>(users);
            await FillRoleNames(userListDtos);

            return new PagedResultDto<UserListDto>(
                userCount,
                userListDtos
                );
        }

        public async Task<FileDto> GetUsersToExcel()
        {
            var users = await UserManager.Users.ToListAsync();
            var userListDtos = ObjectMapper.Map<List<UserListDto>>(users);
            await FillRoleNames(userListDtos);

            return _userListExcelExporter.ExportToFile(userListDtos);
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Users_Create, AppPermissions.Pages_Administration_Users_Edit)]
        public async Task<GetUserForEditOutput> GetUserForEdit(NullableIdDto<long> input)
        {
            //Getting all available roles
            var userRoleDtos = await _roleManager.Roles
                .OrderBy(r => r.DisplayName)
                .Select(r => new UserRoleDto
                {
                    RoleId = r.Id,
                    RoleName = r.Name,
                    RoleDisplayName = r.DisplayName
                })
                .ToArrayAsync();

            var allOrganizationUnits = await _organizationUnitRepository.GetAllListAsync();

            var output = new GetUserForEditOutput
            {
                Roles = userRoleDtos,
                AllOrganizationUnits = ObjectMapper.Map<List<OrganizationUnitDto>>(allOrganizationUnits),
                MemberedOrganizationUnits = new List<string>()
            };

            if (!input.Id.HasValue)
            {
                //Creating a new user
                output.User = new UserEditDto
                {
                    IsActive = true,
                    ShouldChangePasswordOnNextLogin = true,
                    IsTwoFactorEnabled = await SettingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames.UserManagement.TwoFactorLogin.IsEnabled),
                    IsLockoutEnabled = await SettingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames.UserManagement.UserLockOut.IsEnabled)
                };

                foreach (var defaultRole in await _roleManager.Roles.Where(r => r.IsDefault).ToListAsync())
                {
                    var defaultUserRole = userRoleDtos.FirstOrDefault(ur => ur.RoleName == defaultRole.Name);
                    if (defaultUserRole != null)
                    {
                        defaultUserRole.IsAssigned = true;
                    }
                }
            }
            else
            {
                //Editing an existing user
                var user = await UserManager.GetUserByIdAsync(input.Id.Value);

                output.User = ObjectMapper.Map<UserEditDto>(user);

                foreach (var userRoleDto in userRoleDtos)
                {
                    userRoleDto.IsAssigned = await UserManager.IsInRoleAsync(user, userRoleDto.RoleName);
                }

                var organizationUnits = await UserManager.GetOrganizationUnitsAsync(user);
                output.MemberedOrganizationUnits = organizationUnits.Select(ou => ou.Code).ToList();
            }

            return output;
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Users_ChangePermissions)]
        public async Task<GetUserPermissionsForEditOutput> GetUserPermissionsForEdit(EntityDto<long> input)
        {
            var user = await UserManager.GetUserByIdAsync(input.Id);
            var permissions = PermissionManager.GetAllPermissions();
            var grantedPermissions = await UserManager.GetGrantedPermissionsAsync(user);

            return new GetUserPermissionsForEditOutput
            {
                Permissions = ObjectMapper.Map<List<FlatPermissionDto>>(permissions).OrderBy(p => p.DisplayName).ToList(),
                GrantedPermissionNames = grantedPermissions.Select(p => p.Name).ToList()
            };
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Users_ChangePermissions)]
        public async Task ResetUserSpecificPermissions(EntityDto<long> input)
        {
            var user = await UserManager.GetUserByIdAsync(input.Id);
            await UserManager.ResetAllPermissionsAsync(user);
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Users_ChangePermissions)]
        public async Task UpdateUserPermissions(UpdateUserPermissionsInput input)
        {
            var user = await UserManager.GetUserByIdAsync(input.Id);
            var grantedPermissions = PermissionManager.GetPermissionsFromNamesByValidating(input.GrantedPermissionNames);
            await UserManager.SetGrantedPermissionsAsync(user, grantedPermissions);
        }
        /// <summary>
        /// 平台用户新增、修改
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task CreateOrUpdateUser(CreateOrUpdateUserInput input)
        {
            if (input.User.Id.HasValue)
            {
                await UpdateUserAsync(input);
            }
            else
            {
                await CreateUserAsync(input);
            }
        }
        /// <summary>
        /// 删除平台用户
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [AbpAuthorize(AppPermissions.Pages_Administration_Users_Delete)]
        public async Task DeleteUser(EntityDto<long> input)
        {
            if (input.Id == AbpSession.GetUserId())
            {
                throw new UserFriendlyException(L("不可删除自己账户。"));
            }

            var user = await UserManager.GetUserByIdAsync(input.Id);
            CheckErrors(await UserManager.DeleteAsync(user));
        }

        public async Task UnlockUser(EntityDto<long> input)
        {
            var user = await UserManager.GetUserByIdAsync(input.Id);
            user.Unlock();
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Users_Edit)]
        protected virtual async Task UpdateUserAsync(CreateOrUpdateUserInput input)
        {
            Debug.Assert(input.User.Id != null, "input.User.Id should be set.");

            var user = await UserManager.FindByIdAsync(input.User.Id.Value.ToString());

            //Update user properties
            ObjectMapper.Map(input.User, user); //Passwords is not mapped (see mapping configuration)

            if (input.SetRandomPassword)
            {
                user.Password = _passwordHasher.HashPassword(user, User.CreateRandomPassword());
            }
            else if (!input.User.Password.IsNullOrEmpty())
            {
                await UserManager.InitializeOptionsAsync(AbpSession.TenantId);
                CheckErrors(await UserManager.ChangePasswordAsync(user, input.User.Password));
            }

            CheckErrors(await UserManager.UpdateAsync(user));

            //Update roles
            CheckErrors(await UserManager.SetRoles(user, input.AssignedRoleNames));

            //update organization units
            await UserManager.SetOrganizationUnitsAsync(user, input.OrganizationUnits.ToArray());

            if (input.SendActivationEmail)
            {
                user.SetNewEmailConfirmationCode();
                await _userEmailer.SendEmailActivationLinkAsync(
                    user,
                    AppUrlService.CreateEmailActivationUrlFormat(AbpSession.TenantId),
                    input.User.Password
                );
            }
        }

        [AbpAuthorize(AppPermissions.Pages_Administration_Users_Create)]
        protected virtual async Task CreateUserAsync(CreateOrUpdateUserInput input)
        {
            if (AbpSession.TenantId.HasValue)
            {
                await _userPolicy.CheckMaxUserCountAsync(AbpSession.GetTenantId());
            }

            var user = ObjectMapper.Map<User>(input.User); //Passwords is not mapped (see mapping configuration)
            user.TenantId = AbpSession.TenantId;

            //Set password
            if (input.SetRandomPassword)
            {
                user.Password = _passwordHasher.HashPassword(user, User.CreateRandomPassword());
            }
            else if (!input.User.Password.IsNullOrEmpty())
            {
                await UserManager.InitializeOptionsAsync(AbpSession.TenantId);
                foreach (var validator in _passwordValidators)
                {
                    CheckErrors(await validator.ValidateAsync(UserManager, user, input.User.Password));
                }
                user.Password = _passwordHasher.HashPassword(user, input.User.Password);
            }

            user.ShouldChangePasswordOnNextLogin = input.User.ShouldChangePasswordOnNextLogin;

            //Assign roles
            user.Roles = new Collection<UserRole>();
            foreach (var roleName in input.AssignedRoleNames)
            {
                var role = await _roleManager.GetRoleByNameAsync(roleName);
                user.Roles.Add(new UserRole(AbpSession.TenantId, user.Id, role.Id));
            }

            CheckErrors(await UserManager.CreateAsync(user));
            await CurrentUnitOfWork.SaveChangesAsync(); //To get new user's Id.

            //Notifications
            await _notificationSubscriptionManager.SubscribeToAllAvailableNotificationsAsync(user.ToUserIdentifier());
            await _appNotifier.WelcomeToTheApplicationAsync(user);

            //Organization Units
            await UserManager.SetOrganizationUnitsAsync(user, input.OrganizationUnits.ToArray());

            //Send activation email
            if (input.SendActivationEmail)
            {
                user.SetNewEmailConfirmationCode();
                await _userEmailer.SendEmailActivationLinkAsync(
                    user,
                    AppUrlService.CreateEmailActivationUrlFormat(AbpSession.TenantId),
                    input.User.Password
                );
            }
        }
        /// <summary>
        /// 授权
        /// </summary>
        /// <param name="userListDtos"></param>
        /// <returns></returns>
        private async Task FillRoleNames(List<UserListDto> userListDtos)
        {
            /* This method is optimized to fill role names to given list. */

            var userRoles = await _userRoleRepository.GetAll()
                .Where(userRole => userListDtos.Any(user => user.Id == userRole.UserId))
                .Select(userRole => userRole).ToListAsync();

            var distinctRoleIds = userRoles.Select(userRole => userRole.RoleId).Distinct();

            foreach (var user in userListDtos)
            {
                var rolesOfUser = userRoles.Where(userRole => userRole.UserId == user.Id).ToList();
                user.Roles = ObjectMapper.Map<List<UserListRoleDto>>(rolesOfUser);
            }

            var roleNames = new Dictionary<int, string>();
            foreach (var roleId in distinctRoleIds)
            {
                roleNames[roleId] = (await _roleManager.GetRoleByIdAsync(roleId)).DisplayName;
            }

            foreach (var userListDto in userListDtos)
            {
                foreach (var userListRoleDto in userListDto.Roles)
                {
                    userListRoleDto.RoleName = roleNames[userListRoleDto.RoleId];
                }

                userListDto.Roles = userListDto.Roles.OrderBy(r => r.RoleName).ToList();
            }
        }

        /// <summary>
        /// IsActive开关服务
        /// </summary>
        /// <param name="input">开关输入参数</param>
        /// <returns></returns>
        [AbpAuthorize(AppPermissions.Pages_Tenants_Edit)]
        public async Task UpdateIsEmailConfirmedSwitchAsync(SwitchEntityInputDto input)
        {
            var user = await UserManager.GetUserByIdAsync(input.Id);
            user.IsEmailConfirmed = input.SwitchValue;
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="input">要删除的集合</param>
        /// <returns></returns>
        public async Task BatchDelete(List<string> input)
        {
            foreach (var entity in input)
            {
                if (!entity.IsNullOrEmpty())
                {
                    if (long.Parse(entity) == AbpSession.GetUserId())
                    {
                        throw new UserFriendlyException(L("不可删除自己账户。"));
                    }

                    var user = await UserManager.GetUserByIdAsync(long.Parse(entity));
                    CheckErrors(await UserManager.DeleteAsync(user));
                }

            }
        }

        /// <summary>
        /// IsActive开关服务
        /// </summary>
        /// <param name="input">开关输入参数</param>
        /// <returns></returns>
        [HttpPost]
        //[AbpAuthorize(AppPermissions.Pages_Tenants_Edit)]
        public async Task UpdateIsActiveSwitchAsync(SwitchEntityInputDto input)
        {
            var user = await UserManager.GetUserByIdAsync(input.Id);
            user.IsActive = input.SwitchValue;
            user.IsLockoutEnabled = !input.SwitchValue;
        }
        #endregion

        /// <summary>
        ///锁定用户
        /// </summary>
        /// <param name="input">input parameter</param>
        /// <returns></returns>
        //[AbpAuthorize(AppPermissions.Pages_Administration_Users_Create)]
        public async Task UpdateIsLockedAsync(SwitchEntityInputDto input)
        {
            var user = await UserManager.GetUserByIdAsync(input.Id);
            user.IsLockoutEnabled = input.SwitchValue;
        }


        #region 新前端方法
        
        /// <summary>
        /// 创建或修改用户
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task NewCreateOrUpdateUser(NewCreateOrUpdateUserInput input)
        {
            if (!input.User.Id.IsNullOrEmpty())
            {
                await NewUpdateUserAsync(input);
            }
            else
            {
                await NewCreateUserAsync(input);
            }
        }

        //[AbpAuthorize(AppPermissions.Pages_Administration_Users_Edit)]
        protected virtual async Task NewUpdateUserAsync(NewCreateOrUpdateUserInput input)
        {
            Debug.Assert(input.User.Id != null, "input.User.Id should be set.");

            var user = await UserManager.FindByIdAsync(input.User.Id);

            //Update user properties
            user.UserName = input.User.UserName;
            user.OrganizationCode= input.User.OrganizationCode;
            user.DeptCode = input.User.DeptCode;
            user.TenantId = AbpSession.TenantId;
            user.EmailAddress = input.User.EmailAddress;
            user.Name = input.User.Name;
            user.Surname = input.User.Name;
            user.PhoneNumber = input.User.PhoneNumber;
            user.TelNumber = input.User.TelNumber;
            //user.IsLockoutEnabled = !input.User.IsActive;
            user.LastModifierUserId = AbpSession.UserId;
            user.LastModificationTime = Clock.Now;
            user.ShouldChangePasswordOnNextLogin = false;
            user.Sex = input.User.Sex.IsNullOrEmpty()?0:int.Parse(input.User.Sex) ;

            if (!input.User.Password.IsNullOrEmpty())
            {
                await UserManager.InitializeOptionsAsync(AbpSession.TenantId);
                CheckErrors(await UserManager.ChangePasswordAsync(user, input.User.Password));
                //CheckErrors(await UserManager.ChangePasswordAsync(user, "000000"));
            }

            CheckErrors(await UserManager.UpdateAsync(user));

            //删除原来的角色
            long id = long.Parse(input.User.Id);
            int i = await _userroleDapperRepository.ExecuteAsync("delete from AbpUserRoles where UserId=@id", new { id });
            //Update roles
            user.Roles = new Collection<UserRole>();
            foreach (var roleid in input.Roles)
            {
                user.Roles.Add(new UserRole(AbpSession.TenantId, user.Id, int.Parse(roleid)));
            }

            //update organization units
            await UserManager.SetOrganizationUnitsAsync(user, input.Dpts.Select(p => long.Parse(p.Id)).ToArray());
            _cacheManager.GetCache(AppConsts.TenantListKey).Clear();
        }

        //[AbpAuthorize(AppPermissions.Pages_Administration_Users_Create)]
        protected virtual async Task NewCreateUserAsync(NewCreateOrUpdateUserInput input)
        {
            if (AbpSession.TenantId.HasValue)
            {
                await _userPolicy.CheckMaxUserCountAsync(AbpSession.GetTenantId());
            }

            var user = new User(); //Passwords is not mapped (see mapping configuration)
                                   //用户名
            user.UserName = input.User.UserName;
            user.OrganizationCode = input.User.OrganizationCode;
            user.DeptCode = input.User.DeptCode;
            //租户
            user.TenantId = AbpSession.TenantId;
            //邮箱
            user.EmailAddress = input.User.EmailAddress;
            //性别
            user.Sex = int.Parse(input.User.Sex);
            //姓名
            user.Name = input.User.Name;
            user.Surname = input.User.Name;
            //手机
            user.PhoneNumber = input.User.PhoneNumber;
            //座机
            user.TelNumber = input.User.TelNumber;
            user.IsLockoutEnabled = false;
            user.CreatorUserId = AbpSession.UserId;
            user.CreationTime = Clock.Now;
            user.ShouldChangePasswordOnNextLogin = false;

            //Set password
            if (!input.User.Password.IsNullOrEmpty())
            {
                await UserManager.InitializeOptionsAsync(AbpSession.TenantId);
                foreach (var validator in _passwordValidators)
                {
                    CheckErrors(await validator.ValidateAsync(UserManager, user, input.User.Password));
                }
                user.Password = _passwordHasher.HashPassword(user, input.User.Password);
            }

            user.ShouldChangePasswordOnNextLogin = input.User.ShouldChangePasswordOnNextLogin;

            //Assign roles
            user.Roles = new Collection<UserRole>();
            foreach (var roleid in input.Roles)
            {
                user.Roles.Add(new UserRole(AbpSession.TenantId, user.Id, int.Parse(roleid)));
            }

            CheckErrors(await UserManager.CreateAsync(user));
            await CurrentUnitOfWork.SaveChangesAsync(); //To get new user's Id.

            //Notifications
            await _notificationSubscriptionManager.SubscribeToAllAvailableNotificationsAsync(user.ToUserIdentifier());
            await _appNotifier.WelcomeToTheApplicationAsync(user);

            //Organization Units
            await UserManager.SetOrganizationUnitsAsync(user, input.Dpts.Select(p => long.Parse(p.Id)).ToArray());
            _cacheManager.GetCache(AppConsts.TenantListKey).Clear();

            ////默认新增用户为锁定状态
            //user.IsLockoutEnabled = true;

        }
        /// <summary>
        /// 根据Id取用户信息   
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        //[AbpAuthorize(AppPermissions.Pages_Administration_Users_Create, AppPermissions.Pages_Administration_Users_Edit)]
        public async Task<NewGetUserForEditOutput> NewGetUserForEdit(NullableIdDto<long> input)
        {
            //取该人员的角色
            var userRoleDtos = await _userRoleRepository.GetAll().Where(p => p.UserId == input.Id).ToArrayAsync();

            //var allOrganizationUnits = await _organizationUnitRepository.GetAllListAsync();

            var output = new NewGetUserForEditOutput
            {
                Roles = userRoleDtos.Select(p => p.RoleId.ToString()).ToList(),
                
            };

            if (input.Id.HasValue)
            {
                //Editing an existing user
                //var user = await UserManager.GetUserByIdAsync(input.Id.Value);
                 var user =await _userRepository.GetAsync((long )input.Id);
                output.User = ObjectMapper.Map<NewUserEditDto>(user);
                output.User.TelNumber = user.TelNumber;
               
                //var organizationUnits = await UserManager.GetOrganizationUnitsAsync(user);
                var  cominfo= _organizationUnitRepository.GetAll().Where(p=> ("," + user.OrganizationCode + ",").Contains(","+p.Code+",") )
                    .Select(p=>new UnitList { Id=p.Id.ToString(), DisplayName=p.DisplayName }).ToList();
                output.Dpts = cominfo;


            }

            return output;
        }


        /// <summary>
        /// 重置密码 默认为000000
        /// </summary>
        /// <returns></returns>
        public async Task ResetPassword(NullableIdDto<long> input)
        {
            //获取用户
            var user = await UserManager.GetUserByIdAsync((long)input.Id);
            user.Password = _passwordHasher.HashPassword(user, "000000");
        }

        #endregion
    }
}
