using MyDiary.Providers.Factories;
using MyDiary.Models;
using System.Linq;
using System.Windows.Forms;

namespace MyDiary.Forms
{
    public partial class UserStatusesChart : Form
    {
        public UserStatusesChart()
        {
            InitializeComponent();

            this.Text = "MyDiary: Статистика";

            this.Size = new(800, 800);

            this.BackColor = Colors.Background;
            this.ForeColor = Colors.Foreground;

            var chartData = new FactoryProvider().UserProvider.GetAll().GroupBy(x => x.UserType).Select(x => (User.GetUserTypeRussianString(x.Key), x.Count())).ToList();
            this.Controls.Add(ControlsHelper.GetPanelWithChart(chartData, new(800, 800), new(0, 0)));
        }
    }
}
