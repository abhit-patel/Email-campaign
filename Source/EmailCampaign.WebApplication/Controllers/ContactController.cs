using AutoMapper;
using EmailCampaign.Domain.Entities.ViewModel;
using EmailCampaign.Domain.Entities;
using EmailCampaign.Domain.Interfaces;
using EmailCampaign.Infrastructure.Data.Services;
using Microsoft.AspNetCore.Mvc;
using EmailCampaign.Domain.Interfaces.Core;
using EmailCampaign.Infrastructure.Data.Repositories;

namespace EmailCampaign.WebApplication.Controllers
{
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

        [ActionName("ContactView")]
        public async Task<IActionResult> ContactView()
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
                TempData["Message"] = "";
                return View("AddContact");
            }

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



        public async Task<IActionResult> Delete(Guid id)
        {
            var isDeleteed = await _contactRepository.DeleteContactAsync(id);

            if (!isDeleteed)
            {
                TempData["Message"] = "";
                return View("Index");
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ActionName("IsActiveToggle")]

        public async Task<IActionResult> IsActiveToggleAsync(Guid userId)
        {
            var user = await _contactRepository.ActiveToggleAsync(userId);

            if (user == null)
            {
                TempData["Message"] = "";
                return View("AddUser");
            }

            return RedirectToAction("Index");
        }

        //private async Task LoadGroup()
        //{
        //    var roles = await _contactRepository.GetRolesAsSelectListItemsAsync();

        //    ViewBag.Roles = new SelectList(roles, "Value", "Text");
        //}
    }
}
