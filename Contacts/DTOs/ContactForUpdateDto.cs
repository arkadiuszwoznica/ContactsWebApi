using System;
using System.ComponentModel.DataAnnotations;
namespace Contacts.DTOs
{
	public class ContactForUpdateDto
	{
        [Required]
        [MaxLength(32)]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        [MaxLength(64)]
        public string LastName { get; set; } = string.Empty;
        [EmailAddress]
        [MaxLength(64)]
        public string Email { get; set; } = string.Empty;
    }
}