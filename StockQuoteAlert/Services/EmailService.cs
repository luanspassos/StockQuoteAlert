using System.Net;
using System.Net.Mail;

namespace StockQuoteAlert.Services;
public class EmailService
{
    private readonly string _smtpHost;
    private readonly int _smtpPort;
    private readonly string _user;
    private readonly string _password;
    private readonly bool _enableSsl;

    public EmailService(string host, int port, string user, string password, bool enableSsl)
    {
        _smtpHost = host;
        _smtpPort = port;
        _user = user;
        _password = password;
        _enableSsl = enableSsl;
    }

    public void Send(string destination, string subject, string body)
    {
        using var smtp = new SmtpClient(_smtpHost, _smtpPort);
        smtp.EnableSsl = _enableSsl;
        smtp.Credentials = new NetworkCredential(_user, _password);

        var msg = new MailMessage(_user, destination, subject, body);
        smtp.Send(msg);
    }
}