using System.Collections.Generic;
using Core.DomainModels;
using MediatR;

namespace Application.Requests
{
    public class QueueScheduledMailRequest : IRequest
    {
        public IReadOnlyCollection<ScheduledMailModel> Mail;
    }
}