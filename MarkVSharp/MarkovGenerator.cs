/*
 * Created by SharpDevelop.
 * User: Emil
 * Date: 3/13/2011
 * Time: 8:55 PM
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarkVSharp
{
	/// <summary>
	/// Text generator.  Takes text input and generates sentences, words, etc... 
	/// </summary>
	public class MarkovGenerator
	{
		private ChainGenerator _chainGenerator ;
		
		//Used to generate individual words
		private WordGenerator _wordGenerator ;
		
		//Used to generate sentences
		private WordGenerator _sentenceGenerator ;
		public List<char> Punctuation{get; private set;}
		public List<char> SentenceEnd{get; private set;}
        private List<char> _allPunctuation ;
		public int ChainSize{get; private set;}
		
		/// <summary>
		/// Input text from which to build Markov text
		/// </summary>
		/// <param name="text"></param>
		public MarkovGenerator(string text):this(text, new GeneratorParams()){}
		
		public MarkovGenerator(string text, GeneratorParams inputParams)
		{
			//Verify input is sane
			if(inputParams == null)
			{
				throw new Exceptions.InvalidArguments("Null Generator Parameters") ;
			}
			if(inputParams.Punctuation == null)
			{
				throw new Exceptions.InvalidArguments("Null punctuation list") ;
			}
			if(inputParams.SentenceEnd == null)
			{
				throw new Exceptions.InvalidArguments("Null end of sentence") ;
			}
			
			Punctuation = inputParams.Punctuation ;
			SentenceEnd = inputParams.SentenceEnd ;
			_allPunctuation = new List<char>() ;
			_allPunctuation.AddRange(Punctuation) ;
			_allPunctuation.AddRange(SentenceEnd) ;
			_allPunctuation = new List<char>(_allPunctuation.Distinct()) ;
			
			_chainGenerator = new ChainGenerator(text, _allPunctuation.ToArray(), inputParams.ChainSize) ;
			_chainGenerator.GenerateChains() ;
            _wordGenerator = new WordGenerator(_chainGenerator.Chains, inputParams.Rand) ;
            _sentenceGenerator = new WordGenerator(_chainGenerator.Chains, inputParams.Rand) ;
            
		}

        /// <summary>
        /// Generate fixed number of non punctuation words
        /// All words will be lower case
        /// </summary>
        /// <param name="numWords"></param>
        /// <returns></returns>
        public string GenerateWords(int numWords)
        {
            if (numWords < 1)
            {
                throw new Exceptions.InvalidArguments("Number of words generated must be positive");
            }
            StringBuilder sb = new StringBuilder();
            if(_wordGenerator.SubchainsInitialized)
            {
                _wordGenerator.ResetReadOnly() ;
            }
            else
            {
                _wordGenerator.ResetSubchains() ;
            }
            
            for (int i = 0; i < numWords; i++)
            {
                if (i != 0)
                {
                    sb.Append(" ");
                }
				
				sb.Append(_wordGenerator.GetNextWord(w => !IsPunctuation(w), Defs.MAX_ITERATIONS)) ;

            }
            return sb.ToString();
        }

        /// <summary>
        /// Generate a new sentece that contains a minimum number of non punctuation words
        /// This minimum number of words is not guaranteed, but it is a best try
        /// </summary>
        /// <param name="minLength">Minimum length of sentence.  Use 0 to indicate
        /// no minimum</param>
        /// <returns></returns>
        public string GenerateSentence(int minLength)
        {
            if (minLength < 0)
            {
                throw new Exceptions.InvalidArguments("Number of words generated must be positive");
            }

            if (minLength > Defs.MAX_SENTENCE_LENGTH)
            {
                throw new Exceptions.InvalidArguments("Minimum sentence length greater than default maximum of " + 
                    Defs.MAX_SENTENCE_LENGTH);
            }

            StringBuilder sb = new StringBuilder();

            //Word generator should reset to beginning of a sentence if needed
            ChainCondition sentenceStartCondition = new ChainCondition(
                ck => IsSentenceEnd(ck.Words[ck.Words.Length - 1]) || 
                    ck.Words[ck.Words.Length-1] == null, w => !IsPunctuation(w)) ;
            
            //redundant for clarity
            if(!_sentenceGenerator.SubchainsInitialized)
            {
            	_sentenceGenerator.ResetSubchains(sentenceStartCondition, null);
            	
            }
            else if(IsSentenceEnd(_sentenceGenerator.CurrentWord))
            {
            	//We are already at the beginning of a new sentence
            	//This is to try to optimize perfomance so that we don't do a reset more than necessary
            	//A reset is an expensive operation as all chains need to be parsed
            	_sentenceGenerator.GetNextWord(sentenceStartCondition.WordCondition, Defs.MAX_ITERATIONS) ;
            }
            
            if(!(sentenceStartCondition.KeyCondition(_sentenceGenerator.CurrKey) &&
                 (sentenceStartCondition.WordCondition(_sentenceGenerator.CurrentWord))))
            {
            	_sentenceGenerator.ResetReadOnly();
            }
						
			int countWords ;
			for(countWords = 0 ; countWords < Defs.MAX_SENTENCE_LENGTH ; countWords++)
			{
				string nextWord = _sentenceGenerator.CurrentWord;
				
				if(!(countWords == 0 || IsPunctuation(nextWord)))
				{
					//prepend space if not first word or punctuation
					sb.Append(" ") ;
					sb.Append(nextWord) ;
				}
				else if (countWords == 0)
				{
					//capitalize first word
					if(!string.IsNullOrEmpty(nextWord))
					{
						sb.Append(nextWord.Capitalize()) ;
					}
				}
				else
				{
					sb.Append(nextWord) ;
				}
				
				//If we get to sentence end, we are done
				if(IsSentenceEnd(nextWord))
				{
					break ;
				}
				
				if(countWords < minLength)
				{
					_sentenceGenerator.GetNextWord(w => !IsSentenceEnd(w), Defs.MAX_ITERATIONS) ;
				}
				else
				{
					_sentenceGenerator.GetNextWord() ;
				}
			}
			
			if(countWords == Defs.MAX_SENTENCE_LENGTH)
			{
				//shouldn't get here, but if we do, put any random sentence end
				sb.Append(SentenceEnd.FirstOrDefault()) ;
			}
            return sb.ToString();

        }

        private bool IsPunctuation(string inputString)
        {
            return _allPunctuation.Exists(c => c.ToString() == inputString);
        }

        private bool IsSentenceEnd(string inputString)
        {
            return SentenceEnd.Exists(c => c.ToString() == inputString);
        }
		
	}
}
