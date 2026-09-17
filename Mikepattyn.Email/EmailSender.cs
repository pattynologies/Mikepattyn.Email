namespace Mikepattyn.Email;

public sealed class EmailSender(
    ITemplateRenderer templateRenderer,
    ISmtpTransport smtpTransport,
    string fromAddress,
    string fromDisplayName
    ) : IEmailSender
{
    private readonly ITemplateRenderer _templateRenderer = templateRenderer;
    private readonly ISmtpTransport _smtpTransport = smtpTransport;
    private readonly string _fromAddress = fromAddress;
    private readonly string _fromDisplayName = fromDisplayName;

    public async Task SendAsync(EmailSendRequest request, CancellationToken cancellationToken = default)
    {
        var (subject, htmlBody) = await _templateRenderer.RenderAsync(
            request.TemplateId,
            request.Data,
            cancellationToken
        );

        await _smtpTransport.SendAsync(
            new SmtpMessage(
                _fromAddress,
                _fromDisplayName,
                request.To,
                subject,
                htmlBody,
                request.ReplyTo,
                request.Attachments
            ),
            cancellationToken
        );
    }
}
