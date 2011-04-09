/*
 * Created by SharpDevelop.
 * User: Emil
 * Date: 12/30/2010
 * Time: 2:24 PM
 * 
 */
using System;
using System.Collections.Generic;

namespace MarkVSharp
{
    /// <summary>
    /// This class generates random consecutive words based on the passed in chains
    /// </summary>
    public class WordGenerator
    {
        Random _rand ;
        private ChainMap _chains ;
        private ChainMap _onEndSubChains ;
        private ChainMap _initialConditionSubChains ;
        private ChainKey[] _initialConditionSubKeys ;
        private ChainKey[] _keys ;
        private ChainKey _currKey;
        private ChainCondition _alwaysTrueCondition;
        private ChainCondition _currentOnEndCondition;
        
        /// <summary>
        /// Current word.  Before calling Reset this is null
        /// </summary>
        public string CurrentWord{private set; get;}
        
        /// <summary>
        /// If subchains have been initialized by a successful reset
        /// </summary>
        public bool SubchainsInitialized{private set; get;}
        
        internal ChainKey CurrKey{get { return _currKey;}} 
        
        internal ChainMap Chains 
        {
        	get { return _chains;}
        }
        
        /// <summary>
        /// Initializes word generator with default random seed
        /// </summary>
        /// <param name="chains"></param>
        public WordGenerator(ChainMap chains):this(chains, null){}
        
        /// <summary>
        /// Initializes word generator with random seed as parameter
        /// </summary>
        /// <param name="chains"></param>
        /// <param name="rand"> Rng used if better randomness is desired.  The default .NET Rng is 
        /// not completely random </param>
        public WordGenerator(ChainMap chains, Random rand)
        {
            _chains = chains ;
            if(rand != null)
            {
                _rand = rand ;
            }
            else
            {
                _rand = new Random() ;
            }
            Initialize() ;
        }
        
        /// <summary>
        /// Initialize pool of random keys
        /// The probability of picking a key is proportional to the number of values in the ChainMap
        /// </summary>
        public void Initialize()
        {
        	_alwaysTrueCondition = new ChainCondition(p => true, p => true);
        	_keys = CreateKeysArray(_chains, _alwaysTrueCondition.KeyCondition) ;
        	_currentOnEndCondition = _alwaysTrueCondition ;
        }
        
        /// <summary>
        /// Reset using currently existing initialCondition and onEndCondition
        /// This should be a low cpu usage operation
        /// </summary>
        public void ResetReadOnly()
        {
            //verify if reset has never been called
            if(!SubchainsInitialized)
            {
                throw new Exceptions.InitializationRequired("ResetReadOnly should be called after a regular reset") ;
            }
            
            //All chains and words should match condition so we can just get random
            ChainKey currKey = GetRandomKey(_initialConditionSubKeys) ;
            string currWord ;
            GetCandidate(_initialConditionSubChains.GetValues(currKey), null, out currWord) ;
            CurrentWord = currWord;
            _currKey = currKey;
        }
        
        /// <summary>
        /// Reset using new initial and onEndCondition.  This requires quite a bit of CPU
        /// as all the chains are processed to get a valid subset for the initial and on end
        /// conditions
        /// </summary>
        public void ResetSubchains(ChainCondition initialCondition, ChainCondition onEndCondition)
        {
            Reset(initialCondition, onEndCondition) ;
        }
        
        /// <summary>
        /// Reset word generator with no condition
        /// </summary>
        public void ResetSubchains()
        {
        	Reset(null, null) ;
        }
        
        /// <summary>
        /// Reset word generator with same condition for initial chain as final chain
        /// </summary>
        /// <param name="initialCondition"></param>
        public void ResetSubchains(ChainCondition initialCondition)
        {
        	Reset(initialCondition, null) ;
        }
        
        /// <summary>
        /// Reset CurrentWord based on conditions
        /// </summary>
        /// <param name="keyCondition"> Condition for key of first word </param>
        /// <param name="onEndCondition"> Condition for new word after there are no more chains
        /// By default this is the same as @keyCondition   </param>
        private void Reset(ChainCondition initialCondition, ChainCondition onEndCondition)
        {
            ChainCondition realInitialConditions = initialCondition;
            //Set null conditions to always true conditions to simplify our code
            if(realInitialConditions == null)
            {
                realInitialConditions = _alwaysTrueCondition;
            }
            else
            {
                if (realInitialConditions.KeyCondition == null)
                {
                    realInitialConditions.KeyCondition = _alwaysTrueCondition.KeyCondition;
                }
                if (realInitialConditions.WordCondition == null)
                {
                    realInitialConditions.WordCondition = _alwaysTrueCondition.WordCondition;
                }
            }
            
            ChainCondition realOnEndCondition = onEndCondition ;
            //Set on end condition to start condition if it is null
            if(realOnEndCondition == null)
            {
                realOnEndCondition = realInitialConditions ;
            }
            else
            {
                if (realOnEndCondition.KeyCondition == null)
                {
                    realOnEndCondition.KeyCondition = _alwaysTrueCondition.KeyCondition;
                }
                if (realOnEndCondition.WordCondition == null)
                {
                    realOnEndCondition.WordCondition = _alwaysTrueCondition.WordCondition;
                }
            }
            
            ChainMap initialConditionMap ;
            //Only generate subchains if we need to
            if(realInitialConditions != _alwaysTrueCondition)
          	{
            	initialConditionMap = GetSubChain(realInitialConditions) ;	
            }
            else
            {
            	initialConditionMap = Chains;
            }
            
            ChainKey[] initialConditionSubKeys = CreateKeysArray(initialConditionMap, 
                                                                 _alwaysTrueCondition.KeyCondition) ;
            
            //All chains and words should match condition so we can just get random
            ChainKey currKey = GetRandomKey(initialConditionSubKeys) ;
            string currWord ;
            GetCandidate(initialConditionMap.GetValues(currKey), null, out currWord) ;
            
            if(realOnEndCondition != _alwaysTrueCondition)
            {
            	//Verify on end map has valid elements.  If no exception is thrown we are good to go
            	_onEndSubChains = GetSubChain(realOnEndCondition) ;
            }
            else
            {
                _onEndSubChains = initialConditionMap;
            }
            
            CurrentWord = currWord;
            _currKey = currKey;
            _initialConditionSubChains = initialConditionMap ;
            _initialConditionSubKeys = initialConditionSubKeys;
            _currentOnEndCondition = realOnEndCondition;
            SubchainsInitialized = true ;
        }
        
        /// <summary>
        /// Get next word with no conditions
        /// </summary>
        /// <returns></returns>
        public string GetNextWord()
        {
        	return GetNextWord(null) ;
        }
        
        /// <summary>
        /// Get next word.  If the next natural word does not obey given predicate, throw exception
        /// If we reach the end of the list get random key matching end condition passed in Reset
        /// </summary>
        /// <param name="condition"> Predicate next word must obey </param>
        /// <returns></returns>
        public string GetNextWord(Predicate<string> condition)
        {
            //If word generator has never been reset, reset with default values
            if(!SubchainsInitialized)
            {
                ResetSubchains() ;
            }
            
            Predicate<string> realCondition = condition ;
            if(realCondition == null)
            {
            	realCondition = _alwaysTrueCondition.WordCondition;
            }
            
            ChainKey currKey = _currKey;
            string currWord = CurrentWord;
            string[] newChainKeyVals = new string[currKey.Words.Length] ;
            for(int i = 0 ; i < currKey.Words.Length - 1 ; i++)
            {
            	newChainKeyVals[i] = currKey.Words[i+1] ;
            }
            newChainKeyVals[currKey.Words.Length - 1] = currWord ;
            currKey = new ChainKey(newChainKeyVals) ;
            
            List<string> potentialWords = null ;
            try
            {
            	potentialWords = _chains.GetValues(currKey) ;
            }
            catch(Exceptions.InvalidKey)
            {
            	//if we get here that means we hit the end of the list
            	newChainKeyVals = GetRandomKey(_onEndSubChains, ck => !ck.Equals(currKey)).Words;
            	currKey = new ChainKey(newChainKeyVals) ;
            	potentialWords = _chains.GetValues(currKey) ;
            }
            
            bool candidateFound = GetCandidate(potentialWords, condition, out currWord) ;
            if(!candidateFound)
            {
            	throw new Exceptions.NoPossibleElements("Unable to find a state matching given conditions") ;
            }
            
            _currKey = currKey;
            CurrentWord = currWord;
            return currWord;
        }
		
		
        /// <summary>
        /// Get next word.  If the next natural word does not obey given predicate, skip and continue
        /// If we reach the end of the list get random key matching end condition passed in Reset
        /// If for a given word we skip more words than maxIterations, assume condition is null
        /// and return next random word
        /// </summary>
        /// <param name="condition"> Predicate next word must obey </param>
        /// <param name="maxIterations"> Max iterations to try to obey given condition before
        ///  giving up </param>
        /// <returns></returns>
		public string GetNextWord(Predicate<string> condition, int maxIterations)
		{
			if(maxIterations < 1)
			{
				throw new Exceptions.InvalidArguments("Max iterations should be at least 1") ;
			}
			string lastWord = string.Empty;
			for(int i = 0 ; i < maxIterations ; i++)
			{
                try
				{
					return GetNextWord(condition) ;
				}
				catch(Exceptions.NoPossibleElements)
				{
					//junk the current word
					lastWord = GetNextWord() ;
				}
			}
			
			//If we got here we did not find a word matching given condition
			return lastWord ;
		}
        
        /// <summary>
        /// Get subchain of chainkeys and words that satisfies the passed in condition
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        internal ChainMap GetSubChain(ChainCondition condition)
        {
        	//If conditions won't be applied return original chains
        	if(condition == null || condition == _alwaysTrueCondition || 
        	   (condition.WordCondition == null && condition.KeyCondition == null))
        	{
        		return _chains ;
        	}
        	
        	ChainMap newMap = new ChainMap() ;
        	bool valuesFound = false ;
        	foreach(ChainKey key in _chains.GetAllKeys())
        	{
        		if(condition.KeyCondition != null)
        		{
        			if(condition.KeyCondition.Invoke(key))
        			{
        				foreach(string word in _chains.GetValues(key))
        				{
        					if(condition.WordCondition != null)
        					{
        						if(condition.WordCondition.Invoke(word))
        						{
        							newMap.AddToChain(key, word) ;
        							valuesFound = true ;
        						}
        					}
        				}
        			}
        		}
        	}
        	
        	if(!valuesFound)
        	{
        		throw new Exceptions.NoPossibleElements("Chain map filter filtered everything out") ;
        	}
        	
        	return newMap ;
        }
        
        /// <summary>
        /// Get random word from passed in list that matches given predicate
        /// If no such word is found return false
        /// </summary>
        /// <param name="potentialWords"></param>
        /// <param name="condition"></param>
        /// <param name="result"> random word from list or null if nothing is found </param>
        /// <returns>true if a candidate is found, false otherwise</returns>
        internal bool GetCandidate(List<string> potentialWords, Predicate<string> condition, out string result)
        {
        	List<string> currWords = new List<string>(potentialWords) ;
        	while(currWords.Count > 0)
        	{
        		int currIndex = _rand.Next(currWords.Count);
        		if(condition != null)
        		{
        			if (condition.Invoke(currWords[currIndex]))
        			{
        				result = currWords[currIndex] ;
        				return true ;
        			}
        		}
        		else
        		{
        			result = currWords[currIndex] ;
        			return true ;
        		}
        		
        		currWords.RemoveAt(currIndex) ;
        	}
        	result = null ;
        	return false ;
        }
        
        private ChainKey[] CreateKeysArray(ChainMap map, Predicate<ChainKey> keyCondition)
        {
        	List<ChainKey> newKeys = new List<ChainKey>() ;
        	List<ChainKey> origKeys = map.GetAllKeys() ;
        	foreach(ChainKey origKey in origKeys)
        	{
        	    if(keyCondition.Invoke(origKey))
        	    {
        	        //Add key multiple times to ensure probability of picking it is proportional to the
        	        //number of values
        	        foreach(string val in map.GetValues(origKey))
        	        {
        	            newKeys.Add(origKey) ;
        	        }
        	    }
        	}
        	return newKeys.ToArray() ;
        }
        
        
        /// <summary>
        /// Get a random key from class chain map
        /// The probability of getting a key should be proportional to the number of values
        /// </summary>
        /// <returns></returns>
        private ChainKey GetRandomKey()
        {
            return _keys[_rand.Next(_keys.Length)];
        }
        
        /// <summary>
        /// Get a random key from passed in chain map
        /// The probability of getting a key should be proportional to the number of values
        /// </summary>
        /// <returns></returns>
        internal ChainKey GetRandomKey(ChainMap map)
        {
        	ChainKey[] keys = CreateKeysArray(map, _alwaysTrueCondition.KeyCondition) ;
            return keys[_rand.Next(keys.Length)];
        }
        
        /// <summary>
        /// Get a random key from passed in chain array
        /// </summary>
        /// <returns></returns>
        internal ChainKey GetRandomKey(ChainKey[] ckList)
        {
            return ckList[_rand.Next(ckList.Length)];
        }
        
        /// <summary>
        /// Get random key from passed in chain map matching passed in condition
        /// </summary>
        /// <param name="map"></param>
        /// <param name="keyCondition"></param>
        /// <returns></returns>
        internal ChainKey GetRandomKey(ChainMap map, Predicate<ChainKey> keyCondition)
        {
        	ChainKey[] keys = CreateKeysArray(map, keyCondition) ;
            return keys[_rand.Next(keys.Length)];
        }
    }
}
