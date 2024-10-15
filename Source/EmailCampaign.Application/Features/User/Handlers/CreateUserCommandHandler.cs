using EmailCampaign.Application.Features.User.Commands;
using EmailCampaign.Application.Interfaces;
using EmailCampaign.Core.SharedKernel;
using EmailCampaign;
using EmailCampaign.Domain.Entities.ViewModel;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;
using EmailCampaign.Domain.Services;
using EmailCampaign.Domain.Interfaces;
using EmailCampaign.Domain.Entities;

namespace EmailCampaign.Application.Features.User.Handlers
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Domain.Entities.User>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IUserContextService _userContextService;
        private readonly IAuthRepository _authRepository;
        private readonly IUnitOfWork _unitOfWork;


        public CreateUserCommandHandler(IApplicationDbContext dbContext, IUnitOfWork unitOfWork, PasswordHasher passwordHasher, IAuthRepository authRepository, IUserContextService userContextService)
        {
            _dbContext = dbContext;
            _unitOfWork = unitOfWork;
            _authRepository = authRepository;
            _userContextService = userContextService;
        }


        public async Task<Domain.Entities.User> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            HashedPassVM items = await _authRepository.RegisterUser(request.Email, request.Password);

            var user = new Domain.Entities.User();

            user.ID = new Guid();
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Email = request.Email;
            user.BirthDate = request.BirthDate;
            user.Password = request.Password;
            user.SaltKey = items.saltKey;
            user.HashPassword = items.hashedPassword;
            user.ProfilePicture = "/ProfilePics/profile-user.png";
            user.RoleId = request.RoleId;
            user.IsActive = request.IsActive;
            user.IsSuperAdmin = false;
            user.ResetpasswordCode = "";
            user.CreatedBy = Guid.Parse(_userContextService.GetUserId());
            user.CreatedOn = DateTime.UtcNow;
            user.UpdatedBy = Guid.Empty;
            user.UpdatedOn = DateTime.MinValue;
            user.IsDeleted = false;

            await _dbContext.User.AddAsync(user);

            await _dbContext.SaveChangesAsync();

            //await _unitOfWork.SaveChangesAsync();

            return user;
        }
    }
}
