using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeurmaClassifier
{

    // Definition: A clean and unambiguous way to present the prediction results of a classifier

    public class ContingencyTable   //calculate accuracy, precision and recall
    {

        private int truePositives;  
        private int falseNegatives; 
        private int falsePositives; 
        private int trueNegatives;  


        public ContingencyTable(int[] predicted, int[] expected, int positiveValue)
        {
            for (int i = 0; i < predicted.Length; i++)
            {
                bool prediction = predicted[i] == positiveValue;
                bool expectation = expected[i] == positiveValue;

                // If true,
                if (expectation == prediction)
                {
                    if (prediction == true)
                    {
                        truePositives++; // Positive
                    }
                    else
                    {
                        trueNegatives++; // Negative
                    }
                }
                else
                {   //if false
                    if (prediction == true)
                    {
                        falsePositives++; // Positive 
                    }
                    else
                    {
                        falseNegatives++; // Negative 
                    }
                }
            }

        }

        public int TruePositives
        {
            get { return truePositives; }
        }

        public int TrueNegatives
        {
            get { return trueNegatives; }
        }

        public int FalsePositives
        {
            get { return falsePositives; }
        }

        public int FalseNegatives
        {
            get { return falseNegatives; }
        }

        private int Observations //total number of observations 
        {
            get
            {
                return trueNegatives + truePositives +
                    falseNegatives + falsePositives;
            }
        }

        public double Accuracy  //raw performance of the system
        {
            get { return 1.0 * (truePositives + trueNegatives) / Observations; }
        }

        public double Precision
        {
            get
            {
                double f = truePositives + FalsePositives;

                if (f != 0)
                    return truePositives / f;

                return 1.0;
            }
        }

        public double Recall
        {
            get
            {
                return (truePositives == 0) ?
                    0 : (double)truePositives / (truePositives + falseNegatives);
            }
        }

        public double FRatio    //harmonic mean of precision and recall
        {
            get { return 2.0 * (Precision * Recall) / (Precision + Recall); }
        }

    }
}
