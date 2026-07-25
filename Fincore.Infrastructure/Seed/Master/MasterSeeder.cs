using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using Fincore.Domain.Models;
using Fincore.Infrastructure.Data;
using Fincore.Infrastructure.Seed.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Fincore.Infrastructure.Seed.Master
{
    /// <summary>
    /// Phase 1 – Currency ➜ Country ➜ State ➜ City
    /// Also seeds the 4 fixed MasterType records.
    /// </summary>
    public static class MasterSeeder
    {
        public static async Task SeedAsync(AppDbContext db)
        {
            await SeedCurrenciesAsync(db);
            await SeedCountriesAsync(db);
            await SeedStatesAsync(db);
            await SeedCitiesAsync(db);
            await SeedMasterTypesAsync(db);
        }

        private static async Task SeedCurrenciesAsync(AppDbContext db)
        {
            if (await db.Currencies.AnyAsync()) return;

            var data = new List<Currency>
            {
                new() { CurrencyName = "US Dollar",       Symbol = "$"   },
                new() { CurrencyName = "Euro",            Symbol = "€"   },
                new() { CurrencyName = "Indian Rupee",    Symbol = "₹"   },
                new() { CurrencyName = "British Pound",   Symbol = "£"   },
                new() { CurrencyName = "Japanese Yen",    Symbol = "¥"   },
                new() { CurrencyName = "Australian Dol.", Symbol = "A$"  },
                new() { CurrencyName = "Canadian Dol.",   Symbol = "C$"  },
                new() { CurrencyName = "Swiss Franc",     Symbol = "CHF" },
                new() { CurrencyName = "Singapore Dol.",  Symbol = "S$"  },
                new() { CurrencyName = "UAE Dirham",      Symbol = "AED" }
            };
            db.Currencies.AddRange(data);
            await db.SaveChangesAsync();
        }

        private static async Task SeedCountriesAsync(AppDbContext db)
        {
            if (await db.Countries.AnyAsync()) return;

            var currencies = await db.Currencies.ToListAsync();
            int Cur(string name) => currencies.First(c => c.CurrencyName.StartsWith(name)).CurrencyId;

            var data = new List<Country>
            {
                new() { CountryCode = 1,   CountryName = "United States",   CurrencyId = Cur("US") },
                new() { CountryCode = 91,  CountryName = "India",           CurrencyId = Cur("Indian") },
                new() { CountryCode = 44,  CountryName = "United Kingdom",  CurrencyId = Cur("British") },
                new() { CountryCode = 81,  CountryName = "Japan",           CurrencyId = Cur("Japanese") },
                new() { CountryCode = 61,  CountryName = "Australia",       CurrencyId = Cur("Australian") },
                new() { CountryCode = 1,   CountryName = "Canada",          CurrencyId = Cur("Canadian") },
                new() { CountryCode = 49,  CountryName = "Germany",         CurrencyId = Cur("Euro") },
                new() { CountryCode = 33,  CountryName = "France",          CurrencyId = Cur("Euro") },
                new() { CountryCode = 41,  CountryName = "Switzerland",     CurrencyId = Cur("Swiss") },
                new() { CountryCode = 65,  CountryName = "Singapore",       CurrencyId = Cur("Singapore") },
                new() { CountryCode = 971, CountryName = "UAE",             CurrencyId = Cur("UAE") },
                new() { CountryCode = 39,  CountryName = "Italy",           CurrencyId = Cur("Euro") }

                //                new() { CountryCode = "1", CountryName = "United States", ... }
                //new() { CountryCode = "91", CountryName = "India", ... }
                //new() { CountryCode = "44", CountryName = "United Kingdom", ... }
                //new() { CountryCode = "81", CountryName = "Japan", ... }
                //new() { CountryCode = "61", CountryName = "Australia", ... }
                //new() { CountryCode = "1", CountryName = "Canada", ... }
                //new() { CountryCode = "49", CountryName = "Germany", ... }
                //new() { CountryCode = "33", CountryName = "France", ... }
                //new() { CountryCode = "41", CountryName = "Switzerland", ... }
                //new() { CountryCode = "65", CountryName = "Singapore", ... }
                //new() { CountryCode = "971", CountryName = "UAE", ... }
                //new() { CountryCode = "39", CountryName = "Italy", ... }
            };
            db.Countries.AddRange(data);
            await db.SaveChangesAsync();
        }

        private static async Task SeedStatesAsync(AppDbContext db)
        {
            if (await db.States.AnyAsync()) return;

            var countries = await db.Countries.ToListAsync();
            int C(string name) => countries.First(c => c.CountryName == name).CountryId;

            var data = new List<State>
            {
                new() { StateName = "California",     CountryId = C("United States") },
                new() { StateName = "New York",       CountryId = C("United States") },
                new() { StateName = "Texas",          CountryId = C("United States") },
                new() { StateName = "Florida",        CountryId = C("United States") },
                new() { StateName = "Maharashtra",    CountryId = C("India") },
                new() { StateName = "Karnataka",      CountryId = C("India") },
                new() { StateName = "Tamil Nadu",     CountryId = C("India") },
                new() { StateName = "Gujarat",        CountryId = C("India") },
                new() { StateName = "Delhi",          CountryId = C("India") },
                new() { StateName = "England",        CountryId = C("United Kingdom") },
                new() { StateName = "Scotland",       CountryId = C("United Kingdom") },
                new() { StateName = "Wales",          CountryId = C("United Kingdom") },
                new() { StateName = "Tokyo",          CountryId = C("Japan") },
                new() { StateName = "Osaka",          CountryId = C("Japan") },
                new() { StateName = "New South Wales",CountryId = C("Australia") },
                new() { StateName = "Victoria",       CountryId = C("Australia") },
                new() { StateName = "Ontario",        CountryId = C("Canada") },
                new() { StateName = "Quebec",         CountryId = C("Canada") },
                new() { StateName = "Bavaria",        CountryId = C("Germany") },
                new() { StateName = "Berlin",         CountryId = C("Germany") },
                new() { StateName = "Ile-de-France",  CountryId = C("France") },
                new() { StateName = "Provence",       CountryId = C("France") },
                new() { StateName = "Zurich",         CountryId = C("Switzerland") },
                new() { StateName = "Geneva",         CountryId = C("Switzerland") },
                new() { StateName = "Central",        CountryId = C("Singapore") },
                new() { StateName = "Dubai",          CountryId = C("UAE") },
                new() { StateName = "Abu Dhabi",      CountryId = C("UAE") },
                new() { StateName = "Lombardy",       CountryId = C("Italy") },
                new() { StateName = "Lazio",          CountryId = C("Italy") },
                new() { StateName = "West Bengal",    CountryId = C("India") }
            };
            db.States.AddRange(data);
            await db.SaveChangesAsync();
        }

        private static async Task SeedCitiesAsync(AppDbContext db)
        {
            if (await db.Cities.AnyAsync()) return;

            var states = await db.States.ToListAsync();
            int S(string name) => states.First(s => s.StateName == name).StateId;

            var data = new List<City>
            {
                new() { CityName = "Los Angeles",    StateId = S("California") },
                new() { CityName = "San Francisco",  StateId = S("California") },
                new() { CityName = "San Diego",      StateId = S("California") },
                new() { CityName = "New York City",  StateId = S("New York") },
                new() { CityName = "Buffalo",        StateId = S("New York") },
                new() { CityName = "Houston",        StateId = S("Texas") },
                new() { CityName = "Dallas",         StateId = S("Texas") },
                new() { CityName = "Austin",         StateId = S("Texas") },
                new() { CityName = "Miami",          StateId = S("Florida") },
                new() { CityName = "Orlando",        StateId = S("Florida") },
                new() { CityName = "Mumbai",         StateId = S("Maharashtra") },
                new() { CityName = "Pune",           StateId = S("Maharashtra") },
                new() { CityName = "Nagpur",         StateId = S("Maharashtra") },
                new() { CityName = "Bengaluru",      StateId = S("Karnataka") },
                new() { CityName = "Mysuru",         StateId = S("Karnataka") },
                new() { CityName = "Chennai",        StateId = S("Tamil Nadu") },
                new() { CityName = "Coimbatore",     StateId = S("Tamil Nadu") },
                new() { CityName = "Ahmedabad",      StateId = S("Gujarat") },
                new() { CityName = "Surat",          StateId = S("Gujarat") },
                new() { CityName = "New Delhi",      StateId = S("Delhi") },
                new() { CityName = "London",         StateId = S("England") },
                new() { CityName = "Manchester",     StateId = S("England") },
                new() { CityName = "Birmingham",     StateId = S("England") },
                new() { CityName = "Edinburgh",      StateId = S("Scotland") },
                new() { CityName = "Glasgow",        StateId = S("Scotland") },
                new() { CityName = "Cardiff",        StateId = S("Wales") },
                new() { CityName = "Shinjuku",       StateId = S("Tokyo") },
                new() { CityName = "Shibuya",        StateId = S("Tokyo") },
                new() { CityName = "Osaka City",     StateId = S("Osaka") },
                new() { CityName = "Sydney",         StateId = S("New South Wales") },
                new() { CityName = "Newcastle",      StateId = S("New South Wales") },
                new() { CityName = "Melbourne",      StateId = S("Victoria") },
                new() { CityName = "Geelong",        StateId = S("Victoria") },
                new() { CityName = "Toronto",        StateId = S("Ontario") },
                new() { CityName = "Ottawa",         StateId = S("Ontario") },
                new() { CityName = "Montreal",       StateId = S("Quebec") },
                new() { CityName = "Munich",         StateId = S("Bavaria") },
                new() { CityName = "Nuremberg",      StateId = S("Bavaria") },
                new() { CityName = "Berlin City",    StateId = S("Berlin") },
                new() { CityName = "Paris",          StateId = S("Ile-de-France") },
                new() { CityName = "Versailles",     StateId = S("Ile-de-France") },
                new() { CityName = "Marseille",      StateId = S("Provence") },
                new() { CityName = "Zurich City",    StateId = S("Zurich") },
                new() { CityName = "Winterthur",     StateId = S("Zurich") },
                new() { CityName = "Geneva City",    StateId = S("Geneva") },
                new() { CityName = "Singapore City", StateId = S("Central") },
                new() { CityName = "Jurong",         StateId = S("Central") },
                new() { CityName = "Dubai City",     StateId = S("Dubai") },
                new() { CityName = "Deira",          StateId = S("Dubai") },
                new() { CityName = "Abu Dhabi City", StateId = S("Abu Dhabi") },
                new() { CityName = "Milan",          StateId = S("Lombardy") },
                new() { CityName = "Bergamo",        StateId = S("Lombardy") },
                new() { CityName = "Rome",           StateId = S("Lazio") },
                new() { CityName = "Kolkata",        StateId = S("West Bengal") },
                new() { CityName = "Howrah",         StateId = S("West Bengal") },
                new() { CityName = "El Segundo",     StateId = S("California") },
                new() { CityName = "Rochester",      StateId = S("New York") },
                new() { CityName = "Fort Worth",     StateId = S("Texas") },
                new() { CityName = "Tampa",          StateId = S("Florida") },
                new() { CityName = "Aurangabad",     StateId = S("Maharashtra") }
            };
            db.Cities.AddRange(data);
            await db.SaveChangesAsync();
        }

        private static async Task SeedMasterTypesAsync(AppDbContext db)
        {
            if (await db.MasterTypes.AnyAsync()) return;

            var data = new List<MasterType>
            {
                new() { MasterTypeName = "Company"  },
                new() { MasterTypeName = "Vendor"   },
                new() { MasterTypeName = "Employee" },
                new() { MasterTypeName = "Customer" }
            };
            db.MasterTypes.AddRange(data);
            await db.SaveChangesAsync();
        }
    }
}
