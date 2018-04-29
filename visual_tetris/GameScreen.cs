using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace visual_tetris
{
    public partial class GameScreen : Form
    {
        public GameScreen()
        {
            InitializeComponent();
        }

        private void GameScreen_Load(object sender, EventArgs e)
        {
            gameField.Paint += GameField_Paint;
            nextPictureBox.Paint += NextPictureBox_Paint;

            m_bitmaps = new Bitmap[7];
            m_cellSize = Math.Min(gameField.Width / fieldWidth, gameField.Height / fieldHeight);
            UpdateBitmaps();
            
            m_game = new Game(fieldWidth, fieldHeight);

            m_timer = new Timer();
            m_timer.Interval = 500;
            m_timer.Tick += delegate { tick(); };
            m_timer.Start();

            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
            label2.Font = new Font(Program.Fonts.Families[0], label2.Font.Size);
            label3.Font = new Font(Program.Fonts.Families[0], label3.Font.Size);
            scoreLabel.Font = new Font(Program.Fonts.Families[0], scoreLabel.Font.Size);
            pauseButton.Font = new Font(Program.Fonts.Families[0], pauseButton.Font.Size);
            exitButton.Font = new Font(Program.Fonts.Families[0], exitButton.Font.Size);
        }

        private void NextPictureBox_Paint(object sender, PaintEventArgs e)
        {
            Figure f = m_game.NextFigure;
            if (f == null)
                return;
            int s = Math.Min((nextPictureBox.Width - 20) / 4, (nextPictureBox.Height - 20) / 4);
            Point start = new Point(10 + (nextPictureBox.Width - 20 - s * 4) / 2,
                                    10 + (nextPictureBox.Height - 20 - s * 4) / 2);
            for (int i = 0; i < f.Points.Length; ++i)
            {
                int x = f.Points[i].X;
                int y = f.Points[i].Y;
                Rectangle r = new Rectangle(start.X + x * s, start.Y + y * s, s, s);
                e.Graphics.DrawImage(m_bitmaps[(sbyte)f.Type], r);
            }
        }

        private void tick()
        {
            if (m_game.Fall() == false)
            {
                m_game.RemoveLine();
                scoreLabel.Text = m_game.Score.ToString();
                if (m_game.Generate() == true)
                    nextPictureBox.Refresh();
                else
                    gameOver();
            }
            gameField.Refresh();
        }

        private void gameOver()
        {
            m_timer.Stop();
            MessageBox.Show("Your score is " + m_game.Score + "!", "Game over");
            Close();
        }

        private void GameField_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(Pens.White, new Rectangle(field.X - 2, field.Y - 2, field.Width + 4, field.Height + 4));
            for (int y = 0; y < fieldHeight; ++y)
                for (int x = 0; x < fieldWidth; ++x)
                {
                    sbyte type = m_game.GetValue(x, y);
                    if (type == -1)
                        continue;
                    Rectangle rect = new Rectangle(field.X + x * m_cellSize, field.Y + y * m_cellSize, m_cellSize, m_cellSize);
                    e.Graphics.DrawImage(m_bitmaps[type], rect);
                }
            Figure f = m_game.CurrentFigure;
            if (f == null)
                return;
            for (int i = 0; i < f.Points.Length; ++i)
            {
                int x = f.Points[i].X;
                int y = f.Points[i].Y;
                Rectangle r = new Rectangle(field.X + x * m_cellSize, field.Y + y * m_cellSize, m_cellSize, m_cellSize);
                e.Graphics.DrawImage(m_bitmaps[(sbyte)f.Type], r);
            }
            if (m_active == false)
            {
                SolidBrush brush = new SolidBrush(Color.FromArgb(127, 127, 127, 127));
                e.Graphics.FillRectangle(brush, 0, 0, gameField.Width, gameField.Height);
                int size = gameField.Width / 15;
                SizeF s = e.Graphics.MeasureString("P A U S E", new Font(Program.Fonts.Families[0], size));
                e.Graphics.DrawString("P A U S E", new Font(Program.Fonts.Families[0], size), Brushes.White, gameField.Width / 2 - s.Width / 2, gameField.Height / 2 - s.Height / 2);
            }
        }
        
        public void UpdateBitmaps()
        {
            if (m_cellSize == 0)
                return;
            Size s = new Size(m_cellSize, m_cellSize);
            m_bitmaps[0] = new Bitmap(Properties.Resources.red, s);
            m_bitmaps[1] = new Bitmap(Properties.Resources.orange, s);
            m_bitmaps[2] = new Bitmap(Properties.Resources.yellow, s);
            m_bitmaps[3] = new Bitmap(Properties.Resources.green, s);
            m_bitmaps[4] = new Bitmap(Properties.Resources.lightblue, s);
            m_bitmaps[5] = new Bitmap(Properties.Resources.blue, s);
            m_bitmaps[6] = new Bitmap(Properties.Resources.pink, s);
        }

        private void GameScreen_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                pause();
            if (m_active == false)
                return;
            if (e.KeyCode == Keys.A)
                if (m_game.Move(-1))
                    gameField.Refresh();
            if (e.KeyCode == Keys.D)
                if (m_game.Move(1))
                    gameField.Refresh();
            if (e.KeyCode == Keys.S)
                tick();
            if (e.KeyCode == Keys.W)
                if (m_game.Rotate(m_game.CurrentFigure) == true)
                    gameField.Refresh();
        }
        
        const int fieldWidth = 10;
        const int fieldHeight = 16;

        Bitmap[] m_bitmaps;
        int m_cellSize;
        Game m_game;
        Timer m_timer;
        bool m_active = true;
        Rectangle field;

        private void exitButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void GameScreen_Resize(object sender, EventArgs e)
        {
            m_cellSize = Math.Min(gameField.Width / fieldWidth, gameField.Height / fieldHeight);
            field = new Rectangle((gameField.Width - m_cellSize * fieldWidth) / 2,
                                    (gameField.Height - m_cellSize * fieldHeight) / 2,
                                    m_cellSize * fieldWidth, m_cellSize * fieldHeight);
            UpdateBitmaps();
            gameField.Refresh();
        }

        private void pause()
        {
            m_active = !m_active;
            m_timer.Enabled = m_active;
            pauseButton.Text = m_active ? "Pause" : "Play";
            gameField.Refresh();
        }

        private void pauseButton_Click(object sender, EventArgs e)
        {
            pause();
        }
    }
}
