using MyDiary.Models;
using MyDiary.Providers.Factories;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MyDiary.Forms
{
    public partial class AddEditEventType : Form
    {
        private readonly EventType editableEventType;

        private readonly FactoryProvider factory = new();

        private readonly TextBox eventTypeNameTextBox;

        public AddEditEventType(int? eventTypeId = null)
        {
            InitializeComponent();

            if (eventTypeId.HasValue)
                editableEventType = factory.EventTypeProvider.Get(eventTypeId.Value);

            #region Инициализация свойств
            this.Text = eventTypeId.HasValue ? editableEventType.EventTypeName : "Добавить";

            this.Size = new(300, 200);

            this.BackColor = Colors.Background;
            this.ForeColor = Colors.Foreground;

            this.Font = new("Times New Roman", 24F, FontStyle.Regular);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            this.Controls.Add(ControlsHelper.GetLabel("Название", 14, new(200, 30), new(10, 10)));
            Panel eventTypeNamePanel;
            (eventTypeNameTextBox, eventTypeNamePanel) = ControlsHelper.GetPanelWithTextBox(12, new(200, 30), new(10, 40), false, false);
            if (editableEventType is not null) eventTypeNameTextBox.Text = editableEventType.EventTypeName;
            this.Controls.Add(eventTypeNamePanel);

            var buttonText = eventTypeId.HasValue ? "Изменить" : "Добавить";
            var editButton = ControlsHelper.GetButton(buttonText, 12, new(200, 30), new(20, 80));
            editButton.MouseClick += AddEditEventTypeClick;
            this.Controls.Add(editButton);
            #endregion
        }

        #region Обработчики событий
        private void AddEditEventTypeClick(object sender, MouseEventArgs e)
        {
            var name = eventTypeNameTextBox.Text;

            if (editableEventType is not null && editableEventType.EventTypeName.Equals(name))
            {
                MessageBox.Show("Вы не поменяли название", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Название должно быть заполненым", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var currentUser = Cache.UserCache.Instance.CurrentUser;
            var eventTypes = factory.EventTypeProvider.GetAll().Where(x => x.UserLogin.Equals(currentUser.Login)).Select(x => x.EventTypeName).ToList();

            //if (currentUser.UserType != UserType.Admin)
            //{
            //    var admin = factory.UserProvider.GetAll().First(x => x.UserType == UserType.Admin);
            //    var adminEventTypes = factory.EventTypeProvider.GetAll().Where(x => x.UserLogin.Equals(admin.Login)).Select(x => x.EventTypeName);
            //    eventTypes.AddRange(adminEventTypes);
            //}
                
            if (eventTypes.Any(x => x.Equals(name)))
            {
                MessageBox.Show("Тип события с таким названием уже существует", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (editableEventType is null)
            {
                factory.EventTypeProvider.Add(new()
                {
                    EventTypeName = name,
                    UserLogin = currentUser.Login
                });
            }
            else
            {
                editableEventType.EventTypeName = eventTypeNameTextBox.Text;
                factory.EventTypeProvider.Update(editableEventType.EventTypeId, editableEventType);
            }

            this.Close();
        }
        #endregion
    }
}
