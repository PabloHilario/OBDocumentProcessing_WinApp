using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OBDocumentProcessing_WinApp.Model
{
    internal class DocumentKeywords
    {
        public List<string> Dados { get; set; }

        public DocumentKeywords()
        {
            Dados = new List<string>();
        }

        public static DocumentKeywords BuscarNaPlanilha(string caminhoArquivo, string numZeev, string numProtheus)
        {
            // Verifica se o arquivo existe
            if (!File.Exists(caminhoArquivo))
            {
                throw new FileNotFoundException("O arquivo não foi encontrado.", caminhoArquivo);
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            // Usando EPPlus para ler a planilha
            FileInfo fileInfo = new FileInfo(caminhoArquivo);
            using (ExcelPackage package = new ExcelPackage(fileInfo))
            {
                // Assume que os dados estão na primeira planilha
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                // Obtém o número total de linhas
                int totalRows = worksheet.Dimension.Rows;

                // Percorre as linhas da planilha
                for (int row = 1; row <= totalRows; row++)
                {
                    string valorZeev = worksheet.Cells[row, 1].Text; // Supondo que numZeev está na coluna 1
                    string valorProtheus = worksheet.Cells[row, 2].Text; // Supondo que numProtheus está na coluna 2

                    if (valorZeev.Equals(numZeev, StringComparison.OrdinalIgnoreCase) &&
                        valorProtheus.Equals(numProtheus, StringComparison.OrdinalIgnoreCase))
                    {
                        // Se encontrar a linha, cria um objeto Documento e preenche com os dados da linha
                        DocumentKeywords documento = new DocumentKeywords();

                        // Adiciona todos os dados da linha ao objeto Documento
                        for (int col = 1; col <= worksheet.Dimension.Columns; col++)
                        {
                            documento.Dados.Add(worksheet.Cells[row, col].Text);
                        }

                        return documento; // Retorna o documento encontrado
                    }
                }
            }

            return null; // Retorna null se não encontrar
        }
    }
}
