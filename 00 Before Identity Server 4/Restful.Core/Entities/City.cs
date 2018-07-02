namespace Restful.Core.Entities
{
    public class City : Entity
    {
        public int CountryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public Country Country { get; set; }
    }
}
