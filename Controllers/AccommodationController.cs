using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[Authorize]
public class AccommodationController : Controller {
    private readonly AccommodationService _service;

    public AccommodationController(AccommodationService service) {
        _service = service;
    }

    [AllowAnonymous]
    public IActionResult Index() {
        var accommodations = _service.GetAll();
        return View(accommodations);
    }

    [AllowAnonymous]
    public IActionResult Details(string id) {
        var accommodation = _service.GetById(id);
        if (accommodation == null) return NotFound();
        return View(accommodation);
    }

    [Authorize(Roles = "Owner")]
    public IActionResult Create() => View();

    [HttpPost]
    [Authorize(Roles = "Owner")]
    public IActionResult Create(Accommodation accommodation, IFormFile? image) {
        accommodation.OwnerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (image != null) {
            var path = Path.Combine("wwwroot/uploads", image.FileName);
            using var stream = new FileStream(path, FileMode.Create);
            image.CopyTo(stream);
            accommodation.ImagePaths.Add("/uploads/" + image.FileName);
        }

        _service.Create(accommodation);
        return RedirectToAction("Index");
    }

    [Authorize(Roles = "Owner")]
    public IActionResult Edit(string id) {
        var accommodation = _service.GetById(id);
        var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (accommodation.OwnerId != ownerId) return Forbid();
        return View(accommodation);
    }

    [HttpPost]
    [Authorize(Roles = "Owner")]
    public IActionResult Edit(string id, Accommodation updated) {
        var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var existing = _service.GetById(id);
        if (existing.OwnerId != ownerId) return Forbid();
        updated.OwnerId = ownerId;
        _service.Update(id, updated);
        return RedirectToAction("Index");
    }

    [HttpPost]
    [Authorize(Roles = "Owner")]
    public IActionResult Delete(string id) {
        var ownerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var existing = _service.GetById(id);
        if (existing.OwnerId != ownerId) return Forbid();
        _service.Delete(id);
        return RedirectToAction("Index");
    }
}