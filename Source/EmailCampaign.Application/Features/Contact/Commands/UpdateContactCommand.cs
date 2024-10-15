using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Application.Features.Contact.Commands
{
    public class UpdateContactCommand : IRequest<Domain.Entities.Contact>
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string CompanyName { get; set; }
        public bool IsActive { get; set; }
    }
}
