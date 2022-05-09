using System.Threading.Tasks;
using Abp.Dependency;
using Abp.UI;
using Microsoft.AspNetCore.Identity;
using Magicodes.Admin.Authorization.Users;

namespace Magicodes.Admin.Authentication.TwoFactor.Google
{
    public class GoogleAuthenticatorProvider : AdminServiceBase, ITransientDependency
    {
        private readonly GoogleTwoFactorAuthenticateService _googleTwoFactorAuthenticateService;

        public GoogleAuthenticatorProvider(GoogleTwoFactorAuthenticateService googleTwoFactorAuthenticateService)
        {
            _googleTwoFactorAuthenticateService = googleTwoFactorAuthenticateService;
        }

        public const string Name = "GoogleAuthenticator";
        public Task<string> GenerateAsync(string purpose, UserManager<User> userManager, User user)
        {

            var setupInfo = _googleTwoFactorAuthenticateService.GenerateSetupCode("Magicodes.Admin", user.EmailAddress, user.UserName, 300, 300);

            return Task.FromResult(setupInfo.QrCodeSetupImageUrl);
        }
        

        public Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<User> userManager, User user)
        {
            return Task.FromResult(user.IsTwoFactorEnabled && user.UserName != null);
        }
    }
}