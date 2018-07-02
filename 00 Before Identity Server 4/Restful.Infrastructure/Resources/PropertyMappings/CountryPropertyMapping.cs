using System;
using System.Collections.Generic;
using Restful.Core.Entities;
using Restful.Infrastructure.Services;

namespace Restful.Infrastructure.Resources.PropertyMappings
{
    public class CountryPropertyMapping : PropertyMapping<CountryResource, Country>
    {
        public CountryPropertyMapping() : base(new Dictionary<string, List<MappedProperty>>
            (StringComparer.OrdinalIgnoreCase)
        {
            [nameof(CountryResource.EnglishName)] = new List<MappedProperty>
            {
                new MappedProperty{ Name = nameof(Country.EnglishName), Revert = false}
            },
            [nameof(CountryResource.ChineseName)] = new List<MappedProperty>
            {
                new MappedProperty{ Name = nameof(Country.ChineseName), Revert = false}
            },
            [nameof(CountryResource.Abbreviation)] = new List<MappedProperty>
            {
                new MappedProperty{ Name = nameof(Country.Abbreviation), Revert = false}
            }
        })
        {
        }
    }
}
