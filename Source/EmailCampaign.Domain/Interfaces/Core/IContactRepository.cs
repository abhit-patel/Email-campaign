using EmailCampaign.Domain.Entities.ViewModel;
using EmailCampaign.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.Contracts;

namespace EmailCampaign.Domain.Interfaces.Core
{
    public interface IContactRepository
    {
        Task<List<Contact>> GetAllContactAsync();
        Task<Contact> GetContactAsync(Guid id);
        Task<Contact> CreateContactAsync(ContactVM model);
        Task<Contact> UpdateContactAsync(Guid id, ContactVM model);
        Task<bool> DeleteContactAsync(Guid userID);
        Task<Contact> ActiveToggleAsync(Guid userID);
    }
}
