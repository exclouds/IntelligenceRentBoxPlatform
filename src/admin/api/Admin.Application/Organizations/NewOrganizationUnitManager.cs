using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Organizations;
using Abp.Zero;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magicodes.Admin.Organizations
{
    public class NewOrganizationUnitManager : OrganizationUnitManager
    {

        public NewOrganizationUnitManager(IRepository<OrganizationUnit, long> organizationUnitRepository) :
            base(organizationUnitRepository)
        {

        }
        [UnitOfWork]
        public override async Task CreateAsync(OrganizationUnit organizationUnit)
        {
            if (!string.IsNullOrEmpty(organizationUnit.Code))
            {
                //取当前插入的部门Id
                try
                {
                    await OrganizationUnitRepository.InsertAndGetIdAsync(organizationUnit);
                }
                catch (Exception ex)
                {

                    throw;
                }
                

            }
            else
            {
                organizationUnit.Code = await GetNextChildCodeAsync(organizationUnit.ParentId);
                await ValidateOrganizationUnitAsync(organizationUnit);
                //取当前插入的部门Id
                long id = await OrganizationUnitRepository.InsertAndGetIdAsync(organizationUnit);
                //修改code
                var unit = await OrganizationUnitRepository.GetAsync(id);
                unit.Code = (unit.Code + "." + id).Trim('.');
            }
        }
        public override async Task<string> GetNextChildCodeAsync(long? parentId)
        {
            string re = "";
            //不是根节点
            if (parentId.HasValue)
            {
                //上级部门code+','+上级部门Id
                var unit = await OrganizationUnitRepository.GetAllListAsync(p => p.Id == parentId.Value);
                if (unit != null && unit.Count() > 0)
                {
                    re = unit.FirstOrDefault().Code;
                }
            }
            return re.Trim('.');
        }
    }
}
