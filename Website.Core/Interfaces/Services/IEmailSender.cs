﻿using System.Threading.Tasks;

namespace Website.Core.Interfaces.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
