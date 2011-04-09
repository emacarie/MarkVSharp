/*
 * Created by SharpDevelop.
 * User: Emil
 * Date: 1/2/2011
 * Time: 11:42 PM
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
    /// Description of Utils.
    /// </summary>
    public class TestUtils
    {
        public static void CompareChainTables(Dictionary<ChainKey, List<string>> inputChains1,
                                              Dictionary<ChainKey, List<string>> inputChains2)
        {
            CompareChainTables(inputChains1, inputChains2, false) ;
        }

        public static void CompareChainTables(Dictionary<ChainKey, List<string>> inputChains1,
                                              Dictionary<ChainKey, List<string>> inputChains2,
                                             bool ignoreDuplicates)
        {
            CollectionAssert.AreEquivalent(inputChains1.Keys, inputChains2.Keys) ;
            foreach(ChainKey key1 in inputChains1.Keys)
            {
            	if(!ignoreDuplicates)
            	{
                	CollectionAssert.AreEquivalent(inputChains1[key1], inputChains2[key1]) ;
            	}
            	else
            	{
            		CollectionAssert.AreEquivalent(inputChains1[key1].Distinct(), inputChains2[key1].Distinct()) ;
            	}
            }
        }
        
        public static Dictionary<ChainKey, List<string>> GetUnderlyingDictionary(ChainMap inputChainMap)
        {
            Dictionary<ChainKey, List<string>> retValues = new Dictionary<ChainKey, List<string>>() ;
            List<ChainKey> keys = inputChainMap.GetAllKeys() ;
            foreach(ChainKey key in keys)
            {
                retValues.Add(key, inputChainMap.GetValues(key)) ;
            }
            return retValues;
        }
        
        public static void CompareChainTables(ChainMap inputChainMap,
                                              Dictionary<ChainKey, List<string>> inputChains)
        {
            CompareChainTables(GetUnderlyingDictionary(inputChainMap), inputChains) ;
        }
        
        public static void CompareChainTables(ChainMap inputChainMap1,
                                              ChainMap inputChainMap2)
        {
            CompareChainTables(GetUnderlyingDictionary(inputChainMap1), 
                                      GetUnderlyingDictionary(inputChainMap2));
        }
        
        /// <summary>
        /// Compare chain tables ignoring duplicates
        /// </summary>
        public static void CompareChainTablesNoDuplicates(ChainMap inputChainMap1,
                                              ChainMap inputChainMap2)
        {
            CompareChainTables(GetUnderlyingDictionary(inputChainMap1), 
                                      GetUnderlyingDictionary(inputChainMap2), true);
        		
        }
        
        public static ChainGenerator Create2WordGen(string input)
        {
            return new ChainGenerator(input, StandardDelims.ToArray(), 2) ;
        }
        
        public static List<char> StandardDelims = new List<char>{',', '.', '!', '?', ';'};
    }
}
