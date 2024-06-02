using MyDiary.Cache;
using MyDiary.Providers.Factories;
using System.Windows.Forms;

namespace MyDiary.Forms
{
    public partial class Reminder : Form
    {
        private readonly FactoryProvider factory = new();

        public Reminder(int eventId)
        {
            InitializeComponent();

            var @event = factory.EventProvider.Get(eventId);

            this.Text = "Напоминание";
            this.BackColor = Colors.Background;

            this.Controls.Add(ControlsHelper.GetLabel($"{UserCache.Instance.CurrentUser.UserName}, сейчас у тебя", 16, new(this.Width, 50), new(20, 20)));
            
            Panel notePanel;
            TextBox noteTextBox;
            (noteTextBox, notePanel) = ControlsHelper.GetPanelWithTextBox(12, new(this.Width - 60, 100), new(20, 90), false, true);
            noteTextBox.ReadOnly = true;
            noteTextBox.Text = @event.Note;
            this.Controls.Add(notePanel);
            
            this.Controls.Add(ControlsHelper.GetLabel(@event.EventTime.ToString(), 16, new(this.Width, 40), new(20, 210)));
            this.Controls.Add(ControlsHelper.GetLabel(@event.EventPlace, 16, new(this.Width, 50), new(20, 250)));
        }
    }
}
