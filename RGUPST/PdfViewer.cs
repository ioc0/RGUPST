﻿using System;
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
    public partial class PdfViewer : Form
    {
        public PdfViewer()
        {
            InitializeComponent();
        }

        public void ShowMeSomething(string s, int pageNumber)
        {
            this.axAcroPDF1.src = s;
            this.axAcroPDF1.setCurrentPage(pageNumber);
        }
    }
}
