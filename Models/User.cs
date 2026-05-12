using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BookingApp.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;

        // "Owner" ili "Guest"
        public string Role { get; set; } = null!;
    }
}