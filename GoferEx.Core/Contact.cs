using Newtonsoft.Json;
using System;

namespace GoferEx.Core
{
    public class Contact
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string EmailAddress { get; set; }
        public DateTime BirthDate { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }

        public byte[] Photo { get; set; }

        //public Contact(string firstName, string lastName, string email, DateTime birthDate, string phoneNum, string password, byte[] photo = null)
        //{
        //    Id = Guid.NewGuid();
        //    FirstName = firstName;
        //    LastName = lastName;
        //    EmailAddress = email;
        //    BirthDate = birthDate;
        //    PhoneNumber = phoneNum;
        //    Password = password;
        //    Photo = photo;
        //}

        [JsonConstructor]
        public Contact(string firstName, string lastName, string username, string email, DateTime birthDate, string phone, string password, byte[] photo = null, string id = null)
        {
            Id = string.IsNullOrEmpty(id) ? Guid.NewGuid().ToString() : id;
            FirstName = firstName;
            LastName = lastName;
            EmailAddress = email;
            BirthDate = birthDate;
            PhoneNumber = phone;
            Password = password;
            Photo = photo;
            Username = username;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
