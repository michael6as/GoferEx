# GoferContactManager

by Michael Assyag

Web API for managing user's contacts and syncing with external resources (e.g Facebook, Google, Microsoft etc.)

### Prerequisites

*You need .NET core 2.1 SDK installed
*Redis Server installed and running - [Redis](https://redis.io/)

## Deployment

After finished installing we can run the program

First, we run the Node.js static page provider. In the folder where the package.json at run:
```
    npm install
    npm run dev
```
After the node server is running we can start the WebAPI. At the folder 'GoferEx.Server' run the following command:
```    
    dotnet run bin/Debug/GoferEx.Server.dll
```
If there is a problem running the C# application, open the .sln in VS and restore NuGet packages

## Built with
* .NET Core v2.1
* VueJS
* Redis - In-Memory Database for quick DB actions
