/*
 * Created by SharpDevelop.
 * User: Emil
 * Date: 12/28/2010
 * Time: 6:56 PM
 * 
 */
using System;
using System.Text;
using System.Collections.Generic;

namespace MarkVSharp
{
	/// <summary>
	/// Miscelanous utilities relating to string processing
	/// </summary>
	public static class StringUtils
	{
	    /// <summary>
	    /// Split a string according to the delimiters passed in.
	    /// The delimiters are part of the result as single character strings
	    /// </summary>
	    /// <param name="inputString"></param>
	    /// <param name="delimChars"></param>
	    /// <returns></returns>
		public static List<string> SplitAndKeep(string inputString, List<char> delimChars)
		{
			if(inputString == null)
			{
				return null;
			}
			
			List<string> retValues = new List<string>() ;
			StringBuilder sb = new StringBuilder() ;
			bool charsFound = false ;
			foreach(char currChar in inputString)
			{
				if(delimChars.Contains(currChar))
				{
					if(charsFound)
					{
						retValues.Add(sb.ToString()) ;
						sb.Length = 0 ;
						charsFound = false ;
					}
					retValues.Add(currChar.ToString()) ;
				}
				else
				{
					sb.Append(currChar) ;
					charsFound = true ;
				}
			}
			if(charsFound)
			{
			    retValues.Add(sb.ToString());
			}
			return retValues;
		}
		
		/// <summary>
		/// Capitalize first letter of string
		/// </summary>
		/// <param name="inputStr"></param>
		/// <returns></returns>
		internal static string Capitalize(this string inputStr)
		{
			if(string.IsNullOrEmpty(inputStr))
			{
				return inputStr ;
			}
			if(inputStr.Length == 1)
			{
				return inputStr.ToUpperInvariant() ;
			}
			return char.ToUpperInvariant(inputStr[0]) + inputStr.Substring(1) ;
		}
	}
}
