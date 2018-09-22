using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeurmaClassifier
{
    public class Query
    {
        private string queryName;
        private List<string> options;

        public Query(string _queryName, List<string> _options)
        {
            queryName = _queryName;
            options = _options;
        }

        public string QueryName
        {
            get { return queryName; }
        }

        public List<string> Options
        {
            get { return options; }
        }

    }
}
