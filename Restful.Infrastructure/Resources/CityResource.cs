using Restful.Infrastructure.Resources.Hateoas;

namespace Restful.Infrastructure.Resources
{
    public class CityResource: LinkResourceBase
    {
        public int Id { get; set; }
        public int CountryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
