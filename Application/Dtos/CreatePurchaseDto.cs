namespace Application.Dtos
{
    public class CreatePurchaseDto
    {
        public Guid UserId { get; set; } 
        public Guid ProductId { get; set; } 
        public int Quantity { get; set; } 
    }
}