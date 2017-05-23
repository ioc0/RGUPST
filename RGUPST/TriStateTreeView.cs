using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace RGUPST
{
    class TriStateTreeView:TreeView
    {
        public enum CheckedState
        {
            UnInitialised = -1,
            UnChecked,
            Checked,
            Mixed
        };

        public enum TriStateStyles
        {
            Standard = 0,
            Installer
        };

        private int _ignoreClickAction;

        public TriStateTreeView()
        {
            StateImageList = new ImageList();

            for (var i = 0; i < 3; i++)
            {
                var bmp = new Bitmap(16, 16);
                var chkGraphics = Graphics.FromImage(bmp);

                switch (i)
                {
                    case 0:
                        CheckBoxRenderer.DrawCheckBox(chkGraphics, new Point(0, 1),
                            CheckBoxState.UncheckedNormal);
                        break;
                    case 1:
                        CheckBoxRenderer.DrawCheckBox(chkGraphics, new Point(0, 1),
                            CheckBoxState.CheckedNormal);
                        break;
                    case 2:
                        CheckBoxRenderer.DrawCheckBox(chkGraphics, new Point(0, 1),
                            CheckBoxState.MixedNormal);
                        break;
                }

                StateImageList.Images.Add(bmp);
            }
        }

        [Category("Tri-State Tree View")]
        [DisplayName(@"Style")]
        [Description("Style of the Tri-State Tree View")]
        public TriStateStyles TriStateStyleProperty { get; set; } = TriStateStyles.Standard;

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            CheckBoxes = false; // Disable default CheckBox functionality if it's been enabled

            _ignoreClickAction++; // we're making changes to the tree, ignore any other change requests
            UpdateChildState(Nodes, (int)CheckedState.UnChecked, false, true);
            _ignoreClickAction--;
        }

        protected override void OnAfterCheck(TreeViewEventArgs e)
        {
            base.OnAfterCheck(e);

            if (_ignoreClickAction > 0)
            {
                return;
            }

            _ignoreClickAction++; // we're making changes to the tree, ignore any other change requests

            var tn = e.Node;
            tn.StateImageIndex = tn.Checked ? (int)CheckedState.Checked : (int)CheckedState.UnChecked;

            // force all children to inherit the same state as the current node
            UpdateChildState(e.Node.Nodes, e.Node.StateImageIndex, e.Node.Checked, false);

            // populate state up the tree, possibly resulting in parents with mixed state
            UpdateParentState(e.Node.Parent);

            _ignoreClickAction--;
        }

        protected override void OnAfterExpand(TreeViewEventArgs e)
        {
            base.OnAfterExpand(e);

            _ignoreClickAction++; // we're making changes to the tree, ignore any other change requests
            UpdateChildState(e.Node.Nodes, e.Node.StateImageIndex, e.Node.Checked, true);
            _ignoreClickAction--;
        }

        protected void UpdateChildState(TreeNodeCollection nodes, int stateImageIndex, bool Checked,
            bool changeUninitialisedNodesOnly)
        {
            foreach (TreeNode tnChild in nodes)
            {
                if (changeUninitialisedNodesOnly && tnChild.StateImageIndex != -1)
                {
                    continue;
                }

                tnChild.StateImageIndex = stateImageIndex;
                tnChild.Checked = Checked; // override 'checked' state of child with that of parent

                if (tnChild.Nodes.Count > 0)
                {
                    UpdateChildState(tnChild.Nodes, stateImageIndex, Checked, changeUninitialisedNodesOnly);
                }
            }
        }

        protected void UpdateParentState(TreeNode tn)
        {
            if (tn == null)
            {
                return;
            }

            var origStateImageIndex = tn.StateImageIndex;
            int unCheckedNodes = 0, checkedNodes = 0, mixedNodes = 0;

            foreach (TreeNode tnChild in tn.Nodes)
            {
                if (tnChild.StateImageIndex == (int)CheckedState.Checked)
                {
                    checkedNodes++;
                }
                else if (tnChild.StateImageIndex == (int)CheckedState.Mixed)
                {
                    mixedNodes++;
                    break;
                }
                else
                {
                    unCheckedNodes++;
                }
            }

            if (TriStateStyleProperty == TriStateStyles.Installer)
            {
                if (mixedNodes == 0)
                {
                    tn.Checked = unCheckedNodes == 0;
                }
            }

            if (mixedNodes > 0)
            {
                tn.StateImageIndex = (int)CheckedState.Mixed;
            }
            else if (checkedNodes > 0 && unCheckedNodes == 0)
            {
                if (tn.Checked)
                {
                    tn.StateImageIndex = (int)CheckedState.Checked;
                }
                else
                {
                    tn.StateImageIndex = (int)CheckedState.Mixed;
                }
            }
            else if (checkedNodes > 0)
            {
                tn.StateImageIndex = (int)CheckedState.Mixed;
            }
            else
            {
                if (tn.Checked)
                {
                    tn.StateImageIndex = (int)CheckedState.Mixed;
                }
                else
                {
                    tn.StateImageIndex = (int)CheckedState.UnChecked;
                }
            }

            if (origStateImageIndex != tn.StateImageIndex && tn.Parent != null)
            {
                UpdateParentState(tn.Parent);
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.KeyCode == Keys.Space)
            {
                SelectedNode.Checked = !SelectedNode.Checked;
            }
        }

        protected override void OnNodeMouseClick(TreeNodeMouseClickEventArgs e)
        {
            base.OnNodeMouseClick(e);

            var info = HitTest(e.X, e.Y);

            if (info.Location != TreeViewHitTestLocations.StateImage)
            {
                return;
            }

            var tn = e.Node;
            tn.Checked = !tn.Checked;
        }
    }
}

