﻿using System;
using System.Collections.Generic;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace Magicodes.Admin.Authorization.Users.Dto
{
    public class UserListDto : EntityDto<long>, IHasCreationTime
    {
        public string Name { get; set; }

        public string Surname { get; set; }

        public string UserName { get; set; }
        public string Sex { get; set; }


        public string EmailAddress { get; set; }

        public string PhoneNumber { get; set; }

        public Guid? ProfilePictureId { get; set; }

        public bool IsEmailConfirmed { get; set; }

        public List<UserListRoleDto> Roles { get; set; }

        public DateTime? LastLoginTime { get; set; }
        

        public DateTime CreationTime { get; set; }
        public bool IsLockoutEnabled { get; set; }
    }
}