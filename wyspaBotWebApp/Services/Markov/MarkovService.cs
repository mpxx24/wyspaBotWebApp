using System;
using System.IO;
using System.Linq;
using MarkovSharp.TokenisationStrategies;
using NLog;

namespace wyspaBotWebApp.Services.Markov {
    public class MarkovService : IMarkovService {
        private readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private readonly string markovSourceFilePath;

        private StringMarkov stringMarkov;

        public MarkovService(string markovSourceFilePath) {
            this.markovSourceFilePath = markovSourceFilePath;
        }

        public void Initialize(int level = 2) {
            this.logger.Debug($"Initializing string markov. Level: {level}");
            this.stringMarkov = new StringMarkov(level);

            this.LearnInitialData();
        }

        public void Learn(string sentence) {
            this.stringMarkov.Learn(sentence);
        }

        public string GetText() {
            return this.stringMarkov.Walk().First();
        }

        public void PersistMarkovObject() {
            try {
                this.logger.Debug("Trying to persist string markov state!");

                using (var sw = new StreamWriter(this.markovSourceFilePath, false)) {
                    this.logger.Debug($"{this.stringMarkov.SourcePhrases.Count} elements!");
                    foreach (var sourceLine in this.stringMarkov.SourcePhrases) {
                        sw.WriteLine(sourceLine);
                    }
                }
                
                this.logger.Debug("Successfully persisted string markov state.");
            }
            catch (Exception e) {
                this.logger.Debug($"Failed to persist string markov state! {e}");
                throw e;
            }
        }

        private void LearnInitialData() {
            try {
                this.logger.Debug($"Passing initial data for markov model to learn. '{this.markovSourceFilePath}'");

                if (!File.Exists(this.markovSourceFilePath)) {
                    this.logger.Debug($"Failed to pass initial data to string markov model! File '{this.markovSourceFilePath}' doesn't exist");
                    return;
                }

                var file = File.ReadAllLines(this.markovSourceFilePath);

                this.logger.Debug($"Initial source contains {file.Length} items!");

                foreach (var s in file) {
                    this.Learn(s);
                }
            }
            catch (Exception e) {
                this.logger.Debug($"Exception occured while passing initial data for markov model to learn. {e}");
                throw e;
            }
        }
    }
}