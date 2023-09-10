# ContactsWebApi
Hi there!

This is the app on which i was practicing creating APIs to develop my skills of coding fully working, professional apps.
You can see I'm using database to store data and access it through Entity Framework. 
In accordance to good practices of coding I'm using LINQ, automapping and Repository, to make my code clean, short and as simple as it is possible. 

You can use swagger to try the api out. 


# Program.cs
This file contains all needed configurations to this project.


# Infrastucture/Repositories
In this folder you can find two files - interface IContactRepository and ContactRepository class which implements methods included in interface.
ContactRepository class is serving as a data repository for contacts. Key functionalities include:

- Constructor: Initializes with a database context, throwing an exception if null.
- GetContacts: Retrieves contacts based on an optional search parameter from the database.
- GetContact: Fetches a single contact by ID from the database, eagerly loading associated phone data.
- CreateContact: Adds a new contact to the database and saves changes.
- UpdateContact: Updates an existing contact's data in the database.
- DeleteContact: Deletes a contact from the database.

This class abstracts contact data management, relying on ContactsDbContext for database operations, what is an advanced practice of making code clean. 
