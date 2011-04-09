/*
 * Created by SharpDevelop.
 * User: Emil
 * Date: 1/15/2011
 * Time: 5:15 PM
 * 
 */
using System;

/// <summary>
/// Contains list of exception that can be thrown
/// This is not really according to .Net standards, but these exceptions do not
/// deserve their own file
/// </summary>
namespace MarkVSharp
{
    public class Exceptions
    {
        public class NoPossibleElements : Exception
        {
            public NoPossibleElements():base(){}
            public NoPossibleElements(string message):base(message){}
        }
        
        public class InvalidArguments : Exception
        {
            public InvalidArguments():base(){}
            public InvalidArguments(string message):base(message){}
        }
        
        public class InvalidKey : Exception
        {
            public InvalidKey():base(){}
            public InvalidKey(string message):base(message){}
        }
        
        public class InitializationRequired : Exception
        {
            public InitializationRequired():base(){}
            public InitializationRequired(string message):base(message){}
        }
    }
}
