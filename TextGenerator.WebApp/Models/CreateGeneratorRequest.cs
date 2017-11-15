using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TextGenerator.WebApp.Models
{
    public class CreateGeneratorRequest
    {
        public string GeneratorName { get; set; }
        public IEnumerable<GeneratorTextSource> Sources { get; set; }
    }

    public class GeneratorTextSource
    {
        public string Source { get; set; }
        public string Parameter { get; set; }
    }
}