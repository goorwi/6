using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace _6
{
    // движемся с права на лево при праволинейной сначала единичный на Z и движемся пока не найдём S
    public class Turing2
    {
        private string word;
        private List<Tuple<string, List<string>>> grammatic = new List<Tuple<string, List<string>>>();
        private List<Tuple<string, string>> rules = new List<Tuple<string, string>>();
        private int direction;
        private List<Tuple<string, Tuple<string, string>>> steps = new List<Tuple<string, Tuple<string, string>>>();

        public Turing2()
        {
            ReadGrammatic();
            RecognizeGrammatic();
            GetRules();
            ReadWord();
            Console.WriteLine($"Входная строка: {word}");
            var result = Work(word, true);
            steps.Reverse();
            for (int i = 0; i < steps.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {steps[i].Item1}: {steps[i].Item2.Item1} -> {steps[i].Item2.Item2}");
            }
            Console.WriteLine(result);
        }

        private void ReadGrammatic()
        {
            using (var sr = new StreamReader("grammatic.txt"))
            {
                while (!sr.EndOfStream)
                {
                    var str = sr.ReadLine().Split(' ');

                    var was = false;
                    string from = "";
                    List<string> to = new List<string>();
                    foreach (var s in str)
                    {
                        if (s == "->")
                        {
                            was = true;
                            continue;
                        }
                        if (was)
                        {
                            if (s != "|")
                                to.Add(s);
                        }
                        else
                            from = s;
                    }

                    grammatic.Add(Tuple.Create(from, to));
                }
            }
        }

        private void ReadWord()
        {
            using (var sr = new StreamReader("word.txt"))
            {
                word = sr.ReadLine();
            }
        }

        // Праволинейная - 1, Леволинейная - 2
        private void RecognizeGrammatic()
        {
            foreach (var rule in grammatic)
            {
                foreach (var r in rule.Item2)
                {
                    if (Regex.IsMatch(r, @"([a-z][A-Z]){1}"))
                    {
                        direction = 1;
                        return;
                    }
                }
            }
            direction = 2;
        }

        private void GetRules()
        {
            if (direction == 1)
            {
                foreach (var rule in grammatic)
                {
                    foreach (var r in rule.Item2)
                    {
                        var from = r;
                        var to = "";

                        if (Regex.IsMatch(from, @"[a-z]{1}\b"))
                        {
                            to = "Z";
                            from = rule.Item1 + from;
                        }
                        else
                        {
                            to = Regex.Match(from, @"[A-Z]").Value;
                            from = rule.Item1 + Regex.Match(from, @"[a-z]").Value;
                        }
                        rules.Add(Tuple.Create(from, to));
                    }
                }
                rules.Add(Tuple.Create("Z#", "#"));
            }
            else if (direction == 2)
            {
                foreach (var rule in grammatic)
                {
                    foreach (var r in rule.Item2)
                    {
                        var from = r;
                        var to = "";

                        if (Regex.IsMatch(from, @"\b[a-z]{1}"))
                        {
                            to = "Z";
                            from = from + rule.Item1;
                        }
                        else
                        {
                            to = Regex.Match(from, @"[A-Z]").Value;
                            from = Regex.Match(from, @"[a-z]").Value + rule.Item1;
                        }
                        rules.Add(Tuple.Create(from, to));
                    }
                }
                rules.Add(Tuple.Create("#Z", "#"));
            }
        }

        private string Work(string currentWord, bool isStart)
        {
            int a = 0, b = 0;
            // a - стартовый индекс, b - количество символов.
            if (direction == 1)
            {
                a = 0; b = 2;
            }
            else if (direction == 2)
            {
                a = currentWord.Length - 2; b = 2;
            }

            if (currentWord == "#")
                return "Допуск";
            else if (isStart)
            {
                if (direction == 1)
                {
                    currentWord = currentWord.Insert(0, "S");
                }
                else if (direction == 2)
                {
                    currentWord = currentWord.Insert(currentWord.Length, "S");
                    a += 1;
                }

                var part = currentWord.Substring(a, b);
                var newPart = rules.Where(x => x.Item1 == part).ToList();
                foreach (var x in newPart)
                {
                    if (Work(currentWord.Replace(x.Item1, x.Item2), false) == "Допуск")
                    {
                        steps.Add(Tuple.Create(currentWord, x));
                        return "Допуск";
                    }
                }
            }
            else
            {
                var part = currentWord.Substring(a, b);
                var newPart = rules.Where(x => x.Item1 == part).ToList();
                foreach (var x in newPart)
                {
                    if (Work(currentWord.Replace(x.Item1, x.Item2), false) == "Допуск")
                    {
                        steps.Add(Tuple.Create(currentWord, x));
                        return "Допуск";
                    }
                }
            }
            return "Недопуск";
        }
    }
}
