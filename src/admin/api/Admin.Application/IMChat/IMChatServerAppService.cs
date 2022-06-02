using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Dapper.Repositories;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.Linq.Extensions;
using Magicodes.Admin.Authorization.Users;
using Magicodes.Admin.Core.Custom.Basis;
using Magicodes.Admin.Core.Custom.Business;
using Magicodes.Admin.IMChat.Dto;
using Magicodes.Admin.Organizations;
using Magicodes.Admin.TenantReleaseReview.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;


namespace Magicodes.Admin.IMChat
{
    public class IMChatServerAppService : AdminAppServiceBase
    {
        private readonly IRepository<User, long> _userRepository;
        private readonly IRepository<UserRole, long> _userRoleRepository;

        public IMChatServerAppService(IRepository<User, long> userRepository, IRepository<UserRole, long> userRoleRepository)
        {
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
        }
        public Task<List<ServerChatEnListDto>> GetServerChatEnList()
        {
            var query = from usr in _userRepository.GetAll()
                        join userRole in _userRoleRepository.GetAll() on usr.Id equals userRole.UserId into a
                        from uRole in a.DefaultIfEmpty()
                        where uRole.Id==5
                        select new ServerChatEnListDto
                        {
                            ServerChatId = usr.Id,
                            ServerChatName = usr.Name
                        };
            return query.ToListAsync();
        }
    }
}
