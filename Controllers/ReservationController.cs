using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[Authorize]
public class ReservationController : Controller
{
    private readonly ReservationService _reservationService;
    private readonly AccommodationService _accommodationService;

    public ReservationController(ReservationService reservationService, AccommodationService accommodationService)
    {
        _reservationService = reservationService;
        _accommodationService = accommodationService;
    }

    [HttpGet]
    [Authorize(Roles = "Guest")]
    public IActionResult Create(string accommodationId)
    {
        var accommodation = _accommodationService.GetById(accommodationId);
        if (accommodation == null) return NotFound();

        ViewBag.Accommodation = accommodation;
        return View(new Reservation { AccommodationId = accommodationId });
    }

    [HttpPost]
    [Authorize(Roles = "Guest")]
    public IActionResult Create(Reservation reservation)
    {
        var accommodation = _accommodationService.GetById(reservation.AccommodationId);
        if (accommodation == null) return NotFound();

        if (reservation.CheckIn >= reservation.CheckOut)
        {
            ModelState.AddModelError("", "Datum odlaska mora biti posle datuma dolaska.");
            ViewBag.Accommodation = accommodation;
            return View(reservation);
        }

        if (reservation.CheckIn < DateTime.Today)
        {
            ModelState.AddModelError("", "Datum dolaska ne može biti u prošlosti.");
            ViewBag.Accommodation = accommodation;
            return View(reservation);
        }

        reservation.GuestId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        reservation.GuestName = User.FindFirstValue(ClaimTypes.Name);

        var nights = (reservation.CheckOut - reservation.CheckIn).Days;
        reservation.TotalPrice = nights * accommodation.PricePerNight;

        try
        {
            _reservationService.Create(reservation);
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError("", ex.Message);
            ViewBag.Accommodation = accommodation;
            return View(reservation);
        }

        return RedirectToAction("MyReservations");
    }
    [HttpGet]
    [Authorize(Roles = "Guest")]
    public IActionResult MyReservations()
    {
        var guestId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var reservations = _reservationService.GetByGuest(guestId);

        var accommodations = reservations
            .Select(r => _accommodationService.GetById(r.AccommodationId))
            .ToDictionary(a => a.Id, a => a);

        ViewBag.Accommodations = accommodations;
        return View(reservations);
    }

    [HttpGet]
    [Authorize(Roles = "Owner")]
    public IActionResult AccommodationReservations(string accommodationId)
    {
        var accommodation = _accommodationService.GetById(accommodationId);
        if (accommodation == null) return NotFound();

        var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (accommodation.OwnerId != ownerId) return Forbid();

        var reservations = _reservationService.GetByAccommodation(accommodationId);

        ViewBag.Accommodation = accommodation;
        return View(reservations);
    }
}