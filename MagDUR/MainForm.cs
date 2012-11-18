using System.Data;
using System.Data.SqlClient;
using System.IO;
using System;
using System.Windows.Forms;
using MagDUR.Classes;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Syncfusion.Windows.Forms.Tools;

namespace MagDUR
{
    public partial class MainForm : RibbonForm
    {
        // Connect to the local, default instance of SQL Server.
        private readonly Server _server =
            new Server(
                new ServerConnection(new SqlConnection(@"Server=MOBILNY;Database=;User Id=sa;Password=pass;")));

        private readonly DataSet ds = new DataSet();

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (File.Exists("MyTreeView.xml"))
            {
                treeView.Nodes.Clear();
                var serializer = new TreeViewSerializer();
                serializer.DeserializeTreeView(treeView, "MyTreeView.xml");
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            var serializer = new TreeViewSerializer();
            serializer.SerializeTreeView(treeView, "MyTreeView.xml");
        }

        private void rbtnExit_Click(object sender, EventArgs e)
        {
            var serializer = new TreeViewSerializer();
            serializer.SerializeTreeView(treeView, "MyTreeView.xml");

            Close();
        }

        private void rbtnOpenDB_Click(object sender, EventArgs e)
        {
            var openBase = new OpenBase(this);
            openBase.Show();
        }

        private void treeView_BeforeSelect(object sender, TreeViewAdvCancelableSelectionEventArgs args)
        {
            if (treeView.SelectedNode.Parent != null)
            {
                ds.Tables.Clear();

                if (!_server.Databases.Contains(treeView.SelectedNode.Parent.Text))
                {
                    grid.DataSource = null;
                    MessageBox.Show(@"Baza o podanej nazwie nie istnieje.");
                }
                else
                {
                    var connection =
                        new SqlConnection(
                            string.Format(@"Server=MOBILNY;Database={0};User Id=sa;Password=pass;",
                                          treeView.SelectedNode.Parent.Text));

                    var adapter =
                                new SqlDataAdapter(
                                    "Select id, box, nazwa, nr_ident_dostawca, nr_ident_producent, stan_mag, stan_min, kod, dostawca, sekcja, data from " +
                                    treeView.SelectedNode.Text, connection);
                    adapter.Fill(ds, treeView.SelectedNode.Text);
                    grid.DataSource = ds.Tables[0];
                }
            }
        }
    }
}
