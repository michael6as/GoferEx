# GoferContactManager


by Michael Assyag

### This is another branch I started yesterday because I had an idea. Apparantly this idea is the right one i think, but unfortunatly I don't have time for design this solution

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

## Remaining Issues
### Apparently, the Google Authentication Middleware is supported only for HTTPS requests.. (I tried to change iit to HTTP but for no good)
* "The oauth state is missing or invalid" - Done! The problem is with the Extension methods for external providers, it has some hard-coded params that wasn't mentioned. Moreover, there is a big catch clause for every error
* "Cookie was not set" - Done! At first the problem was that I wasn't requested to design a cookies mechanism for my app but Google authentication middleware requires cookies - further reading about CSRF Attacks. Added cookies and it worked
* I'm getting the acces_tokens and all and the last thing i need to do is handle the contacts results. The contacts can be retrieved with the access_token

My login Url is : https://accounts.google.com/o/oauth2/auth?redirect_uri=https://localhost:1906/signin-google&response_type=code&client_id=535817455358-tlgkg5jca5u3kjd3jlhqr4u1ef6udt7f.apps.googleusercontent.com&scope=https://www.googleapis.com/auth/userinfo.profile+https://www.googleapis.com/auth/plus.me+https://www.googleapis.com/auth/plus.profile.language.read+https://www.googleapis.com/auth/userinfo.email+https://www.googleapis.com/auth/plus.profile.agerange.read+https://www.googleapis.com/auth/contacts.readonly&approval_prompt=force&access_type=offline

The redirect from google to my Authentication Middleware is: http://localhost:1906/signin-google?code=4/SQAiSbRfmk4PA4cMtsMUC9Ait9xenEvfGlhDxw9aBuv8rmZXKregBDcfbBkaQwtSNhhD8SKzrQRtp9Q3XejxuEY#

