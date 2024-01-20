using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Acycl_GUI
{
  /// <summary>
  /// Логика взаимодействия для SettingsWindow.xaml
  /// </summary>
  public partial class SettingsWindow : Window
  {
    public int Radius { get; set; }
    public SettingsWindow(int oldRad)
    {
      InitializeComponent();
      RadiusBox.Text = oldRad.ToString();
      Radius = oldRad;
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      if (!Int32.TryParse(RadiusBox.Text, out var result))
      {
        MessageBox.Show("Введено неверное значение");
        return;
      }
      Radius = result;
      Close();
    }
  }
}
