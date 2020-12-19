using System;
using Application.Mails.Types;
using Core.Enums;
using Core.Interfaces.Mails;
using FluentEmail.Core;

namespace Application.Services
{
    public class MailBuilderService : IMailBuilderService
    {
        private readonly IServiceProvider _serviceProvider;

        public MailBuilderService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        
        public IFluentEmail BuildMail(EmailType type, string parameters)
        {
            var email = GetMailByType(type);
            return email
                .Prepare(parameters, _serviceProvider);
        }

        private BaseMail GetMailByType(EmailType type)
        {
            switch (type)
            {
                case EmailType.WelcomeMail:
                    return new WelcomeMail();
            }

            throw new Exception("Mail type not found");
        }
    }
}