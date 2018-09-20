using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;

namespace GoferEx
{
  public class Program
  {
    public static void Main(string[] args)
    {
      CreateWebHostBuilder(args).ConfigureLogging(factory =>
        {
          factory.AddConsole();
          factory.AddFilter("Console", level => level >= LogLevel.Information);
        })
        .UseKestrel(options =>
        {
          options.Listen(IPAddress.Loopback, 1995);
        })
        .UseContentRoot(Directory.GetCurrentDirectory())
        .UseIISIntegration()
        //.UseStartup<Startup>()
        .Build().Run();
      //CreateWebHostBuilder(args).Build().Run();
    }

    public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
       WebHost.CreateDefaultBuilder(args)
           .UseStartup<Startup>();

    private static X509Certificate2 LoadCertificate()
    {
      using (var certificateStream = File.OpenRead(Path.Combine(Environment.CurrentDirectory, "cert.pfx")))
      {
        byte[] certificatePayload;
        using (var memoryStream = new MemoryStream())
        {
          certificateStream.CopyTo(memoryStream);
          certificatePayload = memoryStream.ToArray();
        }

        return new X509Certificate2(certificatePayload, "pass1995");
      }
    }
  }
}
