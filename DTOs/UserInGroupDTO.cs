using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TaskCircle.GroupManagerApi.DTOs
{
    public class UserInGroupDTO
    {
        public int GroupId { get; set; }
        public int UserId { get; set; }
    }
}
