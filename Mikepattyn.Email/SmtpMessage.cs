namespace Mikepattyn.Email;

public sealed record SmtpMessage(
    string FromAddress,
    string FromDisplayName,
    string To,
    string Subject,
    string HtmlBody,
    string? ReplyTo = null,
    IReadOnlyList<EmailAttachment>? Attachments = null
);
