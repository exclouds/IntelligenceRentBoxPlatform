using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Abp;
using Abp.AspNetCore.Mvc.Authorization;
using Abp.AspNetZeroCore.Web.Authentication.External;
using Abp.Authorization;
using Abp.Authorization.Users;
using Abp.MultiTenancy;
using Abp.Configuration;
using Abp.Extensions;
using Abp.Net.Mail;
using Abp.Notifications;
using Abp.Runtime.Caching;
using Abp.Runtime.Security;
using Abp.Runtime.Session;
using Abp.Timing;
using Abp.UI;
using Abp.Zero.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Magicodes.Admin.Authentication.TwoFactor.Google;
using Magicodes.Admin.Authorization;
using Magicodes.Admin.Authorization.Users;
using Magicodes.Admin.MultiTenancy;
using Magicodes.Admin.Web.Authentication.JwtBearer;
using Magicodes.Admin.Web.Authentication.TwoFactor;
using Magicodes.Admin.Web.Models.TokenAuth;
using Magicodes.Admin.Authorization.Impersonation;
using Magicodes.Admin.Identity;
using Magicodes.Admin.Notifications;
using Magicodes.WeChat.SDK.Apis.OAuth;
using Magicodes.WeChat.SDK.Apis.Token;
using Abp.Domain.Repositories;
using Admin.Application.Custom.API.CustRegist.Dto;
using Magicodes.Admin.Organizations;
//using Microsoft.AspNetCore.Cors;

namespace Magicodes.Admin.Web.Controllers
{
    //[EnableCors("AllowAll")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TokenAuthController : AdminControllerBase
    {
        private const string UserIdentifierClaimType = "http://aspnetzero.com/claims/useridentifier";

        private readonly LogInManager _logInManager;
        private readonly ITenantCache _tenantCache;
        private readonly AbpLoginResultTypeHelper _abpLoginResultTypeHelper;
        private readonly TokenAuthConfiguration _configuration;
        private readonly UserManager _userManager;
        private readonly ICacheManager _cacheManager;
        private readonly IOptions<JwtBearerOptions> _jwtOptions;
        private readonly IExternalAuthConfiguration _externalAuthConfiguration;
        private readonly IExternalAuthManager _externalAuthManager;
        private readonly UserRegistrationManager _userRegistrationManager;
        private readonly IImpersonationManager _impersonationManager;
        private readonly IUserLinkManager _userLinkManager;
        private readonly IAppNotifier _appNotifier;
        private readonly ISmsSender _smsSender;
        private readonly IEmailSender _emailSender;
        private readonly IdentityOptions _identityOptions;
        private readonly GoogleAuthenticatorProvider _googleAuthenticatorProvider;
        private readonly IRepository<MyOrganization, long> _organizationUnitRepository;

        //
        //private readonly IUserEmailer _userEmailer;
        //private readonly IPasswordHasher<User> _passwordHasher;
        //private readonly IWebUrlService _webUrlService;

        public TokenAuthController(
            LogInManager logInManager,
            ITenantCache tenantCache,
            AbpLoginResultTypeHelper abpLoginResultTypeHelper,
            TokenAuthConfiguration configuration,
            UserManager userManager,
            ICacheManager cacheManager,
            IOptions<JwtBearerOptions> jwtOptions,
            IExternalAuthConfiguration externalAuthConfiguration,
            IExternalAuthManager externalAuthManager,
            UserRegistrationManager userRegistrationManager,
            IImpersonationManager impersonationManager,
            IUserLinkManager userLinkManager,
            IAppNotifier appNotifier,
            ISmsSender smsSender,
            IEmailSender emailSender,
            IOptions<IdentityOptions> identityOptions,
            GoogleAuthenticatorProvider googleAuthenticatorProvider,
            IRepository<MyOrganization, long> organizationUnitRepository
            //IUserEmailer userEmailer,
            //IPasswordHasher<User> passwordHasher,
            //IWebUrlService webUrlService
            )
        {
            _logInManager = logInManager;
            _tenantCache = tenantCache;
            _abpLoginResultTypeHelper = abpLoginResultTypeHelper;
            _configuration = configuration;
            _userManager = userManager;
            _cacheManager = cacheManager;
            _jwtOptions = jwtOptions;
            _externalAuthConfiguration = externalAuthConfiguration;
            _externalAuthManager = externalAuthManager;
            _userRegistrationManager = userRegistrationManager;
            _impersonationManager = impersonationManager;
            _userLinkManager = userLinkManager;
            _appNotifier = appNotifier;
            _smsSender = smsSender;
            _emailSender = emailSender;
            _googleAuthenticatorProvider = googleAuthenticatorProvider;
            _identityOptions = identityOptions.Value;
            //_userEmailer = userEmailer;
            //_passwordHasher = passwordHasher;
            //_webUrlService = webUrlService;
            _organizationUnitRepository = organizationUnitRepository;

        }

        /// <summary>
        /// 登陆接口
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<AuthenticateResultModel> Authenticate([FromBody] AuthenticateModel model)
        {
            //try
            //{
            //    IsTenantAvailableInput input = new IsTenantAvailableInput();
            //    input.TenancyName = "Default";
            //    AccountAppService account = new AccountAppService(_userEmailer, _userRegistrationManager, _impersonationManager, _userLinkManager, _passwordHasher, _webUrlService);
            //    await account.IsTenantAvailable(input);
            //}
            //catch (Exception ex)
            //{

            //    throw;
            //}



            var loginResult = await GetLoginResultAsync(
                model.UserNameOrEmailAddress,
                model.Password,
                GetTenancyNameOrNull()
            );

            var returnUrl = model.ReturnUrl;
            


            //判断该用户是否被锁定，锁定不能登录
            if (loginResult.User.IsLockoutEnabled)
            {
                throw new UserFriendlyException("该用户已被锁定，请联系管理员！");
            }
            //if (model.SingleSignIn.HasValue && model.SingleSignIn.Value && loginResult.Result == AbpLoginResultType.Success)
            //{
            //	loginResult.User.SetSignInToken();
            //	returnUrl = AddSingleSignInParametersToReturnUrl(model.ReturnUrl, loginResult.User.SignInToken, loginResult.User.Id, loginResult.User.TenantId);
            //}

            //Password reset
            //if (loginResult.User.ShouldChangePasswordOnNextLogin)
            //{
            //	loginResult.User.SetNewPasswordResetCode();
            //	return new AuthenticateResultModel
            //	{
            //		ShouldResetPassword = true,
            //		PasswordResetCode = loginResult.User.PasswordResetCode,
            //		UserId = loginResult.User.Id,
            //		ReturnUrl = returnUrl
            //	};
            //}

            //Two factor auth
            await _userManager.InitializeOptionsAsync(loginResult.Tenant?.Id);
            //string twoFactorRememberClientToken = null;
            //if (await IsTwoFactorAuthRequiredAsync(loginResult, model))
            //{
            //	if (model.TwoFactorVerificationCode.IsNullOrEmpty())
            //	{
            //		//Add a cache item which will be checked in SendTwoFactorAuthCode to prevent sending unwanted two factor code to users.
            //		_cacheManager
            //			.GetTwoFactorCodeCache()
            //			.Set(
            //				loginResult.User.ToUserIdentifier().ToString(),
            //				new TwoFactorCodeCacheItem()
            //			);

            //		return new AuthenticateResultModel
            //		{
            //			RequiresTwoFactorVerification = true,
            //			UserId = loginResult.User.Id,
            //			TwoFactorAuthProviders = await _userManager.GetValidTwoFactorProvidersAsync(loginResult.User),
            //			ReturnUrl = returnUrl
            //		};
            //	}

            //	twoFactorRememberClientToken = await TwoFactorAuthenticateAsync(loginResult.User, model);
            //}

            //Login!
            var accessToken = CreateAccessToken(await CreateJwtClaims(loginResult.Identity, loginResult.User));
            Console.WriteLine(AbpSession.UserId.ToString() + "***********************************************");

            return new AuthenticateResultModel
            {
                AccessToken = accessToken,
                EncryptedAccessToken = GetEncrpyedAccessToken(accessToken),
                ExpireInSeconds = (int)_configuration.Expiration.TotalSeconds,
                TwoFactorRememberClientToken = "",
                UserId = loginResult.User.Id,
                //RoleId=loginResult.,
                ReturnUrl = returnUrl
            };
        }

        [HttpGet]
        [AbpAuthorize]
        public async Task LogOut()
        {
            if (AbpSession.UserId != null)
            {
                var tokenValidityKeyInClaims = User.Claims.First(c => c.Type == AppConsts.TokenValidityKey);
                await _userManager.RemoveTokenValidityKeyAsync(_userManager.GetUser(AbpSession.ToUserIdentifier()), tokenValidityKeyInClaims.Value);
                _cacheManager.GetCache(AppConsts.TokenValidityKey).Remove(tokenValidityKeyInClaims.Value);
            }
        }

        [HttpPost]
        public async Task SendTwoFactorAuthCode([FromBody] SendTwoFactorAuthCodeModel model)
        {
            var cacheKey = new UserIdentifier(AbpSession.TenantId, model.UserId).ToString();

            var cacheItem = await _cacheManager
                .GetTwoFactorCodeCache()
                .GetOrDefaultAsync(cacheKey);

            if (cacheItem == null)
            {
                throw new UserFriendlyException(L("SendSecurityCodeErrorMessage"));
            }

            var user = await _userManager.FindByIdAsync(model.UserId.ToString());

            if (model.Provider != GoogleAuthenticatorProvider.Name)
            {
                cacheItem.Code = await _userManager.GenerateTwoFactorTokenAsync(user, model.Provider);
                var message = L("EmailSecurityCodeBody", cacheItem.Code);

                if (model.Provider == "Email")
                {
                    await _emailSender.SendAsync(await _userManager.GetEmailAsync(user), L("EmailSecurityCodeSubject"),
                        message);
                }
                else if (model.Provider == "Phone")
                {
                    await _smsSender.SendCodeAsync(await _userManager.GetPhoneNumberAsync(user), cacheItem.Code);
                }
            }

            _cacheManager.GetTwoFactorCodeCache().Set(
                    cacheKey,
                    cacheItem
                );
            _cacheManager.GetCache("ProviderCache").Set(
                "Provider",
                model.Provider
            );
        }

        [HttpPost]
        public async Task<ImpersonatedAuthenticateResultModel> ImpersonatedAuthenticate(string impersonationToken)
        {
            var result = await _impersonationManager.GetImpersonatedUserAndIdentity(impersonationToken);
            var accessToken = CreateAccessToken(await CreateJwtClaims(result.Identity, result.User));

            return new ImpersonatedAuthenticateResultModel
            {
                AccessToken = accessToken,
                EncryptedAccessToken = GetEncrpyedAccessToken(accessToken),
                ExpireInSeconds = (int)_configuration.Expiration.TotalSeconds
            };
        }

        [HttpPost]
        public async Task<SwitchedAccountAuthenticateResultModel> LinkedAccountAuthenticate(string switchAccountToken)
        {
            var result = await _userLinkManager.GetSwitchedUserAndIdentity(switchAccountToken);
            var accessToken = CreateAccessToken(await CreateJwtClaims(result.Identity, result.User));

            return new SwitchedAccountAuthenticateResultModel
            {
                AccessToken = accessToken,
                EncryptedAccessToken = GetEncrpyedAccessToken(accessToken),
                ExpireInSeconds = (int)_configuration.Expiration.TotalSeconds
            };
        }

        [HttpGet]
        public List<ExternalLoginProviderInfoModel> GetExternalAuthenticationProviders()
        {
            return ObjectMapper.Map<List<ExternalLoginProviderInfoModel>>(_externalAuthConfiguration.Providers);
        }


        [HttpGet]
        public Task<string> GetWeChatAuthUrl(string url, string state = "")
        {
            var oauthApi = new OAuthApi();
            var result = oauthApi.GetScanCodeAuthorizeUrl(url, state, OAuthScopes.snsapi_login);
            return Task.FromResult(result);
        }

        [HttpPost]
        public async Task<ExternalAuthenticateResultModel> ExternalAuthenticate([FromBody] ExternalAuthenticateModel model)
        {
            var externalUser = await GetExternalUserInfo(model);

            var loginResult = await _logInManager.LoginAsync(new UserLoginInfo(model.AuthProvider, model.ProviderKey, model.AuthProvider), GetTenancyNameOrNull());

            switch (loginResult.Result)
            {
                case AbpLoginResultType.Success:
                    {
                        var accessToken = CreateAccessToken(await CreateJwtClaims(loginResult.Identity, loginResult.User));

                        var returnUrl = model.ReturnUrl;

                        if (model.SingleSignIn.HasValue && model.SingleSignIn.Value && loginResult.Result == AbpLoginResultType.Success)
                        {
                            loginResult.User.SetSignInToken();
                            returnUrl = AddSingleSignInParametersToReturnUrl(model.ReturnUrl, loginResult.User.SignInToken, loginResult.User.Id, loginResult.User.TenantId);
                        }

                        return new ExternalAuthenticateResultModel
                        {
                            AccessToken = accessToken,
                            EncryptedAccessToken = GetEncrpyedAccessToken(accessToken),
                            ExpireInSeconds = (int)_configuration.Expiration.TotalSeconds,
                            ReturnUrl = returnUrl
                        };
                    }
                case AbpLoginResultType.UnknownExternalLogin:
                    {
                        var newUser = await RegisterExternalUserAsync(externalUser);
                        if (!newUser.IsActive)
                        {
                            return new ExternalAuthenticateResultModel
                            {
                                WaitingForActivation = true
                            };
                        }

                        //Try to login again with newly registered user!
                        loginResult = await _logInManager.LoginAsync(new UserLoginInfo(model.AuthProvider, model.ProviderKey, model.AuthProvider), GetTenancyNameOrNull());
                        if (loginResult.Result != AbpLoginResultType.Success)
                        {
                            throw _abpLoginResultTypeHelper.CreateExceptionForFailedLoginAttempt(
                                loginResult.Result,
                                model.ProviderKey,
                                GetTenancyNameOrNull()
                            );
                        }

                        var accessToken = CreateAccessToken(await CreateJwtClaims(loginResult.Identity, loginResult.User));
                        return new ExternalAuthenticateResultModel
                        {
                            AccessToken = accessToken,
                            EncryptedAccessToken = GetEncrpyedAccessToken(accessToken),
                            ExpireInSeconds = (int)_configuration.Expiration.TotalSeconds
                        };
                    }
                default:
                    {
                        throw _abpLoginResultTypeHelper.CreateExceptionForFailedLoginAttempt(
                            loginResult.Result,
                            model.ProviderKey,
                            GetTenancyNameOrNull()
                        );
                    }
            }
        }

        [HttpPost]
        public async Task<ExternalAuthenticateResultModel> WeChatAuthenticate([FromBody] ExternalAuthenticateModel model)
        {
            var externalUser = await GetExternalUserInfo(model);
            Logger.Info($"用户模型:{Newtonsoft.Json.JsonConvert.SerializeObject(externalUser)}");
            Logger.Debug(Newtonsoft.Json.JsonConvert.SerializeObject(new UserLoginInfo(model.AuthProvider, externalUser.ProviderKey, model.AuthProvider)) + GetTenancyNameOrNull());
            var loginResult = await _logInManager.LoginAsync(new UserLoginInfo(model.AuthProvider, externalUser.ProviderKey, model.AuthProvider), GetTenancyNameOrNull());
            Logger.Debug(loginResult.Result.ToString());
            switch (loginResult.Result)
            {
                case AbpLoginResultType.Success:
                    {
                        var accessToken = CreateAccessToken(await CreateJwtClaims(loginResult.Identity, loginResult.User));

                        var returnUrl = model.ReturnUrl;

                        if (model.SingleSignIn.HasValue && model.SingleSignIn.Value && loginResult.Result == AbpLoginResultType.Success)
                        {
                            loginResult.User.SetSignInToken();
                            returnUrl = AddSingleSignInParametersToReturnUrl(model.ReturnUrl, loginResult.User.SignInToken, loginResult.User.Id, loginResult.User.TenantId);
                        }

                        return new ExternalAuthenticateResultModel
                        {
                            AccessToken = accessToken,
                            EncryptedAccessToken = GetEncrpyedAccessToken(accessToken),
                            ExpireInSeconds = (int)_configuration.Expiration.TotalSeconds,
                            ReturnUrl = returnUrl
                        };
                    }
                case AbpLoginResultType.UnknownExternalLogin:
                    {
                        var newUser = await RegisterExternalUserAsync(externalUser);
                        if (!newUser.IsActive)
                        {
                            return new ExternalAuthenticateResultModel
                            {
                                WaitingForActivation = true
                            };
                        }

                        //Try to login again with newly registered user!
                        loginResult = await _logInManager.LoginAsync(new UserLoginInfo(model.AuthProvider, model.ProviderKey, model.AuthProvider), GetTenancyNameOrNull());
                        if (loginResult.Result != AbpLoginResultType.Success)
                        {
                            throw _abpLoginResultTypeHelper.CreateExceptionForFailedLoginAttempt(
                                loginResult.Result,
                                model.ProviderKey,
                                GetTenancyNameOrNull()
                            );
                        }

                        var accessToken = CreateAccessToken(await CreateJwtClaims(loginResult.Identity, loginResult.User));
                        return new ExternalAuthenticateResultModel
                        {
                            AccessToken = accessToken,
                            EncryptedAccessToken = GetEncrpyedAccessToken(accessToken),
                            ExpireInSeconds = (int)_configuration.Expiration.TotalSeconds
                        };
                    }
                default:
                    {
                        throw _abpLoginResultTypeHelper.CreateExceptionForFailedLoginAttempt(
                            loginResult.Result,
                            model.ProviderKey,
                            GetTenancyNameOrNull()
                        );
                    }
            }
        }

        #region Etc

        [AbpMvcAuthorize]
        [HttpGet]
        public async Task<ActionResult> TestNotification(string message = "", string severity = "info")
        {
            if (message.IsNullOrEmpty())
            {
                message = "This is a test notification, created at " + Clock.Now;
            }

            await _appNotifier.SendMessageAsync(
                AbpSession.ToUserIdentifier(),
                message,
                severity.ToPascalCase().ToEnum<NotificationSeverity>()
                );

            return Content("Sent notification: " + message);
        }

        #endregion

        private async Task<User> RegisterExternalUserAsync(ExternalAuthUserInfo externalLoginInfo)
        {
            var user = await _userRegistrationManager.RegisterAsync(
                externalLoginInfo.Name,
                externalLoginInfo.Surname,
                externalLoginInfo.EmailAddress,
                externalLoginInfo.EmailAddress.ToMd5(),
                Authorization.Users.User.CreateRandomPassword(),
                true,
                null
            );

            user.Logins = new List<UserLogin>
            {
                new UserLogin
                {
                    LoginProvider = externalLoginInfo.Provider,
                    ProviderKey = externalLoginInfo.ProviderKey,
                    TenantId = user.TenantId
                }
            };

            await CurrentUnitOfWork.SaveChangesAsync();

            return user;
        }

        private async Task<ExternalAuthUserInfo> GetExternalUserInfo(ExternalAuthenticateModel model)
        {
            var userInfo = await _externalAuthManager.GetUserInfo(model.AuthProvider, model.ProviderAccessCode);
            //if (userInfo.ProviderKey != model.ProviderKey)
            //{
            //    throw new UserFriendlyException(L("CouldNotValidateExternalUser"));
            //}

            return userInfo;
        }

        private async Task<bool> IsTwoFactorAuthRequiredAsync(AbpLoginResult<Tenant, User> loginResult, AuthenticateModel authenticateModel)
        {
            if (!await SettingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames.UserManagement.TwoFactorLogin.IsEnabled))
            {
                return false;
            }

            if (!loginResult.User.IsTwoFactorEnabled)
            {
                return false;
            }

            if ((await _userManager.GetValidTwoFactorProvidersAsync(loginResult.User)).Count <= 0)
            {
                return false;
            }

            if (await TwoFactorClientRememberedAsync(loginResult.User.ToUserIdentifier(), authenticateModel))
            {
                return false;
            }

            return true;
        }

        private async Task<bool> TwoFactorClientRememberedAsync(UserIdentifier userIdentifier, AuthenticateModel authenticateModel)
        {
            if (!await SettingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames.UserManagement.TwoFactorLogin.IsRememberBrowserEnabled))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(authenticateModel.TwoFactorRememberClientToken))
            {
                return false;
            }

            try
            {
                var validationParameters = new TokenValidationParameters
                {
                    ValidAudience = _configuration.Audience,
                    ValidIssuer = _configuration.Issuer,
                    IssuerSigningKey = _configuration.SecurityKey
                };

                foreach (var validator in _jwtOptions.Value.SecurityTokenValidators)
                {
                    if (validator.CanReadToken(authenticateModel.TwoFactorRememberClientToken))
                    {
                        try
                        {
                            SecurityToken validatedToken;
                            var principal = validator.ValidateToken(authenticateModel.TwoFactorRememberClientToken, validationParameters, out validatedToken);
                            var useridentifierClaim = principal.FindFirst(c => c.Type == UserIdentifierClaimType);
                            if (useridentifierClaim == null)
                            {
                                return false;
                            }

                            return useridentifierClaim.Value == userIdentifier.ToString();
                        }
                        catch (Exception ex)
                        {
                            Logger.Debug(ex.ToString(), ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Debug(ex.ToString(), ex);
            }

            return false;
        }

        /* Checkes two factor code and returns a token to remember the client (browser) if needed */
        private async Task<string> TwoFactorAuthenticateAsync(User user, AuthenticateModel authenticateModel)
        {
            var twoFactorCodeCache = _cacheManager.GetTwoFactorCodeCache();
            var userIdentifier = user.ToUserIdentifier().ToString();
            var cachedCode = await twoFactorCodeCache.GetOrDefaultAsync(userIdentifier);
            var provider = _cacheManager.GetCache("ProviderCache").Get("Provider", cache => cache).ToString();


            //Delete from the cache since it was a single usage code
            await twoFactorCodeCache.RemoveAsync(userIdentifier);

            if (authenticateModel.RememberClient)
            {
                if (await SettingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames.UserManagement.TwoFactorLogin.IsRememberBrowserEnabled))
                {
                    return CreateAccessToken(new[]
                        {
                            new Claim(UserIdentifierClaimType, user.ToUserIdentifier().ToString())
                        },
                        TimeSpan.FromDays(365)
                    );
                }
            }

            return null;
        }

        private string GetTenancyNameOrNull()
        {
            if (!AbpSession.TenantId.HasValue)
            {
                return null;
            }
            //px为了发布的前端可以测试进行添加
            return _tenantCache.GetOrNull(AbpSession.TenantId.Value)?.TenancyName;
        }

        private async Task<AbpLoginResult<Tenant, User>> GetLoginResultAsync(string usernameOrEmailAddress, string password, string tenancyName)
        {
            var loginResult = await _logInManager.LoginAsync(usernameOrEmailAddress, password, "Default"); //"Default"


            switch (loginResult.Result)
            {
                case AbpLoginResultType.Success:
                    return loginResult;
                default:
                    throw _abpLoginResultTypeHelper.CreateExceptionForFailedLoginAttempt(loginResult.Result, usernameOrEmailAddress, tenancyName);
            }
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

        private static string GetEncrpyedAccessToken(string accessToken)
        {
            return SimpleStringCipher.Instance.Encrypt(accessToken, AppConsts.DefaultPassPhrase);
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
        #region 客户登录
        [HttpPost]
        public async Task<LoginResultModel> CustLoginOn([FromBody]AuthenticateModel model)
        {



            var loginResult = await GetLoginResultAsync(
                model.UserNameOrEmailAddress,
                model.Password,
                GetTenancyNameOrNull()
            );

            //  var returnUrl = model.ReturnUrl;


            //判断该用户是否被锁定，锁定不能登录
            if (!loginResult.User.IsVerify && string.IsNullOrEmpty(loginResult.User.VerifyRem))
            {
                throw new UserFriendlyException("该用户注册正在审核中，请稍后重试！");
            }
            if (!loginResult.User.IsVerify && !string.IsNullOrEmpty(loginResult.User.VerifyRem))
            {
                throw new UserFriendlyException("该用户注册未通过审核，未通过原因："+ loginResult.User.VerifyRem + "，请联系管理员！");
            }

            //判断该用户是否被锁定，锁定不能登录
            if (loginResult.User.IsLockoutEnabled)
            {
                throw new UserFriendlyException("该用户已被锁定，请联系管理员！");
            }

            await _userManager.InitializeOptionsAsync(loginResult.Tenant?.Id);


            //Login!
            var accessToken = CreateAccessToken(await CreateJwtClaims(loginResult.Identity, loginResult.User));
            Console.WriteLine(AbpSession.UserId.ToString() + "***********************************************");
            var  returnmode= new LoginResultModel
            {
                AccessToken = accessToken,               
                UserId = loginResult.User.Id,
                Name = loginResult.User.Name,
                CompanyType =string.IsNullOrEmpty(loginResult.User.OrganizationCode) ?
                (loginResult.User.UserNature+2) : loginResult.User.UserNature,
                OrganizationCode = loginResult.User.OrganizationCode,
                UserName = loginResult.User.UserName,
               
                IsAdmin = loginResult.User.IsAdmin,
                TenantId= loginResult.User.TenantId

            };
             if (loginResult.User.OrganizationCode.IsNullOrEmpty() )
            {
                returnmode.CompanyType = 2;
            }
            else 
            {
               
               returnmode.CompanyType = loginResult.User.UserNature;

            }
            //if (CompanyType = loginResult.User.OrganizationCode.IsNullOrEmpty() ? 4 :
            //     loginResult.User.IsAdmin ? 4 :)

            return returnmode;
        }
        #endregion
    }
}
