using AutoMapper;
using EmailCampaign.Domain.Entities;
using EmailCampaign.Domain.Entities.ViewModel;
using EmailCampaign.Domain.Interfaces;
using EmailCampaign.Domain.Interfaces.Core;
using EmailCampaign.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Infrastructure.Data.Repositories.Core
{
    public class GroupRepository : IGroupRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public GroupRepository(ApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;

        }
        public async Task<List<Group>> GetAllGroupAsync()
        {
            return await _dbContext.Group.ToListAsync();
        }

        public async Task<Group> GetGroupAsync(Guid id)
        {
            return await _dbContext.Group.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Group> CreateGroupAsync(GroupVM model)
        {
            Group group = new Group
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description,
                IsActive = model.IsActive,

                CreatedBy = Guid.NewGuid(),
                CreatedOn = DateTime.UtcNow,
                UpdatedBy = Guid.Empty,
                UpdatedOn = DateTime.MinValue,
                IsDeleted = false
            };

            await _dbContext.Group.AddAsync(group);
            await _dbContext.SaveChangesAsync();

            return group;
        }

        public async Task<Group> UpdateGroupAsync(Guid id, GroupVM model)
        {
            Group group = await _dbContext.Group.FirstOrDefaultAsync(p => p.Id ==id);

            if(group == null) { return null; }

            group.Name = model.Name;
            group.Description = model.Description;
            group.IsActive = model.IsActive;

            group.UpdatedBy = Guid.NewGuid();
            group.UpdatedOn = DateTime.UtcNow;

            _dbContext.Entry(group).State = EntityState.Modified;

            await _dbContext.SaveChangesAsync();

            return group;
            
        }

        public async Task<bool> DeleteGroupAsync(Guid GroupID)
        {
            Group group = await _dbContext.Group.FirstOrDefaultAsync(p => p.Id == GroupID);

            if (group != null)
            {
                group.UpdatedOn = DateTime.UtcNow;
                group.UpdatedBy = Guid.NewGuid();
                group.IsActive = false;
                group.IsDeleted = true;
                //_dbContext.User.Remove(user);

                _dbContext.Entry(group).State = EntityState.Modified;

                try
                {
                    await _dbContext.SaveChangesAsync();
                    return true;
                }
                catch (DbUpdateException ex)
                {
                    throw;
                }
            }
            return false;
        }


    }
}
