using System;
using System.Collections.Generic;
using System.Text;

namespace Magicodes.Admin.Web.Models.SingleLogIn
{
	public class FirstRet
	{
		private static long serialVersionUID = -2833689273468744284L;
		public string access_token { get; set; }
		public string refresh_token { get; set; }
		public string token_type { get; set; }
		public string expires_in { get; set; }
		public string scope { get; set; }
		public string error { get; set; }

		public string error_description { get; set; }
	}
}
