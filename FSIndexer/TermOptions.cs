using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

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

        public static bool IgnoreItem(string fullPath)
        {
            string ext = new FileInfo(fullPath).Extension;

            return true;
        }

        public static List<string> IgnoreExtensions =
            new List<string>()
            {
                ".!qb",
                ".parts"
            };

        public static List<string> AutoRemoveStartingTags =
            new List<string>()
            {
            };

        public static List<string> AutoRemoveContainingTags =
            new List<string>()
            {
                "wmv",
                "mp4",
                "dvdrip",
                "xvid",
                "540p", "540",
                "720p", "720",
                "1080p", "1080",
                "1280x", "1920x", "x264",
                ".hd.",
                ".dl.",
                ".web.",
                ".avc.",
                ".aac.",
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

        private class MonthData
        {
            public int Index { get; set; }
            public string Name { get; set; }
            public string Abbreviation { get; set; }

            public string IndexTwoDigit { get { return Index.ToString().PadLeft(2, '0'); } }

            public List<string> REMReplaceMonthName1
            {
                get
                {
                    List<string> list = new List<string>();

                    list.Add(@"\.((?:\d{4}|\d{2}))\.(" + Name + @")\.((?:\d{4}|\d{2}))");
                    list.Add(@"\.((?:\d{4}|\d{2}))\.(" + Abbreviation + @")\.((?:\d{4}|\d{2}))");

                    return list;
                }
            }

            public string RPMonthName1 { get { return @".$1." + IndexTwoDigit + ".$3"; } }

            public List<string> REMReplaceMonthName2
            {
                get
                {
                    List<string> list = new List<string>();

                    list.Add(Name + @"\.(\d{2})\.(\d{2})\.(\d{4})");
                    list.Add(Abbreviation + @"\.(\d{2})\.(\d{2})\.(\d{4})");

                    return list;
                }
            }

            public string RPMonthName2 { get { return @".$3." + IndexTwoDigit + ".$2"; } }

            public static List<string> REMPutYearFirst
            {
                get
                {
                    List<string> list = new List<string>();

                    list.Add(@"\.([0-1][^3-9])\.(\d{2})\.([1][3-9]|[2][0])");

                    return list;
                }
            }

            public static string RPYearFirst { get { return @".$3.$2.$1"; } }

            public MonthData() { }
        }

        private static readonly List<MonthData> MonthDataList = new List<MonthData>()
            {
                new MonthData() { Index = 1, Name = "january", Abbreviation = "jan" },
                new MonthData() { Index = 2, Name = "febuary", Abbreviation = "feb" },
                new MonthData() { Index = 3, Name = "march", Abbreviation = "mar" },
                new MonthData() { Index = 4, Name = "april", Abbreviation = "apr" },
                new MonthData() { Index = 5, Name = "may", Abbreviation = "may" },
                new MonthData() { Index = 6, Name = "june", Abbreviation = "jun" },
                new MonthData() { Index = 7, Name = "july", Abbreviation = "jul" },
                new MonthData() { Index = 8, Name = "august", Abbreviation = "aug" },
                new MonthData() { Index = 9, Name = "september", Abbreviation = "sept" },
                new MonthData() { Index = 10, Name = "october", Abbreviation = "oct" },
                new MonthData() { Index = 11, Name = "november", Abbreviation = "nov" },
                new MonthData() { Index = 12, Name = "december", Abbreviation = "dec" },
            };

        public static string ReplaceMonthData(string str)
        {
            string os = str;

            foreach (MonthData md in MonthDataList)
            {
                foreach (string pat in md.REMReplaceMonthName1)
                {
                    Regex regex = new Regex(pat, RegexOptions.IgnoreCase);

                    if (regex.IsMatch(str))
                    {
                        str = regex.Replace(str, md.RPMonthName1);
                    }
                }

                foreach (string pat in md.REMReplaceMonthName2)
                {
                    Regex regex = new Regex(pat, RegexOptions.IgnoreCase);

                    if (regex.IsMatch(str))
                    {
                        str = regex.Replace(str, md.RPMonthName2);
                    }
                }
            }

            //foreach (string pat in MonthData.REMPutYearFirst)
            //{
            //    Regex regex = new Regex(pat, RegexOptions.IgnoreCase);

            //    if (regex.IsMatch(str))
            //    {
            //        str = regex.Replace(str, MonthData.RPYearFirst);
            //    }
            //}

            return str;
        }

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
        // If MNL = 4 and MLL = 4, would change my1234file to my.1234.file
        // If MNL = 5 and MLL = 4, would not change my1234file
        public static string InsertSeperatorBetweenNumbers(string str, int minNumbersLength = 1, int minLettersLength = 1)
        {
            if (!ExcludeRules.InsertSeperatorBetweenNumbersInTerms)
                return str;

            // If there are no numbers in the string just return it
            // If the string is all numbers then just return it
            if (str.Length == RemoveNumbers(str).Length || IsPurelyNumeric(str))
                return str;

            string sep = IndexSeperators.PrimarySeperator;
            string newStr = "";
            string strSection = "";
            string lastChar = string.Empty;

            List<string> newStrList = new List<string>();

            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];
                if (lastChar != string.Empty)
                {
                    if (c.ToString() == sep)
                    {
                        newStrList.Add(strSection + c);
                        strSection = "";
                        continue;
                    }

                    // This char is a number, Previous char is NOT
                    if (char.IsNumber(c) && !char.IsNumber(lastChar[0]))
                    {
                        newStrList.Add(strSection + (strSection.Length >= minLettersLength ? sep : ""));
                        strSection = "";
                    }
                    // This char is NOT a number, Previous char is
                    else if (!char.IsNumber(c) && char.IsNumber(lastChar[0]))
                    {
                        newStrList.Add(strSection + (strSection.Length >= minNumbersLength ? sep : ""));
                        strSection = "";
                    }
                }
                
                lastChar = c.ToString();
                strSection += c;

                if (i == str.Length - 1)
                {
                    newStrList.Add(strSection);
                    strSection = "";
                }
            }

            newStr = string.Join("", newStrList);

            //foreach (char c in str)
            //{
            //    if (lastChar != string.Empty && c.ToString() != sep && lastChar[0].ToString() != sep)
            //    {
            //        // Last char was a number, this char is not
            //        if (char.IsNumber(c) && !char.IsNumber(lastChar[0]))
            //        {
            //            newStr += sep;
            //        }
            //        // Last char was not a number, this char is
            //        else if (!char.IsNumber(c) && char.IsNumber(lastChar[0]))
            //        {
            //            newStr += sep;
            //        }
            //    }

            //    newStr += c;
            //    lastChar = c.ToString();
            //}

            while (newStr.Contains(".."))
            {
                newStr = newStr.Replace("..", ".");
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
