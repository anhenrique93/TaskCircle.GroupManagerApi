using Microsoft.Extensions.Options;
using Npgsql;
using TaskCircle.GroupManagerApi.DTOs;
using TaskCircle.GroupManagerApi.Infrastructure.Repositories.Interfaces;
using TaskCircle.UserManagerApi.Infrastructure.Setting;
using Group = TaskCircle.GroupManagerApi.Model.Group;


namespace TaskCircle.GroupManagerApi.Infrastructure.Repositories;

public class GroupRepository : IGroupRepository
{

    private readonly ConnectionSetting? _connection;

    public GroupRepository(IOptions<ConnectionSetting> connection)
    {
        _connection = connection.Value;
    }

    // Create Group
    public async Task<Group> Create(Group Entity)
    {
        using (var connect = new NpgsqlConnection(_connection?.PostgreSQLString))
        {
            await connect.OpenAsync();
            using (var cmd = new NpgsqlCommand("CALL AddGroup(@name, @adminId)", connect))
            {
                cmd.Parameters.AddWithValue("name", Entity.Name);
                cmd.Parameters.AddWithValue("adminId", Entity.IdAdmin);

                await cmd.ExecuteNonQueryAsync();
            }
        }

        return Entity;
    }


    // Add User to Group
    public async Task<bool> AddUserToGroup(int userId, int groupId)
    {
        int rowsAffected = 0;

        using (var connect = new NpgsqlConnection(_connection?.PostgreSQLString))
        {
            await connect.OpenAsync();
            using (var cmd = new NpgsqlCommand("SELECT * FROM AddUserToGroup(@userId, @groupId)", connect))
            {
                cmd.Parameters.AddWithValue("userId", userId);
                cmd.Parameters.AddWithValue("groupId", groupId);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        rowsAffected = reader.GetInt32(0);
                    }
                }
            }
        }

        return rowsAffected > 0;
    }


    // Get group by id
    public async Task<Group> GetById(int id)
    {
        Group group = null;

        using (var connect = new NpgsqlConnection(_connection?.PostgreSQLString))
        {
            await connect.OpenAsync();
            using (var cmd = new NpgsqlCommand("SELECT * FROM GetGroupById(@id)", connect))
            {
                cmd.Parameters.AddWithValue("id", id);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        group = new Group
                        {
                            IdGroup = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            IdAdmin = reader.GetInt32(2)
                        };
                    }
                }
            }
        }

        return group;
    }

    // Get Group by name
    public async Task<Group> GetGroupByName(string name)
    {
        Group group = null;

        using (var connect = new NpgsqlConnection(_connection?.PostgreSQLString))
        {
            await connect.OpenAsync();
            using (var cmd = new NpgsqlCommand("SELECT * FROM GetGroupByName(@name)", connect))
            {
                cmd.Parameters.AddWithValue("name", name);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        group = new Group
                        {
                            IdGroup = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            IdAdmin = reader.GetInt32(2)
                        };
                    }
                }
            }
        }

        return group;
    }

    // Get User inside a group
    public async Task<UserInGroupDTO> GetUserInGroup(int userId, int groupId)
    {
        UserInGroupDTO userInGroup = null;

        using (var connect = new NpgsqlConnection(_connection?.PostgreSQLString))
        {
            await connect.OpenAsync();
            using (var cmd = new NpgsqlCommand("SELECT * FROM GetUserInGroupById(@userId, @groupId)", connect))
            {
                cmd.Parameters.AddWithValue("userId", userId);
                cmd.Parameters.AddWithValue("groupId", groupId);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        userInGroup = new UserInGroupDTO
                        {
                            UserId = reader.GetInt32(0),
                            GroupId = reader.GetInt32(1)
                        };
                    }
                }
            }
        }

        return userInGroup;
    }

    // Remove user inside a group
    public async Task<bool> RemoveUserFromGroup(int userId, int groupId)
    {
        int rowsAffected = 0;

        using (var connect = new NpgsqlConnection(_connection?.PostgreSQLString))
        {
            await connect.OpenAsync();
            using (var cmd = new NpgsqlCommand("SELECT RemoveUserFromGroup(@userId, @groupId)", connect))
            {
                cmd.Parameters.AddWithValue("userId", userId);
                cmd.Parameters.AddWithValue("groupId", groupId);

                rowsAffected = Convert.ToInt32(await cmd.ExecuteScalarAsync());
            }
        }

        return rowsAffected > 0;
    }

    // Delete a group
    public async Task<Group> Delete(int id)
    {
        Group group = await GetById(id);
        
        using (var connect = new NpgsqlConnection(_connection?.PostgreSQLString))
        {
            await connect.OpenAsync();
            using (var cmd = new NpgsqlCommand("CALL DeleteGroupById(@id)", connect))
            {
                cmd.Parameters.AddWithValue("id", id);
                await cmd.ExecuteNonQueryAsync();
            }
        }

        return group;
    }

    public Task<IEnumerable<Group>> GetAll()
    {
        throw new NotImplementedException();
    }

    // Removes all groups from an admin
    public async Task<bool> RemoveAllAdminGroups(int adminId)
    {
        using (var connect = new NpgsqlConnection(_connection?.PostgreSQLString))
        {
            await connect.OpenAsync();
            using (var cmd = new NpgsqlCommand("CALL RemoveGroupsAndUsersByAdmin(@adminId)", connect))
            {
                cmd.Parameters.AddWithValue("adminId", adminId);
                await cmd.ExecuteNonQueryAsync();
            }
        }

        return true;
    }

    public Task<Group> Update(Group Entity)
    {
        throw new NotImplementedException();
    }

    // Get all users from group
    public async Task<IEnumerable<int>> GetAllUsersFromGroup(int groupId)
    {
        List<int> userIds = new List<int>();

        using (var connect = new NpgsqlConnection(_connection?.PostgreSQLString))
        {
            await connect.OpenAsync();
            using (var cmd = new NpgsqlCommand("SELECT * FROM GetAllUsersFromGroup(@groupId)", connect))
            {
                cmd.Parameters.AddWithValue("groupId", groupId);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        userIds.Add(reader.GetInt32(0));
                    }
                }
            }
        }

        return userIds;
    }

    // Get all groups a user is a part of
    public async Task<IEnumerable<Group>> GetAllGroupsFromUser(int userId)
    {
        List<Group> groups = new List<Group>();

        using (var connect = new NpgsqlConnection(_connection?.PostgreSQLString))
        {
            await connect.OpenAsync();
            using (var cmd = new NpgsqlCommand("SELECT * FROM GetAllGroupsFromUserId(@userId)", connect))
            {
                cmd.Parameters.AddWithValue("userId", userId);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var group = new Group
                        {
                            IdGroup = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            IdAdmin = reader.GetInt32(2)
                        };
                        groups.Add(group);
                    }
                }
            }
        }

        return groups;
    }

    // Get all groups a admin is a owner
    public async Task<IEnumerable<Group>> GetAllGroupsFromAdminId(int adminId)
    {
        List<Group> groups = new List<Group>();

        using (var connect = new NpgsqlConnection(_connection?.PostgreSQLString))
        {
            await connect.OpenAsync();
            using (var cmd = new NpgsqlCommand("SELECT * FROM GetAllGroupsFromAdminId(@adminId)", connect))
            {
                cmd.Parameters.AddWithValue("adminId", adminId);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var group = new Group
                        {
                            IdGroup = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            IdAdmin = adminId
                        };
                        groups.Add(group);
                    }
                }
            }
        }

        return groups;
    }
}