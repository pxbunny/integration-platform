using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Integrations.Gmail.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Integrations.Gmail.Functions;

internal sealed record SendEmailRequest(
    string To,
    string Subject,
    string Body,
    bool IsHtml = false);

internal sealed class SendEmail(IOptions<SmtpOptions> smtpOptions, ILogger<SendEmail> logger)
{
    private readonly SmtpOptions _options = smtpOptions.Value;

    [Function("SendEmail")]
    public async Task<IResult> RunAsync(
        [HttpTrigger("post", Route = "send-email")] HttpRequest _,
        [FromBody] SendEmailRequest request,
        CancellationToken cancellationToken)
    {
        if (!TryParseRecipients(request.To, out var recipients))
        {
            logger.LogError("Invalid email recipients.");
            return TypedResults.BadRequest();
        }

        var message = CreateMessage(request, recipients);

        try
        {
            using var client = await GetAuthenticatedClient(cancellationToken);
            await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while sending email.");
            return TypedResults.InternalServerError();
        }

        return TypedResults.Accepted("Email sent.");
    }

    private static bool TryParseRecipients(string value, out List<MailboxAddress> recipients)
    {
        recipients = [];

        const StringSplitOptions options =
            StringSplitOptions.RemoveEmptyEntries |
            StringSplitOptions.TrimEntries;

        var parts = value.Split([',', ';'], options);

        foreach (var part in parts)
        {
            if (!MailboxAddress.TryParse(part, out var address))
                return false;

            recipients.Add(address);
        }

        return recipients.Count > 0;
    }

    private MimeMessage CreateMessage(SendEmailRequest request, IEnumerable<MailboxAddress> recipients)
    {
        var (_, subject, body, isHtml) = request;

        var message = new MimeMessage();

        message.From.Add(new MailboxAddress(_options.FromName ?? string.Empty, _options.From));
        message.To.AddRange(recipients);
        message.Subject = subject;
        message.Body = new TextPart(isHtml ? "html" : "plain") { Text = body };

        return message;
    }

    private async Task<SmtpClient> GetAuthenticatedClient(CancellationToken cancellationToken = default)
    {
        var client = new SmtpClient();

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

        return client;
    }
}
