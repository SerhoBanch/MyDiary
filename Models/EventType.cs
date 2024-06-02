using MyDiary.Models.Abstract;
using System.Collections.Generic;

namespace MyDiary.Models
{
    class EventType : IModel
    {
        public int EventTypeId { get; set; }
        public string EventTypeName { get; set; }
        public string UserLogin { get; set; }
        public User User { get; set; }
        public IReadOnlyCollection<Event> Events { get; set; } = new List<Event>();

        public override string ToString()
        {
            return EventTypeName;
        }
    }
}
