using System.Collections.Generic;

namespace Restful.Infrastructure.Resources.Hateoas
{
    public class LinkCollectionResourceWrapper<T> : LinkResourceBase
        where T : LinkResourceBase
    {
        public LinkCollectionResourceWrapper(IEnumerable<T> value)
        {
            Value = value;
        }

        public IEnumerable<T> Value { get; set; }
    }
}
