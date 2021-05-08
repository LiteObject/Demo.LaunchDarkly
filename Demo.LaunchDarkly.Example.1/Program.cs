using System;
using LaunchDarkly.Client;

namespace Demo.LaunchDarkly
{
    /// <summary>
    /// Original Source: https://github.com/launchdarkly/hello-dotnet/blob/master/HelloDotNet/Hello.cs
    /// </summary>
    class Program
    {
        public const string SdkKey = "";
        public const string FeatureFlagKey = "";

        static void Main(string[] args)
        {
            if (string.IsNullOrEmpty(SdkKey))
            {
                ShowMessage($"Please set \"{nameof(SdkKey)}\" first.", ConsoleColor.Yellow);
                Environment.Exit(1);
            }

            // LDClient must be a singleton. Be sure that you're not instantiating a new client with every request.
            using LdClient ldClient = new LdClient(SdkKey);
            
            User user = User.WithKey("proxynumber-sh123-call-twilio-api");

            bool showFeature = ldClient.BoolVariation(FeatureFlagKey, user, false);


            if (showFeature)
            {
                ShowMessage($"Feature flag \"{FeatureFlagKey}\" is \"{showFeature}\" for user \"{user.Key}\"");
            }
            else
            {
                ShowMessage($"Feature flag \"{FeatureFlagKey}\" is \"{showFeature}\" for user \"{user.Key}\"", ConsoleColor.Yellow);
            }

            // Here we ensure that the SDK shuts down cleanly and has a chance to deliver analytics
            // events to LaunchDarkly before the program exits. If analytics events are not delivered,
            // the user properties and flag usage statistics will not appear on your dashboard. In a
            // normal long-running application, the SDK would continue running and events would be
            // delivered automatically in the background.
            // ldClient.Dispose();
        }

        private static void ShowMessage(string s, ConsoleColor c = ConsoleColor.Cyan)
        {
            Console.ForegroundColor = c;
            Console.WriteLine(s);
            Console.ResetColor();
        }
    }
}
