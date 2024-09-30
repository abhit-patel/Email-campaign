using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using EmailCampaign.Domain.Entities;
using EmailCampaign.Domain.Entities.ViewModel;
using EmailCampaign.Domain.Interfaces.Core;
using EmailCampaign.Infrastructure.Data.Context;
using EmailCampaign.Infrastructure.Data.Services;
using Microsoft.EntityFrameworkCore;

namespace EmailCampaign.Infrastructure.Data.Repositories.Core
{
    public class ContactRepository : IContactRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IUserContextService _userContextService;
        public ContactRepository(ApplicationDbContext dbContext, IMapper mapper, IUserContextService userContextService)
        {
            _dbContext = dbContext;    
            _mapper = mapper;
            _userContextService = userContextService;
        }

        public async Task<List<Contact>> GetAllContactAsync()
        {
            return await _dbContext.Contact.ToListAsync();
        }

        public async Task<Contact> GetContactAsync(Guid id)
        {
            return await _dbContext.Contact.FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task<Contact> CreateContactAsync(ContactVM model)
        {
            Contact contact = new Contact
            {
                Id = model.Id,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                CompanyName = model.CompanyName,
                IsActive = model.IsActive,

                CreatedBy = Guid.NewGuid(),
                CreatedOn = DateTime.UtcNow,
                UpdatedBy = Guid.Empty,
                UpdatedOn = DateTime.MinValue,
                IsDeleted = false,
            };

            await _dbContext.Contact.AddAsync(contact);

            await _dbContext.SaveChangesAsync();
            return contact;
        }

        public async Task<Contact> UpdateContactAsync(Guid id, ContactVM model)
        {
            Contact contact = await _dbContext.Contact.FirstOrDefaultAsync(p => p.Id == id);

            if(contact == null)
            {
                return null;
            }

            contact.FirstName = model.FirstName;
            contact.LastName = model.LastName;  
            contact.Email = model.Email;
            contact.CompanyName = model.CompanyName;
            contact.IsActive = model.IsActive;

            contact.UpdatedOn = DateTime.UtcNow;
            contact.UpdatedBy = Guid.NewGuid();

          
            _dbContext.Entry(contact).State = EntityState.Modified;
            
            await _dbContext.SaveChangesAsync();

            return contact;

        }
        public async Task<bool> DeleteContactAsync(Guid ID)
        {
            Contact contact = await _dbContext.Contact.FirstOrDefaultAsync(p => p.Id == ID);

            if (contact != null)
            {
                contact.UpdatedOn = DateTime.UtcNow;
                contact.UpdatedBy = Guid.NewGuid();
                contact.IsActive = false;
                contact.IsDeleted = true;
                //_dbContext.User.Remove(user);

                _dbContext.Entry(contact).State = EntityState.Modified;

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

        public async Task<Contact> ActiveToggleAsync(Guid contactID)
        {
            Contact contact = await _dbContext.Contact.FirstOrDefaultAsync(p => p.Id == contactID);

            if (contact != null)
            {
                if (contact.IsActive)
                {
                    contact.IsActive = false;
                }
                else
                {
                    contact.IsActive = true;
                }
            }

            contact.UpdatedOn = DateTime.UtcNow;
            contact.UpdatedBy = Guid.Parse(_userContextService.GetUserId());


            _dbContext.Entry(contact).State = EntityState.Modified;
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw;
            }

            return contact;
        }
    }
}
