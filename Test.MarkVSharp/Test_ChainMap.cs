/*
 * Created by SharpDevelop.
 * User: Emil
 * Date: 1/15/2011
 * Time: 7:20 PM
 * 
 */
using System;
using System.Collections.Generic;
using NUnit.Framework;

using MarkVSharp;

namespace Test.MarkVSharp
{
    /// <summary>
    /// Test ChainMap class.
    /// </summary>
    [TestFixture]
    public class Test_ChainMap
    {
        [Test]
        public void T_AddToChain()
        {
            ChainMap cm = InitSimpleChainMapp() ;
            Dictionary<ChainKey, List<string>> correctMap = new Dictionary<ChainKey, List<string>>() ;
            correctMap.Add(new ChainKey(new string[]{null, "word1"}), new List<string>{"valnull"}) ;
            correctMap.Add(new ChainKey(new string[]{"word1", "word2"}), new List<string>{"val1", "val3"}) ;
            correctMap.Add(new ChainKey(new string[]{"word2", "word3"}), new List<string>{"val2"}) ;
            TestUtils.CompareChainTables(cm, correctMap) ;
        }
        
        [Test]
        public void T_GetAllKeys()
        {
            ChainMap cm = InitSimpleChainMapp() ;
            List<ChainKey> allKeys = cm.GetAllKeys() ;
            List<ChainKey> correctKeys = new List<ChainKey>() ;
            correctKeys.Add(new ChainKey(new string[]{null, "word1"})) ;
            correctKeys.Add(new ChainKey(new string[]{"word1", "word2"})) ;
            correctKeys.Add(new ChainKey(new string[]{"word2", "word3"})) ;
            CollectionAssert.AreEquivalent(allKeys, correctKeys) ;
        }
        
        [Test]
        public void T_GetValues()
        {
            ChainMap cm = InitSimpleChainMapp() ;
            ChainKey keyNull = new ChainKey(new string[]{null, "word1"}) ;
            ChainKey key1 = new ChainKey(new string[]{"word1", "word2"}) ;
            ChainKey key2 = new ChainKey(new string[]{"word2", "word3"}) ;
            ChainKey keyIncorrect = new ChainKey(new string[]{"word2", "word1"}) ;
            CollectionAssert.AreEquivalent(cm.GetValues(keyNull), new List<string>{"valnull"}) ;
            CollectionAssert.AreEquivalent(cm.GetValues(key1), new List<string>{"val1", "val3"}) ;
            CollectionAssert.AreEquivalent(cm.GetValues(key2), new List<string>{"val2"}) ;
			Assert.Throws(typeof(Exceptions.InvalidKey), 
                          delegate { cm.GetValues(keyIncorrect);});
        }
        
        private ChainMap InitSimpleChainMapp()
        {
            ChainMap cm = new ChainMap() ;
            cm.AddToChain(new ChainKey(new string[]{null, "word1"}), "valnull") ;
            cm.AddToChain(new ChainKey(new string[]{"word1", "word2"}), "val1") ;
            cm.AddToChain(new ChainKey(new string[]{"word2", "word3"}), "val2") ;
            cm.AddToChain(new ChainKey(new string[]{"word1", "word2"}), "val3") ;
            return cm ;
        }
    }
}
