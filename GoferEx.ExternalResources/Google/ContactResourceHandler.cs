using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using GoferEx.Core;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Util.Store;
using Google.Contacts;
using Google.GData.Client;
using Newtonsoft.Json;
using Contact = Google.Contacts.Contact;

namespace GoferEx.ExternalResources.Google
{
    public class ContactResourceHandler : IResourceHandler<CustomGoogleToken>
    {
        private const string CLIENT_ID = "535817455358-tlgkg5jca5u3kjd3jlhqr4u1ef6udt7f.apps.googleusercontent.com";
        private const string CLIENT_SECRET = "Mk8mvSz3SmHmi9Enz19vsPbX";
        private const string TOKEN_EXCHANGE_URI = "https://accounts.google.com/o/oauth2/token";
        private OAuth2Parameters _authParams;
        private string _redirectUri;

        public ContactResourceHandler(string redirectUri)
        {
            _redirectUri = redirectUri;
        }

        public void Authenticate(string[] scopes)
        {
            try
            {
                UserCredential credential = GoogleWebAuthorizationBroker.AuthorizeAsync(new ClientSecrets { ClientId = CLIENT_ID, ClientSecret = CLIENT_SECRET }
                    , scopes
                    , "test"
                    , CancellationToken.None
                    , new FileDataStore("test")).Result;

                _authParams = new OAuth2Parameters
                {
                    AccessToken = credential.Token.AccessToken,
                    RefreshToken = credential.Token.RefreshToken
                };
            }
            catch (Exception e)
            {
                throw new AggregateException("Couldn't authenticate the Google contacts API, see inner exception", e);
            }
        }

        public IEnumerable<GoferEx.Core.Contact> RetrieveContacts(CustomGoogleToken token)
        {
            try
            {
                RequestSettings settings = new RequestSettings("Google contacts tutorial", _authParams);
                ContactsRequest cr = new ContactsRequest(settings);
                var contacts = cr.GetContacts();
                return CastEntriesToContacts(contacts.Entries);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        /// This function should take the user's given auth code and exchange it for tokens (access & refresh)
        /// Getting 400 from Google
        /// </summary>
        /// <param name="token">The authorization code for Google API</param>
        /// <returns>True if the exchange succeded</returns>
        public bool ValidateToken(CustomGoogleToken token)
        {
            string google_client_id = CLIENT_ID;
            string google_client_sceret = CLIENT_SECRET;
            string google_redirect_url = _redirectUri;

            /*Get Access Token and Refresh Token*/
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create            
                (TOKEN_EXCHANGE_URI);
            webRequest.Method = "POST";
            string parameters = "code=" + token.AuthCode + "&client_id=" + google_client_id +
                                "&client_secret=" + google_client_sceret + "&redirect_uri="
                                + google_redirect_url + "&grant_type=authorization_code";
            byte[] byteArray = Encoding.UTF8.GetBytes(parameters);
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.ContentLength = byteArray.Length;
            Stream postStream = webRequest.GetRequestStream();

            // Add the post data to the web request
            postStream.Write(byteArray, 0, byteArray.Length);
            postStream.Close();
            WebResponse response = webRequest.GetResponse();
            postStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(postStream);
            string responseFromServer = reader.ReadToEnd();
            GooglePlusAccessToken serStatus = JsonConvert.DeserializeObject
                <GooglePlusAccessToken>(responseFromServer);
            return true;
        }

        private IEnumerable<Core.Contact> CastEntriesToContacts(IEnumerable<Contact> feedContacts)
        {
            IList<Core.Contact> parsedContacts = new List<Core.Contact>();
            foreach (var fContact in feedContacts)
            {
                parsedContacts.Add(
                    new Core.Contact(
                        Guid.NewGuid().ToString(),
                        fContact.Name.FullName,
                        fContact.Name.FamilyName,
                        fContact.Name.GivenName,
                        fContact.PrimaryEmail.Address,
                        fContact.Updated,
                        fContact.Phonenumbers.First().Value, "test"));
            }

            return parsedContacts;
        }
    }
}