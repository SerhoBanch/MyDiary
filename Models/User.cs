using MyDiary.Models.Abstract;
using System;
using System.Collections.Generic;

namespace MyDiary.Models
{
    enum UserType
    {
        Admin,
        Premium,
        Simple
    }

    class User : IModel
    {
        private string login;
        public string GetLogin() { return login; }
        public void SetLogin(string login) { this.login = login; }

        public string Login { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public UserType UserType { get; set; }
        public string UserTypeString => GetUserTypeString(UserType);

        public IReadOnlyCollection<Event> Events { get; set; } = new List<Event>();


        public static UserType GetUserType(string type)
        {
            return type switch
            {
                "ADMIN" => UserType.Admin,
                "PREMIUM" => UserType.Premium,
                "" => UserType.Simple,
                _ => throw new ArgumentException("Не получилось определить пользователя")
            };
        }

        public static string GetUserTypeString(UserType type)
        {
            return type switch
            {
                UserType.Admin => "ADMIN",
                UserType.Premium => "PREMIUM",
                UserType.Simple => "",
                _ => throw new ArgumentException("Не получилось определить пользователя")
            };
        }

        public static string GetUserTypeRussianString(UserType type)
        {
            return type switch
            {
                UserType.Admin => "Администратор",
                UserType.Premium => "Премиум",
                UserType.Simple => "Обычный",
                _ => throw new ArgumentException("Не получилось определить пользователя")
            };
        }
    }
}
