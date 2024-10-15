using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using AutoMapper;
using CsvHelper;
using EmailCampaign.Domain.Entities;
using EmailCampaign.Domain.Entities.ViewModel;
using EmailCampaign.Domain.Interfaces;
using EmailCampaign.Domain.Interfaces.Core;
using EmailCampaign.Domain.Services;
using EmailCampaign.Infrastructure.Data.Context;
using EmailCampaign.Infrastructure.Data.Services;
using EmailCampaign.Infrastructure.Data.Services.LogsService;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OfficeOpenXml;

namespace EmailCampaign.Infrastructure.Data.Repositories.Core
{
    public class ContactRepository : IContactRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ErrorLogFilter _errorLogFilter;
        private readonly IUserContextService _userContextService;
        private readonly INotificationRepository _notificationRepository;
        private readonly IMapper _mapper;
        public ContactRepository(ApplicationDbContext dbContext, IMapper mapper, IUserContextService userContextService, ErrorLogFilter errorLogFilter, INotificationRepository notificationRepository)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _userContextService = userContextService;
            _notificationRepository = notificationRepository;
            _errorLogFilter = errorLogFilter;

        }

        public async Task<List<Contact>> GetAllContactAsync()
        {
            return await _dbContext.Contact.Where(p => p.IsDeleted == false).ToListAsync();
        }

        public async Task<List<Contact>> GetContactForGroupAsync()
        {
            return await _dbContext.Contact.Where(p => p.IsDeleted == false & p.IsActive == true).ToListAsync();
        }

        public async Task<bool> CheckRegisteredEmailAsync(string email)
        {
            return await _dbContext.Contact.AnyAsync(p => p.IsDeleted == false & p.Email == email);
        }

        public async Task<Contact> GetContactAsync(Guid id)
        {
            return await _dbContext.Contact.FirstOrDefaultAsync(p => p.Id == id);
        }


        public async Task<Contact> CreateContactAsync(ContactVM model)
        {
            Contact contact = new Contact
            {
                Id = Guid.NewGuid(),
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                CompanyName = model.CompanyName,
                IsActive = model.IsActive,

                CreatedBy = Guid.Parse(_userContextService.GetUserId()),
                CreatedOn = DateTime.UtcNow,
                UpdatedBy = Guid.Empty,
                UpdatedOn = DateTime.MinValue,
                IsDeleted = false,
            };

            await _dbContext.Contact.AddAsync(contact);

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                await _errorLogFilter.OnException(ex);
            }

            if (contact != null)
            {
                var notification = new Notification
                {
                    Header = "New Contact created.",
                    Body = "User " + _userContextService.GetUserName() + " Created new Contact with " + contact.FirstName + " " + contact.LastName + " (" + contact.Id + ").",
                    PerformOperationBy = Guid.Parse(_userContextService.GetUserId()),
                    PerformOperationFor = Guid.Parse(_userContextService.GetUserId()),
                    RedirectUrl = "/Contact"
                };

                await _notificationRepository.CreateNotificationAsync(notification);
            }


            return contact;
        }

        public async Task<Contact> UpdateContactAsync(Guid id, ContactVM model)
        {
            Contact contact = await _dbContext.Contact.FirstOrDefaultAsync(p => p.Id == id);

            if (contact == null)
            {
                return null;
            }

            contact.FirstName = model.FirstName;
            contact.LastName = model.LastName;
            contact.Email = model.Email;
            contact.CompanyName = model.CompanyName;
            contact.IsActive = model.IsActive;

            contact.UpdatedOn = DateTime.UtcNow;
            contact.UpdatedBy = Guid.Parse(_userContextService.GetUserId());


            _dbContext.Entry(contact).State = EntityState.Modified;

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                await _errorLogFilter.OnException(ex);
            }

            if (contact != null)
            {
                var notification = new Notification
                {
                    Header = "Contact Details updated.",
                    Body = "User " + _userContextService.GetUserName() + " update details of Contact with " + contact.FirstName + " " + contact.LastName + " (" + contact.Id + ").",
                    PerformOperationBy = Guid.Parse(_userContextService.GetUserId()),
                    PerformOperationFor = Guid.Parse(_userContextService.GetUserId()),
                    RedirectUrl = "/Contact"
                };

                await _notificationRepository.CreateNotificationAsync(notification);
            }

            return contact;

        }


        public async Task<Contact> ActiveToggleAsync(string email)
        {
            Contact contact = await _dbContext.Contact.FirstOrDefaultAsync(p => p.Email == email);

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

            if (contact != null)
            {
                var notification = new Notification
                {
                    Header = "Contact IsActive toggle event occur.",
                    Body = "Contact IsActive status is now " + contact.IsActive + "." + " and it's updated by " + _userContextService.GetUserName() + ".",
                    PerformOperationBy = Guid.Parse(_userContextService.GetUserId()),
                    PerformOperationFor = Guid.Parse(_userContextService.GetUserId()),
                    RedirectUrl = "/Contact"
                };

                await _notificationRepository.CreateNotificationAsync(notification);
            }

            return contact;

        }



        public async Task<Contact> DeleteContactAsync(Guid ID)
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
                }
                catch (DbUpdateException ex)
                {
                    throw;
                }
            }
            else
            {
                return new Contact();
            }

            if (contact != null)
            {
                var notification = new Notification
                {
                    Header = "Contact Deleted.",
                    Body = "User " + _userContextService.GetUserName() + " Deleted Contact with " + contact.FirstName + " " + contact.LastName + " (" + contact.Id + ").",
                    PerformOperationBy = Guid.Parse(_userContextService.GetUserId()),
                    PerformOperationFor = Guid.Parse(_userContextService.GetUserId()),
                    RedirectUrl = "/Contact"
                };

                await _notificationRepository.CreateNotificationAsync(notification);
            }

            return contact;
        }

        public async Task<List<ContactVM>> ImportCsv(string filePath)
        {
            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture))
            {
                List<ContactVM> records = csv.GetRecords<ContactVM>().ToList();

                foreach (var item in records)
                {
                    var contact = await CreateContactAsync(item);

                    if (contact == null)
                    {
                        return new List<ContactVM>();
                    }
                }

                return records;
            }
        }

        public async Task<List<ContactVM>> ImportExcel(string filePath)
        {
            List<ContactVM> contactVMs = new List<ContactVM>();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            FileInfo fileInfo = new FileInfo(filePath);
            using (var package = new ExcelPackage(fileInfo))
            {
                var worksheet = package.Workbook.Worksheets.FirstOrDefault();

                if (worksheet != null)
                {
                    var rowCount = worksheet.Dimension.Rows;

                    for (int i = 2; i <= rowCount; i++)
                    {
                        var data = new ContactVM
                        {
                            Id = Guid.NewGuid(),
                            FirstName = worksheet.Cells[i, 2].Value.ToString(),
                            LastName = worksheet.Cells[i, 3].Value.ToString(),
                            Email = worksheet.Cells[i, 4].Value.ToString(),
                            CompanyName = worksheet.Cells[i, 5].Value.ToString(),
                            IsActive = bool.Parse(worksheet.Cells[i, 6].Value.ToString())
                        };

                        contactVMs.Add(data);
                    }

                    foreach (var item in contactVMs)
                    {
                        var contact = await CreateContactAsync(item);

                        if (contact == null)
                        {
                            return new List<ContactVM>();
                        }
                    }

                    return contactVMs;
                }

                return new List<ContactVM>();
            }

        }
    }
}
