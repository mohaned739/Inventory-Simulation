using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using InventoryTesting;
using InventoryModels;

namespace InventorySimulation
{
    public partial class Input : Form
    {
        public Input()
        {
            InitializeComponent();
            this.system = new SimulationSystem();
         }
        SimulationSystem system;
        private void Form1_Load(object sender, EventArgs e)
        {
            string path  = "../../TestCases/TestCase1.txt";
            system.ReadInput(path);
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Order Up to", typeof(int));
            dataTable.Columns.Add("Review Period", typeof(int));
            dataTable.Columns.Add("Start Inventory Quantity", typeof(int));
            dataTable.Columns.Add("Start Lead Days", typeof(int));
            dataTable.Columns.Add("Start Order Quantity", typeof(int));
            dataTable.Columns.Add("Number of Days", typeof(int));
            dataTable.Rows.Add(system.OrderUpTo, system.ReviewPeriod, system.StartInventoryQuantity, system.StartLeadDays, system.StartOrderQuantity, system.NumberOfDays);
            dataGridView1.DataSource = dataTable;

            DataTable dataTable2 = new DataTable();
            dataTable2.Columns.Add("Demand", typeof(int));
            dataTable2.Columns.Add("Probability", typeof(decimal));
            dataTable2.Columns.Add("Cumulative Probability", typeof(decimal));
            dataTable2.Columns.Add("MinRange", typeof(int));
            dataTable2.Columns.Add("MaxRang", typeof(int));
            for (int i = 0; i < system.DemandDistribution.Count; i++)
            {
                dataTable2.Rows.Add(system.DemandDistribution[i].Value, system.DemandDistribution[i].Probability, system.DemandDistribution[i].CummProbability, system.DemandDistribution[i].MinRange, system.DemandDistribution[i].MaxRange);
            }
            dataGridView2.DataSource = dataTable2;
            DataTable dataTable3 = new DataTable();
            dataTable3.Columns.Add("Demand", typeof(int));
            dataTable3.Columns.Add("Probability", typeof(decimal));
            dataTable3.Columns.Add("Cumulative Probability", typeof(decimal));
            dataTable3.Columns.Add("MinRange", typeof(int));
            dataTable3.Columns.Add("MaxRang", typeof(int));
            for (int i = 0; i < system.LeadDaysDistribution.Count; i++)
            {
                dataTable3.Rows.Add(system.LeadDaysDistribution[i].Value, system.LeadDaysDistribution[i].Probability, system.LeadDaysDistribution[i].CummProbability, system.LeadDaysDistribution[i].MinRange, system.LeadDaysDistribution[i].MaxRange);
            }
            dataGridView3.DataSource = dataTable3;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Results res = new Results(system);
            this.Hide();
            res.Show();
        }

        private void ReadData_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }
    }
}
