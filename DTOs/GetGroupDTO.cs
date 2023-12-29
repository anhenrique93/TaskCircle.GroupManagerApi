using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TaskCircle.GroupManagerApi.DTOs
{
    public class GetGroupDTO
    {
        public string? Name { get; set; }
    }
}
