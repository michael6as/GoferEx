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
        public string BirthDate { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }

        public string Photo { get; set; }

        [JsonConstructor]
        public Contact(string firstName, string lastName, string username, string email, string birthDate, string phone, string password, string photo = null, string id = null)
        {
            // In case we adding contacts locally, we will create Guid
            Id = string.IsNullOrEmpty(id) ? Guid.NewGuid().ToString() : id;
            FirstName = firstName;
            LastName = lastName;
            EmailAddress = email;
            DateTime tempDate;
            if (!DateTime.TryParse(birthDate, out tempDate))
            {
                throw new Exception("Birthday is invalid format");
            }
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
