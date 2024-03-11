using System;

namespace Tools
{
    public static class Rand
    {
        private static readonly Random _random = new Random();
        private const double RAND_MAX = 1;

        public static int Next()
        {
            return _random.Next();
        }

        public static int Next(int maxValue)
        {
            return _random.Next(maxValue);
        }

        public static int Next(int minValue, int maxValue)
        {
            return _random.Next(minValue, maxValue);
        }

        public static float NextFloat()
        {
            return (float)_random.NextDouble();
        }

        public static bool NextBool()
        {
            return _random.Next() % 2 == 0;
        }

        public static int NextPositiveOrNegative()
        {
            return Next(0, 2) * 2 - 1;
        }
        public static float NextPrice99(int maxPrice)
        {
            return Next(maxPrice) + 0.99f;
        }

        public static float Logistic(float mu, float s)
        {
            return (float)(mu + s * Math.Log(1.0 / Uniform(0, 1) - 1));
        }

        public static double Uniform(double a, double b)
        {
            return a + _random.NextDouble() * (b - a) / RAND_MAX;
        }
    }
}