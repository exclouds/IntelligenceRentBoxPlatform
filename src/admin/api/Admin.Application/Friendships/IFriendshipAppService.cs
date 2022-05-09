using System.Threading.Tasks;
using Abp.Application.Services;
using Magicodes.Admin.Friendships.Dto;

namespace Magicodes.Admin.Friendships
{
    public interface IFriendshipAppService : IApplicationService
    {
        

        Task BlockUser(BlockUserInput input);

        Task UnblockUser(UnblockUserInput input);

        Task AcceptFriendshipRequest(AcceptFriendshipRequestInput input);
    }
}
