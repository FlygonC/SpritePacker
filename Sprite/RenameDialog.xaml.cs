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

namespace Sprite
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class RenameDialog : Window
    {
        public string input = "Name";

        public RenameDialog(string defualtText)
        {
            InitializeComponent();

            input = defualtText;
            nameBox.Text = input;
        }


        private void ok_Click(object sender, RoutedEventArgs e)
        {
            input = nameBox.Text;
            this.DialogResult = true;
        }

        private void no_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
