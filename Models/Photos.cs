using System.ComponentModel.DataAnnotations.Schema;

namespace MVC_app_main.Models
{
#pragma warning disable
    [Table("photos")]
    public class Photos
    {
        public int Id { get; set; }
        public int? Sol { get; set; }
        public object? Camera { get; set; }
        public string? ImgSrc { get; set; }
        public string? EarthDate { get; set; }
        public object? Rover { get; set; }
    }
#pragma warning restore
}