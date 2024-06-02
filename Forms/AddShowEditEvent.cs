using MyDiary.Cache;
using MyDiary.Models;
using MyDiary.Providers.Factories;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MyDiary.Forms
{
    public partial class AddShowEditEvent : Form
    {
        private readonly ChooseFormType type;

        private readonly FactoryProvider factory = new();

        private readonly ComboBox typeComboBox;
        private readonly TextBox dateTextBox;
        private readonly TextBox timeTextBox;
        private readonly TextBox durationTextBox;
        private readonly TextBox placeTextBox;
        private readonly TextBox descriptionTextBox;
        
        private readonly List<EventType> types;

        private readonly Event @event;

        public AddShowEditEvent(ChooseFormType type, int? eventId = null)
        {
            this.type = type;

            InitializeComponent();

            #region Form initial properties
            var text = type == ChooseFormType.Add ? "Добавить" : type == ChooseFormType.Edit ? "Изменить" : "Показать";
            this.Text = $"MyDiary : {text}";

            this.Size = new(660, 390);

            this.BackColor = Colors.Background;
            this.ForeColor = Colors.Foreground;

            this.Font = new("Times New Roman", 24F, FontStyle.Regular);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            // Type
            this.Controls.Add(ControlsHelper.GetLabel("Тип", 14, new(200, 30), new(10, 22)));
            var adminUser = factory.UserProvider.GetAll().First(x => x.UserType == UserType.Admin);
            var currentUser = UserCache.Instance.CurrentUser;
            types = new(factory.EventTypeProvider.GetAll().Where(x => x.UserLogin.Equals(currentUser.Login) || x.UserLogin.Equals(adminUser.Login)));
            typeComboBox = ControlsHelper.GetComboBox(types, 12, new(200, 30), new(230, 10));
            this.Controls.Add(typeComboBox);

            // Date
            this.Controls.Add(ControlsHelper.GetLabel("Дата(mm.dd.yyyy)", 14, new(200, 30), new(10, 62)));
            Panel datePanel = new();
            (dateTextBox, datePanel) = ControlsHelper.GetPanelWithTextBox(12, new(200, 30), new(230, 60), false, false);
            this.Controls.Add(datePanel);

            // Time
            this.Controls.Add(ControlsHelper.GetLabel("Время(hh:mm)", 14, new(200, 30), new(10, 102)));
            Panel timePanel = new();
            (timeTextBox, timePanel) = ControlsHelper.GetPanelWithTextBox(12, new(200, 30), new(230, 100), false, false);
            this.Controls.Add(timePanel);

            // Duration
            this.Controls.Add(ControlsHelper.GetLabel("Длительность(мин)", 14, new(220, 30), new(10, 142)));
            Panel durationPanel = new();
            (durationTextBox, durationPanel) = ControlsHelper.GetPanelWithTextBox(12, new(200, 30), new(230, 140), false, false);
            this.Controls.Add(durationPanel);

            // Place
            this.Controls.Add(ControlsHelper.GetLabel("Место", 14, new(200, 30), new(10, 182)));
            Panel placePanel = new();
            (placeTextBox, placePanel) = ControlsHelper.GetPanelWithTextBox(12, new(200, 30), new(230, 180), false, false);
            this.Controls.Add(placePanel);

            // Description
            this.Controls.Add(ControlsHelper.GetLabel("Описание", 14, new(200, 30), new(10, 222)));
            Panel descriptionPanel = new();
            (descriptionTextBox, descriptionPanel) = ControlsHelper.GetPanelWithTextBox(10, new(400, 60), new(230, 220), false, true);
            this.Controls.Add(descriptionPanel);

            // AddEditButton
            var addEditButton = ControlsHelper.GetButton(type == ChooseFormType.Add ? "Добавить" : type == ChooseFormType.Edit ? "Изменить" : "", 14, new(150, 35), new(230, 290));
            if (type == ChooseFormType.Show) addEditButton.Visible = false;
            addEditButton.MouseClick += AddEdit;
            this.Controls.Add(addEditButton);

            if (type == ChooseFormType.Show || type == ChooseFormType.Edit)
            {
                // Паранойя: никогда не наступит
                if (!eventId.HasValue) throw new ArgumentException("Что-то пошло не так при определении события");

                @event = factory.EventProvider.Get(eventId.Value);
                typeComboBox.SelectedIndex = types.FindIndex(type => type.EventTypeId == @event.EventTypeId);
                dateTextBox.Text = @event.EventDate.ToString();
                timeTextBox.Text = @event.EventTime.ToString();
                durationTextBox.Text = @event.DurationAtMin.ToString();
                placeTextBox.Text = @event.EventPlace.ToString();
                descriptionTextBox.Text = @event.Note;

                if (type == ChooseFormType.Show)
                {
                    typeComboBox.DropDownStyle = ComboBoxStyle.Simple;
                    dateTextBox.ReadOnly = true;
                    timeTextBox.ReadOnly = true;
                    durationTextBox.ReadOnly = true;
                    placeTextBox.ReadOnly = true;
                    descriptionTextBox.ReadOnly = true;
                }
            }
            #endregion
        }

        #region Обработчики событий
        private void AddEdit(object sender, MouseEventArgs e)
        {
            if (!IsValidAddEdit()) return;

            if (type == ChooseFormType.Add)
            {
                factory.EventProvider.Add(new()
                {
                    UserLogin = UserCache.Instance.CurrentUser.Login,
                    EventTypeId = types[typeComboBox.SelectedIndex].EventTypeId,
                    EventDate = Date.FromString(dateTextBox.Text),
                    EventTime = Time.FromString(timeTextBox.Text),
                    DurationAtMin = Int32.Parse(durationTextBox.Text),
                    EventPlace = placeTextBox.Text,
                    Note = descriptionTextBox.Text
                });
            }
            else if (type == ChooseFormType.Edit)
            {
                factory.EventProvider.Update(@event.EventId, new()
                {
                    UserLogin = UserCache.Instance.CurrentUser.Login,
                    EventTypeId = types[typeComboBox.SelectedIndex].EventTypeId,
                    EventDate = Date.FromString(dateTextBox.Text),
                    EventTime = Time.FromString(timeTextBox.Text),
                    DurationAtMin = Int32.Parse(durationTextBox.Text),
                    EventPlace = placeTextBox.Text,
                    Note = descriptionTextBox.Text
                });
            }

            BackToPreviousForm();
        }
        #endregion

        private bool IsValidAddEdit()
        {
            if (typeComboBox.SelectedIndex == -1 ||
                string.IsNullOrEmpty(dateTextBox.Text) ||
                string.IsNullOrEmpty(timeTextBox.Text) ||
                string.IsNullOrEmpty(durationTextBox.Text) ||
                string.IsNullOrEmpty(placeTextBox.Text) ||
                string.IsNullOrEmpty(descriptionTextBox.Text))
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

            if (!Int32.TryParse(durationTextBox.Text, out _))
            {
                MessageBox.Show("Продолжительность задана в неправильном формате");
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
