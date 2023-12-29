using AutoMapper;
using TaskCircle.GroupManagerApi.DTOs;
using TaskCircle.GroupManagerApi.Infrastructure.Repositories.Interfaces;
using TaskCircle.GroupManagerApi.Infrastructure.Services.Interfaces;
using Group = TaskCircle.GroupManagerApi.Model.Group;

namespace TaskCircle.GroupManagerApi.Infrastructure.Services;

public class GroupService : IGroupService
{
    private readonly IGroupRepository? _groupRepository;
    private readonly IMapper? _mapper;

    public GroupService(IGroupRepository? groupRepository, IMapper? mapper)
    {
        _groupRepository = groupRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Creat a group
    /// </summary>
    /// <param name="addGroupDto">This is a DTO to create a group</param>
    public async Task CreateGroup(AddGroupDTO addGroupDto)
    {
        var groupEntity = _mapper?.Map<Group>(addGroupDto);

        await _groupRepository.Create(groupEntity);
    }

    /// <summary>
    /// Add user to a group
    /// </summary>
    /// <param name="userId"> The id of the user</param>
    /// <param name="groupId">The group</param>
    /// <returns>Bool</returns>
    public async Task<bool> AddUerToGroup(int userId, int groupId)
    {
        var response = await _groupRepository.AddUserToGroup(userId, groupId);

        return response;
    }

    /// <summary>
    /// Get Group By Id
    /// </summary>
    /// <param name="id">Group Id</param>
    /// <returns>Group</returns>
    public async Task<GroupDTO> GetGroupById(int id)
    {
        var group = await _groupRepository.GetById(id);

        var groupEntity = _mapper?.Map<GroupDTO>(group);

        if (groupEntity != null)
        {
            groupEntity.GroupName = group.Name;
            groupEntity.GroupId = group.IdGroup;
            groupEntity.AdminId = group.IdAdmin;
        }

        return groupEntity;
    }

    /// <summary>
    /// Get Group by name
    /// </summary>
    /// <param name="name">Name of the group</param>
    /// <returns>GroupDTO</returns>
    public async Task<GroupDTO> GetGroupByName(string name)
    {
        var group = await _groupRepository.GetGroupByName(name);

        var groupEntity = _mapper?.Map<GroupDTO>(group);

        if (groupEntity != null)
        {
            groupEntity.GroupName = group.Name;
            groupEntity.GroupId = group.IdGroup;
            groupEntity.AdminId = group.IdAdmin;
        }

        return groupEntity;
    }

    /// <summary>
    /// Get user inside a group
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="groupId"></param>
    /// <returns>UserInGroupDTO</returns>
    public async Task<UserInGroupDTO> GetUserInGroup(int userId, int groupId)
    {
        var userInGroup = await _groupRepository.GetUserInGroup(userId, groupId);

        return userInGroup;
    }

    /// <summary>
    /// Removes all groups from an admin
    /// </summary>
    /// <param name="adminId"></param>
    /// <returns>bool</returns>
    public async Task<bool> RemoveAllAdminGroups(int adminId)
    {
        var status = await _groupRepository.RemoveAllAdminGroups(adminId);
        return status;
    }

    /// <summary>
    /// Remove a user from group
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="groupId"></param>
    /// <returns>bool</returns>
    public async Task<bool> RemoveUerFromGroup(int userId, int groupId)
    {
        var status = await _groupRepository.RemoveUserFromGroup(userId, groupId);

        return status;
    }

    /// <summary>
    /// Remove a group By Id
    /// </summary>
    /// <param name="groupId"></param>
    /// <returns>Group</returns>
    public async Task<Group> RemoveGroupById(int groupId)
    {
        var group = await _groupRepository.Delete(groupId);

        return group;
    }

    /// <summary>
    /// Get All users from a group
    /// </summary>
    /// <param name="groupId"></param>
    /// <returns>List of user Id</returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<IEnumerable<int>> GetAllUsersFromGroupId(int groupId)
    {
        var users = await _groupRepository.GetAllUsersFromGroup(groupId);

        return users;
    }

    /// <summary>
    /// Get all the groups in which a specific user is included.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns>Group</returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<IEnumerable<Group>> GetAllGroupsFromUserId(int userId)
    {
        var groups = await _groupRepository.GetAllGroupsFromUser(userId);

        return groups;
    }

    /// <summary>
    ///  Get all the groups in which a specific group is owner.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns>List of groups</returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<IEnumerable<Group>> GetAllGroupsFromAdminId(int userId)
    {
        var groups = await _groupRepository.GetAllGroupsFromAdminId(userId);

        return groups;
    }
}