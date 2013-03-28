using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.ApplicationModel;
using Windows.Storage;

namespace WordFlip
{
    internal class WordFinder
    {
        private static readonly HashSet<string> _contents = new HashSet<string>();//declare hashset to hold dictionary of words
        private HashSet<string> resultList;

        public WordFinder()
        {
            ReadInDictionary();
        }

        public async void ReadInDictionary()//get the words that makeup the dictionary
        {
            StorageFile file = await Package.Current.InstalledLocation.GetFileAsync(@"infl");//get file
            Stream stream = await file.OpenStreamForReadAsync();//open file
            var streamReader = new StreamReader(stream);
            string contents = await streamReader.ReadToEndAsync();//read out file
            string[] contentAry = contents.Split(",".ToCharArray());//seperate out words
            foreach (string str in contentAry.Where(str => !_contents.Contains(str.ToLower())))//add each word to the hashset
            {
                if (str.Length < 2) continue;//single letters are not words so don't add them
                var str_ = str.Replace("\r\n", "");
                _contents.Add(str_.ToLower());
            }
        }


        public HashSet<string> GetPerms(string entry)//exposed function to get permuations that match -- calls pickwords
        {
            PickWords(entry);
            resultList.Remove(entry);
            return resultList;
        }

        private HashSet<string> PickWords(string input_)//function calls Permuation for the permutations with sme manipulations of the string that permutation can't do easily
        {
            resultList = new HashSet<string>();//initalize resultList
            var input = (input_.Replace(" ", "")).ToLower();//remove spaces from input just in case and bring the letters to lowercase
            var output = Permutation(input);//calculate perms for whole word : note the permutation function adds words to the resultlist as it goes.
            while (input.Length > 3)//not really worried about words < 2 chars long, for the other lengths work
            {
                for (var i = 0; i < input.Length; i++)//iteratively remove each letter and run the permutation calculator
                {
                    var temp = input.Remove(i, 1);
                    Permutation(temp);//calculate permutations for this sub-word
                }
               input = input.Remove(input.Length-1, 1);//chop off the last letter of the word -- reverse of what the actual permuation calculator does
                                                       //to me this is a large reason forthe existence of this function (without this an entry like 'enter here' never returned 'enter'
            }
            return output;
        }

        private HashSet<string> Permutation(string str)//actual permutation calculating function (recursive)
        {
            if (resultList.Count > 100) return resultList;//I think 100 is plenty

            if (str.Length < 2)//if str length is 1 just finish
            {
                var ans = new HashSet<string> { str };//list with just the input word in it, this will be a single char
                return ans;
            }

            HashSet<string> perms = Permutation(str.Substring(1));//chop the first letter and send off the next call
            char ch = str[0];//get the first letter
            var result = new HashSet<string>();//this is the result list
            foreach (var perm in perms)//shuffle the first letter through each position in each word returned by previous calls
            {
                for (int i = 0; i < perm.Length + 1; i++)
                {
                    var p = (perm.Substring(0, i) + ch + perm.Substring(i));//shuffle algorithm
                    if (_contents.Contains(p) && !resultList.Contains(p) &&p.Length>2) resultList.Add(p);//internal check-- if matches dictionary add it now
                    if (!result.Contains(p)) result.Add(p);
                }
            }
            return result;//return the new list of perms to function caller
        }


    }
}