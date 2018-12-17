using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FSIndexer
{
    public static class TermOptions
    {
        public static class ExcludeRules
        {
            public static int IgnoreTermsWithChildrenLessThan = 1;
            public static int IgnoreTermsWithLengthLessThan = 2;
            private static int _ignoreTermsWithAgeGreaterThan = 0;
            public static bool IgnoreTermsThatAreAllNumbers = true;
            // Would change my1234file to myfile
            public static bool RemoveNumbersFromTerms = false;
            // Would change my1234file to my.1234.file
            public static bool InsertSeperatorBetweenNumbersInTerms = true; // setting RemoveNumbersFromTerms to true will make this do nothing
            public static long MinimumSizeToIndexInB = 1024 * 1024 * 30; // X MB
            public static long MinimumSizeToIndexInMB
            {
                get
                {
                    return MinimumSizeToIndexInB / 1024 / 1024;
                }
                set
                {
                    MinimumSizeToIndexInB = value * 1024 * 1024;
                }
            }

            public static long MinimumSizeToKeepInB = 1024 * 1024 * 30; // X MB
            public static long MinimumSizeToKeepInMB
            {
                get
                {
                    if (MinimumSizeToKeepInB < 1024 * 1024)
                    {
                        return 0;
                    }
                    else
                    {
                        return MinimumSizeToKeepInB / 1024 / 1024;
                    }
                }
                set
                {
                    MinimumSizeToKeepInB = value * 1024 * 1024;
                }
            }

            private static int MaxAge = (int)DateTime.Now.Subtract(DateTime.MinValue).TotalDays;
            public static int IgnoreTermsWithAgeGreaterThan
            {
                get
                {
                    if (_ignoreTermsWithAgeGreaterThan <= 0)
                    {
                        _ignoreTermsWithAgeGreaterThan = MaxAge;
                    }

                    return _ignoreTermsWithAgeGreaterThan;
                }
                set
                {
                    if (value <= 0)
                        _ignoreTermsWithAgeGreaterThan = MaxAge;
                    else
                        _ignoreTermsWithAgeGreaterThan = value;
                }
            }
        }

        public static List<string> AutoSkipTags =
            new List<string>()
            {
                "and",
                "the",
                "with",
                "a",
                "my",
                "that",
                "her",
                "in",
                "ive",
                "girl",
                "to",
                "who",
                "where",
                "for",
                "you",
                "on",
                "of",
                "it"
            };

        public static List<string> AutoRemoveStartingTags =
            new List<string>()
            {
                "ktr.",
                "sexo.",
                "iak.",
                "fps.",
                "i.",
                "ohrly.",
                "c69.",
                "divas.",
                "oro.",
                "inya.",
                "vbt.",
                "mistress.",
                "ieva.",
                "ggw."
            };

        public static List<string> AutoRemoveContainingTags =
            new List<string>()
            {
                "sex4free",
                "wmv",
                "mp4",
                "dvdrip",
                "xvid",
                "540p", "540",
                "720p", "720",
                "1080p", "1080",
                "1280x", "1920x", "x264",
                "hd",
                "dl",
                "web",
                "avc",
                "aac",
                "[","]",
                "{","}",
                "(",")",
                ",", "&", "!", "#", "$", "%", "^", ";", "'", "?"
            };

        public static List<KeyValuePair<string, string>> AutoReplaceTags 
        {
            get
            {
                return AutoReplaceFormatingTags.Union(AutoReplaceContentTags).Distinct().ToList();
            }
        }

        private static List<KeyValuePair<string, string>> AutoReplaceFormatingTags =
            new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>(" ", "."),
                new KeyValuePair<string, string>("-", "."),
                new KeyValuePair<string, string>(" ", "."),
                new KeyValuePair<string, string>("--", "."),
                new KeyValuePair<string, string>("..", "."),
                new KeyValuePair<string, string>("_", "."),
                new KeyValuePair<string, string>("  ", "."),
                new KeyValuePair<string, string>(" .", "."),
                new KeyValuePair<string, string>(". ", "."),
            };

        private static List<KeyValuePair<string, string>> AutoReplaceContentTags =
            new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("cd", "cd"),
                new KeyValuePair<string, string>("disc", "disc"),
                new KeyValuePair<string, string>(" ", ".")
            };

        public static bool IsPurelyNumeric(string str)
        {
            foreach (char c in str)
            {
                if (!char.IsNumber(c))
                    return false;
            }

            return true;
        }

        public static string RemoveNumbers(string str)
        {
            string newStr = "";

            foreach (char c in str)
            {
                if (!char.IsNumber(c))
                    newStr += c;
            }

            return newStr;
        }

        // Would change my1234file to my.1234.file
        public static string InsertSeperatorBetweenNumbers(string str)
        {
            if (!ExcludeRules.InsertSeperatorBetweenNumbersInTerms)
                return str;

            // If there are no numbers in the string just return it
            // If the string is all numbers then just return it
            if (str.Length == RemoveNumbers(str).Length || IsPurelyNumeric(str))
                return str;

            string sep = IndexSeperators.PrimarySeperator;
            string newStr = "";
            string lastChar = string.Empty;

            foreach (char c in str)
            {
                if (lastChar != string.Empty)
                {
                    // Last char was a number, this char is not
                    if (char.IsNumber(c) && !char.IsNumber(lastChar[0]))
                    {
                        newStr += sep;
                    }
                    // Last char was not a number, this char is
                    else if (!char.IsNumber(c) && char.IsNumber(lastChar[0]))
                    {
                        newStr += sep;
                    }
                }

                newStr += c;
                lastChar = c.ToString();
            }

            return newStr;
        }

        public static bool Exclude(string term)
        {
            if (Main.MoveTrackerList.MoveLocationExists(term))
                return false;

            if (ExcludeRules.IgnoreTermsThatAreAllNumbers && IsPurelyNumeric(term))
                return true;

            return false;
        }

        public static string ApplyRules(string term)
        {
            if (ExcludeRules.RemoveNumbersFromTerms)
                term = RemoveNumbers(term);

            if (Main.MoveTrackerList.MoveLocationExists(term))
                return term;

            if (term.Length < ExcludeRules.IgnoreTermsWithLengthLessThan)
                return string.Empty;

            return term;
        }

        public static List<string> ApplyRules(List<string> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                list[i] = ApplyRules(list[i]);
            }

            list = RemoveExcludedItems(list);

            return list;
        }

        private static List<string> RemoveExcludedItems(List<string> list)
        {
            var newList = new List<string>();

            foreach (var item in list)
            {
                if (Exclude(item))
                    continue;

                if (string.IsNullOrEmpty(item))
                    continue;

                newList.Add(item);
            }

            return newList;
        }
    }
}
