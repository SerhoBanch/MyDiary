using Microsoft.Data.SqlClient;
using MyDiary.Models;
using MyDiary.Providers.Abstract;
using System;
using System.Collections.Generic;

namespace MyDiary.Providers
{
    class EventTypeProvider : CrudProviderBase<EventType, int>
    {
        #region Реализация паттерна Singlton
        private static readonly EventTypeProvider instance = new();
        public static EventTypeProvider Instance => instance;

        private EventTypeProvider() { }
        #endregion

        public override void Add(EventType entity)
        {
            using var connection = GetConnection();
            var query = $"INSERT INTO EventType(EventTypeName, UserLogin) VALUES ('{entity.EventTypeName}', '{entity.UserLogin}')";
            SqlCommand insert = new(query, connection);
            insert.ExecuteNonQuery();
        }

        public override void Delete(int pk)
        {
            using var connection = GetConnection();
            var query = $"DELETE FROM EventType WHERE EventTypeId = {pk}";
            SqlCommand delete = new(query, connection);
            delete.ExecuteNonQuery();
        }

        public override EventType Get(int pk)
        {
            using var connection = GetConnection();

            var query = $"SELECT EventTypeId, EventTypeName, UserLogin FROM EventType WHERE EventTypeId = {pk}";
            SqlCommand select = new(query, connection);
            var result = select.ExecuteReader();

            EventType eventType = null;
            if (result.HasRows)
            {
                result.Read();

                eventType = new()
                {
                    EventTypeId = (int)result["EventTypeId"],
                    EventTypeName = (string)result["EventTypeName"],
                    UserLogin = (string)result["UserLogin"],
                };
            }

            eventType.User ??= GetUser(eventType.UserLogin);
            eventType.Events ??= GetEvents(eventType.EventTypeId);

            return eventType;
        }

        public override IReadOnlyCollection<EventType> GetAll()
        {
            using var connection = GetConnection();
            var query = $"SELECT EventTypeId, EventTypeName, UserLogin FROM EventType";
            SqlCommand select = new(query, connection);
            var result = select.ExecuteReader();

            List<EventType> eventTypes = new();

            if (result.HasRows)
            {
                while (result.Read())
                {
                    eventTypes.Add(new()
                    {
                        EventTypeId = (int)result["EventTypeId"],
                        EventTypeName = (string)result["EventTypeName"],
                        UserLogin = (string)result["UserLogin"],
                    });
                }
            }

            foreach (var eventType in eventTypes)
            {
                eventType.User = GetUser(eventType.UserLogin);
                eventType.Events = GetEvents(eventType.EventTypeId);
            }

            return eventTypes;
        }

        public override void Update(int pk, EventType entity)
        {
            using var connection = GetConnection();
            var query = $"UPDATE EventType SET EventTypeName = '{entity.EventTypeName}' WHERE EventTypeId = {entity.EventTypeId}";
            SqlCommand update = new(query, connection);
            update.ExecuteNonQuery();
        }

        private IReadOnlyCollection<Event> GetEvents(int id)
        {
            using var connection = GetConnection();
            var query = $"SELECT EventId, EventDate, EventTime, DurationAtMin, EventPlace, Note, UserLogin, EventTypeId FROM EventTable WHERE EventTypeId = {id}";
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
                        EventTypeId = (int)result["EventTypeId"],
                    });
                }
            }
            return events;
        }

        private User GetUser(string login)
        {
            using var connection = GetConnection();

            var query = $"SELECT UserLogin, UserPassword, UserName, UserType FROM DiaryUser WHERE UserLogin = '{login}'";
            SqlCommand select = new(query, connection);
            var result = select.ExecuteReader();

            User user = null;
            if (result.HasRows)
            {
                result.Read();

                user = new()
                {
                    Login = (string)result["UserLogin"],
                    Password = (string)result["UserPassword"],
                    UserName = (string)result["UserName"],
                    UserType = User.GetUserType(result["UserType"].ToString())
                };
            }

            return user;
        }
    }
}
