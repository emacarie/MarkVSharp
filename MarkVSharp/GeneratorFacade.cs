using System;
using System.Text;

namespace MarkVSharp
{
	/// <summary>
	/// Combines MarkovGenerator methods to generate advanced text combinations
	/// </summary>
	public class GeneratorFacade
	{
		public MarkovGenerator Generator {get; private set;}
		private Random _rand ;
		
		public GeneratorFacade(MarkovGenerator gen)
		{
			Generator = gen ;
			_rand = new Random() ;
		}
		
		public GeneratorFacade(MarkovGenerator gen, Random rand)
		{
		    Generator = gen ;
		    _rand = rand ;
		}
		
		/// <summary>
		/// Convenience wrapper for generator GenerateWords
		/// </summary>
		/// <returns></returns>
		public string GenerateWords(int numWords)
		{
			return Generator.GenerateWords(numWords) ;
		}
		
		/// <summary>
		/// Convenience wrapper for generator GenerateSentence
		/// </summary>
		/// <returns></returns>
		public string GenerateSentence(int minLength)
		{
			return Generator.GenerateSentence(minLength) ;
		}
		
        /// <summary>
        /// Generate paragraphs using default settings
        /// </summary>
        /// <param name="numParagraphs"></param>
        /// <returns></returns>
		public string GenerateParagraphs(int numParagraphs)
		{
            return GenerateParagraphs(numParagraphs, new ParagraphParams());
		}
		
        /// <summary>
        /// Generate paragraphs using specified settings
        /// The number of sentences will be a randomly generated number
        /// between the minimum and maximum number of sentences
        /// </summary>
        /// <returns></returns>
		public string GenerateParagraphs(int numParagraphs, ParagraphParams inputParams)
		{
            if (numParagraphs < 1)
            {
                throw new Exceptions.InvalidArguments(
                    "The number of paragraphs should be at least one");
            }

            if (inputParams.MinSentences > inputParams.MaxSentences)
            {
                throw new Exceptions.InvalidArguments(
                    "Mininum sentences should be less than or equal to max sentences");
            }
            if (inputParams.MaxSentences < 2)
            {
                throw new Exceptions.InvalidArguments(
                    "Maximum sentences should be at least 2");
            }

            if (inputParams.MinSentences < 0 || inputParams.ParagraphIndent < 0 ||
                inputParams.SentenceIndent < 0)
            {
                throw new Exceptions.InvalidArguments(
                    "All arguments should be positive");
            }

            StringBuilder sb = new StringBuilder();
            
            for(int i = 0 ; i < inputParams.ParagraphIndent ; i++)
            {
                sb.Append(' ') ;
            }
            string paragraphIndent = sb.ToString() ;
            sb.Length = 0 ;
            
            for(int i = 0 ; i < inputParams.SentenceIndent ; i++)
            {
                sb.Append(' ') ;
            }
            string sentenceIndent = sb.ToString() ;
            
            StringBuilder completeParagraphs = new StringBuilder() ;
            
            for(int i = 0 ; i < numParagraphs ; i++)
            {
                sb.Length = 0 ;
                if(i > 0)
                {
                    sb.AppendLine() ;
                }
                sb.Append(paragraphIndent) ;
                int numSentences = _rand.Next(inputParams.MinSentences, inputParams.MaxSentences + 1) ;
                for(int j = 0 ; j < numSentences ; j ++)
                {
                    if(j > 0)
                    {
                        sb.Append(sentenceIndent) ;
                    }
                    string currSentence = Generator.GenerateSentence(Defs.DEFAULT_MIN_SENTENCE_LENGTH);
                    sb.Append(currSentence) ;
                }
                completeParagraphs.Append(sb) ;
            }
            return completeParagraphs.ToString() ;
		}

        /// <summary>
        /// Generate title which is a regular word generation except all the words are in upper case
        /// </summary>
        /// <param name="numWords"></param>
        /// <returns></returns>
        public string GenerateTitle(int numWords)
        {
            string lowerCaseTitle = Generator.GenerateWords(numWords);
            string[] lWords = lowerCaseTitle.Split();
            StringBuilder finalTitle = new StringBuilder();
            for (int i = 0; i < lWords.Length; i++ )
            {
                if (i != 0)
                {
                    finalTitle.Append(" ");
                }
                finalTitle.Append(lWords[i].Capitalize());
            }
            return finalTitle.ToString();
        }
	}
}

