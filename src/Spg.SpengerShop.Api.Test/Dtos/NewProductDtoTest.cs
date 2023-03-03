using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spg.SpengerShop.Api.Test.Models
{
    public class NewProductDtoTest
    {
        public string Name { get; set; } = string.Empty;
        public string Ean13 { get; set; } = string.Empty;
        public DateTime ExpiryDate { get; set; }
    }
}
