﻿using EmailCampaign.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Domain.Interfaces
{
    public interface ILoginRepository
    {
        Task<User> GetUserByEmailAsync(string email);
    }
}
