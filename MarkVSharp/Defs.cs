/*
 * Created by SharpDevelop.
 * User: Emil
 * Date: 1/18/2011
 * Time: 8:56 PM
 * 
 */
using System;

namespace MarkVSharp
{
    /// <summary>
    /// Miscelanous definitions
    /// </summary>
    public class Defs
    {
        /// <summary>
        /// Max iterations before a correct element matching a specified conditions is found
        /// This should apply to any of the random word generators
        /// </summary>
        public const int MAX_ITERATIONS = 100 ;
		
		/// <summary>
		/// Max length of a sentence 
		/// </summary>
		public const int MAX_SENTENCE_LENGTH = 50 ;
		
		/// <summary>
		/// Default minimum number of non punctuation words in a sentence
		/// </summary>
		public const int DEFAULT_MIN_SENTENCE_LENGTH = 3 ;
		
		/// <summary>
		/// Default minimum number of sentences in a paragraph
		/// </summary>
		public const int DEFAULT_MIN_SENTENCES = 4 ;
		
		/// <summary>
		/// Default maximum number of sentences in a paragraph
		/// </summary>
		public const int DEFAULT_MAX_SENTENCES = 15 ;
		
		/// <summary>
		/// Default paragraph indentation
		/// </summary>
		public const int DEFAULT_INDENT = 4 ;
    }
}
