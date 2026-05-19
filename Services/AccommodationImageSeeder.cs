public class AccommodationImageSeeder
{
    private readonly AccommodationService _accommodationService;
    private readonly ILogger<AccommodationImageSeeder> _logger;

    private static readonly string Gallery2 = "/images/accommodations/gallery-2.jpg";
    private static readonly string Gallery3 = "/images/accommodations/gallery-3.jpg";

    public AccommodationImageSeeder(
        AccommodationService accommodationService,
        ILogger<AccommodationImageSeeder> logger)
    {
        _accommodationService = accommodationService;
        _logger = logger;
    }

    public void SeedIfNeeded()
    {
        var accommodations = _accommodationService.GetAll();
        var defaultSets = new[]
        {
            new[] { "/images/accommodations/vila-zlatibor.jpg", Gallery2, Gallery3 },
            new[] { "/images/accommodations/apartman-beograd.jpg", Gallery2 },
            new[] { "/images/accommodations/hotel-kopaonik.jpg", Gallery3, Gallery2 },
            new[] { "/images/accommodations/studio-novi-sad.jpg", Gallery2 },
            new[] { "/images/accommodations/kuca-nis.jpg", Gallery3 },
            new[] { "/images/accommodations/vila-ada.jpg", Gallery2, Gallery3 },
            new[] { "/images/accommodations/penthouse.jpg", Gallery3 },
        };

        var index = 0;
        foreach (var accommodation in accommodations)
        {
            if (!NeedsImages(accommodation))
                continue;

            var images = ResolveImages(accommodation) ?? defaultSets[index % defaultSets.Length];
            index++;

            accommodation.ImagePaths = images.ToList();
            _accommodationService.Update(accommodation.Id, accommodation);
            _logger.LogInformation("Dodeljene slike smestaju: {Name}", accommodation.Name);
        }
    }

    private static string[]? ResolveImages(Accommodation accommodation)
    {
        var text = $"{accommodation.Name} {accommodation.Location}".ToLowerInvariant();

        if (text.Contains("zlatibor") || text.Contains("planin") || text.Contains("kopaonik"))
            return new[] { "/images/accommodations/vila-zlatibor.jpg", Gallery2, Gallery3 };

        if (text.Contains("beograd") || text.Contains("novi sad") || text.Contains("grad"))
            return new[] { "/images/accommodations/apartman-beograd.jpg", Gallery2 };

        if (text.Contains("hotel") || text.Contains("kopaonik"))
            return new[] { "/images/accommodations/hotel-kopaonik.jpg", Gallery3, Gallery2 };

        if (text.Contains("studio") || text.Contains("apartman"))
            return new[] { "/images/accommodations/studio-novi-sad.jpg", Gallery2 };

        if (text.Contains("niš") || text.Contains("nis") || text.Contains("kuća") || text.Contains("kuca"))
            return new[] { "/images/accommodations/kuca-nis.jpg", Gallery3 };

        if (text.Contains("ada") || text.Contains("vila"))
            return new[] { "/images/accommodations/vila-ada.jpg", Gallery2, Gallery3 };

        if (text.Contains("penthouse") || text.Contains("luks"))
            return new[] { "/images/accommodations/penthouse.jpg", Gallery3 };

        return null;
    }

    private static bool NeedsImages(Accommodation accommodation)
    {
        if (accommodation.ImagePaths is not { Count: > 0 })
            return true;

        return accommodation.ImagePaths.All(path =>
            path.StartsWith("/uploads/", StringComparison.OrdinalIgnoreCase));
    }
}
