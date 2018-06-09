using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FplBot
{
    internal static class TextUtilities
    {
        private static Random rng;

        /// <summary>
        /// Join a list of string items together with natural use of commas and the word 'and'
        /// </summary>
        /// <param name="builderToAppendTo">The builder to append the string to</param>
        /// <param name="strings">The list of strings we want to join</param>
        /// <remarks>Uses Oxford comma rules</remarks>
        internal static void StringJoinWithCommasAndAnd(StringBuilder builderToAppendTo, IEnumerable<string> strings)
        {
            if (strings.Count() == 0)
            {
                return;
            }

            for (int count = 0; count < strings.Count(); count++)
            {
                if (strings.Count() == 1 || (count < strings.Count() - 1 && strings.Count() < 3))
                {
                    builderToAppendTo.Append($"{strings.ElementAt(count)}");
                }
                else if (count < strings.Count() - 1)
                {
                    builderToAppendTo.Append($"{strings.ElementAt(count)}, ");
                }
                else
                {
                    if (builderToAppendTo[builderToAppendTo.Length - 1] == ' ')
                    {
                        builderToAppendTo.Remove(builderToAppendTo.Length - 1, 1);
                    }

                    builderToAppendTo.Append($" and {strings.ElementAt(count)}");
                }
            }
        }

        /// <summary>
        /// Returns 's' if count is anything other than 1
        /// </summary>
        /// <param name="count">The count of the thing we want to potentially pluralize</param>
        /// <returns>And 's' if count is more or less than 1</returns>
        internal static string Pluralize(int count)
        {
            if (count == 1)
            {
                return string.Empty;
            }

            return "s";
        }

        /// <summary>
        /// Returns was if singular, were if plural
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        internal static string WasWere(int count)
        {
            if (count > 1)
            {
                return "were";
            }
            else
            {
                return "was";
            }
        }

        /// <summary>
        /// Gets a random adjective to describe a poor situation
        /// </summary>
        /// <returns>A word</returns>
        internal static string GetPoorAdjective()
        {
            if (rng == null)
            {
                rng = new Random();
            }

            List<string> adjectives = new List<string>();

            adjectives.Add("paltry");
            adjectives.Add("miserable");
            adjectives.Add("insignificant");
            adjectives.Add("meager");
            adjectives.Add("pitiful");
            adjectives.Add("shabby");
            adjectives.Add("inadequate");

            return adjectives[rng.Next(0, adjectives.Count)];
        }

        /// <summary>
        /// Gets a random adjective to describe a good situation
        /// </summary>
        /// <returns></returns>
        internal static string GetGoodAdjective()
        {
            if (rng == null)
            {
                rng = new Random();
            }

            List<string> adjectives = new List<string>();

            adjectives.Add("astonishing");
            adjectives.Add("amazing");
            adjectives.Add("extraordinary");
            adjectives.Add("spectacular");
            adjectives.Add("stunning");
            adjectives.Add("astounding");            
            adjectives.Add("incredible");

            return adjectives[rng.Next(0, adjectives.Count)];
        }
    }
}
