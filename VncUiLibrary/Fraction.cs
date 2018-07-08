using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VncUiLibrary
{
    class Fraction
    {
        public int Numerator
        {
            get;
            private set;
        }
        public int Denominator
        {
            get;
            private set;
        }
        /// <summary>
        /// Construct a fraction.
        /// </summary>
        /// <param name="a_numerator"></param>
        /// <param name="a_denominator"></param>
        public Fraction(int a_numerator, int a_denominator)
        {
            int g = gcd(a_numerator, a_denominator);
            Numerator   = a_numerator   / g;
            Denominator = a_denominator / g;
        }

        /// <summary>
        /// Construct a fraction that is the closest with denominator at most a_maxDenominator.
        /// I refer to python's limit_denominator.
        /// 
        /// Fraction(121, 100, 100)
        /// -> Fraction(121, 100)
        /// 
        /// Fraction(121, 100, 10)
        /// -> Fraction(12, 10)
        /// </summary>
        /// <param name="a_numerator"></param>
        /// <param name="a_denominator"></param>
        /// <param name="a_maxDenominator"></param>
        /// <seealso cref="https://github.com/python/cpython/blob/2.7/Lib/fractions.py"/>
        public Fraction(int a_numerator, int a_denominator, int a_maxDenominator)
        {
            if (a_maxDenominator < 1)
            {
                throw new ApplicationException("a_maxDenominator should be at least 1");
            }

            int g = gcd(a_numerator, a_denominator);
            int p0 = 0;
            int q0 = 1;
            int p1 = 1;
            int q1 = 0;
            int n = a_numerator   / g;
            int d = a_denominator / g;
            if (d <= a_maxDenominator)
            {
                Numerator   = n;
                Denominator = d;
                return;
            }

            while (true)
            {
                int a  = n / d;
                int q2 = q0 + a * q1;
                if (q2 > a_maxDenominator)
                {
                    break;
                }

                int newP1 = p0 + a * p1;
                p0 = p1;
                q0 = q1;
                p1 = newP1;
                q1 = q2;

                int newD = n - a * d;
                n = d;
                d = newD;
            }

            int k = (a_maxDenominator - q0) / q1;
            Fraction bound1 = new Fraction(p0 + k * p1, q0 + k * q1);
            Fraction bound2 = new Fraction(p1, q1);
            Fraction baseValue = new Fraction(a_numerator, a_denominator);
            if (Math.Abs((bound2 - baseValue).ToDouble()) <= Math.Abs((bound1 - baseValue).ToDouble()))
            {
                Numerator   = bound2.Numerator;
                Denominator = bound2.Denominator;
            }
            else
            {
                Numerator   = bound1.Numerator;
                Denominator = bound1.Denominator;
            }
        }
        public static Fraction operator -(Fraction a_lhs, Fraction a_rhs)
        {
            return new Fraction((a_lhs.Numerator * a_rhs.Denominator) + (a_rhs.Numerator * a_lhs.Denominator), a_lhs.Denominator * a_rhs.Denominator);
        }
        public double ToDouble()
        {
            return ((double)this.Numerator / this.Denominator);
        }
        private int gcd(int a_lhs, int a_rhs)
        {
            if (a_lhs < a_rhs)
            {
                return gcd(a_rhs, a_lhs);
            }
            while (a_rhs != 0)
            {
                int r = a_lhs % a_rhs;
                a_lhs = a_rhs;
                a_rhs = r;
            }
            return a_lhs;
        }
    }
}
