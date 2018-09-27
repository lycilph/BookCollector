using NHunspell;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Panda.UI.AvalonEdit
{
    // https://github.com/martinkirsche/AsYouTypeSpellChecker
    public class SpellChecker
    {
        private static Lazy<SpellChecker> defaultInstance = new Lazy<SpellChecker>(() => new SpellChecker());
        public static SpellChecker Default { get { return defaultInstance.Value; } }

        public Hunspell HunspellInstance { get; set; }

        public class Word
        {
            public int Index { get; set; }
            public string Value { get; set; }
        }

        public IEnumerable<Word> FindWords(string text)
        {
            foreach (Match m in new Regex(@"\w+").Matches(text))
            {
                yield return new Word() { Index = m.Index, Value = m.Value };
            }
        }

        public IEnumerable<Word> FindSpellingErrors(string text)
        {
            foreach (var word in FindWords(text))
            {
                if (!Spell(word.Value))
                {
                    yield return word;
                }
            }
        }

        public bool Spell(string word)
        {
            return HunspellInstance.Spell(word);
        }

        public List<string> Suggest(string word)
        {
            return HunspellInstance.Suggest(word);
        }
    }
}
