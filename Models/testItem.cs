using Newtonsoft.Json;

namespace TestItemTemplate.Models
{
    public enum ItemState
    {
        /// <summary>
        /// 未测试
        /// </summary>
        未测试,
        /// <summary>
        /// 测试通过
        /// </summary>
        测试通过,
        /// <summary>
        /// 测试失败
        /// </summary>
        测试失败
    }

    public partial class testItem : ObservableObject
    {
        /// <summary>
        /// 测试名称
        /// </summary>
        [ObservableProperty]
        public string name;
        /// <summary>
        /// 测试范围
        /// </summary>
        [ObservableProperty]
        public float minValue;
        /// <summary>
        /// 测试范围
        /// </summary>
        [ObservableProperty]
        public float maxValue;
        /// <summary>
        /// 测试范围
        /// </summary>
        [ObservableProperty]
        public string range;
        /// <summary>
        /// 测试值
        /// </summary>
        [ObservableProperty]
        public float value;

        /// <summary>
        /// 测试范围
        /// </summary>
        [ObservableProperty]
        public float minValuePower;
        /// <summary>
        /// 测试范围
        /// </summary>
        [ObservableProperty]
        public float maxValuePower;
        /// <summary>
        /// 测试范围
        /// </summary>
        [ObservableProperty]
        public string rangePower;
        /// <summary>
        /// 测试值
        /// </summary>
        [ObservableProperty]
        public float valuePower;

        /// <summary>
        /// 测试状态
        /// </summary>
        [ObservableProperty]
        public ItemState state;
        /// <summary>
        /// 测试结果
        /// </summary>
        [ObservableProperty]
        public string result;
        /// <summary>
        /// 测试单位
        /// </summary>
        [ObservableProperty]
        public string unit;

        /// <summary>
        /// 测试时间
        /// </summary>
        [ObservableProperty]
        public int testTime;
        /// <summary>
        /// 是否测试
        /// </summary>
        [ObservableProperty]
        public bool isTest = true;
        public testItem(string name, string range, float value, ItemState state, string result, float minValue, float maxValue, string unit)
        {
            Name = name;
            Range = range;
            Value = value;
            State = ItemState.未测试;
            Result = result;
            MinValue = minValue;
            MaxValue = maxValue;
            Unit = unit;
        }

        public void ResetInfo()
        {
            Value = 0;
            State = ItemState.未测试;
            Result = "";
        }

        public void Compare()
        {
            if (MinValue <= Value && Value <= MaxValue)
            {
                State = ItemState.测试通过;
            }
            else
            {
                State = ItemState.测试失败;
            }
        }
        // 定义一个委托
        public delegate void TestAction();

        // 定义一个委托实例
        [JsonIgnore]
        public TestAction ActionToExecute { get; set; }

        // 执行绑定的委托
        public void Execute()
        {
            ActionToExecute?.Invoke();
        }
    }
}
