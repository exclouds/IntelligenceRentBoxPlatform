using System;
using System.Collections.Generic;
using System.Text;

namespace Magicodes.Admin.Organizations.Dto
{
   public class FindUsersDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string WorkNo { get; set; }
        public string Email { get; set; }
        public string UnitNames { get; set; }
    }
}
