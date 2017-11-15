using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TextGenerator;
using TextGenerator.TextSources;

namespace ConsoleTextGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var theDonald = new RedditTitles("the_donald");
            var td_items = theDonald.GetTextFromSource();
            var raobj = new RedditTitles("randomActsofBlowjob");
            var bj_items = raobj.GetTextFromSource();
            var textGen = new RustyTextGenerator(StateParsingDelegates.ParseSingleWords);
            bj_items.ToList().ForEach(x => textGen.Consume(x));
            td_items.ToList().ForEach(x => textGen.Consume(x));
            while (true)
            {
                Console.WriteLine(textGen.Generate());
                Console.ReadKey();
            }
        }
    }
}
