using MyDiary.Models.Abstract;
using System;

namespace MyDiary.Models
{
    class Reminder : IModel
    {
        public Date ReminderDate { get; set; }
        public Time ReminderTime { get; set; }
        public int EventId { get; set; }
        public Event Event { get; set; }
        public string Note { get; set; }

        public ReminderPK PK => new() { EventId = this.EventId, ReminderDate = this.ReminderDate, ReminderTime = this.ReminderTime };
        public string Text => ReminderDate.ToString() + " " + ReminderTime.ToString();

        public DateTime GetDateTime() => new(ReminderDate.Year, ReminderDate.Month, ReminderDate.Day, ReminderTime.Hours, ReminderTime.Minutes, 0);
    }
}
