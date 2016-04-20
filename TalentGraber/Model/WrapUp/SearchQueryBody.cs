using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Macrodeek.HappyZLModel.WrapUp.SearchRelated
{
    public class QueryString
    {
        public string DefaultField { get; set; }
        public string Operation { get; set; }
        public string Query { get; set; }
    }

    public enum ObjectType
    {
        TalentCandidate
    }

    public class SearchQueryBody
    {
        public SearchQueryBody()
        {
            From = 0;
            Size = 10;
            MinimumShouldMatch = "100%";
        }
        public ObjectType ObjectType { get; set; }
        public string FullText { get; set; }
        public string MinimumShouldMatch { get; set; }
        public int From { get; set; }
        public int Size { get; set; }
        public List<QueryString> QueryStrings { get; set; }

    }
}
