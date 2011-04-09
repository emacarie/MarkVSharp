/*
 * Created by SharpDevelop.
 * User: Emil
 * Date: 1/1/2011
 * Time: 11:31 PM
 * 
 */
using System;
using System.Text;
using System.Collections.Generic;
using NUnit.Framework;

using MarkVSharp;

namespace Test.MarkVSharp
{
    /// <summary>
    /// Description of Test_WordGenerator.
    /// </summary>
    [TestFixture]
    public class Test_WordGenerator
    {
        private string _simpleSentence = "Hello world one. This is hello world two ?" ;
        
        /// <summary>
        /// Ensure that the GetNextWord method returns correct values for two
        /// small chains
        /// </summary>
        [Test]
        public void T_GetNext_SimpleChain()
        {         
            WordGenerator wordGenerator = InitSimpleWordGen();
            wordGenerator.ResetSubchains(new ChainCondition(ck => ck.Words[0] == "."));
            Assert.AreEqual("is", wordGenerator.CurrentWord) ;
            
            Assert.AreEqual("hello", wordGenerator.GetNextWord()) ;
            Assert.AreEqual("world", wordGenerator.GetNextWord()) ;
            
            bool oneFound = false ;
            bool twoFound = false ;
            
            int iter = 0 ;
            
            //Ensure that after hello word either and only "one" or "two" can follow
            while(!(oneFound && twoFound))
            {
                wordGenerator.ResetSubchains(new ChainCondition(ck => ck.Words[0] == "."));
                //get hello
                wordGenerator.GetNextWord() ;
                //get world
                wordGenerator.GetNextWord() ;
                
                //should be one or two
                string currWord = wordGenerator.GetNextWord() ;
                if(currWord == "one")
                {
                    oneFound = true ;
                }
                else if (currWord == "two")
                {
                    twoFound = true ;       
                }
                else 
                {
                    Assert.Fail("Only possible words should be one or two") ;
                }
                
                if(oneFound && twoFound)
                {
                    break ;
                }
                
                //10 tries should be enough to get both possibilities
                iter++;
                if(iter > 10)
                {
                    break ;
                }
            }
            
            Assert.IsTrue(oneFound) ;
            Assert.IsTrue(twoFound) ;
        }
        
        /// <summary>
        /// Verify that with a readonly reset, existing conditions will remain.
        /// Essentially asset ReadOnly reset is equivalent to a regular reset
        /// </summary>
        [Test]
        public void T_ResetReadOnly_ExistingConditionsRemain()
        {
            WordGenerator wordGenerator = InitSimpleWordGen();
            
            //The only valid option for the current word is hello
            wordGenerator.ResetSubchains(new ChainCondition(
                ck => ck.Words[0] == "this" && ck.Words[1] == "is")) ;
            Assert.AreEqual("hello", wordGenerator.CurrentWord) ;
            Assert.AreNotEqual("hello", wordGenerator.GetNextWord()) ;
            wordGenerator.ResetReadOnly() ;
            Assert.AreEqual("hello", wordGenerator.CurrentWord) ;
        }
        
        /// <summary>
        /// Ensure that if Reset is called with an impossible predicate, an exception is thrown
        /// </summary>
        [Test]
        public void T_ResetSubchains_NoOptions()
        {           
            WordGenerator wordGenerator = InitSimpleWordGen();
            Assert.Throws(typeof(Exceptions.NoPossibleElements), 
                          delegate { wordGenerator.ResetSubchains(new ChainCondition(ck => ck.Words[0] == "DoestNotExist")); });
        }
        
        /// <summary>
        /// Ensure that if GetNextWord is called with an impossible predicate, an exception is thrown
        /// </summary>
        [Test]
        public void T_GetNext_NoOptions()
        {
            WordGenerator wordGenerator = InitSimpleWordGen();
            Assert.Throws(typeof(Exceptions.NoPossibleElements), 
                          delegate { wordGenerator.GetNextWord(w => w == "DoestNotExist"); });
        }
        
        /// <summary>
        /// Ensure that if GetNextWord is called it will return words from original string
        /// When sentence end is reached, sentence will continue
        /// </summary>
        [Test] 
        public void T_GetNext_SimpleLongChain()
        {
            ChainGenerator chainGenerator = TestUtils.Create2WordGen(_simpleSentence) ;
            WordGenerator wordGenerator = InitSimpleWordGen();
            wordGenerator.ResetSubchains(new ChainCondition(ck => ck.Words[0] == null && ck.Words[1] == null), null) ;
            
            chainGenerator.GenerateChains() ;
            ChainMap origChains = chainGenerator.Chains;
            
            StringBuilder sb = new StringBuilder() ;
            
            sb.Append(wordGenerator.CurrentWord) ;
            for(int i = 0 ; i < 200 ; i++)
            {
                sb.Append(" ") ;
                sb.Append(wordGenerator.GetNextWord()) ;
            }
            
            chainGenerator = TestUtils.Create2WordGen(sb.ToString()) ;
            chainGenerator.GenerateChains() ;
            ChainMap resultChains = chainGenerator.Chains ;
            
            //When we get to the end we should start from the beginning
            origChains.AddToChain(new ChainKey(new string[]{"two", "?"}), "hello") ;
            origChains.AddToChain(new ChainKey(new string[]{"?", "hello"}), "world") ;
            
            TestUtils.CompareChainTablesNoDuplicates(origChains, resultChains) ;
        }
		
		/// <summary>
		///Ensure that if GenNextWord is called with a maximum number of iterations, we keep getting
		//words even if they do not match the given condition
		/// </summary>
		[Test]
		public void T_GetNext_WithLimit()
		{
            WordGenerator wordGenerator = InitSimpleWordGen();
			//Ensure we are still getting words
			List<char> delims = new List<char>(TestUtils.StandardDelims) ;
			delims.Add(' ') ;
			List<string> possibleWords =  StringUtils.SplitAndKeep(_simpleSentence, delims) ;
			List<string> possibleWordsLower = new List<string>() ;
			possibleWords.ForEach(w => possibleWordsLower.Add(w.ToLowerInvariant())) ;
			for(int i = 0 ; i < 10 ; i++)
			{
				CollectionAssert.Contains(possibleWordsLower, wordGenerator.GetNextWord(w => w == "DoestNotExist", 10)); 
			}
		}
        
        /// <summary>
        /// Ensure that CurrentWord always matches GetNextWord value
        /// </summary>
        [Test]
        public void T_CurrentWord()
        {          
            WordGenerator wordGenerator = InitSimpleWordGen() ;
            wordGenerator.ResetSubchains() ;
            Assert.IsNotNullOrEmpty(wordGenerator.CurrentWord) ;
            for(int i = 0 ; i < 20 ; i ++)
            {
                string currWord = wordGenerator.GetNextWord() ;
                Assert.AreEqual(currWord, wordGenerator.CurrentWord) ;
            }
        }
        
        /// <summary>
        /// Test GetCandidate with various inputs
        /// </summary>
        [Test]
        public void T_GetCandidate()
        {
        	string result ;
  
        	List<string> currWords = new List<string>{ "word1", "word2", "word3" } ;
        	WordGenerator gen = InitSimpleWordGen() ;
        	
        	//None of the words match
        	Assert.IsFalse(gen.GetCandidate(currWords, p=> p.Contains("does not exist"),  out result)) ;
        	
        	//Check each word in list is found
        	foreach(string currWord in currWords)
        	{
        		Assert.IsTrue(gen.GetCandidate(currWords, p=> p == currWord, out result)) ;
        		Assert.AreEqual(result, currWord) ;
        	}
        }
        
        /// <summary>
        /// Test GetSubChain with simple wordgen
        /// </summary>
        [Test]
        public void T_GetSubChain()
        {
        	WordGenerator gen = InitSimpleWordGen() ;
        	
        	//Should give me both "hello world one" and "hello world two"
        	ChainMap tempMap = gen.GetSubChain(new ChainCondition(cw => cw.Words[0] == "hello", w => w.Length == 3)) ;
        	ChainMap correctMap = new ChainMap() ;
        	correctMap.AddToChain(new ChainKey(new string[] {"hello", "world"}), "one") ;
        	correctMap.AddToChain(new ChainKey(new string[] {"hello", "world"}), "two") ;
        	TestUtils.CompareChainTables(correctMap, tempMap) ;
        	
        	//Should give me just "hello world one"
        	tempMap = gen.GetSubChain(new ChainCondition(cw => cw.Words[0] == "hello", w => w.StartsWith("o"))) ;
        	correctMap = new ChainMap() ;
        	correctMap.AddToChain(new ChainKey(new string[] {"hello", "world"}), "one") ;
        	TestUtils.CompareChainTables(correctMap, tempMap) ;
        	
        	//If passed in condition is null nothing should change
        	tempMap = gen.GetSubChain(new ChainCondition(null, null)) ;
        	TestUtils.CompareChainTables(gen.Chains, tempMap) ;
        	
        	//If impossible condition is passed in exception should be thrown
        	Assert.Throws(typeof(Exceptions.NoPossibleElements), delegate 
        	              { gen.GetSubChain(new ChainCondition(null, w => w == "Does not exist")) ;}) ;
        }
        
        /// <summary>
        /// Verify that for a simple test calling getrandomkey will work when a chain map is passed in
        /// </summary>
        [Test]
        public void T_GetRandomKey_WithParams()
        {
        	WordGenerator gen = InitSimpleWordGen() ;
        	ChainMap subMap = new ChainMap() ;
        	ChainKey key1 = new ChainKey(new string[]{"key1", "key2", "key3"}) ;
        	ChainKey key2 = new ChainKey(new string[]{"key1", "key2", "key4"}) ;
        	subMap.AddToChain(key1, "val1") ;
        	subMap.AddToChain(key2, "val2") ;
        	bool key1Found, key2Found ;
        	key1Found = key2Found = false ;
        	
        	for(int i = 0 ; i < 10 ; i++)
        	{
        		ChainKey currKey = gen.GetRandomKey(subMap);
        		if(key1.Equals(currKey))
        		{
        			key1Found = true ;
        		}
        		else if(key2.Equals(currKey))
        		{
        			key2Found = true ;
        		}
        		else
        		{
        			Assert.Fail("Invalid key returned") ;
        		}
        	}
        	if(!(key1Found && key2Found))
        	{
        		Assert.Fail("GetRandomKey is not random") ;
        	}
        }
        
        /// <summary>
        /// Create a word generator with constant Random seed
        /// </summary>
        /// <returns></returns>
        private WordGenerator InitSimpleWordGen()
        {
            ChainGenerator chainGen = 
                new ChainGenerator(_simpleSentence, TestUtils.StandardDelims.ToArray(), 2) ;
            chainGen.GenerateChains() ;            
            return new WordGenerator(chainGen.Chains, new Random(1000)) ;
        }
    }
}
