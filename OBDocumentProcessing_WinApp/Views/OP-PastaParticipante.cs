using Hyland.Types;
using Hyland.Unity;
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
using System.Xml.Linq;

namespace OBDocumentProcessing_WinApp.Views
{
    public partial class OP_PastaParticipante : Form
    {
        public OP_PastaParticipante()
        {
            InitializeComponent();
        }

        private async void btnProcessar_Click(object sender, EventArgs e)
        {
            
        }

        static async Task<OP_Participante> BuscarDadosParticipante(string matricula, string connectionString)
        {
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

        static async Task ImportDoc(string matricula, string CPF, FileInfo Document)
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
                    FileType fileTypePDF = OnbaseUnity._OBApp.Core.FileTypes.Find(2);                    

                    StoreNewDocumentProperties storeDocumentProperties = OnbaseUnity._OBApp.Core.Storage.CreateStoreNewDocumentProperties(documentType, fileTypePDF);
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
    }
}
