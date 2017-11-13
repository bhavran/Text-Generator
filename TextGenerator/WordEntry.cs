using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextGenerator
{
    class WordEntry
    {
        static Random Rand = new Random();
        Dictionary<string, int> FollowingWords = new Dictionary<string, int>();

        public int Occurances = 1;

        public string NextWord()
        {
            //If this word has no "next" words, return blank
            if (FollowingWords.Count == 0)
                return "";

            var Possibilites = new List<string>();
            foreach (string Key in FollowingWords.Keys)
            {
                for (int i = 0; i < FollowingWords[Key]; i++)
                {
                    Possibilites.Add(Key);
                }
            }
            return Possibilites[Rand.Next(0, Possibilites.Count - 1)];
        }

        public void AddWord(string Next)
        {
            if (FollowingWords.ContainsKey(Next))
                FollowingWords[Next] = FollowingWords[Next] + 1;
            else
                FollowingWords[Next] = 1;
        }
    }
}
