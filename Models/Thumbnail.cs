using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace MVC_app_main.Models
{
#pragma warning disable
	public partial class Thumbnail
    {
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string _id { get; set; }
		public int Id { get; set; }
		public string Title { get; set; }
		public string Url { get; set; }
		public string ImageUrl { get; set; }
		public string NewsSite { get; set; }
		public string Summary { get; set; }
		public DateTime PublishedAt { get; set; }
		public DateTime UpdatedAt { get; set; }
	}
#pragma warning restore
}
