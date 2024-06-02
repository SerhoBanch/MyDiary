using Microsoft.Data.SqlClient;
using MyDiary.Models;
using MyDiary.Providers.Abstract;
using System;
using System.Collections.Generic;

namespace MyDiary.Providers
{
    class EventProvider : CrudProviderBase<Event, int>
    {
        #region Реализация паттерна Singlton
        private static readonly EventProvider instance = new();
        public static EventProvider Instance => instance;

        private EventProvider() { }
        #endregion

        public override void Add(Event entity)
        {
            using var connection = GetConnection();
            var query = $"INSERT INTO EventTable(EventDate, EventTime, DurationAtMin, EventPlace, Note, UserLogin, EventTypeId) VALUES ('{entity.EventDate}', '{entity.EventTime}', {entity.DurationAtMin}, '{entity.EventPlace}', '{entity.Note}', '{entity.UserLogin}', {entity.EventTypeId})";
            SqlCommand insert = new(query, connection);
            insert.ExecuteNonQuery();
        }

        public override void Delete(int pk)
        {
            using var connection = GetConnection();
            var query = $"DELETE FROM EventTable WHERE EventId = {pk}";
            SqlCommand delete = new(query, connection);
            delete.ExecuteNonQuery();
        }

        public override Event Get(int pk)
        {
            using var connection = GetConnection();
            var query = $"SELECT EventId, EventDate, EventTime, DurationAtMin, EventPlace, Note, UserLogin, EventTypeId FROM EventTable WHERE EventId = {pk}";
            SqlCommand select = new(query, connection);
            var result = select.ExecuteReader();

            Event @event = null;
            if (result.HasRows)
            {
                result.Read();

                @event = new()
                {
                    EventId = (int)result["EventId"],
                    EventDate = Date.FromDateTime(DateTime.Parse(result["EventDate"].ToString())),
                    EventTime = Time.FromString(result["EventTime"].ToString()),
                    DurationAtMin = (int)result["DurationAtMin"],
                    EventPlace = (string)result["EventPlace"],
                    Note = (string)result["Note"],
                    UserLogin = (string)result["UserLogin"],
                    EventTypeId = (int)result["EventTypeId"]
                };
            }

            @event.User ??= GetUser(@event.UserLogin);
            @event.EventType ??= GetEventType(@event.EventTypeId);

            return @event;
        }

        public override IReadOnlyCollection<Event> GetAll()
        {
            using var connection = GetConnection();

            var query = $"SELECT EventId, EventDate, EventTime, DurationAtMin, EventPlace, Note, UserLogin, EventTypeId FROM EventTable ORDER BY EventDate, EventTime";
            SqlCommand select = new(query, connection);
            var result = select.ExecuteReader();

            List<Event> events = new();

            if (result.HasRows)
            {
                while (result.Read())
                {
                    events.Add(new()
                    {
                        EventId = (int)result["EventId"],
                        EventDate = Date.FromDateTime(DateTime.Parse(result["EventDate"].ToString())),
                        EventTime = Time.FromString(result["EventTime"].ToString()),
                        DurationAtMin = (int)result["DurationAtMin"],
                        EventPlace = (string)result["EventPlace"],
                        Note = (string)result["Note"],
                        UserLogin = (string)result["UserLogin"],
                        EventTypeId = (int)result["EventTypeId"]
                    });
                }
            }


            foreach (var @event in events)
            {
                @event.User = GetUser(@event.UserLogin);
                @event.EventType = GetEventType(@event.EventTypeId);
            }

            return events;
        }

        public override void Update(int pk, Event entity)
        {
            using var connection = GetConnection();
            var query = $"UPDATE EventTable SET EventDate = '{entity.EventDate}', EventTime = '{entity.EventTime}', DurationAtMin = {entity.DurationAtMin}, EventPlace = '{entity.EventPlace}', Note = '{entity.Note}', EventTypeId = {entity.EventTypeId} WHERE EventId = {pk}";
            SqlCommand update = new(query, connection);
            var _ = update.ExecuteNonQuery();
        }

        private User GetUser(string login)
        {
            using var connection = GetConnection();
            var query = $"SELECT UserLogin, UserPassword, UserName, UserType FROM DiaryUser WHERE UserLogin = '{login}'";
            SqlCommand select = new(query, connection);
            var result = select.ExecuteReader();

            if (result.HasRows)
            {
                result.Read();

                return new User
                {
                    Login = (string)result["UserLogin"],
                    Password = (string)result["UserPassword"],
                    UserName = (string)result["UserName"],
                    UserType = User.GetUserType(result["UserType"].ToString())
                };
            }
            throw new ArgumentException("User has not been found");
        }

        private EventType GetEventType(int id)
        {
            using var connection = GetConnection();
            var query = $"SELECT EventTypeId, EventTypeName FROM EventType WHERE EventTypeId = {id}";
            SqlCommand select = new(query, connection);
            var result = select.ExecuteReader();

            if (result.HasRows)
            {
                result.Read();

                return new EventType
                {
                    EventTypeId = (int)result["EventTypeId"],
                    EventTypeName = (string)result["EventTypeName"]
                };
            }
            throw new ArgumentException("Type has not been found");
        }
    }
}
