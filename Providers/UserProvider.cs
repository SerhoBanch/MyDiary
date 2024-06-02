using Microsoft.Data.SqlClient;
using MyDiary.Models;
using MyDiary.Providers.Abstract;
using System;
using System.Collections.Generic;

namespace MyDiary.Providers
{
    class UserProvider : CrudProviderBase<User, string>
    {
        #region Реализация паттерна Singlton
        private static readonly UserProvider instance = new();
        public static UserProvider Instance => instance;

        private UserProvider() { }
        #endregion

        public override void Add(User entity)
        {
            using var connection = GetConnection();
            var query = $"INSERT INTO DiaryUser(UserLogin, UserPassword, UserName, UserType) VALUES ('{entity.Login}', '{entity.Password}', '{entity.UserName}', '{entity.UserTypeString}')";
            SqlCommand insert = new(query, connection);
            insert.ExecuteNonQuery();
        }

        public override void Delete(string pk)
        {
            using var connection = GetConnection();
            var query = $"DELETE FROM DiaryUser WHERE UserLogin = '{pk}'";
            SqlCommand delete = new(query, connection);
            delete.ExecuteNonQuery();
        }

        public override User Get(string pk)
        {
            using var connection = GetConnection();

            var query = $"SELECT UserLogin, UserPassword, UserName, UserType FROM DiaryUser WHERE UserLogin = '{pk}'";
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

            user.Events ??= GetEvents(user.Login);
            return user;
        }

        public override IReadOnlyCollection<User> GetAll()
        {

            using var connection = GetConnection();
                var query = $"SELECT UserLogin, UserPassword, UserName, UserType FROM DiaryUser";
                SqlCommand select = new(query, connection);
                var result = select.ExecuteReader();

            List<User> users = new();

            if (result.HasRows)
                {
                    while (result.Read())
                    {
                        users.Add(new()
                        {
                            Login = (string)result["UserLogin"],
                            Password = (string)result["UserPassword"],
                            UserName = (string)result["UserName"],
                            UserType = User.GetUserType(result["UserType"].ToString())
                        });
                    }
                }

            foreach (var user in users)
                user.Events = GetEvents(user.Login);

            return users;
        }

        public override void Update(string pk, User entity)
        {
            using var connection = GetConnection();
            var query = $"UPDATE DiaryUser SET UserPassword = '{entity.Password}', UserName = '{entity.UserName}', UserType = '{entity.UserTypeString}' WHERE UserLogin = '{entity.Login}'";
            SqlCommand update = new(query, connection);
            update.ExecuteNonQuery();
        }

        private IReadOnlyCollection<Event> GetEvents(string login)
        {
            using var connection = GetConnection();
            var query = $"SELECT EventId, EventDate, EventTime, DurationAtMin, EventPlace, Note, UserLogin, EventTypeId FROM EventTable WHERE UserLogin = '{login}'";
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
    }
}
