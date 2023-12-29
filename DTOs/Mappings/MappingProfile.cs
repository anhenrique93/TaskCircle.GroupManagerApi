using AutoMapper;
using TaskCircle.GroupManagerApi.DTOs;
using TaskCircle.GroupManagerApi.Model;

namespace TaskCircle.UserManagerApi.DTOs.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile() 
    {
        CreateMap<Group, GroupDTO>().ReverseMap();
        CreateMap<Group, AddGroupDTO>().ReverseMap();
        CreateMap<Group, GetGroupDTO>().ReverseMap();
    }
}
