using Spg.SpengerShop.Domain.Interfaces;
using Spg.SpengerShop.Domain.Model;
using System.Linq.Expressions;

namespace Spg.SpengerShop.Application.Services.Extended
{
    public static class ProductServiceExtensions
    {
        public static IReadOnlyProductService UseFilterByName(this IReadOnlyProductService service, string filter)
        {
            Expression<Func<Product, bool>> filterExpression = default!;
            if (!string.IsNullOrEmpty(filter))
            {
                filterExpression = p => p.Name.Contains(filter);
                service.Products = service.Products.Where(filterExpression);
                return service;
            }
            return service;
        }

        public static IReadOnlyProductService UseFilterByExpiryDate(this IReadOnlyProductService service, DateTime from, DateTime to)
        {
            return service;
        }

        public static IReadOnlyProductService UseFilterByStock(this IReadOnlyProductService service, int moreThan)
        {
            return service;
        }

        public static IReadOnlyProductService UseOrder(this IReadOnlyProductService service, string orderByColName)
        {
            return service;
        }
    }
}