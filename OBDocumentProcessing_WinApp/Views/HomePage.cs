using OBDocumentProcessing_WinApp.OnbaseConfig;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OBDocumentProcessing_WinApp.Views
{
    public partial class HomePage : Form
    {
        public HomePage()
        {
            InitializeComponent();
        }

        private async Task LoadFormInPanel(Form form)
        {
            // Limpa o painel antes de adicionar um novo formulário
            pnl_ContetWrapper.Controls.Clear();

            // Define o formulário para que ele possa ser exibido dentro do painel
            form.TopLevel = false; // Permite que o formulário seja adicionado como um controle
            form.FormBorderStyle = FormBorderStyle.None; // Remove bordas do formulário
            form.Dock = DockStyle.Fill; // Preenche o painel

            // Adiciona o formulário ao painel
            pnl_ContetWrapper.Controls.Add(form);

            // Exibe o formulário
            form.Show();
        }

        private async void btn_ProcessPage_Click(object sender, EventArgs e)
        {
            try
            {
                ProcessmentForm processForm = new ProcessmentForm();
                await LoadFormInPanel(processForm);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            //onbaseUnity.disconnectOnbase();
            this.Close();
        }

        private async void btn_Viewer_Click(object sender, EventArgs e)
        {
            try
            {
                OP_PastaParticipante processForm = new OP_PastaParticipante();
                await LoadFormInPanel(processForm);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
