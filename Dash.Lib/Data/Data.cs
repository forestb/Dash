using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Dash.Lib.Data
{
    public static class Data
    {
        #region File Access Methods

        private static string ReadEmbeddedResource()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Dash.Lib.Data.manuf";

            string result = string.Empty;

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                result = reader.ReadToEnd();
            }

            return result;
        }

        private static string[] ParseFile()
        {
            string[] splitNewline = new[] { Environment.NewLine, "\r", "\n" };

            return ReadEmbeddedResource().Split(splitNewline, StringSplitOptions.RemoveEmptyEntries);
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
            if (!IsMacAddressSubset(s))
            {
                return null;
            }

            // the format is
            // mac <tab> manufacturer <space><space>...<space> // comment
            // update the format to simplify parsing and accessing the first and second element (the useful info)
            s = s.Replace("\t", " ");

            string[] splitWhitespace = new[] { " " };
            string[] splitResults = s.Split(splitWhitespace, StringSplitOptions.RemoveEmptyEntries);

            return new Tuple<string, string>(splitResults[0].Replace(":", string.Empty), splitResults[1]);
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
