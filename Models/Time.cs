using System;

namespace MyDiary.Models
{
    public class Time
    {
        private int _hours;
        private int _minutes;

        public int Hours
        {
            get => _hours;
            set
            {
                ValidateHours(value);
                _hours = value;
            }
        }

        public int Minutes
        {
            get => _minutes;
            set
            {
                ValidateMinutes(value);
                _minutes = value;
            }
        }

        /// <summary>
        /// Initialize Time
        /// </summary>
        /// <param name="hours">0 by default</param>
        /// <param name="minutes">0 by default</param>
        public Time(int hours = 0, int minutes = 0)
        {
            ValidateTime(hours, minutes);
            _hours = hours;
            _minutes = minutes;
        }

        private static void ValidateTime(int hours, int minutes)
        {
            ValidateHours(hours);
            ValidateMinutes(minutes);
        }

        private static void ValidateMinutes(int minutes)
        {
            var isValid = 0 <= minutes && minutes < 60;
            if (!isValid) throw new ArgumentException();
        }

        private static void ValidateHours(int hours)
        {
            var isValid = 0 <= hours && hours < 24;
            if (!isValid) throw new ArgumentException();
        }

        public static Time FromString(string time)
        {
            var hours = Int32.Parse(time.Substring(0, 2));
            var minutes = Int32.Parse(time.Substring(3, 2));
            return new(hours, minutes);
        }

        public static Time FromDateTime(DateTime dateTime) => new(dateTime.Hour, dateTime.Minute);

        public override string ToString() => NumberToString(_hours) + ":" + NumberToString(_minutes);

        private static string NumberToString(int value) => value < 10 ? "0" + value.ToString() : value.ToString();

        internal static bool IsValidString(string time)
        {
            if (!Int32.TryParse(time.Substring(0, 2), out var hours) ||
                !Int32.TryParse(time.Substring(3, 2), out var minutes))
                return false;

            try
            {
                ValidateHours(hours);
                ValidateMinutes(minutes);
            }
            catch (ArgumentException)
            {
                return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (obj == this) return true;
            return obj is Time time &&
                   _hours == time._hours &&
                   _minutes == time._minutes;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_hours, _minutes);
        }
    }
}