using FluentValidation;
using Application.Dtos;
using Microsoft.Extensions.Options;
using Infrastructure.Options;
namespace Application.Validators;

public class CategoryDtoValidator : AbstractValidator<CategoryDto>
{
    public CategoryDtoValidator(IOptions<CategoryValidationOptions> categoryValidationOptions)
    {
        var categoryValidation = categoryValidationOptions.Value;

        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("The 'Name' field is required.")
            .MaximumLength(categoryValidation.NameMaxLength)
            .WithMessage($"The 'Name' field must not exceed {categoryValidation.NameMaxLength} characters.");

        RuleFor(c => c.Description)
            .NotEmpty().WithMessage("The 'Description' field is required.")
            .MaximumLength(categoryValidation.DescriptionMaxLength)
            .WithMessage($"The 'Description' field must not exceed {categoryValidation.DescriptionMaxLength} characters.");

        RuleFor(c => c.PhotoUrl)
            .NotEmpty().WithMessage("The 'PhotoUrl' field is required.");
    }
}
