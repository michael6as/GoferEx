using Google.Apis.Auth.OAuth2;
using Google.Contacts;
using Google.GData.Client;
using System;
using System.Threading;
using Google.Apis.Auth.OAuth2.Flows;

namespace GoferEx.ConTest
{
    public class GoogleHandler
    {
        public void Auth()
        {
            string clientId = "535817455358-tlgkg5jca5u3kjd3jlhqr4u1ef6udt7f.apps.googleusercontent.com";
            string clientSecret = "Mk8mvSz3SmHmi9Enz19vsPbX";


            string[] scopes = new string[] { "https://www.googleapis.com/auth/contacts.readonly" };     // view your basic profile info.
            try
            {
                // Use the current Google .net client library to get the Oauth2 stuff.                
                UserCredential credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    new ClientSecrets {ClientId = clientId, ClientSecret = clientSecret}
                    , scopes
                    , "test"
                    , CancellationToken.None).Result;
                    //, new FileDataStore("test")).Result;

                // Translate the Oauth permissions to something the old client libray can read
                OAuth2Parameters parameters = new OAuth2Parameters();
                parameters.AccessToken = credential.Token.AccessToken;
                parameters.RefreshToken = credential.Token.RefreshToken;
                RunContactsSample(parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void RunContactsSample(OAuth2Parameters parameters)
        {
            try
            {
                RequestSettings settings = new RequestSettings("Google contacts tutorial", parameters);
                ContactsRequest cr = new ContactsRequest(settings);
                var f = cr.GetContacts();
                //Feed f = cr.GetContacts();
                foreach (Contact c in f.Entries)
                {
                    Console.WriteLine(c.Name.FullName);
                }
            }
            catch (Exception a)
            {
                Console.WriteLine("A Google Apps error occurred.");
                Console.WriteLine();
            }
        }
    }
}