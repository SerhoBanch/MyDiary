using MyDiary.Cache;
using MyDiary.Controls;
using MyDiary.Models;
using MyDiary.Providers.Factories;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MyDiary.Forms
{
    public partial class ShowEvents : Form
    {
        private readonly Form previousForm;

        private readonly FactoryProvider factory = new();

        private List<Event> events;
        private List<Event> displayEvents;

        private readonly Panel eventsPanel;

        public ShowEvents(Form previousForm)
        {
            this.previousForm = previousForm;

            InitializeComponent();

            #region Инициализация формы
            this.Text = "MyDiary : Список событий";

            this.Size = new(300, 200);

            this.BackColor = Colors.Background;
            this.ForeColor = Colors.Foreground;

            this.Font = new("Times New Roman", 24F, FontStyle.Regular);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            // Кнопка добавления события
            var addButton = ControlsHelper.GetButton("+", 12, new(30, 30), new(5, 5));
            addButton.MouseClick += AddEvent;
            this.Controls.Add(addButton);

            // Панель, на которой показаны все события
            eventsPanel = new();
            eventsPanel.Location = new(5, 40);
            eventsPanel.Size = new(this.Width - 15, this.Height - 40);
            this.Controls.Add(eventsPanel);

            var currentUser = UserCache.Instance.CurrentUser;
            if (currentUser.UserType == UserType.Premium)
            {
                var manipulateEventTypes = ControlsHelper.GetButton("Редактировать типы событий", 12, new(200, 30), new(550, 5));
                manipulateEventTypes.MouseClick += ManipulateEventTypes;
                this.Controls.Add(manipulateEventTypes);
            }

            UpdateEvents();
            #endregion
        }

        #region Обработчики событий
        private void ShowEvent(object sender, MouseEventArgs e)
        {
            var @event = factory.EventProvider.Get((sender as EventLabel).EventId);
            if (@event is not null)
            {
                Form form;
                if (UserCache.Instance.CurrentUser.UserType == UserType.Premium)
                    form = new PremiumAddShowEditEvent(ChooseFormType.Show, @event.EventId);
                else
                    form = new AddShowEditEvent(ChooseFormType.Show, @event.EventId);
                form.FormClosed += AddEditFormClosed;
                form.Show();
                return;
            }
            throw new ArgumentException("Событие не было найдено");
        }

        private void AddEvent(object sender, MouseEventArgs e)
        {
            Form form = new AddShowEditEvent(ChooseFormType.Add);
            form.FormClosed += AddEditFormClosed;
            form.Show();
        }

        private void EditEvent(object sender, MouseEventArgs e)
        {
            var @event = factory.EventProvider.Get((sender as EventButton).EventId);
            if (@event is not null)
            {
                Form form;
                if (UserCache.Instance.CurrentUser.UserType == UserType.Premium)
                    form = new PremiumAddShowEditEvent(ChooseFormType.Edit, @event.EventId);
                else
                    form = new AddShowEditEvent(ChooseFormType.Edit, @event.EventId);
                form.FormClosed += AddEditFormClosed;
                form.Show();
                return;
            }
            throw new ArgumentException("Событие не было найдено");
        }

        private void ShowEvents_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Выход из учётной записи
            UserCache.Instance.CurrentUser = null;
            previousForm.Show();
        }

        private void ManipulateEventTypes(object sender, MouseEventArgs e)
        {
            var form = new ManipulateEventTypes();
            form.FormClosed += AddEditFormClosed;
            form.Show();
        }
        #endregion

        private void UpdateEvents()
        {
            eventsPanel.Controls.Clear();

            var userCache = UserCache.Instance;
            // Получить события только для текущего полязователя
            // Доступны события, которые только будут, то есть нельзя увидить события, которые уже прошли
            events = factory.EventProvider.GetAll().Where(e => e.UserLogin.Equals(userCache.CurrentUser.Login) && e.GetDateTime().CompareTo(DateTime.Now) >= 0).ToList();
            displayEvents = new(events);

            if (displayEvents.Count < 1)
                return;

            Date previousDate = displayEvents[0].EventDate;
            Event previousEvent = null;
            int x = 0;
            int y = 0;
            int width = this.Width - 20;
            int heightLabel = 25;
            int space = 10;
            // Отображение самой первой даты
            eventsPanel.Controls.Add(ControlsHelper.GetPanelWithLabel(previousDate.ToString(), 14, new(width, heightLabel), new(x, y)));
            y += heightLabel + space;
            foreach (var @event in displayEvents)
            {
                if (!@event.EventDate.Equals(previousDate))
                {
                    previousDate = @event.EventDate;

                    // Вывод новой даты по списку
                    eventsPanel.Controls.Add(ControlsHelper.GetPanelWithLabel(@event.EventDate.ToString(), 14, new(width, heightLabel), new(x, y)));
                    y += heightLabel + space;
                }

                // Вывод события
                var eventLabel = ControlsHelper.GetEventLabel(@event.EventId, $"{@event.DisplayNote()}; {@event.EventType}; {@event.EventTime}; {@event.DurationAtMin} мин", 12, new(width - 200, heightLabel), new(x + 10, y));
                eventLabel.MouseClick += ShowEvent;
                eventsPanel.Controls.Add(eventLabel);

                var eventButton = ControlsHelper.GetEventButton(@event.EventId, "Изменить", 12, new(150, 30), new(x + 600, y));
                eventButton.MouseClick += EditEvent;
                eventsPanel.Controls.Add(eventButton);

                y += heightLabel + space;

                previousEvent = @event;
            }
        }

        private void AddEditFormClosed(object sender, FormClosedEventArgs e)
        {
            UpdateEvents();
        }
    }
}
