using System;
using System.Threading;
using System.Threading.Tasks;

namespace SimplySocial.Server.Core.Services
{
    public interface IEmailer
    {
        Task SendEmailConfirmationAsync(String recipient, String username, String confirmationLink, CancellationToken cancellationToken = default);
    }
}
