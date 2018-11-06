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

        private void LearnInitialData() {
            try {
                var path = Path.Combine(this.markovSourceFilePath, @"markov initial source.txt");

                this.logger.Debug($"Passing initial data for markov model to learn. {path}");

                if (!File.Exists(path)) {
                    this.logger.Debug($"Failed to pass initial data to string markov model! File {path} doesn't exist");
                    return;
                }

                var file = File.ReadAllLines(path);
                foreach (var s in file) {
                    this.Learn(s);
                }
            }
            catch (Exception e) {
                this.logger.Debug(e, "Exception occured while passing initial data for markov model to learn.");
                throw e;
            }
        }
    }
}