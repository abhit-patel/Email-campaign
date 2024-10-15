using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Application.Features.User.Commands
{
    public class ChangePasswordCommand : IRequest<Domain.Entities.User>
    {
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}