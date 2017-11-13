using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextGenerator.TextSources
{
    public interface ITextSource
    {
        IEnumerable<string> GetTextFromSource();
    }
}
