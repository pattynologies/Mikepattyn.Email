using Mikepattyn.Email;

namespace Mikepattyn.Email.Tests;

public class BundledTemplateRendererTests
{
    [Fact]
    public async Task RenderAsync_ReplacesPlaceholdersInSubjectAndBody()
    {
        var templatesDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(templatesDir);
        await File.WriteAllTextAsync(
            Path.Combine(templatesDir, "contact.subject.txt"),
            "Message from {{name}}"
        );
        await File.WriteAllTextAsync(
            Path.Combine(templatesDir, "contact.body.html"),
            "<p>Email: {{email}}</p><p>{{message}}</p>"
        );

        try
        {
            var renderer = new BundledTemplateRenderer(templatesDir);
            var (subject, body) = await renderer.RenderAsync(
                "contact",
                new Dictionary<string, string>
                {
                    ["name"] = "Alex",
                    ["email"] = "alex@example.com",
                    ["message"] = "Hello there",
                }
            );

            Assert.Equal("Message from Alex", subject);
            Assert.Equal("<p>Email: alex@example.com</p><p>Hello there</p>", body);
        }
        finally
        {
            Directory.Delete(templatesDir, recursive: true);
        }
    }

    [Fact]
    public async Task RenderAsync_ThrowsWhenTemplateMissing()
    {
        var templatesDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(templatesDir);

        try
        {
            var renderer = new BundledTemplateRenderer(templatesDir);
            await Assert.ThrowsAsync<FileNotFoundException>(
                () => renderer.RenderAsync("missing", new Dictionary<string, string>())
            );
        }
        finally
        {
            Directory.Delete(templatesDir, recursive: true);
        }
    }
}
