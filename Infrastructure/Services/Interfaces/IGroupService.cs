using System.Text.RegularExpressions;
using TaskCircle.GroupManagerApi.DTOs;
using Group = TaskCircle.GroupManagerApi.Model.Group;

namespace TaskCircle.GroupManagerApi.Infrastructure.Services.Interfaces
{
    public interface IGroupService
    {
        Task CreateGroup(AddGroupDTO groupDto);
        Task<GroupDTO> GetGroupByName(string name);
        Task<GroupDTO> GetGroupById(int id);
        Task<bool> AddUerToGroup(int userId, int groupId);
        Task<bool> RemoveUerFromGroup(int userId, int groupId);
        Task<bool> RemoveAllAdminGroups(int adminId);
        Task<UserInGroupDTO> GetUserInGroup(int userId, int groupId);
        Task<Group> RemoveGroupById(int groupId);
        Task<IEnumerable<int>> GetAllUsersFromGroupId(int groupId);
        Task<IEnumerable<Group>> GetAllGroupsFromUserId(int userId);
        Task<IEnumerable<Group>> GetAllGroupsFromAdminId(int userId);
    }
}
