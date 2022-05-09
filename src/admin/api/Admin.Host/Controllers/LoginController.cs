using Abp;
using Abp.AspNetZeroCore.Web.Authentication.External;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.MultiTenancy;
using Abp.Runtime.Caching;
using Abp.Runtime.Security;
using Abp.UI;
using Magicodes.Admin.Authorization;
using Magicodes.Admin.Authorization.Impersonation;
using Magicodes.Admin.Authorization.Users;
using Magicodes.Admin.Dto;
using Magicodes.Admin.MultiTenancy;
using Magicodes.Admin.Web.Authentication.JwtBearer;
using Magicodes.Admin.Web.Models.SingleLogIn;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Magicodes.Admin.Web.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	public class LoginController : AdminControllerBase
	{
		private readonly LogInManager _logInManager;
		private readonly ITenantCache _tenantCache;
		private readonly AbpLoginResultTypeHelper _abpLoginResultTypeHelper;
		private readonly TokenAuthConfiguration _configuration;
		private readonly UserManager _userManager;
		private readonly ICacheManager _cacheManager;
		private readonly IExternalAuthConfiguration _externalAuthConfiguration;
		private readonly IExternalAuthManager _externalAuthManager;
		private readonly UserRegistrationManager _userRegistrationManager;
		private readonly IImpersonationManager _impersonationManager;
		private readonly IUserLinkManager _userLinkManager;
		private readonly IUnitOfWorkManager _unitOfWorkManager;
		private readonly IdentityOptions _identityOptions;
		private readonly IRepository<User, long> _userRepository;
		private readonly TenantAppService _tenantAppService;
		private readonly ILogger _logger;

		public LoginController(
			LogInManager logInManager,
			ITenantCache tenantCache,
			AbpLoginResultTypeHelper abpLoginResultTypeHelper,
			TokenAuthConfiguration configuration,
			UserManager userManager,
			ICacheManager cacheManager,
			IExternalAuthConfiguration externalAuthConfiguration,
			IExternalAuthManager externalAuthManager,
			UserRegistrationManager userRegistrationManager,
			IImpersonationManager impersonationManager,
			IUserLinkManager userLinkManager,
			IUnitOfWorkManager unitOfWorkManager,
			IRepository<User, long> userRepository,
			IOptions<IdentityOptions> identityOptions,
			TenantAppService tenantAppService,
			ILogger<LoginController> logger)
		{
			_logInManager = logInManager;
			_tenantCache = tenantCache;
			_abpLoginResultTypeHelper = abpLoginResultTypeHelper;
			_configuration = configuration;
			_userManager = userManager;
			_cacheManager = cacheManager;
			_externalAuthConfiguration = externalAuthConfiguration;
			_externalAuthManager = externalAuthManager;
			_userRegistrationManager = userRegistrationManager;
			_impersonationManager = impersonationManager;
			_userLinkManager = userLinkManager;
			_unitOfWorkManager = unitOfWorkManager;
			_userRepository = userRepository;
			_identityOptions = identityOptions.Value;
			_tenantAppService = tenantAppService;
			_logger = logger;
		}

		#region 单点登陆修改
		/// <summary>
		/// 根据用户工号获取用户所在的租户列表
		/// </summary>
		/// <param name="LH_LinkCode">陆海code</param>
		/// <param name="url">跳转url</param>
		/// <returns></returns>
		[HttpGet]
		public async Task<List<SingleLogInGetTenantModel>> GetTenantByUserCode(string LH_LinkCode, string url)
		{
			//step1:根据code获取accessToken
			Dictionary<string, string> dic = new Dictionary<string, string>();
			dic.Add("code", LH_LinkCode);
			dic.Add("redirect_uri", url);
			dic.Add("grant_type", "authorization_code");
			dic.Add("client_id", "lh_shipagent");
			dic.Add("client_secret", "1wjgyJCriSO7MXh2tVJTbj5eVLbZfqMk");
			FirstRet firstRt = this.PostLHToken(dic);
			_logger.LogInformation(firstRt.error + "：" + firstRt.error_description);
			if (string.IsNullOrEmpty(firstRt.access_token))
			{
				throw new UserFriendlyException(3000, firstRt.error + "：" + firstRt.error_description);
			}
			//step2：根据accessToken获取陆海通平台用户信息
			SecondRet secondRet = this.GetComCode(firstRt.access_token);
			//SecondRet secondRet = this.GetComCode("AT-105224-I7kilrWoKDHzOWl6sboQawy97BkM1TDC");
			List<SingleLogInGetTenantModel> cacheUserList = new List<SingleLogInGetTenantModel>();
			cacheUserList = _cacheManager
				.GetCache(AppConsts.TenantListKey).GetOrDefault<string, List<SingleLogInGetTenantModel>>(AppConsts.TenantListValueKey);
			if (cacheUserList == null)
			{
				//更新缓存
				await getAllUserTenant();
				cacheUserList = _cacheManager
				.GetCache(AppConsts.TenantListKey).GetOrDefault<string, List<SingleLogInGetTenantModel>>(AppConsts.TenantListValueKey);
			}
			List<SingleLogInGetTenantModel> rt = cacheUserList.Where(x => x.ComCode == secondRet.com_code).ToList();
			return rt;
		}
		/// <summary>
		/// 获取全部租户的全部客户信息更新缓存
		/// </summary>
		/// <returns></returns>
		async Task getAllUserTenant()
		{
			List<SingleLogInGetTenantModel> catchList = new List<SingleLogInGetTenantModel>();
			List<GetDataComboItemDto<string>> tenantList = await _tenantAppService.getTenantsComboItemDtoList();
			foreach (var tenant in tenantList)
			{
				//切换租户
				_unitOfWorkManager.Current.SetTenantId(Int32.Parse(tenant.Value));
				//获取该租户下的客户列表
				var userList = _userRepository.GetAll().ToList();
				foreach (var user in userList)
				{
					SingleLogInGetTenantModel singleLogInGetTenantModel = new SingleLogInGetTenantModel();
					singleLogInGetTenantModel.tenantId = Convert.ToInt32(tenant.Value);
					singleLogInGetTenantModel.tenantName = tenant.DisplayName;
					catchList.Add(singleLogInGetTenantModel);
				}
			}
			if (catchList.Count > 0)
			{
				_cacheManager.GetCache(AppConsts.TenantListKey).Set(AppConsts.TenantListValueKey, catchList, _configuration.Expiration * 365);
			}

		}
		/// <summary>
		/// 登陆
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		public async Task<SingleLogInRetModel> SingleLogIn([FromBody] SingleLogInModel model)
		{
			_unitOfWorkManager.Current.SetTenantId(model.TenantId);
			var user = _userRepository.GetAll().Where(x => x.UserName == model.UserName).FirstOrDefault();
			if (user == null)
			{
				throw new UserFriendlyException(3000, "未找到此用户信息");
			}
			var loginResult = await GetLoginResultAsync(
				user.UserName,
				"000000",
				GetTenancyNameOrNull()
			);
			var returnUrl = model.ReturnUrl;
			//判断该用户是否被锁定，锁定不能登录
			if (loginResult.User.IsLockoutEnabled)
			{
				throw new UserFriendlyException(3000, "该用户已被锁定，请联系管理员！");
			}
			if (loginResult.Result == AbpLoginResultType.Success)
			{
				loginResult.User.SetSignInToken();
				returnUrl = AddSingleSignInParametersToReturnUrl(model.ReturnUrl, loginResult.User.SignInToken, loginResult.User.Id, loginResult.User.TenantId);
			}
			//Login!
			var accessToken = CreateAccessToken(await CreateJwtClaims(loginResult.Identity, loginResult.User));

			//var userinfo = from currentUser in _userRepository.GetAll().Where(x => x.Id == user.Id)
			//			   join 
			return new SingleLogInRetModel
			{
				AccessToken = accessToken,
				EncryptedAccessToken = GetEncrpyedAccessToken(accessToken),
				ExpireInSeconds = (int)_configuration.Expiration.TotalSeconds,
				UserId = loginResult.User.Id,
				ReturnUrl = returnUrl,
				UserRealName = user.Name,
				//性别
				Sex = user.Sex,
				//登陆明
				LogName = user.UserName,
				MobilePhone = user.PhoneNumber,
				Tel = user.SignInToken
			};
		}
		private string GetTenancyNameOrNull()
		{
			if (!AbpSession.TenantId.HasValue)
			{
				return null;
			}
			return _tenantCache.GetOrNull(AbpSession.TenantId.Value)?.TenancyName;
		}

		private async Task<AbpLoginResult<Tenant, User>> GetLoginResultAsync(string usernameOrEmailAddress, string password, string tenancyName)
		{
			var loginResult = await _logInManager.LoginAsync(usernameOrEmailAddress, password, tenancyName);

			switch (loginResult.Result)
			{
				case AbpLoginResultType.Success:
					return loginResult;
				default:
					throw _abpLoginResultTypeHelper.CreateExceptionForFailedLoginAttempt(loginResult.Result, usernameOrEmailAddress, tenancyName);
			}
		}
		private string AddSingleSignInParametersToReturnUrl(string returnUrl, string signInToken, long userId, int? tenantId)
		{
			returnUrl += (returnUrl.Contains("?") ? "&" : "?") +
						 "accessToken=" + signInToken +
						 "&userId=" + Convert.ToBase64String(Encoding.UTF8.GetBytes(userId.ToString()));
			if (tenantId.HasValue)
			{
				returnUrl += "&tenantId=" + Convert.ToBase64String(Encoding.UTF8.GetBytes(tenantId.Value.ToString()));
			}

			return returnUrl;
		}
		private string CreateAccessToken(IEnumerable<Claim> claims, TimeSpan? expiration = null)
		{
			var now = DateTime.UtcNow;

			var jwtSecurityToken = new JwtSecurityToken(
				issuer: _configuration.Issuer,
				audience: _configuration.Audience,
				claims: claims,
				notBefore: now,
				expires: now.Add(expiration ?? _configuration.Expiration),
				signingCredentials: _configuration.SigningCredentials
			);

			return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
		}

		private async Task<IEnumerable<Claim>> CreateJwtClaims(ClaimsIdentity identity, User user, TimeSpan? expiration = null)
		{
			var tokenValidityKey = Guid.NewGuid().ToString();
			var claims = identity.Claims.ToList();
			var nameIdClaim = claims.First(c => c.Type == _identityOptions.ClaimsIdentity.UserIdClaimType);

			if (_identityOptions.ClaimsIdentity.UserIdClaimType != JwtRegisteredClaimNames.Sub)
			{
				claims.Add(new Claim(JwtRegisteredClaimNames.Sub, nameIdClaim.Value));
			}

			var userIdentifier = new UserIdentifier(AbpSession.TenantId, Convert.ToInt64(nameIdClaim.Value));

			claims.AddRange(new[]
			{
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
				new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.Now.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
				new Claim(AppConsts.TokenValidityKey, tokenValidityKey),
				new Claim(AppConsts.UserIdentifier, userIdentifier.ToUserIdentifierString())
			});

			_cacheManager
				.GetCache(AppConsts.TokenValidityKey)
				.Set(tokenValidityKey, "", expiration ?? _configuration.Expiration);

			await _userManager.AddTokenValidityKeyAsync(user, tokenValidityKey,
				DateTime.UtcNow.Add(expiration ?? _configuration.Expiration));

			return claims;
		}
		private static string GetEncrpyedAccessToken(string accessToken)
		{
			return SimpleStringCipher.Instance.Encrypt(accessToken, AppConsts.DefaultPassPhrase);
		}
		#endregion



		#region 陆海通调取方法
		/// <summary>
		/// 用陆海登陆code换取token
		/// </summary>
		/// <param name="paras"></param>
		/// <returns></returns>
		FirstRet PostLHToken(Dictionary<string, string> paras)
		{
			try
			{
				string url = "http://sso.sdland-sea.com/backend/oauth/token";    //需要访问的url地址
				HttpClient client = new HttpClient();

				//添加post请求的header参数
				var content = new FormUrlEncodedContent(paras);
				content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");
				//post异步请求
				var response = client.PostAsync(url, content).Result;
				//返回结果
				var result = response.Content.ReadAsStringAsync().Result;
				FirstRet rt = new FirstRet() { };
				rt = JsonConvert.DeserializeObject<FirstRet>(result);

				return rt;
			}
			catch (global::System.Exception)
			{
				throw;
			}
		}
		/// <summary>
		/// 用token换取用户信息
		/// </summary>
		/// <param name="accessToken"></param>
		/// <returns></returns>
		SecondRet GetComCode(string accessToken)
		{
			try
			{
				string requestUrl = "http://sso.sdland-sea.com/backend/oauth/userinfo?access_token=" + accessToken;
				string retString = string.Empty;

				HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);
				request.Method = "GET";

				HttpWebResponse response = (HttpWebResponse)request.GetResponse();
				Stream myResponseStream = response.GetResponseStream();
				StreamReader streamReader = new StreamReader(myResponseStream);
				retString = streamReader.ReadToEnd();
				streamReader.Close();
				myResponseStream.Close();
				SecondRet ret = JsonConvert.DeserializeObject<SecondRet>(retString);
				return ret;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		#endregion
	}
}
