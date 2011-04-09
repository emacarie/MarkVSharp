/*
 * Created by SharpDevelop.
 * User: Emil
 * Date: 1/15/2011
 * Time: 1:50 AM
 * 
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MarkVSharp
{
    /// <summary>
    /// Combination of words that are used to generate the next word
    /// </summary>
    public class ChainKey
    {
        public string[] Words {get; private set;}
        public ChainKey(string[] words)
        {
            //Ensure that changing original parameter will not change current object
            Words = new string[words.Length] ;
            words.CopyTo(Words, 0) ;
        }
        
        
        public override bool Equals(object obj)
        {
            ChainKey other = obj as ChainKey;
            if (other == null)
            {
                return false;
            }
            if(other.Words.Length != Words.Length)
            {
                return false;
            }
            
            //Ensure all words are equal
            for(int i = 0 ; i < Words.Length ; i++)
            {
                if(Words[i] != other.Words[i])
                {
                    return false ;
                }
            }
            return true ;
        }
        
        public override int GetHashCode()
        {
            int hashCode = 0;
            if (Words != null)
            {
                unchecked {
                    for(int i = 0 ; i < Words.Length ; i++)
                    {
                        if(Words[i] != null)
                        {
                            //Same words in different order will give different hashes
                            hashCode += Words[i].GetHashCode() * (i + 1);
                        }
                    }
                }
            }
            return hashCode;
        }

        /// <summary>
        /// Format is: word1::word2::word3 and so forth
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder() ;
            for(int i = 0 ; i < Words.Length ; i++)
            {
                if(i != 0)
                {
                    sb.Append("::") ;
                }
                sb.Append(Words[i]) ;
            }
            return sb.ToString();
        }
    }
}
