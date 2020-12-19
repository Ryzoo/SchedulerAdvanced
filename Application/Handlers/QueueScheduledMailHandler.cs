using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Requests;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Handlers
{
    public class QueueScheduledMailHandler : AsyncRequestHandler<QueueScheduledMailRequest>
    {
        private const int MailRetry = 2;
        private readonly ILogger<ReadMailFileHandler> _logger;
        private readonly IMediator _mediator;

        public QueueScheduledMailHandler(ILogger<ReadMailFileHandler> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        protected override async Task Handle(QueueScheduledMailRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("SendScheduledMailHandler handled");
            _logger.LogInformation($"Try to send {request.Mail.Count}");
            
            foreach (var mail in request.Mail)
            {
                var mailSent = false;
                var mailRetry = MailRetry;

                do
                {
                    try
                    {
                        await _mediator.Send(new SendMailRequest()
                        {
                            Mail = mail
                        }, cancellationToken);
                        mailSent = true;
                    }
                    catch (Exception)
                    {
                        mailRetry--;
                    }
                    
                } while (!mailSent || mailRetry > 0);
            }
        }
    }
}