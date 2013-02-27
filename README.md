NLP
===

Natural Language Processing Algorithm and Structures - Lectures Michael Collins on coursera 

Models:
Each model has the frequency list of the words added. 
Trigrams and Bigrams keeps record of the bigrams and unigrams respectively added, in order to be able to return the maximum likehood estimator (Qml) of a given word.
Indexing a model returns the frequency observed in it. 
      ie. "the cat and the dog fought" 
            trigram[the,and,cat] = 1 since the word the is only preceeded once with "cat and"; trigram[w, w-1, w-2] = f
            unigram[the] = 2;
The enumeration process of these models return a string[] with each key in it. 
