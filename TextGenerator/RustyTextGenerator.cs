using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace TextGenerator
{
    /// <summary>
    /// Text generator meant to output individual sentences.
    /// Can use a variety of parsing modes.
    /// The name is just due to the first test of it resulting in the text "I am rusty".
    /// </summary>
    public class RustyTextGenerator : ITextGenerator
    {
        private Random Rand = new Random();
        private Dictionary<string, WordEntry> _words = new Dictionary<string, WordEntry>();
        private Dictionary<string, int> _FirstWords = new Dictionary<string, int>();
        private Dictionary<string, EndEntry> _LastWords = new Dictionary<string, EndEntry>();
        private ParsingDelegate _StateParser;

        public RustyTextGenerator()
        {
            _StateParser = StateParsingDelegates.ParseSingleWords;
        }

        public RustyTextGenerator(ParsingDelegate stateParser)
        {
            _StateParser = stateParser;
        }

        public void Consume(string ToParse)
        {

            var Sentences = new string[] { ToParse };
            //Parse each sentence
            foreach (string Sentence in Sentences)
                ParseSentence(Sentence);
        }

        public string Generate()
        {
            StringBuilder sb = new StringBuilder();
            string LastWord = GetRandomFirstWord();
            sb.Append(LastWord);
            bool Continue = true;
            while (Continue)
            {
                string NextWord = _words[LastWord].NextWord();
                //If we have a bad result or the last word of the sentence, stop there.
                if (NextWord == "" || IsLast(NextWord) || sb.Length > 1000)
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

        private void ParseSentence(string ToParse)
        {
            var Words = _StateParser(ToParse);
            if (Words.Count() > 0)
            {
                string FirstWord = Words.First();
                if (!_FirstWords.ContainsKey(FirstWord))
                    _FirstWords.Add(FirstWord, 0);
                _FirstWords[FirstWord] = _FirstWords[FirstWord] + 1;

                string LastWord = Words.Last();
                if (!_LastWords.ContainsKey(LastWord))
                    _LastWords.Add(LastWord, new EndEntry());
                _LastWords[LastWord].TimesLast = _LastWords[LastWord].TimesLast + 1;

                //Loop through the words and generate the hashtable of potential following words
                string PrevWord = null;
                foreach (string Word in Words)
                {
                    if (PrevWord != null)
                        _words[PrevWord].AddWord(Word);

                    if (!_words.ContainsKey(Word))
                        _words.Add(Word, new WordEntry());
                    else
                        _words[Word].Occurances += 1;

                    if (_LastWords.ContainsKey(Word))
                        _LastWords[Word].Occurances = _LastWords[Word].Occurances + 1;
                    PrevWord = Word;
                }
            }
        }

        private string GetRandomWord()
        {
            //With no mapped words we must return 0
            if (_words.Count() == 0)
                return "";

            //Otherwise, return a random entry out of the list of possibilities
            var Possibilites = new List<string>();
            foreach (string Key in _words.Keys)
            {
                for (int i = 0; i < _words[Key].Occurances; i++)
                {
                    Possibilites.Add(Key);
                }
            }
            return Possibilites[Rand.Next(0, Possibilites.Count - 1)];
        }

        private string GetRandomFirstWord()
        {
            //With no mapped words we must return 0
            if (_FirstWords.Count() == 0)
                return "";

            //Otherwise, return a random entry out of the list of possibilities
            var Possibilites = new List<string>();
            foreach (string Key in _FirstWords.Keys)
            {
                for (int i = 0; i < _FirstWords[Key]; i++)
                {
                    Possibilites.Add(Key);
                }
            }
            return Possibilites[Rand.Next(0, Possibilites.Count - 1)];
        }

        private bool IsLast(string Word)
        {
            if (_LastWords.ContainsKey(Word))
            {
                int Odds = (_LastWords[Word].TimesLast / _LastWords[Word].Occurances) * 100;
                return Rand.Next(0, 100) < Odds;
            }
            return false;
        }
    }
}
