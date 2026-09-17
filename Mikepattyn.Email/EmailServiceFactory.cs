namespace Mikepattyn.Email;

public static class EmailServiceFactory
{
    public static IEmailSender CreateFromEnvironment(
        string templatesDirectory,
        ISmtpTransport? smtpTransport = null,
        ITemplateRenderer? templateRenderer = null
    )
    {
        var configuration = ZohoConfigurationResolver.Resolve();
        var transport = smtpTransport ?? new MailKitSmtpTransport(configuration);
        var renderer = templateRenderer ?? new BundledTemplateRenderer(templatesDirectory);

        return new EmailSender(
            renderer,
            transport,
            fromAddress: ZohoConfigurationResolver.GetRequiredEnvironmentVariable("ZohoSmtpFromAddress"),
            fromDisplayName: Environment.GetEnvironmentVariable("ZohoSmtpFromName")
                ?? "Mike Pattyn"
        );
    }
}

internal static class ZohoConfigurationResolver
{
    internal static ZohoEmailConfiguration Resolve(ISecretReader? secretReader = null)
    {
        var directPassword = Environment.GetEnvironmentVariable("ZohoSmtpAppPassword");
        var password = !string.IsNullOrWhiteSpace(directPassword)
            ? directPassword
            : ReadSecret("ZohoSmtpAppPasswordSecretArn", secretReader);

        return new ZohoEmailConfiguration
        {
            Host = Environment.GetEnvironmentVariable("ZohoSmtpHost") ?? "smtppro.zoho.eu",
            Port = int.TryParse(Environment.GetEnvironmentVariable("ZohoSmtpPort"), out var port)
                ? port
                : 587,
            User = GetRequiredEnvironmentVariable("ZohoSmtpUser"),
            Password = password,
        };
    }

    private static string ReadSecret(string environmentVariableName, ISecretReader? secretReader)
    {
        var secretArn = Environment.GetEnvironmentVariable(environmentVariableName);
        if (string.IsNullOrWhiteSpace(secretArn))
        {
            throw new InvalidOperationException(
                $"Configure ZohoSmtpAppPassword or {environmentVariableName}."
            );
        }

        secretReader ??= new SecretsManagerSecretReader();
        return secretReader.ReadSecret(secretArn);
    }

    internal static string GetRequiredEnvironmentVariable(string name)
    {
        var value = Environment.GetEnvironmentVariable(name);
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"Environment variable {name} is required.");
        }

        return value;
    }
}
