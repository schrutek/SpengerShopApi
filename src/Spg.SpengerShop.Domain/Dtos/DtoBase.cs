using Spg.SpengerShop.Domain.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Spg.SpengerShop.Domain.Dtos
{
    public class DtoBase
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public List<Link>? Links { get; set; }

        public void AddLinks(IEnumerable<Link> links)
        {
            Links = new List<Link>();
            Links.AddRange(links);
        }
    }
}
