﻿using System;
namespace Contacts.DTOs
{
	public class ContactsDetailsDto
	{
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}";
        public string Email { get; set; } = string.Empty;
        public List<PhoneDto> Phones { get; set; } = new();
    }
}

