namespace wyspaBotWebApp.Services.Markov {
    public interface IMarkovService {
        void Initialize(int level = 2);

        void Learn(string sentence);

        string GetText();
    }
}