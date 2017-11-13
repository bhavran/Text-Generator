using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace TextGenerator
{
    /// <summary>
    /// A simple text generator that generates potentially huge chunks of text
    /// </summary>
    public class SimpleTextGenerator : ITextGenerator
    {
        private Random Rand = new Random();
        private Dictionary<string, WordEntry> MappedWords = new Dictionary<string, WordEntry>();
        private ParsingDelegate _StateParser;
        private int _maxLength = 1000;

        public SimpleTextGenerator()
        {
            _StateParser = StateParsingDelegates.RegexParse;
        }

        public SimpleTextGenerator(int maxLength)
        {
            _maxLength = maxLength;
        }

        public SimpleTextGenerator(ParsingDelegate stateParser)
        {
            _StateParser = stateParser;
        }

        public SimpleTextGenerator(int maxLength,ParsingDelegate stateParser)
        {
            _maxLength = maxLength;
            _StateParser = stateParser;
        }

        public void Consume(string ToParse)
        {
            var Words = _StateParser(ToParse);
            //Loop through the words and generate the hashtable of potential following words
            string PrevWord = null;
            foreach (string Word in Words)
            {
                if (PrevWord != null)
                    MappedWords[PrevWord].AddWord(Word);

                if (!MappedWords.ContainsKey(Word))
                    MappedWords.Add(Word, new WordEntry());
                else
                    MappedWords[Word].Occurances += 1;
                PrevWord = Word;
            }
        }

        public string Generate()
        {
            StringBuilder sb = new StringBuilder();
            string LastWord = GetRandomWord();
            sb.Append(LastWord);
            bool Continue = true;
            while (Continue)
            {
                string NextWord = MappedWords[LastWord].NextWord();
                //If we have a bad result or the last word of the sentence, stop there.
                if (NextWord == "" || sb.Length > _maxLength)
                {
                    Continue = false;
                }

                if (NextWord != "")
                {
                    sb.Append(" ");
                    sb.Append(NextWord);
                    LastWord = NextWord;
                }
            }
            sb.Append(".");
            return sb.ToString();

        }

        private string GetRandomWord()
        {
            //With no mapped words we must return 0
            if (MappedWords.Count() == 0)
                return "";

            //Otherwise, return a random entry out of the list of possibilities
            var Possibilites = new List<string>();
            foreach (string Key in MappedWords.Keys)
            {
                for (int i = 0; i < MappedWords[Key].Occurances; i++)
                {
                    Possibilites.Add(Key);
                }
            }
            return Possibilites[Rand.Next(0, Possibilites.Count - 1)];
        }

    }
}
