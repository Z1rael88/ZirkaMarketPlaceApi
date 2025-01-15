using Domain.Models;

namespace Application.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string toEmail,IEnumerable<Product> products);
}