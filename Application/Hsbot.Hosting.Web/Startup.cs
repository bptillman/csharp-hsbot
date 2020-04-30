using Hsbot.Azure;
using Hsbot.Core;
using Hsbot.Slack;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Hsbot.Hosting.Web
{
    public class Startup
    {
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public Startup(IConfiguration config, IWebHostEnvironment webHostEnvironment)
        {
            _config = config;
            _webHostEnvironment = webHostEnvironment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging();
            services.AddHsbot(LoadConfig())
                .AddBrainStorageProvider<AzureBrainStorage>()
                .AddChatServices<HsbotSlackConnector, SlackChatMessageTextFormatter>();

            //This registration is what will actually run hsbot as a background
            //process within the website.  We'll need an external keep-alive to
            //make sure the site doesn't shut down when no HTTP requests are coming in.
            services.AddSingleton<IHostedService, HsbotHostedService>();
        }

        private HsbotConfig LoadConfig()
        {
            return new HsbotConfig
            {
                SlackApiKey = _config["slack:apiKey"],
                BrainName = _config["brain:name"] ?? "HsbotBrain",
                BrainStorageConnectionString = _config["brain:connectionString"] ?? "UseDevelopmentStorage=true",
                BrainStorageKey = _config["brain:storageKey"],
                TumblrApiKey = _config["tumblr:apiKey"],
                JiraApiKey = _config["jira:apiKey"],
                DictionaryApiKey = _config["dictionary:apiKey"],
                ThesaurusApiKey= _config["thesaurus:apiKey"],
            };
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseStaticFiles();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
                app.UseBlazorFrameworkFiles();
                app.UseRouting();
                app.UseEndpoints(endpoints =>
                {
                    //endpoints.MapBlazorHub();
                    endpoints.MapFallbackToFile("_content/Hsbot.Blazor.Client/debug.html");
                });
            }

            else
            {
                //We're not actually serving up any web content at present.
                //This website is only acting as a host for the hsbot service,
                //so no need to wire up anything other than a static reply.
                app.Run(async (context) =>
                {
                    await context.Response.WriteAsync("Beep boop bop");
                });
            }
        }
    }
}
