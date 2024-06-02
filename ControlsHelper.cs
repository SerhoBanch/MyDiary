using MyDiary.Controls;
using MyDiary.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MyDiary
{
    class ControlsHelper
    {
        public static ReminderLabel GetReminderLabel(ReminderPK reminderPk, string text, int fontSize, Size size, Point location)
        {
            return new()
            {
                ReminderPk = reminderPk,
                Text = text,
                Font = new("Times New Roman", fontSize, FontStyle.Regular),
                Size = size,
                Location = location
            };
        }

        public static UserLabel GetUserLabel(string login, string text, int fontSize, Size size, Point location)
        {
            return new()
            {
                Login = login,
                Text = text,
                Font = new("Times New Roman", fontSize, FontStyle.Regular),
                Size = size,
                Location = location
            };
        }

        public static EventTypeLabel GetEventTypeLabel(int eventTypeId, string text, int fontSize, Size size, Point location)
        {
            return new()
            {
                EventTypeId = eventTypeId,
                Text = text,
                Font = new("Times New Roman", fontSize, FontStyle.Regular),
                Size = size,
                Location = location
            };
        }

        public static EventLabel GetEventLabel(int eventId, string text, int fontSize, Size size, Point location)
        {
            return new()
            {
                EventId = eventId,
                Text = text,
                Font = new("Times New Roman", fontSize, FontStyle.Regular),
                Size = size,
                Location = location
            };
        }

        public static Label GetLabel(string text, int fontSize, Size size, Point location)
        {
            return new()
            {
                Text = text,
                Font = new("Times New Roman", fontSize, FontStyle.Regular),
                Size = size,
                Location = location
            };
        }

        public static (TextBox, Panel) GetPanelWithTextBox(int sizeFont, Size size, Point location, bool isPassword, bool isMultiline)
        {
            // TextBox with 
            TextBox textBox = new()
            {
                Font = new("Times New Roman", sizeFont, FontStyle.Regular),
                Size = new(size.Width - 2, size.Height - 2),
                Location = new(1, 1),
                BackColor = Colors.TextBoxBackground,
                BorderStyle = BorderStyle.None,
                Multiline = isMultiline
            };

            if (isPassword)
                textBox.PasswordChar = '*';

            Panel panel = new()
            {
                Location = location,
                Size = size
            };
            panel.Controls.Add(textBox);
            panel.Paint += TextBoxBorderPaint;

            return (textBox, panel);
        }

        /// <summary>
        /// Draw border for TextBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void TextBoxBorderPaint(object sender, PaintEventArgs e)
        {
            var panel = (Panel)sender;
            Pen pen = new(Colors.Foreground, 2F);
            e.Graphics.DrawRectangle(pen, 1, 1, panel.Width - 1, panel.Height - 1);
        }

        public static Button GetButton(string text, int sizeFont, Size size, Point location)
        {
            Button button = new()
            {
                Text = text,
                Font = new("Times New Roman", sizeFont, FontStyle.Regular),
                Size = size,
                Location = location,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Colors.Foreground,
                BackColor = Colors.TextBoxBackground
            };
            button.FlatAppearance.BorderSize = 2;
            return button;
        }

        public static EventButton GetEventButton(int eventId, string text, int sizeFont, Size size, Point location)
        {
            EventButton button = new()
            {
                EventId = eventId,
                Text = text,
                Font = new("Times New Roman", sizeFont, FontStyle.Regular),
                Size = size,
                Location = location,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Colors.Foreground,
                BackColor = Colors.TextBoxBackground
            };
            button.FlatAppearance.BorderSize = 2;
            return button;
        }

        public static EventTypeButton GetEventTypeButton(int eventTypeId, string text, int sizeFont, Size size, Point location)
        {
            EventTypeButton button = new()
            {
                EventTypeId = eventTypeId,
                Text = text,
                Font = new("Times New Roman", sizeFont, FontStyle.Regular),
                Size = size,
                Location = location,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Colors.Foreground,
                BackColor = Colors.TextBoxBackground
            };
            button.FlatAppearance.BorderSize = 2;
            return button;
        }

        public static UserButton GetUserButton(string login, string text, int sizeFont, Size size, Point location)
        {
            UserButton button = new()
            {
                Login = login,
                Text = text,
                Font = new("Times New Roman", sizeFont, FontStyle.Regular),
                Size = size,
                Location = location,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Colors.Foreground,
                BackColor = Colors.TextBoxBackground
            };
            button.FlatAppearance.BorderSize = 2;
            return button;
        }

        public static ReminderButton GetReminderButton(ReminderPK reminderPk, string text, int sizeFont, Size size, Point location)
        {
            ReminderButton button = new()
            {
                ReminderPk = reminderPk,
                Text = text,
                Font = new("Times New Roman", sizeFont, FontStyle.Regular),
                Size = size,
                Location = location,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Colors.Foreground,
                BackColor = Colors.TextBoxBackground
            };
            button.FlatAppearance.BorderSize = 2;
            return button;
        }

        public static Panel GetPanelWithLabel(string text, int sizeFont, Size size, Point location)
        {
            Label label = new()
            {
                Text = text,
                Font = new("Times New Roman", sizeFont, FontStyle.Regular),
                Size = size,
                Location = new(2, 2)
            };

            Panel panel = new()
            {
                Location = location,
                Size = size
            };
            panel.Controls.Add(label);
            panel.Paint += LabelBorderPaint;

            return panel;
        }

        public static Panel GetPanelWithBorders(Size size, Point location)
        {
            Panel panel = new()
            {
                Location = location,
                Size = size
            };
            panel.Paint += BorderPaint;

            return panel;
        }

        private static void LabelBorderPaint(object sender, PaintEventArgs e)
        {
            var panel = (Panel)sender;
            Pen pen = new(Colors.Foreground, 2F);
            e.Graphics.DrawLine(pen, new(1, 1), new(panel.Width, 1));
            e.Graphics.DrawLine(pen, new(1, 1), new(1, panel.Height));
        }

        private static void BorderPaint(object sender, PaintEventArgs e)
        {
            var panel = (Panel)sender;
            Pen pen = new(Colors.Foreground, 2F);
            e.Graphics.DrawRectangle(pen, 1, 1, panel.Width - 2, panel.Height - 2);
        }

        public static Button GetLinkButton(string text, int sizeFont, Size size, Point location)
        {
            Button button = new()
            {
                Text = text,
                Font = new("Times New Roman", sizeFont, FontStyle.Regular),
                Size = size,
                Location = location,
                FlatStyle = FlatStyle.Flat,
                ForeColor = Colors.Foreground,
                BackColor = Colors.Background
            };
            button.FlatAppearance.BorderSize = 0;

            return button;
        }

        public static ComboBox GetComboBox<T>(IEnumerable<T> collection, int fontSize, Size size, Point location)
        {
            ComboBox comboBox = new()
            {
                Font = new("Times New Roman", fontSize, FontStyle.Regular),
                Size = size,
                Location = location,
                BackColor = Colors.Background
            };
            comboBox.Items.AddRange(collection.Select(item => item.ToString()).ToArray());
            return comboBox;
        }

        public static CheckBox GetCheckBox(string text, bool isChecked, Size size, Point location)
        {
            return new()
            {
                Font = new("Times New Roman", 12),
                Text = text,
                CheckState = isChecked ? CheckState.Checked : CheckState.Unchecked,
                Size = size,
                Location = location
            };
        }

        public static Panel GetPanelWithChart(List<(string, int)> chartValues, Size size, Point location)
        {
            Panel panel = new()
            {
                Size = size,
                Location = location
            };

            Size spaceSize = new(size.Width - 70, size.Height - 100);

            PictureBox pictureSpace = new()
            {
                Size = spaceSize,
                Location = new(20, 0),
                Image = new Bitmap(spaceSize.Width, spaceSize.Height)
            };

            var maxValue = (double)chartValues.Select(x => x.Item2).Max();
            var elementsAmount = chartValues.Count;

            // 15: расстояние от левого края pictureSpace до первого столбика
            // 20: расстояние от правого края pictureSpace до последнего столбика
            // 5: расстояние между столбиками
            var columnWidth = (spaceSize.Width - 15 - 20 - ((elementsAmount - 1) * 5)) / (double)elementsAmount;
            // 25: расстояние от нижнего края pictureSpace оси x
            // 20: расстояние от верхнего края pictureSpace до столбика с максимальным значением
            var columnMaxHeight = spaceSize.Height - 25.0 - 20.0;
            
            var labelWidth = columnWidth;
            var labelHeight = 20;

            for (int i = 0; i < elementsAmount; ++i)
            {
                panel.Controls.Add(new Label()
                {
                    Text = chartValues[i].Item1,
                    Size = new((int)labelWidth, labelHeight),
                    Location = new(35 + i * ((int)columnWidth + 5), spaceSize.Height - 23)
                });
                panel.Controls.Add(new Label()
                {
                    Text = chartValues[i].Item2.ToString(),
                    Size = new(20, 20),
                    Location = new(0, (int)(20 - 10 + columnMaxHeight - ((chartValues[i].Item2 / maxValue) * columnMaxHeight)))
                });

                Rectangle column = new((int)(15 + i * (columnWidth + 5)), (int)(20 + columnMaxHeight - ((chartValues[i].Item2 / maxValue) * columnMaxHeight)), (int)(columnWidth), (int)(((chartValues[i].Item2 / maxValue) * columnMaxHeight)));
                using var g = Graphics.FromImage(pictureSpace.Image);

                // Отметки на оси ординат (y)
                Pen pen = new(Colors.Foreground, 2);
                g.DrawLine(pen, new(5, column.Y), new(15, column.Y));

                // Отрисовываем столбик
                g.FillRectangle(new SolidBrush(Colors.TextBoxBackground), column);
                g.DrawRectangle(pen, column);
            }

            DrawCoordinates(pictureSpace);

            panel.Controls.Add(pictureSpace);
            return panel;
        }

        private static void DrawCoordinates(PictureBox pictureSpace)
        {
            Pen pen = new(Colors.Foreground, 2);

            using var g = Graphics.FromImage(pictureSpace.Image);

            // Ось ордината (y)
            // Стрелки
            g.DrawLine(pen, new Point(5, 15), new Point(10, 0));
            g.DrawLine(pen, new Point(15, 15), new Point(10, 0));
            // Координатная линия
            g.DrawLine(pen, new Point(10, 0), new Point(10, pictureSpace.Height - 25));

            // Ось абсцис (x)
            // Стрелки
            g.DrawLine(pen, new Point(pictureSpace.Width - 15, pictureSpace.Height - 25 - 5), new Point(pictureSpace.Width, pictureSpace.Height - 25));
            g.DrawLine(pen, new Point(pictureSpace.Width - 15, pictureSpace.Height - 25 + 5), new Point(pictureSpace.Width, pictureSpace.Height - 25));
            // Координатная линия
            g.DrawLine(pen, new Point(10, pictureSpace.Height - 25), new Point(pictureSpace.Width, pictureSpace.Height - 25));
        }
    }
}
