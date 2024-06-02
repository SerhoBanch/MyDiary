using MyDiary.Models;
using MyDiary.Providers.Factories;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace MyDiary.Forms
{
    public partial class EditUser : Form
    {
        private readonly User editableUser;

        private readonly FactoryProvider factory = new();

        public EditUser(string login)
        {
            InitializeComponent();

            editableUser = factory.UserProvider.Get(login);

            #region Инициализация свойств
            this.Text = login;

            this.Size = new(300, 200);

            this.BackColor = Colors.Background;
            this.ForeColor = Colors.Foreground;

            this.Font = new("Times New Roman", 24F, FontStyle.Regular);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            this.Controls.Add(ControlsHelper.GetLabel(editableUser.UserName, 16, new(this.Width, 30), new(20, 20)));

            if (editableUser.UserType == UserType.Admin)
                this.Close();

            var premiumCheckBox = ControlsHelper.GetCheckBox("Премиум", editableUser.UserType == UserType.Premium, new(200, 30), new(20, 50));
            premiumCheckBox.CheckStateChanged += ChangeUserStatus;
            this.Controls.Add(premiumCheckBox);

            var editButton = ControlsHelper.GetButton("Изменить", 12, new(200, 30), new(20, 100));
            editButton.MouseClick += EditUserClick;
            this.Controls.Add(editButton);
            #endregion
        }

        #region Обработчики событий
        private void EditUserClick(object sender, MouseEventArgs e)
        {
            factory.UserProvider.Update(editableUser.Login, editableUser);
            this.Close();
        }

        private void ChangeUserStatus(object sender, EventArgs e)
        {
            var isChecked = (sender as CheckBox).Checked;
            editableUser.UserType = isChecked ? UserType.Premium : UserType.Simple;
        }
        #endregion
    }
}
