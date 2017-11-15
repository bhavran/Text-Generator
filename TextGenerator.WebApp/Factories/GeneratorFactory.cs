using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TextGenerator.WebApp
{
    public static class GeneratorFactory
    {
        public static ITextGenerator BuildGenerator(Models.CreateGeneratorRequest request)
        {
            ITextGenerator textGenerator;
            if (request.GeneratorName == "rusty")
                textGenerator = new RustyTextGenerator();
            else if (request.GeneratorName == "simple")
                textGenerator = new SimpleTextGenerator();
            else
                throw new Exception("Invalid generator name");

            foreach(var source in request.Sources)
            {
                if(source.Source == "reddit")
                {
                    var reddit = new TextSources.RedditTitles(source.Parameter);
                    foreach (var chunk in reddit.GetTextFromSource())
                        textGenerator.Consume(chunk);
                }
                else
                {
                    throw new Exception("Invalid source name");
                }
            }
            return textGenerator;
        }
    }
}