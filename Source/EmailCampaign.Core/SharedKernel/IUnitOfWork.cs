using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Core.SharedKernel
{
    public interface IUnitOfWork : IDisposable
    {
        Task SaveChangesAsync();
    }
}
