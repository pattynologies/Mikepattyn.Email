using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Mikepattyn.Email;

public sealed class ZohoEmailConfiguration
{
    public required string Host { get; init; }
    public required int Port { get; init; }
    public required string User { get; init; }
    public required string Password { get; init; }
}

public sealed class MailKitSmtpTransport(ZohoEmailConfiguration configuration) : ISmtpTransport
{
    private readonly ZohoEmailConfiguration _configuration = configuration;

    public async Task SendAsync(SmtpMessage message, CancellationToken cancellationToken = default)
    {
        var mimeMessage = new MimeMessage();
        mimeMessage.From.Add(new MailboxAddress(message.FromDisplayName, message.FromAddress));
        mimeMessage.To.Add(MailboxAddress.Parse(message.To));
        mimeMessage.Subject = message.Subject;

        if (!string.IsNullOrWhiteSpace(message.ReplyTo))
        {
            mimeMessage.ReplyTo.Add(MailboxAddress.Parse(message.ReplyTo));
        }

        if (message.Attachments is { Count: > 0 })
        {
            var body = new Multipart("mixed") { new TextPart("html") { Text = message.HtmlBody } };
            foreach (var attachment in message.Attachments)
            {
                body.Add(
                    new MimePart(attachment.ContentType)
                    {
                        Content = new MimeContent(new MemoryStream(attachment.Content)),
                        ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                        FileName = attachment.FileName,
                    }
                );
            }

            mimeMessage.Body = body;
        }
        else
        {
            mimeMessage.Body = new TextPart("html") { Text = message.HtmlBody };
        }

        using var client = new SmtpClient();
        await client.ConnectAsync(
            _configuration.Host,
            _configuration.Port,
            SecureSocketOptions.StartTls,
            cancellationToken
        );
        await client.AuthenticateAsync(
            _configuration.User,
            _configuration.Password,
            cancellationToken
        );
        await client.SendAsync(mimeMessage, cancellationToken);
        await client.DisconnectAsync(quit: true, cancellationToken);
    }
}
