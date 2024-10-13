using AutoMapper;
using EmailCampaign.Domain.Entities.ViewModel;
using EmailCampaign.Domain.Entities;
using EmailCampaign.Domain.Interfaces;
using EmailCampaign.Infrastructure.Data.Services;
using Microsoft.AspNetCore.Mvc;
using EmailCampaign.Domain.Interfaces.Core;
using Microsoft.EntityFrameworkCore;
using EmailCampaign.Infrastructure.Data.Repositories;
using System.Collections.Generic;
using System.Xml.Linq;
using EmailCampaign.Infrastructure.Data.Services.LogsService;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EmailCampaign.WebApplication.Controllers.Core
{
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class ContactController : Controller
    {
        private readonly IContactRepository _contactRepository;
        private readonly IMapper _mapper;

        public ContactController(IContactRepository contactRepository, IMapper mapper)
        {
            _contactRepository = contactRepository;
            _mapper = mapper;
        }

        //[CustomAuthorize("User/Index", typeof(IUserService))]
        public async Task<IActionResult> Index()
        {
            List<Contact> contactList = await _contactRepository.GetAllContactAsync();

            return View(contactList);
        }


        public async Task<IActionResult> GetContactById(Guid id)
        {
            Contact contact = await _contactRepository.GetContactAsync(id);

            ContactVM contactVM = _mapper.Map<ContactVM>(contact);

            //await LoadRoles();

            return View("UpdateContact", contactVM);
        }

        [HttpGet]
        public async Task<IActionResult> AddContact()
        {
            //await LoadRoles();

            return View();
        }

        [HttpPost]
        [ActionName("AddContact")]
        public async Task<IActionResult> Post(ContactVM model)
        {
            var contact = await _contactRepository.CreateContactAsync(model);

            if (contact == null)
            {
                TempData["ErrorMessage"] = "Failed to add contact. Please try again.";
                return RedirectToAction("Index");
            }

            var activityLogAttribute = new ActivityLogAttribute("Create", "New Contact created with name {0} Created by {1}", contact.FirstName + " " + contact.LastName + " (" + contact.Id + ")");
            var context = new ActionExecutingContext(
            new ActionContext(HttpContext, RouteData, new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()),
            new List<IFilterMetadata>(),
            new Dictionary<string, object> { },
            this
            );
            await activityLogAttribute.LogActivityAsync(context);


            TempData["SuccessMessage"] = "Contact successfully added!";

            return RedirectToAction("Index");
        }



        [HttpPost]
        [ActionName("ImportFile")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return Content("File not selected");
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/ContactsFile", file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            if (Path.GetExtension(file.FileName).Equals(".csv", StringComparison.OrdinalIgnoreCase))
            {
                List<ContactVM> scvData = await _contactRepository.ImportCsv(filePath);

                foreach (var item in scvData)
                {
                    var contact = await _contactRepository.CreateContactAsync(item);

                    if (contact == null)
                    {
                        TempData["ErrorMessage"] = "import file process failed !!";
                        return RedirectToAction("AddContact");
                    }

                    var activityLogAttribute = new ActivityLogAttribute("Create", "New Contact created by CSV file with name {0} Created by {1}", contact.FirstName + " "+ contact.LastName + " (" + contact.Id + ")");
                    var context = new ActionExecutingContext(
                    new ActionContext(HttpContext, RouteData, new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()),
                    new List<IFilterMetadata>(),
                    new Dictionary<string, object> { },
                    this
                    );
                    await activityLogAttribute.LogActivityAsync(context);
                }
            }
            else if (Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                List<ContactVM> excelData = await _contactRepository.ImportExcel(filePath);

                foreach (var item in excelData)
                {
                    var contact = await _contactRepository.CreateContactAsync(item);

                    if (contact == null)
                    {
                        TempData["ErrorMessage"] = "Import file process failed !!";
                        return RedirectToAction("AddContact");
                    }

                    var activityLogAttribute = new ActivityLogAttribute("Create", "New Contact created by Excel file with name {0} Created by {1}", contact.FirstName + " " + contact.LastName + " (" + contact.Id + ")");
                    var context = new ActionExecutingContext(
                    new ActionContext(HttpContext, RouteData, new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()),
                    new List<IFilterMetadata>(),
                    new Dictionary<string, object> { },
                    this
                    );
                    await activityLogAttribute.LogActivityAsync(context);
                }
            }


            TempData["SuccessMessage"] = "file successfully imported.";
            return RedirectToAction("Index");
        }


        [HttpPost]
        public async Task<IActionResult> Update(Guid id, ContactVM contactModel)
        {

            var contact = await _contactRepository.UpdateContactAsync(id, contactModel);

            if (contact == null)
            {

            }
            return RedirectToAction("Index");
        }


        [ActionName("IsActiveToggle")]
        public async Task<IActionResult> IsActiveToggleAsync(string email)
        {
            var contact = await _contactRepository.ActiveToggleAsync(email);

            if (contact == null)
            {
                TempData["ErrorMessage"] = "Status failed to change.";
                return RedirectToAction("AddUser");
            }

            var activityLogAttribute = new ActivityLogAttribute("IsActive toggle", "Active status changed of user {0} Updated by {1}", contact.FirstName + " " + contact.LastName + " (" + contact.Id + ")");
            var context = new ActionExecutingContext(
            new ActionContext(HttpContext, RouteData, new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()),
            new List<IFilterMetadata>(),
            new Dictionary<string, object> { },
            this
            );
            await activityLogAttribute.LogActivityAsync(context);

            TempData["SuccessMessage"] = "Status has been toggled successfully.";
            return RedirectToAction("Index");
        }



        public async Task<IActionResult> Delete(Guid id)
        {
            var isDeleteed = await _contactRepository.DeleteContactAsync(id);

            if (!isDeleteed)
            {
                TempData["ErrorMessage"] = "Contact delete process failed.";
                return RedirectToAction("Index");
            }



            TempData["SuccessMessage"] = "Contact successfully deleted.";
            return RedirectToAction("Index");
        }



        //private async Task LoadGroup()
        //{
        //    var roles = await _contactRepository.GetRolesAsSelectListItemsAsync();

        //    ViewBag.Roles = new SelectList(roles, "Value", "Text");
        //}
    }
}
