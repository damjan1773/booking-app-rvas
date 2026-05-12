using MongoDB.Driver;
using Microsoft.Extensions.Options;
using BookingApp.Models;

public class AccommodationService {
    private readonly IMongoCollection<Accommodation> _accommodations;

    public AccommodationService(IMongoClient client, IOptions<MongoDbSettings> settings) {
        var db = client.GetDatabase(settings.Value.DatabaseName);
        _accommodations = db.GetCollection<Accommodation>("Accommodations");
    }

    public List<Accommodation> GetAll() =>
        _accommodations.Find(_ => true).ToList();

    public Accommodation GetById(string id) =>
        _accommodations.Find(a => a.Id == id).FirstOrDefault();

    public void Create(Accommodation accommodation) =>
        _accommodations.InsertOne(accommodation);

    public void Update(string id, Accommodation accommodation) =>
        _accommodations.ReplaceOne(a => a.Id == id, accommodation);

    public void Delete(string id) =>
        _accommodations.DeleteOne(a => a.Id == id);

    public List<Accommodation> GetByOwner(string ownerId) =>
        _accommodations.Find(a => a.OwnerId == ownerId).ToList();
}