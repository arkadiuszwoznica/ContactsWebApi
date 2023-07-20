using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.JsonPatch;
using Contacts.Infrastructure;
using Contacts.DTOs;
using Contacts.Domain;

namespace Contacts.Controllers
{
	[ApiController]
	[Route("api/contacts")]
	public class ContactsController : ControllerBase
	{
        private readonly ContactsDbContext _dbContext;

        public ContactsController(ContactsDbContext dbContext)
		{
			_dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
		}


		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<ContactDto>> GetAllContacts([FromQuery] string? search)
		{
			var query = _dbContext.Contacts.AsQueryable();

			if(!string.IsNullOrWhiteSpace(search))
			{
				query = query.Where(c => c.LastName.Contains(search));
			}

			var contactsDto = query.Select(c => new ContactDto
			{
				Id = c.Id,
				FirstName = c.FirstName,
				LastName = c.LastName,
				Email = c.Email
			});
			return Ok(contactsDto);
		}


		[HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<ContactsDetailsDto> GetContact(int id)
		{
			var contact = _dbContext.Contacts.Include(c => c.Phones)
				.FirstOrDefault(c => c.Id == id);

			if (contact is null)
			{
				return NotFound();
			}

			var contactDto = new ContactsDetailsDto()
				{
					Id = contact.Id,
					FirstName = contact.FirstName,
					LastName = contact.LastName,
					Email = contact.Email,
					Phones = contact.Phones
					.Select(p => new PhoneDto()
					{
						Id = p.Id,
						Number = p.Number,
						Description = p.Description
					}).ToList()
				};

            return Ok(contactDto);
        }


		[HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateContact([FromBody] ContactForCreationDto contactForCreationDto)
		{
			if(contactForCreationDto.FirstName == contactForCreationDto.LastName)
			{
				ModelState.AddModelError("wrongName", "firstname and lastname cannot be the same");
			}

			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var maxId = _dbContext.Contacts.Max(c => c.Id);

            var contact = new Contact()
            {
                FirstName = contactForCreationDto.FirstName,
                LastName = contactForCreationDto.LastName,
                Email = contactForCreationDto.Email
            };

            _dbContext.Contacts.Add(contact);
			_dbContext.SaveChanges();

            var contactDto = new ContactDto()
            {
                Id = contact.Id,
                FirstName = contact.FirstName,
                LastName = contact.LastName,
                Email = contact.Email
            };

            return CreatedAtAction(nameof(GetContact), new { id = contact.Id }, contactDto);
        }


		[HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateContact(int id, [FromBody] ContactForUpdateDto contactForUpdateDto)
		{
			var contact = _dbContext
				.Contacts
				.FirstOrDefault(c => c.Id == id);

            if (contact is null)
            {
                return NotFound();
            }

			contact.FirstName = contactForUpdateDto.FirstName;
            contact.LastName = contactForUpdateDto.LastName;
            contact.Email = contactForUpdateDto.Email;

			_dbContext.SaveChanges();

			return NoContent();
        }


		[HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteContact(int id)
        {
            var contact = _dbContext
                .Contacts
                .FirstOrDefault(c => c.Id == id);

            if (contact is null)
            {
                return NotFound();
            }

            _dbContext.Contacts.Remove(contact);
            _dbContext.SaveChanges();

            return NoContent();
        }


		[HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult PartiallyUpdateContatc(int id, [FromBody] JsonPatchDocument<ContactForUpdateDto> patchDocument)
		{
            var contact = _dbContext
                .Contacts
                .FirstOrDefault(c => c.Id == id);

            if (contact is null)
            {
                return NotFound();
            }

			var contactToBePatched = new ContactForUpdateDto()
			{
				FirstName = contact.FirstName,
				LastName = contact.LastName,
				Email = contact.Email
			};

			patchDocument.ApplyTo(contactToBePatched, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

			if (!TryValidateModel(contactToBePatched))
			{
				return BadRequest(ModelState);
            }

            contact.FirstName = contactToBePatched.FirstName;
            contact.LastName = contactToBePatched.LastName;
            contact.Email = contactToBePatched.Email;

            _dbContext.SaveChanges();

            return NoContent();
		}
    }
}