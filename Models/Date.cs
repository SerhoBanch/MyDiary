using System;
using System.Linq;

namespace MyDiary.Models
{
    public class Date : IComparable
    {
        private int year;
        private int month;
        private int day;

        public int Year
        {
            get => year;
            set
            {
                YearValidate(value);
                year = value;
            }
        }
        public int Month
        {
            get => month;
            set
            {
                MonthValidate(value);
                month = value;
            }
        }
        public int Day
        {
            get => day;
            set
            {
                DayValidate(value, month, year);
                day = value;
            }
        }

        public Date(int year = 2021, int month = 1, int day = 1)
        {
            YearValidate(year);
            MonthValidate(month);
            DayValidate(day, month, year);

            this.year = year;
            this.month = month;
            this.day = day;
        }

        private static void YearValidate(int year)
        {
            if (!(2020 <= year))
                throw new ArgumentException("Incorrect year");
        }

        private static void MonthValidate(int month)
        {
            if (!(1 <= month && month <= 12))
                throw new ArgumentException("Incorrect month");
        }

        private static void DayValidate (int day, int month, int year)
        {
            if (day < 1) throw new ArgumentException("Incorrect day");

            if ((new int[] { 1, 3, 5, 7, 8, 10, 12 }).Contains(month))
            {
                if (day > 31) throw new ArgumentException("Incorrect day");
            }

            if ((new int[] { 4, 6, 9, 11 }).Contains(month))
            {
                if (day > 30) throw new ArgumentException("Incorrect day");
            }

            if (2 == month && year % 4 == 0 && year % 100 != 0)
            {
                if (day > 29) throw new ArgumentException("Incorrect day");
            }
            else if (2 == month)
            {
                if (day > 28) throw new ArgumentException("Incorrect day");
            }
        }

        public override string ToString()
        {
            return $"{NumberToString(month)}/{NumberToString(day)}/{year}";
        }

        private static string NumberToString(int value)
        {
            return value < 10 ? "0" + value.ToString() : value.ToString();
        }

        public static bool IsValidString(string date)
        {
            if (!Int32.TryParse(date.Substring(0, 2), out var month) ||
                !Int32.TryParse(date.Substring(3, 2), out var day) ||
                !Int32.TryParse(date.Substring(6, 4), out var year))
                return false;

            try
            {
                YearValidate(year);
                MonthValidate(month);
                DayValidate(day, month, year);
            }
            catch (ArgumentException)
            {
                return false;
            }

            return true;
        }

        public static Date FromString(string date)
        {
            var splitChars = new char[] { ',', '.', '\\', '/', '-', ';', ':', '^' };

            var splitString = date.Split(splitChars);
            if (splitString.Length != 3)
                throw new ArgumentException("Дата задана в неверном формате");

            var month = Int32.Parse(splitString[0]);
            var day = Int32.Parse(splitString[1]);
            var year = Int32.Parse(splitString[2]);

            return new Date(year, month, day);
        }

        public static Date FromDateTime(DateTime dateTime) =>
            new(dateTime.Year, dateTime.Month, dateTime.Day);

        public int CompareTo(object obj)
        {
            if (obj is Date or DateTime)
            {
                var thisDate = this.ToDateTime();
                var compareDate = obj is Date date ? date.ToDateTime() : (DateTime)obj;

                return thisDate.CompareTo(compareDate);
            }
            throw new ArithmeticException("Недопустимо сравнение");
        }

        public DateTime ToDateTime()
        {
            return new(year, month, day);
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (obj == this) return true;
            return obj is Date date &&
                   year == date.year &&
                   month == date.month &&
                   day == date.day;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(year, month, day);
        }
    }
}
