using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeurmaClassifier
{
    public enum LearningAlgorithm
    {
        C45Learning,
        ID3Learning
    }

    public static class LA
    {
        public static string ToString(LearningAlgorithm la)
        {
            switch (la)
            {
                case LearningAlgorithm.C45Learning:
                    return "C4.5 Learning";
                case LearningAlgorithm.ID3Learning:
                    return "ID3 Learning";
                default:
                    return "undefined";                    

            }
        }
    }
}
