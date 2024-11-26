namespace Infrastructure.Options;


public class ProductValidationOptions
{
    public int NameMaxLength { get; set; }
    public int DescriptionMaxLength { get; set; }
    public int RatingMaxLength {  get; set; }
    public int RatingMinLength { get; set; }
}