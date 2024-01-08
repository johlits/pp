using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using pp.Interfaces;

namespace pp.Plugins
{
    internal class TimeZones : IPlugin
    {
        private (string, string)[] timeZoneIds = { 
            ("Pacific Standard Time", "PST"), 
            ("Eastern Standard Time", "EST"), 
            ("UTC", "UTC"), 
            ("GMT Standard Time", "GMT"), 
            ("Central Europe Standard Time", "CET"), 
            ("New Zealand Standard Time", "NZDT") 
            };

        public string GetDescription()
        {
            return "Type /tz to print current time in different zones.";
        }

        public bool ActivatesOn(string input)
        {
            return input == "/tz";
        }

        public void Execute(string input)
        {
            // Get current UTC time
            DateTime utcNow = DateTime.UtcNow;

            foreach ((string, string) timeZoneId in timeZoneIds)
            {
                // Find the time zone
                TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId.Item1);

                // Convert time from UTC to the desired time zone
                DateTime convertedTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, timeZone);

                // Print the time
                Core.DisplayMessage(Core.MC.None, Core.GetUserAlias(), $"{timeZoneId.Item2}\t{convertedTime}");
                Core.GetInterface().ClearInput();
            }
        }

        public List<string> GetCommands()
        {
            return new List<string> { "/tz" };
        }

        public void Ping(int no)
        {
            return;
        }

        public string GetPluginName()
        {
            return "timezones";
        }
    }
}
