using AutoMapper;
using EmailCampaign.Domain.Entities;
using EmailCampaign.Domain.Entities.ViewModel;
using EmailCampaign.Domain.Interfaces;
using EmailCampaign.Domain.Interfaces.Core;
using EmailCampaign.Domain.Services;
using EmailCampaign.Infrastructure.Data.Context;
using EmailCampaign.Infrastructure.Data.Services;
using EmailCampaign.Infrastructure.Data.Services.LogsService;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Infrastructure.Data.Repositories.Core
{
    public class GroupRepository : IGroupRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ErrorLogFilter _errorLogFilter;
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContextService;
        private readonly INotificationRepository _notificationRepository;


        public GroupRepository(ApplicationDbContext dbContext, IMapper mapper, IUserContextService userContextService, INotificationRepository notificationRepository, ErrorLogFilter errorLogFilter)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _userContextService = userContextService;
            _notificationRepository = notificationRepository;
            _errorLogFilter = errorLogFilter;
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
                await _errorLogFilter.OnException(ex);
            }

            if (group != null)
            {
                var notification = new Notification
                {
                    Header = "New Group created.",
                    Body = "User " + _userContextService.GetUserName() + " Created new Contact group with " + group.Name + " (" + group.Id + ").",
                    PerformOperationBy = Guid.Parse(_userContextService.GetUserId()),
                    PerformOperationFor = Guid.Parse(_userContextService.GetUserId()),
                    RedirectUrl = "/Group"
                };

                await _notificationRepository.CreateNotificationAsync(notification);
            }

            return group;
        }

        public async Task<Group> UpdateGroupAsync(Guid id, GroupVM model)
        {
            Group group = await _dbContext.Group.FirstOrDefaultAsync(p => p.Id ==id);

            if(group == null) { return new Group(); }

            group.Name = model.Name;
            group.Description = model.Description;
            group.IsActive = model.IsActive;

            group.UpdatedBy = Guid.Parse(_userContextService.GetUserId());
            group.UpdatedOn = DateTime.UtcNow;

            _dbContext.Entry(group).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                await _errorLogFilter.OnException(ex);
            }

            if (group != null)
            {
                var notification = new Notification
                {
                    Header = "Group Details updated.",
                    Body = "User " + _userContextService.GetUserName() + " updated details of group with " + group.Name + " (" + group.Id + ").",
                    PerformOperationBy = Guid.Parse(_userContextService.GetUserId()),
                    PerformOperationFor = Guid.Parse(_userContextService.GetUserId()),
                    RedirectUrl = "/Group"
                };

                await _notificationRepository.CreateNotificationAsync(notification);
            }

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
            else
            {
                return new Group();
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

            if (group != null)
            {
                var notification = new Notification
                {
                    Header = "Group IsActive toggle event occur.",
                    Body = "Group IsActive status is now " + group.IsActive + "." + " and it's updated by " + _userContextService.GetUserName() + ".",
                    PerformOperationBy = Guid.Parse(_userContextService.GetUserId()),
                    PerformOperationFor = Guid.Parse(_userContextService.GetUserId()),
                    RedirectUrl = "/Group"
                };

                await _notificationRepository.CreateNotificationAsync(notification);
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
                }
                catch (DbUpdateException ex)
                {
                    throw;
                }
            }
            else
            {
                return new Group();
            }

            if (group != null)
            {
                var notification = new Notification
                {
                    Header = " Group Deleted.",
                    Body = "User " + _userContextService.GetUserName() + " Deleted Group with " + group.Name + " (" + group.Id + ").",
                    PerformOperationBy = Guid.Parse(_userContextService.GetUserId()),
                    PerformOperationFor = Guid.Parse(_userContextService.GetUserId()),
                    RedirectUrl = "/Group"
                };

                await _notificationRepository.CreateNotificationAsync(notification);
            }

            return group;
        }



        public async Task<ContactGroupVM> AddContactsGroupAsync(ContactGroupVM contactGroup)
        {
            foreach(var contact in contactGroup.Contacts)
            {
                ContactGroup presentRecord = await _dbContext.ContactGroup.FirstOrDefaultAsync(p => p.ContactId == contact.ContactId && p.GroupID == contactGroup.GroupID);

                if (presentRecord != null)
                {
                    presentRecord.IsSubscribed = contact.IsSelected;

                    _dbContext.Entry(presentRecord).State = EntityState.Modified;
                }
                else
                {
                    var model = new ContactGroup
                    {
                        Id = Guid.NewGuid(),
                        ContactId = contact.ContactId,
                        GroupID = contactGroup.GroupID,
                        IsSubscribed = contact.IsSelected
                    };

                    await _dbContext.ContactGroup.AddAsync(model);
                }
            }

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                await _errorLogFilter.OnException(ex); 
            }

            return contactGroup;
        }


        public async Task<ContactGroupVM> GetContactForGroupAsync( Guid groupId)
        {
            var group = await _dbContext.Group.AsNoTracking().FirstOrDefaultAsync( p => p.Id ==groupId);

            var contactGroup = await _dbContext.ContactGroup.AsNoTracking().Where(p => p.GroupID == groupId && p.IsSubscribed == true).Include(p => p.Contact).ToListAsync();

            var model = new ContactGroupVM
            {
                GroupID = groupId,
                Contacts = contactGroup.Select(p => new ContactSelection
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
        
        public async Task<ContactGroupVM> UpdateContactsGroupAsync(ContactGroupVM contactGroupModel)
        {
            foreach (var contact in contactGroupModel.Contacts)
            {

                ContactGroup contactGroup = await _dbContext.ContactGroup.AsNoTracking().FirstOrDefaultAsync(p => p.ContactId == contact.ContactId && p.GroupID == contactGroupModel.GroupID);

                if (contactGroup == null)
                {
                    return new ContactGroupVM();
                }

                contactGroup.IsSubscribed = contact.IsSelected;

                _dbContext.Entry(contactGroup).State = EntityState.Modified;
            }

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                await _errorLogFilter.OnException(ex);
            }

            return contactGroupModel;
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
