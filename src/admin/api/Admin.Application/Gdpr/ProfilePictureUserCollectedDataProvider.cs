using System.Collections.Generic;
using System.Threading.Tasks;
using Abp;
using Abp.AspNetZeroCore.Net;
using Abp.Dependency;
using Magicodes.Admin.Authorization.Users;
using Magicodes.Admin.Dto;
using Magicodes.Admin.Storage;

namespace Magicodes.Admin.Gdpr
{
    public class ProfilePictureUserCollectedDataProvider : IUserCollectedDataProvider, ITransientDependency
    {
        private readonly UserManager _userManager;
        private readonly IBinaryObjectManager _binaryObjectManager;
        private readonly ITempFileCacheManager _tempFileCacheManager;

        public ProfilePictureUserCollectedDataProvider(
            UserManager userManager,
            IBinaryObjectManager binaryObjectManager,
            ITempFileCacheManager tempFileCacheManager
        )
        {
            _userManager = userManager;
            _binaryObjectManager = binaryObjectManager;
            _tempFileCacheManager = tempFileCacheManager;
        }

        public async Task<List<FileDto>> GetFiles(UserIdentifier user)
        {

            var file = new FileDto("ProfilePicture.png", MimeTypeNames.ImagePng);

            return new List<FileDto> {file};
        }
    }
}