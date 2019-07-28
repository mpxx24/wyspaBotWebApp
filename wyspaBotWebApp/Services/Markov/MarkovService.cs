using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MarkovSharp.TokenisationStrategies;
using NLog;

namespace wyspaBotWebApp.Services.Markov {
    public class MarkovService : IMarkovService {
        private readonly ILogger logger = LogManager.GetCurrentClassLogger();

        private readonly string markovSourceFilePath;

        private StringMarkov stringMarkov;

        private readonly int level;

        public MarkovService(string markovSourceFilePath, int level) {
            this.markovSourceFilePath = markovSourceFilePath;
            this.level = level;
            this.Initialize(this.level);
        }

        public void Learn(string sentence) {
            this.stringMarkov.Learn(sentence);
        }

        public string GetText() {
            var buffer = new List<string>();
            while (!buffer.Any())
            {
                for (var i = 0; i < 20; i++)
                {
                    buffer.Add(this.stringMarkov.Walk().First());
                }

                //temp solution to not-wanted strings
                buffer.RemoveAll(x => x.Contains("@gateway") || x.Contains("@freenode") || x.Contains("https://") || x.Contains("http://") || x.Contains(":Ping timeout:"));
            };

            return buffer.OrderByDescending(x => x.Length).FirstOrDefault();
        }

        public void PersistMarkovObject() {
            var bufforFileName = $"{Path.GetDirectoryName(this.markovSourceFilePath)}\\{Path.GetFileNameWithoutExtension(this.markovSourceFilePath)}2.{Path.GetExtension(this.markovSourceFilePath)}";

            try {
                this.logger.Debug("Trying to persist string markov state!");
                if (File.Exists(this.markovSourceFilePath)) {
                    this.logger.Debug("Creating initial source backup!");
                    File.Copy(this.markovSourceFilePath, bufforFileName);
                }

                using (var sw = new StreamWriter(this.markovSourceFilePath, false)) {
                    foreach (var sourceLine in this.stringMarkov.SourcePhrases) {
                        sw.WriteLine(sourceLine);
                    }
                }

                this.logger.Debug("Successfully persisted string markov state.");

                this.logger.Debug("Removing initial source backup.");
                File.Delete(bufforFileName);
            }
            catch (Exception e) {
                this.logger.Debug($"Failed to persist string markov state! {e}");
                if (File.Exists(bufforFileName) && File.Exists(this.markovSourceFilePath)) {
                    this.logger.Debug("Rollback initial source file.");
                    File.Delete(this.markovSourceFilePath);
                    File.Copy(bufforFileName, this.markovSourceFilePath);
                    File.Delete(bufforFileName);
                }
                throw e;
            }
        }

        public IEnumerable<string> GetMostUsedWords() {
            var stats = new Dictionary<string, int>();
            var result = new List<string>();

            this.logger.Debug($"Getting saved data from file. '{this.markovSourceFilePath}'");

            if (!File.Exists(this.markovSourceFilePath)) {
                this.logger.Debug($"Failed to get data from file! File '{this.markovSourceFilePath}' doesn't exist");
                return null;
            }

            var allData = File.ReadAllText(this.markovSourceFilePath).Split(' ').Where(x => x.Length > 3).ToList();

            foreach (var word in allData) {
                if (stats.ContainsKey(word)) {
                    stats[word] = stats[word] + 1;
                }
                else {
                    stats.Add(word, 1);
                }
            }

            foreach (var stat in stats.OrderByDescending(x => x.Value)) {
                result.Add($"({stat.Value}) {stat.Key}");    
            }
            
            return new List<string>{$"Total number of words ({result.Count})", "Most used words:", $"{string.Join(", ", result.Take(15))}"};
        }

        public string GetPathToSourceFile() {
            return this.markovSourceFilePath;
        }

        private void Initialize(int markovLevel = 2) {
            this.logger.Debug($"Initializing string markov. Level: {markovLevel}");
            this.stringMarkov = new StringMarkov(markovLevel);

            this.LearnInitialData();
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