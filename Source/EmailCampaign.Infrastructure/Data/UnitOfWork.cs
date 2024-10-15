using EmailCampaign.Application.Interfaces;
using EmailCampaign.Core.SharedKernel;
using EmailCampaign.Domain.Interfaces.Log;
using EmailCampaign.Infrastructure.Data.Context;
using EmailCampaign.Infrastructure.Data.Services.LogsService;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ErrorLogFilter _errorLogFilter;
        private readonly IMediator _mediator;
        public UnitOfWork(IMediator mediator, ApplicationDbContext dbContext , ErrorLogFilter errorLogFilter)
        {
            _mediator = mediator; 
            _dbContext = dbContext;
            _errorLogFilter = errorLogFilter;
        }
        
        public async Task SaveChangesAsync()
        {
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await _errorLogFilter.OnException(ex);
            }
        }


        #region IDisposable

        // To detect redundant calls.
        private bool _disposed;

        // Public implementation of Dispose pattern callable by consumers.
        ~UnitOfWork() => Dispose(false);

        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            // Dispose managed state (managed objects).
            if (disposing)
            {
              
            }

            _disposed = true;
        }
        #endregion
    }
}
