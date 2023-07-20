using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.JsonPatch;
using AutoMapper;
using Contacts.Infrastructure;
using Contacts.DTOs;
using Contacts.Domain;
using Contacts.Infrastructure.Repositories;

namespace Contacts.Controllers
{
	[ApiController]
	[Route("api/contacts")]
	public class ContactsController : ControllerBase
	{
        private readonly IContactsRepository _repository;
        private readonly IMapper _mapper;

        public ContactsController(IContactsRepository repository, IMapper mapper)
		{
            _repository = repository;
            _mapper = mapper;
		}


		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<ContactDto>> GetAllContacts([FromQuery] string? search)
		{
            var contacts = _repository.GetContacts(search);
            var contactsDto = _mapper.Map<IEnumerable<ContactDto>>(contacts);
			return Ok(contactsDto);
		}


		[HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<ContactsDetailsDto> GetContact(int id)
		{
            var contact = _repository.GetContact(id);

			if (contact is null)
			{
				return NotFound();
			}

            var contactDto = _mapper.Map<ContactsDetailsDto>(contact);
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

            var contact = new Contact()
            {
                FirstName = contactForCreationDto.FirstName,
                LastName = contactForCreationDto.LastName,
                Email = contactForCreationDto.Email
            };

            _repository.CreateContact(contact); 

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
            var contact = new Contact
            {
                Id = id,
                FirstName = contactForUpdateDto.FirstName,
                LastName = contactForUpdateDto.LastName,
                Email = contactForUpdateDto.Email,
            };

            var success = _repository.UpdateContact(contact);

            if (!success)
            {
                return NotFound();
            }

			return NoContent();
        }


		[HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteContact(int id)
        {
            var success = _repository.DeleteContact(id);

            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }


		[HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult PartiallyUpdateContatc(int id, [FromBody] JsonPatchDocument<ContactForUpdateDto> patchDocument)
		{
            var contact = _repository.GetContact(id);

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

            var success = _repository.UpdateContact(contact);

            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}