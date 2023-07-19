using System;
using Contacts.Domain;
using Contacts.DTOs;
using Contacts.WebAPI.Infrastructure;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace Contacts.Controllers
{
    [ApiController]
    [Route("api/contacts/{contactId:int}/phones")]
    public class PhonesControlles : ControllerBase
    {
        private readonly DataService _dataService;

        public PhonesControlles(DataService dataService)
        {
            _dataService = dataService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PhoneDto>> GetPhones(int contactId)
        {
            var contact = _dataService.Contacts
                .FirstOrDefault(c => c.Id == contactId);

            if (contact is null)
            {
                return NotFound();
            }

            var phonesDto = contact.Phones
                .Select(p => new PhoneDto()
                {
                    Id = p.Id,
                    Number = p.Number,
                    Description = p.Description
                });

            return Ok(phonesDto);
        }

        [HttpGet("{phoneId:int}")]
        public ActionResult<PhoneDto> GetPhone(int contactId, int phoneId)
        {
            var contact = _dataService.Contacts
                .FirstOrDefault(c => c.Id == contactId);

            if (contact is null)
            {
                return NotFound();
            }

            var phoneDto = contact.Phones
                .Where(p => p.Id == phoneId)
                .Select(p => new PhoneDto()
                {
                    Id = p.Id,
                    Number = p.Number,
                    Description = p.Description
                })
                .FirstOrDefault();

            if (phoneDto is null)
            {
                return NotFound();
            }

            return Ok(phoneDto);
        }
    }

}