using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using MarkVSharp;

namespace Performance.MarkVSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Test starting");
            GeneratorFacade gen = new GeneratorFacade(TimeMarkovConstructor()) ;
            TimeGenerateWords(gen, 10, 10000) ;
            TimeGenerateWords(gen, 10000, 10) ;
            TimeGenerateSentences(gen, 10000) ;
            TimeGenerateParagraphs(gen, 500, 10) ;
            TimeGenerateParagraphs(gen, 10, 500) ;
            Console.WriteLine("Test complete");
            Console.ReadLine();
        }
        
        static MarkovGenerator TimeMarkovConstructor()
        {
            Stopwatch timer = new Stopwatch() ;
            timer.Start() ;
            MarkovGenerator gen = new MarkovGenerator(System.IO.File.ReadAllText("PrideAndPrejudice.txt"));
            timer.Stop() ;
            double baseGenTime = timer.ElapsedMilliseconds/1000.0 ;

            Console.WriteLine(string.Format("Time to generate a new Markov Gen: {0:0.00} seconds", baseGenTime));
            return gen ;
        }
        
        static void TimeGenerateWords(GeneratorFacade gen, int numIterations, int numWords)
        {
            Stopwatch timer = new Stopwatch() ;
            timer.Start() ;
            for(int i = 0 ; i < numIterations; i++)
            {
                gen.GenerateWords(numWords) ;
            }
            timer.Stop() ;
            double baseGenTime = timer.ElapsedMilliseconds/1000.0 ;
            
            Console.WriteLine(string.Format("Time to generate {0}x{1} words: {2:0.00} seconds",
                                            numIterations, numWords,  baseGenTime));
        }
        
        static void TimeGenerateSentences(GeneratorFacade gen, int numSentences)
        {
            Stopwatch timer = new Stopwatch() ;
            timer.Start() ;
            for(int i = 0 ; i < numSentences ; i++)
            {
                gen.GenerateSentence(3) ;
            }
            timer.Stop() ;
            double baseGenTime = timer.ElapsedMilliseconds/1000.0 ;
            
            Console.WriteLine(string.Format("Time to generate {0} sentences: {1:0.00} seconds",
                                            numSentences, baseGenTime));
        }
        
        static void TimeGenerateParagraphs(GeneratorFacade gen, int numIterations, 
                                          int numParagraphs)
        {
            Stopwatch timer = new Stopwatch() ;
            timer.Start() ;
            for(int i = 0 ; i < numIterations ; i++)
            {
                gen.GenerateParagraphs(numParagraphs);
            }
            timer.Stop() ;
            double baseGenTime = timer.ElapsedMilliseconds/1000.0 ;
            
            Console.WriteLine(string.Format("Time to generate {0}x{1} paragraphs: {2:0.00} seconds",
                                            numIterations, numParagraphs, baseGenTime));
        }
    }
}
