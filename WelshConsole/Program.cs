using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WelshConsole
{
    class Welsh
    {
        static string inSentence;
        static string[,] testData;
        static int lines;
        const int MAXSIZE = 100;
        static bool fromKeyboard;
        //static WordEntry NONE = new WordEntry("","","");

        class Source
        {
            String[] linesFromFile;

            public Source()
            {
                fromKeyboard = true;
            }
            public Source(string inFile)
            {
                fromKeyboard = false;
                getFromDisk(inFile);
            }
            void getFromDisk(string inFile)
            {
                linesFromFile = System.IO.File.ReadAllLines(inFile);
            }
            public void getData()
            {
                inSentence = Console.ReadLine();
            }
            public void getData(int which)
            {
                inSentence = linesFromFile[which];
            }
            public String tellCorrect(int which)
            {
                return null; //huh?
            }
            public String sendSentence()
            {
                return inSentence;
            }
            public bool isFromKeyboard()
            {
                return fromKeyboard;
            }
            public bool more(int which)
            {
                //similar to next
                return which < linesFromFile.Length;
            }
        }
        class Scanner
        {
            Source source;
            String currentSentence;
            String[] inWords;
            int inWordNumber;
            int currentLocation;
            int sourceLine;

            public Scanner(Source source)
            {
                this.source = source;

            }
            public bool scanSentence()
            {
                //get data from current source
                //use next function to take it apaer?
                //say we are the beginning of the sentence
                if (source.isFromKeyboard())
                {
                    source.getData();
                } else
                {
                    source.getData(sourceLine);
                }
                currentSentence = source.sendSentence();
                toWords();

                return false;
            }
            void toWords()
            {
                inWords = currentSentence.Split(' ');
            }
            public String TellWord()
            {
                return inWords[currentLocation]; //current word
            }
            public int tellCurrentLocation()
            {
                return currentLocation;
            }
            public void increaseCurrentLocation()
            {
                currentLocation++;
            }
            public int howMany()
            {
                //i hate this function definition...
                return inWords.Length;

            }
            public String tellFirst()
            {
                return inWords[0];
            }
            public String tellNext()
            {
                return inWords[currentLocation + 1];
            }
            public String tellLast()
            {
                return inWords[howMany() - 1];
            }
            public void show()
            {
                //print to screen
                Console.Write(currentSentence);
            }
            public void removeEndPunctuation()
            {
                String last = tellLast();
                if(last.EndsWith("?") || last.EndsWith("!") || last.EndsWith("."))
                {
                    last = last.Substring(0, last.Length - 1);
                    inWords[howMany() - 1] = last;
                }
                //maybe add special case for quoted sentences, where . comes before the end quote
            }
        }
        class WordEntry
        {
            //example declaration
            // new WordEntry("blaidd","wolf","NR ");

            String[] data;
            public WordEntry()
            {
                data = new String[]{ "*ERROR*", "*ERROR*", ""};
            }
            public WordEntry(String welsh, String english, String grammar)
            {
                data = new String[]{ welsh, english, grammar};
            }
            public WordEntry(WordEntry other)
            {
                other.data.CopyTo(data, 0);
            }
            public String english()
            {
                return data[1];
            }
            public String welsh()
            {
                return data[0];
            }
            public String grammar()
            {
                return data[2];
            }
            public char POS()
            {
                return grammar().ElementAt(0);
            }
            public char Case() // not applicable in welsh
            {
                return grammar().ElementAt(2);
            }
            public void setCase(char c)
            {
                data[2].Remove(2, 1);
                data[2].Insert(2, c + "");
            }
            public char gender()
            {
                return grammar().ElementAt(3);
            }
            public char person()
            {
                return grammar().ElementAt(3);
            }
            public char number()
            {
                return grammar().ElementAt(4);
            }
            public void setNumber(char c)
            {
                data[2].Remove(4, 1);
                data[2].Insert(4, c + "");
            }
            public char tense()
            {
                return grammar().ElementAt(5);
            }
            public bool isSingular()
            {
                return data[2].ElementAt(6) == 'S';
            }
            public void setSingular()
            {
                data[2].Remove(6, 1);
                data[2].Insert(6, "S");
            }
            public bool isPlural()
            {
                return !isSingular();
            }
            public void setPlural()
            {
                data[2].Remove(6, 1);
                data[2].Insert(6, "P");
            }
            public bool isBad()
            {
                //return malformed data entries
                return false;
            }
        }
        class Program
        {
            static void Main(string[] args)
            {
                Source source = new Source();
                Scanner scanner = new Scanner(source);


                scanner.scanSentence();
                scanner.removeEndPunctuation();
                Console.WriteLine(scanner.howMany());

                Source diskSource = new Source("WelshTest.txt");
                Scanner diskScanner = new Scanner(diskSource);

                diskScanner.scanSentence();
                diskScanner.removeEndPunctuation();
                Console.WriteLine(diskScanner.howMany());
            }
        }
    }
   
}
