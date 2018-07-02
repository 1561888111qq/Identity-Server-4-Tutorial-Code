using System;
using System.Collections.Generic;
using System.Linq;
using Restful.Core.Interfaces;

namespace Restful.Infrastructure.Services
{
    public class PropertyMappingContainer: IPropertyMappingContainer
    {
        private readonly IList<IPropertyMapping> _propertyMappings = new List<IPropertyMapping>();

        public void Register<T>() where T: IPropertyMapping, new()
        {
            _propertyMappings.Add(new T());
        }

        public IPropertyMapping Resolve<TSource, TDestination>() where TDestination: IEntity
        {
            var matchingMapping = _propertyMappings.OfType<PropertyMapping<TSource, TDestination>>().ToList();
            if (matchingMapping.Count == 1)
            {
                return matchingMapping.First();
            }

            throw new Exception($"Cannot find property mapping instance for <{typeof(TSource)},{typeof(TDestination)}");
        }

        public bool ValidMappingExistsFor<TSource, TDestination>(string fields) where TDestination : IEntity
        {
            var propertyMapping = Resolve<TSource, TDestination>();

            if (string.IsNullOrWhiteSpace(fields))
            {
                return true;
            }

            var fieldsAfterSplit = fields.Split(',');
            foreach (var field in fieldsAfterSplit)
            {
                var trimmedField = field.Trim();
                var indexOfFirstSpace = trimmedField.IndexOf(" ", StringComparison.Ordinal);
                var propertyName = indexOfFirstSpace == -1 ?
                    trimmedField : trimmedField.Remove(indexOfFirstSpace);
                if (!propertyMapping.MappingDictionary.ContainsKey(propertyName))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
