# GoferContactManager
by Michael Assyag

Web API for managing user's contacts and syncing with external resources (e.g Facebook, Google, Microsoft etc.)

### Prerequisites

You need .NET core 2.1 SDK installed

## Deployment

After finished installing we can run the program

First, we run the Node.js static page provider. In the folder where the package.json at run:
```
    npm install
    npm run dev
```

## Issues

The Google Auth isn't complete in my app unfortunatly..
I followed the instructions both in Microsft Docs & Google Developer and ended up with the following problem:
* When I try to authenticate via Google I'm able to send the login request to Google API but when I get the Redirect from Google (http://localhost:1906/signin-google) I'm getting an error page with this error:
The error Code is 500
```
An unhandled exception occurred while processing the request.
Exception: The oauth state was missing or invalid.
Unknown location

Exception: An error was encountered while handling the remote login.
Microsoft.AspNetCore.Authentication.RemoteAuthenticationHandler<TOptions>.HandleRequestAsync()
```
My login Url is : https://accounts.google.com/o/oauth2/auth?redirect_uri=http://localhost:1906/signin-google&response_type=code&client_id=535817455358-tlgkg5jca5u3kjd3jlhqr4u1ef6udt7f.apps.googleusercontent.com&scope=https://www.googleapis.com/auth/userinfo.profile+https://www.googleapis.com/auth/plus.me+https://www.googleapis.com/auth/plus.profile.language.read+https://www.googleapis.com/auth/userinfo.email+https://www.googleapis.com/auth/plus.profile.agerange.read+https://www.googleapis.com/auth/contacts.readonly&approval_prompt=force&access_type=offline

The redirect from google to my Authentication Middleware is: http://localhost:1906/signin-google?code=4/SQAiSbRfmk4PA4cMtsMUC9Ait9xenEvfGlhDxw9aBuv8rmZXKregBDcfbBkaQwtSNhhD8SKzrQRtp9Q3XejxuEY#

If this was working right the following step was to exchange the AuthCode I got from Google to access_token and refresh_token.
With these tokens I can retrieve information from Google.

I think the problem is something with Google Authentication Middleware in ASP.NET Core..

