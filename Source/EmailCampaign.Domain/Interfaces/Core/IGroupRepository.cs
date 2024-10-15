using EmailCampaign.Domain.Entities.ViewModel;
using EmailCampaign.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Domain.Interfaces.Core
{
    public interface IGroupRepository
    {
        Task<List<Group>> GetAllGroupAsync();
        Task<Group> GetGroupAsync(Guid id);
        Task<Group> CreateGroupAsync(GroupVM model);
        Task<Group> UpdateGroupAsync(Guid id, GroupVM model);
        Task<Group> ActiveToggleAsync(Guid id);
        Task<Group> DeleteGroupAsync(Guid GroupID);


        Task<ContactGroupVM> AddContactsGroupAsync(ContactGroupVM contactGroup);
        Task<ContactGroupVM> GetContactForGroupAsync(Guid groupId);
        Task<ContactGroupVM> UpdateContactsGroupAsync(ContactGroupVM contactGroup);
        Task<ContactGroup> ContacSubscribeStatus(Guid contactId, Guid groupId);
    }
}
