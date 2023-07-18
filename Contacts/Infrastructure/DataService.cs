using System;
using Contacts.Domain;
namespace Contacts.WebAPI.Infrastructure
{
	public class DataService
	{
		public List<Contact> Contacts { get; }

		public DataService()
		{
			Contacts = new List<Contact>()
			{
					new Contact() {Id =1, FirstName = "Jan", LastName = "Kowalski", Email="jkowalski@w.pl"},
					new Contact() {Id =2, FirstName = "Julian", LastName = "Miron", Email="jmiron@w.pl"}
			};
		}
	}
}