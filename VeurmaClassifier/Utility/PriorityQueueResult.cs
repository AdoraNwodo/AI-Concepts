using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veurma.Utility
{
    public class PriorityQueueResult
    {
        private object node;
        private IComparable priority;

        public object Node
        {
            get { return node; }
            set { node = value; }
        }

        public IComparable Priority
        {
            get { return priority; }
            set { priority = value; }
        }
    }
}
