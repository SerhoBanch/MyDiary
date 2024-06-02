using MyDiary.Models.Abstract;
using System;
using System.Collections.Generic;

namespace MyDiary.Models
{
    class Event : IModel
    {
        public int EventId { get; set; }
        public Date EventDate { get; set; }
        public Time EventTime { get; set; }
        public int DurationAtMin { get; set; }
        public string EventPlace { get; set; }
        public string Note { get; set; }
        public string UserLogin { get; set; }
        public User User { get; set; }
        public int EventTypeId { get; set; }
        public EventType EventType { get; set; }

        public IReadOnlyCollection<Reminder> Reminders { get; set; } = new List<Reminder>();

        public string DisplayNote() => Note.Length < 50 ? Note : Note.Substring(0, 50) + "...";
        public DateTime GetDateTime() => new(EventDate.Year, EventDate.Month, EventDate.Day, EventTime.Hours, EventTime.Minutes, 0);

    }
}
