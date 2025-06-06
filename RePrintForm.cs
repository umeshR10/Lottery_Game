using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FinalTask
{
    public partial class RePrintForm : Form
    {
        private int _userId;
        private string _username;
        private decimal _balance;
        string connectionString = ConfigurationManager.ConnectionStrings["myConstr"].ConnectionString;
        public RePrintForm(int id, string username, decimal balance)
        {
            InitializeComponent();
            createEmptyColumns();
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            this.Hide();
            Dashboard db = new Dashboard(_userId,_username,_balance);
            db.Show();
        }

        private void createEmptyColumns()
        {
            // Clear any existing columns
            dataGridView.Columns.Clear();

            DataGridViewTextBoxColumn Username = new DataGridViewTextBoxColumn();
            Username.Name = "Username";
            Username.HeaderText = "Username";
            dataGridView.Columns.Add(Username);

            DataGridViewTextBoxColumn DrawTime = new DataGridViewTextBoxColumn();
            DrawTime.Name = "Draw Time";
            DrawTime.HeaderText = "Draw Time";
            dataGridView.Columns.Add(DrawTime);

            DataGridViewTextBoxColumn date = new DataGridViewTextBoxColumn();
            date.Name = "Date";
            date.HeaderText = "Date";
            dataGridView.Columns.Add(date);

            DataGridViewTextBoxColumn barcode = new DataGridViewTextBoxColumn();
            barcode.Name = "BarcodeNum";
            barcode.HeaderText = "BarcodeNum";
            dataGridView.Columns.Add(barcode);

            DataGridViewTextBoxColumn qty = new DataGridViewTextBoxColumn();
            qty.Name = "QTY";
            qty.HeaderText = "QTY";
            dataGridView.Columns.Add(qty);

            DataGridViewTextBoxColumn points = new DataGridViewTextBoxColumn();
            points.Name = "Points";
            points.HeaderText = "Points";
            dataGridView.Columns.Add(points);

            DataGridViewTextBoxColumn winamt = new DataGridViewTextBoxColumn();
            winamt.Name = "Win Amount";
            winamt.HeaderText = "Win Amount";
            dataGridView.Columns.Add(winamt);

            DataGridViewTextBoxColumn clmStatus = new DataGridViewTextBoxColumn();
            clmStatus.Name = "Claim Status";
            clmStatus.HeaderText = "Claim Status";
            dataGridView.Columns.Add(clmStatus);

            DataGridViewButtonColumn viewbtn = new DataGridViewButtonColumn();
            viewbtn.Name = "View";
            viewbtn.HeaderText = "View";
            viewbtn.Text = "View";
            viewbtn.UseColumnTextForButtonValue = true;
            dataGridView.Columns.Add(viewbtn);

            DataGridViewButtonColumn rePrint = new DataGridViewButtonColumn();
            rePrint.Name = "Re Print";
            rePrint.HeaderText = "Re Print";
            rePrint.Text = "Re Print";
            rePrint.UseColumnTextForButtonValue = true;
            dataGridView.Columns.Add(rePrint);
        }
    }
}
