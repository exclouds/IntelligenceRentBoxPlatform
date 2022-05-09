using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Organizations;
using Abp.Reflection.Extensions;
using Abp.UI;
using Magicodes.Admin.Attachments;
using Magicodes.Admin.Authorization;
using Magicodes.Admin.Authorization.Roles;
using Magicodes.Admin.Authorization.Roles.Dto;
using Magicodes.Admin.Authorization.Users;
using Magicodes.Admin.Authorization.Users.Dto;
using Magicodes.Admin.Common.Dto;
using Magicodes.Admin.Contents;
using Magicodes.Admin.Core.Custom;
using Magicodes.Admin.Core.Custom.Basis;
using Magicodes.Admin.Core.Custom.DataDictionary;
using Magicodes.Admin.Organizations;
using Magicodes.Admin.Organizations.Dto;
using Microsoft.EntityFrameworkCore;

namespace Magicodes.Admin.Common
{
    /// <summary>
    /// 通用服务
    /// </summary>
    public class CommonAppService : AppServiceBase, ICommonAppService//AdminAppServiceBase
    {
        private readonly IRepository<ObjectAttachmentInfo, long> _objectAttachmentInfoRepository;
        private readonly ISettingManager _settingManager;
        private readonly IRepository<ColumnInfo, long> _columnInfoRepository;
        private readonly RoleManager _roleManager;
        private readonly IRepository<MyOrganization, long> _organizationUnitRepository;
        private readonly IRepository<User, long> _userRepository;
        /// <summary>
        /// 部门信息
        /// </summary>
        private readonly IRepository<Department, int> _departmentRepository;
        private readonly LogInManager _logInManager;
        private readonly IRepository<BaseKey_Value, long> _kvRepository;

        public CommonAppService(
            IRepository<ObjectAttachmentInfo, long> objectAttachmentInfoRepository
            , ISettingManager settingManager,
            IRepository<ColumnInfo, long> columnInfoRepository
            , RoleManager roleManager
            , IRepository<MyOrganization, long> organizationUnitManager
            , IRepository<User, long> userRepository
            , IRepository<Department, int> departmentRepository
            , LogInManager logInManager
            , IRepository<BaseKey_Value, long> kvRepository)
        {
            _objectAttachmentInfoRepository = objectAttachmentInfoRepository;
            _settingManager = settingManager;
            _columnInfoRepository = columnInfoRepository;
            _roleManager = roleManager;
            _organizationUnitRepository = organizationUnitManager;
            _userRepository = userRepository;
            _departmentRepository = departmentRepository;
            _logInManager = logInManager;
            _kvRepository = kvRepository;
        }

        /// <summary>
        /// 获取枚举值列表
        /// </summary>
        /// <returns></returns>
        public List<GetEnumValuesListDto> GetEnumValuesList(GetEnumValuesListInput input)
        {
            Type type = null;
            if (input.FullName.Contains("Magicodes.Admin.Core.Custom"))
            {

                type = typeof(AppCoreModule).GetAssembly().GetType(input.FullName);
            }
            else
            {
                type = typeof(AdminCoreModule).GetAssembly().GetType(input.FullName);
            }
            //var type = typeof(AdminCoreModule).GetAssembly().GetType(input.FullName);
            if (!type.IsEnum) return null;
            var names = Enum.GetNames(type);
            var values = Enum.GetValues(type);
            var list = new List<GetEnumValuesListDto>();
            var index = 0;
            foreach (var value in values)
            {
                list.Add(new GetEnumValuesListDto()
                {
                    DisplayName = L(names[index]),
                    Value = Convert.ToInt32(value)
                });
                index++;
            }
            return list;
        }

        /// <summary>
        /// 获取对象图片列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<List<GetObjectImagesListDto>> GetObjectImages(GetObjectImagesInput input)
        {
            var objectType = Enum.Parse<AttachmentObjectTypes>(input.ObjectType);
            var list = await _objectAttachmentInfoRepository.GetAllIncluding(p => p.AttachmentInfo)
                .Where(p => p.ObjectId == input.ObjectId && p.ObjectType == objectType &&
                            p.AttachmentInfo.AttachmentType == AttachmentTypes.Image)
                .Select(p => new GetObjectImagesListDto
                {
                    Id = p.AttachmentInfo.Id,
                    Name = p.AttachmentInfo.Name,
                    FileLength = p.AttachmentInfo.FileLength,
                    Url = p.AttachmentInfo.Url,
                    IsCover = p.IsCover
                }).ToListAsync();
            return list;
        }

        /// <summary>
        /// 获取对象封面图片
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<GetObjectImagesListDto> GetObjectCoverImage(SetCoverInputDto input)
        {
            var objectType = Enum.Parse<AttachmentObjectTypes>(input.ObjectType);
            var objectAttachmentInfo = await _objectAttachmentInfoRepository.GetAllIncluding(p => p.AttachmentInfo)
                .Where(p =>
                    p.ObjectId == input.ObjectId && p.ObjectType == objectType &&
                    p.AttachmentInfo.AttachmentType == AttachmentTypes.Image)
                .FirstOrDefaultAsync(a => a.IsCover);
            if (objectAttachmentInfo == null)
            {
                return  new GetObjectImagesListDto();
            }
            return new GetObjectImagesListDto
            {
                Id = objectAttachmentInfo.AttachmentInfo.Id,
                Name = objectAttachmentInfo.AttachmentInfo.Name,
                FileLength = objectAttachmentInfo.AttachmentInfo.FileLength,
                Url = objectAttachmentInfo.AttachmentInfo.Url,
                IsCover = objectAttachmentInfo.IsCover
            };
        }

        /// <summary>
        /// 移除对象附件
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task RemoveObjectAttachments(RemoveObjectAttachmentsInput input)
        {
            var objectType = Enum.Parse<AttachmentObjectTypes>(input.ObjectType);
            await _objectAttachmentInfoRepository.DeleteAsync(p => input.Ids.Contains(p.AttachmentInfoId) && p.ObjectType == objectType);
        }

        /// <summary>
        /// 添加附件关联
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task AddObjectAttachmentInfos(AddObjectAttachmentInfosInput input)
        {
            var objectType = Enum.Parse<AttachmentObjectTypes>(input.ObjectType);
            if (objectType == AttachmentObjectTypes.ColumnInfo)
            {
                if (!CheckMaxItemCount(input))
                {
                    throw new UserFriendlyException(L("ExceedTheMaxCount"));
                }
            }
            var attachmentInfos = await _objectAttachmentInfoRepository.GetAll().Where(p => p.ObjectId == input.ObjectId && p.ObjectType == objectType).ToListAsync();
            var objectAttachmentInfos = input.AttachmentInfoIds.Select(p => new ObjectAttachmentInfo
            {
                ObjectType = objectType,
                ObjectId = input.ObjectId,
                AttachmentInfoId = p
            }).ToList();
            foreach (var objectAttachmentInfo in objectAttachmentInfos)
            {
                if (attachmentInfos == null || attachmentInfos.Count == 0 || (attachmentInfos.All(p => p.AttachmentInfoId != objectAttachmentInfo.AttachmentInfoId)))
                    await _objectAttachmentInfoRepository.InsertAsync(objectAttachmentInfo);
            }
        }

        /// <summary>
        /// 设置封面
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public async Task SetCover(SetCoverInputDto input)
        {
            var objectType = Enum.Parse<AttachmentObjectTypes>(input.ObjectType);
            var objectAttachmentInfos = await _objectAttachmentInfoRepository.GetAllIncluding(p => p.AttachmentInfo)
                .Where(p =>
                    p.ObjectId == input.ObjectId && p.ObjectType == objectType &&
                    p.AttachmentInfo.AttachmentType == AttachmentTypes.Image)
                .ToListAsync();

            var objectAttachment = objectAttachmentInfos.FirstOrDefault(a => a.IsCover);
            if (objectAttachment != null)
            {
                objectAttachment.IsCover = false;
            }
            var setObjectAttachment =
                objectAttachmentInfos.FirstOrDefault(a => a.AttachmentInfo.Url == input.AttachmentUrl);
            if (setObjectAttachment == null)
            {
                throw new UserFriendlyException();
            }
            setObjectAttachment.IsCover = true;
        }

        private bool CheckMaxItemCount(AddObjectAttachmentInfosInput input)
        {
            var columnInfoMaxItemCount = _columnInfoRepository.Get(input.ObjectId).MaxItemCount;
            if (!columnInfoMaxItemCount.HasValue)
            {
                return true;
            }
            var columnInfoCurrentCount = _objectAttachmentInfoRepository.GetAll().Count(a => a.ObjectId == input.ObjectId);
            return columnInfoMaxItemCount > columnInfoCurrentCount;
        }
        /// <summary>
        /// 获取角色下拉
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<List<RoleListDto>> GetRolesList()
        {
            var query = _roleManager.Roles;

            var roles = await query
                .ToListAsync();

            return  ObjectMapper.Map<List<RoleListDto>>(roles);
        }
        /// <summary>
        /// 获取组织机构
        /// </summary>
        /// <returns></returns>
        public async Task<List<NewOrganizationUnitDto>> GetNewOrganizationUnitNoUsers()
        {
            ////组织信息
            //var units = await _organizationUnitRepository.GetAll().ToListAsync();
            ////部门
            //var depts =_departmentRepository.GetAll().ToList();

            //获取登录人部门和公司信息
            var user = _userRepository.GetAll().Where(x => x.Id == AbpSession.UserId).FirstOrDefault();
            //var percomlist = units;
            //percomlist = units.Where(x => ("," + user.OrganizationCode + ",").Contains("," + x.Code + ",")).ToList();

            //if (user.OrganizationCode == "01-0002")
            //{
            //    if (user.DeptCode != "")
            //    {
            //        percomlist = units.Where(x => x.Code == user.OrganizationCode).ToList();
            //    }
            //}
            //else
            //{
            //    percomlist = units.Where(x => x.Code == user.OrganizationCode).ToList();
            //}

            //List<NewOrganizationUnitDto> unitlist = new List<NewOrganizationUnitDto>();
            ////找到根节点
            //foreach (var item in percomlist)
            //{
            //    NewOrganizationUnitDto unit = new NewOrganizationUnitDto();
            //    unit.nodeUUid = item.Code;
            //    unit.nodeId = item.Id;
            //    unit.nodeName = item.DisplayName;
            //    unit.disabled = false;
            //    unit.Message = "";
            //    unit.children = GetChildrenUnitNoUser(item.Code, units, depts, IsShowdep);
            //    unitlist.Add(unit);
            //}
            // var query = _organizationUnitRepository.GetAll();
            if (user.UserName == "admin")
            {
                var query = _organizationUnitRepository.GetAll().Select(p => new NewOrganizationUnitDto
                {
                    nodeUUid = p.Code,
                    nodeId = p.Id,
                    nodeName = p.DisplayName,
                    disabled = false,
                    Message = "",
                    children = null
                });
                var unitlist = query.ToList();
                return unitlist;
            }
            //else if (user.Roles.Select(p => p.Id).ToList().Contains(2))
            //{
            //    var query = _organizationUnitRepository.GetAll().Select(p=>new NewOrganizationUnitDto {
            //        nodeUUid = p.Code,
            //        nodeId=p.Id,
            //        nodeName=p.DisplayName,
            //        disabled=false,
            //        Message = "",
            //        children=null
            //    });
            //    var unitlist = query.ToList();
            //    return unitlist;
            //}
            else
            {
                var query = _organizationUnitRepository.GetAll().Where(x => ("," + user.OrganizationCode + ",").Contains("," + x.Code + ",")).Select(p => new NewOrganizationUnitDto
                {
                    nodeUUid = p.Code,
                    nodeId = p.Id,
                    nodeName = p.DisplayName,
                    disabled = false,
                    Message = "",
                    children = null
                });
                var unitlist = query.ToList();
                return unitlist;
            }
             
               
           
        }
        /// <summary>
        /// 获取子部门
        /// </summary>
        /// <param name="parentCode">上级部门code</param>
        /// <param name="unitlist">公司列表</param>
        /// <param name="deplist">部门</param>
        /// <returns></returns>
        private List<NewOrganizationUnitDto> GetChildrenUnitNoUser(string parentCode, List<MyOrganization> unitlist, List<Department> deplist, bool? IsShowdep)
        {
            List<NewOrganizationUnitDto> unitslist = new List<NewOrganizationUnitDto>();
            if (IsShowdep == true)
            {
                //获取部门信息        
                foreach (var item in deplist.FindAll(p => p.Code == parentCode))
                {
                    NewOrganizationUnitDto unit = new NewOrganizationUnitDto();
                    unit.nodeUUid = item.DepCode;
                    unit.nodeId = item.Id;
                    unit.nodeName = item.DepName;
                    unit.disabled = false;
                    unit.Message = "";
                    unit.children = GetChildrenUnitNoUser(item.DepCode, unitlist, deplist, IsShowdep);
                    unitslist.Add(unit);
                }
            }
            
            //获取子公司信息
            foreach (var item in unitlist.FindAll(p => p.ParentCode == parentCode))
            {
                NewOrganizationUnitDto unit = new NewOrganizationUnitDto();
                unit.nodeUUid = item.Code;
                unit.nodeId = item.Id;
                unit.nodeName = item.DisplayName;
                unit.disabled = false;           
                unit.Message = ""; 
                unit.children = GetChildrenUnitNoUser(item.Code, unitlist, deplist, IsShowdep);
                unitslist.Add(unit);
            }
            return unitslist;
        }
        
        /// <summary>
        /// 获取子部门
        /// </summary>
        /// <param name="code">公司code</param>
        /// <returns></returns>
        public List<NewOrganizationUnitDto> GetDeptUnitList(string code)
        {
            if (!code.IsNullOrEmpty())
            {
                code = "," + code + ",";
            }
            var deptlist = _departmentRepository.GetAll().Where(x =>  code.Contains("," + x.Code + ",")).ToList();
            List<NewOrganizationUnitDto> unitslist = new List<NewOrganizationUnitDto>();
            foreach (var item in deptlist)
            {
                NewOrganizationUnitDto unit = new NewOrganizationUnitDto();
                unit.nodeUUid = item.DepCode;
                unit.nodeId = item.Id;
                unit.nodeName = item.DepName;
                unit.disabled = false;
                unit.Message = "";
                unit.children = null;
                unitslist.Add(unit);
            }

            return unitslist;
        }
        /// <summary>
        /// 获取部门所属
        /// </summary>
        /// <param name="code">公司code</param>
        /// <returns></returns>
        public List<NewOrganizationUnitDto> GetDepList(string code)
        {
            var deptlist = _departmentRepository.GetAll().Where(x => x.Code == code).ToList();           
            var unitslist = deptlist.GroupBy(p => p.DepName)
                .Select(p => new NewOrganizationUnitDto
                {
                    nodeUUid = p.FirstOrDefault().DepCode,
                    nodeId=p.FirstOrDefault().Id,
                    nodeName= p.FirstOrDefault().DepName,
                    disabled = false,
                    Message="",
                    children=null
                }).ToList();

            return unitslist;
        }

        /// <summary>
		/// 修改密码
		/// </summary>
		/// <returns></returns>
		public async Task UpdatePassword(UpdatePasswordInput input)
        {
            if (!input.oldPassword.IsNullOrEmpty())
            {
                //获取用户
                var user = await UserManager.GetUserByIdAsync((long)AbpSession.UserId);
                if (user != null)
                {
                    //取租户名称
                    var tent = await TenantManager.GetByIdAsync((int)user.TenantId);
                    var loginResult = await _logInManager.LoginAsync(user.UserName, input.oldPassword, tent.TenancyName);

                    switch (loginResult.Result)
                    {
                        case AbpLoginResultType.Success:
                            await UserManager.ChangePasswordAsync(user, input.newPassword);
                            break;
                        default:
                            throw new UserFriendlyException(3000, "旧密码错误，请重新输入！");
                    }
                }

            }

        }
    }
}
