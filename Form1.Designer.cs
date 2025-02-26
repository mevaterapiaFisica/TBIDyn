
namespace TBIDyn
{
    partial class Form1
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
            this.label1 = new System.Windows.Forms.Label();
            this.TB_DosisDia = new System.Windows.Forms.TextBox();
            this.BT_Calcular = new System.Windows.Forms.Button();
            this.TB_NumFx = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.TB_zRodilla = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.BT_Cancelar = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(121, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Dosis día por plan [cGy]";
            // 
            // TB_DosisDia
            // 
            this.TB_DosisDia.Location = new System.Drawing.Point(148, 31);
            this.TB_DosisDia.Name = "TB_DosisDia";
            this.TB_DosisDia.Size = new System.Drawing.Size(100, 20);
            this.TB_DosisDia.TabIndex = 1;
            // 
            // BT_Calcular
            // 
            this.BT_Calcular.Location = new System.Drawing.Point(148, 140);
            this.BT_Calcular.Name = "BT_Calcular";
            this.BT_Calcular.Size = new System.Drawing.Size(75, 23);
            this.BT_Calcular.TabIndex = 4;
            this.BT_Calcular.Text = "Calcular";
            this.BT_Calcular.UseVisualStyleBackColor = true;
            this.BT_Calcular.Click += new System.EventHandler(this.BT_Calcular_Click);
            // 
            // TB_NumFx
            // 
            this.TB_NumFx.Location = new System.Drawing.Point(148, 66);
            this.TB_NumFx.Name = "TB_NumFx";
            this.TB_NumFx.Size = new System.Drawing.Size(100, 20);
            this.TB_NumFx.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 69);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Nº fracciones";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // TB_zRodilla
            // 
            this.TB_zRodilla.Location = new System.Drawing.Point(148, 97);
            this.TB_zRodilla.Name = "TB_zRodilla";
            this.TB_zRodilla.Size = new System.Drawing.Size(100, 20);
            this.TB_zRodilla.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(24, 104);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "zRodilla [cm]";
            // 
            // BT_Cancelar
            // 
            this.BT_Cancelar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.BT_Cancelar.Location = new System.Drawing.Point(43, 140);
            this.BT_Cancelar.Name = "BT_Cancelar";
            this.BT_Cancelar.Size = new System.Drawing.Size(75, 23);
            this.BT_Cancelar.TabIndex = 7;
            this.BT_Cancelar.Text = "Cancelar";
            this.BT_Cancelar.UseVisualStyleBackColor = true;
            this.BT_Cancelar.Click += new System.EventHandler(this.BT_Cancelar_Click);
            // 
            // Form1
            // 
            this.AcceptButton = this.BT_Calcular;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.BT_Cancelar;
            this.ClientSize = new System.Drawing.Size(268, 176);
            this.Controls.Add(this.BT_Cancelar);
            this.Controls.Add(this.TB_zRodilla);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.TB_NumFx);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.BT_Calcular);
            this.Controls.Add(this.TB_DosisDia);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Autoplan TBI";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox TB_DosisDia;
        private System.Windows.Forms.Button BT_Calcular;
        private System.Windows.Forms.TextBox TB_NumFx;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox TB_zRodilla;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button BT_Cancelar;
    }
}

