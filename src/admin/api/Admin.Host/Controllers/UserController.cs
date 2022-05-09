using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Magicodes.Admin.Web.Controllers
{
	[Route("lh-user/[controller]/[action]")]
	[ApiController]
	public class UserController : AdminControllerBase
	{
		[HttpPost]
		public async Task<string> CreateUserIdentity(string a)
		{
			return a;
		}
	}
}
