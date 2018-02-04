using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace A2
{
    public class TrieTree
    {
        TrieNode root;
        public TrieTree()
        {
            root = new TrieNode();
        }

        public void insertWord(String term)
        {
            char[] letters = term.ToLower().ToCharArray();
            List<Char> letList = CharArraryToList(letters);
            insertWord(letList, term, root);
        }

        private void insertWord(List<Char> subChar, String term, TrieNode root)
        {
            //string fTerm = term.ToLower();
            //char[] letters = subTerm.ToCharArray();
            //char firstLetter = letters[0];
            Dictionary<Char, TrieNode> t = root.GetDictionary();
            if (subChar.Count() == 1)
            {
                if (!t.ContainsKey(subChar[0]))
                {
                    t.Add(subChar[0], new TrieNode());
                    t[subChar[0]].AddTerm(term);
                }
                else
                {
                    t[subChar[0]].AddTerm(term);
                }
            }
            else
            {
                if (!t.ContainsKey(subChar[0]))
                {
                    TrieNode n = new TrieNode();
                    t.Add(subChar[0], n);
                }
                char temp = subChar[0];
                subChar.RemoveAt(0);
                insertWord(subChar, term, t[temp]);
            }
        }

        public List<String> GetResults(String term)
        {
            List<Char> list = CharArraryToList(term.ToCharArray());
            TrieNode node = getResultRoot(list, root);
            return getResults(new List<String>(), node);
        }

        private List<String> getResults(List<String> list, TrieNode root)
        {
            if (root == null)
            {
                return list;
            } else if (list.Count == 10) {
                return list;
            }
            else
            {
                if (root.wordCheck())
                {
                    list.Add(root.getTerm());
                }
                Dictionary<Char, TrieNode> t = root.GetDictionary();
                List<Char> n = new List<Char>(t.Keys);
                foreach (Char c in n)
                {
                    getResults(list, t[c]);
                }
                return list;
            }
        }

        private TrieNode getResultRoot(List<Char> subChar, TrieNode root)
        {
            Dictionary<Char, TrieNode> t = root.GetDictionary();
            if (subChar.Count == 0)
            {
                return root;
            }
            else if (!t.ContainsKey(subChar[0]))
            {
                return null;
            }
            else
            {
                char temp = subChar[0];
                subChar.RemoveAt(0);
                return getResultRoot(subChar, t[temp]);
            }
        }

        private List<Char> CharArraryToList(Char[] chars)
        {
            List<Char> list = new List<char>();
            foreach(Char c in chars)
            {
                list.Add(c);
            }
            return list;
        } 

    }
}