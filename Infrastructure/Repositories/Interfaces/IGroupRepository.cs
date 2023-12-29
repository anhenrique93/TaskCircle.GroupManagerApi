using Group = TaskCircle.GroupManagerApi.Model.Group;

using System.Text.RegularExpressions;
using TaskCircle.GroupManagerApi.DTOs;

namespace TaskCircle.GroupManagerApi.Infrastructure.Repositories.Interfaces;

public interface IGroupRepository : IRepository<Group>
{
    Task<bool> AddUserToGroup(int userId, int groupId);
    Task<bool> RemoveUserFromGroup(int userId, int groupId);

    Task<bool> RemoveAllAdminGroups(int adminId);

    Task<Group> GetGroupByName(string groupName);
    
    Task<UserInGroupDTO> GetUserInGroup(int userId, int groupId);
    Task<IEnumerable<int>> GetAllUsersFromGroup(int groupId);
    Task<IEnumerable<Group>> GetAllGroupsFromUser(int userId);
    Task<IEnumerable<Group>> GetAllGroupsFromAdminId(int adminId);
}
