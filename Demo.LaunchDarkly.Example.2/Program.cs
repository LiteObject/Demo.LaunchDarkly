using System;
using System.Linq;
using LaunchDarkly.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Demo.LaunchDarkly.Example._2
{
    class Program
    {
        private static readonly IServiceProvider Provider;

        static Program()
        {
            IServiceCollection services = new ServiceCollection();
            Provider = ConfigureServices(services);
        }

        static void Main(string[] args)
        {
            LaunchDarklyOptions launchDarklyOptions = Provider.GetRequiredService<IOptions<LaunchDarklyOptions>>().Value;
            User user = User.WithKey(Environment.UserName); // proxynumber-sh123-call-twilio-api

            // Example 1: How to call LaunchDarkly directly using LDClient 
            /* var ldClient = Provider.GetRequiredService<ILdClient>();
            bool showFeature = ldClient.BoolVariation(launchDarklyOptions.FeatureFlagKey, user, false); */

            // Example 2: How to wrap LaunchDarkly with a service
            var featureService = Provider.GetRequiredService<IFeatureFlagsService>();
            bool showFeature = featureService.GetFeatureFlag<bool>(launchDarklyOptions.FeatureFlagKey, user.Key);

            if (showFeature)
            {
                ShowMessage($"Feature flag \"{launchDarklyOptions.FeatureFlagKey}\" is \"{showFeature}\" for user \"{user.Key}\"");
            }
            else
            {
                ShowMessage($"Feature flag \"{launchDarklyOptions.FeatureFlagKey}\" is \"{showFeature}\" for user \"{user.Key}\"", ConsoleColor.Yellow);
            }
        }

        private static ServiceProvider ConfigureServices(IServiceCollection services)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            services.Configure<LaunchDarklyOptions>(options => configuration.GetSection(nameof(LaunchDarklyOptions)).Bind(options));

            /*var launchDarklyOptions = new LaunchDarklyOptions();
               configuration.GetSection(LaunchDarklyOptions.ConfigSectionKey).Bind(launchDarklyOptions);*/

            services.AddSingleton<ILdClient, LdClient>(sp =>
            {
                // var launchDarklyOptions = configuration.GetSection(LaunchDarklyOptions.ConfigSectionKey).Get<LaunchDarklyOptions>();
                var launchDarklyOptions = sp.GetRequiredService<IOptions<LaunchDarklyOptions>>().Value;

                if(string.IsNullOrWhiteSpace(launchDarklyOptions.SdkKey) 
                   || string.IsNullOrWhiteSpace(launchDarklyOptions.FeatureFlagKey))
                {
                    throw new ArgumentException($"LaunchDarkly {nameof(launchDarklyOptions.SdkKey)} or {nameof(launchDarklyOptions.FeatureFlagKey)} is missing.");
                }

                return new LdClient(launchDarklyOptions.SdkKey);
            });

            services.AddSingleton<IFeatureFlagsService, FeatureFlagService>();
             
            DisplayRegisteredTypes(services);

            return services.BuildServiceProvider();
        }

        private static void ShowMessage(string value, ConsoleColor c = ConsoleColor.Cyan)
        {
            Console.ForegroundColor = c;
            Console.WriteLine(value);
            Console.ResetColor();
        }
        
        /// <summary>
        /// This private method is not necessary for this demonstration.
        /// </summary>
        /// <param name="services"></param>
        private static void DisplayRegisteredTypes(IServiceCollection services)
        {
            var filteredServices = services
                .Where(s => s.ServiceType.FullName != null && !s.ServiceType.FullName.StartsWith("Microsoft"))
                .Select(s => new
                {
                    TypeName = s.ServiceType.Name,
                    ImplName = s.ImplementationType?.Name ?? s.ImplementationFactory?.Method.ReturnType.Name,
                    LifeTime = s.Lifetime
                })
                .OrderBy(s => s.TypeName);

            foreach (var service in filteredServices)
            {
                ShowMessage($"- {service.TypeName} -> {service.ImplName} [{service.LifeTime}]", ConsoleColor.DarkBlue);
            }
            
            ShowMessage(Environment.NewLine);
        }
    }
}
