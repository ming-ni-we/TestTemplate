using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TestItemTemplate.Models;
using TestItemTemplate.ViewModels.Pages;
using Wpf.Ui.Controls;

namespace TestItemTemplate.Views.Pages
{
    public partial class DashboardPage : INavigableView<DashboardViewModel>
    {
        public DashboardViewModel ViewModel { get; }

        public DashboardPage(DashboardViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();
            testItemsDataGrid1.ItemsSource = viewModel.TestItems;
        }

        private void channelText_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {

        }

        private string Mess = "";
        private void snCodeText_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                bool isSfRule = true;
                if (snCodeText.Text.Length != Settings.Default.snLength)
                {
                    isSfRule = false;
                }
                else
                {
                    for (int i = 0; i < Settings.Default.snLength; i++)
                    {
                        if (Settings.Default.snRule[i] != '*')
                        {
                            if (Settings.Default.snRule[i] != snCodeText.Text[i])
                            {
                                isSfRule = false;
                                break;
                            }
                        }
                    }
                }
                if (Settings.Default.IsMes)
                {
                    if (isSfRule)
                    {
                        if (ViewModel.isTestRunning)
                        {
                            System.Windows.MessageBox.Show("正在测试中！");
                        }
                        else
                        {
                            if (ViewModel.mes.Start_New_CShape(snCodeText.Text, "1", "", " ",
                                " ", " ", ref Mess))
                            {
                                ViewModel.SncodeStore = snCodeText.Text;
                                Task.Run(() =>
                                {
                                    ViewModel.StartTest();
                                });
                            }
                            else
                            {
                                System.Windows.MessageBox.Show("过站失败\n" + Mess);
                            }
                        }
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("条码规程检测失败，请查看条码或更改条码规则！");
                    }
                    snCodeText.Text = "";
                    snCodeText.Focus();
                }
                else
                {
                    if (ViewModel.isTestRunning)
                    {
                        System.Windows.MessageBox.Show("正在测试中！");
                    }
                    else
                    {
                        Task.Run(() =>
                        {
                            ViewModel.StartTest();
                        });
                    }
                    snCodeText.Text = "";
                    snCodeText.Focus();
                }
            }
            else
            {
                foreach (var item in ViewModel.TestItems)
                {
                    Debug.WriteLine(item.Name + item.IsTest.ToString());
                }
            }
        }

        private void ToggleSwitch_Checked(object sender, RoutedEventArgs e)
        {
            ViewModel.IORelay.SendIOState();
        }

        private void ToggleSwitch_Unchecked(object sender, RoutedEventArgs e)
        {
            ViewModel.IORelay.SendIOState();
        }

        // 当前拖拽的项
        private testItem _draggedItem;
        // 捕获鼠标按下事件，记录要拖拽的项
        private void DataGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is FrameworkElement element && element.DataContext is not null)
            {
                var dataGridRow = FindAncestor<DataGridRow>(element);
                if (dataGridRow != null)
                {
                    // 如果事件源是 CheckBox，不触发拖放操作
                    if (HasCheckBoxParent(element))
                    {
                        return;
                    }

                    var dataGrid = (System.Windows.Controls.DataGrid)sender;
                    dataGridRow = FindVisualParent<DataGridRow>(e.OriginalSource as DependencyObject);
                    if (dataGridRow == null) return;

                    _draggedItem = dataGridRow.Item as testItem;

                    if (_draggedItem != null)
                    {
                        DragDrop.DoDragDrop(dataGridRow, _draggedItem, DragDropEffects.Move);
                    }
                }
            }
        }
        private bool HasCheckBoxParent(DependencyObject element)
        {
            while (element != null)
            {
                // 如果父级是 CheckBox，则返回 true
                if (element is CheckBox)
                {
                    return true;
                }

                // 获取父级控件
                element = VisualTreeHelper.GetParent(element);
            }

            return false; // 未找到 CheckBox 父级
        }
        private void CheckBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var checkbox = (System.Windows.Controls.CheckBox)sender;
            if (checkbox != null)
            {

                // 获取 CheckBox 的绑定对象
                var dataContext = checkbox.DataContext;

                // 确保绑定对象不为空，并且是你的数据模型类型
                if (dataContext is testItem model)
                {
                    // 修改绑定的布尔属性值
                    model.IsTest = !model.IsTest;

                    // 如果需要手动更新界面，可以触发通知（取决于你的数据模型实现了 INotifyPropertyChanged）
                }
            }
            e.Handled = true; // 阻止事件向上冒泡
        }
        // 辅助方法：查找控件的父级
        private T FindAncestor<T>(DependencyObject current) where T : DependencyObject
        {
            while (current != null && current is not T)
            {
                current = VisualTreeHelper.GetParent(current);
            }
            return current as T;
        }
        // 在拖拽过程中阻止其他效果
        private void DataGrid_DragOver(object sender, DragEventArgs e)
        {
            // 仅当拖放有效时处理事件
            if (!e.Handled)
            {
                e.Effects = DragDropEffects.Move;
                e.Handled = true;
            }
        }

        // 接收拖拽，调整集合顺序
        private void DataGrid_Drop(object sender, DragEventArgs e)
        {
            if (_draggedItem == null) return;

            var dataGrid = (System.Windows.Controls.DataGrid)sender;
            var droppedItem = GetRowItemFromPoint((System.Windows.Controls.DataGrid)dataGrid, e.GetPosition(dataGrid));

            if (droppedItem == null || ReferenceEquals(droppedItem, _draggedItem)) return;

            // 调整数据源的顺序
            int oldIndex = ViewModel.TestItems.IndexOf(_draggedItem);
            int newIndex = ViewModel.TestItems.IndexOf(droppedItem);

            ViewModel.TestItems.Move(oldIndex, newIndex);
            _draggedItem = null;
        }

        // 阻止拖拽进入的视觉变化
        private void DataGrid_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Move;
            e.Handled = true;
        }

        // 辅助方法：获取鼠标指向的 DataGrid 行
        private static T FindVisualParent<T>(DependencyObject child) where T : DependencyObject
        {
            var parentObject = VisualTreeHelper.GetParent(child);

            if (parentObject == null) return null;

            if (parentObject is T parent)
                return parent;

            return FindVisualParent<T>(parentObject);
        }

        private testItem GetRowItemFromPoint(System.Windows.Controls.DataGrid dataGrid, Point point)
        {
            var hitTestResult = VisualTreeHelper.HitTest(dataGrid, point);
            if (hitTestResult == null) return null;

            var row = FindVisualParent<DataGridRow>(hitTestResult.VisualHit);
            return row?.Item as testItem;
        }
    }
}
