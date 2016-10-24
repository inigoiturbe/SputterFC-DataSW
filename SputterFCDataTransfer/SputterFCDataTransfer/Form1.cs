#region Usings
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.IO;
using System.Globalization;
#endregion

namespace SputterFCDataTransfer
{
    #region Structures
    public struct P1_Stc
    {
        public Int64 idP1;
        public Int64 idPos;
        public double X;
        public double Y;
    }
    public struct RunSec_Stc
    {
        public int StartIndex;
        public int EndIndex;
    } 
  
    public struct Table_Stc
    {
        public string Name;
        public List<Param_Stc> Params;
        public double X;
        public double Y;
    }
    public struct Param_Stc
    {
        public string Name;
        public List<Double> Values;
    }  

    public struct Index_Stc
    {
        public string ParamName;
        public int TableIndex;
    }

    #endregion

    public partial class Form1 : Form
    {
        #region Variables
        MySqlConnection Connection;
        public List<string> ParamList = new List<string>();
        public List<Param_Stc> FileData = new List<Param_Stc>();
        public double XFirst = 0, XLast = 0;

        public char DS;
        public Table_Stc[] Tables;
        public Table_Stc[] TablesNorm;
        public Index_Stc[] Indexes;
        public List<Double> Positions;
        public List<Double> Velocities;
        public List<DateTime> Dates;
        public List<Double> PositionsNorm;
        public List<Double> VelocitiesNorm;
        public List<DateTime> DatesNorm;
        public List<int> MinPos;
        public List<RunSec_Stc> RunSecs;
        public RunSec_Stc Selected;
        public int SamplingRate = 25;
        public double IntRangeX = 300, IntRangeY = 200;
        #endregion

        #region Initialize 
        public Form1()
        {
            InitializeComponent();
        }
        #endregion

        #region Collect Tables Data   
        private void Collect_Tables()
        {
            string sqlquery = "SELECT distinct COUNT(TableName) FROM fesputterfc_offsets";
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            MySqlCommand command = new MySqlCommand();
            command.CommandText = sqlquery;
            command.Connection = Connection;
            adapter.SelectCommand = command;
            object result = command.ExecuteScalar();
            if (result != null)
            {
                int r = Convert.ToInt32(result);
                Tables = new Table_Stc[r];
            }
            sqlquery = "SELECT distinct TableName,X,Y FROM fesputterfc_offsets";
            MySqlDataReader data;
            command.CommandText = sqlquery;
            command.Connection = Connection;
            adapter.SelectCommand = command;
            data = command.ExecuteReader();
            int count = 0;
            while (data.Read())
            {
                if (!data.IsDBNull(0))
                    Tables[count].Name = data.GetString(0);
                if (!data.IsDBNull(1))
                    Tables[count].X = data.GetDouble(1);
                if (!data.IsDBNull(2))
                    Tables[count].Y = data.GetDouble(2);
                count++;

            }
            data.Close();
            for (int i = 0; i < Tables.Length; i++)
            {
                Tables[i].Params = new List<Param_Stc>();

                sqlquery = "SELECT Param FROM fesputterfc_ref INNER JOIN fesputterfc_offsets ON fesputterfc_offsets.idfefcoffset = fesputterfc_ref.idfefcoffset WHERE fesputterfc_offsets.TableName = '" + Tables[i].Name + "'";
                command.CommandText = sqlquery;
                command.Connection = Connection;
                adapter.SelectCommand = command;
                data = command.ExecuteReader();
                while (data.Read())
                {
                    if (!data.IsDBNull(0))
                    {
                        Param_Stc Aux = new Param_Stc();
                        Aux.Name = data.GetString(0);
                        Tables[i].Params.Add(Aux);
                    }

                }
                data.Close();
            }
        }
        #endregion

        #region Program Load
        private void Form1_Load(object sender, EventArgs e)
        {
            DS = Convert.ToChar(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            Connect_BEDB();
            if (Connection.State == ConnectionState.Open)
            {
                Output_ListBox.Items.Add(DateTime.Now.ToString() + " LOTs list updated");
                Collect_Tables();
                //Interpolate(48);
                //Create_Interpolation_Table();
                //Create_Tables();
                Update_LOTs_List();
                //Read_Table_Fields();
                FirstLabel_Combo.SelectedIndex = 0;
                LastLabel_Combo.SelectedIndex = 0;
                FirstLabel_Num.Enabled = false;
                LastLabel_Num.Enabled = false;
            }

        }
        #endregion

        #region Create DB Tables        
        private void Create_Tables()
        {
            for (int i = 0; i < Tables.Length; i++)
            {
                string sqlquery = "CREATE TABLE " + Tables[i].Name + " (id" + Tables[i].Name + " INT NOT NULL AUTO_INCREMENT,";
                foreach (Param_Stc Param in Tables[i].Params)
                {
                    string Aux = Param.Name.Replace('.', '_');
                    sqlquery = sqlquery + Aux + " DOUBLE NULL DEFAULT NULL COMMENT '" + Param.Name + "',";
                }
                sqlquery = sqlquery + "idPos INT NULL DEFAULT NULL,  PRIMARY KEY(id" + Tables[i].Name + "))";
                MySqlDataAdapter adapter = new MySqlDataAdapter();
                MySqlCommand command = new MySqlCommand();
                command.CommandText = sqlquery;
                command.Connection = Connection;
                adapter.SelectCommand = command;
                command.ExecuteNonQuery();
                sqlquery = "ALTER TABLE " + Tables[i].Name + " ADD foreign key (idPos) references fepositions(idPos)";
                command.CommandText = sqlquery;
                command.Connection = Connection;
                adapter.SelectCommand = command;
                command.ExecuteNonQuery();
            }
        }
        #endregion

        #region Create Interpolation Table
        public void Create_Interpolation_Table()
        {
            string sqlquery = "CREATE TABLE fesputterfci (idfesputterfci INT NOT NULL AUTO_INCREMENT,";
            for (int i = 1; i < Tables.Length; i++)
            {
                foreach (Param_Stc Param in Tables[i].Params)
                {
                    string Aux = Param.Name.Replace('.', '_');
                    sqlquery = sqlquery + Aux + " DOUBLE NULL DEFAULT NULL COMMENT '" + Param.Name + "',";
                }
            }
            sqlquery = sqlquery + "idPos INT NULL DEFAULT NULL,idP1 INT NULL DEFAULT NULL,  PRIMARY KEY(idfesputterfci))";
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            MySqlCommand command = new MySqlCommand();
            command.CommandText = sqlquery;
            command.Connection = Connection;
            adapter.SelectCommand = command;
            command.ExecuteNonQuery();
            sqlquery = "ALTER TABLE fesputterfci ADD foreign key (idPos) references fepositions(idPos)";
            command.CommandText = sqlquery;
            command.Connection = Connection;
            adapter.SelectCommand = command;
            command.ExecuteNonQuery();
            sqlquery = "ALTER TABLE fesputterfci ADD foreign key (idP1) references fep1laser(idP1)";
            command.CommandText = sqlquery;
            command.Connection = Connection;
            adapter.SelectCommand = command;
            command.ExecuteNonQuery();
        }

        #endregion

        #region Search_Sections
        private void Search_Run_Sections()
        {
            RunSecs = new List<RunSec_Stc>();
            RunSec_Stc Aux = new RunSec_Stc(); ;
            Boolean Section = false;
            for (int i = 0; i < Positions.Count; i++)
            {
                if (!Section && Velocities[i] > 0)
                {
                    Aux = new RunSec_Stc();
                    Aux.StartIndex = i;
                    Section = true;
                }
                if (Section && Velocities[i] == 0)
                {
                    Aux.EndIndex = i;
                    RunSecs.Add(Aux);
                    Section = false;

                }
            }
        }
        #endregion

        #region DataGrid Rows create
        private void Set_DataGrid_Rows()
        {
            GV.Sections = new List<string[]>();
            GV.SecNames = new List<string>();
            int count = 1;
            foreach (RunSec_Stc Sec in RunSecs)
            {
                Double Length = Positions[Sec.EndIndex] - Positions[Sec.StartIndex];
                string[] row = { "Section " + count.ToString(), Dates[Sec.StartIndex].ToString(), Dates[Sec.EndIndex].ToString(), Length.ToString() };
                GV.Sections.Add(row);
                GV.SecNames.Add("Section " + count.ToString());
                count++;
            }
        }
        #endregion

        #region Normalize Values
        private void Normalize_Values()
        {
            PositionsNorm = new List<double>();
            DatesNorm = new List<DateTime>();
            VelocitiesNorm = new List<double>();
            for (int i = Selected.StartIndex; i < Selected.EndIndex; i = i + SamplingRate)
            {
                PositionsNorm.Add(Positions[i]);
                VelocitiesNorm.Add(Velocities[i]);
                DatesNorm.Add(Dates[i]);
            }
            TablesNorm = new Table_Stc[Tables.Length];
            for (int i = 0; i < Tables.Length; i++)
            {
                TablesNorm[i].Name = Tables[i].Name;
                TablesNorm[i].X = Tables[i].X;
                TablesNorm[i].Y = Tables[i].Y;
                TablesNorm[i].Params = new List<Param_Stc>();

                foreach (Param_Stc Param in Tables[i].Params)
                {

                    Param_Stc Aux = new Param_Stc();
                    Aux.Name = Param.Name;


                    if (Param.Values != null)
                    {
                        Aux.Values = new List<double>();
                        for (int j = Selected.StartIndex; j < Selected.EndIndex; j = j + SamplingRate)
                        {
                            Aux.Values.Add(Calculate_Media(Param.Values, j, SamplingRate));
                        }
                    }
                    TablesNorm[i].Params.Add(Aux);
                }
            }

        }
        #endregion

        #region Calcualte Media
        private Double Calculate_Media(List<Double> Values, int Index, int Count)
        {
            Double res = 0;
            for (int i = Index; i < Index + 50; i++)
            {
                res = res + Values[i];
            }
            res = res / Count;
            return res;
        }
        #endregion

        #region Reference Positions
        private void Reference_Positions()
        {
            double ScalingFactor = (XFirst - XLast) / (1000 * (PositionsNorm[PositionsNorm.Count - 1] - PositionsNorm[0]));
            double InitPos = PositionsNorm[0];
            for (int i = 0; i < PositionsNorm.Count; i++)
            {
                PositionsNorm[i] = XFirst - 1000 * (PositionsNorm[i] - InitPos) * ScalingFactor;
            }
        }
        #endregion

        #region Get LOT ID
        private Int64 Get_LOT_ID(string LOTNr)
        {
            Int64 idLOT = -1;
            string sqlquery = "SELECT idLOT FROM felots WHERE LOTNr='" + LOTNr + "'";
            MySqlDataReader data;
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            MySqlCommand command = new MySqlCommand();
            command.CommandText = sqlquery;
            command.Connection = Connection;
            adapter.SelectCommand = command;
            data = command.ExecuteReader();
            if (data.Read())
            {
                if (!data.IsDBNull(0))
                    idLOT = data.GetInt64(0);
            }
            data.Close();
            return idLOT;
        }
        #endregion

        #region Get Min Max
        private void Get_Min_Max(Int64 idLOT, ref double Min, ref double Max)
        {       

            for (int i = 1; i < Tables.Length; i++)
            {

                //Select min(fepositions.X), max(fepositions.X) FROM fesputterfc_sz2 INNER JOIN fepositions ON fesputterfc_sz2.idPos = fepositions.idPos WHERE fepositions.idLOT =
                string sqlquery = "Select min(fepositions.X), max(fepositions.X) FROM " + Tables[i].Name + " INNER JOIN fepositions ON " + Tables[i].Name + ".idPos = fepositions.idPos WHERE fepositions.idLOT =" + idLOT.ToString();
                MySqlDataReader data;
                MySqlDataAdapter adapter = new MySqlDataAdapter();
                MySqlCommand command = new MySqlCommand();
                command.CommandText = sqlquery;
                command.Connection = Connection;
                adapter.SelectCommand = command;
                data = command.ExecuteReader();
                if (data.Read())
                {
                    if (!data.IsDBNull(0))
                    {
                        double Aux = data.GetDouble(0);
                        if (i == 1)
                            Min = Aux;
                        else
                        {
                            if (Aux > Min)
                                Min = Aux;
                        }
                        
                    }
                    if (!data.IsDBNull(1))
                    {
                        double Aux = data.GetDouble(1);
                        if (i == 1)
                            Max = Aux;
                        else
                        {
                            if (Aux < Max)
                                Max = Aux;
                        }
                    }
                }
                data.Close();
            }
        }
        #endregion

        #region Interpolate
        private void Interpolate(Int64 idLOT)
        {
            Double Min = 0, Max = 0;
            Get_Min_Max(idLOT, ref Min, ref Max);
            List<P1_Stc> P1Lists = new List<P1_Stc>();
            string sqlquery = "SELECT fepositions.idPos,fep1laser.idP1,X,Y FROM fepositions INNER JOIN fep1laser ON fepositions.idPos = fep1laser.idPos WHERE X>=" + Min.ToString().Replace(',', '.') + " AND X<=" + Max.ToString().Replace(',', '.') + "  AND fepositions.idLOT=" + idLOT.ToString() + " ORDER BY X";
            MySqlDataReader data;
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            MySqlCommand command = new MySqlCommand();
            command.CommandText = sqlquery;
            command.Connection = Connection;
            adapter.SelectCommand = command;
            data = command.ExecuteReader();
            while (data.Read())
            {
                P1_Stc Aux = new P1_Stc();
                if (!data.IsDBNull(0))
                    Aux.idPos = data.GetInt64(0);
                if (!data.IsDBNull(1))
                    Aux.idP1 = data.GetInt64(1);
                if (!data.IsDBNull(2))
                    Aux.X = data.GetDouble(2);
                if (!data.IsDBNull(3))
                    Aux.Y = data.GetDouble(3);
                P1Lists.Add(Aux);
            }
            data.Close();

            TransferProgress.Value = 0;
            int Index = 0;
            foreach (P1_Stc P1 in P1Lists)
            {
                TransferProgress.Value = (100 * Index) / P1Lists.Count;
                List<Param_Stc> Parameters = new List<Param_Stc>();
                Get_Param_Names(ref Parameters);

                for (int i = 1; i < Tables.Length; i++)
                {
                    sqlquery = "SELECT ";
                    Boolean first = true;
                    foreach (Param_Stc Param in Tables[i].Params)
                    {
                        if (first)
                        {
                            sqlquery = sqlquery + Param.Name.Replace('.', '_');
                            first = false;
                        }
                        else
                            sqlquery = sqlquery + "," + Param.Name.Replace('.', '_');

                    }
                    if (Tables[i].Name == "fesputterfc_sz2")
                        sqlquery = sqlquery + ",SPEED";

                    sqlquery = sqlquery + " FROM " + Tables[i].Name + " INNER JOIN fepositions ON fepositions.idPos=" + Tables[i].Name + ".idPos INNER JOIN felots ON fepositions.idLOT=felots.idLOT WHERE felots.idLOT=" + idLOT.ToString() + "  AND fepositions.X BETWEEN " + (P1.X - IntRangeX).ToString().Replace(',', '.') + " AND " + (P1.X + IntRangeX).ToString().Replace(',', '.');
                    if (Tables[i].Y != 0)
                        sqlquery = sqlquery + " AND fepositions.Y BETWEEN " + (P1.Y - IntRangeY).ToString().Replace(',', '.') + " AND " + (P1.Y + IntRangeY).ToString().Replace(',', '.');
                    command.CommandText = sqlquery;
                    command.Connection = Connection;
                    adapter.SelectCommand = command;
                    data = command.ExecuteReader();
                    while (data.Read())
                    {
                        foreach (Param_Stc Param in Tables[i].Params)
                        {
                            Save_Val(ref Parameters, Param.Name, data.GetDouble(Param.Name.Replace('.', '_')));
                          
                           
                        }
                        if (Tables[i].Name == "fesputterfc_sz2")
                        {
                            Save_Val(ref Parameters, "WINDING.LineSPEEDPV", data.GetDouble("SPEED"));
                        }
                
                    }
                    data.Close();
                }
                int count = 0;
                string CommandString = "INSERT  INTO fesputterfci(";
                string CommandString1 = " VALUES(";
                Boolean first1 = true;
                foreach (Param_Stc Param in Parameters)
                {
                    if (Param.Values != null)
                    {                        
                        if(first1)
                        {
                            CommandString = CommandString + Param.Name.Replace('.', '_');
                            CommandString1 = CommandString1 + Get_Media(Param.Values).ToString().Replace(',', '.');
                            first1 = false;
                        }
                        else
                        {
                            CommandString = CommandString +","+ Param.Name.Replace('.', '_');
                            CommandString1 = CommandString1 + "," + Get_Media(Param.Values).ToString().Replace(',', '.');

                        }

                        //command.Parameters.Add(SQLParams[count]);
                    }                    
                     count++;
                }
                CommandString = CommandString + ",idPos,idP1)";
                CommandString1 = CommandString1 + ","+P1.idPos.ToString()+","+P1.idP1.ToString()+")";
                sqlquery = CommandString + CommandString1;
                command.CommandText = sqlquery;
                command.Connection = Connection;
                adapter.SelectCommand = command;
                command.ExecuteNonQuery();
                Index++;
            }
        }
        #endregion

        #region Get Media
        private double Get_Media(List<double> Values)
        {
            double res = 0;
            foreach(double Val in Values)
            {
                res = res + Val;
            }
            res = res / Values.Count;
            return res;
        }
        #endregion

        #region Save Value
        private void Save_Val(ref List<Param_Stc> Parameters, string ParName,double Value)
        {
            
            for(int i=0;i<Parameters.Count;i++)
            {
                if(Parameters[i].Name==ParName)
                {
                    Param_Stc Aux = Parameters[i];
                    if (Aux.Values == null)
                        Aux.Values = new List<double>();
                    Aux.Values.Add(Value);
                    Parameters[i] = Aux;
                    break;
                }
               

            }

        }
        #endregion

        #region Get Parameters Names
        private void Get_Param_Names(ref List<Param_Stc> Parameters)
        {
            for (int i = 1; i < Tables.Length; i++)
            {
                foreach(Param_Stc Param in Tables[i].Params)
                {
                    Param_Stc Aux = new Param_Stc();
                    Aux.Name = Param.Name;
                    Parameters.Add(Aux);
                }
                if (Tables[i].Name == "fesputterfc_sz2")
                {
                    Param_Stc Aux = new Param_Stc();
                    Aux.Name = "WINDING.LineSPEEDPV";
                    Parameters.Add(Aux);
                }

            }
        }
        #endregion

        #region Transfer Button        
        private void Transfer_button_Click(object sender, EventArgs e)
        {           
            if (Get_Label_Positions())
            {
                Transfer_button.Enabled = false;
                Delete_Button.Enabled = false;
                openFileDialog1.Title = "Select Sputter Data Files";
                openFileDialog1.InitialDirectory = @"P:\Documents\line IMP\23_IMP_OPERATIONS\03_Process\03_Process tracking\05_FC_Sputtering\2016";
                openFileDialog1.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                openFileDialog1.FilterIndex = 1;
                openFileDialog1.RestoreDirectory = true;
                openFileDialog1.Multiselect = false;
                openFileDialog1.FileName = "";
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {                       
                    Read_CSV_File(openFileDialog1.FileName);
                    Output_ListBox.Items.Add(DateTime.Now.ToString() + " Data from selected file collected");
                    Output_ListBox.Items.Add(DateTime.Now.ToString() + " Searching for Run Sections");
                    Search_Run_Sections();
                    if(RunSecs.Count>0)
                    {
                        //Get_Label_Positions();
                        Set_DataGrid_Rows();
                        Sec_Form SectionsForm = new Sec_Form();
                        DialogResult result = SectionsForm.ShowDialog();
                        if (result == DialogResult.OK)
                        {
                            result = MessageBox.Show("Are you sure you want to transfer FC Sputter data for LOT: " + LOTS_list.Text, "Transfer confirmation", MessageBoxButtons.YesNo);
                            if (result == DialogResult.Yes)
                            {
                                Selected = RunSecs[GV.SelSec];                                        
                                Normalize_Values();
                                Output_ListBox.Items.Add(DateTime.Now.ToString() + " Referencing positions");
                                Reference_Positions();
                                Output_ListBox.Items.Add(DateTime.Now.ToString() + " Tranfering data to DB");
                                Transfer_DB();
                                Output_ListBox.Items.Add(DateTime.Now.ToString() + " Interpolating data");
                                Interpolate(Get_LOT_ID((String)LOTS_list.SelectedItem));
                                TransferProgress.Value = 0;
                                Output_ListBox.Items.Add(DateTime.Now.ToString() + " Data Transfer Finished");
                            }
                        }
                    }                        
                }
                
                Transfer_button.Enabled = true;
                Delete_Button.Enabled = true;
            }
              

        }
    
        #endregion

        #region LOTs Listbox
        private void LOTS_list_SelectedIndexChanged(object sender, EventArgs e)
        {
            String LOTNr = (String)LOTS_list.SelectedItem;            
            Update_SubModules_List(LOTNr);
            Output_ListBox.Items.Add(DateTime.Now.ToString() + " Label lists updated for LOT Nr: " + LOTNr);
        }
        #endregion        

        #region Save Value
        public Boolean Save_Value(int Index, double Value)
        {
            int count = 0;
            foreach (Param_Stc Param in Tables[Indexes[Index].TableIndex].Params)
            {
                if (Param.Name == Indexes[Index].ParamName)
                {
                    Param_Stc Aux = Param;
                    if (Aux.Values == null)
                        Aux.Values = new List<double>();
                    Aux.Values.Add(Value);
                    Tables[Indexes[Index].TableIndex].Params[count] = Aux;
                    return true;
                }
                count++;
            }
            return false;
        }
        #endregion

        #region Get Indexes
        public Boolean Get_Indexes(string word,int Index)
        {
           
            for(int i=0;i<Tables.Length;i++)
            {
                foreach(Param_Stc Param in Tables[i].Params)
                {
                    if(Param.Name== word)
                    {
                        Indexes[Index].ParamName = word;
                        Indexes[Index].TableIndex = i;
                        return true;
                    }
                }
            }
            return false;
        }
        #endregion

        #region Read CSV
        private Boolean Read_CSV_File(string Path)
        {

            Positions = new List<double>();
            Velocities = new List<double>();
            Dates = new List<DateTime>();
            Boolean DataOK = true;
            FileData = new List<Param_Stc>();
            string[] lines = File.ReadAllLines(Path);
            int LineIndex = 0;
            double prev_pos = -1;
            foreach (string line in lines)
            {
                int count = line.Split(',').Length - 1;
                int count1 = line.Split(';').Length - 1;
                string[] words = { };
                if (count > count1)
                    words = line.Split(',');
                else if (count < count1)
                    words = line.Split(';');
                switch (LineIndex)
                {
                    case 0:
                        {
                            Indexes = new Index_Stc[words.Length];
                            for(int i=0;i<words.Length;i++)
                            {
                                Get_Indexes(words[i], i);
                            }
                            break;
                        }

                    default: 
                        {
                            
                            double Vel;
                            if (DS == (char)44)
                                Vel = double.Parse(words[3].Replace('.', ','));
                            else
                                Vel = double.Parse(words[3]);
                            Velocities.Add(Vel);
                            double Pos;
                            if (DS == (char)44)
                                Pos = double.Parse(words[4].Replace('.', ','));
                            else
                                Pos = double.Parse(words[4]);
                            Positions.Add(Pos);
                                
                            DateTime Date = DateTime.Parse(words[1]);
                            Dates.Add(Date);
                            for (int i =5; i < words.Length; i++)
                            {
                                if(words[i]!="")
                                {
                                    if (DS == (char)44)
                                        Save_Value(i, double.Parse(words[i].Replace('.', ',')));
                                    else
                                        Save_Value(i, double.Parse(words[i]));
                                }                                 

                            }
                            
                            }
                            break;
                        }
                LineIndex++;
            }
              
            return DataOK;
        }
           
        
        
        #endregion      

        #region Connect DB
        public Boolean Connect_BEDB()
        {
            Connection = new MySqlConnection();
            Connection.ConnectionString = "server=FL-FILE01.flisom.local;user id=root;password=CuInGaSe2;port=3306;database=backenddatabase_v3.0;";
            try
            {
                Connection.Open();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion     

        #region Update LOTS list        
        private void Update_LOTs_List()
        {
            LOTS_list.Items.Clear();
            string sqlquery = "SELECT LOTNr FROM felots ORDER BY LOTNr";
            MySqlDataReader data;
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            MySqlCommand command = new MySqlCommand();
            command.CommandText = sqlquery;
            command.Connection = Connection;
            adapter.SelectCommand = command;
            data = command.ExecuteReader();
            while (data.Read())
            {
                if (!data.IsDBNull(0))
                    LOTS_list.Items.Add(data.GetString(0));
            }
            data.Close();
        }
        #endregion

        #region Update submodules list
        private void Update_SubModules_List(String LOTNr)
        {
            
            String PrevMod = "";
            FirstSubModules_Combo.Items.Clear();
            string sqlquery = "SELECT SubModuleName FROM fep1laser INNER JOIN fepositions ON fepositions.idPos=fep1laser.idMinPos INNER JOIN felots ON felots.idLOT=fepositions.idLOT WHERE felots.LOTNr='" + LOTNr + "' ORDER BY fepositions.X";
            MySqlDataReader data;
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            MySqlCommand command = new MySqlCommand();
            command.CommandText = sqlquery;
            command.Connection = Connection;
            adapter.SelectCommand = command;
            data = command.ExecuteReader();
            while (data.Read())
            {
                if (!data.IsDBNull(0))
                {
                    string Aux = data.GetString(0);
                    string[] parts = Aux.Split('-');
                    if (parts[0] != PrevMod)
                    {
                        PrevMod = parts[0];
                        FirstSubModules_Combo.Items.Add(PrevMod);
                        LastSubModules_Combo.Items.Add(PrevMod);
                    }
                }
            }
            data.Close();
        }
        #endregion
       
        #region Get label positions
        private Boolean Get_Label_Positions()
        {
            Boolean DataOK = false;
            string sqlquery = "";
            MySqlDataReader data;
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            MySqlCommand command = new MySqlCommand();
            if (FirstLabel_Combo.SelectedIndex==0)
            {
                sqlquery = "SELECT X FROM fepositions INNER JOIN fep1laser ON fepositions.idPos =fep1laser.idMaxPos WHERE fep1laser.SubModuleName='" + FirstSubModules_Combo.Text + "-1'";               
                command.CommandText = sqlquery;
                command.Connection = Connection;
                adapter.SelectCommand = command;
                data = command.ExecuteReader();
                if (data.Read())
                {
                    if (!data.IsDBNull(0))
                        XFirst = data.GetDouble(0);
                    DataOK = true;
                }
                else
                    DataOK = false;
                data.Close();
            }
            else
            {
                XFirst = (double)FirstLabel_Num.Value*1000;
                DataOK = true;
            }


            if (LastLabel_Combo.SelectedIndex == 0)
            {
                sqlquery = "SELECT X FROM fepositions INNER JOIN fep1laser ON fepositions.idPos =fep1laser.idMinPos WHERE fep1laser.SubModuleName='" + LastSubModules_Combo.Text + "-1'";
                command.CommandText = sqlquery;
                command.Connection = Connection;
                adapter.SelectCommand = command;
                data = command.ExecuteReader();
                if (data.Read())
                {
                    if (!data.IsDBNull(0))
                        XLast = data.GetDouble(0);
                    DataOK = true;
                }
                else
                    DataOK = false;
                data.Close();                
            }
            else
            {
                XLast=(double)LastLabel_Num.Value*1000;
                DataOK = true;
            }


            double Aux = 0;
            if (XLast>XFirst)
            {
                XLast = Aux;
                XFirst = XLast;
                XLast = Aux;

            }
            GV.LabelsDistance = XFirst- XLast  ;
            return DataOK;

        }
        #endregion

        #region Update LOT dates
        private void Update_LOT_Dates(Int64 idLOT, DateTime StartDate, DateTime EndDate)
        {
            DateTime Start = DateTime.Now;
            DateTime End = new DateTime(2014, 1, 1);
            string sqlquery = "SELECT FCStartDate,FCEndDate FROM felots WHERE idLOT=" + idLOT.ToString();
            MySqlDataReader data;
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            MySqlCommand command = new MySqlCommand();
            command.CommandText = sqlquery;
            command.Connection = Connection;
            adapter.SelectCommand = command;
            data = command.ExecuteReader();
            if (data.Read())
            {
                if (!data.IsDBNull(0))
                    Start = data.GetDateTime(0);
                if (!data.IsDBNull(1))
                    End = data.GetDateTime(1);

            }
            data.Close();

            if (StartDate > Start)
                StartDate = Start;
            if (EndDate < End)
                EndDate = End;
            sqlquery = "UPDATE felots SET FCStartDate='" + StartDate.Year.ToString() + "-" + StartDate.Month.ToString("00") + "-" + StartDate.Day.ToString("00") + " " + StartDate.Hour.ToString("00") + ":" + StartDate.Minute.ToString("00") + ":" + StartDate.Second.ToString("00") + "', FCEndDate='" + EndDate.Year.ToString() + "-" + EndDate.Month.ToString("00") + "-" + EndDate.Day.ToString("00") + " " + EndDate.Hour.ToString("00") + ":" + EndDate.Minute.ToString("00") + ":" + EndDate.Second.ToString("00") + "' WHERE idLOT=" + idLOT.ToString();
            command.CommandText = sqlquery;
            command.Connection = Connection;
            adapter.SelectCommand = command;
            
                command.ExecuteNonQuery();
        }
        #endregion

        #region Transfer to DB
        private void Transfer_DB()
        {
            DateTime StartDate = DatesNorm[0];
            DateTime EndDate = DatesNorm[DatesNorm.Count-1];
            Boolean Continue = false;
            Int64 idLOT = -1;
            String LOTNr = (String)LOTS_list.SelectedItem;
            string sqlquery = "SELECT idLOT FROM felots WHERE LOTNr ='" + LOTNr + "'";
            MySqlDataReader data;
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            MySqlCommand command = new MySqlCommand();
            command.CommandText = sqlquery;
            command.Connection = Connection;
            adapter.SelectCommand = command;
            data = command.ExecuteReader();
            if (data.Read())
            {
                if (!data.IsDBNull(0))
                    idLOT = data.GetInt64(0);
            }
            data.Close();
            if (idLOT == -1)
            {
                sqlquery = "INSERT INTO felots(LOTNr,FCStartDate,FCEndDate) VALUES('" + LOTNr + "','" + StartDate.Year.ToString() + StartDate.Month.ToString("00") + "-" + StartDate.Day.ToString("00") + " " + StartDate.Hour.ToString("00") + ":" + StartDate.Minute.ToString("00") + ":" + StartDate.Second.ToString("00") + "','" + EndDate.Year.ToString() + EndDate.Month.ToString("00") + "-" + EndDate.Day.ToString("00") + " " + EndDate.Hour.ToString("00") + ":" + EndDate.Minute.ToString("00") + ":" + EndDate.Second.ToString("00") + "')";
                command.CommandText = sqlquery;
                command.Connection = Connection;
                adapter.SelectCommand = command;                
                command.ExecuteNonQuery();                
                idLOT = command.LastInsertedId;
                Continue = true;
            }
            else
            {
                sqlquery = "select COUNT(felots.idLOT) FROM fesputterfci INNER JOIN fepositions ON fepositions.idPos=fesputterfci.idPos INNER JOIN felots ON fepositions.idLOT=felots.idLOT WHERE felots.idLOT=" + idLOT.ToString();
                command.CommandText = sqlquery;
                command.Connection = Connection;
                adapter.SelectCommand = command;
                object res = command.ExecuteScalar();
                int Count = 0;
                if (res != null)
                {
                    Count = Convert.ToInt32(res);
                }
                if (Count > 0)
                {
                    DialogResult result = MessageBox.Show("LOT data already existing, confirm transfer?", "Transfer confirmation", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        Output_ListBox.Items.Add(DateTime.Now.ToString() + "Deleteting LOT data");
                        Delete_LOT_Info(LOTNr);
                        Update_LOT_Dates(idLOT, StartDate, EndDate);
                        Continue = true;
                    }
                    else
                        Continue = false;

                }
                else
                {
                    Update_LOT_Dates(idLOT, StartDate, EndDate);
                    Continue = true;
                }

               
                    
            }
            if (Continue)
            {   
                for(int i=1;i<TablesNorm.Length;i++)
                {
                    List<Int64> idPos = new List<long>();
                    TransferProgress.Value = 0;
                    var idPosParameter = new MySqlParameter("idPos", MySqlDbType.Int64);
                    var SpeedParameter = new MySqlParameter("SPEED", MySqlDbType.Double);
                    List<MySqlParameter> SQLParams = new List<MySqlParameter>();
                    command = new MySqlCommand("INSERT INTO fepositions(X,Y,idLOT) VALUES(?X, ?Y, ?idLOT)", Connection);
                    var XParameter = new MySqlParameter("X", MySqlDbType.Double);
                    var YParameter = new MySqlParameter("Y", MySqlDbType.Double);
                    var idLOTParameter = new MySqlParameter("idLOT", MySqlDbType.Int64);
                    command.Parameters.Add(XParameter);
                    command.Parameters.Add(YParameter);
                    command.Parameters.Add(idLOTParameter);
                    String CommandString = "INSERT INTO "+ TablesNorm[i].Name + "(";
                    String CommandString1 = " VALUES(";
                    Boolean first = true;
                    foreach (Param_Stc Param in TablesNorm[i].Params)
                    {
                        if(first)
                        {
                            int j = 0;
                            foreach (double Value in Param.Values)
                            {
                                XParameter.Value = PositionsNorm[j] - TablesNorm[i].X;
                                YParameter.Value = TablesNorm[i].Y;
                                idLOTParameter.Value = idLOT;
                                command.ExecuteNonQuery();
                                idPos.Add(command.LastInsertedId);
                                j++;
                            }
                            first = false;
                        }
                       
                        string Name = Param.Name.Replace('.', '_');
                        CommandString = CommandString + Name + ",";
                        CommandString1 = CommandString1 + " ?" + Name + ",";
                        var Parameter = new MySqlParameter(Name, MySqlDbType.Double);
                        SQLParams.Add(Parameter);
                                      
                    }
                    if(TablesNorm[i].Name== "fesputterfc_sz2")
                    {
                        CommandString = CommandString + "SPEED,";
                        CommandString1 = CommandString1 + " ?SPEED,";
                    }
                    CommandString = CommandString + "idPos)";
                    CommandString1 = CommandString1 + " ?idPos)";
                    CommandString = CommandString + CommandString1;
                    command = new MySqlCommand(CommandString, Connection);
                    command.Parameters.Add(idPosParameter);
                    if (TablesNorm[i].Name == "fesputterfc_sz2")
                    {
                        command.Parameters.Add(SpeedParameter);
                    }
                    foreach (MySqlParameter SQLParam in SQLParams)
                    {
                        command.Parameters.Add(SQLParam);
                    }
                    for (int j=0;j<TablesNorm[i].Params[0].Values.Count;j++)
                    {
                        TransferProgress.Value = (100*j)/ TablesNorm[i].Params[0].Values.Count;
                        int count = 0;
                        foreach(Param_Stc Param in TablesNorm[i].Params)
                        {
                            SQLParams[count].Value = Param.Values[j];
                            idPosParameter.Value = idPos[j];
                            if (TablesNorm[i].Name == "fesputterfc_sz2")
                            {
                                SpeedParameter.Value = VelocitiesNorm[j];
                            }
                                count++;
                        }
                        command.ExecuteNonQuery();
                        Int64 LastID = command.LastInsertedId;
                        sqlquery = "UPDATE " + TablesNorm[i].Name + " SET Date='" + DatesNorm[j].Year.ToString() + "-" + DatesNorm[j].Month.ToString("00") + "-" + DatesNorm[j].Day.ToString("00") + " " + DatesNorm[j].Hour.ToString("00") + ":" + DatesNorm[j].Minute.ToString("00") + ":" + DatesNorm[j].Second.ToString("00") + "' WHERE id"+ TablesNorm[i].Name + "=" + LastID.ToString();
                        MySqlDataAdapter adapter1 = new MySqlDataAdapter();
                        MySqlCommand command1 = new MySqlCommand();
                        command1.CommandText = sqlquery;
                        command1.Connection = Connection;
                        adapter1.SelectCommand = command1;
                        command1.ExecuteNonQuery();
                    }

                }
            }

        }

        private void FirstLabel_Combo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(FirstLabel_Combo.SelectedIndex==0)
            {
                FirstLabel_Num.Enabled = false;
                FirstSubModules_Combo.Enabled = true;

            }
            else
            {
                FirstLabel_Num.Enabled = true;
                FirstSubModules_Combo.Enabled = false;
            }
        }

        private void LastLabel_Combo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (LastLabel_Combo.SelectedIndex == 0)
            {
                LastLabel_Num.Enabled = false;
                LastSubModules_Combo.Enabled = true;

            }
            else
            {
                LastLabel_Num.Enabled = true;
                LastSubModules_Combo.Enabled = false;
            }
        }

        private void TransferProgress_Click(object sender, EventArgs e)
        {

        }
        #endregion

        #region Delete Button 
        private void Delete_Button_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to delete FC Sputter data for LOT: " + LOTS_list.Text, "Delete confirmation", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                Transfer_button.Enabled = false;
                Delete_Button.Enabled = false;
                Output_ListBox.Items.Add(DateTime.Now.ToString() + " Deleting LOT");
                Delete_LOT_Info(LOTS_list.Text);
                Output_ListBox.Items.Add(DateTime.Now.ToString() + " Delete finished");
                Transfer_button.Enabled = true;
                Delete_Button.Enabled = true;
            }
        }
        #endregion

        #region Delete LOT Info       
        private void Delete_LOT_Info(string LOTNr)
        {
            Int64 idLOT = Get_LOT_ID(LOTNr);           
           
            if (idLOT > 0)
            {
                for(int i=1;i<Tables.Length;i++)
                {
                    List<Int64[]> IDs = new List<Int64[]>();

                    string sqlquery = "SELECT "+ Tables[i].Name + ".idPos,id" + Tables[i].Name + " FROM "+ Tables[i].Name + " INNER JOIN fepositions ON fepositions.idPos = " + Tables[i].Name + ".idPos INNER JOIN felots ON felots.idLOT = fepositions.idLOT WHERE felots.idLOT =" + idLOT.ToString();
                    MySqlDataReader data;
                    MySqlDataAdapter adapter = new MySqlDataAdapter();
                    MySqlCommand command = new MySqlCommand();
                    command.CommandText = sqlquery;
                    command.Connection = Connection;
                    adapter.SelectCommand = command;
                    data = command.ExecuteReader();
                    while (data.Read())
                    {
                        Int64 idPos=0, idfc=0;
                        if (!data.IsDBNull(0))
                            idPos = data.GetInt64(0);
                        if (!data.IsDBNull(1))
                            idfc = data.GetInt64(1);
                        Int64[] Aux = { idPos,idfc };
                        IDs.Add(Aux);
                    }
                    data.Close();
                    TransferProgress.Value = 0;
                    int Count = 0;
                    foreach(Int64[] ID in IDs)
                    {
                        TransferProgress.Value = (int)(100 * Count / IDs.Count);

                        sqlquery = "DELETE FROM " + Tables[i].Name + " WHERE id" + Tables[i].Name + "=" + ID[1].ToString();
                        command.CommandText = sqlquery;
                        command.Connection = Connection;
                        adapter.SelectCommand = command;
                        command.ExecuteNonQuery();
                        sqlquery = "DELETE FROM fepositions WHERE idPos=" + ID[0].ToString();
                        command.CommandText = sqlquery;
                        command.Connection = Connection;
                        adapter.SelectCommand = command;
                        try
                        {
                            command.ExecuteNonQuery();
                        }
                        catch(Exception Ex)
                        {

                        }
                        Count++;
                    }
                    List<Int64> IDi = new List<long>();
                    sqlquery = "SELECT idfesputterfci FROM fesputterfci INNER JOIN fepositions ON fepositions.idPos=fesputterfci.idPos WHERE fepositions.idLOT=" + idLOT.ToString();
                    command.CommandText = sqlquery;
                    command.Connection = Connection;
                    adapter.SelectCommand = command;
                    data = command.ExecuteReader();
                    while (data.Read())
                    {
                        if(!data.IsDBNull(0))
                            IDi.Add(data.GetInt64(0));
                    }
                    data.Close();
                    Count = 0;
                    foreach(Int64 ID in IDi)
                    {
                        TransferProgress.Value = (int)(100 * Count / IDi.Count);
                        sqlquery = "DELETE FROM fesputterfci WHERE idfesputterfci=" + ID.ToString();
                        command.CommandText = sqlquery;
                        command.Connection = Connection;
                        adapter.SelectCommand = command;
                        command.ExecuteNonQuery();
                        Count++;
                    }
                    TransferProgress.Value = 0;


                }
               

            }
        }
        #endregion

       
    }

}
