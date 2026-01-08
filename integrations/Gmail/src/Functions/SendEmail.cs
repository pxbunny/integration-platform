using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Integrations.Gmail.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace Integrations.Gmail.Functions;

internal sealed record SendEmailRequest(
    string To,
    string Subject,
    string Body,
    bool IsHtml = false);

internal sealed class SendEmail(EmailSenderService emailSender, ILogger<SendEmail> logger)
{
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

        try
        {
            await emailSender.SendAsync(
                recipients,
                request.Subject,
                request.Body,
                request.IsHtml,
                cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while sending email.");
            return Results.InternalServerError();
        }

        return Results.Accepted("Email sent.");
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

}
