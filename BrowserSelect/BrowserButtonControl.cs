using System;
using System.Drawing;
using System.Windows.Forms;

namespace BrowserSelectMod
{
    //=============================================================================================================
    public partial class BrowserButtonControl : UserControl
    //=============================================================================================================
    {
        public BrowserModel browser;
        public bool CreateRule { get; set; } = false;

        //-------------------------------------------------------------------------------------------------------------
        public BrowserButtonControl(BrowserModel browser)
        //-------------------------------------------------------------------------------------------------------------
        {
            InitializeComponent();

            this.browser = browser;

            BrowserIcon.Image = browser.String2Icon();//.ToBitmap();
            BrowserIcon.SizeMode = PictureBoxSizeMode.Zoom;
        }

        //-------------------------------------------------------------------------------------------------------------
        public new event EventHandler Click
        //-------------------------------------------------------------------------------------------------------------
        {
            add
            {
                base.Click += value;
                foreach (Control control in Controls)
                {
                    control.Click += value;
                }
            }
            remove
            {
                base.Click -= value;
                foreach (Control control in Controls)
                {
                    control.Click -= value;
                }
            }
        }

        //-------------------------------------------------------------------------------------------------------------
        private void RuleButton_Click(object sender, EventArgs e)
        //-------------------------------------------------------------------------------------------------------------
        {
            CreateRule = true;
        }
    }
}
