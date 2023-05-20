using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Spg.SpengerShop.Domain.Helpers
{
    public class LinkResourceBase
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public List<Link>? Links { get; set; }

        public LinkResourceBase()
        { }
        public LinkResourceBase(IEnumerable<Link> links)
        {
            Links = new List<Link>();
            Links.AddRange(links);
        }
    }
}
