using System.Windows.Forms;
using System;
using System.Threading;
using System.Drawing;

namespace SkinMaker
{
    public partial class MainWındow : Form
    {
        public static readonly string[] Dirs = new string[] { "Left", "Down", "Up", "Right" };

        Thread thread;

        public MainWındow()
        {
            InitializeComponent();
            thread = new Thread(new ThreadStart(RenderThread));
            thread.Start();
        }

        void MaınWındow_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
        }

        void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch(comboBox1.SelectedIndex)
            {
                case 0: CharacterTreeSetup(); break;
            }

            canmiss.Visible = comboBox1.SelectedIndex == 0;
        }

        void CharacterTreeSetup()
        {
            treeView1.Nodes.Clear();

            TreeNode rootNode = new TreeNode("Character Skin");

            TreeNode idle = new TreeNode("Idle");
            TreeNode hit = new TreeNode("Hit");
            TreeNode miss = new TreeNode("Miss");

            for (int i = 0; i < 4; i++)
            {
                hit.Nodes.Add(new TreeNode(Dirs[i]));
                miss.Nodes.Add(new TreeNode(Dirs[i]));
            }

            rootNode.Nodes.Add(idle);
            rootNode.Nodes.Add(hit);
            if(canmiss.Checked) rootNode.Nodes.Add(miss);

            treeView1.Nodes.Add(rootNode);

            treeView1.ExpandAll();
        }

        void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            CharacterTreeSetup();
        }

        void RenderThread()
        {
            while(true)
            {
                Thread.Sleep(50);
                Size size = pictureBox1.Size;
                Bitmap bitmap = new Bitmap(size.Width, size.Height);
                Graphics g = Graphics.FromImage(bitmap);
                g.FillRectangle(Brushes.Black, new RectangleF(0, 0, size.Width, size.Height));
                
                pictureBox1.Image = bitmap;
            }
        }
    }
}
