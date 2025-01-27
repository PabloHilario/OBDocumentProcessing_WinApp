using Hyland.Unity;
using Hyland.Unity.WorkView;
using OBDocumentProcessing_WinApp.Model;
using OBDocumentProcessing_WinApp.OnbaseConfig;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OBDocumentProcessing_WinApp.Views
{
    public partial class ProcessmentForm : Form
    {
        public ProcessmentForm()
        {
            InitializeComponent();
        }

        private void btn_Buscar_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderBrowser = new FolderBrowserDialog())
            {
                // Configurações do diálogo
                folderBrowser.Description = "Escolha um modelo de dados valido do tipo .xlsx";
                folderBrowser.ShowNewFolderButton = true;

                // Mostra o diálogo e verifica se o usuário selecionou uma pasta
                if (folderBrowser.ShowDialog() == DialogResult.OK)
                {
                    // Define o caminho da pasta no TextBox
                    txt_PathOrigin.Text = folderBrowser.SelectedPath;
                }
            }
        }

        private async void btn_Processar_Click(object sender, EventArgs e)
        {
            try
            {
                string pathOrigin = txt_PathOrigin.Text.Trim();
                string tipoProcessamento = select_Processamento.SelectedItem.ToString();
                string fileType = fileTypeComboBox.SelectedItem.ToString();
                if (!String.IsNullOrEmpty(tipoProcessamento) && !String.IsNullOrEmpty(fileType))
                {
                    if (!String.IsNullOrEmpty(pathOrigin))
                    {
                        DirectoryInfo folder = new DirectoryInfo(pathOrigin);
                        if (folder.Exists)
                        {
                            DirectoryInfo[] subFolders = folder.GetDirectories();
                            foreach (DirectoryInfo subFolder in subFolders)
                            {
                                await GetDataProcess(subFolder, tipoProcessamento, pathOrigin, fileType);
                            }
                        }
                        else
                        {
                            MessageBox.Show($"A Pasta de Origem não está disponvel ou seu usuário não tem acesso a este diretório.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    else
                    {
                        MessageBox.Show($"O Path de Origem para o processamento de documentos é obrigatório.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    MessageBox.Show($"Os parametros: Tipo de processamento e Extensão são obrigatórios.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                
            }
            catch (Exception ex) 
            {
                MessageBox.Show($"{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task GetDataProcess(DirectoryInfo folderName, string tipoProcessamento, string pathOrigin, string fileType)
        {
            /*
             * Contratação                                                              Aquisição
             * Nº1 -  Zeev                                                              Nº1 -  Zeev
             * Nº2 -  Contratada                                                        Nº2 -  Contratada
             * Nº3 -  Numero da Protheus                                                Nº3 -  Pedido de Compra
             * Nº4 -  Numero Contrato                                                   Nº4 -  Orgão Demandante
             * Nº5 -  Orgão Demandante
             */

            switch (tipoProcessamento)
            {
                case "Contratação":
                    string[] cn_splitFolderName = folderName.Name.Split('_');
                    if (cn_splitFolderName.Length == 5)
                    {
                        string numZeev = cn_splitFolderName[0].Trim();
                        string contratada = cn_splitFolderName[1].Trim();
                        string numProtheus = cn_splitFolderName[2].Replace("PASTA", "").Replace("Pasta", "").Replace("pasta", "").Trim();
                        string numContrato = cn_splitFolderName[3].Trim();
                        string orgaoDemandante = cn_splitFolderName[4].Replace("-", "/").Trim();

                        
                        bool connected = OnbaseUnity.IsConnected();
                        if (connected)
                        {
                            Keyword kw_numZeev = OnbaseUnity.CreateKeyword_Ext("(CN) - N° Zeev", numZeev);
                            Keyword kw_numProtheus = OnbaseUnity.CreateKeyword_Ext("(CN) - N° Protheus", numProtheus);
                            Keyword kw_contratada = OnbaseUnity.CreateKeyword_Ext("(CN) - Contratada", contratada);
                            Keyword kw_numContrato = OnbaseUnity.CreateKeyword_Ext("(CN) - N° do Contrato", numContrato); //verificar Keyword
                            Keyword kw_orgaoDemandante = OnbaseUnity.CreateKeyword_Ext("(CN) - Órgão Demandante", orgaoDemandante);

                            List<string> folders_tiposDocumentais = Directory.GetDirectories($@"{pathOrigin}\{folderName.Name}").ToList();
                            foreach (string folderFullPath in folders_tiposDocumentais)
                            {
                                DirectoryInfo folder = new DirectoryInfo(folderFullPath);
                                string nameTipoDocumental = $"CN - {folder.Name.Trim()}";

                                //Monsta o Document Type
                                DocumentType documentType = OnbaseUnity._OBApp.Core.DocumentTypes.Find(nameTipoDocumental);
                                //Pega os documentos dentro da folder desse tipo documental
                                List<string> arquivos = Directory.GetFiles(folder.FullName).ToList();

                                FileType fileTypePDF = OnbaseUnity._OBApp.Core.FileTypes.Find(fileType);

                                StoreNewDocumentProperties storeDocumentProperties = OnbaseUnity._OBApp.Core.Storage.CreateStoreNewDocumentProperties(documentType, fileTypePDF);

                                storeDocumentProperties.AddKeyword("(CN) - N° Zeev", int.Parse(numZeev));
                                storeDocumentProperties.AddKeyword("(CN) - N° Protheus", int.Parse(numProtheus));
                                storeDocumentProperties.AddKeyword("(CN) - Contratada", contratada);
                                storeDocumentProperties.AddKeyword("(CN) - N° do Contrato", numContrato);
                                storeDocumentProperties.AddKeyword("(CN) - Órgão Demandante", orgaoDemandante);
                                storeDocumentProperties.DocumentDate = DateTime.Now;
                                storeDocumentProperties.Comment = "";
                                storeDocumentProperties.Options = StoreDocumentOptions.SkipWorkflow;


                                
                                foreach (string arquivo in arquivos)
                                {
                                    List<string> fileList = new List<string>();
                                    fileList.Add(arquivo);

                                    Document newDocument = OnbaseUnity._OBApp.Core.Storage.StoreNewDocument(fileList, storeDocumentProperties);
                                    dgv_Document.Rows.Add(newDocument.Name, true);
                                }  
                            }
                        }

                    }
                    break;

                case "Aquisição":
                    string[] aq_splitFolderName = folderName.Name.Split('_');
                    if (aq_splitFolderName.Length == 4)
                    {
                        string numZeev = aq_splitFolderName[0].Trim();
                        string contratada = aq_splitFolderName[1].Trim();
                        string numPedidoCompra = aq_splitFolderName[2].Replace("PEDIDO DE COMPRA-", "").Trim();
                        string orgaoDemandante = aq_splitFolderName[3].Replace("-", "/").Trim();

                        
                        bool connected = OnbaseUnity.IsConnected();
                        if (connected)
                        {
                            Keyword kw_numZeev = OnbaseUnity.CreateKeyword_Ext("(AQ) - Nº Zeev", numZeev);
                            Keyword kw_numPedidoCompra = OnbaseUnity.CreateKeyword_Ext("(AQ) - Pedido de Compra", numZeev);                                                        
                            Keyword kw_contratada = OnbaseUnity.CreateKeyword_Ext("(AQ) - Contratada", numZeev);
                            Keyword kw_orgaoDemandante = OnbaseUnity.CreateKeyword_Ext("(AQ) - Orgão Demandante", numZeev);

                            List<string> folders_tiposDocumentais = Directory.GetDirectories($@"{pathOrigin}\{folderName.Name}").ToList();
                            foreach (string folderFullPath in folders_tiposDocumentais)
                            {
                                DirectoryInfo folder = new DirectoryInfo(folderFullPath);
                                string nameTipoDocumental = $"AQ - {folder.Name.Trim()}";

                                //Monsta o Document Type
                                DocumentType documentType = OnbaseUnity._OBApp.Core.DocumentTypes.Find(nameTipoDocumental);
                                //Pega os documentos dentro da folder desse tipo documental
                                List<string> arquivos = Directory.GetFiles(folder.FullName).ToList();

                                FileType fileTypePDF = OnbaseUnity._OBApp.Core.FileTypes.Find(fileType);

                                StoreNewDocumentProperties storeDocumentProperties = OnbaseUnity._OBApp.Core.Storage.CreateStoreNewDocumentProperties(documentType, fileTypePDF);

                                storeDocumentProperties.AddKeyword("(AQ) - Nº Zeev", int.Parse(numZeev));
                                storeDocumentProperties.AddKeyword("(AQ) - Contratada", contratada);
                                storeDocumentProperties.AddKeyword("(AQ) - Pedido de Compra", int.Parse(numPedidoCompra));
                                storeDocumentProperties.AddKeyword("(AQ) - Orgão Demandante", orgaoDemandante);
                                storeDocumentProperties.DocumentDate = DateTime.Now;
                                storeDocumentProperties.Comment = "";
                                storeDocumentProperties.Options = StoreDocumentOptions.SkipWorkflow;


                                foreach (string arquivo in arquivos)
                                {
                                    List<string> fileList = new List<string>();
                                    fileList.Add(arquivo);

                                    Document newDocument = OnbaseUnity._OBApp.Core.Storage.StoreNewDocument(fileList, storeDocumentProperties);
                                    dgv_Document.Rows.Add(newDocument.Name, true);
                                }
                            }
                        }
                    }
                    break;

                default:
                    MessageBox.Show($"O parametro informado é inválido", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;

            }
        }
    }
}
