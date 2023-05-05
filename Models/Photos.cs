using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace MVC_app_main.Models
{
#pragma warning disable
    public class Photos
    {
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string _id { get; set; }
		public int Id { get; set; }
		public int Sol { get; set; }
		public object Camera { get; set; }
		public string ImgSrc { get; set; }
		public string EarthDate { get; set; }
		public object Rover { get; set; }
	}
#pragma warning restore
}