using System;
namespace Contacts.Domain
{
	public class Contact
	{
		public int Id { get; set; }
		public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
		public ICollection<Phone> Phones { get; set; } = new List<Phone>();
    }
}