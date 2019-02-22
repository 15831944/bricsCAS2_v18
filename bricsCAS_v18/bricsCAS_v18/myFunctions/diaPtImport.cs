using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CAS.myFunctions
{
    public partial class dlgPtImport : Form
    {
        private List<int> _lsErrorLine = new List<int>();

        public dlgPtImport()
        {
            InitializeComponent();
        }

        public List<int> LsErrorLine
        {
            set { _lsErrorLine = value; }
        }

        private void DlgPtImport_Load(object sender, EventArgs e)
        {

        }
    }
}
