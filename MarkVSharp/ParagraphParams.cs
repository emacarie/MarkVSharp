using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarkVSharp
{
    /// <summary>
    /// Container for parameters for generating a paragraph
    /// </summary>
    public class ParagraphParams
    {
        //minimm number of sentences
        public int MinSentences = 4;
        //maximum number of sentences
        public int MaxSentences = 8;
        //number of spaces before first sentences
        public int ParagraphIndent = 4;
        //number of spaces before sentences after the first
        public int SentenceIndent = 2;
    }
}
