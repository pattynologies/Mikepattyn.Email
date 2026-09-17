using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;

namespace Mikepattyn.Email;

internal interface ISecretReader
{
    string ReadSecret(string secretArn);
}

internal sealed class SecretsManagerSecretReader(IAmazonSecretsManager? secretsManager = null) : ISecretReader
{
    private readonly IAmazonSecretsManager _secretsManager = secretsManager ?? new AmazonSecretsManagerClient();

    public string ReadSecret(string secretArn)
    {
        var response = _secretsManager
            .GetSecretValueAsync(new GetSecretValueRequest { SecretId = secretArn })
            .GetAwaiter()
            .GetResult();

        if (string.IsNullOrWhiteSpace(response.SecretString))
        {
            throw new InvalidOperationException($"Secret {secretArn} has no string value.");
        }

        return response.SecretString;
    }
}
