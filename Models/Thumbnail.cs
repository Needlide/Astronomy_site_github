using System;
using System.Collections.Generic;

namespace MVC_app_main.Models
{
    public partial class Thumbnail
    {
        public int Id { get; set; }
        public string? Title { get; set; } = null!;
        public string? Url { get; set; } = null!;
        public string? ImageUrl { get; set; } = null!;
        public string? NewsSite { get; set; } = null!;
        public string? Summary { get; set; } = null!;
        public DateTime PublishedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
