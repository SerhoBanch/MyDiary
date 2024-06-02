using MyDiary.Providers.Factories;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MyDiary.Forms
{
    public partial class Registration : Form
    {
        private readonly Form previousForm;

        private readonly FactoryProvider factory = new();

        private readonly TextBox userNameTextBox;
        private readonly TextBox loginTextBox;
        private readonly TextBox passwordTextBox;
        private readonly TextBox password2TextBox;

        public Registration(Form previous)
        {
            this.previousForm = previous;

            InitializeComponent();

            #region Инициализация свойств формы
            this.Text = "MyDiary : Registration";

            this.Size = new(500, 400);

            this.BackColor = Colors.Background;
            this.ForeColor = Colors.Foreground;

            this.Font = new("Times New Roman", 24F, FontStyle.Regular);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            // Заголовок
            this.Controls.Add(ControlsHelper.GetLabel("Регистрация", 20, new(500, 50), new(150, 20)));

            // Имя пользователя
            this.Controls.Add(ControlsHelper.GetLabel("Ник", 16, new(140, 30), new(30, 72)));
            Panel userNamePanel = new();
            (userNameTextBox, userNamePanel) = ControlsHelper.GetPanelWithTextBox(14, new(200, 30), new(200, 70), false, false);
            this.Controls.Add(userNamePanel);

            // Логин пользователя
            this.Controls.Add(ControlsHelper.GetLabel("Логин", 16, new(140, 30), new(30, 112)));
            Panel loginPanel = new();
            (loginTextBox, loginPanel) = ControlsHelper.GetPanelWithTextBox(14, new(200, 30), new(200, 110), false, false);
            this.Controls.Add(loginPanel);

            // Пароль пользователя
            this.Controls.Add(ControlsHelper.GetLabel("Пароль", 16, new(140, 30), new(30, 152)));
            Panel passwordPanel = new();
            (passwordTextBox, passwordPanel) = ControlsHelper.GetPanelWithTextBox(14, new(200, 30), new(200, 150), true, false);
            this.Controls.Add(passwordPanel);

            // Подтверждение пароля
            this.Controls.Add(ControlsHelper.GetLabel("Пароль 2", 16, new(140, 30), new(30, 192)));
            Panel password2Panel = new();
            (password2TextBox, password2Panel) = ControlsHelper.GetPanelWithTextBox(14, new(200, 30), new(200, 190), true, false);
            this.Controls.Add(password2Panel);

            // Кнопки создания аккаунта (одна создаёт премиум аккаунт, другая обычный)
            var signUpPremiumButton = ControlsHelper.GetLinkButton("Премиум", 14, new(150, 35), new(100, 270));
            var signUpButton = ControlsHelper.GetLinkButton("Обычный", 14, new(150, 35), new(250, 270));
            
            signUpButton.MouseClick += SignUp;
            signUpPremiumButton.MouseClick += SignUpPremium;

            this.Controls.Add(signUpButton);
            this.Controls.Add(signUpPremiumButton);
            #endregion
        }

        #region Обработчики событий
        private void SignUpPremium(object sender, MouseEventArgs e)
        {
            SignUp(true);
        }

        private void SignUp(object sender, MouseEventArgs e)
        {
            SignUp(false);
        }

        private void Registration_FormClosed(object sender, FormClosedEventArgs e)
        {
            previousForm.Show();
        }
        #endregion

        /// <summary>
        /// Проверка на валидность пароля
        /// </summary>
        /// <returns></returns>
        private bool IsValidSigningUp()
        {
            if (factory.UserProvider.GetAll().Select(user => user.Login).Contains(loginTextBox.Text))
            {
                MessageBox.Show("Пользователь с таким логином уже существует");
                return false;
            }

            if (string.IsNullOrEmpty(passwordTextBox.Text) || 
                string.IsNullOrEmpty(password2TextBox.Text) ||
                string.IsNullOrEmpty(loginTextBox.Text) ||
                string.IsNullOrEmpty(userNameTextBox.Text))
            {
                MessageBox.Show("Все поля должны быть заполнены");
                return false;
            }

            if (passwordTextBox.Text != password2TextBox.Text)
            {
                MessageBox.Show("Пароли не совпадают");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Создать пользователя
        /// </summary>
        /// <param name="isPremium"></param>
        private void SignUp(bool isPremium)
        {
            if (IsValidSigningUp())
            {
                factory.UserProvider.Add(new()
                    {
                        Login = loginTextBox.Text,
                        UserName = userNameTextBox.Text,
                        Password = passwordTextBox.Text,
                        UserType = isPremium ? Models.UserType.Premium : Models.UserType.Simple
                    });
                BackToPreviousForm();
            }
        }

        private void BackToPreviousForm()
        {
            previousForm.Show();
            this.Close();
        }
    }
}
