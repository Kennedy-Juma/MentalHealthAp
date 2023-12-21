using MentalHealthAp.Models;

namespace MentalHealthAp.Services
{
    public interface IEmailService
    {
        bool SendEmail(Email request);
    }
}
