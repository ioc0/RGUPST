using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RGUPST
{
    public partial class Viewer : Form
    {
        public Viewer()
        {
            InitializeComponent();
            
            
        }

        public void ShowMeSuperDuper(string s)
        {
            this.webBrowser1.Navigate(s);
        }
    }
}
