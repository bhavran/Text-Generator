using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextGenerator
{
    public interface ITextGenerator
    {
        void Consume(string text);
        string Generate();
    }
}
