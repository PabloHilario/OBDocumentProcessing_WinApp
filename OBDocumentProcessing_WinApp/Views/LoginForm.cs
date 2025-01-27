using OBDocumentProcessing_WinApp.Model;
using OBDocumentProcessing_WinApp.OnbaseConfig;
using OBDocumentProcessing_WinApp.Service;
using OBDocumentProcessing_WinApp.ViewModel;
using OBDocumentProcessing_WinApp.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OBDocumentProcessing_WinApp
{
    public partial class LoginForm : Form
    {
        Thread thread;
        public LoginForm()
        {
            InitializeComponent();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
        private void LoadHome()
        {
            Application.Run(new HomePage());
        }

        private async void btn_Login_Click(object sender, EventArgs e)
        {
            try
            {
                LoginViewModel dataLogin = new LoginViewModel
                {
                    Matricula = txt_User.Text.Trim().ToUpper(),
                    Senha = txt_Pwd.Text.Trim(),
                };

                OnbaseUser userLogged = await OnbaseUnity.AuthOnbase(dataLogin);
                if (userLogged != null && !String.IsNullOrEmpty(userLogged.SessionID)); 
                {

                    this.Close();
                    thread = new Thread(LoadHome);
                    thread.SetApartmentState(ApartmentState.STA);
                    thread.Start();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
