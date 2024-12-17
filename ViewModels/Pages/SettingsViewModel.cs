using System.Collections.ObjectModel;
using System.IO.Ports;
using TestItemTemplate.ViewModels.Windows;
using TestItemTemplate.Views.Windows;
using Wpf.Ui.Appearance;
using Wpf.Ui.Controls;

namespace TestItemTemplate.ViewModels.Pages
{
    public partial class SettingsViewModel : ObservableObject
    {
        [ObservableProperty]
        private ApplicationTheme _currentTheme = ApplicationTheme.Unknown;

        [ObservableProperty]
        private bool isMes = false;
        /// <summary>
        /// Sn码位数
        /// </summary>
        [ObservableProperty]
        private int snLength;
        /// <summary>
        /// Sn码规则
        /// </summary>
        [ObservableProperty]
        private string snRule;
        /// <summary>
        /// 应用名称
        /// </summary>
        [ObservableProperty]
        private string applicationName;

        [ObservableProperty]
        private bool afterFailContinue;


        /// <summary>
        /// 从当前设备获取到的所有串口集合
        /// </summary>
        [ObservableProperty]
        public ObservableCollection<string> availablePorts = new ObservableCollection<string>();
        public SettingsViewModel()
        {
            InitSetting();
        }

        private void InitSetting()
        {


            IsMes = Settings.Default.IsMes;
            SnLength = Settings.Default.snLength;
            SnRule = Settings.Default.snRule;
            ApplicationName = Settings.Default.ApplicationName;
            AfterFailContinue = Settings.Default.AfterFailContinue;


            foreach (var item in SerialPort.GetPortNames())
            {
                AvailablePorts.Add(item);
            }
        }

        private void SaveSetting()
        {


            Settings.Default.IsMes = IsMes;
            Settings.Default.snLength = SnLength;
            Settings.Default.snRule = SnRule;
            Settings.Default.ApplicationName = ApplicationName;
            Settings.Default.AfterFailContinue = AfterFailContinue;

            Settings.Default.Save();
        }


        public bool isLogin = false;

        [RelayCommand]
        private void OnSaveSetting()
        {

            if (isLogin)
            {
                SaveSetting();
                Wpf.Ui.Controls.MessageBox messageBox = new Wpf.Ui.Controls.MessageBox();
                messageBox.Content = "修改配置成功";
                messageBox.Title = "提示";
                messageBox.CloseButtonText = "OK";
                messageBox.ShowDialogAsync();
            }
            else
            {
                var loginWindow = new LoginWindow();
                var mainWindow = App.GetService<MainWindow>();
                loginWindow.Owner = mainWindow;
                bool result = (bool)loginWindow.ShowDialog();
                if (result)
                {
                    isLogin = true;
                    SaveSetting();
                    Wpf.Ui.Controls.MessageBox messageBox = new Wpf.Ui.Controls.MessageBox();
                    messageBox.Content = "修改配置成功";
                    messageBox.Title = "提示";
                    messageBox.CloseButtonText = "OK";
                    messageBox.ShowDialogAsync();
                }
                else
                {
                    Wpf.Ui.Controls.MessageBox messageBox = new Wpf.Ui.Controls.MessageBox();
                    messageBox.Content = "取消保存配置";
                    messageBox.Title = "提示";
                    messageBox.CloseButtonText = "OK";
                    messageBox.ShowDialogAsync();
                }
            }
        }


    }
}
