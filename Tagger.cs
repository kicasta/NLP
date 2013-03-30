using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLP.Models;

namespace NLP
{
   
    public class Tagger
    {
        public static List<string> Viterbi(
            List<string> str,
            Trigram q,
            Bigram e,
            Unigram freqList,
            HashSet<string> tags)
        {
            return Viterbi(str,q,e,freqList,tags,false);
        }

        
        public static List<string> Viterbi(
           List<string> str,
           Trigram q,
           Bigram e,
           Unigram freqList,
           HashSet<string> tags,
           bool splitRare)
        {
            List<string> y = new List<string>();
            Dictionary<string, double> current = new Dictionary<string, double>();
            Dictionary<string, double> previous = new Dictionary<string, double>();
            string key;
            double prob = 0;
            double max = 0;

            if (freqList.Contains(str[0]))
            {
                y.Add("STOP");

                foreach (string v in tags)
                {
                    prob = q.Qml(v, "*", "*") * e.Qml(str[0], v);
                    current.Add("*:" + v, prob);

                    if (prob >= max)
                    {
                        max = prob;
                        y.RemoveAt(y.Count - 1);
                        y.Add(v);
                    }
                }
            }
            else
            {
                string v = (splitRare) ? ProperRare(str[0]) : "_RARE_";
                y.Add(v);

                prob = Math.Max(q.Qml("_RARE_", "*", "*"), 0);
                current.Add("*:" + "_RARE_", prob);

            }

            previous = current;
            current = new Dictionary<string, double>();
            max = 0;
            if (freqList.Contains(str[1]))
            {
                y.Add("STOP");

                foreach (string u in tags)
                {
                    foreach (string v in tags)
                    {
                        try
                        {
                            key = "*:" + u;
                            prob = previous[key] * q.Qml(v, "*", u) * e.Qml(str[1], v);
                            current.Add(string.Format("{0}:{1}", u, v), prob);
                            if (prob >= max)
                            {
                                max = prob;
                                y.RemoveAt(y.Count - 1);
                                y.Add(v);
                            }
                        }
                        catch (Exception)
                        {
                            continue;

                        }
                    }
                }
            }
            else
            {
                string v = (splitRare) ? ProperRare(str[1]) : "_RARE_";
                y.Add(v);

                foreach (string u in tags)
                {
                    try
                    {
                        key = "*:" + u;
                        prob = previous[key] * q.Qml(v, "*", u) * e.Qml(str[1], v);
                        current.Add(string.Format("{0}:{1}", u, v), prob);
                        if (prob >= max)
                        {
                            max = prob;
                            y.RemoveAt(y.Count - 1);
                            y.Add(v);
                        }
                    }
                    catch (Exception)
                    {
                        continue;

                    }
                }
            }


            for (int i = 2; i < str.Count; i++)
            {
                previous = current;
                current = new Dictionary<string, double>();
                max = 0;

                if (freqList.Contains(str[i]))
                {
                    y.Add("STOP");

                    foreach (string v in tags)
                    {
                        foreach (string u in tags)
                        {
                            foreach (string w in tags)
                            {
                                key = w + ":" + u;

                                try
                                {
                                    prob = previous[key] * q.Qml(v, w, u) * e.Qml(str[i], v);
                                }
                                catch (Exception)
                                {

                                    prob = 0;
                                }


                                try
                                {
                                    if (prob > 0)
                                        current.Add(string.Format("{0}:{1}", u, v), prob);

                                }
                                catch (Exception)
                                {
                                    if (prob > current[string.Format("{0}:{1}", u, v)])
                                    {
                                        current.Remove(string.Format("{0}:{1}", u, v));
                                        current.Add(string.Format("{0}:{1}", u, v), prob);
                                    }
                                }

                                if (prob >= max)
                                {
                                    max = prob;
                                    y.RemoveAt(y.Count - 1);
                                    y.Add(v);
                                }

                            }
                        }
                    }
                }
                else
                {
                    string v = (splitRare) ? ProperRare(str[i]) : "_RARE_";
                    y.Add(v);

                    foreach (string u in tags)
                    {
                        foreach (string w in tags)
                        {
                            key = w + ":" + u;

                            try
                            {
                                prob = previous[key] * q.Qml(v, w, u);
                            }
                            catch (Exception)
                            {
                                prob = 0;
                            }


                            try
                            {
                                if (prob > 0)
                                    current.Add(string.Format("{0}:{1}", u, v), prob);

                            }
                            catch (Exception)
                            {
                                if (prob > current[string.Format("{0}:{1}", u, v)])
                                {
                                    current.Remove(string.Format("{0}:{1}", u, v));
                                    current.Add(string.Format("{0}:{1}", u, v), prob);
                                }
                            }

                            if (prob >= max)
                            {
                                max = prob;
                                y.RemoveAt(y.Count - 1);
                                y.Add(v);
                            }

                        }
                    }

                }
            }

            return y;
        }


        static string ProperRare(string word)
        {
            for (int i = 0; i < word.Length; i++)
                if (Char.IsDigit(word[i]))
                    return "NUMERIC";

            if (word == word.ToUpper())
                return "ALL_CAPITALS";

            if (word[word.Length - 1] == Char.ToUpper(word[word.Length - 1]))
                return "LAST_CAPITAL";

            return "_RARE_";
        }


        public static List<string> EMTagger(List<string> sentence, HashSet<string> tags, Bigram emissionProb)
        {
            double prob = 0;
            double valMax = 0;

            List<string> argmax = new List<string>();

            foreach (var word in sentence)
            {
                valMax = 0;
                argmax.Add("STOP");

                foreach (var tag in tags)
                {
                    prob = emissionProb.Qml(word, tag);
                    
                    if (prob >= valMax)
                    {
                        valMax = prob;
                        argmax.RemoveAt(argmax.Count - 1);
                        argmax.Add(tag);
                    }
                }
            }
            return argmax;
        }
      
    }
}
