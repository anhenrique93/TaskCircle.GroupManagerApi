using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TaskCircle.GroupManagerApi.DTOs
{
    public class GroupDTO
    {
        public int GroupId { get; set; }
        public int AdminId { get; set; }
        public string? GroupName { get; set; }
    }
}
