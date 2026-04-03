using System.Net;
using System.Net.Mail;

namespace StockQuoteAlert.Services
{
    public class EmailService
    {
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _usuario;
        private readonly string _senha;
        private readonly bool _enableSsl;

        public EmailService(string host, int port, string usuario, string senha, bool enableSsl)
        {
            _smtpHost = host;
            _smtpPort = port;
            _usuario = usuario;
            _senha = senha;
            _enableSsl = enableSsl;
        }

        public void Send(string destino, string assunto, string corpo)
        {
            using var smtp = new SmtpClient(_smtpHost, _smtpPort);
            smtp.EnableSsl = _enableSsl;
            smtp.Credentials = new NetworkCredential(_usuario, _senha);

            var msg = new MailMessage(_usuario, destino, assunto, corpo);
            smtp.Send(msg);
        }
    }
}