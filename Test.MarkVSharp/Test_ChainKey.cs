/*
 * Created by SharpDevelop.
 * User: Emil
 * Date: 1/15/2011
 * Time: 2:35 AM
 * 
 */
using System;
using NUnit.Framework;

using MarkVSharp;

namespace Test.MarkVSharp
{
    /// <summary>
    /// Test ChainKey class
    /// </summary>
    [TestFixture]
    public class Test_ChainKey
    {
        [Test]
        public void T_ToString()
        {
            ChainKey ck = new ChainKey(new string[]{"word1", "word2", "word3"}) ;
            Assert.AreEqual("word1::word2::word3", ck.ToString()) ;
        }
        
        /// <summary>
        /// Verify that for keys with the same words in different order we get different hash codes
        /// </summary>
        [Test]
        public void T_GetHashCode_OrderMatters()
        {
            ChainKey ck = new ChainKey(new string[]{"word1", "word2"}) ;
            ChainKey ckEqual = new ChainKey(new string[]{"word1", "word2"}) ;
            ChainKey ckReverse = new ChainKey(new string[]{"word2", "word1"}) ;
            Assert.AreEqual(ck.GetHashCode(), ckEqual.GetHashCode()) ;
            Assert.AreNotEqual(ck.GetHashCode(), ckReverse.GetHashCode()) ;
        }
        
        /// <summary>
        /// Verify some simple chain keys generate different hash codes
        /// </summary>
        [Test]
        public void T_GetHashCode_SimpleDifferent()
        {
        	ChainKey ck1 = new ChainKey(new string[]{"", "this"}) ;
        	ChainKey ck2 = new ChainKey(new string[]{"do", "this"}) ;
        	Assert.AreNotEqual(ck1.GetHashCode(), ck2.GetHashCode()) ;
        }
        
        [Test]
        public void T_Equal()
        {
            ChainKey ck = new ChainKey(new string[]{"word1", "word2"}) ;
            ChainKey ckEqual = new ChainKey(new string[]{"word1", "word2"}) ;
            ChainKey ckReverse = new ChainKey(new string[]{"word2", "word1"}) ;
            Assert.AreEqual(ck, ckEqual) ;
            Assert.AreNotEqual(ck, ckReverse) ;
        }
    }
}
