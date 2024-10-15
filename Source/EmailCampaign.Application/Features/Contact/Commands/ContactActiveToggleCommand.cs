using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Application.Features.Contact.Commands
{
    public class ContactActiveToggleCommand : IRequest<Domain.Entities.Contact>
    {
        public string email { get; set; }
    
    }
}
