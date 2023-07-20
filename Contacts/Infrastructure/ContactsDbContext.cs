using System;
using Microsoft.EntityFrameworkCore;
using Contacts.Domain;
namespace Contacts.Infrastructure
{
	public class ContactsDbContext : DbContext
	{
		public DbSet<Contact> Contacts => Set<Contact>();
        public DbSet<Phone> Phones => Set<Phone>();


        public ContactsDbContext(DbContextOptions<ContactsDbContext> options) : base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Contact>().HasData(new List<Contact>
			{
                new Contact()
				{
					Id =1, FirstName = "Jan", LastName = "Kowalski [EF Core]", Email="jkowalski@w.pl"
				},
                new Contact()
				{
					Id =2, FirstName = "Julian", LastName = "Miron", Email="jmiron@w.pl"
				}
            });

			modelBuilder.Entity<Phone>().HasData(new List<Phone>
			{
                new Phone()
				{
					Id=1, Number="998889221", Description="dom", ContactId = 1
				},
                new Phone()
				{
					Id=2, Number="111222333", Description="praca", ContactId = 1
				}
            });
		}
	}
}

