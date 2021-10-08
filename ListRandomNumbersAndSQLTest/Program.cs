using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListRandomNumbersAndSQLTest
{
    class Program
    {
        static void Main(string[] args)
        {
        }

        /// <summary>
        /// Gets a random list of numbers given an integer N.
        /// Guaranteed to be random by using a different seed to the Random number generator
        /// </summary>
        /// <param name="N"></param>
        /// <returns></returns>
        public List<int> GetRandomList(int N)
        {
            //Using the shuffling extension method below
            return new List<int>(Enumerable.Range(0, N + 1)).Shuffle();
        }
    }

    public static class ListExtensions
    {
        /// <summary>
        /// Uses Fisher–Yates shuffle algorithm
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        public static List<T> Shuffle<T>(this List<T> input)
        {
            List<T> list = new System.Collections.Generic.List<T>(input);
            Random rand = new Random((int)DateTime.Now.Ticks);
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = rand.Next(0, i + 1);
                T temp = list[j];
                list[j] = list[i];
                list[i] = temp;
            }

            return list;
        }
    }
}
