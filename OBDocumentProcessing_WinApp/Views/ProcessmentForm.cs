using Hyland.Unity;
using Hyland.Unity.WorkView;
using OBDocumentProcessing_WinApp.Model;
using OBDocumentProcessing_WinApp.OnbaseConfig;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
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
                folderBrowser.Description = "Escolha a pasta com os itens a serem processados";
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
                string fileTypeName = fileTypeComboBox.SelectedItem.ToString();
                if (!String.IsNullOrEmpty(tipoProcessamento) && !String.IsNullOrEmpty(fileTypeName))
                {
                    if (!String.IsNullOrEmpty(pathOrigin))
                    {
                        DirectoryInfo folder = new DirectoryInfo(pathOrigin);
                        if (folder.Exists)
                        {

                            switch (tipoProcessamento)
                            {
                                case "Contratação":
                                    
                                    DirectoryInfo[] subFoldersContratacao = folder.GetDirectories();
                                    foreach (DirectoryInfo subFolder in subFoldersContratacao)
                                    {
                                        await ImportProcess_DIFT(subFolder, tipoProcessamento, pathOrigin, fileTypeName);
                                    }

                                break;

                                case "Aquisição":

                                    DirectoryInfo[] subFoldersAquisicao = folder.GetDirectories();
                                    foreach (DirectoryInfo subFolder in subFoldersAquisicao)
                                    {
                                        await ImportProcess_DIFT(subFolder, tipoProcessamento, pathOrigin, fileTypeName);
                                    }

                                break;

                                case "OP - Pasta Participante":
                                    await ProcessarPastaParticipante(pathOrigin, fileTypeName);
                                break;

                                default:
                                    break;
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


        #region Processamento DIFT/CN

        private async Task ImportProcess_DIFT(DirectoryInfo folderName, string tipoProcessamento, string pathOrigin, string fileTypeName)
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
                        string numProtheus = cn_splitFolderName[2].Replace("PASTA", "").Replace("Pasta", "").Replace("pasta", "").Replace("N PROTHEUS", "").Trim();
                        string numContrato = cn_splitFolderName[3].Replace("CONTRATO", "").Trim();
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

                                FileType fileType = OnbaseUnity._OBApp.Core.FileTypes.Find(fileTypeName);

                                StoreNewDocumentProperties storeDocumentProperties = OnbaseUnity._OBApp.Core.Storage.CreateStoreNewDocumentProperties(documentType, fileType);

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
                        string numPedidoCompra = aq_splitFolderName[2].Replace("PEDIDO DE COMPRA-", "").Replace("-", "").Trim();
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

                                FileType fileType = OnbaseUnity._OBApp.Core.FileTypes.Find(fileTypeName);

                                StoreNewDocumentProperties storeDocumentProperties = OnbaseUnity._OBApp.Core.Storage.CreateStoreNewDocumentProperties(documentType, fileType);

                                storeDocumentProperties.AddKeyword("(AQ) - Nº Zeev", int.Parse(numZeev));
                                storeDocumentProperties.AddKeyword("(AQ) - Contratada", contratada);
                                storeDocumentProperties.AddKeyword("(AQ) - Pedido de Compra", Int64.Parse(numPedidoCompra));
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

        #endregion

        #region Processamento Pasta do Participante

        private async Task ProcessarPastaParticipante(string pastaOrigem, string fileTypeName)
        {
            List<Dictionary<string, List<FileInfo>>> dataRaw = BuscaCPFs($@"{pastaOrigem}");

            foreach (Dictionary<string, List<FileInfo>> data in dataRaw)
            {
                try
                {
                    //Define a variavel CPF
                    string cpf = data.Keys.FirstOrDefault();

                    //Busca Matricula por CPF
                    string matricula = await ObterMatriculaPorCpf(cpf);

                    //Buscas os dados do Participante
                    OP_Participante participante = await BuscarDadosParticipante(matricula);

                    if (participante != null && data.Keys.FirstOrDefault().Count() > 0)
                    {
                        foreach (FileInfo document in data.Values.FirstOrDefault())
                        {
                            await ImportDoc(participante.Matricula, participante.CPF, document, fileTypeName);
                        }
                    }
                    else
                    {
                        RegistrarLog(
                            CPF: cpf,
                            Matricula: matricula,
                            additionalInfo: "Não existem documentos deste participante para importação ou dados são nulos."
                        );
                    }
                }
                catch (Exception ex)
                {
                    RegistrarLog(
                        CPF: string.Empty,
                        Matricula: string.Empty,
                        documentName: string.Empty,
                        exception: ex
                    );
                }
            }
        }

        static async Task<OP_Participante> BuscarDadosParticipante(string matricula)
        {
            #region Connection String DB Petros
            string connectionString = @"string ConnectionString = @""Data Source=CLSQL19ONB\ONB;"";
                     ConnectionString += ""User ID=hsi;"";
                     ConnectionString += ""Password=wstinol;"";
                     ConnectionString += ""Initial Catalog=ONBASE_PROD;"";";
            #endregion                        

            #region Query SQL
            string query = "select * from openquery(ps07, 'select NUM_MATRICULA_PETROS, NOME, PATROCINADORA,PRODUTO, NUM_MATR_PETROS_GERADOR,CPF from SIBARQ.ONBASEF WHERE NUM_MATRICULA_PETROS = @MATRICULA')";
            #endregion

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    // Parâmetro para evitar SQL Injection
                    command.Parameters.AddWithValue("@MATRICULA", matricula);

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            OP_Participante participante = new OP_Participante();
                            Console.WriteLine("Dados encontrados:");
                            while (reader.Read())
                            {
                                participante = new OP_Participante
                                {
                                    Matricula = matricula,
                                    Nome = reader["NOME"].ToString(),
                                    Patrocinadora = reader["PATROCINADORA"].ToString(),
                                    Plano = reader["PRODUTO"].ToString(),
                                    NumeroGerador = reader["NUM_MATR_PETROS_GERADOR"].ToString(),
                                    CPF = reader["CPF"].ToString()
                                };
                            }

                            if (!String.IsNullOrEmpty(participante.Matricula) && !String.IsNullOrEmpty(participante.CPF))
                            {
                                Console.WriteLine($"Matrícula: {participante.Matricula}, CPF: {participante.CPF}");
                                return participante;
                            }
                            else
                            {
                                Console.WriteLine("Participante não localizado ou o dados são nulos.");
                                return null;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Nenhum registro encontrado para o CPF informado.");
                            return null;
                        }
                    }
                }
            }
        }

        static List<Dictionary<string, List<FileInfo>>> BuscaCPFs(string originPath)
        {
            List<Dictionary<string, List<FileInfo>>> Data = new List<Dictionary<string, List<FileInfo>>>();

            //Os CPFs estão no nome das Pastas

            DirectoryInfo directoryInfo = new DirectoryInfo(originPath);
            List<DirectoryInfo> folders = directoryInfo.GetDirectories().ToList();

            foreach (DirectoryInfo folder in folders)
            {
                if (!String.IsNullOrEmpty(folder.Name))
                {
                    Dictionary<string, List<FileInfo>> cpfXdoc = new Dictionary<string, List<FileInfo>>();
                    cpfXdoc.Add(folder.Name, folder.GetFiles().ToList());

                    Data.Add(cpfXdoc);
                }
            }

            return Data;
        }

        static async Task ImportDoc(string matricula, string CPF, FileInfo Document, string fileTypeName)
        {
            try
            {
                if (OnbaseUnity.IsConnected())
                {
                    //Monta o Document Type
                    int indexChar = Document.Name.IndexOf('_');
                    string documentTypeName = $"OP - {Document.Name.Substring(0, indexChar)}";
                    DocumentType documentType = OnbaseUnity._OBApp.Core.DocumentTypes.Find(documentTypeName);

                    //Image File Format
                    FileType fileType = OnbaseUnity._OBApp.Core.FileTypes.Find(fileTypeName);

                    StoreNewDocumentProperties storeDocumentProperties = OnbaseUnity._OBApp.Core.Storage.CreateStoreNewDocumentProperties(documentType, fileType);
                    storeDocumentProperties.DocumentDate = DateTime.Now;
                    storeDocumentProperties.Comment = "";
                    storeDocumentProperties.Options = StoreDocumentOptions.SkipWorkflow;

                    List<string> fileList = new List<string>();
                    fileList.Add(Document.FullName);

                    Document newDocument = OnbaseUnity._OBApp.Core.Storage.StoreNewDocument(fileList, storeDocumentProperties);

                    #region Add KeywordRecord (Reindexação)

                    KeywordModifier keyModifier = newDocument.CreateKeywordModifier();
                    KeywordRecordType keywordRecordType = OnbaseUnity._OBApp.Core.KeywordRecordTypes.Find(117); //(OP)-Mat. - Part. - Patroc - Plano - Ger                  

                    EditableKeywordRecord editableKwRecord_Participante = keywordRecordType.CreateEditableKeywordRecord();
                    Hyland.Unity.Keyword kw_matricula = OnbaseUnity.CreateKeyword_Ext(".Matrícula", matricula);
                    editableKwRecord_Participante.AddKeyword(kw_matricula);

                    Hyland.Unity.Keyword kw_CPF = OnbaseUnity.CreateKeyword_Ext(".CPF", CPF);
                    keyModifier.AddKeyword(kw_CPF);

                    Hyland.Unity.Keyword kw_TipoEntrada = OnbaseUnity.CreateKeyword_Ext(".Tipo de Entrada", "DIGITALIZAÇÃO INTERNA");
                    keyModifier.AddKeyword(kw_TipoEntrada);

                    //Reutilização da variavel keywordRecordType para atribuir as Keyword Processo e Subprocesso
                    keywordRecordType = OnbaseUnity._OBApp.Core.KeywordRecordTypes.Find(114);
                    EditableKeywordRecord editableKwRecord_ProcessoSubProcesso = keywordRecordType.CreateEditableKeywordRecord();
                    Hyland.Unity.Keyword kw_Processo = OnbaseUnity.CreateKeyword_Ext("(OP)-Processo", "00 DOCUMENTOS COMUNS");
                    editableKwRecord_ProcessoSubProcesso.AddKeyword(kw_Processo);
                    Hyland.Unity.Keyword kw_SubProcesso = OnbaseUnity.CreateKeyword_Ext("(OP)-SubProcesso", "SEM SUBPROCESSO");
                    editableKwRecord_ProcessoSubProcesso.AddKeyword(kw_SubProcesso);

                    KeywordType keywordTypeDataDocumento = OnbaseUnity._OBApp.Core.KeywordTypes.Find(155);
                    Hyland.Unity.Keyword kw_DataDocumento = OnbaseUnity.CreateKeyword(keywordTypeDataDocumento, DateTime.Now.Date.ToString());
                    keyModifier.AddKeyword(kw_DataDocumento);

                    keyModifier.AddKeywordRecord(editableKwRecord_Participante);
                    keyModifier.AddKeywordRecord(editableKwRecord_ProcessoSubProcesso);
                    keyModifier.ApplyChanges();

                    #endregion

                    RegistrarLog(
                        documentId: newDocument.ID.ToString(),
                        documentName: newDocument.Name,
                        CPF: CPF,
                        Matricula: matricula
                    );
                }
            }
            catch (Hyland.Unity.UnityAPIException ex)
            {
                //MessageBox.Show($"{ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                RegistrarLog(
                    CPF: CPF,
                    Matricula: matricula,
                    documentName: Document?.Name,
                    exception: ex
                );
            }
        }

        static void RegistrarLog(string documentId = null, string documentName = null, string CPF = null, string Matricula = null, Exception exception = null, string additionalInfo = null)
        {
            // Configuração do diretório de logs
            string logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ArchiveLog");

            // Cria o diretório se não existir
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            // Define o caminho do arquivo de log com data no nome
            string logFileName = $"Processamento_{DateTime.Now:yyyyMMdd}.log";
            string filePath = Path.Combine(logDirectory, logFileName);

            // Construção do conteúdo do log
            StringBuilder logContent = new StringBuilder();
            logContent.AppendLine($"Data/Hora: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");

            if (exception == null)
            {
                // Log de sucesso
                logContent.AppendLine("[SUCESSO] Documento processado com êxito");
                logContent.AppendLine($"ID Documento: {documentId ?? "N/A"}");
                logContent.AppendLine($"Nome Documento: {documentName ?? "N/A"}");
                logContent.AppendLine($"CPF: {CPF ?? "N/A"}");
                logContent.AppendLine($"Matrícula: {Matricula ?? "N/A"}");
            }
            else
            {
                // Log de erro
                logContent.AppendLine("[ERRO] Falha no processamento");
                logContent.AppendLine($"CPF: {CPF ?? "N/A"}");
                logContent.AppendLine($"Matrícula: {Matricula ?? "N/A"}");
                logContent.AppendLine($"Documento: {documentName ?? "N/A"}");
                logContent.AppendLine($"Mensagem de erro: {exception.Message}");

                if (exception is Hyland.Unity.UnityAPIException unityEx)
                {
                    logContent.AppendLine($"Tipo de erro: Unity API (OnBase)");
                    logContent.AppendLine($"Mensagem de erro: {unityEx.Message}");
                }

                logContent.AppendLine($"Stack Trace: {exception.StackTrace}");

                if (exception.InnerException != null)
                {
                    logContent.AppendLine($"Inner Exception: {exception.InnerException.Message}");
                }
            }

            if (!string.IsNullOrEmpty(additionalInfo))
            {
                logContent.AppendLine($"Informações adicionais: {additionalInfo}");
            }

            logContent.AppendLine(new string('=', 80)); // Separador visual

            try
            {
                // Escreve no arquivo de log
                File.AppendAllText(filePath, logContent.ToString());
                Console.WriteLine($"Log registrado em: {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Falha ao registrar log: {ex.Message}");
            }
        }

        /// <summary>
        /// Consulta a matrícula de um participante no banco de dados usando seu CPF
        /// </summary>
        /// <param name="cpf">CPF do participante a ser consultado</param>
        /// <returns>Retorna a matrícula encontrada ou string vazia se não encontrado</returns>
        private async Task<string> ObterMatriculaPorCpf(string cpf)
        {
            // Obtém o caminho absoluto para o banco de dados na pasta DB do projeto
            string caminhoBancoDados = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DB", "ConsultaMatricula.db");

            // Verifica se o arquivo do banco de dados existe
            if (!File.Exists(caminhoBancoDados))
            {
                Console.WriteLine("Erro: Arquivo do banco de dados não encontrado.");
                return string.Empty;
            }

            // Cria a connection string
            string connectionString = $"Data Source={caminhoBancoDados};Version=3;";

            const string query = "SELECT Matricula FROM MatriculasParticipante WHERE CPF = @CPF";

            try
            {
                using (var conexaoComBanco = new SQLiteConnection(connectionString))
                {
                    conexaoComBanco.Open();

                    using (var comandoSql = new SQLiteCommand(query, conexaoComBanco))
                    {
                        comandoSql.Parameters.AddWithValue("@CPF", cpf);

                        object resultadoConsulta = comandoSql.ExecuteScalar();

                        if (resultadoConsulta != null)
                        {
                            string matriculaEncontrada = resultadoConsulta.ToString();
                            Console.WriteLine($"Matrícula {matriculaEncontrada} encontrada para CPF {cpf}");
                            return matriculaEncontrada;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao consultar matrícula para CPF {cpf}: {ex.Message}");
                // Em produção, considere usar um sistema de log adequado
            }

            Console.WriteLine($"Nenhuma matrícula encontrada para o CPF: {cpf}");
            return string.Empty;
        }

        #endregion
    }
}
