using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechHub.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendEmail(string recipient,string subject,string body);
    }
}
