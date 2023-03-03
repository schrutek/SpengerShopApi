﻿using Spg.SpengerShop.Domain.Dtos;
using Spg.SpengerShop.Domain.Interfaces;
using Spg.SpengerShop.Domain.Model;
using Spg.SpengerShop.Repository.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spg.SpengerShop.Application.Services
{
    public class ProductService : IAddUpdateableProductService, IReadOnlyProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public IEnumerable<Product> GetAll()
        {
            return _productRepository.GetAll();
        }

        public Product GetByName(string name)
        {
            throw new NotImplementedException();
        }

        public void Create(Product newProduct)
        {
            // Die Bedingungen zum Eintragen prüfen
            _productRepository.Create(newProduct);
        }

        public bool Update(Product product)
        {
            throw new NotImplementedException();
        }

        public bool Delete(int id)
        {
            throw new NotImplementedException();
        }

        IEnumerable<ProductDto> IReadOnlyProductService.GetAll()
        {
            throw new NotImplementedException();
        }
    }
}