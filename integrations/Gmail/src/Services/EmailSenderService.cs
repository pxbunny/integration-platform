using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Integrations.Gmail.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Integrations.Gmail.Services;

internal sealed class EmailSenderService(IOptions<SmtpOptions> smtpOptions, ILogger<EmailSenderService> logger)
{
    private readonly SmtpOptions _options = smtpOptions.Value;

    public async Task SendAsync(
        IEnumerable<MailboxAddress> recipients,
        string subject,
        string body,
        bool isHtml,
        CancellationToken cancellationToken)
    {
        var message = CreateMessage(recipients, subject, body, isHtml);

        using var client = new SmtpClient();

        var socketOption = _options.UseSsl
            ? SecureSocketOptions.SslOnConnect
            : SecureSocketOptions.StartTlsWhenAvailable;

        await client.ConnectAsync(
            _options.Host,
            _options.Port,
            socketOption,
            cancellationToken);

        var user = _options.User;
        var password = _options.Password;

        if (!string.IsNullOrWhiteSpace(user) && !string.IsNullOrWhiteSpace(password))
            await client.AuthenticateAsync(user, password, cancellationToken);

        await client.SendAsync(message, cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);

        logger.LogInformation("Email sent to {RecipientCount} recipients.", message.To.Count);
    }

    private MimeMessage CreateMessage(
        IEnumerable<MailboxAddress> recipients,
        string subject,
        string body,
        bool isHtml)
    {
        var message = new MimeMessage();

        message.From.Add(new MailboxAddress(_options.FromName ?? string.Empty, _options.From));
        message.To.AddRange(recipients);
        message.Subject = subject;
        message.Body = new TextPart(isHtml ? "html" : "plain") { Text = body };

        return message;
    }
}
