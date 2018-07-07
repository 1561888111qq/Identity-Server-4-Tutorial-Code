using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Restful.Api.Helpers;
using Restful.Core.Helpers;
using Restful.Core.Entities;
using Restful.Core.Interfaces;
using Restful.Infrastructure.Extensions;
using Restful.Infrastructure.Resources;
using Restful.Infrastructure.Resources.Hateoas;
using Restful.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;

namespace Restful.Api.Controllers
{
    [Route("api/countries")]
    public class CountryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICountryRepository _countryRepository;
        private readonly IMapper _mapper;
        private readonly IUrlHelper _urlHelper;
        private readonly IPropertyMappingContainer _propertyMappingContainer;
        private readonly ITypeHelperService _typeHelperService;
        
        public CountryController(
            IUnitOfWork unitOfWork,
            ICountryRepository countryRepository,
            IMapper mapper,
            IUrlHelper urlHelper,
            IPropertyMappingContainer propertyMappingContainer,
            ITypeHelperService typeHelperService)
        {
            _unitOfWork = unitOfWork;
            _countryRepository = countryRepository;
            _mapper = mapper;
            _urlHelper = urlHelper;
            _propertyMappingContainer = propertyMappingContainer;
            _typeHelperService = typeHelperService;
        }

        [HttpGet(Name = "GetCountries")]
        [Authorize(Roles = "管理员")]
        public async Task<IActionResult> GetCountries(CountryResourceParameters countryResourceParameters,
            [FromHeader(Name = "Accept")] string mediaType)
        {
            foreach (var claim in User.Claims)
            {
                Console.WriteLine(claim.Type + ", " + claim.Value);
            }

            if (!_propertyMappingContainer.ValidMappingExistsFor<CountryResource, Country>(countryResourceParameters.OrderBy))
            {
                return BadRequest("Can't find the fields for sorting.");
            }

            if (!_typeHelperService.TypeHasProperties<CountryResource>(countryResourceParameters.Fields))
            {
                return BadRequest("Can't find the fields on Resource.");
            }

            var pagedList = await _countryRepository.GetCountriesAsync(countryResourceParameters);
            var countryResources = _mapper.Map<List<CountryResource>>(pagedList);

            if (mediaType == "application/vnd.solenovex.hateoas+json")
            {
                var meta = new
                {
                    pagedList.TotalItemsCount,
                    pagedList.PaginationBase.PageSize,
                    pagedList.PaginationBase.PageIndex,
                    pagedList.PageCount
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(meta, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                }));

                var links = CreateLinksForCountries(countryResourceParameters, pagedList.HasPrevious, pagedList.HasNext);
                var shapedResources = countryResources.ToDynamicIEnumerable(countryResourceParameters.Fields);
                var shapedResourcesWithLinks = shapedResources.Select(country =>
                {
                    var countryDict = country as IDictionary<string, object>;
                    var countryLinks = CreateLinksForCountry((int)countryDict["Id"], countryResourceParameters.Fields);
                    countryDict.Add("links", countryLinks);
                    return countryDict;
                });
                var linkedCountries = new
                {
                    value = shapedResourcesWithLinks,
                    links
                };

                return Ok(linkedCountries);
            }
            else
            {
                var previousPageLink = pagedList.HasPrevious ?
                    CreateCountryUri(countryResourceParameters,
                        PaginationResourceUriType.PreviousPage) : null;

                var nextPageLink = pagedList.HasNext ?
                    CreateCountryUri(countryResourceParameters,
                        PaginationResourceUriType.NextPage) : null;

                var meta = new
                {
                    pagedList.TotalItemsCount,
                    pagedList.PaginationBase.PageSize,
                    pagedList.PaginationBase.PageIndex,
                    pagedList.PageCount,
                    previousPageLink,
                    nextPageLink
                };

                Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(meta, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                }));

                return Ok(countryResources.ToDynamicIEnumerable(countryResourceParameters.Fields));
            }
        }
        
        [DisableCors]
        [HttpGet("{id}", Name = "GetCountry")]
        public async Task<IActionResult> GetCountry(int id, string fields = null, bool includeCities = false)
        {
            if (!_typeHelperService.TypeHasProperties<CountryResource>(fields))
            {
                return BadRequest("Can't find the fields on Resource.");
            }
            var country = await _countryRepository.GetCountryByIdAsync(id, includeCities);
            if (country == null)
            {
                return NotFound();
            }
            var countryResource = _mapper.Map<CountryResource>(country);

            var links = CreateLinksForCountry(id, fields);
            var result = countryResource.ToDynamic(fields) as IDictionary<string, object>;
            result.Add("links", links);

            return Ok(result);
        }

        [HttpPost(Name = "CreateCountry")]
        [RequestHeaderMatchingMediaType("Content-Type", new[] { "application/vnd.solenovex.country.create+json" })]
        [RequestHeaderMatchingMediaType("Accept", new[] { "application/vnd.solenovex.country.display+json" })]
        public async Task<IActionResult> CreateCountry([FromBody] CountryAddResource country)
        {
            if (country == null)
            {
                return BadRequest();
            }

            var countryModel = _mapper.Map<Country>(country);
            _countryRepository.AddCountry(countryModel);
            if (!await _unitOfWork.SaveAsync())
            {
                throw new Exception("Error occurred when adding");
            }

            var countryResource = Mapper.Map<CountryResource>(countryModel);

            var links = CreateLinksForCountry(countryModel.Id);
            var linkedCountryResource = countryResource.ToDynamic() as IDictionary<string, object>;
            linkedCountryResource.Add("links", links);

            return CreatedAtRoute("GetCountry", new { id = linkedCountryResource["Id"] }, linkedCountryResource);
        }

        [HttpPost(Name = "CreateCountryWithContinent")]
        [RequestHeaderMatchingMediaType("Content-Type", new[] { "application/vnd.solenovex.countrywithcontinent.create+json" })]
        [RequestHeaderMatchingMediaType("Accept", new[] { "application/vnd.solenovex.countrywithcontinent.display+json" })]
        public async Task<IActionResult> CreateCountryWithContinent(
            [FromBody] CountryAddWithContinentResource country)
        {
            if (country == null)
            {
                return BadRequest();
            }

            var countryModel = _mapper.Map<Country>(country);
            _countryRepository.AddCountry(countryModel);
            if (!await _unitOfWork.SaveAsync())
            {
                throw new Exception("Error occurred when adding");
            }

            var countryResource = Mapper.Map<CountryResource>(countryModel);

            var links = CreateLinksForCountry(countryModel.Id);
            var linkedCountryResource = countryResource.ToDynamic() as IDictionary<string, object>;
            linkedCountryResource.Add("links", links);

            return CreatedAtRoute("GetCountry", new { id = linkedCountryResource["Id"] }, linkedCountryResource);
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> BlockCreatingCountry(int id)
        {
            var country = await _countryRepository.GetCountryByIdAsync(id);
            if (country == null)
            {
                return NotFound();
            }

            return StatusCode(StatusCodes.Status409Conflict);
        }

        [HttpDelete("{id}", Name = "DeleteCountry")]
        public async Task<IActionResult> DeleteCountry(int id)
        {
            var country = await _countryRepository.GetCountryByIdAsync(id);
            if (country == null)
            {
                return NotFound();
            }

            _countryRepository.DeleteCountry(country);

            if (!await _unitOfWork.SaveAsync())
            {
                throw new Exception($"Deleting country {id} failed when saving.");
            }

            return NoContent();
        }

        [HttpPut("{id}", Name = "UpdateCountry")]
        [RequestHeaderMatchingMediaType("Content-Type", new[] { "application/vnd.solenovex.country.update+json" })]
        public async Task<IActionResult> UpdateCountry(int id, [FromBody] CountryUpdateResource countryUpdate)
        {
            if (countryUpdate == null)
            {
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                return new UnprocessableEntityObjectResult(ModelState);
            }
            var country = await _countryRepository.GetCountryByIdAsync(id, includeCities: true);
            if (country == null)
            {
                return NotFound();
            }
            _mapper.Map(countryUpdate, country);
            if (!await _unitOfWork.SaveAsync())
            {
                throw new Exception($"Updating country {id} failed when saving.");
            }
            return NoContent();
        }

        private string CreateCountryUri(CountryResourceParameters parameters, PaginationResourceUriType uriType)
        {
            switch (uriType)
            {
                case PaginationResourceUriType.PreviousPage:
                    var previousParameters = new
                    {
                        pageIndex = parameters.PageIndex - 1,
                        pageSize = parameters.PageSize,
                        orderBy = parameters.OrderBy,
                        fields = parameters.Fields,
                        chineseName = parameters.ChineseName,
                        englishName = parameters.EnglishName
                    };
                    return _urlHelper.Link("GetCountries", previousParameters);
                case PaginationResourceUriType.NextPage:
                    var nextParameters = new
                    {
                        pageIndex = parameters.PageIndex + 1,
                        pageSize = parameters.PageSize,
                        orderBy = parameters.OrderBy,
                        fields = parameters.Fields,
                        chineseName = parameters.ChineseName,
                        englishName = parameters.EnglishName
                    };
                    return _urlHelper.Link("GetCountries", nextParameters);
                default:
                case PaginationResourceUriType.CurrentPage:
                    var currentParameters = new
                    {
                        pageIndex = parameters.PageIndex,
                        pageSize = parameters.PageSize,
                        orderBy = parameters.OrderBy,
                        fields = parameters.Fields,
                        chineseName = parameters.ChineseName,
                        englishName = parameters.EnglishName
                    };
                    return _urlHelper.Link("GetCountries", currentParameters);
            }
        }

        private IEnumerable<LinkResource> CreateLinksForCountry(int id, string fields = null)
        {
            var links = new List<LinkResource>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                links.Add(
                    new LinkResource(
                        _urlHelper.Link("GetCountry", new { id }), "self", "GET"));
            }
            else
            {
                links.Add(
                    new LinkResource(
                        _urlHelper.Link("GetCountry", new { id, fields }), "self", "GET"));
            }

            links.Add(
                new LinkResource(
                    _urlHelper.Link("DeleteCountry", new { id }), "delete_country", "DELETE"));

            links.Add(
                new LinkResource(
                    _urlHelper.Link("CreateCityForCountry", new { countryId = id }), "create_city_for_country", "POST"));

            links.Add(
                new LinkResource(
                    _urlHelper.Link("GetCitiesForCountry", new { countryId = id }), "get_cities_for_country", "GET"));

            return links;
        }

        private IEnumerable<LinkResource> CreateLinksForCountries(CountryResourceParameters countryResourceParameters,
            bool hasPrevious, bool hasNext)
        {
            var links = new List<LinkResource>
            {
                new LinkResource(
                    CreateCountryUri(countryResourceParameters, PaginationResourceUriType.CurrentPage),
                    "self", "GET")
            };

            if (hasPrevious)
            {
                links.Add(
                    new LinkResource(
                        CreateCountryUri(countryResourceParameters, PaginationResourceUriType.PreviousPage),
                        "previous_page", "GET"));
            }

            if (hasNext)
            {
                links.Add(
                    new LinkResource(
                        CreateCountryUri(countryResourceParameters, PaginationResourceUriType.NextPage),
                        "next_page", "GET"));
            }

            return links;
        }
    }
}
