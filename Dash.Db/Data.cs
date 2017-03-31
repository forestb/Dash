using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Dash.Db
{
    public static class Data
    {
        #region File Access Methods

        /// <summary>
        /// This file is marked as a resource and will always be included with the project output. Ideally this file
        /// should probably be downloaded and parsed from the source, but I'm unsure of the Wiresharks rules on 
        /// dynamically linking to their database.
        /// </summary>
        private static string Filepath = @"Data\manuf";

        private static string ReadFile()
        {
            string fileNameToProcess = Path.Combine(Environment.CurrentDirectory, Filepath);
            return File.ReadAllText(fileNameToProcess);
        }

        private static string[] ParseFile()
        {
            string[] splitNewline = new[] { Environment.NewLine, "\r", "\n" };

            return ReadFile().Split(splitNewline, StringSplitOptions.RemoveEmptyEntries);
        }

        #endregion

        private static List<Tuple<string, string>> ParseManufacturerDataSet()
        {
            List<Tuple<string, string>> manufacturerDataSet = new List<Tuple<string, string>>();
            
            foreach (var result in ParseFile())
            {
                var manufacturerData = ParseManufacturerData(result);

                if (manufacturerData != null)
                {
                    manufacturerDataSet.Add(manufacturerData);
                }
            }
            
            return manufacturerDataSet;
        }

        private static Tuple<string, string> ParseManufacturerData(string s)
        {
            s = s.Replace("\t", " ");

            string[] splitWhitespace = new[] { " " };

            if (IsMacAddressSubset(s))
            {
                var splitResults = s.Split(splitWhitespace, StringSplitOptions.RemoveEmptyEntries);

                return new Tuple<string, string>(splitResults[0].Replace(":", string.Empty), splitResults[1]);
            }

            return null;
        }

        /// <summary>
        /// Returns true if the first 8 characters in the string match the format of a valid MacAddress, e.g. 0A:BC:DE
        /// </summary>
        /// <remarks>
        /// The standard (IEEE 802) format for printing MAC-48 addresses in human-friendly form is six groups of two 
        /// hexadecimal digits, separated by hyphens - or colons :., so ^([0-9A-Fa-f]{2}[:-]){5}([0-9A-Fa-f]{2})$
        /// </remarks>
        /// <seealso cref="http://stackoverflow.com/questions/4260467/what-is-a-regular-expression-for-a-mac-address"/>
        /// <param name="s"></param>
        /// <returns></returns>
        private static bool IsMacAddressSubset(string s)
        {
            if (s.Length < 8)
            {
                return false;
            }

            return Regex.IsMatch(s.Substring(0, 8), "^([0-9A-Fa-f]{2}[:-]){2}([0-9A-Fa-f]{2})$");
        }

        public static List<string> AmazonDataSet
            => ParseManufacturerDataSet().Where(x => x.Item2 == "AmazonTe").Select(x => x.Item1).ToList();
    }
}
