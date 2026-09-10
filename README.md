The solution is built following the Domain Driven Design DDD to adapt the architecture to the business needs. 

The domain Entites store the object model and their requirements. Each entity has its own business rules.
The Repository Pattern links entity storage and domain model using abstract interface.
The domain Service has a primary PenaltyService that acts independently with its own responsability. It is a component of LibraryService which acts as the orchestator of the solution. LibraryService implements the different interfaces of the Repository Pattern and uses them to execute the diverse business actions.


Member uses a boolean to store the type of member. It uses less memory than an enum and works only because there are currently 2 types of members. As soon as more types of members exists, an enum must be used. We can think of additionnal informations like a name, but it wasn't in the specifications.


A book must have a title as a way of identification, but the author can be missing. Some book authors are not known. Even if some rare books don't have any title, the use gives it a name for identification.

A loan is linked to a member and a book. It has a start date but doesn't store the end date as the loan is deleted when the book is returned. 

PenaltyService implements a penalty that is reflected negatively on the balance. When a member returns a loan after the due date, a penalty will be substracted from their balance. It permits the user to know the amount to top up. We can think of a maximum balance deficit that blocks a member from borrowing a book if their balance is negative for too long.

LibraryService acts as a librarian that registers a new member or a book. It can modify the numberOfCopies of a book if one is missing or a new one is added.
It registers new loans and returns of books.

Improvment:

The next step could be to create a web UI linked to the backend via a API REST. 
