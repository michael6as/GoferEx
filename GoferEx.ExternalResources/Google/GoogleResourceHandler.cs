using GoferEx.Core;
using GoferEx.ExternalResources.Abstract;
using Google.Contacts;
using Google.GData.Client;
using Google.GData.Contacts;
using Google.GData.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Contact = GoferEx.Core.Contact;

namespace GoferEx.ExternalResources.Google
{
    public class GoogleResourceHandler : IResourceHandler
    {
        private readonly byte[] _defImg;
        public GoogleResourceHandler(byte[] defaultImgArr)
        {
            _defImg = defaultImgArr;
        }

        public IEnumerable<Contact> RetrieveContacts(ResourceAuthToken authParams)
        {
            RequestSettings settings = new RequestSettings("ContactsManager", authParams.AuthToken);
            ContactsRequest contactsProvider = new ContactsRequest(settings);
            var googleContactResult = contactsProvider.GetContacts();
            var contactsList = CastEntriesToContacts(googleContactResult.Entries, contactsProvider);
            return contactsList;
        }

        public bool AddContacts(ResourceAuthToken authParams, IEnumerable<Contact> contacts)
        {
            //if (SendPreRequest($"https://www.google.com/m8/feeds/contacts/{authParams.Id}/full", authParams,
            //        "POST") != HttpStatusCode.Created) return false;

            RequestSettings settings = new RequestSettings("ContactsManager", authParams.AuthToken);
            ContactsRequest contactsProvider = new ContactsRequest(settings);
            foreach (var contact in contacts)
            {
                CreateContact(contactsProvider, contact);
            }
            return true;

        }

        public bool DeleteContact(ResourceAuthToken authParams, string contactId)
        {
            RequestSettings settings = new RequestSettings("ContactsManager", authParams.AuthToken);
            ContactsRequest cr = new ContactsRequest(settings);
            var retrievedContact = cr.Retrieve<global::Google.Contacts.Contact>(new Uri(contactId));
            cr.Delete(retrievedContact);
            return true;
        }

        private void CreateContact(ContactsRequest cr, Contact contact)
        {
            var newEntry = new global::Google.Contacts.Contact
            {
                Name = new Name()
                {
                    FullName = contact.FirstName + contact.LastName,
                    GivenName = contact.FirstName,
                    FamilyName = contact.LastName,
                }
            };
            // Set the contact's name.

            // Set the contact's e-mail addresses.
            newEntry.Emails.Add(new EMail()
            {
                Primary = true,
                Rel = ContactsRelationships.IsHome,
                Address = contact.EmailAddress
            });
            // Set the contact's phone numbers.
            newEntry.Phonenumbers.Add(new PhoneNumber()
            {
                Primary = true,
                Rel = ContactsRelationships.IsWork,
                Value = contact.PhoneNumber
            });
            // Insert the contact.
            Uri feedUri = new Uri(ContactsQuery.CreateContactsUri("default"));
            global::Google.Contacts.Contact createdEntry = cr.Insert(feedUri, newEntry);
            //return createdEntry;
        }

        private HttpStatusCode SendPreRequest(string uri, ResourceAuthToken token, string method)
        {
            var request = WebRequest.CreateHttp(uri);
            request.Method = method;
            request.PreAuthenticate = true;
            request.Headers.Add("Authorization", "Bearer " + token.AuthToken.AccessToken);
            request.Accept = "application/json";

            var myWebResponse = request.GetResponse();
            var responseStream = myWebResponse.GetResponseStream();
            if (responseStream == null) return HttpStatusCode.NotFound;

            var myStreamReader = new StreamReader(responseStream, Encoding.Default);
            var json = myStreamReader.ReadToEnd();

            responseStream.Close();
            myWebResponse.Close();
            return HttpStatusCode.OK;
        }


        private IEnumerable<Contact> CastEntriesToContacts(IEnumerable<global::Google.Contacts.Contact> feedContacts, ContactsRequest contactReq)
        {
            return (from fContact in feedContacts
                    where !string.IsNullOrEmpty(fContact.Name.FullName)
                    select CastToGoferContact(fContact, contactReq)).ToList();
        }

        private Contact CastToGoferContact(global::Google.Contacts.Contact gContact, ContactsRequest contactReq)
        {
            if (gContact.Id == null)
            {
                throw new Exception("No Id, Invalid Google contact");
            }

            var id = gContact.Id;
            var firstName = gContact.Name.GivenName;
            var lastName = gContact.Name.FamilyName ?? gContact.Name.GivenName;
            var username = gContact.Name.FullName;
            var email = gContact.Emails.FirstOrDefault() != null ? gContact.PrimaryEmail.Address : "";

            var birthdate = gContact.Updated.ToString("dd/MM/yyyy");
            var phone = gContact.Phonenumbers.FirstOrDefault() != null ? CreatePhoneNumber(gContact.Phonenumbers.FirstOrDefault()) : "";
            var photo = GetContactPhoto(gContact.PhotoUri, contactReq);
            return new Contact(firstName, lastName, username, email, birthdate, phone, "", photo, id);
        }

        private string CreatePhoneNumber(PhoneNumber phoneNumObj)
        {
            var noSpaceNumber = phoneNumObj.Value.Replace(" ", string.Empty).Replace("-", string.Empty);
            return noSpaceNumber.Replace("+972", "05");
        }

        private string GetContactPhoto(Uri photoUri, ContactsRequest contactsApi)
        {
            try
            {
                using (GDataReturnStream photoStream = (GDataReturnStream)contactsApi.Service.Query(photoUri))
                {
                    byte[] photoBuffer;
                    using (var ms = new MemoryStream())
                    {
                        photoStream.CopyTo(ms);
                        photoBuffer = ms.ToArray();
                    }
                    return Convert.ToBase64String(photoBuffer);

                }
            }
            catch (Exception)
            {
                return Convert.ToBase64String(_defImg);
            }
        }
    }
}