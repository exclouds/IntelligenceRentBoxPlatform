using Abp.Application.Services;
using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.Organizations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Magicodes.Admin.PublicService
{
    /// <summary>
    /// 获取组织信息公共方法
    /// </summary>
    class OrganizationUnitPublicService: AppServiceBase, IApplicationService
    {
        //组织机构
        private readonly IRepository<OrganizationUnit, long> _organizationUnitRepository;
        //人员组织对应
        private readonly IRepository<UserOrganizationUnit, long> _userOrganizationUnitRepository;


        public OrganizationUnitPublicService(IRepository<OrganizationUnit, long> organizationUnitRepository,
            IRepository<UserOrganizationUnit, long> userOrganizationUnitRepository)
        {
            _organizationUnitRepository = organizationUnitRepository;
            _userOrganizationUnitRepository = userOrganizationUnitRepository;
        }

        /// <summary>
        /// 根据用户Id获取同组织下所有用户Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<List<int>> GetUsers(int id)
        {
            List<int> userIds = new List<int>();
            //
            //var organization = from 
            return userIds;
        }
    }
}
