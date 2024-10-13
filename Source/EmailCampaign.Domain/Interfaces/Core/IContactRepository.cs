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
        Task<Contact> ActiveToggleAsync(string email);
        Task<List<Contact>> GetContactForGroupAsync();
        Task<bool> DeleteContactAsync(Guid userID);

        Task<List<ContactVM>> ImportExcel(string filePath);
        Task<List<ContactVM>> ImportCsv(string filePath);
    }
}
