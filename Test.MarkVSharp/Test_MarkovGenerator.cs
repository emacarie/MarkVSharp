/*
 * Created by SharpDevelop.
 * User: Emil
 * Date: 3/13/2011
 * Time: 9:08 PM
 *
 */
 
using System;
using System.Collections.Generic;
using NUnit.Framework;

using MarkVSharp;

namespace Test.MarkVSharp
{
	/// <summary>
	/// Test MarkovGenerator
	/// </summary>
    [TestFixture]
	public class Test_MarkovGenerator
	{
		private List<char> _defaultPunctuation = new List<char>{',', ';', '\"' };
		private List<char> _defaultSentenceEnd = new List<char>{'!', '?', '.'};
		private string _simpleString = @"This is a markov chain test in order to chain test words.
										is this a chain test ?  Grammar should work, or not !" ;
		Random _rand = new Random(1000) ;
		
		/// <summary>
		/// Test happy path Markov Generator Constructor
		/// </summary>
		[Test]
		public void Test_Constructor_Happy()
		{
			MarkovGenerator gen = new MarkovGenerator("Test Markov Gen More words typing here") ;
			CollectionAssert.AreEquivalent(gen.Punctuation, _defaultPunctuation) ;
			CollectionAssert.AreEquivalent(gen.SentenceEnd, _defaultSentenceEnd) ;
			
			gen = new MarkovGenerator("Test Markov Gen", 
			                          new GeneratorParams()
			                          {Punctuation = new List<char>{'a', 'b'},
			                          	SentenceEnd = new List<char> {'c', 'd'}}) ;
			CollectionAssert.AreEquivalent(gen.Punctuation, new List<char>{'a', 'b'}) ;
			CollectionAssert.AreEquivalent(gen.SentenceEnd, new List<char> {'c', 'd'}) ;
		}
		
		/// <summary>
		/// Test error edge cases for constructor
		/// </summary>
		[Test]
		public void Test_Constructor_Errors()
		{
			string initText = "Test Text For Test" ;
			Assert.Throws(typeof(Exceptions.InvalidArguments), 
			              delegate { new MarkovGenerator(initText, null);}) ;
			Assert.Throws(typeof(Exceptions.InvalidArguments), 
			              delegate { new MarkovGenerator(initText, new GeneratorParams() {Punctuation = null});}) ;
			Assert.Throws(typeof(Exceptions.InvalidArguments), 
			              delegate { new MarkovGenerator(initText, new GeneratorParams() {SentenceEnd = null});}) ;
			Assert.Throws(typeof(Exceptions.InvalidArguments), 
			              delegate { new MarkovGenerator(initText, new GeneratorParams() {ChainSize = 20});}) ;
		}
		
		/// <summary>
		/// Verify an exception is thrown when trying to generate a negative
        /// number of words
		/// </summary>
		[Test]
		public void Test_GetNumWords_NegativeNumWords()
		{
			MarkovGenerator gen = InitBaseMarkovGenerator() ;
            Assert.Throws<Exceptions.InvalidArguments>(delegate { gen.GenerateWords(-2); });
		}

        /// <summary>
        /// Verify GenerateWords generates correct number of non punctuation words
        /// </summary>
        [Test]
        public void Test_GenerateWords_CorrectNumber()
        {
            MarkovGenerator gen = InitBaseMarkovGenerator();
            List<char> correctSplit = new List<char>();
            correctSplit.Add(' ');
            correctSplit.AddRange(_defaultSentenceEnd);
            correctSplit.AddRange(_defaultPunctuation);
            string[] correctWords = _simpleString.Split(correctSplit.ToArray());
            string[] correctWordsLower = new string[correctWords.Length] ;
            for(int i = 0 ; i < correctWords.Length ; i++)
            {
            	correctWordsLower[i] = correctWords[i].ToLowerInvariant() ;
            }
            string[] words = gen.GenerateWords(20).Split(new char[]{' '});
            Assert.AreEqual(20, words.Length);
            foreach (string word in words)
            {
                CollectionAssert.DoesNotContain(_defaultPunctuation, word);
                CollectionAssert.DoesNotContain(_defaultSentenceEnd, word);
                CollectionAssert.Contains(correctWordsLower, word);
            }
        }

        /// <summary>
        /// Verify an exception is thrown when trying to generate a sentence with a
        /// negative number of words
        /// </summary>
        [Test]
        public void Test_GenerateSentence_NegativeNumWords()
        {
            MarkovGenerator gen = InitBaseMarkovGenerator();
            Assert.Throws<Exceptions.InvalidArguments>(delegate { gen.GenerateSentence(-2); });
        }

        /// <summary>
        /// Verify an exception is thrown when trying to generate a sentence with too high 
        /// min word count
        /// </summary>
        [Test]
        public void Test_GenerateSentence_TooManyMinWords()
        {
            MarkovGenerator gen = InitBaseMarkovGenerator();
            Assert.Throws<Exceptions.InvalidArguments>(delegate { gen.GenerateSentence(Defs.MAX_SENTENCE_LENGTH + 1); });
        }

        /// <summary>
        /// Verify that generate sentence generates a sentence which starts
        /// the same way as one of the sentences in the input and ends the same way
        /// </summary>
        [Test]
        public void Test_GenerateSentence_CorrectStart_CorrectEnd()
        {
            MarkovGenerator gen = InitBaseMarkovGenerator();
            bool thisIsAStart = false;
            bool isThisAStart = false;
            bool grammarShouldWorkStart = false;

            bool dotEnd = false;
            bool questionEnd = false;
            bool exclamationEnd = false;

            //Verify all the sentences starts and ends are used
            for (int i = 0; i < 30; i++)
            {
                string currSentence = gen.GenerateSentence(0);
                //sentence starts
                if (currSentence.StartsWith("This is a"))
                {
                    thisIsAStart = true;
                }
                else if (currSentence.StartsWith("Is this a"))
                {
                    isThisAStart = true;
                }
                else if (currSentence.StartsWith("Grammar should work"))
                {
                    grammarShouldWorkStart = true;
                }

                //sentence ends
                if (currSentence.EndsWith("."))
                {
                    dotEnd = true;
                }
                else if (currSentence.EndsWith("?"))
                {
                    questionEnd = true;
                }
                else if (currSentence.EndsWith("!"))
                {
                    exclamationEnd = true;
                }
            }

            Assert.IsTrue(thisIsAStart);
            Assert.IsTrue(isThisAStart);
            Assert.IsTrue(grammarShouldWorkStart);
            Assert.IsTrue(dotEnd);
            Assert.IsTrue(questionEnd);
            Assert.IsTrue(exclamationEnd);
        }
		
		/// <summary>
		/// Create simple Markov Generator to run tests with
		/// </summary>
		/// <returns></returns>
		public MarkovGenerator InitBaseMarkovGenerator()
		{
			MarkovGenerator gen = new MarkovGenerator(_simpleString, 
			                                          new GeneratorParams(){
			                                            Punctuation = _defaultPunctuation,
			                                            SentenceEnd = _defaultSentenceEnd, 
			                                            Rand = _rand}) ;
			return gen ;
		}
	}
}
