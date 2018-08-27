using Google.Apis.Auth.OAuth2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Google.Apis.PeopleService.v1;
using Google.GData.Contacts;
using System.Threading;
using Google.Apis.Util.Store;

namespace GoferEx.ExternalResources.Google
{
    public class aaa
    {
        private string _clientId;
        private string _clientSecret;

        public aaa(string appClientId, string appClientSecret)
        {
            _clientId = appClientId;
            _clientSecret = appClientSecret;
        }
        public async System.Threading.Tasks.Task AAsync()
        {
            UserCredential credential;
            using (var stream = new FileStream("client_secrets.json", FileMode.Open, FileAccess.Read))
            {
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    new[] {PeopleServiceService.Scope.Contacts},
                    "user", CancellationToken.None); //new FileDataStore("Books.ListMyLibrary"));
            }
        }

    }
}
