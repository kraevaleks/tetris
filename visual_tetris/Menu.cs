using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace visual_tetris
{
    public partial class Menu : Form
    {
        public Menu()
        {
            InitializeComponent();
            playButton.Font = new Font(Program.Fonts.Families[0], 48);
            playButton.Location = new Point(Width / 2 - playButton.Width / 2, Height / 2 - playButton.Height / 2);
            authorLabel.Font = new Font(Program.Fonts.Families[0], authorLabel.Font.Size);
        }

        private void playButton_Click(object sender, EventArgs e)
        {
            GameScreen form = new GameScreen();
            form.FormClosed += delegate { this.Show(); };
            form.Show();
            Hide();
        }

        private void Menu_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                Close();
        }
    }
}
