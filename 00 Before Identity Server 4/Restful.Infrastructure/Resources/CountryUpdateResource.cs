using System.Collections.Generic;

namespace Restful.Infrastructure.Resources
{
    public class CountryUpdateResource
    {
        public CountryUpdateResource()
        {
            Cities = new List<CityResource>();
        }

        public string EnglishName { get; set; }

        public string ChineseName { get; set; }

        public string Abbreviation { get; set; }

        public List<CityResource> Cities { get; set; }
    }
}
