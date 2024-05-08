using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChitChat
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			
		}

		private void registerButton_Click(object sender, RoutedEventArgs e)
		{
			usernameLabel.Visibility = Visibility.Visible;
			username.Visibility = Visibility.Visible;
			confirmPasswordLabel.Visibility = Visibility.Visible;
			confirmPassword.Visibility = Visibility.Visible;
			cancelRegistrationButton.Visibility = Visibility.Visible;
			loginButton.Visibility = Visibility.Collapsed;
        }

		private void cancelRegistrationButton_Click(object sender, RoutedEventArgs e)
		{
			usernameLabel.Visibility = Visibility.Collapsed;
            username.Visibility = Visibility.Collapsed;
			confirmPasswordLabel.Visibility = Visibility.Collapsed;
            confirmPassword.Visibility = Visibility.Collapsed;
            cancelRegistrationButton.Visibility = Visibility.Collapsed;
            loginButton.Visibility = Visibility.Visible;
        }

		private void loginButton_Click(object sender, RoutedEventArgs e)
		{

        }
    }
}