using System.ComponentModel.DataAnnotations.Schema;

namespace MVC_app_main.Models
{
#pragma warning disable
    [Table("photos")]
    public class Photos
    {
        public int Id { get; set; }
        public int? Sol { get; set; }
        public string? Camera { get; set; }
        public string? Img_src { get; set; }
        public string? Earth_date { get; set; }
        public string? Rover { get; set; }
    }
#pragma warning restore
}