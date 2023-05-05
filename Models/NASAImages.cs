using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MVC_app_main.Models
{
#pragma warning disable
	public class ImagesGallery
    {
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string _id { get; set; }
		public int Id { get; set; }
		public string Center { get; set; }
		public string Title { get; set; }
		public string NASAID { get; set; }
		public string MediaType { get; set; }
		public /*List<string>*/ BsonString Keywords { get; set; }
		public DateTime DateCreated { get; set; }
		public string SecondDescription { get; set; }
		public string SecondaryCreator { get; set; }
		public string Description { get; set; }
		public string Href { get; set; }
	}
#pragma warning restore
}
