using GoferEx.Core;
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
    // TODO 1: Add Another interface for strategy-pattern the resources
    public class GoogleResourceHandler : IResourceHandler
    {
        public IEnumerable<Contact> RetrieveContacts(ResourceAuthToken authParams)
        {
            try
            {
                RequestSettings settings = new RequestSettings("ContactsManager", authParams.AuthToken);
                ContactsRequest contactsProvider = new ContactsRequest(settings);
                var googleContactResult = contactsProvider.GetContacts();
                var contactsList = CastEntriesToContacts(googleContactResult.Entries, contactsProvider);
                return contactsList;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
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

        public bool DeleteContacts(ResourceAuthToken authParams, IEnumerable<Contact> contacts)
        {
            RequestSettings settings = new RequestSettings("ContactsManager", authParams.AuthToken);
            ContactsRequest cr = new ContactsRequest(settings);
            foreach (var contact in contacts)
            {
                var retrievedContact = cr.Retrieve<global::Google.Contacts.Contact>(new Uri(contact.Id));
            }

            return true;
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