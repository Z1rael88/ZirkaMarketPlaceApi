using FluentValidation;
using Application.Dtos;
using Microsoft.Extensions.Options;
using Infrastructure.Options;



namespace Application.Validators;

public class ProductDtoValidator : AbstractValidator<CreateProductDto>
{
    public ProductDtoValidator(IOptions<ProductValidationOptions> productValidationOptions)
    {
        var productValidation = productValidationOptions.Value;

        RuleFor(p => p.Name)
            .NotEmpty().WithMessage("The 'Name' field is required.")
            .MaximumLength(productValidation.NameMaxLength)
            .WithMessage($"The 'Name' field must not exceed {productValidation.NameMaxLength} characters.");

        RuleFor(p => p.Description)
            .NotEmpty().WithMessage("The 'Description' field is required.")
            .MaximumLength(productValidation.DescriptionMaxLength)
            .WithMessage($"The 'Description' field must not exceed {productValidation.DescriptionMaxLength} characters.");

        RuleFor(p => p.PhotoUrl)
            .NotEmpty().WithMessage("The 'PhotoUrl' field is required.")
            .Must(IsValidUrl).WithMessage("The 'PhotoUrl' field must be a valid URL.");


    }

    private bool IsValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out _);
    }
}

