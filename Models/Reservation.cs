using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Reservation
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public string AccommodationId { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    public string GuestId { get; set; }

    public string GuestName { get; set; }

    public DateTime CheckIn { get; set; }

    public DateTime CheckOut { get; set; }

    public decimal TotalPrice { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}