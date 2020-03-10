using RefactoringDemo.Domain.SharedKernel;
using System;

namespace RefactoringDemo.Domain.ECommerce.Orders
{
    public class Customer : Entity
    {
        public Customer(string name, DateTime birthDate, string email, string nif)
        {
            Name = name;
            BirthDate = birthDate;
            Email = email;
            Nif = nif;

            //AddNotifications(name.Notifications);
            //AddNotifications(email.Notifications);
            //AddNotifications(Document.Notifications);
        }

        public string Name { get; private set; }

        public string Email { get; private set; }

        public DateTime BirthDate { get; private set; }

        public string Nif { get; }

        public DateTime? LastPurchaseDate { get; private set; }

        public void UpdateLastPurchaseDate(DateTime lastPurchaseDate)
            => LastPurchaseDate = lastPurchaseDate;

        public void Uodate(string name, string email, DateTime birthDate)
        {
            //AddNotifications(name.Notifications);
            //AddNotifications(email.Notifications);

            Name = name;
            Email = email;
            BirthDate = birthDate;
        }
    }
}