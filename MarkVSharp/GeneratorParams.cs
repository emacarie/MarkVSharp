/*
 * Created by SharpDevelop.
 * User: Emil
 * Date: 3/14/2011
 * Time: 11:00 PM
 * 
 * 
 */
using System;
using System.Collections.Generic;

namespace MarkVSharp
{
	/// <summary>
	/// Contains parameters needed for the Markov Generator
	/// </summary>
	public class GeneratorParams
	{
		//Punctuation characters
		public List<char> Punctuation;
		
		//Characters that represent the end of a sentence
		public List<char> SentenceEnd;
		
		//Random number generator that servers as a seed
		public Random Rand;
		
		//How many words the markov chain consists of
		public int ChainSize;
		
		/// <summary>
		/// Set default parameters
		/// </summary>
		public GeneratorParams()
		{
			Punctuation = new List<char> {',', ';', '\"' } ;
			SentenceEnd = new List<char> {'!','?','.'} ;
			Rand = new Random() ;
			ChainSize = 2;
		}
	}
}
