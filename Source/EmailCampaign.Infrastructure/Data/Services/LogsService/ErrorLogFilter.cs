using EmailCampaign.Domain.Entities;
using EmailCampaign.Domain.Interfaces.Log;
using EmailCampaign.Infrastructure.Data.Repositories.Log;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using MimeKit.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Infrastructure.Data.Services.LogsService
{
    public class ErrorLogFilter 
    {
        private readonly IErrorLogRepository _errorLogRepository;
        private IHttpContextAccessor _httpContextAccessor;

        public ErrorLogFilter(IErrorLogRepository errorLogRepository , IHttpContextAccessor httpContextAccessor)
        {
            _errorLogRepository = errorLogRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task OnException(Exception exception)
        {
            //var exception = context.Exception;
            var errorLog = new ErrorLog
            {
                Id = Guid.NewGuid(),
                Message = exception.Message,
                Detail = exception.InnerException?.Message ?? "No inner Exception",
                Stacktrace = exception.StackTrace,
                InnerException = exception.InnerException?.ToString(),
                PageURL = _httpContextAccessor.HttpContext.Request.Path,
                Device = "",
                AddedOn= DateTime.UtcNow
            };
            
            await _errorLogRepository.ErrorLogAsync(errorLog);
        }
    }
}
