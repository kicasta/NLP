using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLP.Models;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace NLP
{
    /// <summary>
    /// Train Hidden Markov Models
    /// Tag Data using Viterbi Algorithm
    /// </summary>
    class HMM
    {

        #region vars
        static Trigram transition = new Trigram();
        static Bigram emission = new Bigram();
        static Unigram freqList = new Unigram();
        static HashSet<string> tags = new HashSet<string>();

        static string trainingPath = "gene.train";
        static string inputPath = "gene.test";
        static string trigramPath = "transition.tgm";
        static string bigramPath = "emission.bgm";
        static string unigramPath = "freqList.ugm";
        static string tagsPath = "tags.set";
        #endregion

        public static void Tag()
        {
            Tag(false);
        }

        public static void Tag(bool EMtag)
        {
            List<string> tagSequence = new List<string>();
            StreamWriter sw1 = new StreamWriter("gene_test.p1.out");
            StreamWriter sw2 = new StreamWriter("gene_test.p3.out");

            foreach (List<string> sentence in ParseInputData(inputPath))
            {
                if (EMtag)
                {
                    tagSequence.Clear();

                    tagSequence = Tagger.EMTagger(sentence, tags, emission);

                    for (int i = 0; i < sentence.Count; i++)
                    {
                        sw1.WriteLine(sentence[i] + " " + tagSequence[i]);
                    }

                    sw1.WriteLine();
                }

                tagSequence.Clear();

                tagSequence = Tagger.Viterbi(sentence, transition, emission, freqList, tags, true);

                for (int i = 0; i < sentence.Count; i++)
                {
                    sw2.WriteLine(sentence[i] + " " + tagSequence[i]);
                }

                sw2.WriteLine();
            }

            sw1.Close();
            sw2.Close();

        }

        public static void Train()
        {
            if (!(File.Exists(trigramPath) && File.Exists(bigramPath) && File.Exists(tagsPath)))
            {
                if (File.Exists("tmp.train"))
                    ParseTrainingData("tmp.train");
                else
                    ParseTrainingData(ReplaceTrainingFile(trainingPath, BuildFreqList(trainingPath), true));

                SerializeModel(trigramPath, transition);
                SerializeModel(bigramPath, emission);
                SerializeModel(unigramPath, freqList);
                SerializeTags(tagsPath);
            }
            else
            {
                transition = (Trigram)DeserializeModel(trigramPath);
                emission = (Bigram)DeserializeModel(bigramPath);
                freqList = (Unigram)DeserializeModel(unigramPath);
                tags = DeserializeTags(tagsPath);
            }
        }

        static void SerializeModel(string path, Model model)
        {
            Stream stream = File.Open(path, FileMode.Create);
            BinaryFormatter bformatter = new BinaryFormatter();
            bformatter.Serialize(stream, model);
            stream.Close();
        }

        static Model DeserializeModel(string path)
        {
            Model model = null;
            Stream stream = File.Open(path, FileMode.Open);
            BinaryFormatter bformatter = new BinaryFormatter();

            model = (Model)bformatter.Deserialize(stream);
            stream.Close();

            return model;

        }

        static void SerializeTags(string path)
        {
            Stream stream = File.Open(path, FileMode.Create);
            BinaryFormatter bformatter = new BinaryFormatter();
            bformatter.Serialize(stream, tags);
            stream.Close();
        }

        static HashSet<string> DeserializeTags(string path)
        {
            HashSet<string> tagSet = null;
            Stream stream = File.Open(path, FileMode.Open);
            BinaryFormatter bformatter = new BinaryFormatter();

            tagSet = (HashSet<string>)bformatter.Deserialize(stream);
            stream.Close();

            return tagSet;

        }

        static Unigram BuildFreqList(string path)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line != "")
                        freqList.AddWord(line.Split(' ')[0]);
                }
            }

            return freqList;
        }

        static string ReplaceTrainingFile(string path, Unigram freqList)
        {
            return ReplaceTrainingFile(path, freqList, false);

        }

        static string ReplaceTrainingFile(string path, Unigram freqList, bool splitRares)
        {

            //FileStream fs = File.Open(path, FileMode.Open, FileAccess.ReadWrite);

            string outpath = "tmpSP.train";

            StreamReader sr = new StreamReader(path);
            StreamWriter sw = new StreamWriter(outpath);

            string line;
            string[] str;

            while ((line = sr.ReadLine()) != null)
            {
                if (line != "")
                {
                    str = line.Split(' ');

                    if (freqList[str[0]] < 5)
                        sw.WriteLine(str[0] + ((splitRares) ? SplitRareTag(str[0]) : " _RARE_"));
                    else
                        sw.WriteLine(line);
                }
                else
                    sw.WriteLine(line);
            }

            sr.Close();
            sw.Close();
            //fs.Close();

            //File.Delete(path);
            //File.Move("tmp.train", path);

            return outpath;

        }

        static string SplitRareTag(string word)
        {
            for (int i = 0; i < word.Length; i++)
                if (Char.IsDigit(word[i]))
                    return " NUMERIC";

            if (word == word.ToUpper())
                return " ALL_CAPITALS";

            if (word[word.Length - 1] == Char.ToUpper(word[word.Length - 1]))
                return " LAST_CAPITAL";

            return " _RARE_";
        }

        static void ParseTrainingData(string path)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                string prev1 = "*";
                string prev2 = "*";
                string line;

                string[] str;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line != "")
                    {
                        str = line.Split(' ');

                        transition.AddWord(str[1], prev2, prev1);
                        emission.AddWord(str[0], str[1]);
                        tags.Add(str[1]);

                        prev2 = prev1;
                        prev1 = str[1];
                    }
                    else
                    {
                        transition.AddWord("STOP", prev2, prev1);
                        prev1 = "*";
                        prev2 = "*";

                    }
                }

            }
        }

        static IEnumerable ParseInputData(string path)
        {
            List<string> current = new List<string>();

            using (StreamReader sr = new StreamReader(path))
            {
                string line = "";
                while ((line = sr.ReadLine()) != null)
                {
                    if (line != "")
                        current.Add(line);
                    else
                    {
                        yield return current;
                        current.Clear();
                    }
                }
            }
        }

    }
}
