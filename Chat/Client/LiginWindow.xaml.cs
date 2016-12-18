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

namespace Client
{

    public partial class LoginWindow : Window
    {
        public string UserName { get; set; }

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            txtName.Text = "";
            this.Close();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if (txtName.Text != "")
            {
                UserName = txtName.Text;
                this.Close();
            }
            else
            {
                MessageBox.Show("Введите имя!");
            }
        }
    }
}
