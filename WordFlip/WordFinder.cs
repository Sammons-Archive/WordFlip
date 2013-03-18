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
        private static readonly Dictionary<string, string> _contents = new Dictionary<string, string>();

        public WordFinder()
        {
            Read();
        }

        public async void Read()
        {
            StorageFile file = await Package.Current.InstalledLocation.GetFileAsync(@"infl");
            Stream stream = await file.OpenStreamForReadAsync();
            var streamReader = new StreamReader(stream);
            string contents = await streamReader.ReadToEndAsync();
            string[] contentAry = contents.Split(" ".ToCharArray());
            foreach (string str in contentAry.Where(str => !_contents.ContainsKey(str.ToLower())))
            {
                _contents.Add(str.ToLower(), str.ToLower());
            }
        }


        /*
         This function reads the contents of the infl library, which contains all of the words
         */

        public static List<string> GetPerms(string entry)
        {
            //_contents = _contents.ToLower();
            List<string> attemp = PickWords(entry);
            return attemp;
        }

        private static List<string> PickWords(string input_)
        {
            string input = input_;
            input = input.Replace(" ", "");
            var output = new List<string>();

            while (output.ToArray().Length < 1 && input.Length > 2)
            {
                int last = 0;
                Dictionary<int, string> list = EveryPerm(input, ref last);
                output.AddRange(from key in list
                                let stringTemp = key.Value.ToLower()
                                where
                                    _contents.ContainsKey(key.Value) && key.Value.Length > 2 && key.Value != input &&
                                    !output.Contains(key.Value)
                                select stringTemp);
                input = input.Substring(1);
            }
            return output;
        }

        private static Dictionary<int, string> EveryPerm(string str, ref int last)
        {
            Dictionary<int, string> results = Permutation(str, ref last);
            for (int i = 0; i < str.Length; i++)
            {
                foreach (char ch in str.ToCharArray())
                {
                    Dictionary<int, string> result = Permutation(str.Replace(ch.ToString(), ""), ref last);
                    foreach (var a in result)
                    {
                        results.Add(a.Key, a.Value);
                    }
                }
                str = str.Replace(str[0].ToString(), "");
            }
            return results;
        }

        private static Dictionary<int, string> Permutation(string str, ref int last)
        {
            if (str.Length < 2)
            {
                var ans = new Dictionary<int, string> {{last, str}};
                last++;
                return ans;
            }

            Dictionary<int, string> perms = Permutation(str.Substring(1), ref last);
            char ch = str[0];
            var result = new Dictionary<int, string>();
            foreach (var perm in perms)
            {
                for (int i = 0; i < perm.Value.Length + 1; i++)
                {
                    result.Add(last, perm.Value.Substring(0, i) + ch + perm.Value.Substring(i));
                    last++;
                }
            }
            return result;
        }
    }
}