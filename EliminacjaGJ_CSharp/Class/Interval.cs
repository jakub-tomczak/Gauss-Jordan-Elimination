﻿using FesetroundLibrary;
using System;
using System.Numerics.MPFR;

namespace EliminacjaGJ_CSharp.Class
{
    enum IAPrecision
    {
        LONGDOUBLE = 80,
        DOUBLE = 60,
        FLOAT = 60
    }

    class Interval<T>
    {
        public Interval() { }
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

        public static IAPrecision precision;

        public static Interval<T> IntRead(string str)
        {
            Interval<T> temp = new Interval<T>();
            mpfr_struct rop = new mpfr_struct();
            MPFRLibrary.mpfr_init2(rop, (ulong)precision);
            MPFRLibrary.mpfr_set_str(rop, str, 10, (int)Rounding.TowardsMinusInfinity);
            //implementation using mpfr needed


            return temp;

        }

        public override string ToString()
        {
            return String.Format($"[{this.a},{this.b}]");
        }
        public static Interval<T> operator +(Interval<T> a, Interval<T> b)
        {
            Interval<T> temp = new Interval<T>();
            FesetroundLibrary.FesetRound.SET_FPU_DOWNWARD();

            temp.a = (dynamic)a.a + (dynamic)b.a;
            FesetroundLibrary.FesetRound.SET_FPU_UPWARD();

            temp.b = (dynamic)a.b + (dynamic)b.b;

            FesetroundLibrary.FesetRound.SET_FPU_TONEAREST();

            return temp;
        }
        public static Interval<T> operator -(Interval<T> first, Interval<T> second)
        {
            Interval<T> temp = new Interval<T>();
            FesetroundLibrary.FesetRound.SET_FPU_DOWNWARD();

            temp.a = (dynamic)first.a - (dynamic)second.b;
            FesetroundLibrary.FesetRound.SET_FPU_UPWARD();

            temp.b = (dynamic)first.b - (dynamic)second.a;

            FesetroundLibrary.FesetRound.SET_FPU_TONEAREST();

            return temp;

        }
        public static Interval<T> operator *(Interval<T> x, Interval<T> y)
        {
            FesetRound.SET_FPU_DOWNWARD();
            Interval<T> r = new Interval<T>((dynamic)0, (dynamic)0);
            T x1y1, x1y2, x2y1;


            //asign r.a 
            x1y1 = (dynamic)x.a * y.a;
            x1y2 = (dynamic)x.a * y.b;
            x2y1 = (dynamic)x.b * y.a;
            r.a = (dynamic)x.b * y.b;
            if ((dynamic)x2y1 < r.a)
                r.a = x2y1;
            if ((dynamic)x1y2 < r.a)
                r.a = x1y2;
            if ((dynamic)x1y1 < (r.a))
                r.a = x1y1;


            //asign r.b
            FesetRound.SET_FPU_UPWARD();
            x1y1 = (dynamic)x.a * y.a;
            x1y2 = (dynamic)x.a * y.b;
            x2y1 = (dynamic)x.b * y.a;

            r.b = (dynamic)x.b * y.b;
            if ((dynamic)x2y1 > r.b)
                r.b = x2y1;
            if ((dynamic)x1y2 > r.b)
                r.b = x1y2;
            if ((dynamic)x1y1 > r.b)
                r.b = x1y1;
            FesetRound.SET_FPU_TONEAREST();

            return r;
        }


    }
}