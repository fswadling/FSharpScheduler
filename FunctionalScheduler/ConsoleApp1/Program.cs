using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp1
{
    public static class Program
    {
        public static IEnumerable<T> DoWhile<T>(this IEnumerable<T> s, Func<T, bool> predicate)
        {
            foreach (T item in s)
            {
                yield return item;
                if (!predicate(item))
                {
                    break;
                }
            }
        }

        public static IEnumerable<T> InfiniteSequence<T>(T seed, Func<T,T> generator)
        {
            T currentValue = seed;
            while (true)
            {
                yield return currentValue;
                currentValue = generator(currentValue);
            }
        }

        public static T AdjustInvalidValue<T>(Func<T, bool> isValueInvalid, Func<T, T> adjustValue, T value)
        {
            if (isValueInvalid(value))
            {
                var newValue = adjustValue(value);
                return AdjustInvalidValue(isValueInvalid, adjustValue, newValue);
            }
            else
            {
                return value;
            }
        }

        public static bool IsWeekend(DateTime date) =>
            date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;

        public static bool IsSpanishHoliday(DateTime date) =>
            date.Day == 12 && date.Month == 10;

        public static bool IsUSHoliday(DateTime date) =>
            date.Day == 13 && date.Month == 10;

        public static bool IsUKHoliday(DateTime date) =>
            date.Day == 14 && date.Month == 10;

        public static void Main(string[] args)
        {
            var startDate = new DateTime(2020, 10, 10);
            var endDate = new DateTime(2020, 10, 20);

            var schedule = InfiniteSequence(startDate, x => x.AddDays(1))
                .Select(x => AdjustInvalidValue(
                    isValueInvalid: d => IsWeekend(d) || IsUKHoliday(d) || IsUSHoliday(d) || IsSpanishHoliday(d), 
                    adjustValue: x => x.AddDays(1), 
                    value: x))
                .Distinct()
                .TakeWhile(x => x <= endDate);

            foreach (var date in schedule)
            {
                Console.WriteLine(date.ToString());
            }

            Console.WriteLine("------");
            Console.WriteLine("Reverse schedule ignore spanish holiday");

            var startDate2 = new DateTime(2020, 12, 20);
            var endDate2 = new DateTime(2020, 10, 10);

            var schedule2 = InfiniteSequence(startDate2, x => x.AddDays(-7))
                .Select(x => AdjustInvalidValue(
                    isValueInvalid: d => IsWeekend(d) || IsUKHoliday(d) || IsUSHoliday(d),
                    adjustValue: x => x.AddDays(-1), 
                    value: x))
                .Distinct()
                .DoWhile(x => x >= endDate2);

            foreach (var date in schedule2)
            {
                Console.WriteLine(date.ToString());
            }
            
        }
    }
}
