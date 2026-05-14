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

    public List<Reservation> GetUpcomingByAccommodation(string accommodationId) =>
        _reservations.Find(r =>
            r.AccommodationId == accommodationId &&
            r.CheckOut >= DateTime.Today
        ).SortBy(r => r.CheckIn).ToList();

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
    public void Create(Reservation reservation)
    {
        if (!IsAvailable(reservation.AccommodationId, reservation.CheckIn, reservation.CheckOut))
            throw new InvalidOperationException("Smeštaj nije dostupan u izabranom terminu.");

        _reservations.InsertOne(reservation);
    }

    public void DeleteByAccommodation(string accommodationId) =>
        _reservations.DeleteMany(r => r.AccommodationId == accommodationId);
}
