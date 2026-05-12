using BookingApp.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BookingApp.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;

        public UserService(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.DatabaseName);
            _users = database.GetCollection<User>("Users");
        }

        public User? GetByEmail(string email) =>
            _users.Find(u => u.Email == email).FirstOrDefault();

        public void Create(User user) =>
            _users.InsertOne(user);

        public bool EmailExists(string email) =>
            _users.Find(u => u.Email == email).Any();
    }
}