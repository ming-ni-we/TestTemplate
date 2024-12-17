using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Documents;
using TestItemTemplate.Helpers;
using TestItemTemplate.Models;

namespace TestItemTemplate.ViewModels.Pages
{
    public partial class DashboardViewModel : ObservableObject,IDisposable
    {
        public static string LoginUser = "";
        public MES2_0_VC6_JSON.MES2_0_VC6_JSON mes;
        [ObservableProperty]
        ObservableCollection<testItem> testItems = new ObservableCollection<testItem>() {
                new testItem("测试时间1", $"",
                0, ItemState.未测试, "",0, 0,""),
                new testItem("测试时间2", $"",
                0, ItemState.未测试, "",0, 0,""),
                new testItem("测试时间3", $"",
                0, ItemState.未测试, "",0, 0,""),
                new testItem("测试时间4", $"",
                0, ItemState.未测试, "",0, 0,""),
                new testItem("测试时间5", $"",
                0, ItemState.未测试, "",0, 0,""),
                new testItem("测试时间6", $"",
                0, ItemState.未测试, "",0, 0,""),
        };
        public int SearchIndexbyName(string name)
        {
            for (int i = 0; i < TestItems.Count; i++)
            {
                if (TestItems[i].Name == name)
                {
                    return i;
                }
            }
            return -1;
        }

        public CANDevice canDevice = new CANDevice();

        public bool isTestRunning = false;

        private SerialCommnication commnication = new SerialCommnication("COM1", 9600);

        [ObservableProperty]
        public IORelay iORelay;
        [ObservableProperty]
        public PowerSuply3020 powerSuply3020;
        [ObservableProperty]
        public PowerSuply48 powerSuply48;
        [ObservableProperty]
        public Load2108 load2108;
        [ObservableProperty]
        public VoltageReader voltageReader = new VoltageReader();

        [ObservableProperty]
        private string infoText = "等待测试";

        [ObservableProperty]
        private string sncode = "";
        [ObservableProperty]
        private string sncodeStore = "";

        private Logger logger;

        // 定义不同的测试函数
        static void TestFunction1()
        {
            Debug.WriteLine("Test Function 1 executed.");
        }

        static void TestFunction2()
        {
            Debug.WriteLine("Test Function 2 executed.");
        }
        public DashboardViewModel()
        {
            /*            canDevice.Init_CAN1(21, 0, 0, 3);*/
            iORelay = new IORelay(commnication);

            TestItems[0].ActionToExecute += TestFunction1;
            // 循环执行每个测试项的委托
            foreach (var testItem in testItems)
            {
                testItem.Execute();
            }

            TestItems= LoadFromFile("testItems.txt");
        }

        /// <summary>
        /// 重置ui显示
        /// </summary>
        /// <param name="channel"></param>
        private void ResetInfo()
        {
            for (int i = 0; i < TestItems.Count; i++)
            {
                TestItems[i].ResetInfo();
            }
            InfoText = "测试中";
            isTestRunning = true;
            logger = new Logger(Sncode);
            startTime=DateTime.Now;
        }

        DateTime startTime;

        public void StartTest()
        {

            #region 上传mes，并统计数据
            //linq分别取出合格与不合格的数组
            var NGResult = from x in TestItems
                           where x.State == ItemState.测试失败
                           select x;
            var PassResult = from x in TestItems
                             where x.State == ItemState.测试通过
                             select x;
            if (NGResult.Count() == 0)
            {
                InfoText = "PASS";
            }
            else
            {
                InfoText = "NG";
                StopTest();
                return;
            }
            if (Settings.Default.IsMes)
            {
                List<string> list = new List<string>();

                //上传mes，传入多值
                foreach (var item in TestItems)
                {
                    logger.Log(item.Name + ":" + item.Value);
                    list.Add(new string(string.Join(";", [item.Name, item.Value.ToString(), item.MaxValue.ToString(),
                        item.MinValue.ToString(), ((item.MaxValue == 1f ? 1 : (item.MaxValue + item.MinValue)) / 2).ToString(), item.Unit,
                        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), LoginUser, "1号机位", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                              item.Result, "CCD001", SncodeStore, "null", "null"])));
                }
                string Mess = "";
                mes.TestDataCollect2MainChild_New_CShape(SncodeStore, "VH", "设备的编号",
                                            "软件的版本", "P6灯光急停开关烟雾感应测试", null, "1",
                             null, null, null, "测试时长", "总测试次数",
                             "文本", string.Join("|", list), ref Mess);

                if (NGResult.Count() == 0)
                {
                    mes.Complete_New_CShape(SncodeStore, "1", "",
                                              "", "", ref Mess);
                }
                else
                {
                    mes.NcComplete_New_CShape(SncodeStore, "FAIL", "测试未通过",
                        "测试项未通过", "", "", "",
                        "", "", "", "",
                        "", "", "", ref Mess);
                }
            }
            TestItems[10].TestTime = (int)(DateTime.Now - startTime).TotalSeconds;
            #endregion
        }
        public void StopTest()
        {
            Thread.Sleep(3000);
            isTestRunning = false;
            InfoText = "NG";
        }

        // 保存到 JSON 文件
        public void SaveToFile(ObservableCollection<testItem> testItems, string filePath)
        {
            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            string json = JsonConvert.SerializeObject(testItems, settings);
            File.WriteAllText(filePath, json);
        }

        // 从 JSON 文件加载
        public ObservableCollection<testItem> LoadFromFile(string filePath)
        {
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<ObservableCollection<testItem>>(json);
        }

        public void Dispose()
        {
            SaveToFile(TestItems, "testItems.txt");
        }
    }
}
