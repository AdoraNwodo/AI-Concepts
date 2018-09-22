using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeurmaClassifier
{
    public class AttributeListInfo
    {
        //Information about Attributes

        private int numberOfInputs;
        private List<string> inputs;

        public int NumberOfInputs
        {
            get { return numberOfInputs; }
            set { numberOfInputs = value; }
        }

        public List<string> Inputs
        {
            get { return inputs; }
            set { inputs = value; }
        }
    }
}
