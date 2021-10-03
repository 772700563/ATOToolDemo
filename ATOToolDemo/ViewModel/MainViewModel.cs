using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Windows.Forms;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Data;
using LiveCharts;
using LiveCharts.Wpf;
using System.IO;
using System.Text;
using System;
using System.Windows;
using System.Windows.Media;
using ATOToolDemo.Model;
using LiveCharts.Configurations;
using MessageBox = System.Windows.MessageBox;

namespace ATOToolDemo.ViewModel
{

    public class MainViewModel : ViewModelBase
    {
        #region [properties]

        #region [��־�ļ����г�����]

        /// <summary>
        /// ���г����ļ�������
        /// </summary>
        private int curr_fileSingle_idx;
        public int Curr_fileSingle_idx
        {
            get { return curr_fileSingle_idx; }
            set
            {
                curr_fileSingle_idx = value;
                RaisePropertyChanged();
                if (Curr_fileSingle_idx >= 0)
                    Filename_logSingle = FileNames[Curr_fileSingle_idx];
            }
        }

        /// <summary>
        /// ���г����г�������
        /// </summary>
        private int curr_trainSingle_idx;
        public int Curr_trainSingle_idx
        {
            get { return curr_trainSingle_idx; }
            set
            {
                curr_trainSingle_idx = value;
                RaisePropertyChanged();

            }
        }

        /// <summary>
        /// ���г����г���
        /// </summary>
        private BindingList<ComboBoxItem> trainNameList_LogSingle;
        public BindingList<ComboBoxItem> TrainNameList_LogSingle
        {
            get { return trainNameList_LogSingle; }
            set
            {
                trainNameList_LogSingle = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// ���г��ļ���
        /// </summary>
        private string filename_logSingle;
        public string Filename_logSingle
        {
            get { return filename_logSingle; }
            set
            {
                filename_logSingle = value;
                RaisePropertyChanged();
                ExcelHelper excelhelper_Single = new ExcelHelper(Filename_logSingle);
                Data_Single = excelhelper_Single.ExcelToDataTable("sheet1", true);
                TrainNameList_LogSingle = new BindingList<ComboBoxItem>();
                for (int i = 1; i < Data_Single.Rows.Count - 3000; i++)
                {
                    var item = new ComboBoxItem()
                    {
                        Content = Data_Single.Rows[i][0]
                    };
                    TrainNameList_LogSingle.Add(item);
                }
                #region ��ʱ������ʼ��
                ChartDatas paraTemp = new ChartDatas();
                paraTemp.Time = new BindingList<double>();
                paraTemp.Speed = new BindingList<double>();
                paraTemp.Status = new BindingList<double>();
                paraTemp.TargetAcc = new BindingList<double>();
                paraTemp.TargetSpeed = new BindingList<double>();
                paraTemp.TrainPosition = new BindingList<double>();
                paraTemp.TrainAcc = new BindingList<double>();
                paraTemp.DeltaAcc = new BindingList<double>();
                Double temp = 0;
                #endregion
                for (int i = 1; i < Data_Single.Rows.Count; i++)
                {
                    if (Data_Single.Rows[i][15].ToString() != "Error" &&
                        (paraTemp.TrainPosition.Count - 1 < 0 || Double.Parse(Data_Single.Rows[i][9].ToString()) + temp != paraTemp.TrainPosition[paraTemp.TrainPosition.Count - 1]) && Double.Parse(Data_Single.Rows[i][9].ToString()) != 0)
                    {
                        if (paraTemp.TrainPosition.Count - 1 < 0)
                        {
                            paraTemp.Time.Add(((i - 1) * 0.25));
                            paraTemp.Speed.Add(Double.Parse(Data_Single.Rows[i][2].ToString()));
                            paraTemp.TargetSpeed.Add(Double.Parse(Data_Single.Rows[i][3].ToString()));
                            paraTemp.TargetAcc.Add(Double.Parse(Data_Single.Rows[i][6].ToString()));
                            paraTemp.TrainAcc.Add(Double.Parse(Data_Single.Rows[i][6].ToString()));
                            paraTemp.DeltaAcc.Add(Double.Parse(Data_Single.Rows[i][7].ToString()));
                            paraTemp.TrainPosition.Add(Double.Parse(Data_Single.Rows[i][9].ToString()) + temp);
                            if (Data_Single.Rows[i][15].ToString() == "Traction")
                                paraTemp.Status.Add(2.0);
                            else if (Data_Single.Rows[i][15].ToString() == "Coast")
                                paraTemp.Status.Add(1.0);
                            else if (Data_Single.Rows[i][15].ToString() == "Brake")
                                paraTemp.Status.Add(0.0);
                            continue;
                        }
                        else if (paraTemp.TrainPosition[paraTemp.TrainPosition.Count - 1] - Double.Parse(Data_Single.Rows[i][9].ToString()) - temp < 0.1 &&
                            paraTemp.TrainPosition[paraTemp.TrainPosition.Count - 1] - Double.Parse(Data_Single.Rows[i][9].ToString()) - temp > 0)
                            continue;
                        paraTemp.Time.Add(((i - 1) * 0.25));
                        paraTemp.Speed.Add(Double.Parse(Data_Single.Rows[i][2].ToString()));
                        paraTemp.TargetSpeed.Add(Double.Parse(Data_Single.Rows[i][3].ToString()));
                        if (Double.Parse(Data_Single.Rows[i][6].ToString()) - paraTemp.TargetAcc[paraTemp.TrainAcc.Count - 1] > 10 || Double.Parse(Data_Single.Rows[i][6].ToString()) - paraTemp.TrainAcc[paraTemp.TrainAcc.Count - 1] < -10)
                            paraTemp.TrainAcc.Add(paraTemp.TrainAcc[paraTemp.TrainAcc.Count - 1]);
                        else
                            paraTemp.TrainAcc.Add(Double.Parse(Data_Single.Rows[i][6].ToString()));
                        if (Double.Parse(Data_Single.Rows[i][5].ToString()) - paraTemp.TargetAcc[paraTemp.TargetAcc.Count - 1] > 10 || Double.Parse(Data_Single.Rows[i][5].ToString()) - paraTemp.TargetAcc[paraTemp.TargetAcc.Count - 1] < -10)
                            paraTemp.TargetAcc.Add(paraTemp.TargetAcc[paraTemp.TargetAcc.Count - 1]);
                        else
                            paraTemp.TargetAcc.Add(Double.Parse(Data_Single.Rows[i][5].ToString()));
                        if (Double.Parse(Data_Single.Rows[i][7].ToString()) > 1000 || Double.Parse(Data_Single.Rows[i][7].ToString()) < -1000)
                            paraTemp.DeltaAcc.Add(1.0);
                        else
                            if (Double.Parse(Data_Single.Rows[i][7].ToString()) - paraTemp.DeltaAcc[paraTemp.DeltaAcc.Count - 1] > 10 || Double.Parse(Data_Single.Rows[i][7].ToString()) - paraTemp.DeltaAcc[paraTemp.DeltaAcc.Count - 1] < -10)
                            paraTemp.DeltaAcc.Add(paraTemp.DeltaAcc[paraTemp.DeltaAcc.Count - 1]);
                        else
                            paraTemp.DeltaAcc.Add(Double.Parse(Data_Single.Rows[i][7].ToString()));
                        if (paraTemp.TrainPosition.Count == 0)
                            paraTemp.TrainPosition.Add(Double.Parse(Data_Single.Rows[i][9].ToString()) + temp);
                        else
                        {
                            if (Double.Parse(Data_Single.Rows[i][9].ToString()) + temp > paraTemp.TrainPosition[paraTemp.TrainPosition.Count - 1])
                                paraTemp.TrainPosition.Add(Double.Parse(Data_Single.Rows[i][9].ToString()) + temp);
                            else
                            {
                                temp = paraTemp.TrainPosition[paraTemp.TrainPosition.Count - 1];
                                paraTemp.TrainPosition.Add(Double.Parse(Data_Single.Rows[i][9].ToString()) + temp);
                            }
                        }
                        if (Data_Single.Rows[i][15].ToString() == "Traction")
                            paraTemp.Status.Add(2.0);
                        else if (Data_Single.Rows[i][15].ToString() == "Coast")
                            paraTemp.Status.Add(1.0);
                        else if (Data_Single.Rows[i][15].ToString() == "Brake")
                            paraTemp.Status.Add(0.0);
                        else
                            paraTemp.Status.Add(-1.0);
                    }
                }

                MySinChartDatass = paraTemp;
            }
        }

        /// <summary>
        /// ���г�ͼ�����
        /// </summary>
        private ChartDatas mySinChartDatass;
        public ChartDatas MySinChartDatass
        {
            get { return mySinChartDatass; }
            set
            {
                mySinChartDatass = value;
                RaisePropertyChanged();
            }
        }

        private BindingList<string> mySinChartTypes;
        public BindingList<string> MySinChartTypes
        {
            get { return mySinChartTypes; }
            set
            {
                mySinChartTypes = value;
                RaisePropertyChanged();
            }
        }

        private int mySinChartTypesIdx;
        public int MySinChartTypesIdx
        {
            get { return mySinChartTypesIdx; }
            set
            {
                mySinChartTypesIdx = value;
                RaisePropertyChanged();
            }
        }
        private BindingList<string> mySinGra;
        public BindingList<string> MySinGra
        {
            get { return mySinGra; }
            set
            {
                mySinGra = value;
                RaisePropertyChanged();
            }
        }

        private int mySinGraIdx;
        public int MySinGraIdx
        {
            get { return mySinGraIdx; }
            set
            {
                mySinGraIdx = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region [��־�ļ����г�����]

        /// <summary>
        /// ��ʾ���г�չʾ������
        /// </summary>
        private BindingList<MultyTrainDataGrid> logFileProp_Mult;
        public BindingList<MultyTrainDataGrid> LogFileProp_Mult
        {
            get
            {
                return logFileProp_Mult;
            }
            set
            {
                logFileProp_Mult = value;
                RaisePropertyChanged();
            }
        }

        private int curr_fileMult_idx;
        public int Curr_fileMult_idx
        {
            get { return curr_fileMult_idx; }
            set
            {
                curr_fileMult_idx = value;
                RaisePropertyChanged();
                if (Curr_fileMult_idx >= 0)
                    Filename_logMult = FileNames[Curr_fileMult_idx];
            }
        }

        private string filename_logMult;
        public string Filename_logMult
        {
            get { return filename_logMult; }
            set
            {
                filename_logMult = value;
                RaisePropertyChanged();
                ExcelHelper excelhelper_Mult = new ExcelHelper(Filename_logMult);
                Data_Mult = excelhelper_Mult.ExcelToDataTable("sheet1", true);
                //TrainNameList_LogMult = new BindingList<ComboBoxItem>();
                for (int i = 1; i < Data_Mult.Rows.Count - 3000; i++)
                {
                    var item = new ComboBoxItem()
                    {
                        Content = Data_Mult.Rows[i][0]
                    };
                    //TrainNameList_LogMult.Add(item);
                }
                #region ��ʱ������ʼ��
                ChartDatas paraTemp = new ChartDatas();
                paraTemp.Time = new BindingList<double>();
                paraTemp.Speed = new BindingList<double>();
                paraTemp.Status = new BindingList<double>();
                paraTemp.TargetAcc = new BindingList<double>();
                paraTemp.TargetSpeed = new BindingList<double>();
                paraTemp.TrainPosition = new BindingList<double>();
                paraTemp.TrainAcc = new BindingList<double>();
                paraTemp.DeltaAcc = new BindingList<double>();
                Double temp = 0;
                #endregion
                for (int i = 1; i < Data_Mult.Rows.Count; i++)
                {
                    if (Data_Mult.Rows[i][15].ToString() != "Error" &&
                        (paraTemp.TrainPosition.Count - 1 < 0 ||
                        Double.Parse(Data_Mult.Rows[i][9].ToString()) + temp != paraTemp.TrainPosition[paraTemp.TrainPosition.Count - 1])
                        && Double.Parse(Data_Mult.Rows[i][9].ToString()) != 0)
                    {
                        if (paraTemp.TrainPosition.Count - 1 < 0)
                        {
                            paraTemp.Time.Add(((i - 1) * 0.25));
                            paraTemp.Speed.Add(Double.Parse(Data_Mult.Rows[i][2].ToString()));
                            paraTemp.TargetSpeed.Add(Double.Parse(Data_Mult.Rows[i][3].ToString()));
                            paraTemp.TargetAcc.Add(Double.Parse(Data_Mult.Rows[i][6].ToString()));
                            paraTemp.TrainAcc.Add(Double.Parse(Data_Mult.Rows[i][6].ToString()));
                            paraTemp.DeltaAcc.Add(Double.Parse(Data_Mult.Rows[i][7].ToString()));
                            paraTemp.TrainPosition.Add(Double.Parse(Data_Mult.Rows[i][9].ToString()) + temp);
                            if (Data_Mult.Rows[i][15].ToString() == "Traction")
                                paraTemp.Status.Add(2.0);
                            else if (Data_Mult.Rows[i][15].ToString() == "Coast")
                                paraTemp.Status.Add(1.0);
                            else if (Data_Mult.Rows[i][15].ToString() == "Brake")
                                paraTemp.Status.Add(0.0);
                            continue;
                        }
                        else if (paraTemp.TrainPosition[paraTemp.TrainPosition.Count - 1] - Double.Parse(Data_Mult.Rows[i][9].ToString()) - temp < 0.1 &&
                            paraTemp.TrainPosition[paraTemp.TrainPosition.Count - 1] - Double.Parse(Data_Mult.Rows[i][9].ToString()) - temp > 0)
                            continue;
                        paraTemp.Time.Add(((i - 1) * 0.25));
                        paraTemp.Speed.Add(Double.Parse(Data_Mult.Rows[i][2].ToString()));
                        paraTemp.TargetSpeed.Add(Double.Parse(Data_Mult.Rows[i][3].ToString()));
                        if (Double.Parse(Data_Mult.Rows[i][6].ToString()) - paraTemp.TargetAcc[paraTemp.TrainAcc.Count - 1] > 10 || Double.Parse(Data_Mult.Rows[i][6].ToString()) - paraTemp.TrainAcc[paraTemp.TrainAcc.Count - 1] < -10)
                            paraTemp.TrainAcc.Add(paraTemp.TrainAcc[paraTemp.TrainAcc.Count - 1]);
                        else
                            paraTemp.TrainAcc.Add(Double.Parse(Data_Mult.Rows[i][6].ToString()));
                        if (Double.Parse(Data_Mult.Rows[i][5].ToString()) - paraTemp.TargetAcc[paraTemp.TargetAcc.Count - 1] > 10 || Double.Parse(Data_Mult.Rows[i][5].ToString()) - paraTemp.TargetAcc[paraTemp.TargetAcc.Count - 1] < -10)
                            paraTemp.TargetAcc.Add(paraTemp.TargetAcc[paraTemp.TargetAcc.Count - 1]);
                        else
                            paraTemp.TargetAcc.Add(Double.Parse(Data_Mult.Rows[i][5].ToString()));
                        if (Double.Parse(Data_Mult.Rows[i][7].ToString()) > 1000 || Double.Parse(Data_Mult.Rows[i][7].ToString()) < -1000)
                            paraTemp.DeltaAcc.Add(1.0);
                        else
                            if (Double.Parse(Data_Mult.Rows[i][7].ToString()) - paraTemp.DeltaAcc[paraTemp.DeltaAcc.Count - 1] > 10 || Double.Parse(Data_Mult.Rows[i][7].ToString()) - paraTemp.DeltaAcc[paraTemp.DeltaAcc.Count - 1] < -10)
                            paraTemp.DeltaAcc.Add(paraTemp.DeltaAcc[paraTemp.DeltaAcc.Count - 1]);
                        else
                            paraTemp.DeltaAcc.Add(Double.Parse(Data_Mult.Rows[i][7].ToString()));
                        if (paraTemp.TrainPosition.Count == 0)
                            paraTemp.TrainPosition.Add(Double.Parse(Data_Mult.Rows[i][9].ToString()) + temp);
                        else
                        {
                            if (Double.Parse(Data_Mult.Rows[i][9].ToString()) + temp > paraTemp.TrainPosition[paraTemp.TrainPosition.Count - 1])
                                paraTemp.TrainPosition.Add(Double.Parse(Data_Mult.Rows[i][9].ToString()) + temp);
                            else
                            {
                                temp = paraTemp.TrainPosition[paraTemp.TrainPosition.Count - 1];
                                paraTemp.TrainPosition.Add(Double.Parse(Data_Mult.Rows[i][9].ToString()) + temp);
                            }
                        }
                        if (Data_Mult.Rows[i][15].ToString() == "Traction")
                            paraTemp.Status.Add(2.0);
                        else if (Data_Mult.Rows[i][15].ToString() == "Coast")
                            paraTemp.Status.Add(1.0);
                        else if (Data_Mult.Rows[i][15].ToString() == "Brake")
                            paraTemp.Status.Add(0.0);
                        else
                            paraTemp.Status.Add(-1.0);
                    }
                }
                MyMultyChartDatas.Add(paraTemp);
            }
        }

        private BindingList<string> myMultychartTypes;
        public BindingList<string> MyMultyChartTypes
        {
            get { return myMultychartTypes; }
            set
            {
                myMultychartTypes = value;
                RaisePropertyChanged();
            }
        }

        private int myMultychartTypesIdx;
        public int MyMultychartTypesIdx
        {
            get { return myMultychartTypesIdx; }
            set
            {
                myMultychartTypesIdx = value;
                RaisePropertyChanged();
            }
        }

        private BindingList<ChartDatas> myMultyChartDatas;
        public BindingList<ChartDatas> MyMultyChartDatas
        {
            get { return myMultyChartDatas; }
            set
            {
                myMultyChartDatas = value;
                RaisePropertyChanged();
            }
        }

        private int trainNumber;
        public int TrainNumber
        {
            get { return trainNumber; }
            set { trainNumber = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region [����ָ������]
        private int curr_file_idx;
        public int Curr_file_idx
        {
            get { return curr_file_idx; }
            set
            {
                curr_file_idx = value;
                RaisePropertyChanged();
                if (Curr_file_idx >= 0)
                    Filename = FileNames[curr_file_idx];
            }
        }

        private int curr_train_idx;
        public int Curr_train_idx
        {
            get { return curr_train_idx; }
            set
            {
                curr_train_idx = value;
                RaisePropertyChanged();
            }
        }

        private int curr_MultProp_idx;
        public int Curr_MultProp_idx
        {
            get { return curr_MultProp_idx; }
            set
            {
                curr_MultProp_idx = value;
                RaisePropertyChanged();
            }
        }

        private BindingList<ComboBoxItem> trainNameList;
        public BindingList<ComboBoxItem> TrainNameList
        {
            get { return trainNameList; }
            set
            {
                trainNameList = value;
                RaisePropertyChanged();
            }
        }

        private string filename;
        public string Filename
        {
            get { return filename; }
            set
            {
                filename = value;
                RaisePropertyChanged();
                ExcelHelper excelhelper = new ExcelHelper(Filename);
                Data = excelhelper.ExcelToDataTable("sheet1", true);
                TrainNameList = new BindingList<ComboBoxItem>();
                for (int i = 1; i < Data.Rows.Count - 3000; i++)
                {
                    var item = new ComboBoxItem()
                    {
                        Content = Data.Rows[i][0]
                    };
                    TrainNameList.Add(item);
                }
            }
        }
        #region [ָ������]

        private string description; //ָ��1������
        public string Description
        {
            get { return description; }
            set
            {
                description = value;
                RaisePropertyChanged();
            }

        }

        private string eosa_time; //ָ��2��EOASʱ��
        public string EOSA_Time
        {
            get { return eosa_time; }
            set
            {
                eosa_time = value; RaisePropertyChanged();
            }
        }

        private string dist;  //ָ��3�����о���
        public string Dist
        {
            get { return dist; }
            set { dist = value; RaisePropertyChanged(); }
        }

        private string total_mileage; // ָ��4�������
        public string Total_Mileage
        {
            get { return total_mileage; }
            set { total_mileage = value; RaisePropertyChanged(); }
        }

        private string train_speed;  //ָ��5�� �п���
        public string Train_Speed
        {
            get { return train_speed; }
            set { train_speed = value; RaisePropertyChanged(); }
        }

        private string braking_level;  //ָ��6���ƶ���λ
        public string Braking_Level
        {
            get { return braking_level; }
            set { braking_level = value; RaisePropertyChanged(); }
        }

        #endregion


        #endregion

        #region [�ļ���������]
        private ObservableCollection<string> filenames; //��־���ȡ���ļ���
        public ObservableCollection<string> FileNames
        {
            get { return filenames; }
            set
            {
                filenames = value;
                RaisePropertyChanged();
            }
        }
        private ObservableCollection<string> filenames_Sim; //��־���ȡ���ļ���
        public ObservableCollection<string> FileNames_Sim
        {
            get { return filenames_Sim; }
            set
            {
                filenames_Sim = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region [Live chart]
        private BindingList<LiveChartParemeters> myCharts;
        public BindingList<LiveChartParemeters> MyCharts
        {
            get { return myCharts; }
            set
            {
                myCharts = value;
                RaisePropertyChanged();
            }
        }


        #endregion

        #region [ASC]
        private ObservableCollection<string> ascFileNames;
        public ObservableCollection<string> AscFileNames
        {
            get { return ascFileNames; }
            set
            {
                ascFileNames = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<string> ascSimFileNames;
        public ObservableCollection<string> AscSimFileNames
        {
            get { return ascSimFileNames; }
            set
            {
                ascSimFileNames = value;
                RaisePropertyChanged();
            }
        }

        private BindingList<string> oriDatas_Asc;
        public BindingList<string> OriDatas_Asc
        {
            get { return oriDatas_Asc; }
            set
            {
                oriDatas_Asc = value;
                RaisePropertyChanged();
            }
        }

        private BindingList<AscDatas> processedData_Asc;
        public BindingList<AscDatas> ProcessedData_Asc
        {
            get { return processedData_Asc; }
            set
            {
                processedData_Asc = value;
                RaisePropertyChanged();

            }
        }

        private string ascData;
        public string AscData
        {
            get { return ascData; }
            set
            {
                ascData = value;
                if (Last_Ascidx != -1)
                {
                    if (ProcessedData_Asc[Last_Ascidx].Data != Last_AscData)
                    {
                        Background_Change.Color = Colors.Green;
                        Text_Change = "���޸�";
                        ProcessedData_Asc[Last_Ascidx].Asc_Background = new SolidColorBrush(Colors.GreenYellow);
                    }
                }
                Last_Ascidx = Curr_Ascpara_idx;
                Last_AscData = AscData;
                RaisePropertyChanged();
            }
        }

        private int last_Ascidx;
        public int Last_Ascidx
        {
            get { return last_Ascidx; }
            set
            {
                last_Ascidx = value;
            }
        }

        private int curr_Ascfile_idx;
        public int Curr_Ascfile_idx
        {
            get { return curr_Ascfile_idx; }
            set
            {
                curr_Ascfile_idx = value;
                RaisePropertyChanged();

            }
        }

        private int curr_Ascpara_idx;
        public int Curr_Ascpara_idx
        {
            get { return curr_Ascpara_idx; }
            set
            {
                curr_Ascpara_idx = value;
                RaisePropertyChanged();
            }
        }

        private string last_AscData;
        public string Last_AscData
        {
            get { return last_AscData; }
            set { last_AscData = value; }
        }

        private List<string> save_NewDatas;
        public List<string> Save_NewDatas
        {
            get { return save_NewDatas; }
            set { save_NewDatas = value; }
        }


        #endregion

        #region [����]
        private string nowTime;
        public string NowTime
        {
            get { return nowTime; }
            set
            {
                nowTime = value;
                RaisePropertyChanged();
            }
        }

        private string text_Save;
        public string Text_Save
        {
            get { return text_Save; }
            set
            {
                text_Save = value;
                RaisePropertyChanged();
            }
        }

        private SolidColorBrush background_Save;
        public SolidColorBrush Background_Save
        {
            get { return background_Save; }
            set
            {
                background_Save = value;
                RaisePropertyChanged();
            }
        }

        private string text_Change;
        public string Text_Change
        {
            get { return text_Change; }
            set
            {
                text_Change = value;
                RaisePropertyChanged();
            }
        }

        private SolidColorBrush background_Change;
        public SolidColorBrush Background_Change
        {
            get { return background_Change; }
            set
            {
                background_Change = value;
                RaisePropertyChanged();
            }
        }

        private double height;
        public double Height
        {
            get { return height; }
            set
            {
                height = value;
                RaisePropertyChanged();
            }
        }

        private double height_Datagrid;

        public double Height_Datagrid
        {
            get { return height_Datagrid; }
            set
            {
                height_Datagrid = value;
                RaisePropertyChanged();
            }
        }


        #endregion

        #region[�ļ�������]
        private BindingList<FileCache> myCache;
        public BindingList<FileCache> MyCache
        {
            get
            {
                return myCache;
            }
            set
            {
                myCache = value;
                RaisePropertyChanged();
            }
        }

        private int myCache_Idx;
        public int MyCache_Idx
        {
            get { return myCache_Idx; }
            set
            {
                myCache_Idx = value;
                RaisePropertyChanged();
            }
        }
        #endregion
        #endregion

        #region [private properties]
        private DataTable Data
        {
            get; set;
        }
        private DataTable Data_Mult
        {
            get; set;
        }
        private DataTable Data_Single
        {
            get;
            set;
        }
        #endregion

        #region [commands]

        #region [����ָ������]

        public RelayCommand UpdataIndicator { get; set; } //��������ָ��

        #endregion

        #region [�ļ���������]
        public RelayCommand SaveLogFileToExcelCommand { get; set; }  //���Ϊexcel
        public RelayCommand ReadLogFilesCommand { get; set; } //��־��ȡ�ļ�
        public RelayCommand DeleteFileCache { get; set; }
        #endregion

        #region [���г�����]
        public RelayCommand ShowSingleTrain { get; set; }

        public RelayCommand ShowSinTrainRadio { get; set; }
        #endregion

        #region [���г�����]
        public RelayCommand AddLogFile { get; set; }
        public RelayCommand ShowMultTrain { get; set; }
        public RelayCommand DeleteMultTrain { get; set; }
        public RelayCommand MoveUp { get; set; }
        public RelayCommand MoveDown { get; set; }
        public RelayCommand UpdataMultTrain { get; set; }
        public RelayCommand ShowMulTrainRadio { get; set; }
        //public RelayCommand ChangeStatus { get; set; }
        //public RelayCommand ChangeAcc { get; set; }

        #endregion

        #region [ASC]
        public RelayCommand Read_AscFiles { get; set; }
        public RelayCommand Show_AscDatas { get; set; }

        public RelayCommand Save_AscNewFiles { get; set; }
        #endregion

        #endregion

        #region [funcs]
        private void InitProperties()
        {
            AscSimFileNames = new ObservableCollection<string>();
            AscFileNames = new ObservableCollection<string>();
            FileNames = new ObservableCollection<string>();
            FileNames_Sim = new ObservableCollection<string>();
            LogFileProp_Mult = new BindingList<MultyTrainDataGrid>();
            ProcessedData_Asc = new BindingList<AscDatas>();
            MyCharts = new BindingList<LiveChartParemeters>();
            MyMultyChartDatas = new BindingList<ChartDatas>();
            MyMultyChartTypes = new BindingList<string>() { "S-Vͼ", "S-Tͼ", "V-Tͼ", "Statusͼ", "ACCͼ", "����ʾ" };
            MySinChartTypes = new BindingList<string>() { "S-Vͼ", "S-Tͼ", "V-Tͼ", "Statusͼ", "ACCͼ", "����ʾ" };
            MySinGra = new BindingList<string>() { "����", "����һ��", "��Сһ��" };
            Height_Datagrid = Height / 1.3;
            Background_Save = new SolidColorBrush(Colors.Red);
            Background_Change = new SolidColorBrush(Colors.Red);
            myCache = new BindingList<FileCache>();

            DateTime dt = DateTime.Now;
            NowTime = dt.ToLongDateString().ToString();
            text_Change = "δ�޸�";
            text_Save = "δ����";


            Curr_file_idx = -1;
            Curr_train_idx = -1;
            TrainNumber = 0;

            MyCache_Idx = -1;
            Curr_fileMult_idx = -1;
            Curr_fileSingle_idx = -1;
            Curr_trainSingle_idx = -1;
            Curr_Ascfile_idx = -1;
            MyMultychartTypesIdx = -1;
            MySinChartTypesIdx = -1;
            MySinGraIdx = -1;
        }   //��ʼ������

        private void InitCommands()
        {
            ReadLogFilesCommand = new RelayCommand(readLogFiles);
            UpdataIndicator = new RelayCommand(updataIndicator);
            SaveLogFileToExcelCommand = new RelayCommand(saveLogFileToExcelCommand);
            AddLogFile = new RelayCommand(addLogFile);
            ShowSingleTrain = new RelayCommand(showSingleTrain);
            ShowMultTrain = new RelayCommand(showMultTrain);
            DeleteMultTrain = new RelayCommand(deleteMultTrain);
            MoveUp = new RelayCommand(moveUp);
            MoveDown = new RelayCommand(moveDown);
            Read_AscFiles = new RelayCommand(read_AscFiles);
            Show_AscDatas = new RelayCommand(show_AscDatas);
            Save_AscNewFiles = new RelayCommand(save_AscNewFiles);
            ShowMulTrainRadio = new RelayCommand(showMulTrainRadio);
            ShowSinTrainRadio = new RelayCommand(showSinTrainRadio);
            DeleteFileCache = new RelayCommand(deleteFileCache);
        }  //��ʼ������

        #region [�ļ�����]
        private void readLogFiles()
        {
            System.Windows.Forms.OpenFileDialog openfiledialog = new System.Windows.Forms.OpenFileDialog();
            openfiledialog.Filter = "(*.xls)|*.xls|(*.xlsx)|*.xlsx";
            openfiledialog.DefaultExt = "xls|xlsx";
            if (openfiledialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var ext_idx = openfiledialog.FileName.LastIndexOf(".") + 1;
                var end_idx = openfiledialog.FileName.Length;
                var ext = openfiledialog.FileName.Substring(ext_idx, end_idx - ext_idx);
                if (ext != "xls" && ext != "xlsx")
                {
                    MessageBox.Show("��ȡ�ļ�ʧ�ܣ����ȡ��ȷ��ʽ��EXCEL�ļ���", "����", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                FileNames.Add(openfiledialog.FileName);
                var start_idx = openfiledialog.FileName.LastIndexOf("\\") + 1;
                string SimName = openfiledialog.FileName.Substring(start_idx, openfiledialog.FileName.LastIndexOf(".") - start_idx);
                FileNames_Sim.Add(SimName);
                FileCache temp = new FileCache();
                temp.FileName = SimName;
                temp.FileType = "��־�ļ�";
                myCache.Add(temp);
                MessageBox.Show("��ȡEXCEl�ļ��ɹ�", "֪ͨ", MessageBoxButton.OK, MessageBoxImage.Information);

            }
        }  //��־�ж�ȡ�ļ�
        private void saveLogFileToExcelCommand()
        {
            System.Windows.Forms.SaveFileDialog saveFileDialog = new SaveFileDialog();
            string temp_fileNames = "";
            saveFileDialog.Filter = "(*.xls)|*.xls|(*.xlsx)|*.xlsx";
            saveFileDialog.DefaultExt = "xls|xlsx";
            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                temp_fileNames = saveFileDialog.FileName;
            }
            ExcelHelper excel_create = new ExcelHelper(temp_fileNames);
            DataTable data_New = new DataTable();
            if (Data == null)
            {
                MessageBox.Show("����δ֪������ȷ��Ҫ��������ݴ���", "����", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                for (int i = 0; i <= 7; i++)
                {
                    data_New.Columns.Add(new DataColumn(Data.Rows[0][i].ToString(), typeof(string)));
                }
                DataRow datarow_tmp;
                for (int i = 1; i < Data.Rows.Count; i++)
                {
                    datarow_tmp = data_New.NewRow();
                    for (int j = 0; j <= 7; j++)
                    {
                        datarow_tmp[Data.Rows[0][j].ToString()] = Data.Rows[i][j].ToString();
                    }
                    data_New.Rows.Add(datarow_tmp);
                }

                excel_create.DataTableToExcel(data_New, "sheet1", true);
            }
            catch
            {
                MessageBox.Show("����δ֪������ȷ�����Ϊ��ȷ��ʽ��EXCEL�ļ�", "����", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

        } // ���Ϊexcel�ļ�

        private void deleteFileCache()
        {
            try
            {
                if (myCache[MyCache_Idx].FileType == "ASC�ļ�")
                {
                    string filename = myCache[MyCache_Idx].FileName; 
                    AscSimFileNames.Remove(filename);
                    AscFileNames.Remove(filename);
                    MyCache.RemoveAt(MyCache_Idx);
                }
                else
                {
                    string filename = myCache[MyCache_Idx].FileName;
                    FileNames_Sim.Remove(filename);
                    FileNames.Remove(filename);
                    MyCache.RemoveAt(MyCache_Idx);
                }
            }
            catch
            {
                MessageBox.Show("ɾ���ļ�ʧ�ܣ���ȷ���Ƿ�ѡ�ж�Ӧ�ļ���", "����", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        #region [����ָ��]
        private void updataIndicator()
        {
            if (Curr_train_idx >= 0)
            {

                Description = Data.Rows[Curr_train_idx][1].ToString();
                EOSA_Time = Data.Rows[Curr_train_idx][3].ToString();
                Total_Mileage = Data.Rows[Curr_train_idx][5].ToString();
                Dist = Data.Rows[Curr_train_idx][4].ToString();
                Train_Speed = Data.Rows[Curr_train_idx][6].ToString();
                Braking_Level = Data.Rows[Curr_train_idx][10].ToString();

            }
        }    //��������ָ��
        #endregion

        #region [���г�����]
        private void addLogFile()
        {
            MultyTrainDataGrid propTmp = new MultyTrainDataGrid();
            try
            {
                propTmp.FileName = FileNames_Sim[Curr_fileMult_idx];
                propTmp.ChartType = MyMultyChartTypes[MyMultychartTypesIdx];
                propTmp.Granularity = new ObservableCollection<string>() { "0.5��", "1��", "1.5��" };
                propTmp.MyGraItem = Granularity.����;
                propTmp.Gra_idx = -1;
                if (LogFileProp_Mult.Count + 1 > TrainNumber)
                {
                    TrainNumber = LogFileProp_Mult.Count + 1;
                }
                else
                    TrainNumber++;
                propTmp.Number = TrainNumber;
                LogFileProp_Mult.Add(propTmp);
            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("����δ֪������ȷ����ѡ��ø���ָ��", "����", MessageBoxButton.OK, MessageBoxImage.Error);
            }


        } //���г�
        private void showMultTrain()
        {
            MyCharts.Clear();
            for (int j = 0; j < LogFileProp_Mult.Count; j++)
            {
                string thisTemp = " ";
                double Gra_Temp = 1.0;
                if (LogFileProp_Mult[j].MyGraItem == Granularity.����һ��)
                    Gra_Temp = 2;
                else if (LogFileProp_Mult[j].MyGraItem == Granularity.��Сһ��)
                    Gra_Temp = 0.5;
                else if (LogFileProp_Mult[j].MyGraItem == Granularity.����)
                    Gra_Temp = 1.0;
                try
                {
                    thisTemp = LogFileProp_Mult[j].ChartType;
                }
                catch
                {
                    MessageBox.Show("����δѡ��ͼ�����ͣ�", "����", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                ChartDatas TempChartData = new ChartDatas();

                for (int i = 0; i < FileNames_Sim.Count; i++)
                {
                    if (LogFileProp_Mult[j].FileName == FileNames_Sim[i])
                    {
                        TempChartData = MyMultyChartDatas[i];
                        break;
                    }
                }

                if (thisTemp == "����ʾ")
                    continue;
                if (thisTemp == "V-Tͼ" || thisTemp == "S-Tͼ" || thisTemp == "S-Vͼ")
                {
                    LiveChartParemeters temp_Chart = new LiveChartParemeters();
                    var mapper = Mappers.Xy<MeasureModel>()
                        .X(model => model.X)
                        .Y(model => model.Y);
                    Charting.For<MeasureModel>(mapper);
                    ChartValues<MeasureModel> valuesTemp = new ChartValues<MeasureModel>();
                    string titleTemp = "";
                    double max = 0;
                    if (thisTemp == "V-Tͼ")
                    {
                        double max_time = 0;
                        for (int i = 0; i < TempChartData.Speed.Count; i++)
                        {

                            valuesTemp.Add(new MeasureModel
                            {
                                X = TempChartData.Time[i],
                                Y = TempChartData.Speed[i]
                            });
                            if (TempChartData.Speed[i] > max)
                            {
                                max = TempChartData.Speed[i];
                            }
                            if (TempChartData.Time[i] > max_time)
                            {
                                max_time = TempChartData.Time[i];
                            }
                        }
                        temp_Chart.Title_X = "Time"; temp_Chart.Title_Y = "TrainSpeed";
                        temp_Chart.Width_MyChart = max_time * 10 * Gra_Temp;
                        titleTemp = "V-Tͼ";
                        temp_Chart.MaxValue_MyChart = max * 1.1;
                        temp_Chart.Step_X = 5;
                        temp_Chart.Step_Y = max / 2;
                    }
                    if (thisTemp == "S-Tͼ")
                    {
                        double max_time = 0;
                        for (int i = 0; i < TempChartData.TrainPosition.Count; i++)
                        {
                            valuesTemp.Add(new MeasureModel
                            {
                                X = TempChartData.Time[i],
                                Y = TempChartData.TrainPosition[i]
                            });
                            if (TempChartData.TrainPosition[i] > max)
                            {
                                max = TempChartData.TrainPosition[i];
                            }
                            if (TempChartData.Time[i] > max_time)
                            {
                                max_time = TempChartData.Time[i];
                            }
                        }
                        temp_Chart.Title_X = "Time"; temp_Chart.Title_Y = "TrainPosition";
                        temp_Chart.Width_MyChart = max_time * 10 * Gra_Temp;
                        titleTemp = "S-Tͼ";
                        temp_Chart.MaxValue_MyChart = max * 1.1;
                        temp_Chart.Step_X = 5;
                        temp_Chart.Step_Y = max / 2;
                    }
                    if (thisTemp == "S-Vͼ")
                    {
                        double max_position = 0;
                        for (int i = 0; i < TempChartData.TrainPosition.Count; i++)
                        {
                            valuesTemp.Add(new MeasureModel
                            {
                                Y = TempChartData.Speed[i],
                                X = TempChartData.TrainPosition[i]
                            });
                            if (TempChartData.Speed[i] > max)
                            {
                                max = TempChartData.Speed[i];
                            }
                            if (TempChartData.TrainPosition[i] > max_position)
                            {
                                max_position = TempChartData.TrainPosition[i];
                            }
                        }
                        temp_Chart.Title_Y = "TrainSpeed";
                        temp_Chart.Title_X = "TrainPosition";
                        temp_Chart.Width_MyChart = max_position * 10 * Gra_Temp;
                        titleTemp = "S-Vͼ";
                        temp_Chart.MaxValue_MyChart = max * 1.1;
                        if (max_position > 20000)
                            temp_Chart.Step_X = (int)max_position / 1000;
                        else
                            temp_Chart.Step_X = (int)max_position / 100;
                        temp_Chart.Step_Y = max / 10;
                        temp_Chart.Step_Y = max / 2;
                    }
                    temp_Chart.MinValue_MyChart = 0;
                    temp_Chart.Height_MyChart = Height / 2.7;
                    temp_Chart.SeriesCollection = new SeriesCollection{
                    new LineSeries
                        {
                    Values = valuesTemp,
                    LineSmoothness = 0,
                    PointGeometry = null,
                    Stroke=Brushes.Red,
                    StrokeThickness = 2,
                    Title = titleTemp,
                        } };
                    MyCharts.Add(temp_Chart);
                }
                else if (thisTemp == "Statusͼ")
                {
                    LiveChartParemeters temp_Chart = new LiveChartParemeters();
                    var mapper = Mappers.Xy<MeasureModel>()
                        .X(model => model.X)
                        .Y(model => model.Y);
                    Charting.For<MeasureModel>(mapper);
                    ChartValues<MeasureModel> valuesTemp = new ChartValues<MeasureModel>();
                    double max = 0;
                    for (int i = 1; i < TempChartData.Status.Count; i++)
                    {
                        if ((i >= 1 && TempChartData.Status[i - 1] != TempChartData.Status[i]) || i == 0)
                            valuesTemp.Add(new MeasureModel
                            {
                                X = TempChartData.TrainPosition[i],
                                Y = TempChartData.Status[i],
                            });
                        if (TempChartData.TrainPosition[i] >= max)
                        {
                            max = TempChartData.TrainPosition[i];
                        }
                    }
                    temp_Chart.MaxValue_MyChart = 2;
                    temp_Chart.Height_MyChart = Height / 2.7;
                    temp_Chart.Width_MyChart = max * 10 * Gra_Temp; ;
                    temp_Chart.MinValue_MyChart = 0;
                    if (max > 20000)
                        temp_Chart.Step_X = (int)max / 1000;
                    else
                        temp_Chart.Step_X = (int)max / 100;
                    temp_Chart.Step_Y = max / 10;
                    temp_Chart.Step_Y = 1;
                    temp_Chart.Title_X = "TrainPosition";
                    temp_Chart.Title_Y = "Status";
                    temp_Chart.Label_Y = new ObservableCollection<string>() { "Brake", "Coast", "Traction" };
                    temp_Chart.SeriesCollection = new SeriesCollection{
                    new StepLineSeries
                        {
                    Values = valuesTemp,
                    PointGeometry = null,
                    Stroke=Brushes.CadetBlue,
                    StrokeThickness = 2,
                    Fill= Brushes.Green,
                    AlternativeStroke= Brushes.Transparent,
                    Title="Statu״̬"
                        },
                };
                    MyCharts.Add(temp_Chart);
                }
                else if (thisTemp == "ACCͼ")
                {
                    LiveChartParemeters temp_Chart = new LiveChartParemeters();
                    var mapper = Mappers.Xy<MeasureModel>()
                        .X(model => model.X)
                        .Y(model => model.Y);
                    Charting.For<MeasureModel>(mapper);
                    ChartValues<MeasureModel> valuesTemp1 = new ChartValues<MeasureModel>();
                    ChartValues<MeasureModel> valuesTemp2 = new ChartValues<MeasureModel>();
                    ChartValues<MeasureModel> valuesTemp3 = new ChartValues<MeasureModel>();

                    double max = 0;
                    double min = 0;
                    double max_position = 0;
                    for (int i = 0; i < TempChartData.TrainAcc.Count; i++)
                    {
                        if (min >= TempChartData.TrainAcc[i] && min - TempChartData.TargetAcc[i] <= 10)
                            min = TempChartData.TrainAcc[i];
                        if (min >= TempChartData.TargetAcc[i] && min - TempChartData.TargetAcc[i] <= 10)
                            min = TempChartData.TargetAcc[i];
                        if (max <= TempChartData.TrainAcc[i] && TempChartData.TrainAcc[i] - max <= 10)
                            max = TempChartData.TrainAcc[i];
                        if (max <= TempChartData.TargetAcc[i] && TempChartData.TrainAcc[i] - max <= 10)
                            max = TempChartData.TargetAcc[i];
                        if (TempChartData.TrainPosition[i] > max_position)
                        {
                            max_position = TempChartData.TrainPosition[i];
                        }
                        valuesTemp1.Add(new MeasureModel
                        {
                            X = TempChartData.TrainPosition[i],
                            Y = TempChartData.DeltaAcc[i],
                        });
                        valuesTemp2.Add(new MeasureModel
                        {
                            X = TempChartData.TrainPosition[i],
                            Y = TempChartData.TrainAcc[i],
                        });
                        valuesTemp3.Add(new MeasureModel
                        {
                            X = TempChartData.TrainPosition[i],
                            Y = TempChartData.TargetAcc[i],
                        });
                    }
                    temp_Chart.MaxValue_MyChart = max * 1.1;
                    temp_Chart.Height_MyChart = Height / 2.7;
                    temp_Chart.MinValue_MyChart = min * 1.1;
                    temp_Chart.Width_MyChart = max_position * 20 * Gra_Temp;
                    if (max_position > 20000)
                        temp_Chart.Step_X = (int)max_position / 1000;
                    else
                        temp_Chart.Step_X = (int)max_position / 100;
                    temp_Chart.Step_Y = max / 2;
                    temp_Chart.Title_X = "TrainPosition";
                    temp_Chart.SeriesCollection = new SeriesCollection{
                    new ColumnSeries
                        {
                    Fill = Brushes.Black,
                    Values = valuesTemp1,
                    PointGeometry = null,
                    Stroke=Brushes.Blue,
                    Foreground = Brushes.Blue,
                    StrokeThickness = 6,
                    Title = "Delta ACC",
                    Visibility = Visibility.Visible,
                    MaxColumnWidth = 10,
                        },
                    new LineSeries
                    {
                    Fill=Brushes.Transparent,
                    Values = valuesTemp2,
                    LineSmoothness = 1,
                    PointGeometry = null,
                    Stroke=Brushes.DarkRed,
                    StrokeThickness = 2,
                    Title = "TrainACC"
                    },
                    new LineSeries{
                    Fill=Brushes.Transparent,
                    Values = valuesTemp3,
                    LineSmoothness = 1,
                    PointGeometry = null,
                    Stroke=Brushes.Coral,
                    StrokeThickness = 2,
                    Title="TargetACC"
                    },
                };
                    MyCharts.Add(temp_Chart);
                }
            }
        }

        private void deleteMultTrain()
        {

            try
            {
                LogFileProp_Mult.RemoveAt(Curr_MultProp_idx);
                showMultTrain();
            }
            catch
            {
                MessageBox.Show("����δ֪������ȷ����ѡ��ø���ָ��", "����", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        private void moveDown()
        {
            try
            {
                var idx = Curr_MultProp_idx;
                MultyTrainDataGrid tmp = LogFileProp_Mult[idx];
                if (LogFileProp_Mult.Count != idx + 1)
                {
                    LogFileProp_Mult.RemoveAt(idx);
                    LogFileProp_Mult.Insert(idx + 1, tmp);
                }
            }
            catch
            {
                MessageBox.Show("����δ֪������ȷ����ѡ��ø���ָ��", "����", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void moveUp()
        {
            try
            {
                var idx = Curr_MultProp_idx;
                MultyTrainDataGrid tmp = LogFileProp_Mult[idx];
                if (idx != 0)
                {
                    LogFileProp_Mult.RemoveAt(idx);
                    LogFileProp_Mult.Insert(idx - 1, tmp);
                }
            }
            catch
            {
                MessageBox.Show("����δ֪������ȷ����ѡ��ø���ָ��", "����", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void showMulTrainRadio()
        {
            if(MyCharts!=null)
            {
                MyCharts.Clear();
            }
        }
        #endregion

        #region [���г�����]
        private void showSinTrainRadio()
        {
            if (MyCharts != null)
            {
                MyCharts.Clear();
            }
        }
        private void showSingleTrain()
        {
            if (MySinChartDatass == null)
            {
                MessageBox.Show("����δ֪������ȷ����ѡ����ļ���", "֪ͨ", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            string thisTemp = "";
            string thisGra = " ";
            try { thisTemp = MySinChartTypes[MySinChartTypesIdx]; }
            catch
            {
                MessageBox.Show("����δѡ��ͼ�����ͣ�", "֪ͨ", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            try { thisGra = MySinGra[MySinGraIdx]; }
            catch
            {
                MessageBox.Show("����δѡ��ʱ�����ȣ�", "֪ͨ", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            double times = 1.0;
            if (thisGra == "����һ��")
            {
                times = 2.0;
            }
            else if (thisGra == "��Сһ��")
            {
                times = 0.5;
            }
            MyCharts.Clear();
            if (thisTemp == "V-Tͼ" || thisTemp == "S-Tͼ" || thisTemp == "S-Vͼ")
            {
                LiveChartParemeters temp_Chart = new LiveChartParemeters();
                var mapper = Mappers.Xy<MeasureModel>()
                    .X(model => model.X)
                    .Y(model => model.Y);
                Charting.For<MeasureModel>(mapper);
                ChartValues<MeasureModel> valuesTemp = new ChartValues<MeasureModel>();
                string titleTemp = "";
                double max = 0;
                if (thisTemp == "V-Tͼ")
                {
                    double max_time = 0;
                    for (int i = 0; i < MySinChartDatass.Speed.Count; i++)
                    {
                        valuesTemp.Add(new MeasureModel
                        {
                            X = MySinChartDatass.Time[i],
                            Y = MySinChartDatass.Speed[i]
                        });
                        if (MySinChartDatass.Speed[i] > max)
                        {
                            max = MySinChartDatass.Speed[i];
                        }
                        if (MySinChartDatass.Time[i] > max_time)
                        {
                            max_time = MySinChartDatass.Time[i];
                        }
                    }
                    temp_Chart.Title_X = "Time";
                    temp_Chart.Title_Y = "TrainSpeed";
                    temp_Chart.Width_MyChart = max_time * 10 * times;
                    titleTemp = "V-Tͼ";
                    temp_Chart.MaxValue_MyChart = max * 1.08;
                    temp_Chart.Step_X = 5;
                    temp_Chart.Step_Y = max / 10;
                }
                else if (thisTemp == "S-Tͼ")
                {
                    double max_time = 0;

                    for (int i = 0; i < MySinChartDatass.TrainPosition.Count; i++)
                    {
                        valuesTemp.Add(new MeasureModel
                        {
                            X = MySinChartDatass.Time[i],
                            Y = MySinChartDatass.TrainPosition[i]
                        });
                        if (MySinChartDatass.TrainPosition[i] > max)
                        {
                            max = MySinChartDatass.TrainPosition[i];
                        }
                        if (MySinChartDatass.Time[i] > max_time)
                        {
                            max_time = MySinChartDatass.Time[i];
                        }
                    }
                    temp_Chart.Title_X = "Time";
                    temp_Chart.Title_Y = "TrainPosition";
                    temp_Chart.Width_MyChart = max_time * 10 * times;
                    titleTemp = "S-Tͼ";
                    temp_Chart.MaxValue_MyChart = max * 1.08;
                    temp_Chart.Step_X = 5;
                    temp_Chart.Step_Y = max / 10;
                }
                else if (thisTemp == "S-Vͼ")
                {
                    double max_position = 0;
                    for (int i = 0; i < MySinChartDatass.TrainPosition.Count; i++)
                    {
                        valuesTemp.Add(new MeasureModel
                        {
                            Y = MySinChartDatass.Speed[i],
                            X = MySinChartDatass.TrainPosition[i]
                        });
                        if (MySinChartDatass.Speed[i] > max)
                        {
                            max = MySinChartDatass.Speed[i];
                        }
                        if (MySinChartDatass.TrainPosition[i] > max_position)
                        {
                            max_position = MySinChartDatass.TrainPosition[i];
                        }
                    }
                    temp_Chart.Title_Y = "TrainSpeed";
                    temp_Chart.Title_X = "TrainPosition";
                    temp_Chart.Width_MyChart = max_position * 10 * times;
                    titleTemp = "S-Vͼ";
                    temp_Chart.MaxValue_MyChart = max * 1.08;
                    if (max_position > 20000)
                        temp_Chart.Step_X = (int)max_position / 1000;
                    else temp_Chart.Step_X = (int)max_position / 100;
                    temp_Chart.Step_Y = max / 10;
                }
                temp_Chart.MinValue_MyChart = 0;
                temp_Chart.Height_MyChart = Height / 1.3;
                temp_Chart.SeriesCollection = new SeriesCollection{
                    new LineSeries
                        {
                    Values = valuesTemp,
                    LineSmoothness = 0,
                    PointGeometry = null,
                    Stroke=Brushes.Red,
                    StrokeThickness = 2,
                    Title = titleTemp,
                        },
                };
                MyCharts.Add(temp_Chart);
            }
            else if (thisTemp == "Statusͼ")
            {
                LiveChartParemeters temp_Chart = new LiveChartParemeters();
                var mapper = Mappers.Xy<MeasureModel>()
                    .X(model => model.X)
                    .Y(model => model.Y);
                Charting.For<MeasureModel>(mapper);
                ChartValues<MeasureModel> valuesTemp = new ChartValues<MeasureModel>();
                double max = 0;
                for (int i = 1; i < MySinChartDatass.Status.Count; i++)
                {
                    if ((i >= 1 && MySinChartDatass.Status[i - 1] != MySinChartDatass.Status[i]) || i == 0)
                        valuesTemp.Add(new MeasureModel
                        {
                            X = MySinChartDatass.TrainPosition[i],
                            Y = MySinChartDatass.Status[i],
                        });
                    if (MySinChartDatass.TrainPosition[i] >= max)
                    {
                        max = MySinChartDatass.TrainPosition[i];
                    }
                }
                temp_Chart.MaxValue_MyChart = 2;
                temp_Chart.Height_MyChart = Height / 1.3;
                temp_Chart.Width_MyChart = max * 10 * times;
                temp_Chart.MinValue_MyChart = 0;
                if (max > 20000)
                    temp_Chart.Step_X = (int)max / 1000;
                else temp_Chart.Step_X = (int)max / 100;
                temp_Chart.Step_Y = max / 10;
                temp_Chart.Step_Y = 1;
                temp_Chart.Label_Y = new ObservableCollection<string>() { "Brake", "Coast", "Traction" };
                temp_Chart.Title_X = "TrainPosition";
                temp_Chart.Title_Y = "Status";

                temp_Chart.SeriesCollection = new SeriesCollection{
                    new StepLineSeries
                        {
                    Values = valuesTemp,
                    PointGeometry = null,
                    Stroke=Brushes.CadetBlue,
                    StrokeThickness = 2,
                    Fill= Brushes.Green,
                    AlternativeStroke= Brushes.Transparent,
                    Title = "Status״̬"
                        },
                };
                MyCharts.Add(temp_Chart);
            }
            else if (thisTemp == "ACCͼ")
            {
                LiveChartParemeters temp_Chart = new LiveChartParemeters();
                var mapper = Mappers.Xy<MeasureModel>()
                    .X(model => model.X)
                    .Y(model => model.Y);
                Charting.For<MeasureModel>(mapper);
                ChartValues<MeasureModel> valuesTemp1 = new ChartValues<MeasureModel>();
                ChartValues<MeasureModel> valuesTemp2 = new ChartValues<MeasureModel>();
                ChartValues<MeasureModel> valuesTemp3 = new ChartValues<MeasureModel>();

                double max = 0;
                double min = 0;
                double max_position = 0;
                for (int i = 0; i < MySinChartDatass.TrainAcc.Count; i++)
                {
                    if (min >= MySinChartDatass.TrainAcc[i] && min - MySinChartDatass.TargetAcc[i] <= 10)
                        min = MySinChartDatass.TrainAcc[i];
                    if (min >= MySinChartDatass.TargetAcc[i] && min - MySinChartDatass.TargetAcc[i] <= 10)
                        min = MySinChartDatass.TargetAcc[i];
                    if (max <= MySinChartDatass.TrainAcc[i] && MySinChartDatass.TrainAcc[i] - max <= 10)
                        max = MySinChartDatass.TrainAcc[i];
                    if (max <= MySinChartDatass.TargetAcc[i] && MySinChartDatass.TrainAcc[i] - max <= 10)
                        max = MySinChartDatass.TargetAcc[i];
                    if (MySinChartDatass.TrainPosition[i] > max_position)
                    {
                        max_position = MySinChartDatass.TrainPosition[i];
                    }
                    valuesTemp1.Add(new MeasureModel
                    {
                        X = MySinChartDatass.TrainPosition[i],
                        Y = MySinChartDatass.DeltaAcc[i],
                    });
                    valuesTemp2.Add(new MeasureModel
                    {
                        X = MySinChartDatass.TrainPosition[i],
                        Y = MySinChartDatass.TrainAcc[i],
                    });
                    valuesTemp3.Add(new MeasureModel
                    {
                        X = MySinChartDatass.TrainPosition[i],
                        Y = MySinChartDatass.TargetAcc[i],
                    });
                }
                temp_Chart.MaxValue_MyChart = max * 1.08;
                temp_Chart.Height_MyChart = Height / 1.3;
                temp_Chart.MinValue_MyChart = min * 1.08;
                temp_Chart.Width_MyChart = max_position * 20 * times;
                if (max_position > 20000)
                    temp_Chart.Step_X = (int)max_position / 1000;
                else
                    temp_Chart.Step_X = (int)max_position / 100;
                temp_Chart.Step_Y = max / 10;
                temp_Chart.Step_Y = 1;
                temp_Chart.Title_X = "TrainPosition";
                temp_Chart.SeriesCollection = new SeriesCollection{
                    new ColumnSeries
                        {
                    Fill = Brushes.Black,
                    Values = valuesTemp1,
                    PointGeometry = null,
                    Stroke=Brushes.Blue,
                    Foreground = Brushes.Blue,
                    StrokeThickness = 6,
                    Title = "Delta ACC",
                    Visibility = Visibility.Visible,
                    MaxColumnWidth = 10,
                        },
                    new LineSeries
                    {
                    Fill=Brushes.Transparent,
                    Values = valuesTemp2,
                    LineSmoothness = 1,
                    PointGeometry = null,
                    Stroke=Brushes.DarkRed,
                    StrokeThickness = 2,
                    Title = "TrainACC"
                    },
                    new LineSeries{
                    Fill=Brushes.Transparent,
                    Values = valuesTemp3,
                    LineSmoothness = 1,
                    PointGeometry = null,
                    Stroke=Brushes.Coral,
                    StrokeThickness = 2,
                    Title="TargetACC"
                    },
                };
                MyCharts.Add(temp_Chart);
            }
        }
        #endregion

        #region[ASC]
        private void read_AscFiles()
        {

            System.Windows.Forms.OpenFileDialog openfiledialog = new System.Windows.Forms.OpenFileDialog();
            openfiledialog.Filter = "(*.txt)|*.txt";
            openfiledialog.DefaultExt = "txt";
            if (openfiledialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var ext_idx = openfiledialog.FileName.LastIndexOf(".") + 1;
                var ext = openfiledialog.FileName.Substring(ext_idx, 3);
                AscFileNames.Add(openfiledialog.FileName);
                
                if (ext != "txt")
                {
                    MessageBox.Show("���ȡTXT�ļ���", "����", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                var start_idx = openfiledialog.FileName.LastIndexOf("\\") + 1;
                AscSimFileNames.Add(openfiledialog.FileName.Substring(start_idx, openfiledialog.FileName.LastIndexOf(".") - start_idx));
                Curr_Ascpara_idx = -1;
                FileCache temp = new FileCache();
                temp.FileName = openfiledialog.FileName.Substring(start_idx, openfiledialog.FileName.LastIndexOf(".") - start_idx);
                temp.FileType = "ASC�ļ�";
                myCache.Add(temp);
                MessageBox.Show("��ȡTXT�ļ��ɹ���", "֪ͨ", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        private void show_AscDatas()
        {
            try
            {
                OriDatas_Asc = new BindingList<string>();
                using (StreamReader read_Asc = new StreamReader(AscFileNames[Curr_Ascfile_idx], Encoding.Default))
                {
                    int lineCount = 0;
                    while (read_Asc.Peek() > 0)
                    {
                        lineCount++;
                        string temp = read_Asc.ReadLine();
                        if (!temp.Contains("#"))
                            OriDatas_Asc.Add(temp);
                    }
                }
                ProcessedData_Asc.Clear();
                for (int i = 0; i < OriDatas_Asc.Count; i++)
                {

                    int idx_begin;
                    int idx_end;
                    int length;
                    AscDatas tempPara = new AscDatas();
                    string temp = OriDatas_Asc[i];

                    idx_end = temp.IndexOf("=");
                    tempPara.Name = temp.Substring(0, idx_end).TrimEnd();

                    idx_begin = temp.IndexOf("<") + 1;
                    idx_end = temp.IndexOf(">") - 1;
                    length = idx_end - idx_begin + 1;
                    try { tempPara.Range = temp.Substring(idx_begin, length); }
                    catch { tempPara.Range = ""; }

                    idx_begin = temp.IndexOf(">") + 1;
                    idx_end = temp.IndexOf("@") - 1;
                    length = idx_end - idx_begin + 1;
                    try { tempPara.Data = temp.Substring(idx_begin, length).Trim(); }
                    catch { tempPara.Data = temp.Substring(idx_begin).Trim(); }

                    idx_begin = temp.IndexOf("@") + 1;
                    tempPara.Tips = temp.Substring(idx_begin);

                    idx_begin = temp.IndexOf("(");
                    idx_end = temp.LastIndexOf(")");
                    length = idx_end - idx_begin;
                    try { tempPara.Data_Property = temp.Substring(idx_begin, length); }
                    catch { }

                    tempPara.Asc_Background = default;

                    ProcessedData_Asc.Add(tempPara);
                }
                Text_Save = "δ����";
                Background_Save.Color = Colors.Red;
                Text_Change = "δ�޸�";
                Background_Change.Color = Colors.Red;
                Last_Ascidx = -1;
            }
            catch
            {
                MessageBox.Show("����δ֪������ȷ����ѡ���ļ���", "����", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void save_AscNewFiles()
        {
            try
            {
                var saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "(*.txt)|*.txt";
                saveFileDialog.DefaultExt = "txt";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Save_NewDatas = new List<string>();
                    if (save_AscNewDatas() == false)
                    {
                        return;
                    }
                    string fileNames = saveFileDialog.FileName;
                    StreamWriter ascWrite = new StreamWriter(fileNames);
                    foreach (string item in Save_NewDatas)
                    {
                        ascWrite.WriteLine(item);
                    }
                    ascWrite.Close();
                    Background_Save.Color = Colors.Green;
                    Text_Save = "�ѱ���";
                    MessageBox.Show("������ɣ�", "֪ͨ", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch
            {
                MessageBox.Show("����δ֪������ȷ�����ΪTXT�ļ�", "����", MessageBoxButton.OK, MessageBoxImage.Error);
            }


        }
        private bool save_AscNewDatas()
        {

            if (OriDatas_Asc == null || OriDatas_Asc.Count == 0)
            {
                MessageBox.Show("û��Ҫ�����ASC���ݣ�", "����", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            for (int i = 0; i < OriDatas_Asc.Count - 2; i++)
            {
                string temp = ProcessedData_Asc[i].Name + " = " + ProcessedData_Asc[i].Data_Property + '<' + ProcessedData_Asc[i].Range + '>'
                    + ProcessedData_Asc[i].Data + " @" + ProcessedData_Asc[i].Tips;
                Save_NewDatas.Add(temp);
            }
            Save_NewDatas.Add("#ATOEND");
            Save_NewDatas.Insert(0, "#ATO");
            return true;
        }
        #endregion

        #endregion

        public MainViewModel(double myheight)  //ViewModel���캯��
        {
            this.Height = myheight;
            InitCommands();
            InitProperties();
        }
    }

}
