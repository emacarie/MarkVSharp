using System;
using System.Text;
using NUnit.Framework;

using MarkVSharp;

namespace Test.MarkVSharp
{
	/// <summary>
	/// Test GeneratorFacade class
	/// </summary>
	[TestFixture]
	public class Test_GeneratorFacade
	{
		/// <summary>
		/// Verify GenerateParagraphs generates the appropriate amount of 
		/// sentences for one paragraph
		/// </summary>
		[Test]
		public void T_GenerateParagraphs_Length()
		{
            GeneratorFacade facade = InitBaseFacade();
            //seven sentences should be generated
            string resultStr = facade.GenerateParagraphs(3, new ParagraphParams 
                { MinSentences = 7, MaxSentences = 7 });
            string[] splitSentences = resultStr.Split(facade.Generator.SentenceEnd.ToArray(), 
                                                      StringSplitOptions.RemoveEmptyEntries);
            Assert.AreEqual(21, splitSentences.Length); 
        }

        /// <summary>
        /// Verify indentation is correct both at the paragraph level and
        /// at the sentence level
        /// </summary>
        [Test]
        public void T_GenerateParagraphs_Indentation()
        {
            GeneratorFacade facade = InitBaseFacade();
            ParagraphParams defaultParams = new ParagraphParams { ParagraphIndent = 8, SentenceIndent = 4 };
            string resultStr = facade.GenerateParagraphs(5, defaultParams);
            StringBuilder sb = new StringBuilder();
            
            for (int i = 0; i < defaultParams.ParagraphIndent; i++)
            {
                sb.Append(" ");
            }
            string paragraphIndent = sb.ToString();
            sb.Length = 0;

            for (int i = 0; i < defaultParams.SentenceIndent; i++)
            {
                sb.Append(" ");
            }
            string sentenceIndent = sb.ToString();
            sb.Length = 0;
            string[] paragraphSplits = resultStr.Split(new string[] { paragraphIndent }, StringSplitOptions.RemoveEmptyEntries);
            
            //Verify we got 5 paragraphs
            Assert.AreEqual(5, paragraphSplits.Length);
            foreach (string paragraph in paragraphSplits)
            {
                string trimmedParagraph = paragraph.TrimEnd() ;
                string[] sentenceSplits = trimmedParagraph.Split(new string[] { sentenceIndent }, StringSplitOptions.RemoveEmptyEntries);
                //Check sentences are of appropriate sizes
                Assert.GreaterOrEqual(sentenceSplits.Length, defaultParams.MinSentences);
                Assert.LessOrEqual(sentenceSplits.Length, defaultParams.MaxSentences); 

                //Check sentences actually end with a punctuation mark
                foreach (string sentence in sentenceSplits)
                {
                    Assert.Contains(sentence[sentence.Length - 1], facade.Generator.SentenceEnd);
                }
            }
        }

        /// <summary>
        /// For GenerateTitle verify that we have the correct number of words and that 
        /// they are all capitalized
        /// </summary>
        [Test]
        public void T_GenerateTitle_Length_And_Capitalization()
        {
            GeneratorFacade facade = InitBaseFacade();
            string resultingTitle = facade.GenerateTitle(9);
            string[] words = resultingTitle.Split(' ');
            Assert.AreEqual(9, words.Length);
            foreach (string word in words)
            {
                Assert.IsTrue(char.IsUpper(word[0]));
            }
        }

        /// <summary>
        /// Test that GenerateParagraphs will throw exceptions for invalid input
        /// </summary>
        [Test]
        public void T_GenerateParagraphs_Validate_Input()
        {
            GeneratorFacade facade = InitBaseFacade();
            Assert.Throws<Exceptions.InvalidArguments>(
                delegate { facade.GenerateParagraphs(0); });
            Assert.Throws<Exceptions.InvalidArguments>(
                delegate { facade.GenerateParagraphs(1, new ParagraphParams { MaxSentences = 1 }); });
            Assert.Throws<Exceptions.InvalidArguments>(
                delegate { facade.GenerateParagraphs(1, new ParagraphParams { MinSentences = -1 }); });
            Assert.Throws<Exceptions.InvalidArguments>(
                delegate { facade.GenerateParagraphs(1, new ParagraphParams { MaxSentences = 4 ,
                    MinSentences = 5 }); });
            Assert.Throws<Exceptions.InvalidArguments>(
                delegate { facade.GenerateParagraphs(1, new ParagraphParams { ParagraphIndent = -1 }); });
            Assert.Throws<Exceptions.InvalidArguments>(
                delegate { facade.GenerateParagraphs(1, new ParagraphParams { SentenceIndent = -1 }); });
        }

        private GeneratorFacade InitBaseFacade()
        {
            Test_MarkovGenerator markovTest = new Test_MarkovGenerator();
            MarkovGenerator gen = markovTest.InitBaseMarkovGenerator();
            return new GeneratorFacade(gen, new Random(3000));
        }
	}
}

