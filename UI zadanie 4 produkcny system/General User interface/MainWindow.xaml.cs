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
using System.Windows.Navigation;
using System.Windows.Shapes;
using UI_zadanie_4_produkcny_system.Model;

namespace UI_zadanie_4_produkcny_system
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

        private void init_btn_Click(object sender, RoutedEventArgs e)
        {
            
           
            try
            {
               Facts.LoadFacts(@"C:\Users\Rony\documents\visual studio 2015\Projects\UI zadanie 4 produkcny system\UI zadanie 4 produkcny system\FileFolter\Facts.txt", this);
                Rules.Load_za_Rules(@"C:\Users\Rony\documents\visual studio 2015\Projects\UI zadanie 4 produkcny system\UI zadanie 4 produkcny system\FileFolter\Rules.txt",this);
              

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
           
        }

        public MainWindow getCurrentMainWindow()
        {
            return this;
        }

        private void start_btn_Click(object sender, RoutedEventArgs e)
        {
            Logic.run(this);
        }
    }
}
