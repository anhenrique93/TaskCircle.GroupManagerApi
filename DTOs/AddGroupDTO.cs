using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TaskCircle.GroupManagerApi.DTOs
{
    public class AddGroupDTO
    {
        [JsonIgnore]
        public int IdGroup { get; set; }
        [JsonIgnore]
        public int IdAdmin { get; set; }
        [Required]
        [MinLength(5)]
        public string? Name { get; set; }
    }
}
