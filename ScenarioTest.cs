using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LiveRoulette
{
    public partial class ScenarioTest : Form
    {
        

        public ScenarioTest()
        {
            InitializeComponent();
        }


        private void comboScenario_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<Criteria.ICriteria> m_lstCriterias = new List<Criteria.ICriteria>();
            ComboBox cb = (ComboBox)sender;
            string filename = Directory.GetCurrentDirectory() + "\\TestScript\\" + cb.Text + ".txt";
            if (!File.Exists(filename))
            {
                MessageBox.Show($"Not Exist {filename} File");
                return;
            }
            m_lstCriterias.Add(new Criteria.ANumbers());
            m_lstCriterias.Add(new Criteria.BConsecutiveNumbers());
            m_lstCriterias.Add(new Criteria.CColors());
            m_lstCriterias.Add(new Criteria.DOddEven());
            m_lstCriterias.Add(new Criteria.EHalfs());
            m_lstCriterias.Add(new Criteria.FDozen());
            m_lstCriterias.Add(new Criteria.GColumns());
            m_lstCriterias.Add(new Criteria.H2Dozens());
            m_lstCriterias.Add(new Criteria.I2Columns());
            m_lstCriterias.Add(new Criteria.JConsecutiveRB());
            m_lstCriterias.Add(new Criteria.KConsecutiveColours());
            m_lstCriterias.Add(new Criteria.LConsecutiveOE());
            m_lstCriterias.Add(new Criteria.MConsecutiveFS());
            m_lstCriterias.Add(new Criteria.NConsecutiveDozen());
            m_lstCriterias.Add(new Criteria.OConsecutiveDozenIndividual());
            m_lstCriterias.Add(new Criteria.PConsecutiveColumn());
            m_lstCriterias.Add(new Criteria.QConsecutiveColumnIndividual());

            GlobalData.SetCurrentBalance(550);

            string[] text = File.ReadAllLines(filename);
            int size = text.Length;
            int i;
            this.lstInput.Items.Clear();
            this.lstOutput.Items.Clear();
            this.lstView_Stats.Items.Clear();
            int algo_index = comboScenario.SelectedIndex;
            m_lstCriterias[algo_index].SetInitSequence();
            for (i = 0; i < size; i++)
            {
                string inData = $"In{i + 1} : " + text[i];
                this.lstInput.Items.Add(inData);
                string[] number_arr = text[i].Split(',');
                GlobalData.glstTestNumbers.Clear();

                for (int j = 0; j < number_arr.Length; j++)
                {
                    GlobalData.glstTestNumbers.Add((Global.BET_SPOT)int.Parse(number_arr[j]));
                }

                /*if (i > 3)
                    GlobalData.SetCurrentBalance(450);*/
                
                Global.BET_ITEMS result;
                if (number_arr.Length < 11)
                    result = new Global.BET_ITEMS();
                else
                    result = m_lstCriterias[algo_index].CheckBetPlaces(GlobalData.glstTestNumbers, GlobalData.GetCurrentBalance());

                string strIt = $"Out{i + 1} : {m_lstCriterias[algo_index].GetCriteriaName()}, Count {result.placeCount}";
                
                this.lstOutput.Items.Add(strIt);
                if (result.placeCount == 0)
                    continue;

                for (int j = 0; j < result.arrBetSpots.Count(); j++)
                {
                    string strAdd = $"           {Global.GetBetSpotString(result.arrBetSpots[j])}, Stake {result.betStake[j]}";
                    this.lstOutput.Items.Add(strAdd);
                }

            }
        }

        private void lstInput_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListBox lstInput = (ListBox)sender;
            int index = lstInput.SelectedIndex;
            GlobalData.glstTestNumbers.Clear();
            string line = (string)lstInput.SelectedItem;
            if (line == "" || line == null)
                return;
            line = line.Replace(':', ',');
            string[] number_arr = line.Split(',');
            for (int j = 1; j < number_arr.Length; j++)
            {
                GlobalData.glstTestNumbers.Add((Global.BET_SPOT)int.Parse(number_arr[j]));
            }

            lstView_Stats.Items.Clear();
            for (int i = 0; i < GlobalData.glstTestNumbers.Count; i++)
            {
                ListViewItem lvItem = new ListViewItem((i + 1).ToString());

                for (int j = 1; j < lstView_Stats.Columns.Count; j++)
                    lvItem.SubItems.Add("");

                lvItem.UseItemStyleForSubItems = false;
                Global.BET_SPOT betNumber = GlobalData.glstTestNumbers[i];
                Global.BET_SPOT betColor = Global.GetBetColor(betNumber);

                lvItem.SubItems[colStatsNumber.Index].Text = Global.GetBetSpotString(betNumber);
                if (betColor == Global.BET_SPOT.COLOR_RED)
                    lvItem.SubItems[colStatsNumber.Index].ForeColor = Color.Red;
                else if (betColor == Global.BET_SPOT.ZERO)
                    lvItem.SubItems[colStatsNumber.Index].ForeColor = Color.Green;

                if (betNumber != Global.BET_SPOT.ZERO)
                {
                    Global.BET_SPOT betOddEven = Global.GetBetOddEven(betNumber);
                    lvItem.SubItems[colStatsOddEven.Index].Text = Global.GetBetSpotString(betOddEven);

                    Global.BET_SPOT betDozen = Global.GetBetDozen(betNumber);
                    lvItem.SubItems[colStatsDozen.Index].Text = Global.GetBetSpotString(betDozen);

                    Global.BET_SPOT betColumn = Global.GetBetColumn(betNumber);
                    lvItem.SubItems[colStatsColumn.Index].Text = Global.GetBetSpotString(betColumn);

                    Global.BET_SPOT betHalf = Global.GetBetHalf(betNumber);
                    lvItem.SubItems[colStatsHalf.Index].Text = Global.GetBetSpotString(betHalf);
                }
                lstView_Stats.Items.Add(lvItem);
            }
        }

        private void ScenarioTest_FormClosed(object sender, FormClosedEventArgs e)
        {
            Global.gbOperationMode = Global.LIVESCENARIO_MODE.LIVE_MODE;
        }

        private void ScenarioTest_Load(object sender, EventArgs e)
        {

        }
    }
}
