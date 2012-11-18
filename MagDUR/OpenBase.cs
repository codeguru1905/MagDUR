using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Syncfusion.Windows.Forms.Tools;

namespace MagDUR
{
    public partial class OpenBase : Form
    {
        private readonly MainForm mainForm;

        //Connect to the local, default instance of SQL Swerver.
        private readonly Server _server =
            new Server(
                new ServerConnection(new SqlConnection(connectionString: @"Server=MOBILNY;Database=;User Id=sa;Password=pass;")));

        public OpenBase(MainForm mainForm)
        {
            this.mainForm = mainForm;
            InitializeComponent();
        }

        private void OpenBase_Load(object sender, EventArgs e)
        {
            foreach (Database db in _server.Databases)
            {
                cmbExistsDB.Items.Add(((Database)db).Name);
                foreach (Table tb in _server.Databases[db.Name].Tables)
                {
                    listBox.Items.Add(tb.Name);
                }
            }

            cmbExistsDB.Text = cmbExistsDB.Items[0].ToString();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            var node = new TreeNodeAdv(cmbExistsDB.Text.Trim());
            mainForm.treeView.Nodes.Add(node);

            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
