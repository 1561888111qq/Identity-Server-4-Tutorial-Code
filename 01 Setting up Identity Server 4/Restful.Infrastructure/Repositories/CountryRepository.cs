using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Restful.Core.Helpers;
using Restful.Core.Entities;
using Restful.Core.Interfaces;
using Restful.Infrastructure.Database;
using Restful.Infrastructure.Extensions;
using Restful.Infrastructure.Resources;
using Restful.Infrastructure.Services;

namespace Restful.Infrastructure.Repositories
{
    public class CountryRepository : ICountryRepository
    {
        private readonly MyContext _myContext;
        private readonly IPropertyMappingContainer _propertyMappingContainer;

        public CountryRepository(MyContext myContext, IPropertyMappingContainer propertyMappingContainer)
        {
            _myContext = myContext;
            _propertyMappingContainer = propertyMappingContainer;
        }

        public async Task<PaginatedList<Country>> GetCountriesAsync(CountryResourceParameters parameters)
        {
            var query = _myContext.Countries.AsQueryable();

            if (!string.IsNullOrEmpty(parameters.EnglishName))
            {
                var englishNameClause = parameters.EnglishName.Trim().ToLowerInvariant();
                query = query.Where(x => x.EnglishName.ToLowerInvariant() == englishNameClause);
            }

            if (!string.IsNullOrEmpty(parameters.ChineseName))
            {
                var chineseNameClause = parameters.ChineseName.Trim().ToLowerInvariant();
                query = query.Where(x => x.ChineseName.ToLowerInvariant() == chineseNameClause);
            }

            query = query.ApplySort(parameters.OrderBy, _propertyMappingContainer.Resolve<CountryResource, Country>());

            var count = await query.CountAsync();
            var items = await query
                .Skip(parameters.PageSize * parameters.PageIndex)
                .Take(parameters.PageSize).ToListAsync();

            return new PaginatedList<Country>(parameters.PageIndex, parameters.PageSize, count, items);
        }

        public async Task<IEnumerable<Country>> GetCountriesAsync(IEnumerable<int> ids)
        {
            return await _myContext.Countries.Where(x => ids.Contains(x.Id)).ToListAsync();
        }

        public async Task<Country> GetCountryByIdAsync(int id, bool includeCities = false)
        {
            if (!includeCities)
            {
                return await _myContext.Countries.FindAsync(id);
            }
            return await _myContext.Countries.Include(x => x.Cities).SingleOrDefaultAsync(x => x.Id == id);
        }

        public void AddCountry(Country country)
        {
            _myContext.Countries.Add(country);
        }

        public void UpdateCountry(Country country)
        {
            _myContext.Countries.Update(country);
        }

        public async Task<bool> CountryExistAsync(int countryId)
        {
            return await _myContext.Countries.AnyAsync(x => x.Id == countryId);
        }

        public void DeleteCountry(Country country)
        {
            _myContext.Countries.Remove(country);
        }

    }
}
