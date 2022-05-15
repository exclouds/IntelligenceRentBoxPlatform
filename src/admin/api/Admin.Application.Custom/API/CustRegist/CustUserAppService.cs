using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Dapper.Repositories;
using Abp.Domain.Repositories;
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
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Linq.Dynamic.Core;
using Abp.Linq.Extensions;
using Microsoft.EntityFrameworkCore;



namespace Admin.Application.Custom.API.CustRegist
{
    public class CustUserAppService : AppServiceBase
    {
        #region 注入依赖
        private readonly IRepository<Department, int> _departmentRepository;
        private readonly IRepository<MyOrganization, long> _organizationUnitRepository;
        private readonly IRepository<User, long> _userRepository;
        private readonly IEnumerable<IPasswordValidator<User>> _passwordValidators;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IDapperRepository<MyOrganization, long> _sqlDapperRepository;
        // private TokenAuthController

        public CustUserAppService(IRepository<Department, int> departmentRepository
            , IRepository<MyOrganization, long> organizationUnitRepository
            , IRepository<User, long> userRepository,
             IEnumerable<IPasswordValidator<User>> passwordValidators,
            IPasswordHasher<User> passwordHasher,
            IDapperRepository<MyOrganization, long> sqlDapperRepository)
        {
            _departmentRepository = departmentRepository;
            _organizationUnitRepository = organizationUnitRepository;
            _userRepository = userRepository;
            _passwordValidators = passwordValidators;
            _passwordHasher = passwordHasher;
            _sqlDapperRepository = sqlDapperRepository;

        }
        #endregion

        #region 获取用户信息
        public async Task<PagedResultDto<UserRegistList>> GetAllCustUsers(UserSearchDto input)
        {

            var query = from user in _userRepository.GetAll().Where(p=>p.CreatorUserId==AbpSession.UserId)
                      // .Where(p=> p.UserNature == input.UserNature)
                       .WhereIf(
                           !input.Filter.IsNullOrWhiteSpace(),
                           u =>
                               (u.Name.Contains(input.Filter) ||
                               u.Surname.Contains(input.Filter) ||
                               u.UserName.Contains(input.Filter) ||
                               u.EmailAddress.Contains(input.Filter))
                               

                       )
                        select new UserRegistList
                        {
                            Id = user.Id,
                            Name = user.Name,
                            UserName = user.UserName,
                            EmailAddress = user.EmailAddress,
                            PhoneNumber = user.PhoneNumber,
                            TelNumber = user.TelNumber,
                            Sex = user.Sex,
                            UserNature = user.UserNature,
                            CompanyType = user.UserNature==1?"租客":
                                        user.UserNature == 2 ? "箱东" :
                                        user.UserNature == 3 ? "平台" : "",
                            CreationTime = user.CreationTime
                        };

            var userCount = await query.CountAsync();
            var users = query
            .OrderBy(input.Sorting)
            .PageBy(input)
            .ToList();


            
            return new PagedResultDto<UserRegistList>(userCount, users);



        }

        #endregion

        #region 获取单个用户信息
        public async Task<User> GetSingleUserInfo(long id)
        {
            var entity = await _userRepository.GetAsync(id);
            if (entity == null)
            {
                throw new UserFriendlyException(3000, "该用户不存在，请刷新重试！");
            }
            return entity;
        }
        #endregion
    }

}
