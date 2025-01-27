namespace OBDocumentProcessing_WinApp.Views
{
    partial class ProcessmentForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dgv_Processment = new System.Windows.Forms.DataGridView();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.dgv_Document = new System.Windows.Forms.DataGridView();
            this.Document = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Uploaded = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.btn_Processar = new System.Windows.Forms.Button();
            this.txt_PathOrigin = new System.Windows.Forms.TextBox();
            this.btn_Buscar = new System.Windows.Forms.Button();
            this.select_Processamento = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.fileTypeComboBox = new System.Windows.Forms.ComboBox();
            this.lbl_FileType = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Processment)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Document)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgv_Processment
            // 
            this.dgv_Processment.AllowUserToAddRows = false;
            this.dgv_Processment.AllowUserToDeleteRows = false;
            this.dgv_Processment.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.dgv_Processment.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_Processment.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv_Processment.Location = new System.Drawing.Point(0, 86);
            this.dgv_Processment.Margin = new System.Windows.Forms.Padding(2);
            this.dgv_Processment.Name = "dgv_Processment";
            this.dgv_Processment.ReadOnly = true;
            this.dgv_Processment.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgv_Processment.RowHeadersWidth = 51;
            this.dgv_Processment.RowTemplate.Height = 24;
            this.dgv_Processment.Size = new System.Drawing.Size(706, 414);
            this.dgv_Processment.TabIndex = 3;
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(50, 145);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 51;
            this.dataGridView1.Size = new System.Drawing.Size(240, 150);
            this.dataGridView1.TabIndex = 6;
            // 
            // dgv_Document
            // 
            this.dgv_Document.AllowUserToAddRows = false;
            this.dgv_Document.AllowUserToDeleteRows = false;
            this.dgv_Document.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.dgv_Document.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_Document.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Document,
            this.Uploaded});
            this.dgv_Document.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv_Document.GridColor = System.Drawing.SystemColors.ControlLightLight;
            this.dgv_Document.Location = new System.Drawing.Point(0, 86);
            this.dgv_Document.Name = "dgv_Document";
            this.dgv_Document.ReadOnly = true;
            this.dgv_Document.RowHeadersWidth = 51;
            this.dgv_Document.Size = new System.Drawing.Size(706, 414);
            this.dgv_Document.TabIndex = 8;
            // 
            // Document
            // 
            this.Document.HeaderText = "Document";
            this.Document.MinimumWidth = 100;
            this.Document.Name = "Document";
            this.Document.ReadOnly = true;
            this.Document.Width = 553;
            // 
            // Uploaded
            // 
            this.Uploaded.HeaderText = "Uploaded";
            this.Uploaded.MinimumWidth = 50;
            this.Uploaded.Name = "Uploaded";
            this.Uploaded.ReadOnly = true;
            // 
            // btn_Processar
            // 
            this.btn_Processar.Location = new System.Drawing.Point(12, 48);
            this.btn_Processar.Name = "btn_Processar";
            this.btn_Processar.Size = new System.Drawing.Size(99, 23);
            this.btn_Processar.TabIndex = 5;
            this.btn_Processar.Text = "Processar";
            this.btn_Processar.UseVisualStyleBackColor = true;
            this.btn_Processar.Click += new System.EventHandler(this.btn_Processar_Click);
            // 
            // txt_PathOrigin
            // 
            this.txt_PathOrigin.Location = new System.Drawing.Point(117, 14);
            this.txt_PathOrigin.Name = "txt_PathOrigin";
            this.txt_PathOrigin.Size = new System.Drawing.Size(290, 20);
            this.txt_PathOrigin.TabIndex = 4;
            // 
            // btn_Buscar
            // 
            this.btn_Buscar.Location = new System.Drawing.Point(12, 12);
            this.btn_Buscar.Name = "btn_Buscar";
            this.btn_Buscar.Size = new System.Drawing.Size(99, 23);
            this.btn_Buscar.TabIndex = 6;
            this.btn_Buscar.Text = "Buscar Origem";
            this.btn_Buscar.UseVisualStyleBackColor = true;
            this.btn_Buscar.Click += new System.EventHandler(this.btn_Buscar_Click);
            // 
            // select_Processamento
            // 
            this.select_Processamento.FormattingEnabled = true;
            this.select_Processamento.Items.AddRange(new object[] {
            "Contratação",
            "Aquisição"});
            this.select_Processamento.Location = new System.Drawing.Point(573, 14);
            this.select_Processamento.Name = "select_Processamento";
            this.select_Processamento.Size = new System.Drawing.Size(121, 21);
            this.select_Processamento.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(428, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(139, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Selecione o Processamento";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.fileTypeComboBox);
            this.panel1.Controls.Add(this.lbl_FileType);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.select_Processamento);
            this.panel1.Controls.Add(this.btn_Buscar);
            this.panel1.Controls.Add(this.txt_PathOrigin);
            this.panel1.Controls.Add(this.btn_Processar);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(706, 86);
            this.panel1.TabIndex = 7;
            // 
            // fileTypeComboBox
            // 
            this.fileTypeComboBox.FormattingEnabled = true;
            this.fileTypeComboBox.Items.AddRange(new object[] {
            "PDF",
            "MS Power Point",
            "MS Word Document",
            "MS Excel Spreadsheet",
            "Image File Format",
            "Zip Compression Archive"});
            this.fileTypeComboBox.Location = new System.Drawing.Point(573, 48);
            this.fileTypeComboBox.Name = "fileTypeComboBox";
            this.fileTypeComboBox.Size = new System.Drawing.Size(121, 21);
            this.fileTypeComboBox.TabIndex = 10;
            // 
            // lbl_FileType
            // 
            this.lbl_FileType.AutoSize = true;
            this.lbl_FileType.Location = new System.Drawing.Point(440, 51);
            this.lbl_FileType.Name = "lbl_FileType";
            this.lbl_FileType.Size = new System.Drawing.Size(110, 13);
            this.lbl_FileType.TabIndex = 9;
            this.lbl_FileType.Text = "Selecione a Extensão";
            // 
            // ProcessmentForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(706, 500);
            this.Controls.Add(this.dgv_Document);
            this.Controls.Add(this.dgv_Processment);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "ProcessmentForm";
            this.Text = "ProcessmentForm";
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Processment)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_Document)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.DataGridView dgv_Processment;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridView dgv_Document;
        private System.Windows.Forms.DataGridViewTextBoxColumn Document;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Uploaded;
        private System.Windows.Forms.Button btn_Processar;
        private System.Windows.Forms.TextBox txt_PathOrigin;
        private System.Windows.Forms.Button btn_Buscar;
        private System.Windows.Forms.ComboBox select_Processamento;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ComboBox fileTypeComboBox;
        private System.Windows.Forms.Label lbl_FileType;
    }
}