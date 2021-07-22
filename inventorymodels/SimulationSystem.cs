using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryModels
{
    public class SimulationSystem
    {
        public SimulationSystem()
        {
            DemandDistribution = new List<Distribution>();
            LeadDaysDistribution = new List<Distribution>();
            SimulationCases = new List<SimulationCase>();
            PerformanceMeasures = new PerformanceMeasures();
        }

        ///////////// INPUTS /////////////

        public int OrderUpTo { get; set; }
        public int ReviewPeriod { get; set; }
        public int NumberOfDays { get; set; }
        public int StartInventoryQuantity { get; set; }
        public int StartLeadDays { get; set; }
        public int StartOrderQuantity { get; set; }
        public List<Distribution> DemandDistribution { get; set; }
        public List<Distribution> LeadDaysDistribution { get; set; }

        ///////////// OUTPUTS /////////////

        public List<SimulationCase> SimulationCases { get; set; }
        public PerformanceMeasures PerformanceMeasures { get; set; }


        public void ReadInput(string path)
        {
            string[] lines = System.IO.File.ReadAllLines(path);
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i] == "") { continue; }
                else if (lines[i] == "OrderUpTo")
                {
                    i++;
                    this.OrderUpTo = int.Parse(lines[i]);
                }
                else if (lines[i] == "ReviewPeriod")
                {
                    i++;
                    this.ReviewPeriod = int.Parse(lines[i]);
                }
                else if (lines[i] == "StartInventoryQuantity")
                {
                    i++;
                    this.StartInventoryQuantity = int.Parse(lines[i]);
                }
                else if (lines[i] == "StartLeadDays")
                {
                    i++;
                    this.StartLeadDays = int.Parse(lines[i]);
                }
                else if (lines[i] == "StartOrderQuantity")
                {
                    i++;
                    this.StartOrderQuantity = int.Parse(lines[i]);
                }
                else if (lines[i] == "NumberOfDays")
                {
                    i++;
                    this.NumberOfDays = int.Parse(lines[i]);
                }
                else if (lines[i] == "DemandDistribution")
                {
                    i++;
                    decimal cumProb = 0;
                    int min = 1;
                    int max;
                    while (lines[i] != "")
                    {
                        string[] line = lines[i].Split(',');
                        Distribution dis = new Distribution();
                        dis.Value = int.Parse(line[0]);
                        dis.Probability = decimal.Parse(line[1].TrimStart());
                        cumProb += dis.Probability;
                        dis.CummProbability = cumProb;
                        dis.MinRange = min;
                        max = (int)(cumProb * 100);
                        dis.MaxRange = max;
                        min = max + 1;
                        this.DemandDistribution.Add(dis);
                        i++;
                    }
                }
                else if (lines[i] == "LeadDaysDistribution")
                {
                    i++;
                    decimal cumProb = 0;
                    int min = 1;
                    int max;
                    while (i < lines.Length && lines[i] != "")
                    {
                        string[] line = lines[i].Split(',');
                        Distribution dis = new Distribution();
                        dis.Value = int.Parse(line[0]);
                        dis.Probability = decimal.Parse(line[1].TrimStart());
                        cumProb += dis.Probability;
                        dis.CummProbability = cumProb;
                        dis.MinRange = min;
                        max = (int)(cumProb * 100);
                        dis.MaxRange = max;
                        min = max + 1;
                        this.LeadDaysDistribution.Add(dis);
                        i++;
                    }
                }

            }
        }


        public void FillTable()
        {
            int daysUntilOrderArrives = this.StartLeadDays;
            int dayWithinCycle=1;
            int shortage = 0;
            int cycle = 1;
            var rand = new Random();
            for (int i = 0; i < this.NumberOfDays; i++){
                SimulationCase row = new SimulationCase();
                row.Day = i + 1;
                row.Cycle = cycle;
                row.DayWithinCycle = dayWithinCycle;
                row.BeginningInventory = this.StartInventoryQuantity;
                row.RandomDemand = rand.Next(1, 100);
                row.Demand = Get_Demand(row.RandomDemand);
                if (row.BeginningInventory<row.Demand){
                    
                    row.EndingInventory = 0;
                    row.ShortageQuantity = row.Demand - row.BeginningInventory;
                    shortage += row.ShortageQuantity;
                }
                else{
                    row.EndingInventory = row.BeginningInventory - row.Demand;
                }

                if (row.EndingInventory < shortage){
                    shortage -= row.EndingInventory;
                    row.EndingInventory = 0;
                }
                else{
                    row.EndingInventory -= shortage;
                    shortage = 0;
                    
                }

                this.StartInventoryQuantity = row.EndingInventory;
                row.ShortageQuantity = shortage;
                if (daysUntilOrderArrives != 0){
                    daysUntilOrderArrives--;
                    if (daysUntilOrderArrives == 0){
                        this.StartInventoryQuantity += this.StartOrderQuantity;
                        this.StartOrderQuantity = 0;
                    }
                }


                if (this.ReviewPeriod == dayWithinCycle)
                {
                    row.OrderQuantity = this.OrderUpTo - row.EndingInventory + row.ShortageQuantity;
                    row.RandomLeadDays = rand.Next(1, 100);
                    row.LeadDays = Get_Lead(row.RandomLeadDays);
                    this.StartOrderQuantity += row.OrderQuantity;
                    dayWithinCycle = 0;
                    daysUntilOrderArrives = row.LeadDays;
                    cycle++;
                }
     

                row.DaysUntilOrderArrives = daysUntilOrderArrives;
                this
                    .PerformanceMeasures.EndingInventoryAverage += row.EndingInventory;
                this.PerformanceMeasures.ShortageQuantityAverage += row.ShortageQuantity;
                this.SimulationCases.Add(row);
                dayWithinCycle++;
            }
            this.PerformanceMeasures.EndingInventoryAverage /= this.NumberOfDays;
            this.PerformanceMeasures.ShortageQuantityAverage /= this.NumberOfDays;


        }

        private int Get_Demand(int randomNum)
        {
            foreach (Distribution dis in this.DemandDistribution)
            {
                if (randomNum <= dis.MaxRange)
                {
                    return dis.Value;
                }
            }
            return 0;
        }

        private int Get_Lead(int randomNum)
        {
            foreach (Distribution dis in this.LeadDaysDistribution)
            {
                if (randomNum <= dis.MaxRange)
                {
                    return dis.Value;
                }
            }
            return 0;
        }
    }

}
