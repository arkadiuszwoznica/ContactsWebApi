using System;
using Microsoft.EntityFrameworkCore;
using Contacts.DTOs;
using Contacts.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Contacts.Controllers
{
    [ApiController]
    [Route("api/contacts/{contactId:int}/phones")]
    public class PhonesControlles : ControllerBase
    {
        private readonly ContactsDbContext _dbContext;

        public PhonesControlles(ContactsDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<IEnumerable<PhoneDto>> GetPhones(int contactId)
        {
            var contact = _dbContext.Contacts.Include(c => c.Phones)
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<PhoneDto> GetPhone(int contactId, int phoneId)
        {
            var phones = _dbContext.Phones
                .Where(p => p.ContactId == contactId && p.Id == phoneId);

            if (phones is null)
            {
                return NotFound();
            }

            var phoneDto = phones
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