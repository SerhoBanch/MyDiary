using Microsoft.Data.SqlClient;
using MyDiary.Models;
using MyDiary.Providers.Abstract;
using System;
using System.Collections.Generic;

namespace MyDiary.Providers
{
    class ReminderProvider : CrudProviderBase<Reminder, ReminderPK>
    {
        #region Реализация паттерна Singlton
        private static readonly ReminderProvider instance = new();
        public static ReminderProvider Instance => instance;

        private ReminderProvider() { }
        #endregion

        public override void Add(Reminder entity)
        {
            using var connection = GetConnection();
            var query = $"INSERT INTO Reminder(EventId, ReminderTime, ReminderDate, Note) VALUES ({entity.EventId}, '{entity.ReminderTime}', '{entity.ReminderDate}', '{entity.Note}')";
            SqlCommand insert = new(query, connection);
            insert.ExecuteNonQuery();
        }

        public override void Delete(ReminderPK pk)
        {
            using var connection = GetConnection();
            var query = $"DELETE FROM Reminder WHERE EventId = {pk.EventId} and ReminderTime = '{pk.ReminderTime}' and ReminderDate = '{pk.ReminderDate}'";
            SqlCommand delete = new(query, connection);
            delete.ExecuteNonQuery();
        }

        public override Reminder Get(ReminderPK pk)
        {
            using var connection = GetConnection();

            var query = $"SELECT EventId, ReminderTime, ReminderDate, Note FROM Reminder WHERE EventId = {pk.EventId} and ReminderTime = '{pk.ReminderTime}' and ReminderDate = '{pk.ReminderDate}'";
            SqlCommand select = new(query, connection);
            var result = select.ExecuteReader();

            Reminder reminder = null;
            if (result.HasRows)
            {
                result.Read();

                reminder = new()
                {
                    EventId = (int)result["EventId"],
                    ReminderTime = Time.FromString(result["ReminderTime"].ToString()),
                    ReminderDate = Date.FromDateTime(DateTime.Parse(result["ReminderDate"].ToString())),
                    Note = (string)result["Note"]
                };
            }

            reminder.Event ??= GetEvent(reminder.EventId);
            return reminder;
        }

        public override IReadOnlyCollection<Reminder> GetAll()
        {
            using var connection = GetConnection();
            var query = $"SELECT EventId, ReminderTime, ReminderDate, Note FROM Reminder";
            SqlCommand select = new(query, connection);
            var result = select.ExecuteReader();

            List<Reminder> reminders = new();

            if (result.HasRows)
            {
                while (result.Read())
                {
                    reminders.Add(new()
                    {
                        EventId = (int)result["EventId"],
                        ReminderTime = Time.FromString(result["ReminderTime"].ToString()),
                        ReminderDate = Date.FromDateTime(DateTime.Parse(result["ReminderDate"].ToString())),
                        Note = (string)result["Note"]
                    });
                }
            }

            foreach (var reminder in reminders)
                reminder.Event = GetEvent(reminder.EventId);

            return reminders;
        }

        public override void Update(ReminderPK pk, Reminder entity)
        {
            using var connection = GetConnection();
            var query = $"UPDATE Reminder SET Note = '{entity.Note}', ReminderTime = '{entity.ReminderTime}', ReminderDate = '{entity.ReminderDate}' WHERE EventId = {pk.EventId} and ReminderTime = '{pk.ReminderTime}' and ReminderDate = '{pk.ReminderDate}'";
            SqlCommand update = new(query, connection);
            update.ExecuteNonQuery();
        }

        private Event GetEvent(int id)
        {
            using var connection = GetConnection();
            var query = $"SELECT EventId, EventDate, EventTime, DurationAtMin, EventPlace, Note, UserLogin, EventTypeId FROM EventTable WHERE EventId = {id}";
            SqlCommand select = new(query, connection);
            var result = select.ExecuteReader();

            if (result.HasRows)
            {
                result.Read();

                return new()
                {
                    EventId = (int)result["EventId"],
                    EventDate = Date.FromDateTime(DateTime.Parse(result["EventDate"].ToString())),
                    EventTime = Time.FromString(result["EventTime"].ToString()),
                    DurationAtMin = (int)result["DurationAtMin"],
                    EventPlace = (string)result["EventPlace"],
                    Note = (string)result["Note"],
                    UserLogin = (string)result["UserLogin"],
                    EventTypeId = (int)result["EventTypeId"],
                };
            }
            throw new ArgumentException("Event has not been found");
        }
    }
}
