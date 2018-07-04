using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Restful.Core.Entities;
using Restful.Core.Interfaces;
using Restful.Infrastructure.Resources;
using Restful.Infrastructure.Resources.Hateoas;

namespace Restful.Api.Controllers
{
    [Route("api/countries/{countryId}/cities")]
    public class CityController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICountryRepository _countryRepository;
        private readonly ICityRepository _cityRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CityController> _logger;
        private readonly IUrlHelper _urlHelper;

        public CityController(IUnitOfWork unitOfWork,
            ICountryRepository countryRepository,
            ICityRepository cityRepository,
            IMapper mapper,
            ILogger<CityController> logger,
            IUrlHelper urlHelper)
        {
            _unitOfWork = unitOfWork;
            _countryRepository = countryRepository;
            _cityRepository = cityRepository;
            _mapper = mapper;
            _logger = logger;
            _urlHelper = urlHelper;
        }

        [HttpGet(Name = "GetCitiesForCountry")]
        public async Task<IActionResult> GetCitiesForCountry(int countryId)
        {
            if (!await _countryRepository.CountryExistAsync(countryId))
            {
                return NotFound();
            }

            var citiesForCountry = await _cityRepository.GetCitiesForCountryAsync(countryId);
            var citiesResources = _mapper.Map<IEnumerable<CityResource>>(citiesForCountry);

            citiesResources = citiesResources.Select(CreateLinksForCity);

            var wrapper = new LinkCollectionResourceWrapper<CityResource>(citiesResources);

            return Ok(CreateLinksForCitties(wrapper));
        }

        [HttpGet("{cityId}", Name = "GetCityForCountry")]
        public async Task<IActionResult> GetCityForCountry(int countryId, int cityId)
        {
            if (!await _countryRepository.CountryExistAsync(countryId))
            {
                _logger.LogInformation("Not Found");
                return NotFound();
            }

            var cityForCountry = await _cityRepository.GetCityForCountryAsync(countryId, cityId);
            if (cityForCountry == null)
            {
                return NotFound();
            }
            var cityResource = _mapper.Map<CityResource>(cityForCountry);
            return Ok(CreateLinksForCity(cityResource));
        }

        [HttpPost(Name = "CreateCityForCountry")]
        public async Task<IActionResult> CreateCityForCountry(int countryId, [FromBody] CityAddResource city)
        {
            if (city == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            if (!await _countryRepository.CountryExistAsync(countryId))
            {
                return NotFound();
            }

            var cityModel = _mapper.Map<City>(city);
            _cityRepository.AddCityForCountry(countryId, cityModel);
            if (!await _unitOfWork.SaveAsync())
            {
                throw new Exception("Error occurred when adding");
            }

            var cityResource = Mapper.Map<CityResource>(cityModel);

            return CreatedAtRoute("GetCityForCountry", new { countryId, cityId = cityModel.Id }, CreateLinksForCity(cityResource));
        }

        [HttpDelete("{cityId}", Name = "DeleteCityForCountry")]
        public async Task<IActionResult> DeleteCityForCountry(int countryId, int cityId)
        {
            if (!await _countryRepository.CountryExistAsync(countryId))
            {
                return NotFound();
            }

            var city = await _cityRepository.GetCityForCountryAsync(countryId, cityId);
            if (city == null)
            {
                return NotFound();
            }

            _cityRepository.DeleteCity(city);

            if (!await _unitOfWork.SaveAsync())
            {
                throw new Exception($"Deleting city {cityId} for country {countryId} failed when saving.");
            }

            return NoContent();
        }

        [HttpPut("{cityId}", Name = "UpdateCityForCountry")]
        public async Task<IActionResult> UpdateCityForCountry(int countryId, int cityId, 
            [FromBody] CityUpdateResource cityUpdate)
        {
            if (cityUpdate == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            if (!await _countryRepository.CountryExistAsync(countryId))
            {
                return NotFound();
            }

            var city = await _cityRepository.GetCityForCountryAsync(countryId, cityId);
            if (city == null)
            {
                //var cityToAdd = _mapper.Map<City>(cityUpdate);
                //cityToAdd.Id = cityId; // 如果Id不是自增的话
                //_cityRepository.AddCityForCountry(countryId, cityToAdd);

                //if (!await _unitOfWork.SaveAsync())
                //{
                //    return StatusCode(500, $"Upserting city {cityId} for country {countryId} failed when inserting");
                //}

                //var cityResource = Mapper.Map<CityResource>(cityToAdd);
                //return CreatedAtRoute("GetCityForCountry", new { countryId, cityId }, cityResource);
                return NotFound();
            }

            // 把cityUpdate的属性值都映射给city
            _mapper.Map(cityUpdate, city);

            _cityRepository.UpdateCityForCountry(city);

            if (!await _unitOfWork.SaveAsync())
            {
                throw new Exception($"Updating city {cityId} for country {countryId} failed when saving.");
            }

            return NoContent(); 
        }

        [HttpPatch("{cityId}", Name = "PartiallyUpdateCityForCountry")]
        public async Task<IActionResult> PartiallyUpdateCityForCountry(int countryId, int cityId, 
            [FromBody] JsonPatchDocument<CityUpdateResource> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            if (!await _countryRepository.CountryExistAsync(countryId))
            {
                return NotFound();
            }

            var city = await _cityRepository.GetCityForCountryAsync(countryId, cityId);
            if (city == null)
            {
                //var cityUpdate = new CityUpdateResource();
                //patchDoc.ApplyTo(cityUpdate);
                //var cityToAdd = _mapper.Map<City>(cityUpdate);
                //cityToAdd.Id = cityId; // 只适用于Id不是自增的情况

                //_cityRepository.AddCityForCountry(countryId, cityToAdd);

                //if (!await _unitOfWork.SaveAsync())
                //{
                //    return StatusCode(500, $"P city {cityId} for country {countryId} failed when inserting");
                //}
                //var cityResource = Mapper.Map<CityResource>(cityToAdd);
                //return CreatedAtRoute("GetCityForCountry", new { countryId, cityId }, cityResource);

                return NotFound();
            }

            var cityToPatch = _mapper.Map<CityUpdateResource>(city);

            patchDoc.ApplyTo(cityToPatch, ModelState);

            TryValidateModel(cityToPatch);

            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }

            _mapper.Map(cityToPatch, city);
            _cityRepository.UpdateCityForCountry(city);

            if (!await _unitOfWork.SaveAsync())
            {
                throw new Exception($"Patching city {cityId} for country {countryId} failed when saving.");
            }

            return NoContent();
        }

        private CityResource CreateLinksForCity(CityResource city)
        {
            city.Links.Add(
                new LinkResource(
                    href: _urlHelper.Link("GetCityForCountry", new { cityId = city.Id }), 
                    rel: "self", 
                    method: "GET"));
            city.Links.Add(
                new LinkResource(
                    href: _urlHelper.Link("UpdateCityForCountry", new { cityId = city.Id }), 
                    rel: "update_city", 
                    method: "PUT"));
            city.Links.Add(
                new LinkResource(
                    href: _urlHelper.Link("PartiallyUpdateCityForCountry", new { cityId = city.Id }), 
                    rel: "partially_update_city", 
                    method: "PATCH"));
            city.Links.Add(
                new LinkResource(
                    href: _urlHelper.Link("DeleteCityForCountry", new { cityId = city.Id }), 
                    rel: "delete_city", 
                    method: "DELETE"));

            return city;
        }

        private LinkCollectionResourceWrapper<CityResource> CreateLinksForCitties(
            LinkCollectionResourceWrapper<CityResource> citiesWrapper)
        {
            citiesWrapper.Links.Add(
                new LinkResource(
                    href: _urlHelper.Link("GetCitiesForCountry", null),
                    rel: "self",
                    method: "GET"));

            return citiesWrapper;
        }
    }
}
