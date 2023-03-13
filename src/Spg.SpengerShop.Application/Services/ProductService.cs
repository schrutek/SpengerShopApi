using Spg.SpengerShop.Application.Mock;
using Spg.SpengerShop.Domain.Dtos;
using Spg.SpengerShop.Domain.Exceptions;
using Spg.SpengerShop.Domain.Helpers;
using Spg.SpengerShop.Domain.Interfaces;
using Spg.SpengerShop.Domain.Model;

namespace Spg.SpengerShop.Application.Services
{
    public class ProductService : IAddUpdateableProductService, IReadOnlyProductService
    {
        private readonly IRepositoryBase<Product> _productRepository;
        private readonly IReadOnlyRepositoryBase<Product> _readOnlyProductRepository;
        private readonly IReadOnlyRepositoryBase<Category> _readOnlyCategoryRepository;
        private readonly IDateTimeService _dateTimeService;

        public ProductService(
            IRepositoryBase<Product> productRepository,
            IReadOnlyRepositoryBase<Product> readOnlyProductRepository,
            IReadOnlyRepositoryBase<Category> readOnlyCategoryRepository,
            IDateTimeService dateTimeService)
        {
            _productRepository = productRepository;
            _readOnlyProductRepository = readOnlyProductRepository;
            _readOnlyCategoryRepository = readOnlyCategoryRepository;
            _dateTimeService = dateTimeService;
        }
        public IQueryable<Product> Products { get; set; }

        public IReadOnlyProductService Load()
        {
            Products = _readOnlyProductRepository.GetAll().Result;
            return this;
        }

        public IEnumerable<ProductDto> GetData()
        {
            List<ProductDto> data = new List<ProductDto>();
            foreach (Product product in Products)
            {
                data.Add(new ProductDto() 
                {
                    Name = product.Name, 
                    Ean13 = product.Ean13, 
                    ExpiryDate = product.ExpiryDate, 
                    DeliveryDate = product.DeliveryDate
                });
            }
            return data;
        }

        public PagenatedList<ProductDto> GetDataPaged(int pageIndex, int pageSize)
        {
            return PagenatedList<ProductDto>.Create(GetData().AsQueryable(), pageIndex, pageSize);
        }

        public Product GetByName(string name)
        {
            throw new NotImplementedException();
        }

        public void Create(ProductDto dto)
        {
            // Init
            Category category = _readOnlyCategoryRepository.GetSingleOrDefaultByGuid<Category>(dto.CategoryId);

            // Validation
            if (dto.ExpiryDate < _dateTimeService.UtcNow.AddDays(14))
            {
                throw new ProductCreateValidationException("Datum muss 2 Wochen in Zukunft liegen!");
            }

            // Mapping
            Product newProduct = new Product(dto.Name, 20, dto.Ean13, "M", dto.ExpiryDate, category);

            // Act + Save
            _productRepository.Create(newProduct);

            // [Save]
        }

        public bool Update(Product product)
        {
            throw new NotImplementedException();
        }

        public bool Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}
