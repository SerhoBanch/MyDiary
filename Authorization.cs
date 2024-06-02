using MyDiary.Cache;
using MyDiary.Forms;
using MyDiary.Models;
using MyDiary.Providers.Factories;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace MyDiary
{
    public partial class Authorization : Form
    {
        private readonly Thread showRemindersThread = new(ShowReminders);

        private readonly FactoryProvider factory = new();

        private readonly TextBox loginTextBox;
        private readonly TextBox passwordTextBox;

        public Authorization()
        {
            InitializeComponent();

            #region Инициализация свойств формы
            this.Text = "MyDiary : Authorization";

            this.Size = new(600, 400);

            this.BackColor = Colors.Background;
            this.ForeColor = Colors.Foreground;

            this.Font = new("Times New Roman", 24F, FontStyle.Regular);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            // Title
            this.Controls.Add(ControlsHelper.GetLabel("это ЛИЧНЫЙ ежедневник", 20, new(500, 50), new(80, 20)));
            this.Controls.Add(ControlsHelper.GetLabel("Вход только для своих", 18, new(500, 50), new(130, 70)));
            // Login
            this.Controls.Add(ControlsHelper.GetLabel("Логин", 16, new(140, 30), new(30, 132)));
            Panel loginPanel = new();
            (loginTextBox, loginPanel) = ControlsHelper.GetPanelWithTextBox(14, new(200, 30), new(170, 130), false, false);
            this.Controls.Add(loginPanel);

            // Password
            this.Controls.Add(ControlsHelper.GetLabel("Пароль", 16, new(140, 30), new(30, 182)));
            Panel passwordPanel = new();
            (passwordTextBox, passwordPanel) = ControlsHelper.GetPanelWithTextBox(14, new(200, 30), new(170, 180), true, false);
            this.Controls.Add(passwordPanel);

            // Buttons
            var signInButton = ControlsHelper.GetButton("Войти", 14, new(150, 35), new(200, 230));
            var signUpButton = ControlsHelper.GetLinkButton("Зарегестрироваться", 10, new(200, 35), new(175, 270));

            signInButton.Click += SignIn;
            signUpButton.Click += SignUp;

            this.Controls.Add(signInButton);
            this.Controls.Add(signUpButton);
            #endregion
        }

        #region Обработчики событий
        private void SignUp(object sender, EventArgs e)
        {
            this.Hide();
            new Registration(this).Show();

            SetTextOfTextBoxEmpty();
        }

        private void SignIn(object sender, EventArgs e)
        {
            var usersProvider = factory.UserProvider;
            var user = usersProvider
                .GetAll()
                .FirstOrDefault(user => user.Login.Equals(loginTextBox.Text) &&
                                user.Password.Equals(passwordTextBox.Text));

            if (user == null)
            {
                MessageBox.Show("Неверный логин или пароль!");
                return;
            }

            UserCache.Instance.CurrentUser = user;

            if (user.UserType == UserType.Admin)
            {
                this.Hide();
                new AdminMenu(this).Show();
            }
            else
            {
                this.Hide();
                new ShowEvents(this).Show();
            }
            // Если пользователь имеет премиумный аккаунт, то запускаем отдельный поток,
            // в котором будут проверяться уведомления и в котором они же будут выводиться 
            if (user.UserType == UserType.Premium && !showRemindersThread.IsAlive) showRemindersThread.Start();

            SetTextOfTextBoxEmpty();
        }

        private void Authorization_FormClosed(object sender, FormClosedEventArgs e) { }// => showRemindersThread.Interrupt();
        #endregion

        private void SetTextOfTextBoxEmpty()
        {
            loginTextBox.Text = string.Empty;
            passwordTextBox.Text = string.Empty;
        }

        private static void ShowReminders()
        {
            FactoryProvider factory = new();

            // Крутиться всё время
            // При закрытии окна программа не завершит своё выполение, пока мы не выйдем с этого метода
            // То есть, если пользователь вышел и закрыл все окна, программа будет ещё крутиться в фоновом режиме меньше минуты
            while (true)
            {
                var user = UserCache.Instance.CurrentUser;

                // Если пользователь вышел с системы
                if (user == null)
                    return;

                var dateTime = DateTime.Now;

                // Загружаем данные с бд
                var events = factory.EventProvider.GetAll().Where(@event => @event.UserLogin.Equals(user.Login)).ToList();
                var dbReminders = factory.ReminderProvider.GetAll();
                List<Models.Reminder> reminders = new();
                foreach (var @event in events)
                    reminders.AddRange(dbReminders.Where(x => x.EventId == @event.EventId));

                foreach (var reminder in reminders)
                {
                    // Convert for local date and time
                    var date = Date.FromDateTime(dateTime);
                    var time = Time.FromDateTime(dateTime);

                    if (reminder.ReminderDate.Equals(date) && reminder.ReminderTime.Equals(time))
                    {
                        Forms.Reminder reminderForm = new(reminder.EventId);
                        Application.Run(reminderForm);
                    }
                }

                // Проверяем данные каждую минуту
                Thread.Sleep(60000);
            }
        }
    }
}
