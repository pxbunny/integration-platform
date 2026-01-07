namespace Integrations.Gmail.Options;

internal sealed class SmtpOptions
{
    public const string SectionName = "Smtp";

    public string Host { get; init; } = null!;

    public int Port { get; init; } = 587;

    public bool UseSsl { get; init; }

    public string From { get; init; } = null!;

    public string? FromName { get; init; }

    public string? User { get; init; }

    public string? Password { get; init; }
}
