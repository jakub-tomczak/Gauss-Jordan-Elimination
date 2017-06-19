using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EliminacjaGJ_CSharp.Class
{
    class Error<T>
    {
        T min;
        T max;
        T mean;
        T variance;
        T standVar;

        public T Min
        {
            get
            {
                return min;
            }
        }
    }
}
