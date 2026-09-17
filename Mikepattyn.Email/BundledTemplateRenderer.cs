namespace Mikepattyn.Email;

public sealed class BundledTemplateRenderer(string templatesDirectory) : ITemplateRenderer
{
    private readonly string _templatesDirectory = templatesDirectory;

    public async Task<(string Subject, string HtmlBody)> RenderAsync(
        string templateId,
        IReadOnlyDictionary<string, string> data,
        CancellationToken cancellationToken = default
    )
    {
        var subjectPath = Path.Combine(_templatesDirectory, $"{templateId}.subject.txt");
        var bodyPath = Path.Combine(_templatesDirectory, $"{templateId}.body.html");

        if (!File.Exists(subjectPath) || !File.Exists(bodyPath))
        {
            throw new FileNotFoundException($"Template '{templateId}' was not found.");
        }

        var subjectTemplate = await File.ReadAllTextAsync(subjectPath, cancellationToken);
        var bodyTemplate = await File.ReadAllTextAsync(bodyPath, cancellationToken);

        return (
            ReplacePlaceholders(subjectTemplate, data),
            ReplacePlaceholders(bodyTemplate, data)
        );
    }

    internal static string ReplacePlaceholders(
        string template,
        IReadOnlyDictionary<string, string> data
    )
    {
        var result = template;
        foreach (var (key, value) in data)
        {
            result = result.Replace($"{{{{{key}}}}}", value ?? string.Empty, StringComparison.Ordinal);
        }

        return result;
    }
}
