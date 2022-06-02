using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Dapper.Repositories;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Abp.Organizations;
using Abp.UI;
using Magicodes.Admin.Authorization;
using Magicodes.Admin.Authorization.Users;
using Magicodes.Admin.Core.Custom.Basis;
using Magicodes.Admin.EntityFrameworkCore;
using Magicodes.Admin.Organizations.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace Magicodes.Admin.Organizations
{
    [AbpAuthorize(AppPermissions.Pages_Administration_OrganizationUnits)]
    public class OrganizationUnitAppService : AdminAppServiceBase, IOrganizationUnitAppService
    {

        private readonly OrganizationUnitManager _organizationUnitManager;
        private readonly NewOrganizationUnitManager _neworganizationUnitManager;
        private readonly IRepository<MyOrganization, long> _organizationUnitRepository;
        private readonly IRepository<UserOrganizationUnit, long> _userOrganizationUnitRepository;
        private readonly IDapperRepository<MyOrganization, long> _organizationUnitDapperRepository;
        //用户信息
        private readonly IRepository<User, long> _userRepository;
        /// <summary>
        /// 部门信息
        /// </summary>
        private readonly IRepository<Department, int> _departmentRepository;
        public OrganizationUnitAppService(
            OrganizationUnitManager organizationUnitManager,
            NewOrganizationUnitManager neworganizationUnitManager,
            IRepository<MyOrganization, long> organizationUnitRepository,
            IRepository<UserOrganizationUnit, long> userOrganizationUnitRepository,
            IDapperRepository<MyOrganization, long> organizationUnitDapperRepository,
            IRepository<User, long> userRepository,
            IRepository<Department, int> departmentRepository)
        {
            _organizationUnitManager = organizationUnitManager;
            _neworganizationUnitManager = neworganizationUnitManager;
            _organizationUnitRepository = organizationUnitRepository;
            _userOrganizationUnitRepository = userOrganizationUnitRepository;
            _organizationUnitDapperRepository = organizationUnitDapperRepository;
            _userRepository = userRepository;
            _departmentRepository = departmentRepository;
        }
        /// <summary>
        /// 查询组织结构和组织结构下部门数量
        /// </summary>
        /// <returns></returns>
        public async Task<ListResultDto<OrganizationUnitDto>> GetOrganizationUnits()
        {
            var query =
                from ou in _organizationUnitRepository.GetAll()
                join dou in _departmentRepository.GetAll() on ou.Code equals dou.Code into g
                select new { ou, memberCount = g.Count() };

            var items = await query.ToListAsync();

            return new ListResultDto<OrganizationUnitDto>(
                items.Select(item =>
                {
                    var dto = ObjectMapper.Map<OrganizationUnitDto>(item.ou);
                    return dto;
                }).ToList());
        }
        #region 公司信息
        /// <summary>
        /// 查询公司下所有子公司信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<OrganizationUnitListDto>> GetOrganizationUnitOrg(GetOrganizationUnitUsersInput input)
        {
            var query = from ou in _organizationUnitRepository.GetAll()
                        select new OrganizationUnitListDto
                        {
                            Id = ou.Id,
                            DisplayName = ou.DisplayName,
                            Code = ou.Code,
                            ShortName = ou.ShortName
                        };

            var totalCount = await query.CountAsync();
            var items = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();

            return new PagedResultDto<OrganizationUnitListDto>(
                totalCount,
                items);
            //var query = from ou in _organizationUnitRepository.GetAll()
            //            where ou.ParentCode == input.code
            //            select new { ou };

            //var totalCount = await query.CountAsync();
            //var items = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();

            //return new PagedResultDto<OrganizationUnitListDto>(
            //    totalCount,
            //    items.Select(item =>
            //    {
            //        var dto = ObjectMapper.Map<OrganizationUnitListDto>(item.ou);
            //        dto.AddedTime = item.ou.CreationTime;
            //        return dto;
            //    }).ToList());
        }
        /// <summary>
        /// 获取单个公司信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public CompanyDto GetOrganizationUnitForEdit(NullableIdDto<long> input)
        {

            if (input.Id.HasValue)
            {
                var query = (from com in _organizationUnitRepository.GetAll().Where(p => p.Id == input.Id)
                             join p in _organizationUnitRepository.GetAll() on com.ParentCode equals p.Code
                             into pacom
                             from parentcom in pacom.DefaultIfEmpty()
                             select new CompanyDto
                             {
                                 Id = com.Id,
                                 CreationTime = com.CreationTime,
                                 parentName = parentcom.DisplayName,
                                 ParentCode = com.ParentCode,
                                 DisplayName = com.DisplayName,
                                 ShortName = com.ShortName,
                                 Code = com.Code,
                                 ParentId = parentcom.Id
                             }).FirstOrDefault();


                return query;
            }
            else
            {
                throw new UserFriendlyException("请选择要修改的公司！");

            }


        }

        /// <summary>
        /// 创建公司组织
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        //[AbpAuthorize(AppPermissions.Pages_Administration_OrganizationUnits_Create)]
        public async Task<OrganizationUnitDto> CreateOrganizationUnit(CreateOrganizationUnitInput input)
        {
            //判断组织机构代码不可重复不可为空
            if (string.IsNullOrEmpty(input.Code))
            {
                throw new UserFriendlyException(L("请输入公司代码！"));

            }
            else
            {
                int count = (from o in _organizationUnitRepository.GetAll().Where(o => o.Code == input.Code) select o).Count();
                if (count > 0)
                {
                    throw new UserFriendlyException(L("公司组织机构代码重复！"));
                }
            }
            if (_organizationUnitRepository.GetAllIncluding().Any(p => p.DisplayName == input.DisplayName))
            {
                throw new UserFriendlyException("公司名称已存在,请重新输入!");
            }
            var organizationDto = new MyOrganization()
            {
                CreatorUserId = AbpSession.UserId,
                CreationTime = DateTime.Now,
                TenantId = AbpSession.TenantId
            };
            organizationDto.Code = input.Code;
            organizationDto.DisplayName = input.DisplayName;
            organizationDto.ParentCode = input.ParentCode;
            organizationDto.ShortName = input.ShortName;
            organizationDto.CompanyType = input.CompanyType;
            await _organizationUnitRepository.InsertAsync(organizationDto);
            return ObjectMapper.Map<OrganizationUnitDto>(organizationDto);
        }
        /// <summary>
        /// 客户组织机构修改
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        //[AbpAuthorize(AppPermissions.Pages_Administration_OrganizationUnits_Edit)]
        public async Task<OrganizationUnitDto> UpdateOrganizationUnit(UpdateOrganizationUnitInput input)
        {
            //判断公司不能重复
            var org = await _organizationUnitRepository.GetAsync(input.Id);
            //判断组织机构代码不可重复不可为空
            if (string.IsNullOrEmpty(input.Code))
            {
                throw new UserFriendlyException(L("请输入公司代码！"));

            }
            else 
            {
                int count = _organizationUnitRepository.GetAll().Where(o => o.Code == input.Code && o.Id!= input.Id).Count();
                if (count > 0)
                {
                    throw new UserFriendlyException(L("公司组织机构代码重复！"));
                }
            }
            if (input.DisplayName != org.DisplayName)
            {
                if (_organizationUnitRepository.GetAll().Any(p => p.DisplayName == input.DisplayName))
                {
                    throw new UserFriendlyException(L("公司已存在，请重新输入！"));
                }
            }
            var organizationUnit = await _organizationUnitRepository.GetAsync(input.Id);
            
            organizationUnit.Code = input.Code;
            organizationUnit.DisplayName = input.DisplayName;
            organizationUnit.ParentCode = input.ParentCode;
            organizationUnit.ShortName = input.ShortName;
            await _organizationUnitManager.UpdateAsync(organizationUnit);

            return await CreateOrganizationUnitDto(organizationUnit);
        }
        /// <summary>
        /// 删除公司
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [AbpAuthorize(AppPermissions.Pages_Administration_OrganizationUnits_Delete)]
        public async Task DeleteOrganizationUnit(EntityDto<long> input)
        {
            //获取所有下属部门
            var model = _organizationUnitRepository.Get(input.Id);
            var alldep = _departmentRepository.GetAll().Where(p => p.Code == model.Code).ToList();

            foreach (var dep in alldep)
            {
                dep.IsDeleted = true;
                dep.DeleterUserId = AbpSession.UserId;
                dep.DeletionTime = DateTime.Now;
            }

            //获取所有下属子公司
            var allcom = _organizationUnitRepository.GetAll().Where(p => p.ParentCode == model.Code).ToList();
            foreach (var com in allcom)
            {
                com.IsDeleted = true;
                com.DeleterUserId = AbpSession.UserId;
                com.DeletionTime = DateTime.Now;
            }
            await _organizationUnitManager.DeleteAsync(input.Id);
        }
        #endregion



        #region 部门信息
        /// <summary>
        /// 查询公司下所有部门信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PagedResultDto<OrganizationUnitDepListDto>> GetOrganizationUnitUsers(GetOrganizationUnitUsersInput input)
        {
            var query = from dou in _departmentRepository.GetAll()
                            //join ou in _organizationUnitRepository.GetAll() on dou.Code equals ou.Code
                        where dou.Code == input.code
                        select new OrganizationUnitDepListDto
                        {
                            Id = dou.Id,
                            AddedTime = dou.CreationTime,
                            DepCode = dou.DepCode,
                            DepName = dou.DepName

                        };

            var totalCount = await query.CountAsync();
            var items = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();

            return new PagedResultDto<OrganizationUnitDepListDto>(
                totalCount, items);
            //var query = from dou in _departmentRepository.GetAll()
            //            join ou in _organizationUnitRepository.GetAll() on dou.Code equals ou.Code
            //            where dou.Code == input.code
            //            select new { dou };

            //var totalCount = await query.CountAsync();
            //var items = await query.OrderBy(input.Sorting).PageBy(input).ToListAsync();

            //return new PagedResultDto<OrganizationUnitDepListDto>(
            //    totalCount,
            //    items.Select(item =>
            //    {
            //        var dto = ObjectMapper.Map<OrganizationUnitDepListDto>(item.dou);
            //        dto.AddedTime = item.dou.CreationTime;
            //        return dto;
            //    }).ToList());
        }
        /// <summary>
        /// 获取单个部门信息
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public ComDepartmentDto GetDepartmentForEdit(NullableIdDto<long> input)
        {

            if (input.Id.HasValue)
            {
                var query = from dep in _departmentRepository.GetAll().Where(p => p.Id == input.Id)
                            join com in _organizationUnitRepository.GetAll() on dep.Code equals com.Code
                            select new ComDepartmentDto
                            {
                                Id = dep.Id,
                                DepCode = dep.DepCode,
                                DepName = dep.DepName,
                                CreationTime = dep.CreationTime,
                                DepNCCName = dep.DepName,
                                Code = dep.Code,
                                parentId = com.Id.ToString(),
                                parentName = com.DisplayName,
                            };
                return query.FirstOrDefault();
            }
            else
            {
                throw new UserFriendlyException("请选择要修改的部门！");

            }


        }
        /// <summary>
        /// 创建部门
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        //[AbpAuthorize(AppPermissions.Pages_Administration_OrganizationUnits_Create)]
        public async Task<CreateDept> CreateDept(CreateDept input)
        {
            //判断组织机构代码不可重复不可为空
            if (string.IsNullOrEmpty(input.DepCode))
            {
                throw new UserFriendlyException(L("请输入部门代码！"));

            }
            else
            {
                int count = (from o in _departmentRepository.GetAll().Where(o => o.Code == input.Code && o.DepCode==input.DepCode) select o).Count();
                if (count > 0)
                {
                    throw new UserFriendlyException(L("部门代码重复！"));
                }
            }
            if (_departmentRepository.GetAllIncluding().Any(p => p.DepName == input.DepName && p.Code == input.Code))
            {
                throw new UserFriendlyException("部门名称已存在,请重新输入!");
            }
            var deptDto = new Department()
            {
                CreatorUserId = AbpSession.UserId,
                CreationTime = DateTime.Now,
                TenantId = AbpSession.TenantId
            };
            deptDto.Code = input.Code;
            deptDto.DepCode = input.DepCode;
            deptDto.DepName = input.DepName;
            await _departmentRepository.InsertAsync(deptDto);
            return ObjectMapper.Map<CreateDept>(deptDto);
        }
        /// <summary>
        /// 部门修改
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        //[AbpAuthorize(AppPermissions.Pages_Administration_OrganizationUnits_Edit)]
        public async Task UpdateDept(UpdateDept input)
        {
            //判断公司不能重复
            var org = await _departmentRepository.GetAsync(input.Id);
            int count = (from o in _departmentRepository.GetAll().Where(o => o.Code == input.Code && o.DepCode == input.DepCode && o.Id != input.Id) select o).Count();
            if (count > 0)
            {
                throw new UserFriendlyException(L("部门代码重复！"));
            }
            if (_departmentRepository.GetAll().Any(p => p.DepName == input.DepName && p.Code == input.Code && p.Id != input.Id))
            {
                throw new UserFriendlyException(L("部门已存在，请重新输入！"));
            }
            var deptDto = await _departmentRepository.GetAsync(input.Id);
            deptDto.Code = input.Code;
            deptDto.DepCode = input.DepCode;
            deptDto.DepName = input.DepName;
        }

        public async Task DeleteDept(EntityDto<int> input)
        {
            await _departmentRepository.DeleteAsync(input.Id);
        }

        #endregion


        //[AbpAuthorize(AppPermissions.Pages_Administration_OrganizationUnits_ManageOrganizationTree)]
        //public async Task<OrganizationUnitDto> MoveOrganizationUnit(MoveOrganizationUnitInput input)
        //{
        //    await _organizationUnitManager.MoveAsync(input.Id, input.NewParentId);

        //    return await CreateOrganizationUnitDto(
        //        await _organizationUnitRepository.GetAsync(input.Id)
        //        );
        //}

        


        //[AbpAuthorize(AppPermissions.Pages_Administration_OrganizationUnits_ManageMembers)]
        //public async Task RemoveUserFromOrganizationUnit(UserToOrganizationUnitInput input)
        //{
        //    await UserManager.RemoveFromOrganizationUnitAsync(input.UserId, input.OrganizationUnitId);
        //}

        //[AbpAuthorize(AppPermissions.Pages_Administration_OrganizationUnits_ManageMembers)]
        //public async Task AddUsersToOrganizationUnit(UsersToOrganizationUnitInput input)
        //{
        //    foreach (var userId in input.UserIds)
        //    {
        //        await UserManager.AddToOrganizationUnitAsync(userId, input.OrganizationUnitId);
        //    }
        //}

        //[AbpAuthorize(AppPermissions.Pages_Administration_OrganizationUnits_ManageMembers)]
        //public async Task<PagedResultDto<NameValueDto>> FindUsers(FindOrganizationUnitUsersInput input)
        //{
        //    var userIdsInOrganizationUnit = _userOrganizationUnitRepository.GetAll()
        //        .Where(uou => uou.OrganizationUnitId == input.OrganizationUnitId)
        //        .Select(uou => uou.UserId);

        //    var query = UserManager.Users
        //        .Where(u => !userIdsInOrganizationUnit.Contains(u.Id))
        //        .WhereIf(
        //            !input.Filter.IsNullOrWhiteSpace(),
        //            u =>
        //                u.Name.Contains(input.Filter) ||
        //                u.Surname.Contains(input.Filter) ||
        //                u.UserName.Contains(input.Filter) ||
        //                u.EmailAddress.Contains(input.Filter)
        //        );

        //    var userCount = await query.CountAsync();
        //    var users = await query
        //        .OrderBy(u => u.Name)
        //        .ThenBy(u => u.Surname)
        //        .PageBy(input)
        //        .ToListAsync();

        //    return new PagedResultDto<NameValueDto>(
        //        userCount,
        //        users.Select(u =>
        //            new NameValueDto(
        //                u.FullName + " (" + u.EmailAddress + ")",
        //                u.Id.ToString()
        //            )
        //        ).ToList()
        //    );
        //}


        /// <summary>
        ///     批量从组织中移除用户
        /// </summary>
        /// <param name="userIds">用户Id列表</param>
        /// <param name="organizationUnitId">组织机构Id</param>
        /// <returns></returns>
        //[AbpAuthorize(AppPermissions.Pages_Administration_OrganizationUnits_MemberBatchDelete)]
        //public async Task BatchRemoveUserFromOrganizationUnit(List<long> userIds, long organizationUnitId)
        //{
        //    await _userOrganizationUnitRepository.DeleteAsync(ou =>
        //        userIds.Contains(ou.UserId) && ou.OrganizationUnitId == organizationUnitId);
        //}

        private async Task<OrganizationUnitDto> CreateOrganizationUnitDto(OrganizationUnit organizationUnit)
        {
            var dto = ObjectMapper.Map<OrganizationUnitDto>(organizationUnit);
            return dto;
        }

        #region 新前端方法
        //private int uuid = 0;
        /// <summary>
        /// 获取组织机构，树形显示
        /// </summary>
        /// <returns></returns>
        public async Task<List<NewOrganizationUnitDto>> GetNewOrganizationUnits()
        {
            //非系统用户
            var user = await _userRepository.FirstOrDefaultAsync((long)AbpSession.UserId);
            string sql = null;
            sql = $" select * from AbpOrganizationUnits where  TenantId={AbpSession.TenantId} and IsDeleted=0 and CompanyType=1";
            var units = _organizationUnitDapperRepository.Query<MyOrganization>(sql).AsQueryable();
            //var units = await _organizationUnitRepository.GetAll().Where(o => o.CompanyType == 1).ToListAsync();
            sql = $" select * from Department where  TenantId={AbpSession.TenantId} and IsDeleted=0";

            var query = _organizationUnitDapperRepository.Query<Department>(sql).AsQueryable();
            var depts = query.ToList();
            List<NewOrganizationUnitDto> unitlist = new List<NewOrganizationUnitDto>();
            //找到根节点
            foreach (var item in units)
            {
                if (item.ParentCode.IsNullOrEmpty())
                {
                    NewOrganizationUnitDto unit = new NewOrganizationUnitDto();
                    unit.nodeUUid = item.Code;
                    unit.nodeId = item.Id;
                    unit.nodeName = item.DisplayName;
                    unit.disabled = false;
                    unit.children = GetChildrenUnit(item.Code, units.ToList(), depts);
                    unitlist.Add(unit);
                }
            }


            return unitlist;
        }
        /// <summary>
        /// 获取组织机构，树形显示
        /// </summary>
        /// <param name="CompanyType">公司类型(1:平台；2：箱东、租客)</param>
        /// <returns></returns>
        public async Task<List<NewOrganizationUnitDto>> GetNewOrganizationUnitsByType()
        {
            string sql = $" select * from AbpOrganizationUnits where  TenantId={AbpSession.TenantId} and IsDeleted=0 and CompanyType<>1";
            var units = _organizationUnitDapperRepository.Query<MyOrganization>(sql).AsQueryable();

            List<NewOrganizationUnitDto> unitlist = new List<NewOrganizationUnitDto>();
            //找到根节点
            foreach (var item in units)
            {
                NewOrganizationUnitDto unit = new NewOrganizationUnitDto();
                unit.nodeUUid = item.Code;
                unit.nodeId = item.Id;
                unit.nodeName = item.DisplayName;
                unit.disabled = false;
                unit.children = null;
                unitlist.Add(unit);
            }
            return unitlist;
        }
        /// <summary>
        /// 获取子部门
        /// </summary>
        /// <param name="parentCode">上级部门code</param>
        /// <param name="unitlist">公司列表</param>
        /// <param name="deplist">部门</param>
        /// <returns></returns>
        private List<NewOrganizationUnitDto> GetChildrenUnit(string parentCode, List<MyOrganization> unitlist, List<Department> deplist)
        {
            List<NewOrganizationUnitDto> unitslist = new List<NewOrganizationUnitDto>();         
            
            //获取子公司信息
            foreach (var item in unitlist.FindAll(p => p.ParentCode == parentCode))
            {
                NewOrganizationUnitDto unit = new NewOrganizationUnitDto();
                unit.nodeUUid = item.Code;
                unit.nodeId = item.Id;
                unit.nodeName = item.DisplayName;
                unit.disabled = false;
                int c = 0;
                ////如果该公司下面没有子公司，则直接获取该公司下的部门数量
                //c = deplist.FindAll(p => p.Code == item.Code).Count();
                //取该公司下的所有子公司           
                c = unitlist.Where(p => p.ParentCode == item.Code).Count();
               
                //var idlist = unitlist.Where(p => p.Code.Contains(item.Code)).Where(p => p.Id != item.Id).Select(p => p.Code).Distinct().ToList();
                //取子公司下的所有部门
                //var deptchildlist = from ou in deplist
                //                    join uou in idlist on ou.Code equals uou
                //                    select new { ou.DepCode };
                //c += deptchildlist.Distinct().Count();


                unit.Message =c>0? ("(" + c + ")"):""; //根节点取所有部门总和
                unit.children = GetChildrenUnit(item.Code, unitlist, deplist);
                unitslist.Add(unit);
            }
            return unitslist;
        }





        /// <summary>
        /// 根据组织机构id取人员，已经在该组织下的人员不再显示。
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        //[AbpAuthorize(AppPermissions.Pages_Administration_OrganizationUnits_ManageMembers)]
        //public async Task<PagedResultDto<FindUsersDto>> FindAllUsers(NewFindOrganizationUnitUsersInput input)
        //{
        //    var userIdsInOrganizationUnit = _userOrganizationUnitRepository.GetAll()
        //        .Where(uou => uou.OrganizationUnitId == input.OrganizationUnitId)
        //        .Select(uou => uou.UserId);
        //    var query = UserManager.Users;
        //    if (input.Type == 1)
        //    {
        //        query = query
        //           .Where(u => userIdsInOrganizationUnit.Contains(u.Id))
        //           .WhereIf(
        //               !input.Filter.IsNullOrWhiteSpace(),
        //               u =>
        //                   u.Name.Contains(input.Filter) ||
        //                   u.Surname.Contains(input.Filter) ||
        //                   u.UserName.Contains(input.Filter) ||
        //                   u.EmailAddress.Contains(input.Filter)
        //           );
        //    }

        //    if (input.Type == 2)
        //    {
        //        query = query
        //           .Where(u => !userIdsInOrganizationUnit.Contains(u.Id))
        //           .WhereIf(
        //               !input.Filter.IsNullOrWhiteSpace(),
        //               u =>
        //                   u.Name.Contains(input.Filter) ||
        //                   u.Surname.Contains(input.Filter) ||
        //                   u.UserName.Contains(input.Filter) ||
        //                   u.EmailAddress.Contains(input.Filter)
        //           );
        //    }


        //    var userCount = await query.CountAsync();
        //    var users = await query
        //        .OrderBy("DeletionTime desc")
        //        .PageBy(input)
        //        .ToListAsync();

        //    return new PagedResultDto<FindUsersDto>(
        //        userCount,
        //        users.Select(item =>
        //        {
        //            var dto = new FindUsersDto
        //            {
        //                Id = item.Id,
        //                Name = item.Name,
        //                Email = item.EmailAddress,
        //                WorkNo = item.Surname,
        //                UnitNames = GetUnitNames(item.Id)
        //            };
        //            return dto;
        //        }).ToList());
        //}


        /// <summary>
        /// 根据用户id取所在组织机构名称
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private string GetUnitNames(long id)
        {
            //取业务类型
            var bus = _userOrganizationUnitRepository.GetAll().Where(p => p.UserId == id).ToList();
            string s = "";
            foreach (var item in bus)
            {
                var u = _organizationUnitRepository.GetAllList().Where(p => p.Id == item.OrganizationUnitId);
                if (u.Count() > 0)
                    s += u.FirstOrDefault().DisplayName + ",";
            }
            return s.TrimEnd(',');
        }

        /// <summary>
        /// 根据id获取部门信息
        /// </summary>
        /// <param name="input">部门id</param>
        /// <returns></returns>
        //[AbpAuthorize(AppPermissions.Pages_Administration_OrganizationUnits_ManageMembers)]
        //public OrganizationUnitForEditDto GetOrganizationUnitForEdit(NullableIdDto<long> input)
        //{
        //    OrganizationUnitForEditDto editDto;
        //    if (input.Id.HasValue)
        //    {
        //        var unit = from info in _organizationUnitRepository.GetAll().Where(p => p.Id == input.Id.Value)
        //                   join type in _organizationUnitRepository.GetAllIncluding(p => p.DisplayName) on info.ParentId equals type
        //                           .Id
        //                       into JoinedEmpDept
        //                   from type in JoinedEmpDept.DefaultIfEmpty()
        //                   select new OrganizationUnitForEditDto { Id = info.Id.ToString(), UnitName = info.DisplayName, ParentId = info.ParentId.HasValue ? info.ParentId.ToString() : "", ParentName = type.DisplayName };
        //        editDto = unit.FirstOrDefault();
        //    }
        //    else
        //    {
        //        editDto = new OrganizationUnitForEditDto();

        //    }

        //    return editDto;
        //}
        /// <summary>
        /// 修改子部门code
        /// </summary>
        /// <param name="Pcode">上级部门code</param>
        /// <param name="Pid">上级部门id</param>
        //private async void UpdateUnitCode(string Pcode, long Pid)
        //{
        //    string NewCode = "";
        //    //取子部门
        //    var codelist = await _organizationUnitRepository.GetAll().Where(p => p.ParentId == Pid).ToListAsync();
        //    foreach (var item in codelist)
        //    {
        //        //新code：NewCode+code的后5位
        //        NewCode = Pcode + "." + item.Id;
        //        //修改数据库
        //        var organizationUnit = await _organizationUnitRepository.GetAsync(item.Id);
        //        organizationUnit.Code = NewCode;
        //        //修改该部门下面的子部门code
        //        UpdateUnitCode(NewCode, item.Id);
        //    }
        //}

        #endregion
    }
}