using System;
using System.Collections.Generic;
using System.Globalization;

namespace WikipediaApi.Helpers {
    public static class WikiApiHelper {
        /// <summary>
        ///     Prepares the word for searching.
        /// </summary>
        /// <param name="wordToClear">The word to clear.</param>
        /// <returns>properly formated word</returns>
        public static List<string> PrepareWordForSearching(string wordToClear) {
            var possibleCombinations = new List<string> {
                wordToClear,
                CultureInfo.CurrentCulture.TextInfo.ToTitleCase(wordToClear.ToLower()),
                CultureInfo.CurrentCulture.TextInfo.ToTitleCase(wordToClear.Replace(' ', '_').ToLower()),
                CultureInfo.CurrentCulture.TextInfo.ToLower(wordToClear)
            };

            return possibleCombinations;
        }

        /// <summary>
        ///     Clears the article.
        /// </summary>
        /// <returns>article without e.g. [[ ]] or <ref></ref> </returns>
        public static string ClearAndPrepareArticle(string article, WikiLanguage language) {
            var result = RemoveRef(article);
            result = RemoveSquareBracketsAndLines(result);
            result = RemoveNbsp(result);
            result = RemoveApostrophe(result);
            result = RemoveDoubleCurlyBrackets(result);
            result = AddArticleAdress(result, language);

            return result;
        }

        /// <summary>
        ///     Removes the ref tags from article and text between those tags.
        /// </summary>
        /// <param name="article">The article.</param>
        /// <returns>article without <ref></ref> in it</returns>
        private static string RemoveRef(string article) {
            if (article.Contains("<ref>") && article.Contains("</ref>")) {
                var indexFrom = 0;
                var indexTo = 0;
                while (indexFrom != -1 && indexTo != -1 && indexTo < article.Length) {
                    indexFrom = article.IndexOf("<ref>", indexFrom + "<ref>".Length, StringComparison.Ordinal);
                    indexTo = article.IndexOf("</ref>", indexTo, StringComparison.Ordinal);
                    var res = indexFrom != -1 && indexTo != -1
                        ? indexTo > indexFrom
                            ? article.Substring(indexFrom, indexTo - indexFrom)
                            : string.Empty
                        : string.Empty;

                    if (!string.IsNullOrEmpty(res)) {
                        article = article.Replace(res, "");
                    }
                }

                return article.Replace("<ref>", string.Empty).Replace("</ref>", string.Empty);
            }

            return article;
        }

        /// <summary>
        ///     Removes [[ | ]] from the article and text between [[ |
        /// </summary>
        /// <param name="article">The article.</param>
        /// <returns>article without [[ | ]]</returns>
        private static string RemoveSquareBracketsAndLines(string article) {
            if (article.Contains("[[") && article.Contains("]]")) {
                var indexFrom = 0;
                var indexToLine = 0;
                var indexToSquareBrackets = 0;

                while (indexFrom != -1 && indexToLine < article.Length && indexToSquareBrackets < article.Length) {
                    indexFrom = article.IndexOf("[[", indexFrom == -1 ? 0 : indexFrom + "[[".Length,
                                                StringComparison.Ordinal);
                    indexToLine = article.IndexOf("|", indexFrom == -1 ? 0 : indexFrom, StringComparison.Ordinal);
                    indexToSquareBrackets = article.IndexOf("]]", indexFrom == -1 ? 0 : indexFrom,
                                                            StringComparison.Ordinal);

                    var res = indexToLine < indexToSquareBrackets && indexToLine - indexFrom > 0 && indexFrom != -1
                        ? article.Substring(indexFrom, indexToLine - indexFrom)
                        : string.Empty;

                    if (!string.IsNullOrEmpty(res)) {
                        article = article.Replace(res, "");
                    }
                }

                return article.Replace("[[", string.Empty).Replace("|", string.Empty).Replace("]]", string.Empty);
            }

            return article;
        }

        /// <summary>
        ///     Removes nbsp from article
        /// </summary>
        /// <param name="article"></param>
        /// <returns>article without nbsp</returns>
        private static string RemoveNbsp(string article) {
            return article.Replace("&nbsp;", " ");
        }

        /// <summary>
        ///     Removes '' from article
        /// </summary>
        /// <param name="article"></param>
        /// <returns>article without ''</returns>
        private static string RemoveApostrophe(string article) {
            return article.Replace("''", string.Empty);
        }

        private static string RemoveDoubleCurlyBrackets(string article) {
            if (article.Contains("{{") && article.Contains("}}")) {
                var indexFrom = 0;
                var indexTo = 0;
                while (indexFrom != -1 && indexTo != -1 && indexTo < article.Length) {
                    indexFrom = article.IndexOf("{{", indexFrom + "{{".Length, StringComparison.Ordinal);
                    indexTo = article.IndexOf("}}", indexFrom, StringComparison.Ordinal);
                    var res = indexFrom != -1 && indexTo != -1
                        ? article.Substring(indexFrom, indexTo - indexFrom)
                        : string.Empty;

                    if (!string.IsNullOrEmpty(res)) {
                        article = res.Contains("IPA") ? article.Replace(res, "?") : article.Replace(res, " ");
                    }
                }

                return article.Replace("{{", string.Empty).Replace("}}", string.Empty);
            }

            return article;
        }

        /// <summary>
        ///     Adds link to Wikipedia page for article
        /// </summary>
        /// <param name="article"></param>
        /// <param name="language"></param>
        /// <returns>article with link</returns>
        private static string AddArticleAdress(string article, WikiLanguage language) {
            string wikipediaArticleAddress;
            switch (language) {
                case WikiLanguage.Polish:
                    wikipediaArticleAddress = $"Aby dowiedzieć się więcej:{Environment.NewLine}https://pl.wikipedia.org/wiki/{WikiApi.Word}";
                    break;
                case WikiLanguage.English:
                    wikipediaArticleAddress = $"To know more: {Environment.NewLine} https://en.wikipedia.org/wiki/{WikiApi.Word}";
                    break;
                default:
                    wikipediaArticleAddress = $"Aby dowiedzieć się więcej:{Environment.NewLine}https://pl.wikipedia.org/wiki/{WikiApi.Word}";
                    break;
            }
            return article + wikipediaArticleAddress;
        }
    }
}