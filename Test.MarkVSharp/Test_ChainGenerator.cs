/*
 * Created by SharpDevelop.
 * User: Emil
 * Date: 1/15/2011
 * Time: 7:35 PM
 * 
 */
using System;
using NUnit.Framework;
using System.Collections.Generic;

using MarkVSharp;

namespace Test.MarkVSharp
{
    /// <summary>
    /// Test ChainGenerator
    /// </summary>
    [TestFixture]
    public class Test_ChainGenerator
    {
        readonly string _advancedChainString = "Words from.sentence,will never end ? because words from sentence is good." ;
		readonly char[] _delims = TestUtils.StandardDelims.ToArray() ;
		
		/// <summary>
		/// Test various exceptions and parameter error handling for constructor
		/// </summary>
		[Test]
		public void T_Constructor_Exceptions()
		{
			string origText = "Hello world string.";
			ChainGenerator chainGenerator = new ChainGenerator(origText, _delims, 2) ;
			Assert.Throws(typeof(Exceptions.InvalidArguments), 
                          delegate { chainGenerator =  new ChainGenerator(origText, _delims, 0);});
			Assert.Throws(typeof(Exceptions.InvalidArguments), 
                          delegate { chainGenerator =  new ChainGenerator(origText, _delims, 20);});
			Assert.Throws(typeof(Exceptions.InvalidArguments), 
                          delegate { chainGenerator =  new ChainGenerator(null, _delims, 2);});
			
		}
		
		/// <summary>
		/// Test complex 2 element chain generation
		/// </summary>
		[Test]
		public void T_Chains_2()
		{
			ChainGenerator chainGenerator = new ChainGenerator(_advancedChainString, _delims, 2) ;
			chainGenerator.GenerateChains() ;
			
			Dictionary<ChainKey, List<string>> correctChains = 
			    new Dictionary<ChainKey, List<string>>{
			    {new ChainKey(new string[]{null, null}), new List<string>{"words"}},
			    {new ChainKey(new string[]{null, "words"}), new List<string>{"from"}},
			    {new ChainKey(new string[]{"words", "from"}), new List<string>{".", "sentence"}},
				{new ChainKey(new string[]{"from", "."}), new List<string>{"sentence"}},
				{new ChainKey(new string[]{".", "sentence"}), new List<string>{","}},
				{new ChainKey(new string[]{"sentence", ","}), new List<string>{"will"}},
				{new ChainKey(new string[]{",", "will"}), new List<string>{"never"}},
				{new ChainKey(new string[]{"will", "never"}), new List<string>{"end"}},
				{new ChainKey(new string[]{"never", "end"}), new List<string>{"?"}},
				{new ChainKey(new string[]{"end", "?"}), new List<string>{"because"}},
				{new ChainKey(new string[]{"?", "because"}), new List<string>{"words"}},
				{new ChainKey(new string[]{"because", "words"}), new List<string>{"from"}},
				{new ChainKey(new string[]{"from", "sentence"}), new List<string>{"is"}},
				{new ChainKey(new string[]{"sentence", "is"}), new List<string>{"good"}},
				{new ChainKey(new string[]{"is", "good"}), new List<string>{"."}}
			};
			
			TestUtils.CompareChainTables(chainGenerator.Chains, correctChains) ;
			
		}
		
		/// <summary>
		/// Test 1 element chain generation
		/// </summary>
		[Test]
		public void T_Chains_1()
		{
			ChainGenerator chainGenerator = new ChainGenerator(_advancedChainString, _delims, 1) ;
			chainGenerator.GenerateChains() ;
			
			Dictionary<ChainKey, List<string>> correctChains = 
			    new Dictionary<ChainKey, List<string>>{
			    {new ChainKey(new string[]{null}), new List<string>{"words"}},
			    {new ChainKey(new string[]{"words"}), new List<string>{"from", "from"}},
				{new ChainKey(new string[]{"from"}), new List<string>{".", "sentence"}},
				{new ChainKey(new string[]{"."}), new List<string>{"sentence"}},
				{new ChainKey(new string[]{"sentence"}), new List<string>{",", "is"}},
				{new ChainKey(new string[]{","}), new List<string>{"will"}},
				{new ChainKey(new string[]{"will"}), new List<string>{"never"}},
				{new ChainKey(new string[]{"never"}), new List<string>{"end"}},
				{new ChainKey(new string[]{"end"}), new List<string>{"?"}},
				{new ChainKey(new string[]{"?"}), new List<string>{"because"}},
				{new ChainKey(new string[]{"because"}), new List<string>{"words"}},
				{new ChainKey(new string[]{"is"}), new List<string>{"good"}},
				{new ChainKey(new string[]{"good"}), new List<string>{"."}}
			};
			
			TestUtils.CompareChainTables(chainGenerator.Chains, correctChains) ;
		}
	       
    }
}
