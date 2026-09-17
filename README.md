# Mikepattyn.Email

.NET 10 library for rendering file-based email templates and sending them over SMTP with [MailKit](https://github.com/jstedfast/MailKit). The SMTP password can come from an environment variable or from AWS Secrets Manager.

## Projects

| Project | Role |
|---------|------|
| `Mikepattyn.Email` | Sender, file-template renderer, MailKit transport |
| `Mikepattyn.Email.Tests` | xUnit tests |

## Usage

```csharp
var sender = EmailServiceFactory.CreateFromEnvironment(templatesDirectory);

await sender.SendAsync(
    new EmailSendRequest(
        TemplateId: "contact",
        Data: new Dictionary<string, string>
        {
            ["name"] = "Alex",
            ["email"] = "alex@example.com",
        },
        To: "inbox@example.com",
        ReplyTo: "alex@example.com"
    )
);
```

Each template is two files in the templates directory:

- `{templateId}.subject.txt`
- `{templateId}.body.html`

Placeholders use `{{name}}` syntax.

## Configuration

`EmailServiceFactory.CreateFromEnvironment` reads these variables. Do not put passwords in source or in this file.

| Variable | Required | Notes |
|----------|----------|-------|
| `ZohoSmtpUser` | yes | SMTP username |
| `ZohoSmtpFromAddress` | yes | From address |
| `ZohoSmtpAppPassword` | one of | App password in the environment |
| `ZohoSmtpAppPasswordSecretArn` | one of | Secrets Manager secret that holds the app password |
| `ZohoSmtpHost` | no | Defaults to `smtppro.zoho.eu` |
| `ZohoSmtpPort` | no | Defaults to `587` |
| `ZohoSmtpFromName` | no | Defaults to `Mike Pattyn` |

Provide either `ZohoSmtpAppPassword` or `ZohoSmtpAppPasswordSecretArn`.

## Build and test

Requires the .NET 10 SDK.

```bash
dotnet test Mikepattyn.Email.slnx
```

## License

MIT
