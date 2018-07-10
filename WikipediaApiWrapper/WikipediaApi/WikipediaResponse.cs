using System.Collections.Generic;
using Newtonsoft.Json;

namespace WikipediaApi {

    public class Normalized {
        [JsonProperty(PropertyName = "from")]
        public string From { get; set; }
        [JsonProperty(PropertyName = "to")]
        public string To { get; set; }
    }

    public class Revision {
        [JsonProperty(PropertyName = "contentformat")]
        public string ContentFormat { get; set; }
        [JsonProperty(PropertyName = "contentmodel")]
        public string ContentModel { get; set; }
        [JsonProperty(PropertyName = "*")]
        public string Content { get; set; }
    }

    public class ArticleInfo {
        [JsonProperty(PropertyName = "pageid")]
        public int Pageid { get; set; }
        [JsonProperty(PropertyName = "ns")]
        public int Ns { get; set; }
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }
        [JsonProperty(PropertyName = "revisions")]
        public IList<Revision> Revisions { get; set; }
    }

    public class Query {
        [JsonProperty(PropertyName = "normalized")]
        public IList<Normalized> Normalized { get; set; }
        [JsonProperty(PropertyName = "pages")]
        public Dictionary<object, object> Pages { get; set; }
        public ArticleInfo ArticleInfo { get; set; }
    }

    public class RootObject {
        [JsonProperty(PropertyName = "batchcomplete")]
        public string BatchComplete { get; set; }
        [JsonProperty(PropertyName = "query")]
        public Query Query { get; set; }
    }

}