using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace Окно_КИ_Переверни_фишки
{
    public partial class Form1 : Form
    {
        // Двумерный массив для работы с кнопками как с матрицей
        Button[,] chips = new Button[4, 4];

        // Цвета теперь ЧБ
        Color color1 = Color.White;
        Color color2 = Color.Black;

        Random rnd = new Random();
        int timeLeft = 600;
        bool gameStarted = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Инициализируем матрицу 4x4, находя кнопки по их именам button1...button16
            for (int i = 1; i <= 16; i++)
            {
                // Вычисляем индексы "по-программистски" (0-3)
                int r = (i - 1) / 4;
                int c = (i - 1) % 4;

                // Ищем кнопку на форме по имени "button" + номер
                Control[] found = this.Controls.Find("button" + i, true);
                if (found.Length > 0 && found[0] is Button btn)
                {
                    chips[r, c] = btn;

                    // Делаем кнопку круглой
                    GraphicsPath path = new GraphicsPath();
                    path.AddEllipse(0, 0, btn.Width, btn.Height);
                    btn.Region = new Region(path);

                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderSize = 0;
                }
            }

            GenerateField();
        }

        private void GenerateField()
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (chips[i, j] == null) continue;

                    int randColor = rnd.Next(0, 2);
                    chips[i, j].BackColor = (randColor == 0) ? color1 : color2;

                    // Настраиваем цвета при наведении, чтобы фишка не "мигала" другим цветом
                    UpdateSelectionColors(chips[i, j]);
                }
            }
        }

        private void Chip_Click(object sender, EventArgs e)
        {
            if (!gameStarted) return;

            Button clickedBtn = (Button)sender;
            int row = -1, col = -1;

            // Определяем координаты нажатой кнопки в нашей матрице
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (chips[i, j] == clickedBtn)
                    {
                        row = i;
                        col = j;
                        break;
                    }
                }
            }

            if (row != -1 && col != -1)
            {
                Flip(row, col);      // Центр
                Flip(row - 1, col);  // Верх
                Flip(row + 1, col);  // Низ
                Flip(row, col - 1);  // Лево
                Flip(row, col + 1);  // Право
            }

            CheckWin();
        }

        private void Flip(int r, int c)
        {
            // Проверка границ массива
            if (r >= 0 && r < 4 && c >= 0 && c < 4)
            {
                // Инверсия ЧБ
                chips[r, c].BackColor = (chips[r, c].BackColor == color1) ? color2 : color1;
                UpdateSelectionColors(chips[r, c]);
            }
        }

        private void UpdateSelectionColors(Button btn)
        {
            btn.FlatAppearance.MouseDownBackColor = btn.BackColor;
            btn.FlatAppearance.MouseOverBackColor = btn.BackColor;
        }

        private void CheckWin()
        {
            Color firstColor = chips[0, 0].BackColor;
            bool win = true;

            foreach (Button btn in chips)
            {
                if (btn.BackColor != firstColor)
                {
                    win = false;
                    break;
                }
            }

            if (win)
            {
                gameTimer.Stop();
                gameStarted = false;
                MessageBox.Show("Задача решена!", "Победа", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // --- Кнопки управления ---

        private void btnStart_Click_1(object sender, EventArgs e)
        {
            gameStarted = true;
            gameTimer.Start();
        }

        private void btnNewGame_Click_1(object sender, EventArgs e)
        {
            gameTimer.Stop();
            timeLeft = 600;
            lblTimer.Text = "10:00";
            GenerateField();
            gameStarted = true;
            gameTimer.Start();
        }

        private void btnCancel_Click_1(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Действительно хотите выйти?", "Выход", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                Close();
            }
        }

        private void gameTimer_Tick_1(object sender, EventArgs e)
        {
            if (timeLeft > 0)
            {
                timeLeft--;
                int minutes = timeLeft / 60;
                int seconds = timeLeft % 60;
                lblTimer.Text = $"{minutes:00}:{seconds:00}";
            }
            else
            {
                gameTimer.Stop();
                gameStarted = false;
                MessageBox.Show("Время вышло!", "Конец игры", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void tableLayoutPanel1_CellPaint(object sender, TableLayoutCellPaintEventArgs e)
        {
            using (Pen pen = new Pen(Color.Black, 1))
            {
                Rectangle rect = e.CellBounds;
                e.Graphics.DrawLine(pen, rect.Left, rect.Bottom - 1, rect.Right, rect.Bottom - 1);
                e.Graphics.DrawLine(pen, rect.Right - 1, rect.Top, rect.Right - 1, rect.Bottom);
                if (e.Column == 0) e.Graphics.DrawLine(pen, rect.Left, rect.Top, rect.Left, rect.Bottom);
                if (e.Row == 0) e.Graphics.DrawLine(pen, rect.Left, rect.Top, rect.Right, rect.Top);
            }
        }
    }
}