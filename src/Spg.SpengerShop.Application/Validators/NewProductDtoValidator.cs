using FluentValidation;
using Spg.SpengerShop.Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spg.SpengerShop.Application.Validators
{
    public class NewProductDtoValidator : AbstractValidator<NewProductDto>
    {
        public NewProductDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .Length(3, 10)
                .WithMessage("Länge: 3-10!!");
        }
    }
}
