using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace TextGenerator
{
    public delegate IEnumerable<string> ParsingDelegate(string str);
    /// <summary>
    /// A collection of static delegate methods which can be used to parse out the individual states
    /// for the markov chains.
    /// </summary>
    public class StateParsingDelegates
    {
        public const string BasicRegex = @"((?:the |an? )*(?:\S)+(?: (?:\w\w?\w? ) *\w+)*)";
        /// <summary>
        /// Uses regex to parse the given string into words and phrases
        /// </summary>
        public static IEnumerable<string> RegexParse(string ToParse)
        {
            List<string> Results = new List<string>();
            var Split = Regex.Split(ToParse, BasicRegex, RegexOptions.IgnoreCase)
                .Where(x => x.Trim() != "")
                        .Select(x =>
                            x.TrimEnd(new char[] { ',', '.', '!', ':', ';', '?', '"', ')' })
                             .TrimStart(new char[] { '"', '(' }));
            return Split.ToList();
        }

        /// <summary>
        /// Parses a string into single words.
        /// </summary>
        public static IEnumerable<string> ParseSingleWords(string ToParse)
        {
            ToParse = ToParse.Replace("(", "").Replace(")", "").Replace("\"", "").Replace("\\", "").Replace("/", "");
            return ToParse.Split(new string[] { " ", ",", ";", ":", "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        /// <summary>
        /// Parses a string into pairs of words
        /// </summary>
        public static IEnumerable<string> Parse2Words(string ToParse)
        {
            List<string> Pairs = new List<string>();
            ToParse = ToParse.Replace("(", "").Replace(")", "").Replace("\"", "").Replace("\\", "").Replace("/", "");
            var Words = ToParse.Split(new char[] { ' ', ';', '\n' }).Where(x => x.Trim() != "");
            for (int i = 0; i < Words.Count(); i += 2)
            {
                if (i < Words.Count() - 2)
                {
                    Pairs.Add(string.Concat(Words.ElementAt(i), " ", Words.ElementAt(i + 1)));
                }
                else if (i < Words.Count() - 1)
                {
                    Pairs.Add(Words.ElementAt(i));
                }
            }
            return Pairs;
        }



    }
}
