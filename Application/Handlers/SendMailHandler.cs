using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Requests;
using Core.Interfaces.Mails;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Handlers
{
    public class SendMailHandler : AsyncRequestHandler<SendMailRequest>
    {
        private readonly ILogger<SendMailHandler> _logger;
        private readonly IMailBuilderService _mailBuilderService;

        public SendMailHandler(ILogger<SendMailHandler> logger, IMailBuilderService mailBuilderService)
        {
            _logger = logger;
            _mailBuilderService = mailBuilderService;
        }
        
        protected override async Task Handle(SendMailRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Try to send mail");

            try
            {
                await _mailBuilderService
                    .BuildMail(request.Mail.EmailType, request.Mail.Params)
                    .SendAsync();
                _logger.LogInformation($"Mail sent.");
            }
            catch (Exception e)
            {
                _logger.LogInformation($"Not sent: {e.Message}");
                throw new Exception($"Not sent: {e.Message}");
            }
        }
    }
}