using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using TaskCircle.GroupManagerApi.DTOs;
using TaskCircle.GroupManagerApi.Infrastructure.Services.Interfaces;
using TaskCircle.GroupManagerApi.Model;

namespace TaskCircle.GroupManagerApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[EnableCors("Group")]
public class GroupController : Controller
{

    private readonly IGroupService? _groupService;

    public GroupController(IGroupService? groupService)
    {
        _groupService = groupService;
    }

    /// <summary>
    /// This Add a Group
    /// </summary>
    /// <response code="200">Ok</response>
    /// <response code="404">This name group already taken!</response>
    /// <response code="401">Unauthorized</response>
    /// <param name="addGroupDto"></param>
    /// <returns>Group</returns>
    [HttpPost, Authorize]
    public async Task<ActionResult> Post([FromBody] AddGroupDTO addGroupDto)
    {
        //Get Logged User
        var user = GetAuthenticateUser();

        if (user.Id is null)
        {
            return Unauthorized();
        }
        else
        {
            addGroupDto.IdAdmin = (int)user.Id;
        }

        if (addGroupDto is null) return BadRequest("Invalid Data");

        var group = await _groupService.GetGroupByName(addGroupDto.Name);

        if (group != null)
            return BadRequest("This name group already taken!");

        await _groupService.CreateGroup(addGroupDto);
        
        return Ok(addGroupDto);
    }

    /// <summary>
    /// This get a group by id
    /// </summary>
    /// <response code="200">Ok</response>
    /// <response code="404">Group not foud</response>
    /// <response code="401">Unauthorized</response>
    /// <param name="id">Id of the Group</param>
    /// <returns>GroupDTO</returns>
    [HttpGet("{id:int}"), Authorize]
    public async Task<ActionResult<GroupDTO>> GetGroupById(int id)
    {
        var groupDto = await _groupService.GetGroupById(id);

        if (groupDto is null) return NotFound("Group Not found!");

        return Ok(groupDto);
    }

    /// <summary>
    /// This get a group by name
    /// </summary>
    /// <response code="200">Ok</response>
    /// <response code="404">Group not foud</response>
    /// <response code="401">Unauthorized</response>
    /// <param name="name">name of the Group</param>
    /// <returns>GroupDTO</returns>
    [HttpGet, Authorize]
    public async Task<ActionResult<GroupDTO>> GetGroupByName(string name)
    {

        if (name is null) return BadRequest();

        var groupDto = await _groupService.GetGroupByName(name);

        if (groupDto is null) return NotFound("Group Not found!");

        return Ok(groupDto);
    }

    /// <summary>
    /// This return a user inside a group
    /// </summary>
    /// <response code="200">Ok</response>
    /// <response code="404">Group not found</response>
    /// <response code="404">User not found</response>
    /// <response code="401">Unauthorized</response>
    /// <param name="userId">User ID</param>
    /// <param name="groupId">Group ID</param>
    /// <returns>Ok</returns>
    [HttpGet("get-user/{groupId:int}/{userId:int}"), Authorize]
    public async Task<ActionResult<UserInGroupDTO>> GetUserInGroup(int groupId, int userId)
    {
        // Get logged User
        var user = GetAuthenticateUser();

        if (user.Id is null) return Unauthorized();

        var group = await _groupService.GetGroupById(groupId);

        if (group is null) return NotFound("Group not found");

        // Verifica se pertence ao grupo
        var loggedUserIsPartOfGroup = await _groupService.GetUserInGroup((int)user.Id, groupId);

        if (loggedUserIsPartOfGroup is null && (int)user.Id != group.AdminId) return Unauthorized();

        var userInGroup = await _groupService.GetUserInGroup(userId, groupId);

        if (userInGroup is null) return NotFound("User not fount");

        return Ok(userInGroup);
    }

    /// <summary>
    /// This return all users inside a group
    /// </summary>
    /// <response code="200">Ok</response>
    /// <response code="404">Group not found</response>
    /// <response code="404">Can't find any user</response>
    /// <param name="groupId">Group ID</param>
    /// <returns>Users Id</returns>
    [HttpGet("{groupId:int}/users"), Authorize]
    public async Task<ActionResult<IEnumerable<int>>> GetAllUsersFromGroupId(int groupId)
    {
        // Get logged User
        var user = GetAuthenticateUser();

        if (user.Id is null) return Unauthorized();

        var group = await _groupService.GetGroupById(groupId);

        if (group is null) return NotFound("Group Not Found");

        // Verifica se pertence ao grupo
        var loggedUserIsPartOfGroup = await _groupService.GetUserInGroup((int)user.Id, groupId);

        if (loggedUserIsPartOfGroup is null && (int)user.Id != group.AdminId) return Unauthorized();

        var users = await _groupService.GetAllUsersFromGroupId(groupId);

        if (users.Count() == 0) return NotFound("Can't find any user");

        return Ok(users);
    }

    /// <summary>
    /// This method returns all the groups in which a specific user is included.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns>List of groups</returns>
    /// <response code="404">Can't find any Group</response>
    [HttpGet("in-groups/{userId:int}"), Authorize]
    public async Task<ActionResult<IEnumerable<Group>>> GetAllGroupsFromUserId(int userId)
    {
        // Get logged User
        var user = GetAuthenticateUser();

        if (user.Id is null) return Unauthorized();

        var groups = await _groupService.GetAllGroupsFromUserId(userId);

        if (groups.Count() == 0) return NotFound("Can't find any Group");

        return Ok(groups);
    }

    /// <summary>
    /// This method returns all the groups in which a specific user is owner.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns>List of groups</returns>
    /// <response code="404">Can't find any Group</response>
    [HttpGet("my-groups/{userId:int}"), Authorize]
    public async Task<ActionResult<IEnumerable<Group>>> GetAllGroupsFromAdminId(int userId)
    {
        // Get logged User
        var user = GetAuthenticateUser();

        if (user.Id is null) return Unauthorized();

        var groups = await _groupService.GetAllGroupsFromAdminId(userId);

        if (groups.Count() == 0) return NotFound("Can't find any Group");

        return Ok(groups);
    }

    /// <summary>
    /// This add a user to a group
    /// </summary>
    /// <response code="200">Ok</response>
    /// <response code="404">Group not found</response>
    /// <response code="400">Something went wrong, please try again later</response>
    /// <response code="400">This user is already in the group</response>
    /// <param name="userId">User ID</param>
    /// <param name="groupId">Group ID</param>
    /// <returns>Ok</returns>
    [HttpPost("add-user/{groupId:int}/{userId:int}"), Authorize]
    public async Task<ActionResult> AddUserToGroup(int groupId, int userId)
    {
        // Get logged User
        var user = GetAuthenticateUser();

        if (user.Id is null) return Unauthorized();

        var group = await _groupService.GetGroupById(groupId);

        if (group is null)
            return NotFound("Group not foud");

        // Verifica se é admin do grupo  
        if ((int)user.Id != group.AdminId) return Unauthorized();

        var userInGroup = await _groupService.GetUserInGroup(userId, groupId);

        if (userInGroup != null)
            return BadRequest("This user is already in the group");

        var response = await _groupService.AddUerToGroup(userId, groupId);

        if (!response)
            return BadRequest("Something went wrong, please try again later");

        return Ok($"User {userId} added to group '{group.GroupName}'");
    }

    /// <summary>
    /// This Delete a user from group
    /// </summary>
    /// <response code="200">Ok</response>
    /// <response code="404">Group not found</response>
    /// <response code="400">Something went wrong, please try again later</response>
    /// <response code="400">User not found!</response>
    /// <param name="userId">User ID</param>
    /// <param name="groupId">Group ID</param>
    /// <returns>Ok</returns>
    [HttpDelete("remove-user/{groupId:int}/{userId:int}"), Authorize]
    public async Task<ActionResult> RemoveUserFromGroup(int groupId, int userId)
    {
        // Get logged User
        var user = GetAuthenticateUser();

        if (user.Id is null) return Unauthorized();

        var group = await _groupService.GetGroupById(groupId);

        if (group is null) return NotFound("Group Not Found!");

        // Verifica se é admin do grupo  
        if ((int)user.Id != group.AdminId) return Unauthorized();

        var userInGroup = await _groupService.GetUserInGroup(userId, groupId);

        if (userInGroup is null) return NotFound("User not found!");

        var statusRemove = await _groupService.RemoveUerFromGroup(userId, groupId);

        if (!statusRemove) return BadRequest("Something went wrong, please try again later");

        return Ok($"User {userInGroup.UserId} deleted!");
    }

    /// <summary>
    /// This Delete a group
    /// </summary>
    /// <response code="200">Ok</response>
    /// <response code="404">Group not found</response>
    /// <response code="400">Something went wrong, please try again later</response>
    /// <param name="groupId">Group ID</param>
    /// <returns>Ok</returns>
    [HttpDelete("groupId:int"), Authorize]
    public async Task<ActionResult<Group>> RemoveGroup(int groupId)
    {
        // Get logged User
        var user = GetAuthenticateUser();

        if (user.Id is null) return Unauthorized();

        var group = await _groupService.GetGroupById(groupId);

        if (group is null) return NotFound("Group Not Found");

        // Verifica se é admin do grupo  
        if ((int)user.Id != group.AdminId) return Unauthorized();

        var groupRemoved = await _groupService.RemoveGroupById(groupId);

        if (groupRemoved is null) return BadRequest("omething went wrong, please try again later");

        return Ok(groupRemoved);
    }

    /// <summary>
    /// This Delete all groups from an admin
    /// </summary>
    /// <response code="200">Ok</response>
    /// <response code="400">Something went wrong, please try again later</response>
    /// <param name="adminId">Admin ID</param>
    /// <returns>Ok</returns>
    [HttpDelete("remove-all"), Authorize]
    public async Task<ActionResult> RemoveAllAdminGroups()
    {
        // Get logged User
        var user = GetAuthenticateUser();

        if (user.Id is null) return Unauthorized();

        var statusRemoved = await _groupService.RemoveAllAdminGroups((int)user.Id);

        if (!statusRemoved) return BadRequest("Something went wrong, please try again later");

        return Ok($"Admin {user.Id} groups removed!");
    }
    
    private (int? Id, string? Email) GetAuthenticateUser()
    {
        //Verificar se é o usuario logado
        var accessToken = HttpContext.Request.Headers["Authorization"].ToString().Split(' ')[1];

        // Decodificar o token de acesso
        var handler = new JwtSecurityTokenHandler();
        var tokenS = handler.ReadToken(accessToken) as JwtSecurityToken;
        var userIdClaim = tokenS.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
        var emailClaim = tokenS.Claims.First(claim => claim.Type == ClaimTypes.Email).Value;

        if (userIdClaim != null)
        {
            var userId = int.Parse(userIdClaim);
            var email = emailClaim;

            return (userId, email);
        }

        return (null, null);

    }
}