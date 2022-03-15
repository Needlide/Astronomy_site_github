using System.ComponentModel.DataAnnotations.Schema;

namespace MVC_app_main.Models
{
    [Table("[dbo].APOD")]
    public class APOD
    {
        public string? copyright { get; set; }
        public string? date { get; set; }
        public string? explanation { get; set; }
        public string? hdurl { get; set; }
        public string? media_type { get; set; }
        public string? service_version { get; set; }
        public string? title { get; set; }
        public string? url { get; set; }
    }
}
