using AutoMapper;
using EmailCampaign.Application.Features.RoleWithPermission.Commands;
using EmailCampaign.Application.Interfaces;
using EmailCampaign.Domain.Entities;
using EmailCampaign.Domain.Entities.ViewModel;
using EmailCampaign.Domain.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Application.Features.RoleWithPermission.Handlers
{
    public class CreateRolePermissionCommandHandler : IRequestHandler<CreateRolePermissionCommand, RolePermissionVM>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly IUserContextService _userContextService;
        private readonly IMapper _mapper;


        public CreateRolePermissionCommandHandler(IApplicationDbContext dbContext, IUserContextService userContextService, IMapper mapper)
        {
            _dbContext = dbContext;
            _userContextService = userContextService;
            _mapper = mapper;
        }
        public async Task<RolePermissionVM> Handle(CreateRolePermissionCommand request, CancellationToken cancellationToken)
        {
            Role newRole = new Role
            {
                Id = Guid.NewGuid(),
                Name = request.RoleName,
                CreatedBy = Guid.Parse(_userContextService.GetUserId()),
                CreatedOn = DateTime.Now,
                UpdatedBy = Guid.Empty,
                UpdatedOn = DateTime.MinValue,
                IsDeleted = false,
            };

            if (newRole == null)
            {
                return new RolePermissionVM();
            }

            await _dbContext.Role.AddAsync(newRole);


            foreach (var permissions in request.PermissionList)
            {
                RolePermissionDBVM dbModel = _mapper.Map<RolePermissionDBVM>(permissions);
                dbModel.RoleId = newRole.Id;

                RolePermission newItem = new RolePermission
                {
                    Id = Guid.NewGuid(),
                    RoleId = dbModel.RoleId,
                    PermissionId = dbModel.PermissionId,
                    IsAddEdit = dbModel.IsAddEdit,
                    IsDelete = dbModel.IsDelete,
                    IsView = dbModel.IsView,
                    CreatedBy = Guid.Parse(_userContextService.GetUserId()),
                    CreatedOn = DateTime.Now,
                    UpdatedBy = Guid.Empty,
                    UpdatedOn = DateTime.MinValue,
                    IsDeleted = false,
                };

                if(newItem == null) { return new RolePermissionVM(); }

                await _dbContext.RolePermission.AddAsync(newItem);

            }


            await _dbContext.SaveChangesAsync();

            RolePermissionVM rolePermissionVM = _mapper.Map<RolePermissionVM>(request);

            return rolePermissionVM;
        }
    }
}
