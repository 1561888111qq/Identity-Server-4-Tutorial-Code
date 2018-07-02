using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Restful.Core.Entities;
using Restful.Core.Interfaces;
using Restful.Infrastructure.Database;

namespace Restful.Infrastructure.Repositories
{
    public class CityRepository : ICityRepository
    {
        private readonly MyContext _myContext;

        public CityRepository(MyContext myContext)
        {
            _myContext = myContext;
        }

        public async Task<List<City>> GetCitiesForCountryAsync(int countryId)
        {
            return await _myContext.Cities.Where(x => x.CountryId == countryId).ToListAsync();
        }

        public async Task<City> GetCityForCountryAsync(int countryId, int cityId)
        {
            return await _myContext.Cities.SingleOrDefaultAsync(x => x.CountryId == countryId && x.Id == cityId);
        }

        public void AddCityForCountry(int countryId, City city)
        {
            city.CountryId = countryId;
            _myContext.Cities.Add(city);
        }

        public void DeleteCity(City city)
        {
            _myContext.Cities.Remove(city);
        }

        public void UpdateCityForCountry(City city)
        {
            _myContext.Update(city);
        }

    }
}
