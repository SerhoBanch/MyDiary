using MyDiary.Models;

namespace MyDiary.Cache
{
    class UserCache
    {
        #region Реализация паттерна Singleton
        private static readonly UserCache instance = new();
        public static UserCache Instance => instance;
        private UserCache() { }
        #endregion

        public User CurrentUser { get; set; } = null;

    }
}
