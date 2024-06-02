namespace MyDiary.Providers.Factories
{
    class FactoryProvider
    {
        public EventProvider EventProvider { get; }
        public EventTypeProvider EventTypeProvider { get; }
        public UserProvider UserProvider { get; }
        public ReminderProvider ReminderProvider { get; }

        public FactoryProvider()
        {
            EventProvider = EventProvider.Instance;
            EventTypeProvider = EventTypeProvider.Instance;
            UserProvider = UserProvider.Instance;
            ReminderProvider = ReminderProvider.Instance;
        }
    }
}
