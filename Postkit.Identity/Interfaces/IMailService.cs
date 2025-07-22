using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Postkit.Identity.Interfaces
{
    public interface IMailService
    {
        Task SendConfirmationEmail(string toEmail, string name, string confirmationLink, string appName);
    }
}
