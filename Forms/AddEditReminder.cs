using MyDiary.Models;
using MyDiary.Providers.Factories;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MyDiary.Forms
{
    public partial class AddEditReminder : Form
    {
        private readonly FactoryProvider factory = new();

        private readonly TextBox dateTextBox;
        private readonly TextBox timeTextBox;

        private readonly int eventId;
        private readonly Models.Reminder editableReminder;

        public AddEditReminder(int eventId, ReminderPK reminderPk = null)
        {
            InitializeComponent();

            #region Инициализация свойств формы
            var text = reminderPk == null ? "Добавить" : "Изменить";
            this.Text = $"MyDiary : {text}";

            this.Size = new(500, 200);

            this.BackColor = Colors.Background;
            this.ForeColor = Colors.Foreground;

            this.Font = new("Times New Roman", 24F, FontStyle.Regular);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            this.eventId = eventId;

            // Date
            this.Controls.Add(ControlsHelper.GetLabel("Дата(mm.dd.yyyy)", 14, new(200, 30), new(10, 12)));
            Panel datePanel = new();
            (dateTextBox, datePanel) = ControlsHelper.GetPanelWithTextBox(12, new(200, 30), new(230, 10), false, false);
            this.Controls.Add(datePanel);

            // Time
            this.Controls.Add(ControlsHelper.GetLabel("Время(hh:mm)", 14, new(200, 30), new(10, 52)));
            Panel timePanel = new();
            (timeTextBox, timePanel) = ControlsHelper.GetPanelWithTextBox(12, new(200, 30), new(230, 50), false, false);
            this.Controls.Add(timePanel);

            // Кнопка добавления/редактирования
            var addEditButton = ControlsHelper.GetButton(reminderPk == null ? "Добавить" : "Изменить", 14, new(150, 35), new(230, 90));
            addEditButton.MouseClick += AddEdit;
            this.Controls.Add(addEditButton);

            if (reminderPk is not null)
            {
                editableReminder = factory.ReminderProvider.Get(reminderPk);

                dateTextBox.Text = editableReminder.ReminderDate.ToString();
                timeTextBox.Text = editableReminder.ReminderTime.ToString();
            }
            #endregion
        }

        #region Обработчики событий
        private void AddEdit(object sender, MouseEventArgs e)
        {
            if (!IsValidAddEdit()) return;

            var reminderDate = Date.FromString(dateTextBox.Text);
            var reminderTime = Time.FromString(timeTextBox.Text);

            // Редактировать
            if (editableReminder is not null)
            {
                if (editableReminder.ReminderDate.Equals(reminderDate) &&
                    editableReminder.ReminderTime.Equals(reminderTime))
                {
                    MessageBox.Show("Вы ничего не изменили", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (factory.ReminderProvider.GetAll().Where(x => x.EventId == editableReminder.EventId).Any())
                {
                    MessageBox.Show("Напоминание на эту дату и это время уже стоит", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                factory.ReminderProvider.Update(editableReminder.PK, new() { ReminderDate = reminderDate, ReminderTime = reminderTime });
                BackToPreviousForm();
                return;
            }

            // Добавить
            if (factory.ReminderProvider.GetAll().Where(x => x.EventId == eventId).Any(x => x.ReminderDate.Equals(reminderDate) && x.ReminderTime.Equals(reminderTime)))
            {
                MessageBox.Show("Напоминание на эту дату и это время уже стоит", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            factory.ReminderProvider.Add(new() { ReminderDate = reminderDate, ReminderTime = reminderTime, EventId = eventId });
            BackToPreviousForm();
        }
        #endregion

        private bool IsValidAddEdit()
        {
            if (string.IsNullOrEmpty(dateTextBox.Text) ||
                string.IsNullOrEmpty(timeTextBox.Text))
            {
                MessageBox.Show("Все поля должны быть заполнены");
                return false;
            }

            if (!Date.IsValidString(dateTextBox.Text))
            {
                MessageBox.Show("Дата задана в неправильном формате");
                return false;
            }

            if (!Time.IsValidString(timeTextBox.Text))
            {
                MessageBox.Show("Время задано в неправильном формате");
                return false;
            }

            return true;
        }

        private void BackToPreviousForm()
        {
            this.Close();
        }
    }
}
