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
    public partial class AdminMenu : Form
    {
        private readonly Form previousForm;

        private readonly FactoryProvider factory = new();

        private List<User> users;
        private readonly Panel usersPanel;

        private List<EventType> eventTypes;
        private readonly Panel eventTypesPanel;

        public AdminMenu(Form previous)
        {
            InitializeComponent();

            previousForm = previous;

            #region Инициализация свойств формы
            this.Text = "MyDiary : Администратор";

            this.Size = new(640, 500);

            this.BackColor = Colors.Background;
            this.ForeColor = Colors.Foreground;

            this.Font = new("Times New Roman", 24F, FontStyle.Regular);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            this.FormClosed += ClosedAdminMenu;

            // Панель, на которой показаны все пользователи
            usersPanel = ControlsHelper.GetPanelWithBorders(new(300, this.Height - 120), new(5, 40));
            this.Controls.Add(usersPanel);

            // Панель, на которой показаны все типы событий
            eventTypesPanel = ControlsHelper.GetPanelWithBorders(new(300, this.Height - 120), new(315, 40));
            this.Controls.Add(eventTypesPanel);

            // Кнопка вывода графика
            var chartButton = ControlsHelper.GetButton("Статистика", 12, new(150, 30), new(5, 5));
            chartButton.MouseClick += ShowChart;
            this.Controls.Add(chartButton);

            // Кнопка добавления типа события
            var eventTypeButton = ControlsHelper.GetButton("Добавить", 12, new(150, 30), new(465, 5));
            eventTypeButton.MouseClick += AddEventType;
            this.Controls.Add(eventTypeButton);

            UpdateData();
            #endregion
        }

        #region Обработчики событий
        private void ClosedAdminMenu(object sender, FormClosedEventArgs e)
        {
            Cache.UserCache.Instance.CurrentUser = null;
            previousForm.Show();
        }

        private void DeleteUser(object sender, MouseEventArgs e)
        {
            factory.UserProvider.Delete((sender as UserButton).Login);
            UpdateData();
        }

        private void EditUser(object sender, MouseEventArgs e)
        {
            var form = new EditUser((sender as UserLabel).Login);
            form.FormClosed += SupportFormClosed;
            form.Show();
        }

        private void AddEventType(object sender, MouseEventArgs e)
        {
            var form = new AddEditEventType();
            form.FormClosed += SupportFormClosed;
            form.Show();
        }

        private void DeleteEventType(object sender, MouseEventArgs e)
        {
            factory.EventTypeProvider.Delete((sender as EventTypeButton).EventTypeId);
            UpdateData();
        }

        private void EditEventType(object sender, MouseEventArgs e)
        {
            var form = new AddEditEventType((sender as EventTypeLabel).EventTypeId);
            form.FormClosed += SupportFormClosed;
            form.Show();
        }

        private void ShowChart(object sender, MouseEventArgs e)
        {
            new UserStatusesChart().Show();
        }
        #endregion

        private void SupportFormClosed(object sender, FormClosedEventArgs e)
        {
            UpdateData();
        }

        private void UpdateData()
        {
            #region Получаем данные с бд
            users = factory.UserProvider.GetAll().Where(x => x.UserType != UserType.Admin).ToList();
            eventTypes = factory.EventTypeProvider.GetAll().Where(x => x.User.UserType == UserType.Admin).ToList();
            #endregion

            int x = 10;
            int y = 10;
            int width = 130;
            int heightLabel = 25;
            int space = 10;

            #region Обновляем список пользователей
            usersPanel.Controls.Clear();
            foreach (var user in users)
            {
                var userLabel = ControlsHelper.GetUserLabel(user.Login, user.Login, 12, new(width, heightLabel), new(x, y));
                userLabel.MouseDoubleClick += EditUser;
                usersPanel.Controls.Add(userLabel);
                var deleteUserButton = ControlsHelper.GetUserButton(user.Login, "Удалить", 12, new(150, 30), new(x + 130, y));
                deleteUserButton.MouseClick += DeleteUser;
                usersPanel.Controls.Add(deleteUserButton);
                y += heightLabel + space;
            }
            #endregion

            y = 10;

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
