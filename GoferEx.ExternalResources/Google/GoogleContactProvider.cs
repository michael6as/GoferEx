using Google.Apis.Auth.OAuth2;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Google.Apis.PeopleService.v1;
using Google.Apis.Services;
using Google.Apis.PeopleService.v1.Data;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Google.GData.Client;
using Google.Contacts;
using Google.GData.Contacts;
using Google.GData.Extensions;

namespace GoferEx.ExternalResources.Google
{
    public class GoogleContactProvider
    {
        private string _clientId;
        private string _clientSecret;
        private PeopleServiceService _service;

        public GoogleContactProvider(string appClientId, string appClientSecret)
        {
            _clientId = appClientId;
            _clientSecret = appClientSecret;
        }

        public void GetAccessToken(string code)
        {
            string google_client_id = _clientId;
            string google_client_sceret = _clientSecret;
            string google_redirect_url = "http://localhost:1906/signin-google";

            /*Get Access Token and Refresh Token*/
            HttpWebRequest webRequest = (HttpWebRequest) WebRequest.Create
                ("https://accounts.google.com/o/oauth2/token");
            webRequest.Method = "POST";
            string parameters = "code=" + code + "&client_id=" + google_client_id +
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
            /*End*/

            GetContacts(serStatus);
        }

        private void GetContacts(GooglePlusAccessToken serStatus)
        {
            string refreshToken = serStatus.RefreshToken;
            string accessToken = serStatus.AccessToken;
            string scopes = @"https://www.google.com/m8/feeds/contacts/
            default / full / ";
            OAuth2Parameters oAuthparameters = new OAuth2Parameters()
            {
                Scope = scopes,
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };

            RequestSettings settings = new RequestSettings
                ("<var>YOUR_APPLICATION_NAME</var>", oAuthparameters);
            ContactsRequest cr = new ContactsRequest(settings);
            ContactsQuery query = new ContactsQuery(ContactsQuery
                .CreateContactsUri("default"));
            query.NumberToRetrieve = 5000;
            Feed<Contact> feed = cr.Get<Contact>(query);

            StringBuilder sb = new StringBuilder();
            int i = 1;
            foreach (Contact entry in feed.Entries)
            {
                foreach (EMail email in entry.Emails)
                {
                    sb.Append("<span>" + i + ". </span>").Append(email.Address)
                        .Append("<br/>");
                    i++;
                }
            }
            /*End*/
            var sss = sb.ToString();
        }
    }

    //    public void Con()
    //    {
    //        UserCredential credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
    //            new ClientSecrets
    //            {
    //                ClientId = _clientId,
    //                ClientSecret = _clientSecret
    //            },
    //            new[] { "profile", "https://www.googleapis.com/auth/contacts.readonly" },
    //            "me",
    //            CancellationToken.None).Result;

    //        // Create the service.            
    //        _service =  new PeopleServiceService(new BaseClientService.Initializer()
    //        {
    //            HttpClientInitializer = credential,
    //            ApplicationName = "GoferMichael",
    //        });
    //    }

    //    public void GetS()
    //    {
    //        PeopleResource.ConnectionsResource.ListRequest peopleRequest =
    //            _service.People.Connections.List("people/me");
    //        peopleRequest.PersonFields = "names,emailAddresses";
    //        ListConnectionsResponse connectionsResponse = peopleRequest.Execute();
    //        IList<Person> connections = connectionsResponse.Connections;
    //    }
    //}
}
