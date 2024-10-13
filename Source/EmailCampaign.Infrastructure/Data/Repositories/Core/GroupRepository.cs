using AutoMapper;
using EmailCampaign.Domain.Entities;
using EmailCampaign.Domain.Entities.ViewModel;
using EmailCampaign.Domain.Interfaces;
using EmailCampaign.Domain.Interfaces.Core;
using EmailCampaign.Infrastructure.Data.Context;
using EmailCampaign.Infrastructure.Data.Services;
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
        private readonly IUserContextService _userContextService;


        public GroupRepository(ApplicationDbContext dbContext, IMapper mapper, IUserContextService userContextService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _userContextService = userContextService;

        }
        public async Task<List<Group>> GetAllGroupAsync()
        {
            return await _dbContext.Group.Where(p=> p.IsDeleted == false).ToListAsync();
        }

        public async Task<Group> GetGroupAsync(Guid id)
        {
            return await _dbContext.Group.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Group> CreateGroupAsync(GroupVM model)
        {
            Group group = new Group
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                Description = model.Description,
                IsActive = model.IsActive,

                CreatedBy = Guid.Parse(_userContextService.GetUserId()),
                CreatedOn = DateTime.UtcNow,
                UpdatedBy = Guid.Empty,
                UpdatedOn = DateTime.MinValue,
                IsDeleted = false
            };

            await _dbContext.Group.AddAsync(group);
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw;
            }

            return group;
        }

        public async Task<Group> UpdateGroupAsync(Guid id, GroupVM model)
        {
            Group group = await _dbContext.Group.FirstOrDefaultAsync(p => p.Id ==id);

            if(group == null) { return null; }

            group.Name = model.Name;
            group.Description = model.Description;
            group.IsActive = model.IsActive;

            group.UpdatedBy = Guid.Parse(_userContextService.GetUserId());
            group.UpdatedOn = DateTime.UtcNow;

            _dbContext.Entry(group).State = EntityState.Modified;

            await _dbContext.SaveChangesAsync();

            return group;
            
        }


        public async Task<Group> ActiveToggleAsync(Guid id)
        {
            Group group = await _dbContext.Group.FirstOrDefaultAsync(p => p.Id == id);

            if (group != null)
            {
                if (group.IsActive)
                {
                    group.IsActive = false;
                }
                else
                {
                    group.IsActive = true;
                }
            }

            group.UpdatedOn = DateTime.UtcNow;
            group.UpdatedBy = Guid.Parse(_userContextService.GetUserId());


            _dbContext.Entry(group).State = EntityState.Modified;
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw;
            }

            return group;

        }


        public async Task<Group> DeleteGroupAsync(Guid GroupID)
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
                    return group;
                }
                catch (DbUpdateException ex)
                {
                    throw;
                }
            }
            return new Group();
        }




        public async Task<ContactGroup> AddContactsGroupAsync(Guid groupId, Guid contactId, bool isSelected)
        {
            var model = new ContactGroup
            {
                Id = Guid.NewGuid(),
                ContactId = contactId,
                GroupID = groupId,
                IsSubscribed = isSelected
            };

            await _dbContext.ContactGroup.AddAsync(model);

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw;
            }

            return model;
        }


        public async Task<ContactGroupVM> GetContactForGroupAsync( Guid groupId)
        {
            var group = await _dbContext.Group.AsNoTracking().FirstOrDefaultAsync( p => p.Id ==groupId);

            var contactgroup = await _dbContext.ContactGroup.AsNoTracking().Where(p => p.GroupID == groupId && p.IsSubscribed == true).Include(p => p.Contact).ToListAsync();

            var model = new ContactGroupVM
            {
                GroupID = groupId,
                Contacts = contactgroup.Select(p => new ContactSelection
                {
                    ContactId = p.ContactId,
                    ContactName = $"{p.Contact.FirstName} {p.Contact.LastName} ",
                    Email = $"{p.Contact.Email}",
                    CompanyName = $"{p.Contact.CompanyName}",
                    IsSelected = p.IsSubscribed
                }).ToList(),
            };

            return model;
        }
        
        public async Task<ContactGroup> UpdateContactsGroupAsync(Guid groupId, Guid contactId, bool isSelected)
        {
            ContactGroup contactGroup = await _dbContext.ContactGroup.AsNoTracking().FirstOrDefaultAsync(p => p.ContactId == contactId && p.GroupID == groupId);

            if (contactGroup == null)
            {
                return new ContactGroup();
            }

            var model = new ContactGroup
            {
                Id = contactGroup.Id,
                ContactId = contactId,
                GroupID = groupId,
                IsSubscribed = isSelected
            };

            _dbContext.Entry(model).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw;
            }

            return model;
        }


        public async Task<ContactGroup> ContacSubscribeStatus(Guid contactId, Guid groupId)
        {
            ContactGroup model = await _dbContext.ContactGroup.FirstOrDefaultAsync(p => p.ContactId == contactId && groupId == groupId);

            if (model == null)
            {
                return new ContactGroup();
            }

            model.IsSubscribed = false;

            _dbContext.Entry(model).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw;
            }

            return model;
        }
    }
}
