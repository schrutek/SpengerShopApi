using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spg.SpengerShop.Domain.Helpers
{
    public class LinkCollectionWrapper<T> : LinkResourceBase
    {
        public IEnumerable<T> Value { get; set; } = new List<T>();

        public LinkCollectionWrapper()
        { }
        public LinkCollectionWrapper(IEnumerable<T> value)
        {
            Value = value;
        }
    }
}
