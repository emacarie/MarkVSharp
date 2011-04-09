/*
 * Created by SharpDevelop.
 * User: Emil
 * Date: 1/15/2011
 * Time: 4:32 PM
 * 
 */
using System;
using System.Collections.Generic;

namespace MarkVSharp
{
    /// <summary>
    /// Generate a Dictionary of chain string pairs that will be passed into word generators
    /// The first ChainSize elements will have nulls in their chains
    /// </summary>
    public class ChainGenerator
    {
        private List<char> _delims ;
        private int _chainSize ;
        private string _text;
        
        public ChainMap Chains {get; private set;}
        
        
        public ChainGenerator(string text, char[] delims, int chainSize)
        {
            if(text == null || text.Length < 10)
            {
                throw new Exceptions.InvalidArguments("Invalid input text") ;
            }
            
            if (chainSize < 1 || chainSize > 10)
            {
                throw new Exceptions.InvalidArguments("Invalid chain size") ;
            }
            
            _text = text ;
            _delims = new List<char>(delims) ;
            _chainSize = chainSize;
        }
        
        /// <summary>
        /// Generate markov chains using input text
        /// </summary>
        public void GenerateChains()
        {
            //Split text
            string[] splits = _text.Split() ;
            List<string> advancedSplits = new List<string>() ;
            foreach(string baseSplit in splits)
            {
                advancedSplits.AddRange(StringUtils.SplitAndKeep(baseSplit.ToLower(), _delims)) ;
            }
            
            
            Chains = new ChainMap() ;
            if(advancedSplits.Count < _chainSize * 2)
            {
                throw new Exceptions.InvalidArguments(string.Format("Chain size: {0} to small relative to text split {1}",
                                                         _chainSize, advancedSplits.Count)) ;
            }
            
            //Insert null elements in the beginning so chains are created for the first _chainSize elements
            for(int i = 0 ; i < _chainSize ; i++)
            {
                advancedSplits.Insert(0, null) ;
            }
            
            List<string>.Enumerator listEnum = advancedSplits.GetEnumerator() ;
            
            string[] chainWords = new string[_chainSize] ;
            
            for(int i = 0 ; i < _chainSize ; i++)
            {
                listEnum.MoveNext();
                chainWords[i] = listEnum.Current ;
            }
            
            while(listEnum.MoveNext())
            {
                string current = listEnum.Current;
                ChainKey currKey = new ChainKey(chainWords) ;
                Chains.AddToChain(currKey, current) ;
                 
                for(int i = 0 ; i < _chainSize - 1 ; i++)
                {
                    chainWords[i] = chainWords[i+1] ;
                }
                chainWords[_chainSize - 1] = current ;
            }
        }
    }
}
