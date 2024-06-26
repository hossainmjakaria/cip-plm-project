﻿using PLM.Library.Infrustuctures;
using PLM.Library.Utility;

namespace PLM.Library.Services;

public interface IParkingSeederService
{
    public Task SeedAsync();
    public bool HasSeeded();
}

public class ParkingSeederService(IParkingDataSeederRepository repository, AppSettings appSettings) : IParkingSeederService
{
    private bool seeded = false;

    public async Task SeedAsync()
    {
        if (!HasSeeded())
        {
            this.seeded = await repository.SeedAsync(appSettings.TotalSpots);
        }
    }

    public bool HasSeeded() => seeded;
}