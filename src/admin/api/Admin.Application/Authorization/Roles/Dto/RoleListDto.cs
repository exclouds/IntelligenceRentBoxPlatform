﻿using System;
using Abp.Application.Services.Dto;
using Abp.Domain.Entities.Auditing;

namespace Magicodes.Admin.Authorization.Roles.Dto
{
    public class RoleListDto 
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public string DisplayName { get; set; }

        public bool IsStatic { get; set; }

        public bool IsDefault { get; set; }

        public DateTime CreationTime { get; set; }
    }
}