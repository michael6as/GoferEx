using Google.Contacts;
using Google.GData.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using Contact = GoferEx.Core.Contact;

namespace GoferEx.ExternalResources.Google
{
    public class GoogleResourceHandler : IResourceHandler
    {
        public IEnumerable<Contact> RetrieveContacts(OAuth2Parameters authParams)
        {
            try
            {
                RequestSettings settings = new RequestSettings("ContactsManager", authParams);
                ContactsRequest contactsProvider = new ContactsRequest(settings);
                var contacts = contactsProvider.GetContacts();
                return CastEntriesToContacts(contacts.Entries, contactsProvider);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private IEnumerable<Contact> CastEntriesToContacts(IEnumerable<global::Google.Contacts.Contact> feedContacts, ContactsRequest contactReq)
        {
            IList<Core.Contact> parsedContacts = new List<Contact>();
            foreach (global::Google.Contacts.Contact fContact in feedContacts)
            {
                if (string.IsNullOrEmpty(fContact.Name.FullName))
                {

                }
                else
                {
                    parsedContacts.Add(
                        new Contact(
                            fContact.Name.FullName,
                            fContact.Name.FamilyName,
                            fContact.Name.GivenName,
                            fContact.PrimaryEmail.Address,
                            fContact.Updated,
                            fContact.Phonenumbers.First().Value, "", GetContactPhoto(fContact, contactReq)));
                }
            }
            return parsedContacts;
        }

        private byte[] GetContactPhoto(global::Google.Contacts.Contact googleContact, ContactsRequest contactsApi)
        {
            try
            {
                using (System.IO.Stream photoStream = contactsApi.GetPhoto(googleContact))
                {
                    var photoInBytes = new byte[photoStream.Length];
                    photoStream.Read(photoInBytes, 0, photoInBytes.Length);
                    return photoInBytes;
                }
            }
            catch (Exception)
            {
                // TODO: Create default photo for unavailable photos
                return null;
            }
        }
    }
}