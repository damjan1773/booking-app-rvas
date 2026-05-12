using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Accommodation {
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public string OwnerId { get; set; }
    public string Name { get; set; }
    public string Location { get; set; }
    public string Description { get; set; }
    public decimal PricePerNight { get; set; }
    public List<string> Amenities { get; set; } = new();
    public List<string> ImagePaths { get; set; } = new();
}