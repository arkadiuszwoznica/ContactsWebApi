﻿using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;
using AutoMapper;
using Contacts.DTOs;
using Contacts.Domain;
using Contacts.Infrastructure.Repositories;
using Microsoft.Extensions.Caching.Memory;

namespace Contacts.Controllers
{
    [ApiController]
    [Route("api/contacts")]
    public class ContactsController : ControllerBase
    {
        private readonly IContactsRepository _repository;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _memoryCache;

        public ContactsController(IContactsRepository repository, IMapper mapper, IMemoryCache memoryCache)
        {
            _repository = repository;
            _mapper = mapper;
            _memoryCache = memoryCache;
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
        //[ResponseCache(CacheProfileName = "Any-60")]
        public ActionResult<ContactsDetailsDto> GetContact(int id)
        {
            var cacheKey = $"{nameof(ContactsController)}-{nameof(GetContact)}-{id}";


            if (!_memoryCache.TryGetValue<ContactsDetailsDto>(cacheKey, out var contactDto))
            {
                var contact = _repository.GetContact(id);

                if (contact is not null)
                {
                    contactDto = _mapper.Map<ContactsDetailsDto>(contact);

                    _memoryCache.Set(cacheKey, contactDto, TimeSpan.FromSeconds(60));
                }
            }

            // var contact = _repository.GetContact(id);

            if (contactDto is null)
            {
                return NotFound();
            }

            return Ok(contactDto);
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateContact([FromBody] ContactForCreationDto contactForCreationDto)
        {
            if (contactForCreationDto.FirstName == contactForCreationDto.LastName)
            {
                ModelState.AddModelError("wrongName", "firstname and lastname cannot be the same");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var contact = _mapper.Map<Contact>(contactForCreationDto);
            _repository.CreateContact(contact);

            var contactDto = _mapper.Map<ContactDto>(contact);

            return CreatedAtAction(nameof(GetContact), new { id = contact.Id }, contactDto);
        }


        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateContact(int id, [FromBody] ContactForUpdateDto contactForUpdateDto)
        {
            var contact = _mapper.Map<Contact>(contactForUpdateDto);
            contact.Id = id;

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

            var contactToBePatched = _mapper.Map<ContactForUpdateDto>(contact);

            patchDocument.ApplyTo(contactToBePatched, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!TryValidateModel(contactToBePatched))
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(contactToBePatched, contact);

            var success = _repository.UpdateContact(contact);

            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}