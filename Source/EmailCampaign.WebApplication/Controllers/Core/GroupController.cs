using AutoMapper;
using EmailCampaign.Domain.Entities;
using EmailCampaign.Domain.Entities.ViewModel;
using EmailCampaign.Domain.Interfaces.Core;
using EmailCampaign.Infrastructure.Data.Repositories.Core;
using Microsoft.AspNetCore.Mvc;

namespace EmailCampaign.WebApplication.Controllers.Core
{
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class GroupController : Controller
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IContactRepository _contactRepository;
        private readonly IMapper _mapper;

        public GroupController(IGroupRepository groupRepository, IMapper mapper, IContactRepository contactRepository)
        {
            _groupRepository = groupRepository;
            _contactRepository = contactRepository;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            List<Group> groupList = await _groupRepository.GetAllGroupAsync();

            return View(groupList);
        }

        public async Task<IActionResult> GetGroupById(Guid id)
        {
            Group group = await _groupRepository.GetGroupAsync(id);

            GroupVM GroupVM = _mapper.Map<GroupVM>(group);

            //await LoadRoles();

            return View("UpdateGroup", GroupVM);
        }

        [HttpGet]
        public IActionResult AddGroup()
        {
            //await LoadRoles();

            return View();
        }

        [HttpPost]
        [ActionName("AddGroup")]
        public async Task<IActionResult> Post(GroupVM model)
        {
            var group = await _groupRepository.CreateGroupAsync(model);

            if (group == null)
            {
                TempData["ErrorMessage"] = "Failed to add group. Please try again.";
                return RedirectToAction("Index");
            }

            TempData["SuccessMessage"] = "group successfully added!";

            return RedirectToAction("Index");
        }


        [HttpPost]
        public async Task<IActionResult> Update(Guid id, GroupVM groupModel)
        {

            var group = await _groupRepository.UpdateGroupAsync(id, groupModel);

            if (group == null)
            {

            }
            return RedirectToAction("Index");
        }


        [HttpGet]
        [ActionName("ViewContactsForGroup")]
        public async Task<IActionResult> ViewContactListForGroup(Guid id)
        {
            var contactsGroup = await _groupRepository.GetContactForGroupAsync(id);

            if (contactsGroup == null)
            {
                return View(new ContactGroupVM());
            }

            return View(contactsGroup);
        }


        [ActionName("ContactMapping")]
        public async Task<IActionResult> MapGroupContact( Guid id )
        {
            var Contacts = await _contactRepository.GetContactForGroupAsync();

            var model = new ContactGroupVM
            {
                GroupID = id,
                Contacts = Contacts.Select(c => new ContactSelection
                {
                    ContactId = c.Id,
                    ContactName = $"{c.FirstName} {c.LastName} ",
                    Email = $"{c.Email}",
                    CompanyName = $"{c.CompanyName}",
                    IsSelected = false
                }).ToList()
            };

            return View(model);
        }


        [HttpPost]
        [ActionName("AddContactMapping")]
        public async Task<IActionResult> SaveContactMapping(ContactGroupVM groupContactVM)
        {
            if(groupContactVM == null)
            {
                return RedirectToAction("Index");
            }

            var selectedContacts = groupContactVM.Contacts.Where(p => p.IsSelected == true).ToList();

            foreach(var contact in selectedContacts)
            {
                    var contactsGroup = await _groupRepository.AddContactsGroupAsync(groupContactVM.GroupID, contact.ContactId, contact.IsSelected);

                    if (contactsGroup == null)
                    {
                        return View("Index");
                    }
            }

            return RedirectToAction("Index");
        }


        [HttpGet]
        [ActionName("GetContactsForGroup")]
        public async Task<IActionResult> GetAllContactForGroup(Guid id)
        {
            if(id == Guid.Empty)
            {
                return RedirectToAction("Index");
            }

            ContactGroupVM model = await _groupRepository.GetContactForGroupAsync(id);

            if (model == null)
            {
                TempData["ErrorMessage"] = "";
                return RedirectToAction("Index");
            }

            return View(model);
        }



        [ActionName("UpdateContactMapping")]
        public async Task<IActionResult> UpdateContactMapping(ContactGroupVM groupContactVM)
        {
            if (groupContactVM == null)
            {
                return RedirectToAction("Index");
            }

            foreach (var contact in groupContactVM.Contacts)
            {
                var contactsGroup = await _groupRepository.UpdateContactsGroupAsync(groupContactVM.GroupID, contact.ContactId, contact.IsSelected);

                if (contactsGroup == null)
                {
                    return RedirectToAction("Index");
                }
            }

            return RedirectToAction("Index");

        }




        [ActionName("IsActiveToggle")]
        public async Task<IActionResult> IsActiveToggleAsync(Guid id)
        {
            var contact = await _groupRepository.ActiveToggleAsync(id);

            if (contact == null)
            {
                TempData["ErrorMessage"] = "Status failed to change.";
                return RedirectToAction("AddGroup");
            }

            TempData["SuccessMessage"] = "Status has been toggled successfully.";
            return RedirectToAction("Index");
        }



        public async Task<IActionResult> Delete(Guid id)
        {
            var group = await _groupRepository.DeleteGroupAsync(id);

            if (!group.IsDeleted)
            {
                TempData["ErrorMessage"] = "Group delete process failed.";
                return RedirectToAction("Index");
            }

            TempData["SuccessMessage"] = "Contact successfully deleted.";
            return RedirectToAction("Index");
        }
    }
}
