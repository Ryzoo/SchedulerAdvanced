using Core.DomainModels;
using MediatR;

namespace Application.Requests
{
    public class SendMailRequest : IRequest
    {
        public ScheduledMailModel Mail;
    }
}