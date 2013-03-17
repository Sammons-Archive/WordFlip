using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace WordFlip
{
    internal class WordFinder
    {
        private static Dictionary<string,string> _contents = new Dictionary<string, string>();
        public WordFinder ()
        {
            Read();
        }

    public async void Read()
    {
        var file = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync(@"infl");
        var stream = await file.OpenStreamForReadAsync();
        var streamReader = new StreamReader(stream);
        var contents = await streamReader.ReadToEndAsync();
        var contentAry = contents.Split(" ".ToCharArray());
        foreach (var str in contentAry.Where(str => !_contents.ContainsKey(str.ToLower())))
        {
            _contents.Add(str.ToLower(),str.ToLower());
        }

    }


        /*
         This function reads the contents of the infl library, which contains all of the words
         */

        public static List<string> GetPerms(string entry)
        {
            //_contents = _contents.ToLower();
            var attemp = PickWords(entry);
            return attemp;
        }

        private static List<string> PickWords(string input_)
        {
            var input = input_;
            input = input.Replace(" ", "");
            var output = new List<string>();
  
            while (output.ToArray().Length < 1 && input.Length > 2)
            {
                var last = 0;
                var list = EveryPerm(input, ref last);
                output.AddRange(from key in list
                                let stringTemp = key.Value.ToLower()
                                where _contents.ContainsKey(key.Value) && key.Value.Length > 2 && key.Value != input && !output.Contains(key.Value)
                                select stringTemp);
                input = input.Substring(1);

            }
            return output;
        }

        private static Dictionary<int, string> EveryPerm(string str, ref int last)
        {
            var results = Permutation(str, ref last);
            for (var i = 0; i < str.Length; i++)
            {
                foreach (var ch in str.ToCharArray())
                {
                var result = Permutation(str.Replace(ch.ToString(),""), ref last);
                    foreach (var a in result)
                    {
                            results.Add(a.Key, a.Value);
                    }
                }
                str = str.Replace(str[0].ToString(), "");
            }
            return results;
        }

        private static Dictionary<int,string> Permutation(string str, ref int last)
        {
            if (str.Length < 2)
            {
                var ans = new Dictionary<int, string>{{last,str}};
                last++;
                return ans;
            }

            var perms = Permutation(str.Substring(1), ref last);
            var ch = str[0];
            var result = new Dictionary<int, string>();
            foreach (var perm in perms)
            {
                for (var i = 0; i < perm.Value.Length + 1; i++)
                {
                    result.Add(last,perm.Value.Substring(0,i)+ch+perm.Value.Substring(i));
                    last++;
                }
            }
            return result;
        }
    }
}
