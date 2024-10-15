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
using MediatR;
using EmailCampaign.Application.Features.Contact.Queries;
using EmailCampaign.Application.Features.Contact.Commands;

namespace EmailCampaign.WebApplication.Controllers.Core
{
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class ContactController : Controller
    {
        private readonly IContactRepository _contactRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public ContactController(IContactRepository contactRepository, IMapper mapper, IMediator mediator)
        {
            _contactRepository = contactRepository;
            _mapper = mapper;
            _mediator = mediator;
        }

        //[CustomAuthorize("User/Index", typeof(IUserService))]
        public async Task<IActionResult> Index()
        {
            var contactList = await _mediator.Send(new GetAllContactQuery());

            return View(contactList);
        }


        public async Task<IActionResult> GetContactById(Guid id, CancellationToken cancellationToken)
        {
            GetContactByIdQuery getContactByIdQuery = new GetContactByIdQuery
            {
                Id = id
            };

            var contact = await _mediator.Send(getContactByIdQuery, cancellationToken);

            //await LoadRoles();

            return View("UpdateContact", contact);
        }

        [HttpGet]
        public async Task<IActionResult> AddContact()
        {
            //await LoadRoles();

            return View();
        }

        [HttpPost]
        [ActionName("AddContact")]
        public async Task<IActionResult> Post(CreateContactCommand model, CancellationToken cancellationToken)
        {

            var isRegisteredEmail = await _contactRepository.CheckRegisteredEmailAsync(model.Email);

            if (isRegisteredEmail)
            {
                TempData["ErrorMessage"] = "Contact Email is already present, please use another email.";
                return RedirectToAction("AddContact");
            }

            var contact = await _mediator.Send(model, cancellationToken);

            if (contact == null)
            {
                TempData["ErrorMessage"] = "Failed to add contact. Please try again.";
                return RedirectToAction("Index");
            }

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

                if (scvData == null)
                {
                    TempData["ErrorMessage"] = "import file process failed !!";
                    return RedirectToAction("AddContact");
                }

            }
            else if (Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                List<ContactVM> excelData = await _contactRepository.ImportExcel(filePath);

                if (excelData == null)
                {
                    TempData["ErrorMessage"] = "Import file process failed !!";
                    return RedirectToAction("AddContact");
                }

            }

            TempData["SuccessMessage"] = "file successfully imported.";
            return RedirectToAction("Index");
        }


        [HttpPost]
        public async Task<IActionResult> Update(Guid id, UpdateContactCommand contactModel, CancellationToken cancellationToken)
        {
            var contact = await _mediator.Send(contactModel, cancellationToken);

            if (contact == null)
            {

            }
            return RedirectToAction("Index");
        }


        [ActionName("IsActiveToggle")]
        public async Task<IActionResult> IsActiveToggleAsync(string email, CancellationToken cancellationToken)
        {
            ContactActiveToggleCommand contactActiveToggleCommand = new ContactActiveToggleCommand
            {
                email = email
            };

            Contact contact = await _mediator.Send(contactActiveToggleCommand, cancellationToken);

            if (contact == null)
            {
                TempData["ErrorMessage"] = "Status failed to change.";
                return RedirectToAction("AddUser");
            }

            TempData["SuccessMessage"] = "Status has been toggled successfully.";
            return RedirectToAction("Index");
        }



        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            DeleteContactCommand deleteContactCommand = new DeleteContactCommand
            {
                Id = id
            };

            var contact = await _mediator.Send(deleteContactCommand, cancellationToken);

            if (!contact.IsDeleted)
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
