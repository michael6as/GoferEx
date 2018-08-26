using Google.Apis.Auth.OAuth2;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Google.Apis.PeopleService.v1;
using Google.Apis.Services;
using Google.Apis.PeopleService.v1.Data;

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
        public void Con()
        {
            UserCredential credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                new ClientSecrets
                {
                    ClientId = _clientId,
                    ClientSecret = _clientSecret
                },
                new[] { "profile", "https://www.googleapis.com/auth/contacts.readonly" },
                "me",
                CancellationToken.None).Result;

            // Create the service.            
            _service =  new PeopleServiceService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "GoferMichael",
            });
        }

        public void GetS()
        {
            PeopleResource.ConnectionsResource.ListRequest peopleRequest =
                _service.People.Connections.List("people/me");
            peopleRequest.PersonFields = "names,emailAddresses";
            ListConnectionsResponse connectionsResponse = peopleRequest.Execute();
            IList<Person> connections = connectionsResponse.Connections;
        }
    }
}
