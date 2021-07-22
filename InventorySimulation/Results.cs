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
    public partial class Results : Form
    {
        public Results(SimulationSystem system)
        {
            InitializeComponent();
            this.system = system;
        }
        SimulationSystem system;
        private void Results_Load(object sender, EventArgs e)
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Day", typeof(int));
            dataTable.Columns.Add("Cycle", typeof(int));
            dataTable.Columns.Add("Day Within Cycle", typeof(int));
            dataTable.Columns.Add("Beginning Inventory", typeof(int));
            dataTable.Columns.Add("Random Digits for Demand", typeof(int));
            dataTable.Columns.Add("Demand", typeof(int));
            dataTable.Columns.Add("Ending Inventory", typeof(int));
            dataTable.Columns.Add("Shortage Quantity", typeof(int));
            dataTable.Columns.Add("Order Quantity", typeof(int));
            dataTable.Columns.Add("Random Digits for Lead time", typeof(int));
            dataTable.Columns.Add("Lead Time", typeof(int));
            dataTable.Columns.Add("DaysUntilOrderArrives", typeof(int));
            system.FillTable();
            dataGridView1.DataSource = dataTable;
            foreach (SimulationCase row in system.SimulationCases)
            {
                dataTable.Rows.Add(row.Day, row.Cycle, row.DayWithinCycle, row.BeginningInventory,row.RandomDemand,row.Demand, row.EndingInventory, row.ShortageQuantity, row.OrderQuantity, row.RandomLeadDays,row.LeadDays,row.DaysUntilOrderArrives);
            }
            textBox1.Text = system.PerformanceMeasures.EndingInventoryAverage.ToString();
            textBox2.Text = system.PerformanceMeasures.ShortageQuantityAverage.ToString();
            string result = TestingManager.Test(system, Constants.FileNames.TestCase1);
            MessageBox.Show(result);
        }

       
        private void Results_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

    }
}
