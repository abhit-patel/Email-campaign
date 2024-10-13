using EmailCampaign.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Domain.Interfaces.Log
{
    public interface IErrorLogRepository
    {
        Task ErrorLogAsync(ErrorLog log);

        List<ErrorLog> GetErrorLog();
    }
}
