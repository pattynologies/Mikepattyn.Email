namespace Mikepattyn.Email;

public interface IEmailSender
{
    Task SendAsync(EmailSendRequest request, CancellationToken cancellationToken = default);
}
