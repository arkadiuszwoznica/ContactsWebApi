using System;
using Microsoft.AspNetCore.Mvc;
namespace Contacts.Controllers
{
	[ApiController]
	public class ContactsController : Controller
	{
		[HttpGet("api/contacts")]
		public IActionResult Get()
		{
			return new JsonResult(
				new List<object>()
				{
					new {Id =1, FirstName = "Jan", LastName = "Kowalski", Email="jkowalski@w.pl"},
					new {Id =2, FirstName = "Julian", LastName = "Miron", Email="jmiron@w.pl"}
				}
			);
		}
	}
}