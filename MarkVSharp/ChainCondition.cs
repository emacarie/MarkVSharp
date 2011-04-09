/*
 * Created by SharpDevelop.
 * User: Emil
 * Date: 1/18/2011
 * Time: 10:16 PM
 * 
 */
using System;

namespace MarkVSharp
{
    /// <summary>
    /// Condition used to find a word
    /// </summary>
    public class ChainCondition
    {
        public Predicate<ChainKey> KeyCondition ;
        public Predicate<string> WordCondition ;
        
        public ChainCondition():this(null, null){}
        
        public ChainCondition(Predicate<ChainKey> keyCondition):this(keyCondition, null){}
        
        public ChainCondition(Predicate<ChainKey> keyCondition, Predicate<string> wordCondition)
        {
            KeyCondition = keyCondition;
            WordCondition = wordCondition ;
        }
    }
}
