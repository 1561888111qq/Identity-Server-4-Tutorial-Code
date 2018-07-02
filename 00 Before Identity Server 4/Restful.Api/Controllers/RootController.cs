using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Restful.Infrastructure.Resources.Hateoas;

namespace Restful.Api.Controllers
{
    [Route("api")]
    public class RootController: Controller
    {
        private readonly IUrlHelper _urlHelper;

        public RootController(IUrlHelper urlHelper)
        {
            _urlHelper = urlHelper;
        }

        [HttpGet(Name = "GetRoot")]
        public IActionResult GetRoot([FromHeader(Name = "Accept")]string mediaType)
        {
            if (mediaType == "application/vnd.mycompany.hateoas+json")
            {
                var links = new List<LinkResource>
                {
                    new LinkResource(
                        _urlHelper.Link("GetRoot", null), "self", "GET"),
                    new LinkResource(
                        _urlHelper.Link("GetCountries", null), "countries", "GET"),
                    new LinkResource(
                        _urlHelper.Link("CreateCountry", null), "create_country", "POST")
                };

                return Ok(links);
            }

            return NoContent();
        }
    }
}
