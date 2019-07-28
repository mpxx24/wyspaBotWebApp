using System.Collections.Generic;

namespace wyspaBotWebApp.Services.Markov {
    public interface IMarkovService {
        void Learn(string sentence);

        string GetText();

        void PersistMarkovObject();

        IEnumerable<string> GetMostUsedWords();
        string GetPathToSourceFile();
    }
}