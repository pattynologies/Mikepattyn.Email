namespace Mikepattyn.Email;

public interface ITemplateRenderer
{
    Task<(string Subject, string HtmlBody)> RenderAsync(
        string templateId,
        IReadOnlyDictionary<string, string> data,
        CancellationToken cancellationToken = default
    );
}
