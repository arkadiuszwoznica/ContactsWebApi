using System;
using Microsoft.AspNetCore.Mvc;
using Contacts.Infrastructure;
namespace Contacts.Controllers
{
	[ApiController]
	[Route("api/contacts")]
	public class ContactsController : ControllerBase
	{

		[HttpGet]
		public IActionResult Get()
		{
			return new JsonResult(
				DataService.Instance.Contacts
			);
		}
	}
}