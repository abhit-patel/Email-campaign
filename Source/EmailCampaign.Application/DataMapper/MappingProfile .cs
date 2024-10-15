using AutoMapper;
using EmailCampaign.Application.Features.User.Commands;
using EmailCampaign.Domain.Entities;
using EmailCampaign.Domain.Entities.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailCampaign.Application.DataMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<UserRegisterVM, User>();
            CreateMap<UserRegisterVM, CreateUserCommand>();
            CreateMap<UpdateUserCommand, User>();
            CreateMap<User, UpdateUserCommand>();
            CreateMap<User, UserRegisterVM>();
            CreateMap<ProfileVM, User>();
            CreateMap<User, ProfileVM>();
            CreateMap<RoleVM, Role>();
            CreateMap<Role, RoleVM>();
            CreateMap<PermissionVM, Permission>();
            CreateMap<Permission, PermissionVM>();
            CreateMap<RolePermissionDBVM, RolePermission>();
            CreateMap<RolePermission, RolePermissionDBVM>();
            CreateMap<ContactVM, Contact>();
            CreateMap<Contact, ContactVM>();
            CreateMap<GroupVM, Group>();
            CreateMap<Group, GroupVM>();
            CreateMap<PermissionListVM, RolePermissionDBVM>();
            CreateMap<RolePermissionVM, Role>();
        }
    }
}
