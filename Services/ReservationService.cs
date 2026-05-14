using MongoDB.Driver;
using Microsoft.Extensions.Options;
using BookingApp.Models;

public class ReservationService
{
    private readonly IMongoCollection<Reservation> _reservations;

    public ReservationService(IMongoClient client, IOptions<MongoDbSettings> settings)
    {
        var db = client.GetDatabase(settings.Value.DatabaseName);
        _reservations = db.GetCollection<Reservation>("Reservations");
    }

    public List<Reservation> GetAll() =>
        _reservations.Find(_ => true).ToList();

    public Reservation GetById(string id) =>
        _reservations.Find(r => r.Id == id).FirstOrDefault();

    public List<Reservation> GetByGuest(string guestId) =>
        _reservations.Find(r => r.GuestId == guestId)
                     .SortByDescending(r => r.CreatedAt)
                     .ToList();

    public List<Reservation> GetByAccommodation(string accommodationId) =>
        _reservations.Find(r => r.AccommodationId == accommodationId)
                     .SortBy(r => r.CheckIn)
                     .ToList();

    public void Delete(string id) =>
        _reservations.DeleteOne(r => r.Id == id);

    public bool IsAvailable(string accommodationId, DateTime checkIn, DateTime checkOut)
    {
        var overlap = _reservations.Find(r =>
            r.AccommodationId == accommodationId &&
            r.CheckIn < checkOut &&
            r.CheckOut > checkIn
        ).Any();

        return !overlap;
    }
}
