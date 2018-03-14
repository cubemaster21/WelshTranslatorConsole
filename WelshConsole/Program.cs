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
        static WordEntry NONE = new WordEntry(" "," ","        ");

        class Source
        {

            public Source()
            {
                fromKeyboard = true;
            }
            public Source(string inFile)
            {
                fromKeyboard = false;
                testData = new String[MAXSIZE, 2];
                getFromDisk(inFile);
            }
            void getFromDisk(string inFile)
            {
                String[] linesFromFile = System.IO.File.ReadAllLines(inFile);
                for(int i = 0;i < linesFromFile.Length; i++)
                {
                    testData[i, i % 2] = linesFromFile[i];
                }
                lines = linesFromFile.Length / 2;
            }
            public void getData()
            {
                Console.WriteLine("Enter the Welsh:");
                inSentence = Console.ReadLine();
            }
            public void getData(int which)
            {
                inSentence = testData[which, 0];
            }
            public String tellCorrect(int which)
            {
                return testData[which, 1];
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
                return which < lines;
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
                sourceLine = 0;
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
                    if (source.more(sourceLine))
                    {
                        source.getData(sourceLine);
                        sourceLine++;
                    } else
                    {
                        return false;
                    }
                }
                currentSentence = source.sendSentence();
                toWords();
                currentLocation = 0;
                return false;
            }
            void toWords()
            {
                inWords = currentSentence.Split(' ');
                inWordNumber = inWords.Length;
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
            public void setFirst(String s)
            {
                inWords[0] = s;
            }
            public String tellLast()
            {
                return inWords[howMany() - 1];
            }
            public void show()
            {
                //print to screen
                Console.WriteLine("\nIndividual Input Words");
                for(int i = 0;i < inWordNumber; i++)
                {
                    Console.WriteLine("{0}. {1}", i + 1, inWords[i]);
                }
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
                //This version is wayyy better than the given function, which goes south when they sentence ends irregularly.
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
                data = new String[] { other.welsh(), other.english(), other.grammar() };
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
                bool test = data[2][0] == 'Z';
                if (test)
                {
                    Console.WriteLine("BAD");
                }
                return test;
                
            }
        }
        class WelshEnglishPiece
        {
            //"to descrive subjects verb parts, prepositional phrases, etc.:
            //multi part words
            const int MAXSIZE = 10;
            int actualLength;
            WordEntry[] theWords;
            int[] dictLocs;
            char[] type;
            bool[] present;
            bool filled;
            bool plural;


            public WelshEnglishPiece(int howMany, char[] theTypes)
            {
                theWords = new WordEntry[howMany]; //HOW MANY WHAT
                type = theTypes;
                actualLength = howMany;
                clean();
                

            }
            public void clean()
            {
                for (int i = 0; i < actualLength; i++)
                    theWords[i] = new WordEntry("_", "_", "        ");

                present = new bool[actualLength]; //keeps track of whether a spot is filled....
                filled = false;
                plural = false;
            }
            public void fill(WordEntry w)
            {
                for(int i = 0;i < actualLength; i++)
                {
                    if(w.POS() == type[i])
                    {
                        theWords[i] = w;
                        present[i] = true;
                        filled = true;
                        i = actualLength;
                    }
                }
            }
            public WordEntry tell(char c)
            {
                for(int i = 0;i < actualLength; i++)
                {
                    if(c == type[i])
                    {
                        return theWords[i];
                    }
                }
                return NONE;
            }
            public String show(int i)
            {
                return theWords[i].welsh();
            }
            public bool empty()
            {
                return !filled; // This function makes absolutely no sense
            }
            public bool isEmpty(char c)
            {
                for (int i = 0; i < actualLength; i++)
                    if (c == type[i])
                        return !present[i];
                return true;
            }
            public bool isFilled(char c)
            {
                for (int i = 0; i < actualLength; i++)
                    if (c == type[i])
                        return present[i];
                return false;
            }
            public bool isPlural()
            {
                return plural;
            }
            public void makePlural()
            {
                plural = true;
            }
        }
        class WelshParser
        {
            // 
            // 
            Scanner sc;
            WelshEnglishIntermediateCode ic;
            WelshEnglishWordTable wt;
            bool Be;
            public WelshParser(Scanner aSc, WelshEnglishIntermediateCode aIc, WelshEnglishWordTable aWt)
            {
                // 
                sc = aSc;
                ic = aIc;
                wt = aWt;

            }
            public void DoParse()
            {
                //
                //
                //
                Be = false;
                ic.Clean();
                AdjustEnds();
                TryExclamation();
                TryVerbPart();// moved this to before the subject
                TrySubject();
                
                if (BeIsh())
                    TryPredicate();
                else
                    TryObject();
            }
            void TryExclamation()
            {
                //
                if (ic.IsExclamation())
                {
                    bool done = false;
                    while(More() && !done)
                    {
                        WordEntry c = wt.tell(wt.find(sc.TellWord()));
                        while(c.isBad() && !done)
                        {
                            if (More())
                            {
                                sc.increaseCurrentLocation();
                                if (More())
                                    c = wt.tell(wt.find(sc.TellWord()));
                                else
                                    done = true;
                            }
                            else
                            {
                                done = true;
                            }
                        }
                        if (Vocish())
                        {
                            ic.Vocative.fill(c);
                            if(wt.tellChange() == 'P')
                            {
                                ic.Vocative.makePlural();
                            }
                            sc.increaseCurrentLocation();
                        }
                        else
                        {
                            done = true;
                        }
                    }
                }
            }
            void TryNounPart(WelshEnglishPiece p)
            {
                //
                //
                //
                bool done = false;
                while(More() && !done)
                {
                    WordEntry c = wt.tell(wt.find(sc.TellWord()));
                    while(c.isBad() && !done)
                    {
                        if (More())
                        {
                            sc.increaseCurrentLocation();
                            if (More())
                                c = wt.tell(wt.find(sc.TellWord()));
                            else
                                done = true;
                        }
                        else
                            done = true;
                    }
                    if (Nounish())
                    {
                        p.fill(c);
                        if (wt.tellChange() == 'P')
                            p.makePlural();
                        sc.increaseCurrentLocation();
                    }
                    else
                        done = true;
                }
            }
            void TrySubject()
            {
                //
                TryNounPart(ic.Subject);
            }
            void TryVerbPart()
            {
                //
                while(More() && Verbish())
                {
                    if (BeIsh())
                        Be = true;
                    if(wt.tell(wt.find(sc.TellWord())).person() == '1')
                    {
                        WordEntry t = new WordEntry(" ", "I", "P     ");
                        ic.Subject.fill(t);
                    }
                    //if first person, fill that word entry into the subject
                    ic.VerbPart.fill(wt.tell(wt.find(sc.TellWord())));
                    sc.increaseCurrentLocation();
                }
            }
            void TryObject()
            {
                //
                TryNounPart(ic.Object);
            }
            void TryPredicate()
            {
                //
                TryNounPart(ic.Predicate);
            }
            bool BeIsh()
            {
                //
                //
                //
                if (More())
                {
                    char c = wt.tell(wt.find(sc.TellWord())).POS();
                    char d = 'N';
                    return (c == 'V' && d == 'N');
                }
                return false;
            }
            bool Verbish()
            {
                //
                return More() && wt.tell(wt.find(sc.TellWord())).POS() == 'V';
            }
            bool Nounish()
            {
                //
                //
                if (More())
                {
                    char c = wt.tell(wt.find(sc.TellWord())).POS();
                    return c == 'N' || c == 'A' || c == 'P' || c == 'D' || c == 'S' || c == 'M' || c == 'J';
                }
                return false;
            }
            bool Vocish()
            {
                //
                //
                if (More())
                {
                    char c = wt.tell(wt.find(sc.TellWord())).POS();
                    return c == 'N' || c == 'A' || c == 'P' || c == 'D' || c == 'S' || c == 'M' || c == 'J' || c == 'E';
                }
                return false;
            }
            bool First()
            {
                //
                //
                return wt.tell(wt.find(sc.TellWord())).person() == '1';
            }
            void AdjustEnds()
            {
                //
                //
                //
                if(char.IsUpper(sc.tellFirst()[0]))
                {
                    ic.SetUpperCase(true);
                    StringBuilder temp = new StringBuilder(sc.tellFirst());
                    temp[0] = char.ToLower(temp[0]);
                    sc.setFirst(temp.ToString());
                }
                if (sc.tellLast().EndsWith("."))
                {
                    sc.removeEndPunctuation();
                    ic.SetEndPunctuation('.');
                }
                else if (sc.tellLast().EndsWith("!"))
                {
                    sc.removeEndPunctuation();
                    ic.SetEndPunctuation('!');
                    ic.SetExclamation(true);
                }
            }
            bool More()
            {
                //
                return sc.tellCurrentLocation() < sc.howMany();
            }
        }//
        //
        class WelshEnglishIntermediateCode
        {
            //
            //

            public WelshEnglishPiece Vocative, Subject, VerbPart, Predicate, Object;
            bool InitialUppercase, Exclamation;
            char EndPunctuation; //
            public WelshEnglishIntermediateCode(int HowMany)
            {
                //What is this parameter used for?
                //
                InitialUppercase = false;
                Exclamation = false;
                EndPunctuation = ' ';
                char[] nounish = { 'A', 'P', 'D', 'S', 'M', 'J', 'N' };
                char[] vocish = { 'E', 'A', 'P', 'D', 'S', 'M', 'J', 'N' };
                Vocative = new WelshEnglishPiece(8, vocish);
                Subject = new WelshEnglishPiece(7, nounish);
                char[] verbish = { 'V', 'B'};
                VerbPart = new WelshEnglishPiece(2, verbish);
                Predicate = new WelshEnglishPiece(7, nounish);
                Object = new WelshEnglishPiece(7, nounish);
            }
            public void Clean()  //
            {
                //
                //
                //
                InitialUppercase = false;
                Exclamation = false;
                EndPunctuation = ' ';
                Vocative.clean();
                Subject.clean();
                VerbPart.clean();
                Predicate.clean();
                Object.clean();
            }

            public void SetUpperCase(bool b)
            {
                InitialUppercase = true;
            }
            public bool IsUpperCase()
            {
                return InitialUppercase;
            }
            public void SetExclamation(bool b)
            {
                Exclamation = b;
            }
            public bool IsExclamation()
            {
                return Exclamation;
            }

            public void SetEndPunctuation(char c)
            {
                //
                EndPunctuation = c;
            }

            public char TellEndPunctuation()
            {
                //
                return EndPunctuation;
            }

            public void Show()   //
            {
                //
                //
                //TODO
            }
        }//
        class WelshEnglishWordTable
        {
            int currentSize;
            char change;
            const int MAXSIZE = 100;
            WordEntry[] lex;

            int lexBuildingIndex = 0;

            public WelshEnglishWordTable()
            {
                //read from disk, or keep in code
                currentSize = 0;
                change = ' ';
                lex = new WordEntry[MAXSIZE];
                //polish example
                //pushLex("dziecko", "child", "NIXNS");
                pushLex("mund", "goes", "VIX3S");
                pushLex("mund", "go", "VIX1P");
                
                pushLex("dysgu", "learn", "NIXNS"); //also means teach...
                pushLex("gorwedd", "lie", "NIXNS");
                pushLex("hoffi", "like", "NIXNS");
                pushLex("chwarae", "play", "NIXNS");
                pushLex("darllen", "read", "NIXNS");
                pushLex("rhedeg", "run", "NIXNS");
                pushLex("canu", "sing", "NIXNS"); //or to play an instrument
                pushLex("eistedd", "sit", "NIXNS");
                pushLex("sefull", "stand", "NIXNS");
                pushLex("cerdedd", "walk", "NIXNS");
                pushLex("gwisgo", "wear", "NIXNS");
                pushLex("gweithio", "work", "NIXNS");
                pushLex("ysgrifennu", "write", "NIXNS");
                pushLex("a", "and", "NIXNS"); //ac before a vowel
                pushLex("a'r", "and the", "NIXNS");
                pushLex("ar", "on", "NIXNS");
                pushLex("dan", "under", "NIXNS");
                pushLex("drwɥ", "through", "NIXNS");
                pushLex("trwɥ", "through", "NIXNS");
                pushLex("drwɥ'r", "through the", "NIXNS");
                pushLex("trwɥ'r", "through the", "NIXNS");
                pushLex("i", "to", "NIXNS"); //or into
                pushLex("i'r", "to the", "NIXNS");
                pushLex("wrth", "near", "NIXNS"); //or by
                pushLex("yn", "in", "NIXNS");
                pushLex("Saesneg", "English", "NIXNS");
                pushLex("Cymraeg", "Welsh", "NIXNS");
                pushLex("awyrn", "aeroplane", "NIXNS");
                pushLex("eroplên", "aeroplane", "NIXNS");
                pushLex("anthem", "anthem", "NIXNS");
                pushLex("llyfr", "book", "NIXNS");
                pushLex("bachgen", "boy", "NIXNS");
                pushLex("bws", "bus", "NIXNS");
                pushLex("eglwɥs", "church", "NIXNS");
                pushLex("cloc", "clock", "NIXNS");
                pushLex("ci", "dog", "NIXNS");
                pushLex("drws", "door", "NIXNS");
                pushLex("fferm", "farm", "NIXNS");
                pushLex("cae", "field", "NIXNS");
                pushLex("tân", "fire", "NIXNS");
                pushLex("llawr", "floor", "NIXNS");
                pushLex("het", "hat", "NIXNS");
                pushLex("tɥ", "house", "NIXNS"); //There's supposed to be a circumflex on that upsidedown h, but I couldn't figure out how
                pushLex("map", "map", "NIXNS");
                pushLex("enw", "name", "NIXNS");
                pushLex("papur", "paper", "NIXNS");
                pushLex("piano", "piano", "NIXNS");
                pushLex("afon", "river", "NIXNS");
                pushLex("ffordd", "road", "NIXNS");
                pushLex("ystafell", "room", "NIXNS");
                pushLex("eira", "snow", "NIXNS");
                pushLex("haf", "summer", "NIXNS");
                pushLex("haul", "sun", "NIXNS");
                pushLex("bwrdd", "table", "NIXNS");
                pushLex("mur", "wall", "NIXNS");
                pushLex("ffenestr", "window", "NIXNS");
                pushLex("heno", "tonight", "NIXNS");
                pushLex("heddiw", "today", "NIXNS");
                pushLex("fi", "I", "PIXNS");
                pushLex("ti", "thou", "PIXNS");
                pushLex("ef", "he", "PIXMS");
                pushLex("hi", "she", "PIXFS");
                pushLex("ni", "we", "PIXMP");
                pushLex("chwi", "you", "PIXNS");
                pushLex("hwɥ", "they", "PIXMP");
                //set current size after loading all the words
                currentSize = lexBuildingIndex;
            }
            /*
             * This function exists solely to help me keep this dictionary clean when I try to add new things
             * */
            private void pushLex(String welsh, String English, String info)
            {
                lex[lexBuildingIndex++] = new WordEntry(welsh, English, info);
            }
            public int find(String word)
            {
                //search function. Is the word provided english or welsh?
                for(int i = 0;i < currentSize; i++)
                {
                    if(lex[i].welsh() == word)
                    {
                        return i;
                    }
                }
                String cutDefinite = word.Substring(0, word.Length - (word.EndsWith("'r") ? 2 : 0)); //removes 'r that may be added from definite article
                for (int i = 0; i < currentSize; i++)
                {
                    if (lex[i].welsh() == cutDefinite)
                    {
                        return i;
                    }
                }


                return -1;
            }
            public char tellChange()
            {
                //I'm not sure why I'm supposed to change the change var back to  ' ' after this, but whatever
                char temp = change;
                change = ' ';
                return temp;
            }
            public WordEntry tell(int i)
            {
                if(i < 0 || i >= currentSize)
                {
                    return new WordEntry(" ", "UNKNOWN", "Z     ");
                }
                return lex[i];
            }
        }
        class EnglishGenerator
        {
            const int MAXWORDS = 20;
            WelshEnglishIntermediateCode ic;
            WelshEnglishWordTable wt;
            String[] words;
            int outLocation;
            int actualLength;

            public EnglishGenerator(WelshEnglishIntermediateCode aic, WelshEnglishWordTable awt)
            {
                this.ic = aic;
                this.wt = awt;
                words = new String[MAXWORDS];
                actualLength = 0;
            }
            public void generate()
            {
                actualLength = 0;
                getExclamation();
                getSubject();
                getVerbPart();
                getPredicate();
                getObject();
                adjustEnds();
            }
            void getExclamation()
            {
                if (!ic.Vocative.empty())
                {
                    foreach (char c in new char[]{ 'E','P', 'D', 'S', 'M', 'J' })
                    {
                        words[actualLength] = ic.Vocative.tell(c).english();
                        actualLength++;
                    }
                    if (!ic.Vocative.isEmpty('N'))
                    {
                        
                        words[actualLength] = ic.Vocative.tell('N').english() + (ic.Vocative.isPlural() ? "s" : "");
                        actualLength++;
                    }
                }
            }
            void getSubject()
            {
                getNounPart(ic.Subject);
            }
            void getNounPart(WelshEnglishPiece p)
            {
                if (!p.empty())
                {
                    if(p.isEmpty('S') && p.isEmpty('D') && p.isEmpty('M') && p.isEmpty('P') && !p.isEmpty('N'))
                    {
                        //no possessive, demonstrative, numeral, pronoun, but noun is present
                        words[actualLength] = "the";
                        actualLength++;
                    }
                    if (!p.isEmpty('P'))
                    {
                        words[actualLength] = p.tell('P').english();
                        actualLength++;
                    }
                    if (!p.isEmpty('D'))
                    {
                        words[actualLength] = p.tell('P').english();
                        actualLength++;
                    }
                    if (!p.isEmpty('S'))
                    {
                        if(p.isEmpty('M') && p.isEmpty('J') && p.isEmpty('N') && p.isEmpty('D'))
                        {
                            words[actualLength] = "mine";
                        }
                        else
                        {
                            words[actualLength] = p.tell('S').english();
                        }
                        actualLength++;
                    }
                    if (!p.isEmpty('M'))
                    {
                        words[actualLength] = p.tell('M').english();
                        actualLength++;
                    }
                    if (!p.isEmpty('J'))
                    {
                        words[actualLength] = p.tell('J').english();
                        actualLength++;
                    }
                    if (!p.isEmpty('N'))
                    {
                        words[actualLength] = p.tell('N').english() + (p.isPlural() ? "s" : "");
                        actualLength++;
                    }
                }
            }
            void getVerbPart()
            {
                if (!ic.VerbPart.empty())
                {
                    words[actualLength] = ic.VerbPart.tell('V').english();
                    actualLength++;
                }
            }
            void getPredicate()
            {
                getNounPart(ic.Predicate);
            }
            void getObject()
            {
                getNounPart(ic.Predicate);
            }
            void adjustEnds()
            {
                if (ic.IsUpperCase())
                {
                    StringBuilder temp = new StringBuilder(words[0]);
                    temp[0] = char.ToUpper(temp[0]);
                    words[0] = temp.ToString();
                }
                if(ic.TellEndPunctuation() != ' ')
                {
                    words[actualLength - 1] = words[actualLength - 1] + ic.TellEndPunctuation();
                }
            }
            public String tell(int i)
            {
                return words[i];
            }
            public int tellSize()
            {
                return actualLength;
            }
            public String tellFirst()
            {
                outLocation = 1;
                return words[0];
            }
            public String tellNext()
            {
                if(outLocation < actualLength)
                    return words[outLocation++];
                return "";
            }
            public void show()
            {
                //pretty print
            }
        }
        class EnglishOutput
        {
            EnglishGenerator generator;
            String outSentence;
            String nextWord;

            public EnglishOutput(EnglishGenerator gen)
            {
                generator = gen;
                outSentence = "";
            }
            void toSentence()
            {
                outSentence = generator.tell(0);
                for(int i = 1;i < generator.tellSize(); i++)
                {
                    outSentence += " " + generator.tell(i);
                }
            }
            public void tellSentence()
            {
                toSentence();
                Console.WriteLine("\nTranslation: ");
                Console.WriteLine(outSentence + "\n");

            }
        }
        class WelshToEnglish
        {
            Source source;
            Scanner scanner;
            WelshEnglishIntermediateCode code;
            WelshEnglishWordTable table;
            WelshParser parser;
            EnglishGenerator generator;
            EnglishOutput output;
            const int PIECENUMBER = 5;

            public WelshToEnglish()
            {
                //create new pieces
                source = new Source();
                scanner = new Scanner(source);
                code = new WelshEnglishIntermediateCode(PIECENUMBER);
                table = new WelshEnglishWordTable();
                parser = new WelshParser(scanner, code, table);
                generator = new EnglishGenerator(code, table);
                output = new EnglishOutput(generator);
            }
            public void doASentence()
            {
                //use pieces in correct order
                scanner.scanSentence();
                scanner.show();
                parser.DoParse();
                code.Show();
                generator.generate();
                generator.show();
                output.tellSentence();
            }
        }
        class Program
        {
            static void Main(string[] args)
            {
                /*Source source = new Source();
                Scanner scanner = new Scanner(source);


                scanner.scanSentence();
                scanner.removeEndPunctuation();
                Console.WriteLine(scanner.howMany());

                Source diskSource = new Source("WelshTest.txt");
                Scanner diskScanner = new Scanner(diskSource);

                diskScanner.scanSentence();
                diskScanner.removeEndPunctuation();
                Console.WriteLine(diskScanner.howMany());*/

                //Actual code will loop to test 25 sentences.
                WelshToEnglish we = new WelshToEnglish();

                for(int i = 0;i < 25; i++)
                {
                    we.doASentence();
                }
            }
        }
    }
   
}
