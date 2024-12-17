using TestItemTemplate.ViewModels.Pages;
using Wpf.Ui.Controls;

namespace TestItemTemplate.Views.Pages
{
    [ObservableObject]
    public partial class SettingsPage : INavigableView<SettingsViewModel>
    {
        [ObservableProperty]
        private int snLen = 10;
        public SettingsViewModel ViewModel { get; }

        public SettingsPage(SettingsViewModel viewModel)
        {
            ViewModel = viewModel;
            DataContext = this;

            InitializeComponent();
        }
        private void SnRuleTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            SnLen = SnRuleTextBox.Text.Length;
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            SnLen = SnRuleTextBox.Text.Length;
        }

        private void ToggleSwitch_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void ToggleSwitch_Unchecked(object sender, RoutedEventArgs e)
        {

        }
    }
}
