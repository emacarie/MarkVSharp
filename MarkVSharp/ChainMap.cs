/*
 * Created by SharpDevelop.
 * User: Emil
 * Date: 1/15/2011
 * Time: 5:09 PM
 * 
 */
using System;
using System.Collections;
using System.Collections.Generic;

namespace MarkVSharp
{
    /// <summary>
    /// Class which acts as a dictionary for all the chains and the strings they point to
    /// This is different from a normal dictionary in the sense that each ChainKey points to a 
    /// List of strings
    /// </summary>
    public class ChainMap
    {
        private Dictionary<ChainKey, List<string>> _chains ;
        public ChainMap()
        {
            _chains = new Dictionary<ChainKey, List<string>>() ;
        }
        
        /// <summary>
        /// Add to list of values corresponding to chain key
        /// If list does not exist create it
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddToChain(ChainKey key, string value)
        {
            if(!_chains.ContainsKey(key))
            {
                _chains.Add(key, new List<string>()) ;
            }
            _chains[key].Add(value) ;
        }
        
        /// <summary>
        /// Get a copy of all chainkeys
        /// </summary>
        /// <returns>copy of all keys</returns>
        public List<ChainKey> GetAllKeys()
        {
            List<ChainKey> newKeys = new List<ChainKey>() ;
            foreach(ChainKey key in _chains.Keys)
            {
                newKeys.Add(new ChainKey(key.Words)) ;
            }
            return newKeys;
        }
        
        /// <summary>
        /// Get copy of List of values corresponding to passed in key
        /// </summary>
        /// <param name="key"></param>
        /// <returns>copy of values corresponding to passed in key</returns>
        public List<string> GetValues(ChainKey key)
        {
            List<string> retVals ;
            if(!_chains.TryGetValue(key, out retVals))
            {
                throw new Exceptions.InvalidKey("Invalid key") ;
            }
            return retVals;
        }
    }
}
