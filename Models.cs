using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;

namespace NLP.Models
{
    public class Unigram : IEnumerable
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

        public float Qml(string word)
        {
            try
            {
                return (float)freqList[word] / (float)freqList.Keys.Count;
            }
            catch (Exception)
            {
                return 0;
            }
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
    public class Bigram : IEnumerable
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

        public float Qml(string word, string w1)
        {
            try
            {
                return (float)freqList[word][w1] / (float)unigram[w1];
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
    public class Trigram : IEnumerable
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

        public float Qml(string word, string w1, string w2)
        {
            try
            {
                return (float)freqList[word][w1, w2] / (float)bigram[w1, w2];
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

}
