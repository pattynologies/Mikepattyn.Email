namespace Mikepattyn.Email;

public interface ISmtpTransport
{
    Task SendAsync(SmtpMessage message, CancellationToken cancellationToken = default);
}
