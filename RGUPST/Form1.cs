﻿using System;
using System.Windows.Forms;
using System.Xml;

namespace RGUPST
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void tsmExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void tsmOpen_Click(object sender, EventArgs e)
        {
            var dlg = new OpenFileDialog();
            dlg.Title = "Open XML Document";
            dlg.Filter = "XML Files (*.xml)|*.xml";
            dlg.FileName = Application.StartupPath + "\\..\\..\\example.xml";
            if (dlg.ShowDialog() == DialogResult.OK)
                try
                {
                    Cursor = Cursors.WaitCursor;

                    var xDoc = new XmlDocument();
                    xDoc.Load(dlg.FileName);

                    treeView1.Nodes.Clear();
                    treeView1.Nodes.Add(new
                        TreeNode(xDoc.DocumentElement.Name));
                    var tNode = new TreeNode();
                    tNode = treeView1.Nodes[0];
                    addTreeNode(xDoc.DocumentElement, tNode);

                    treeView1.ExpandAll();
                }
                catch (XmlException xExc)
                    //Exception is thrown is there is an error in the Xml
                {
                    MessageBox.Show(xExc.Message);
                }
                catch (Exception ex) //General exception
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    Cursor = Cursors.Default; //Change the cursor back
                }
        }

        private void addTreeNode(XmlNode xmlNode, TreeNode treeNode)
        {
            XmlNode xNode;
            TreeNode tNode;
            XmlNodeList xNodeList;
            if (xmlNode.HasChildNodes) //The current node has children
            {
                xNodeList = xmlNode.ChildNodes;
                for (var x = 0; x <= xNodeList.Count - 1; x++)
                    //Loop through the child nodes
                {
                    xNode = xmlNode.ChildNodes[x];
                    treeNode.Nodes.Add(new TreeNode(xNode.Name));
                    tNode = treeNode.Nodes[x];
                    addTreeNode(xNode, tNode);
                }
            }
            else //No children, so add the outer xml (trimming off whitespace)
            {
                treeNode.Text = xmlNode.OuterXml.Trim();
            }
        }

        private void actionOnClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            switch (e.Node.Text)
            {
                case "":
                    return;
                case "1987":
                    WebViewShow();
                    break;
                case "USA":
                    PdfShowDocument();
                    break;
            }
        }

        private static void PdfShowDocument()
        {
            var pdfViewer = new PdfViewer();
            pdfViewer.ShowMeSomething(@"C:\Test\card1.pdf.pdf");
            pdfViewer.Show();
        }

        private static void WebViewShow()
        {
            var view = new Viewer();
            view.ShowMeSuperDuper(
                @"https://ru.wikipedia.org/wiki/%D0%97%D0%B0%D0%B3%D0%BB%D0%B0%D0%B2%D0%BD%D0%B0%D1%8F_%D1%81%D1%82%D1%80%D0%B0%D0%BD%D0%B8%D1%86%D0%B0");
            view.Show();
        }
    }
}