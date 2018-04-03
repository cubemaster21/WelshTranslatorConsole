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
                    testData[i/ 2, i % 2] = linesFromFile[i];
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
                }
                else
                {
                    if (source.more(sourceLine))
                    {
                        source.getData(sourceLine);
                        sourceLine++;
                    }
                    else
                    {
                        return false;
                    }
                }
                currentSentence = source.sendSentence();
                toWords();
                currentLocation = 0;
                return true;
            }
            void toWords()
            {
                inWords = currentSentence.Split(' ');
                inWordNumber = inWords.Length;
            }
            public String TellWord()
            {
                try
                {
                    return inWords[currentLocation]; //current word}

                }
                catch (Exception e)
                {
                    
                    throw new Exception("Failed To Fetch Word at index: " + currentLocation);
                }
                return null;
            }
            public int tellCurrentLocation()
            {
                return currentLocation;
            }
            public void increaseCurrentLocation()
            {
                currentLocation++;
            }
            public bool hasNext()
            {
                return currentLocation < howMany();
            }
            public void setCurrentLocation(int location) // this is used for backtracking
            {
                currentLocation = location;
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
            public String tellPrevious()
            {
                if (currentLocation == 0) return "";
                return inWords[currentLocation - 1];
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

            //translation tags
            public bool noEnglishIndefinite = false;

            public String irregularPresentParticiple = null;


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
            public override String ToString()
            {
                return data[0] + " | " + data[1] + " | " + data[2];
            }
            public WordEntry addIRPP(String english)//add irregular present participle
            {
                irregularPresentParticiple = english;
                return this;

            }
            public bool hasIrregularPresentParticiple()
            {
                return irregularPresentParticiple != null;
            }
            public WordEntry setNoEI(bool val)
            {
                noEnglishIndefinite = val;
                return this;

            }
            public String english()
            {
                return data[1];
            }
            
            public bool isRegular()
            {
                return data[2][1] == 'R';
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
                return grammar()[0];
            }
            public char Case() // not applicable in welsh
            {
                return grammar()[2];
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
                return data[2].ElementAt(4) == 'S';
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

            bool definite = false;
            bool presentParticiple = false;
            String typesString = "";

            List<WelshEnglishPiece> extraPrepositionals = new List<WelshEnglishPiece>();

            public WelshEnglishPiece(char[] theTypes) // removed the howmany variable
            {
                theWords = new WordEntry[theTypes.Length]; //HOW MANY WHAT
                type = theTypes;
                actualLength = theTypes.Length;
                clean();
                for(int i = 0; i < type.Length; i++)
                {
                    typesString += type[i];
                }

            }
            public WelshEnglishPiece(String theTypes) // removed the howmany variable
            {
                theWords = new WordEntry[theTypes.Length]; //HOW MANY WHAT
                type = theTypes.ToArray();
                actualLength = theTypes.Length;
                clean();
                for (int i = 0; i < type.Length; i++)
                {
                    typesString += type[i];
                }

            }
            public void clean()
            {
                for (int i = 0; i < actualLength; i++)
                    theWords[i] = new WordEntry("_", "_", "        ");

                present = new bool[actualLength]; //keeps track of whether a spot is filled....
                filled = false;
                plural = false;
                definite = false;
                presentParticiple = false;
                extraPrepositionals.Clear();
            }
            public WelshEnglishPiece fill(WordEntry w)
            {
                if ((isEmpty(w.POS()) && isEmpty('N')) || ("D".Contains(w.POS()) && !isEmpty('N') && isEmpty('D'))){ 
                    for (int i = 0; i < actualLength; i++)
                    {
                        if (w.POS() == type[i])
                        {
                            theWords[i] = w;
                            present[i] = true;
                            filled = true;
                            return this;
                        }
                    }
                }  else 
                {
                    bool filled = false;
                    while (!filled) { 

                        for(int i = 0;i < getExtraPrepositionalsCount(); i++)
                        {
                            if (!extraPrepositionals[i].isEmpty(w.POS())) continue;
                            return extraPrepositionals[i].fill(w);
                            
                        }
                        extraPrepositionals.Add(new WelshEnglishPiece("RNAVD"));
                    }
                }
                return null;
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
            public void show()
            {
                for(int i = 0;i < actualLength; i++)
                {
                    Console.WriteLine(type[i] + ": "  + show(i));
                }
                for(int i = 0;i < extraPrepositionals.Count; i++)
                {
                    Console.WriteLine("\tSub" + i + ":");
                    extraPrepositionals[i].show();
                }
            }
            public bool empty()
            {
                return !filled; // This function makes absolutely no sense
            }
            public bool hasPiece(char c)
            {
                for (int i = 0; i < actualLength; i++)
                    if (c == type[i])
                        return true;
                return false;
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
            public void setDefinite(bool def)
            {
                this.definite = def;
            }
            public bool isDefinite()
            {
                //loop to see if a word ends with 'r?
                return definite;
            }
            public void setPresentParticiple(bool p)
            {
                presentParticiple = p;
            }
            public bool isPresentParticiple()
            {
                return presentParticiple;
            }
            public String getTypesString()
            {
                return typesString;
            }

            public bool hasExtraPrepositionals()
            {
                return extraPrepositionals.Count != 0;
            }
            public int getExtraPrepositionalsCount()
            {
                return extraPrepositionals.Count;
            }
            public WelshEnglishPiece getExtraPrepositional(int index)
            {
                return extraPrepositionals[index];
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

                //try the article from the front?
                bool articleStart = false;
                String word = sc.TellWord();
                if(word == "y" || word == "yr")
                {
                    sc.increaseCurrentLocation();
                    articleStart = true;
                    //keep in mind that this isn't always meaningless
                }
                

                TryExclamation();
                TryVerbPart();// moved this to before the subject
                for (int i = 1; i < 4 && ic.Subject.empty(); i++)
                {
                    TrySubject(i);
                }
                
                TryPresentParticiple();

                if (Be || BeIsh()) // replaced beish
                {
                    TryPredicate();
                }
                else
                    TryObject();

                if(articleStart && ic.VerbPart.empty())
                {
                    ic.Subject.setDefinite(true);
                }
            }
            
            void TryPresentParticiple()
            {
                int backupPosition = sc.tellCurrentLocation();

                bool done = false;
                while (More() && !done)
                {
                    WordEntry c = wt.find(sc.TellWord());
                    while (c.isBad() && !done)
                    {
                        if (More())
                        {
                            sc.increaseCurrentLocation();
                            if (More())
                                c = wt.find(sc.TellWord());
                            else
                                done = true;
                        }
                        else
                            done = true;
                    }
                    char pos = wt.find(sc.TellWord()).POS();
                    if ("RV".Contains(pos)  && ic.PresParticiple.isEmpty(pos)) // if its a preposition or berb
                    {
                        ic.PresParticiple.fill(c);
                        sc.increaseCurrentLocation();
                    }
                    else
                        done = true;
                }
                //if there isn't a verb in the present participle, we need to undo what we've done.
                if (ic.PresParticiple.isEmpty('V'))
                {
                    ic.PresParticiple.clean();
                    sc.setCurrentLocation(backupPosition);
                }
            }
            
            void TryExclamation()
            {
                //
                if (ic.IsExclamation())
                {
                    bool done = false;
                    while(More() && !done)
                    {
                        WordEntry c = wt.find(sc.TellWord());
                        while(c.isBad() && !done)
                        {
                            if (More())
                            {
                                sc.increaseCurrentLocation();
                                if (More())
                                    c = wt.find(sc.TellWord());
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
                bool done = false;
                while (More() && !done)
                {
                    WordEntry c = wt.find(sc.TellWord());
                    while (c.isBad() && !done)
                    {
                        if (More())
                        {
                            sc.increaseCurrentLocation();
                            if (More())
                                c = wt.find(sc.TellWord());
                            else
                                done = true;
                        }
                        else
                            done = true;
                    }
                    if (Nounish(c))
                    {
                        p.fill(c);
                        if (wt.tellChange() == 'P')
                            p.makePlural();
                        //if the subject is plural, the verb needs to change form

                        sc.increaseCurrentLocation();
                    }
                    else
                        done = true;
                }
            }
            void TrySubject(int attemptNumber)
            {

                bool done = false;
                while (More() && !done)
                {
                    WordEntry c = wt.find(sc.TellWord(), attemptNumber);
                    while (c.isBad() && !done)
                    {
                        if (More())
                        {
                            sc.increaseCurrentLocation();
                            if (More())
                                c = wt.find(sc.TellWord(), attemptNumber);
                            else
                                done = true;
                        }
                        else
                            done = true;
                    }
                    
                    if (Nounish(c))
                    {
                        ic.Subject.fill(c);
                        if (wt.tellChange() == 'P')
                            ic.Subject.makePlural();
                        //if the subject is plural, the verb needs to change form
                        if (c.isPlural())
                        {
                            ic.VerbPart.makePlural();
                        }
                        if(sc.tellPrevious().EndsWith("'r"))
                        {
                            ic.Subject.setDefinite(true);
                        }
                        sc.increaseCurrentLocation();
                    }
                    else
                        done = true;
                }
            }
            void TryVerbPart()
            {
                //
                while(More() && Verbish())
                {
                    
                    if (BeIsh())
                       Be = true;
                    //if (wt.find(sc.TellWord()).person() == '1')
                    //{
                    //   WordEntry t = new WordEntry(" ", "I", "P     ");
                    //  ic.Subject.fill(t);
                    //}
                    //if first person, fill that word entry into the subject
                    ic.VerbPart.fill(wt.find(sc.TellWord()));
                    sc.increaseCurrentLocation();
                }
                
            }
            void TryObject()
            {
                WelshEnglishPiece p = ic.Object;
                bool done = false;
                while (More() && !done)
                {
                    WordEntry c = wt.find(sc.TellWord());
                    while (c.isBad() && !done)
                    {
                        if (More())
                        {
                            sc.increaseCurrentLocation();
                            if (More())
                                c = wt.find(sc.TellWord());
                            else
                                done = true;
                        }
                        else
                            done = true;
                    }
                    if ("NAPDSMJFV".Contains(c.POS()))
                    {
                        p.fill(c);
                        if (wt.tellChange() == 'P')
                            p.makePlural();
                        //if the subject is plural, the verb needs to change form

                        sc.increaseCurrentLocation();
                    }
                    else
                        done = true;
                }
            }
            void TryPredicate()
            {
                //
                WelshEnglishPiece p = ic.Predicate;
                bool done = false;
                while (More() && !done)
                {
                    WordEntry c = wt.find(sc.TellWord());
                    while (c.isBad() && !done)
                    {
                        if (More())
                        {
                            sc.increaseCurrentLocation();
                            if (More())
                                c = wt.find(sc.TellWord());
                            else
                                done = true;
                        }
                        else
                            done = true;
                    }
                    
                    if (ic.Predicate.getTypesString().Contains(c.POS()))
                    {
                        WelshEnglishPiece placedIn = p.fill(c);
                        if (sc.TellWord().EndsWith("'r"))
                            placedIn.setDefinite(true);
                        
                        if (wt.tellChange() == 'P')
                            p.makePlural();
                        //if the subject is plural, the verb needs to change form

                        sc.increaseCurrentLocation();
                    }
                    else
                        done = true;
                }
            }
            bool BeIsh()
            {
                //
                //
                //
                if (More())
                {
                    char c = wt.find(sc.TellWord()).POS();
                    char d = 'N';
                    return (c == 'V' && d == 'N');
                }
                return false;
            }
            bool Verbish()
            {
                //
                return More() && wt.find(sc.TellWord()).POS() == 'V';
            }
            bool Nounish(WordEntry w)
            {
                //
                //
                if (More())
                {
                    char c = w.POS();
                    return c == 'N' || c == 'A' || c == 'P' || c == 'D' || c == 'S' || c == 'M' || c == 'J' || c == 'F';
                }
                return false;
            }
            bool Predicatish()
            {
                //
                //
                if (More())
                {
                    char c = wt.find(sc.TellWord()).POS();
                    return c == 'N' || c == 'A' || c == 'P' || c == 'D' || c == 'S' || c == 'M' || c == 'J' || c == 'F' || c == 'R' || c == 'C'; // added preposition
                }
                return false;
            }
            bool Vocish()
            {
                //
                //
                if (More())
                {
                    char c = wt.find(sc.TellWord()).POS();
                    return c == 'N' || c == 'A' || c == 'P' || c == 'D' || c == 'S' || c == 'M' || c == 'J' || c == 'E';
                }
                return false;
            }
            bool First()
            {
                //
                //
                return wt.find(sc.TellWord()).person() == '1';
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

            public WelshEnglishPiece Vocative, Subject, VerbPart, Predicate, Object, PresParticiple, Prepositional;
            bool InitialUppercase, Exclamation;
            char EndPunctuation; //
            public WelshEnglishIntermediateCode(int HowMany)
            {
                //What is this parameter used for?
                //
                InitialUppercase = false;
                Exclamation = false;
                EndPunctuation = ' ';
                char[] nounish = { 'A', 'P', 'D', 'S', 'M', 'J', 'N', 'R'};
                char[] predicatish = { 'A', 'P', 'D', 'S', 'M', 'J', 'N' , 'R' };
                char[] vocish = { 'E', 'A', 'P', 'D', 'S', 'M', 'J', 'N' };
                Vocative = new WelshEnglishPiece(vocish);
                Subject = new WelshEnglishPiece(nounish);
                char[] verbish = { 'V', 'B'};
                VerbPart = new WelshEnglishPiece(verbish);
                Predicate = new WelshEnglishPiece("APDSMJNRVC");
                Object = new WelshEnglishPiece(new char[]{ 'A', 'P', 'D', 'S', 'M', 'J', 'N', 'R', 'V' }); // added verb
                char[] presPartish = { 'R', 'V'};
                PresParticiple = new WelshEnglishPiece(presPartish);

                char[] prepositional = { 'R', 'N', 'J', 'A'};
                Prepositional = new WelshEnglishPiece(nounish);

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
                PresParticiple.clean();
                Prepositional.clean();
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
                Console.WriteLine("\nVerb Part:");
                VerbPart.show();
                Console.WriteLine("\nSubject Part:");
                Subject.show();
                Console.WriteLine("\nPresent Participle: ");
                PresParticiple.show();
                Console.WriteLine("\nPredicate Part:");
                Predicate.show();
                Console.WriteLine("\nObject Part:");
                Object.show();
                //
                //
                //TODO

            }
        }//
        class WelshEnglishWordTable
        {
            int currentSize;
            char change;
            const int MAXSIZE = 1000;
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

                //Verbs
                //pushLex("mynd", "goes", "VIX3SP");
                pushLex("mynd", "go", "VIX1PP").addIRPP("going");
                
                pushLex("dysgu", "learn", "VRX1PP"); //also means teach...
                pushLex("gorwedd", "lie", "VRX1PP").addIRPP("lying");
                pushLex("hoffi", "like", "VRX1PP");
                pushLex("chwarae", "play", "VRX1PP").addIRPP("playing");
                pushLex("darllen", "read", "VRX1PP").addIRPP("reading");
                pushLex("rhedeg", "run", "VRX1PP");
                pushLex("canu", "sing", "VRX1PP"); //or to play an instrument
                pushLex("eistedd", "sit", "VRX1PP");
                pushLex("sefyll", "stand", "VRX1PP");
                pushLex("cerdded", "walk", "VRX1PP");
                pushLex("gwisgo", "wear", "VRX1PP").addIRPP("wearing");
                pushLex("gweithio", "work", "VRX1PP");
                pushLex("ysgrifennu", "write", "VRX1PP");
                pushLex("cysgu", "sleep", "VRX1PP").addIRPP("sleeping");
                //To be verbs
                pushLex("wyf", "am", "VIX1PP"); // I
                pushLex("wyt", "art", "VIX2PP"); // Thou
                pushLex("mae", "is", "VIX3SP"); // he/she
                pushLex("ydym", "are", "VIX3PP"); // we
                pushLex("ydych", "are", "VIX2PP"); // you
                pushLex("maent", "are", "VIX3PP"); // they

                //Weird verb cheats for the and modifiers
                pushLex("y", "the", "AIX3SP"); // these don't get recgonized if set to articles
                pushLex("yr", "the", "AIX3SP");


                //Conjunctions
                pushLex("a", "and", "CRXNP"); //is the conjunction technically considered plural....?
                pushLex("ac", "and", "CRXNP");
                pushLex("a'r", "and the", "CRXNP");
                pushLex("ond", "but", "CRXNP");

                //Prepositions
                pushLex("ar", "on", "RRXN3P");
                pushLex("dan", "under", "RRXN3P");
                pushLex("drwy", "through", "RRXN3P");
                pushLex("trwy", "through", "RRXN3P");
                //pushLex("drwy'r", "through the", "RRXN3P");
                //pushLex("trwy'r", "through the", "RRXN3P");
                pushLex("i", "to", "RRXN3P"); //or into
                //pushLex("i'r", "to the", "RRXN3P");
                pushLex("wrth", "near", "RRXN3P"); //or by
                pushLex("yn", "in", "RRXN3P");

                //Nouns
                pushLex("Saesneg", "English", "NIXNS").setNoEI(true);
                pushLex("Cymraeg", "Welsh", "NIXNS").setNoEI(true);
                pushLex("awyrn", "aeroplane", "NIXNS");
                pushLex("eroplên", "aeroplane", "NIXNS");
                pushLex("anthem", "anthem", "NIXNS");
                pushLex("llyfr", "book", "NIXNS");
                pushLex("bachgen", "boy", "NIXNS");
                pushLex("bechgyn", "boys", "NIXNP");
                pushLex("bws", "bus", "NIXNS");
                pushLex("eglwys", "church", "NIXNS");
                pushLex("cloc", "clock", "NIXNS");
                pushLex("ci", "dog", "NIXNS");
                pushLex("drws", "door", "NIXNS");
                pushLex("fferm", "farm", "NIXNS");
                pushLex("cae", "field", "NIXNS");
                pushLex("tân", "fire", "NIXNS");
                pushLex("llawr", "floor", "NIXNS");
                pushLex("het", "hat", "NIXNS");
                pushLex("tŷ", "house", "NIXNS");
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
                pushLex("heno", "tonight", "NIXNS").setNoEI(true);
                pushLex("heddiw", "today", "NIXNS").setNoEI(true);
                pushLex("dyn", "man", "NIXNS");
                pushLex("dydd", "day", "NIXNS");
                pushLex("bore", "morning", "NIXNS");


                pushLex("wythnos", "week", "NIXFS");
                pushLex("cadair", "chair", "NIXFS");
                pushLex("pib", "pipe", "NIXFS");
                pushLex("tref", "town", "NIXFS");
                pushLex("gardd", "garden", "NIXFS");
                pushLex("basged", "basket", "NIXFS");
                pushLex("desg", "desk", "NIXFS");
                pushLex("mam", "mother", "NIXFS");
                pushLex("geneth", "girl", "NIXFS");
                pushLex("ynys", "island", "NIXFS");
                pushLex("cegin", "kitchen", "NIXFS");
                pushLex("ysgol", "school", "NIXFS");
                

                //automatically push extra words for feminine "the"
                int count = lexBuildingIndex;
                for(int i = 0;i < count; i++)
                {
                    if(lex[i].gender() == 'F')
                    {
                        string mutated = lex[i].welsh().Substring(1);
                        string newStart = lex[i].welsh()[0] + "";
                        switch (lex[i].welsh()[0])
                        {
                            case 'c':
                                newStart = "g";
                                break;
                            case 'p':
                                newStart = "b";
                                break;
                            case 't':
                                newStart = "d";
                                break;
                            case 'g':
                                newStart = "";
                                break;
                            case 'b':
                                newStart = "f";
                                break;
                            case 'd':
                                newStart = "dd";
                                break;
                            case 'm':
                                newStart = "f";
                                break;
                        }
                        if(newStart + mutated != lex[i].welsh()) // only push new one if it changes
                            pushLex(newStart + mutated, lex[i].english(), lex[i].grammar());
                    }
                }

                //demonstrative adjectives
                pushLex("hwn", "this", "DRXMS");
                pushLex("hon", "this", "DRXFS");
                pushLex("hyn", "these", "DRXNP");
                pushLex("bob", "every", "DRXNP");
                pushLex("pob", "every", "DRXNP");
                pushLex("hwnnw", "that", "DRXMS");
                pushLex("honno", "that", "DRXFS");
                pushLex("hynny", "those", "DRXNP");



                //Pronouns
                pushLex("fi", "I", "PIXNP");
                pushLex("i", "I", "PIXNP");
                pushLex("ti", "thou", "PIXNP");
                pushLex("ef", "he", "PIXMS");
                pushLex("hi", "she", "PIXFS");
                pushLex("ni", "we", "PIXMP");
                pushLex("chwi", "you", "PIXNP");
                pushLex("hwy", "they", "PIXMP");
                //set current size after loading all the words
                currentSize = lexBuildingIndex;
            }
            /*
             * This function exists solely to help me keep this dictionary clean when I try to add new things
             * */
            private WordEntry pushLex(String welsh, String English, String info)
            {
                return lex[lexBuildingIndex++] = new WordEntry(welsh, English, info);
            }
            public WordEntry find(String word)
            {
                return find(word, 1);
            }
            public WordEntry find(String word, int attemptNumber)
            {
                int versionsFound = 0;
                //search function. Is the word provided english or welsh?
                int lastFoundIndex = -1; // backup incase the second attempt fails
                for (int i = 0; i < currentSize; i++)
                {
                    if (lex[i].welsh() == word)
                    {
                        lastFoundIndex = i;
                        if (++versionsFound == attemptNumber)
                            return lex[i];
                        else
                            continue;
                        
                    }
                }
                if (lastFoundIndex != -1) return lex[lastFoundIndex];
                String cutDefinite = word.Substring(0, word.Length - (word.EndsWith("'r") ? 2 : 0)); //removes 'r that may be added from definite article
                for (int i = 0; i < currentSize; i++)
                {
                    if (lex[i].welsh() == cutDefinite)
                    {
                        lastFoundIndex = i;
                        if (++versionsFound == attemptNumber)
                            return lex[i];
                        else
                            continue;
                    }
                }
                if (lastFoundIndex != -1) return lex[lastFoundIndex];

                Console.WriteLine("No Definition found for: " + word);
                return new WordEntry(" ", "UNKNOWN", "Z     ");
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
                getPresentParticiple();


                getPredicate();
                getObject();

                //fix the am liking scenario




                adjustEnds();
            }
            void getPresentParticiple()
            {
                bool like = !ic.PresParticiple.empty() && ic.PresParticiple.tell('V').english() == "like";
                if (like)
                {
                    if (!ic.PresParticiple.isEmpty('R') && !ic.PresParticiple.isEmpty('V'))
                    {
                        String word = ic.PresParticiple.tell('V').english();
                        if (!ic.VerbPart.isPlural())
                        {
                            if (ic.PresParticiple.tell('V').isRegular())
                            {
                                word += "s";
                            }
                        }
                        words[actualLength++] = word;
                    }
                }
                else if (!ic.PresParticiple.empty())
                {
                    if (!ic.PresParticiple.isEmpty('R') && !ic.PresParticiple.isEmpty('V'))
                    {
                        String english = ic.PresParticiple.tell('V').english();
                        if ("aeiou".Contains(english[english.Length - 1]))
                        {
                            english = english.Substring(0, english.Length - 1);
                        }
                        else if ("aeiou".Contains(english[english.Length - 2])) english += english[english.Length - 1];
                        english += "ing";

                        if (ic.PresParticiple.tell('V').hasIrregularPresentParticiple()) english = ic.PresParticiple.tell('V').irregularPresentParticiple;

                        words[actualLength++] = english;
                    }
                }
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

                    if (!p.isEmpty('V'))
                    {
                        words[actualLength++] = "to " + p.tell('V').english();
                    }
                    if (!p.isEmpty('R'))
                    {
                        words[actualLength] = p.tell('R').english();
                        actualLength++;
                    }
                    if (!p.isEmpty('A') && p.isEmpty('D'))
                    {
                        words[actualLength++] = p.tell('A').english();
                    }
                    if (p.isDefinite() && p.isEmpty('P') && (!p.hasPiece('D') || p.isEmpty('D')))
                        
                    {
                        //otherwise use the indefinite article, which doesn't exist in welsh
                        words[actualLength++] = "the";
                    } else
                    if ((p.isEmpty('S') && p.isEmpty('D') && p.isEmpty('M') && p.isEmpty('P') && !p.isEmpty('N') && p.isEmpty('A')) || (!p.isDefinite() && p.isEmpty('P') && p.isEmpty('A') && p.isEmpty('V') && p.isEmpty('D'))) 
                    {
                        //no possessive, demonstrative, numeral, pronoun, but noun is present
                        //OR if the piece has been deemed definite because of the Y before it
                        if (!p.isEmpty('N') && !p.tell('N').noEnglishIndefinite)
                        {
                            words[actualLength] = "a";
                            actualLength++;
                        }
                    } 




                    if (!p.isEmpty('P'))
                    {
                        words[actualLength] = p.tell('P').english();
                        actualLength++;
                    }
                    if (!p.isEmpty('D'))
                    {
                        words[actualLength] = p.tell('D').english();
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
                    if (!p.isEmpty('C'))
                    {
                        words[actualLength++] = p.tell('C').english();
                    }
                    
                    //Console.WriteLine("Extra Prepositionals: " + p.getExtraPrepositionalsCount());
                    for(int i = 0;i < p.getExtraPrepositionalsCount(); i++)
                    {
                        getNounPart(p.getExtraPrepositional(i));
                    }
                }
            }
            void getVerbPart()
            {
                bool liking = !ic.PresParticiple.empty() && ic.PresParticiple.tell('V').english() == "like";
                if (!ic.VerbPart.empty() && !liking)
                {
                    String word = ic.VerbPart.tell('V').english();
                    //fix the ending of the word
                    if (!ic.VerbPart.isPlural())
                    {
                        if (ic.VerbPart.tell('V').isRegular())
                        {
                            word += "s";
                        }
                    } else
                    {
                        //is plural
                        if(word == "is")
                        {
                            //stupidly irregular case
                            word = "are";
                        }
                    }


                    words[actualLength] = word;
                    actualLength++;
                }
            }
            void getPredicate()
            {
                
                getNounPart(ic.Predicate);
            }
            void getObject()
            {
                if (!ic.Object.isEmpty('V'))
                {
                    words[actualLength++] = "to " + ic.Object.tell('V');
                }
                getNounPart(ic.Object);
                //FIX THIS
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
            public void toSentence()
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
            public String getSentence()
            {
                
                return outSentence;
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
            public WelshToEnglish(String filename)
            {
                source = new Source(filename);
                Console.WriteLine("Loaded testfile with " + lines + " Lines");
                scanner = new Scanner(source);
                code = new WelshEnglishIntermediateCode(PIECENUMBER);
                table = new WelshEnglishWordTable();
                parser = new WelshParser(scanner, code, table);
                generator = new EnglishGenerator(code, table);
                output = new EnglishOutput(generator);
            }

            public void runTestFile()
            {
                int index = 0;
                int correctCounter = 0;
                Console.WriteLine("Starting test....");
                while (scanner.scanSentence())
                {
                    //scanner.show();
                    parser.DoParse();
                    //code.Show();

                    generator.generate();
                    //generator.show();
                    output.toSentence();
                    if(output.getSentence() == testData[index, 1])
                    {
                        //correct
                        correctCounter++;
                       
                    } else
                    {
                        Console.WriteLine("Translate incorrect for line: " + ((index + 1) * 2 - 1));
                    }
                    index++;
                }
                Console.WriteLine("Test Completed! " + correctCounter + " / " + lines + " translations correct");
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


                Console.OutputEncoding = System.Text.Encoding.Unicode;//required to be able to read that stupid upside down h
                Console.InputEncoding = System.Text.Encoding.Unicode;

                WelshToEnglish we = new WelshToEnglish("welshtest.txt");

                we.runTestFile();
                WelshToEnglish manualInput = new WelshToEnglish();
                for (int i = 0; i < 25; i++)
                {
                    manualInput.doASentence();
                }
                
            }
        }
    }
   
}
