using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Dapper.Repositories;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.UI;
using Admin.Application.Custom.API.CustRegist.Dto;
using Magicodes.Admin;
using Magicodes.Admin.Authorization.Users;
using Magicodes.Admin.Core.Custom.Basis;
using Magicodes.Admin.Organizations;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Application.Custom.API.CustRegist
{
    [AbpAllowAnonymous]
    public class CustRegistAppService : AppServiceBase
    {
        #region 注入依赖
        private readonly IRepository<Department, int> _departmentRepository;      
        private readonly IRepository<MyOrganization, long> _organizationUnitRepository;
        private readonly IRepository<User, long> _userRepository;
        private readonly IEnumerable<IPasswordValidator<User>> _passwordValidators;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IDapperRepository<MyOrganization, long> _sqlDapperRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        // private TokenAuthController

        public CustRegistAppService(IRepository<Department, int> departmentRepository                    
            , IRepository<MyOrganization, long> organizationUnitRepository
            , IRepository<User, long> userRepository,
             IEnumerable<IPasswordValidator<User>> passwordValidators,
            IPasswordHasher<User> passwordHasher,
            IDapperRepository<MyOrganization, long> sqlDapperRepository,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _departmentRepository = departmentRepository;         
            _organizationUnitRepository = organizationUnitRepository;
            _userRepository = userRepository;
            _passwordValidators = passwordValidators;
            _passwordHasher = passwordHasher;
            _sqlDapperRepository = sqlDapperRepository;
            _unitOfWorkManager = unitOfWorkManager;

        }
        #endregion

        #region 个人注册
        public async Task PersonRegist(UserRegistDto dto)
        {
            if (!AbpSession.TenantId.HasValue)
            {
                _unitOfWorkManager.Current.SetTenantId(1);
            }
            if (!dto.Id.IsNullOrEmpty())
            {
                await NewUpdateUserAsync(dto);
            }
            else
            {
                 await NewCreateUserAsync(dto);
            }
            
        }

        private async Task NewUpdateUserAsync(UserRegistDto dto)
        {
            var user = await UserManager.FindByIdAsync(dto.Id);
            user.UserName = dto.UserName;
          //  user.OrganizationCode = dto.OrganizationCode;         
            //邮箱
            user.EmailAddress = dto.EmailAddress;
            //性别
            user.Sex = dto.Sex.HasValue? dto.Sex.Value:0;
            //姓名
            user.Name = dto.Name;
            user.Surname = dto.Name;
            //手机
            user.PhoneNumber = dto.PhoneNumber;

            user.TelNumber = dto.TelNumber;

            user.LastModifierUserId = AbpSession.UserId;
            user.LastModificationTime = DateTime.Now;



            //Set password
            if (!dto.Password.IsNullOrEmpty())
            {
                await UserManager.InitializeOptionsAsync(AbpSession.TenantId.HasValue ? AbpSession.TenantId : 1);
                foreach (var validator in _passwordValidators)
                {
                    CheckErrors(await validator.ValidateAsync(UserManager, user, dto.Password));
                }
                user.Password = _passwordHasher.HashPassword(user, dto.Password);
            }

            user.ShouldChangePasswordOnNextLogin = false;

           

        }

        private async Task NewCreateUserAsync(UserRegistDto dto)
        {
           
            int count = (from o in _userRepository.GetAll().Where(o => o.UserName == dto.UserName) select o).Count();
            if (count > 0)
            {
                throw new UserFriendlyException(L("登录账号已存在，请重新填写！"));
            }
            var user = new User();
            user.UserName = dto.UserName;
           
            user.OrganizationCode = dto.OrganizationCode;
            if (dto.OrganizationCode.IsNullOrEmpty() && dto.IsActive)
            {
                if (!AbpSession.UserId.HasValue)
                    throw new UserFriendlyException(L("请先登录！"));
                var users = _userRepository.Get(AbpSession.UserId.Value);
                dto.OrganizationCode = users.OrganizationCode;
            }
            //租户
            user.TenantId = AbpSession.TenantId.HasValue ? AbpSession.TenantId : 1;
            //邮箱
            user.EmailAddress = dto.EmailAddress;
            //性别
            user.Sex = dto.Sex.HasValue? dto.Sex.Value:0;
            //姓名
            user.Name = dto.Name;
            user.Surname = dto.Name;
            //手机
            user.PhoneNumber = dto.PhoneNumber;
            user.TelNumber = dto.TelNumber;

            user.CreatorUserId = AbpSession.UserId;
            user.CreationTime = DateTime.Now;
            user.ShouldChangePasswordOnNextLogin = false;
            user.IsVerify = dto.IsActive; //是否审核通过
            user.IsAdmin = false; //不是公司管理员
            user.IsLockoutEnabled =  false ;//初始不锁账号
            user.UserNature = dto.CompanyType;//客户

            //Set password
            if (!dto.Password.IsNullOrEmpty())
            {
                await UserManager.InitializeOptionsAsync(AbpSession.TenantId.HasValue ? AbpSession.TenantId : 1);
                foreach (var validator in _passwordValidators)
                {
                    CheckErrors(await validator.ValidateAsync(UserManager, user, dto.Password));
                }
                user.Password = _passwordHasher.HashPassword(user, dto.Password);
            }

            user.ShouldChangePasswordOnNextLogin = false;

            user.Roles = new Collection<UserRole>();
            user.Roles.Add(new UserRole(AbpSession.TenantId, user.Id, dto.CompanyType==0?9:8));

            await UserManager.CreateAsync(user);
            //long id = user.Id;

          //  int i = await _sqlDapperRepository.ExecuteAsync("insert into AbpUserRoles(CreationTime,CreatorUserId,TenantId,UserId,RoleId) values(getdate(), 2, 1,id,)@", new {  });
        }
        #endregion

        #region 公司注册
        public async Task CompanyRegist(UserRegistDto dto)
        {
            if (!AbpSession.TenantId.HasValue)
            {
                _unitOfWorkManager.Current.SetTenantId(1);
            }

            if (!dto.Id.IsNullOrEmpty())
            {
                await NewUpdateCompanyAsync(dto);
            }
            else
            {
                await NewCreateCompanyAsync(dto);
            }
        }

        private async Task NewUpdateCompanyAsync(UserRegistDto dto)
        {
            #region 更新公司信息
            
            var organizationDto = await _organizationUnitRepository.GetAsync(long.Parse(dto.Id));
                  
            organizationDto.ShortName = dto.ShortName;
            organizationDto.BusinessLicense = dto.BusinessLicense;
            organizationDto.CompanyType = dto.CompanyType == 1 ? 2 :
                                          dto.CompanyType == 0 ? 3 : 1; 
            organizationDto.LastModifierUserId = AbpSession.UserId;
            organizationDto.LastModificationTime = DateTime.Now;

            #endregion
           
            #region 更新账户信息
            var user = _userRepository.GetAll().Where(p=>p.OrganizationCode== organizationDto.Code).FirstOrDefault();
                 
            //邮箱
            user.EmailAddress = dto.EmailAddress;
            //性别
            user.Sex = 0;
            //姓名
            user.Name = dto.Name;
            user.Surname = dto.Name;
            //手机
            user.PhoneNumber = dto.PhoneNumber;

            user.TelNumber = dto.TelNumber;

            user.LastModifierUserId = AbpSession.UserId;
            user.LastModificationTime = DateTime.Now;

            user.UserNature = dto.CompanyType ;//客户

            #endregion
        }

        private async Task NewCreateCompanyAsync(UserRegistDto dto)
        {
            #region 先创建公司信息

            //判断组织机构代码不可重复不可为空
            if (string.IsNullOrEmpty(dto.Name))
            {
                throw new UserFriendlyException(L("请输入公司名称！"));

            }
            else
            {
                int count = (from o in _organizationUnitRepository.GetAll().Where(o => o.DisplayName == dto.Name) select o).Count();
                if (count > 0)
                {
                    throw new UserFriendlyException(L("公司名称已注册，请重新确认公司名称！"));
                }
                count = (from o in _userRepository.GetAll().Where(o => o.UserName == dto.UserName) select o).Count();
                if (count > 0)
                {
                    throw new UserFriendlyException(L("登录账号已存在，请重新填写！"));
                }
            }
            
            var organizationDto = new MyOrganization()
            {
                CreatorUserId = AbpSession.UserId,
                CreationTime = DateTime.Now,
                TenantId = AbpSession.TenantId.HasValue? AbpSession.TenantId:1
            };
            string code = "";

            string sql = "exec GetCompanyCode '" + dto.Name + "'";

            var modelinfo1 = _sqlDapperRepository.Query<GetCodeDto>(sql).AsQueryable().FirstOrDefault();

            if (modelinfo1 == null)
            {
                throw new UserFriendlyException(L("注册失败！"));
            }
            if (modelinfo1.flag == "T")
                code = modelinfo1.msg;
            else
            {
                throw new UserFriendlyException(L(modelinfo1.msg));
            }
            organizationDto.Code = code;
            organizationDto.DisplayName = dto.Name;
            organizationDto.ParentCode = dto.ParentCode;
            organizationDto.ShortName = dto.ShortName;
            organizationDto.BusinessLicense = dto.BusinessLicense;
            organizationDto.CompanyType = dto.CompanyType == 1 ? 2 :
                              dto.CompanyType == 0 ? 3 : 1;//客户 dto.CompanyType;


            await _organizationUnitRepository.InsertAsync(organizationDto);
            //   return ObjectMapper.Map<OrganizationUnitDto>(organizationDto);

            #endregion

            #region 创建账户信息
            var user = new User();
            user.UserName = dto.UserName;
            user.OrganizationCode = code;
            //租户
            user.TenantId = AbpSession.TenantId.HasValue ? AbpSession.TenantId : 1;
            //邮箱
            user.EmailAddress = dto.EmailAddress;
            //性别
            user.Sex = 0;
            //姓名
            user.Name = dto.Name;
            user.Surname = dto.Name;
            //手机
            user.PhoneNumber = dto.PhoneNumber;
            user.TelNumber = dto.TelNumber;

            user.CreatorUserId = AbpSession.UserId;
            user.CreationTime = DateTime.Now;
            user.ShouldChangePasswordOnNextLogin = false;
            user.IsLockoutEnabled = false; //是否锁住
            user.IsAdmin = true; //不是公司管理员
            user.UserNature = dto.CompanyType;//客户
            user.IsVerify = false;
            //Set password
            if (!dto.Password.IsNullOrEmpty())
            {
                await UserManager.InitializeOptionsAsync(AbpSession.TenantId.HasValue ? AbpSession.TenantId : 1);
                foreach (var validator in _passwordValidators)
                {
                    CheckErrors(await validator.ValidateAsync(UserManager, user, dto.Password));
                }
                user.Password = _passwordHasher.HashPassword(user, dto.Password);
            }


            user.Roles = new Collection<UserRole>();
            user.Roles.Add(new UserRole(AbpSession.TenantId, user.Id, dto.CompanyType == 0 ? 9 : 8));

            await UserManager.CreateAsync(user);
           
            #endregion
        }
        #endregion

        #region 获取登录客户基础信息
        public LoginUserInfoDto GetLoginUserInfo(string id)
        {
            var query =( from a in _userRepository.GetAll().Where(p=>p.Id.ToString()==id)
                            //join b in _organizationUnitRepository.GetAll() on a.OrganizationCode equals b.Code
                            //into cominfo from b in cominfo.DefaultIfEmpty()
                        select new LoginUserInfoDto
                        {
                            Name = a.Name,
                            CompanyType = a.UserNature,
                            OrganizationCode = a.OrganizationCode,
                            UserName = a.UserName,
                            IsActive = a.IsActive,
                            IsAdmin = a.IsAdmin,
                            TenantId = a.TenantId
                        }).FirstOrDefault();
            return query;
        }
        #endregion
    }
}
