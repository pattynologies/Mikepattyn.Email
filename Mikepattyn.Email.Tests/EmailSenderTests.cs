using Mikepattyn.Email;

namespace Mikepattyn.Email.Tests;

public class EmailSenderTests
{
    [Fact]
    public async Task SendAsync_RendersTemplateAndSendsViaTransport()
    {
        var transport = new RecordingSmtpTransport();
        var renderer = new FakeTemplateRenderer(
            ("contact", "New message", "<p>Body</p>")
        );
        var sender = new EmailSender(
            renderer,
            transport,
            fromAddress: "info@mikepattyn.nl",
            fromDisplayName: "Mike Pattyn"
        );

        await sender.SendAsync(
            new EmailSendRequest(
                TemplateId: "contact",
                Data: new Dictionary<string, string> { ["email"] = "visitor@example.com" },
                To: "info@mikepattyn.nl",
                ReplyTo: "visitor@example.com"
            )
        );

        var sent = Assert.Single(transport.Messages);
        Assert.Equal("info@mikepattyn.nl", sent.FromAddress);
        Assert.Equal("Mike Pattyn", sent.FromDisplayName);
        Assert.Equal("info@mikepattyn.nl", sent.To);
        Assert.Equal("New message", sent.Subject);
        Assert.Equal("<p>Body</p>", sent.HtmlBody);
        Assert.Equal("visitor@example.com", sent.ReplyTo);
    }

    private sealed class RecordingSmtpTransport : ISmtpTransport
    {
        public List<SmtpMessage> Messages { get; } = [];

        public Task SendAsync(SmtpMessage message, CancellationToken cancellationToken = default)
        {
            Messages.Add(message);
            return Task.CompletedTask;
        }
    }

    private sealed class FakeTemplateRenderer : ITemplateRenderer
    {
        private readonly Dictionary<string, (string Subject, string HtmlBody)> _templates;

        public FakeTemplateRenderer(params (string Id, string Subject, string HtmlBody)[] templates)
        {
            _templates = templates.ToDictionary(t => t.Id, t => (t.Subject, t.HtmlBody));
        }

        public Task<(string Subject, string HtmlBody)> RenderAsync(
            string templateId,
            IReadOnlyDictionary<string, string> data,
            CancellationToken cancellationToken = default
        )
        {
            if (!_templates.TryGetValue(templateId, out var template))
            {
                throw new KeyNotFoundException(templateId);
            }

            return Task.FromResult(template);
        }
    }
}
