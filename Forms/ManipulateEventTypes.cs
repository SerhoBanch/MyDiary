using MyDiary.Controls;
using MyDiary.Models;
using MyDiary.Providers.Factories;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MyDiary.Forms
{
    public partial class ManipulateEventTypes : Form
    {
        private readonly FactoryProvider factory = new();

        private List<EventType> eventTypes;
        private readonly Panel eventTypesPanel;

        public ManipulateEventTypes()
        {
            InitializeComponent();

            this.Text = "Редактирование типов событий";

            this.Size = new(330, 500);

            this.BackColor = Colors.Background;
            this.ForeColor = Colors.Foreground;

            this.Font = new("Times New Roman", 24F, FontStyle.Regular);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            // Панель, на которой показаны все типы событий
            eventTypesPanel = ControlsHelper.GetPanelWithBorders(new(300, this.Height - 120), new(5, 40));
            this.Controls.Add(eventTypesPanel);

            // Кнопка добавления типа события
            var eventTypeButton = ControlsHelper.GetButton("Добавить", 12, new(150, 30), new(130, 5));
            eventTypeButton.MouseClick += AddEventType;
            this.Controls.Add(eventTypeButton);

            UpdateData();
        }

        #region Обработчики событий
        private void AddEventType(object sender, MouseEventArgs e)
        {
            var form = new AddEditEventType();
            form.FormClosed += SupportFormClosed;
            form.Show();
        }

        private void DeleteEventType(object sender, MouseEventArgs e)
        {
            var eventTypeId = (sender as EventTypeButton).EventTypeId;
            var events = factory.EventProvider.GetAll().Where(x => x.EventTypeId == eventTypeId);
            foreach (var @event in events)
            {
                // Event type with empty string
                @event.EventTypeId = 1;
                factory.EventProvider.Update(@event.EventId, @event);
            }
            factory.EventTypeProvider.Delete(eventTypeId);
            UpdateData();
        }

        private void EditEventType(object sender, MouseEventArgs e)
        {
            var form = new AddEditEventType((sender as EventTypeLabel).EventTypeId);
            form.FormClosed += SupportFormClosed;
            form.Show();
        }
        #endregion

        private void SupportFormClosed(object sender, FormClosedEventArgs e)
        {
            UpdateData();
        }

        private void UpdateData()
        {
            var currentUser = Cache.UserCache.Instance.CurrentUser;
            if (currentUser.UserType != UserType.Premium)
                this.Close();

            #region Получаем данные с бд
            eventTypes = factory.EventTypeProvider.GetAll().Where(x => x.UserLogin == currentUser.Login).ToList();
            #endregion

            int x = 10;
            int y = 10;
            int width = 130;
            int heightLabel = 25;
            int space = 10;

            #region Обновляем список типов событий
            eventTypesPanel.Controls.Clear();
            foreach (var eventType in eventTypes)
            {
                var eventTypeLabel = ControlsHelper.GetEventTypeLabel(eventType.EventTypeId, eventType.EventTypeName, 12, new(width, heightLabel), new(x, y));
                eventTypeLabel.MouseDoubleClick += EditEventType;
                eventTypesPanel.Controls.Add(eventTypeLabel);
                var deleteEventTypeButton = ControlsHelper.GetEventTypeButton(eventType.EventTypeId, "Удалить", 12, new(150, 30), new(x + 130, y));
                deleteEventTypeButton.MouseClick += DeleteEventType;
                eventTypesPanel.Controls.Add(deleteEventTypeButton);
                y += heightLabel + space;
            }
            #endregion
        }
    }
}
