namespace Mikepattyn.Email;

public sealed record EmailAttachment(string FileName, string ContentType, byte[] Content);

public sealed record EmailSendRequest(
    string TemplateId,
    IReadOnlyDictionary<string, string> Data,
    string To,
    string? ReplyTo = null,
    IReadOnlyList<EmailAttachment>? Attachments = null
);
