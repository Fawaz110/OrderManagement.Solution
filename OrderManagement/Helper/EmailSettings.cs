using System.Net.Mail;
using System.Net;
using Core.Entities.Order.Aggregate;

namespace OrderManagement.Helper
{
    public class EmailSettings
    {
        public static void SendEmail(string toEmail, string title, Order order)
        {
            var client = new SmtpClient("smtp.gmail.com", 587);
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential("mmohamedfawzi23@gmail.com", "vereocmapeaeeirv");

            client.Send("mmohamedfawzi23@gmail.com", toEmail, title, $"Your Order Status Has been Changed to {order.Status}");
        }
    }
}
