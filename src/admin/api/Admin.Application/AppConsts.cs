﻿namespace Magicodes.Admin
{
	/// <summary>
	/// Some consts used in the application.
	/// </summary>
	public class AppConsts
	{
		/// <summary>
		/// Default page size for paged requests.
		/// </summary>
		public const int DefaultPageSize = 10;

		/// <summary>
		/// Maximum allowed page size for paged requests.
		/// </summary>
		public const int MaxPageSize = 1000;

		/// <summary>
		/// Default pass phrase for SimpleStringCipher decrypt/encrypt operations
		/// </summary>
		public const string DefaultPassPhrase = "gsKxGZ012HLL3MI5";

		public const int ResizedMaxProfilPictureBytesUserFriendlyValue = 1024;

		public const int MaxProfilPictureBytesUserFriendlyValue = 5;

		public const string TokenValidityKey = "token_validity_key";

		public static string UserIdentifier = "user_identifier";
		/// <summary>
		/// 单点登陆缓存key
		/// </summary>
		public static string TenantListKey = "singlelogin_tenantlist";
		/// <summary>
		/// 单点登陆获取具体值的key
		/// </summary>
		public static string TenantListValueKey = "LHSingleLogInList";
	}
}
