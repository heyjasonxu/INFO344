using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace A2
{
    public class TrieNode
    {
        Dictionary<Char, TrieNode> dictionary;
        Char letter;
        bool isWord;
        String term;
        public TrieNode()
        {
            dictionary = new Dictionary<Char, TrieNode>();
            isWord = false;
        }

        public TrieNode(Char letter)
        {
            this.letter = letter;
        }

        public Dictionary<Char, TrieNode> GetDictionary()
        {
            return dictionary;
        }

        public void AddTerm(String term)
        {
            isWord = true;
            this.term = term;
        }

        public void SetLetter(Char letter)
        {
           this.letter = letter;
        }

        public string getTerm()
        {
            return term;
        }

        public bool wordCheck()
        {
            return isWord;
        }
        
    }
}