//using Azure.Extensions.AspNetCore.Configuration.Secrets;
//using Azure.Identity;
//using Azure.Security.KeyVault.Secrets;
//using LCU.Hosting;
//using Microsoft.AspNetCore;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Logging.ApplicationInsights;
//using Microsoft.Extensions.Logging.AzureAppServices;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Fathym.LCU.Services.StateAPIs
//{
//    /// <summary>
//    ///     Class used for constructing the standard LCU Host for web apps
//    /// </summary>
//    /// <example>
//    ///     using LCU.Runtime.Hosting;
//    ///     using System.Threading.Tasks;

//    ///     namespace LCU.Runtime.Web
//    ///     {
//    ///         public class Program
//    ///         {
//    ///             public static async Task Main(string[] args)
//    ///             {
//    ///                 await LCUHostBuilder<Startup>.Start(args);
//    ///             }
//    ///         }
//    ///     }
//    /// </example>
//    /// <typeparam name="TStartup">The type of the class to use for Startup.</typeparam>
//    public class LCUFunctionHostBuilder<TStartup> : LCUHostBuilder<TStartup>
//        where TStartup : class
//    {
//        #region Fields
//        #endregion

//        #region Constructors
//        public LCUFunctionHostBuilder(string[] args)
//            : this(Host.CreateDefaultBuilder(args ?? Array.Empty<string>()))
//        { }

//        public LCUFunctionHostBuilder(IHostBuilder innerHostBuilder)
//            : base(innerHostBuilder)
//        { }
//        #endregion

//        #region Static
//        public static async Task StartFunctionHost(string[] args)
//        {
//            var hostBldr = new LCUFunctionHostBuilder<TStartup>(args);

//            hostBldr.ConfigureWebHost();

//            await hostBldr.Build();
//        }
//        #endregion

//        #region API Methods
//        public override void ConfigureWebHost()
//        {
//            logger.LogInformation($"Configuring function host for {GetType().FullName}");

//            innerHostBuilder = innerHostBuilder;

//            logger.LogInformation($"Configured function for {GetType().FullName}");

//            //base.ConfigureWebHost();
//        }
//        #endregion

//        #region Helpers
//        protected override void configureServices(IServiceCollection services)
//        {
//            base.configureServices(services);
//        }
//        #endregion
//    }
//}
