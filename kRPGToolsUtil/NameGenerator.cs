using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PCLStorage;

//http://forum.codecall.net/topic/49665-java-random-name-generator/
namespace kRPGToolsUtil
{

/**
 * This class is released under GNU general public license
 * 
 * Description: This class generates random names from syllables, and provides programmer a 
 * simple way to set a group of rules for generator to avoid unpronounceable and bizarre names. 
 * 
 * SYLLABLE FILE REQUIREMENTS/FORMAT: 
 * 1) all syllables are separated by line break. 
 * 2) Syllable should not contain or start with whitespace, as this character is ignored and only first part of the syllable is read. 
 * 3) + and - characters are used to set rules, and using them in other way, may result in unpredictable results.
 * 4) Empty lines are ignored.
 * 
 * SYLLABLE CLASSIFICATION:
 * Name is usually composed from 3 different class of syllables, which include prefix, middle part and suffix. 
 * To declare syllable as a prefix in the file, insert "-" as a first character of the line.  
 * To declare syllable as a suffix in the file, insert "+" as a first character of the line.  
 * everything else is read as a middle part.
 * 
 * NUMBER OF SYLLABLES: 
 * Names may have any positive number of syllables. In case of 2 syllables, name will be composed from prefix and suffix. 
 * In case of 1 syllable, name will be chosen from amongst the prefixes.
 * In case of 3 and more syllables, name will begin with prefix, is filled with middle parts and ended with suffix.
 * 
 * ASSIGNING RULES:
 * I included a way to set 4 kind of rules for every syllable. To add rules to the syllables, write them right after the 
 * syllable and SEPARATE WITH WHITESPACE. (example: "aad +v -c"). The order of rules is not important.
 * 
 * RULES:
 * 1) +v means that next syllable must definitely start with a Vowel.
 * 2) +c means that next syllable must definitely start with a consonant.
 * 3) -v means that this syllable can only be added to another syllable, that ends with a Vowel.
 * 4) -c means that this syllable can only be added to another syllable, that ends with a consonant.
 * So, our example: "aad +v -c" means that "aad" can only be after consonant and next syllable must start with Vowel.
 * Beware of creating logical mistakes, like providing only syllables ending with consonants, but expecting only Vowels, which will be detected 
 * and Exception will be thrown.
 * 
 * TO START:
 * Create a new NameGenerator object, provide the syllable file, and create names using compose() method.
 * 
 *
 */
public class NameGenerator {
    readonly List<string> _pre = new List<string>();
    readonly List<string> _mid = new List<string>();
    readonly List<string> _sur = new List<string>();
    
    private static readonly char[] Vowels = {'a', 'e', 'i', 'o', 'u', 'ä', 'ö', 'õ', 'ü', 'y'};
    private static readonly char[] Consonants = {'b', 'c', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'm', 'n', 'p', 'q', 'r', 's', 't', 'v', 'w', 'x', 'y'};

    readonly Random _random = new Random();
    private string _fileName;
    
    /**
     * Create new random name generator object. refresh() is automatically called.
     * @param fileName insert file name, where syllables are located
     * @throws IOException
     */
    public NameGenerator(string file) 
    {
        _fileName = file;
        Refresh();
    }
    
    /**
     * Change the file. refresh() is automatically called during the process.
     * @param fileName insert the file name, where syllables are located.
     * @throws IOException
     */
    public void ChangeFile(string file){
        if (file == null)
        {
            throw new FileNotFoundException("File name cannot be null");
        }
        _fileName = file;
        Refresh();
    }
    
    /**
     * Refresh names from file. No need to call that method, if you are not changing the file during the operation of program, as this method
     * is called every time file name is changed or new NameGenerator object created.
     * @throws IOException
     */
    public void Refresh() 
    {
        //Using Install-Package PCLStorage for the nuget package
        IFolder localStorage = FileSystem.Current.LocalStorage;
        var path = Path.GetDirectoryName(_fileName);
        Task<IFolder> contentFolderTask = localStorage.GetFolderAsync(path);
        Task.WaitAll(contentFolderTask);
        var contentFolder = contentFolderTask.Result;

        var fileName = Path.GetFileName(_fileName);
        var fileTask = contentFolder.GetFileAsync(fileName);
        Task.WaitAll(fileTask);
        var file = fileTask.Result;

        var fileContentTask = file.ReadAllTextAsync();
        Task.WaitAll(fileContentTask);
        string fileContent = fileContentTask.Result;
        using (StringReader sr = new StringReader(fileContent))
        {
            string line = "";
            while (line != null)
            {
                line = sr.ReadLine();
                if (!string.IsNullOrEmpty(line))
                {
                    if (line.StartsWith("-"))
                    {
                        _pre.Add(line.Substring(1).ToLower());
                    }
                    else if (line.StartsWith("+"))
                    {
                        _sur.Add(line.Substring(1).ToLower());
                    }
                    else
                    {
                        _mid.Add(line.ToLower());
                    }
                }
            }
        }
    }
        
    private static string Upper(string s)
    {
        return s.Substring(0,1).ToUpper() + s.Substring(1);
    }
    
    private bool ContainsConsFirst(IEnumerable<string> array)
    {
        return array.Any(ConsonantFirst);
    }

    private bool ContainsVocFirst(IEnumerable<string> array)
    {
        return array.Any(VowelFirst);
    }

    private static bool AllowCons(IEnumerable<string> array)
    {
        return array.Any(s => HatesPreviousVowels(s) || !HatesPreviousConsonants(s));
    }

    private static bool AllowVocs(IEnumerable<string> array)
    {
        return array.Any(s => HatesPreviousConsonants(s) || !HatesPreviousVowels(s));
    }

    private static bool ExpectsVowel(string s)
    {
        return s.Substring(1).Contains("+v");
    }

    private static bool ExpectsConsonant(string s)
    {
        return s.Substring(1).Contains("+c");
    }

    private static bool HatesPreviousVowels(string s)
    {
        return s.Substring(1).Contains("-c");
    }

    private static bool HatesPreviousConsonants(string s)
    {
        return s.Substring(1).Contains("-v");
    }

    private static string PureSyl(string s){
        s = s.Trim();
        if (s.StartsWith("+") || s.StartsWith("-"))
        {
            s = s.Substring(1);
        }
        return s.Split(' ')[0];
    }
    
    private bool VowelFirst(string s)
    {
        return Vowels.Contains(First(s));
    }
    
    private bool ConsonantFirst(string s)
    {
        return Consonants.Contains(First(s));
    }
    
    private bool VowelLast(string s)
    {
        return Vowels.Contains(Last(s));
    }
    
    private bool ConsonantLast(string s)
    {
        return Consonants.Contains(Last(s));        
    }

    private static char First(string s, bool toLower = true)
    {
        if (string.IsNullOrEmpty(s))
        {
            return "".ToCharArray()[0];
        }
        string l = s.Substring(0,1);
        if (toLower)
        {
            l = l.ToLower();
        }
        return l.ToCharArray()[0];
    }
    private static char Last(string s, bool toLower = true)
    {
        if (string.IsNullOrEmpty(s))
        {
            return "".ToCharArray()[0];
        }
        string l = s.Substring(s.Length - 1);
        if (toLower)
        {
            l = l.ToLower();
        }
        return l.ToCharArray()[0];
    }
    /**
     * Compose a new name.
     * @param syls The number of syllables used in name.
     * @return Returns composed name as a String
     * @throws Exception when logical mistakes are detected inside chosen file, and program is unable to complete the name.
     */
    public string Compose(int syls){
        if(syls > 2 && _mid.Count == 0) throw new Exception("You are trying to create a name with more than 3 parts, which requires middle parts, " +
                "which you have none in the file "+_fileName+". You should add some. Every word, which doesn't have + or - for a prefix is counted as a middle part.");
        if(_pre.Count == 0) throw new Exception("You have no prefixes to start creating a name. add some and use \"-\" prefix, to identify it as a prefix for a name. (example: -asd)");
        if(_sur.Count == 0) throw new Exception("You have no suffixes to end a name. add some and use \"+\" prefix, to identify it as a suffix for a name. (example: +asd)");
        if(syls < 1) throw new Exception("compose(int syls) can't have less than 1 syllable");
        int expecting = 0; // 1 for Vowel, 2 for consonant
        int last; // 1 for Vowel, 2 for consonant
        int a = (int)(_random.NextDouble() * _pre.Count);
        
        last = VowelLast(PureSyl(_pre[a])) ? 1 : 2;
        
        if(syls > 2){
            if(ExpectsVowel(_pre[a])){ 
                expecting = 1;                
                if(!ContainsVocFirst(_mid)) throw new Exception("Expecting \"middle\" part starting with Vowel, " +
                        "but there is none. You should add one, or remove requirement for one.. "); 
            }
            if(ExpectsConsonant(_pre[a])){ 
                expecting = 2;                
                if(!ContainsConsFirst(_mid)) throw new Exception("Expecting \"middle\" part starting with consonant, " +
                "but there is none. You should add one, or remove requirement for one.. "); 
            }
        }
        else{
            if(ExpectsVowel(_pre[a])){ 
                expecting = 1;                
                if(!ContainsVocFirst(_sur)) throw new Exception("Expecting \"suffix\" part starting with Vowel, " +
                        "but there is none. You should add one, or remove requirement for one.. "); 
            }
            if(ExpectsConsonant(_pre[a])){ 
                expecting = 2;                
                if(!ContainsConsFirst(_sur)) throw new Exception("Expecting \"suffix\" part starting with consonant, " +
                "but there is none. You should add one, or remove requirement for one.. "); 
            }
        }
        if(VowelLast(PureSyl(_pre[a])) && !AllowVocs(_mid)) throw new Exception("Expecting \"middle\" part that allows last character of prefix to be a Vowel, " +
        "but there is none. You should add one, or remove requirements that cannot be fulfilled.. the prefix used, was : \""+_pre[(a)]+"\", which" +
        "means there should be a part available, that has \"-v\" requirement or no requirements for previous syllables at all.");
        
        if(ConsonantLast(PureSyl(_pre[a])) && !AllowCons(_mid)) throw new Exception("Expecting \"middle\" part that allows last character of prefix to be a consonant, " +
        "but there is none. You should add one, or remove requirements that cannot be fulfilled.. the prefix used, was : \""+_pre[(a)]+"\", which" +
        "means there should be a part available, that has \"-c\" requirement or no requirements for previous syllables at all.");
        
        int[] b = new int[syls];
        for(int i = 0; i<b.Length-2; i++){
                        
            do{
                b[i] = (int)(_random.NextDouble() * _mid.Count);
                //System.out.println("exp " +expecting+" VowelF:"+VowelFirst(mid.get(b[i]))+" syl: "+mid.get(b[i]));
            }            
            while(     (expecting == 1 && !VowelFirst(PureSyl(_mid[b[i]])))
                    || (expecting == 2 && !ConsonantFirst(PureSyl(_mid[b[i]])))
                    || (last == 1 && HatesPreviousVowels(_mid[b[i]]))
                    || (last == 2 && HatesPreviousConsonants(_mid[b[i]])));
            
            expecting = 0;
            if(ExpectsVowel(_mid[b[i]]))
            { 
                expecting = 1;
                if (i < b.Length - 3 && !ContainsVocFirst(_mid)) throw new Exception("Expecting \"middle\" part starting with Vowel, " +
                        "but there is none. You should add one, or remove requirement for one.. ");
                if (i == b.Length - 3 && !ContainsVocFirst(_sur)) throw new Exception("Expecting \"suffix\" part starting with Vowel, " +
                "but there is none. You should add one, or remove requirement for one.. "); 
            }
            if(ExpectsConsonant(_mid[(b[i])]))
            { 
                expecting = 2;                
                if(i < b.Length-3 && !ContainsConsFirst(_mid)) throw new Exception("Expecting \"middle\" part starting with consonant, " +
                "but there is none. You should add one, or remove requirement for one.. "); 
                if(i == b.Length-3 && !ContainsConsFirst(_sur)) throw new Exception("Expecting \"suffix\" part starting with consonant, " +
                "but there is none. You should add one, or remove requirement for one.. "); 
            }
            if(VowelLast(PureSyl(_mid[(b[i])])) && !AllowVocs(_mid) && syls > 3) throw new Exception("Expecting \"middle\" part that allows last character of last syllable to be a Vowel, " +
                    "but there is none. You should add one, or remove requirements that cannot be fulfilled.. the part used, was : \""+_mid[(b[i])]+"\", which " +
                    "means there should be a part available, that has \"-v\" requirement or no requirements for previous syllables at all."); 
                    
            if(ConsonantLast(PureSyl(_mid[(b[i])])) && !AllowCons(_mid) && syls > 3) throw new Exception("Expecting \"middle\" part that allows last character of last syllable to be a consonant, " +
                    "but there is none. You should add one, or remove requirements that cannot be fulfilled.. the part used, was : \""+_mid[(b[i])]+"\", which " +
                    "means there should be a part available, that has \"-c\" requirement or no requirements for previous syllables at all.");
            if(i == b.Length-3)
            {                
                if(VowelLast(PureSyl(_mid[(b[i])])) && !AllowVocs(_sur)) throw new Exception("Expecting \"suffix\" part that allows last character of last syllable to be a Vowel, " +
                        "but there is none. You should add one, or remove requirements that cannot be fulfilled.. the part used, was : \""+_mid[(b[i])]+"\", which " +
                        "means there should be a suffix available, that has \"-v\" requirement or no requirements for previous syllables at all."); 
                        
                if(ConsonantLast(PureSyl(_mid[(b[i])])) && !AllowCons(_sur)) throw new Exception("Expecting \"suffix\" part that allows last character of last syllable to be a consonant, " +
                        "but there is none. You should add one, or remove requirements that cannot be fulfilled.. the part used, was : \""+_mid[(b[i])]+"\", which " +
                        "means there should be a suffix available, that has \"-c\" requirement or no requirements for previous syllables at all.");
            }
            last = VowelLast(PureSyl(_mid[(b[i])])) ? 1 : 2;
        }        
        
        int c;
        do
        {
            c = (int) (_random.NextDouble()*_sur.Count);
        } while (   (expecting == 1 && !VowelFirst(PureSyl(_sur[c])))
                 || (expecting == 2 && !ConsonantFirst(PureSyl(_sur[c])))
                 || (last == 1 && HatesPreviousVowels(_sur[c]))
                 || (last == 2 && HatesPreviousConsonants(_sur[c])));
        
        string name = Upper(PureSyl(_pre[(a)].ToLower()));        
        for(int i = 0; i<b.Length-2; i++)
        {
            name += (PureSyl(_mid[(b[i])].ToLower()));            
        }
        if(syls > 1)
            name += (PureSyl(_sur[c].ToLower()));
        return name;        
    }
}
}
