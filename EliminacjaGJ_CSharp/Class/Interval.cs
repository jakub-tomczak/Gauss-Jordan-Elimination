using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EliminacjaGJ_CSharp.Class
{
    class Interval<T>
    {
        public Interval(T a, T b)
        {
            this.a = a;
            this.b = b;
        }

        T a;
        T b;


        public T A
        {
            get
            {
                return a;
            }

            set
            {
                a = value;
            }
        }

        public T B
        {
            get
            {
                return b;
            }

            set
            {
                b = value;
            }
        }

        public static Interval<T> operator+(Interval<T> a, Interval<T> b)
        {
            return new Interval<T>((dynamic)a.a + b.a, (dynamic)a.b + b.b);
        }

    }
}
