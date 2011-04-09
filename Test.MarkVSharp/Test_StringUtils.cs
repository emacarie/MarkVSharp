/*
 * Created by SharpDevelop.
 * User: Emil
 * Date: 12/28/2010
 * Time: 6:58 PM
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

using MarkVSharp;

namespace Test.MarkVSharp
{
    /// <summary>
    /// Test StringUtils methods
    /// </summary>
    [TestFixture]
    public class Test_StringUtils
    {
        readonly string _advancedDelimString = ".hello?world!from;my.string!with;manythings..inbetween?";
        readonly string _noChangeString = "Hello world of peace";
        readonly List<char> _delims = TestUtils.StandardDelims ;
        
        /// <summary>
        /// Verify that splitting a complex string and keeping the delimiters works correctly
        /// </summary>
        [Test]
        public void T_SplitAndKeep_Complex()
        {
            List<string> result = StringUtils.SplitAndKeep(_advancedDelimString, _delims) ;
            List<string> correctResult = new List<string>{".", "hello", "?", "world", "!", "from",
                ";", "my", ".", "string", "!", "with", ";", "manythings", ".", ".",
                "inbetween", "?"};
            CollectionAssert.AreEqual(result, correctResult) ;
        }
        
        /// <summary>
        /// Verify that splitting a string that contains no delimiters will return the
        /// original string
        /// </summary>                
        [Test]
        public void T_SplitAndKeep_NoChange()
        {
            List<string> result = StringUtils.SplitAndKeep(_noChangeString, _delims) ;
            Assert.True(result.Count == 1) ;
            Assert.AreEqual(result[0], _noChangeString) ;
        }
    }
}
