using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Restful.Api.Helpers;
using Restful.Core.Entities;
using Restful.Core.Interfaces;
using Restful.Infrastructure.Resources;

namespace Restful.Api.Controllers
{
    [Route("api/countrycollections")]
    public class CountryCollectionsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICountryRepository _countryRepository;
        private readonly IMapper _mapper;

        public CountryCollectionsController(
            IUnitOfWork unitOfWork,
            ICountryRepository countryRepository,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _countryRepository = countryRepository;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCountryCollection(
            [FromBody] IEnumerable<CountryAddResource> countries)
        {
            if (countries == null)
            {
                return BadRequest();
            }

            var countriesModel = _mapper.Map<IEnumerable<Country>>(countries);
            foreach (var country in countriesModel)
            {
                _countryRepository.AddCountry(country);
            }

            if (!await _unitOfWork.SaveAsync())
            {
                throw new Exception("Error occurred when adding");
            }

            var countryResources = _mapper.Map<IEnumerable<CountryResource>>(countriesModel);
            var idsStr = string.Join(",", countryResources.Select(x => x.Id));

            return CreatedAtRoute("GetCountryCollection", new { ids = idsStr }, countryResources);
        }

        [HttpGet("({ids})", Name = "GetCountryCollection")]
        public async Task<IActionResult> GetCountryCollection(
            [ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<int> ids)
        {
            if (ids == null)
            {
                return BadRequest();
            }

            var idList = ids.ToList();
            var countries = await _countryRepository.GetCountriesAsync(idList);

            if (idList.Count != countries.Count())
            {
                return NotFound();
            }

            var countryResources = Mapper.Map<IEnumerable<CountryResource>>(countries);
            return Ok(countryResources);
        }
    }
}
