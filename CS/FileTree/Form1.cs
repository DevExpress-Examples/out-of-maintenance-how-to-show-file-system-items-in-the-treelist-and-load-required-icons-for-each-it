using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using FileTreeList;

namespace FileTree
{
    public partial class Form1 : XtraForm
    {
        private TreeListFileHelper _helper;
        public Form1()
        {
            InitializeComponent();
            _helper = new TreeListFileHelper(treeList1);
        }
    }
}
