using System;
using AutoMapper;
using Contacts.Domain;
using Contacts.DTOs;
namespace Contacts.Configurations.Mapper
{
	public class ContactsProfile : Profile
	{
		public ContactsProfile()
		{
			CreateMap<Contact, ContactDto>();
			CreateMap<Contact, ContactsDetailsDto>();
			CreateMap<Phone, PhoneDto>();
		}
	}
}

