using DataAccess.DataInterfaces;
using DataAccess.Requests;
using RestSharp;
using RestSharp.Authenticators;
using System.Security.Cryptography;
using ScriptureMemoryLibrary;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VerseAppNew.Server.Services;

public interface IEmailSenderService
{
    Task<RestResponse> SendEmail(Email email);
    Task<IResult> SendForgotUsernameEmail(ForgotUsernameRequest request);
    Task<IResult> SendPasswordResetOtp(ForgotPasswordRequest request);
}

public class Email
{
    public class To
    {
        public string Name { get; set; }
        public string EmailAddress { get; set; }

        public To(string name, string email)
        {
            Name = name;
            EmailAddress = email;
        }
    }

    public class From
    {
        public string Name { get; set; }
        public string EmailAddress { get; set; }

        public From(string name, string email)
        {
            Name = name;
            EmailAddress = email;
        }
    }

    public To ToRecipient { get; set; }
    public From FromSender { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }

    public Email(To to, From from, string subject, string body)
    {
        ToRecipient = to;
        FromSender = from;
        Subject = subject;
        Body = body;
    }
}

public sealed class EmailSenderService : IEmailSenderService
{
    private readonly IUserData userContext;
    private readonly IConfiguration config;
    private readonly IServiceProvider serviceProvider;

    public EmailSenderService(IUserData userContext, IConfiguration config, IServiceProvider serviceProvider)
    {
        this.userContext = userContext;
        this.config = config;
        this.serviceProvider = serviceProvider;
    }

    public async Task<RestResponse> SendEmail(Email email)
    {
        // Done with MailGun -- near deployment, set up mailgun, custom domain, etc

        var options = new RestClientOptions("https://api.mailgun.net")
        {
            Authenticator = new HttpBasicAuthenticator("api", config["Email:ApiKey"]
                ?? throw new Exception("Email:ApiKey not found"))
        };

        var client = new RestClient(options);
        var request = new RestRequest("/v3/sandbox46f66d0bbe7c4ab497fb9876019eab27.mailgun.org/messages", Method.Post);
        request.AlwaysMultipartFormData = true;
        request.AddParameter("from", $"Mailgun Sandbox <postmaster@sandbox46f66d0bbe7c4ab497fb9876019eab27.mailgun.org>");
        request.AddParameter("to", "Joshua Robertson <therealjoshrobertson@gmail.com>");
        request.AddParameter("subject", "Hello Joshua Robertson");
        request.AddParameter("text", "Congratulations Joshua Robertson, you just sent an email with Mailgun! You are truly awesome!");
        return await client.ExecuteAsync(request);
    }

    public async Task<IResult> SendForgotUsernameEmail(ForgotUsernameRequest request)
    {

        var fullName = request.FirstName.Trim() + " " + request.LastName.Trim();
        var usernames = (await userContext.GetUsernamesByProfile(
            request.FirstName.Trim(),
            request.LastName.Trim(),
            request.Email.Trim())).ToList();

        if (!usernames.Any()) 
            return Results.NotFound();

        var bodyLines = usernames.Select(u => $"- {u}");
        var body = $@"Hi {request.FirstName.Trim()},

                    We received a request to retrieve the username(s) associated with this email address for VerseApp.

                    The following username(s) are linked to your account:
                    {string.Join(Environment.NewLine, bodyLines)}

                    If you did not request this email, you can ignore it.

                    Sincerely,
                    The VerseApp Team";

        await SendEmail(
            new Email(
                new Email.To(fullName, request.Email.Trim()),
                new Email.From("VerseApp", Data.emailFromAddress),
                body,
                "Forgot Username"
            )
        );

        return Results.NoContent();
    }

    public async Task<IResult> SendPasswordResetOtp(ForgotPasswordRequest request)
    {
        var scope = serviceProvider.CreateScope();
        var userContext = scope.ServiceProvider.GetRequiredService<IUserData>();

        var recoveryInfo = await userContext.GetPasswordRecoveryInfo(
            request.Username.Trim(),
            request.Email.Trim());

        if (recoveryInfo is null)
            return Results.NotFound();

        var otp = RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");

        await userContext.UpsertPasswordResetToken(request.Username.Trim(), otp);

        var body = $@"Hi {request.Username.Trim()},

                    We received a request to reset the password for your VerseApp account.

                    Your OTP is: {otp}

                    This code expires in 15 minutes.";

        await SendEmail(
            new Email(
                new Email.To(request.Username, request.Email.Trim()),
                new Email.From("VerseApp", Data.emailFromAddress),
                body,
                "Reset Password"
            )
        );

        return Results.Ok();
    }
}
