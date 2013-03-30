using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Runtime.Serialization;

namespace NLP.Models
{
    #region Unigram
    [Serializable]
    public class Unigram : Model, IEnumerable
    {
        Dictionary<string, int> freqList;
        int count;

        public Unigram()
        {
            freqList = new Dictionary<string, int>();
            count = 0;
        }

        public void AddWord(string word)
        {
            try
            {
                freqList[word]++;
            }
            catch (Exception)
            {
                freqList.Add(word, 1);
            }
            count++;
        }

        public double Qml(string word)
        {
            try
            {
                return (double)freqList[word] / (double)freqList.Keys.Count;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public bool Contains(string word)
        {
            return freqList.Keys.Contains(word);
        }

        public int this[string word]
        {
            get
            {
                try
                {
                    return freqList[word];
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }

        public int Count
        { get { return count; } }

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            foreach (var item in freqList.Keys)
            {
                yield return new string[] { item }; 
            }
        }

        #endregion

    }
    #endregion

    #region Bigram
    [Serializable]
    public class Bigram : Model, IEnumerable
    {
        Dictionary<string, Unigram> freqList;
        Unigram unigram;
        int count;

        public Bigram()
        {
            freqList = new Dictionary<string, Unigram>();
            unigram = new Unigram();
            count = 0;
        }

        public void AddWord(string word, string w1)
        {
            try
            {
                unigram.AddWord(w1);
                freqList[word].AddWord(w1);
            }
            catch (Exception)
            {
                Unigram tmp = new Unigram();
                tmp.AddWord(w1);
                freqList.Add(word, tmp);
            }
            count++;

        }

        public double Qml(string word, string w1)
        {
            try
            {
                return (double)freqList[word][w1] / (double)unigram[w1];
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public int this[string word, string w1]
        {
            get
            {
                try
                {
                    return freqList[word][w1];
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }

        public int Count
        { get { return freqList.Count; } }

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            foreach (var item in freqList.Keys)
            {
                foreach (string[] w in freqList[item])
                {
                    yield return new string[]{item, w[0]};
                }
            }
        }

        #endregion

    }
    #endregion

    #region Trigram
    [Serializable]
    public class Trigram : Model, IEnumerable
    {
        Dictionary<string, Bigram> freqList;
        Bigram bigram;
        int count;

        public Trigram()
        {
            freqList = new Dictionary<string, Bigram>();
            bigram = new Bigram();
            count = 0;
        }

        public void AddWord(string word, string w1, string w2)
        {
            try
            {
                bigram.AddWord(w1, w2);
                freqList[word].AddWord(w1, w2);
            }
            catch (Exception)
            {
                Bigram tmp = new Bigram();
                tmp.AddWord(w1, w2);
                freqList.Add(word, tmp);
            }
            count++;
        }

        public double Qml(string word, string w1, string w2)
        {
            try
            {
                return (double)freqList[word][w1, w2] / (double)bigram[w1, w2];
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public int this[string word, string w1, string w2]
        {
            get
            {
                try
                {
                    return freqList[word][w1, w2];
                }
                catch (Exception)
                {
                    return 0;
                }
            }
        }

        public int Count
        { get { return count; } }

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            foreach (var item in freqList.Keys)
            {
                foreach (string[] w in freqList[item])
                {
                    yield return new string[]{item, w[0], w[1]};
                }
            }
        }

        #endregion

      
    }
    #endregion

    [Serializable]
    public class Model
    {
 
    }
}
