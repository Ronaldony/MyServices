using System.Windows.Forms;
using TestClientApp.Components;

namespace TestClientApp
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            TestNetwork_DataIntegrity = new ButtonComponent();
            richTextBox1 = new RichTextBox();
            contextMenuStrip1 = new ContextMenuStrip(components);
            Log = new GroupBox();
            Clear = new Button();
            Log.SuspendLayout();
            SuspendLayout();
            // 
            // TestNetwork_DataIntegrity
            // 
            TestNetwork_DataIntegrity.AutoEllipsis = true;
            TestNetwork_DataIntegrity.BackColor = Color.SkyBlue;
            TestNetwork_DataIntegrity.FlatStyle = FlatStyle.Flat;
            TestNetwork_DataIntegrity.Location = new Point(21, 23);
            TestNetwork_DataIntegrity.Name = "TestNetwork_DataIntegrity";
            TestNetwork_DataIntegrity.Size = new Size(170, 47);
            TestNetwork_DataIntegrity.TabIndex = 0;
            TestNetwork_DataIntegrity.Text = "Test_Network_DataIntegrity";
            TestNetwork_DataIntegrity.UseVisualStyleBackColor = true;
            TestNetwork_DataIntegrity.Click += buttonTestNetwork_DataIntegrity;
            // 
            // richTextBox1
            // 
            richTextBox1.Location = new Point(6, 22);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new Size(524, 361);
            richTextBox1.TabIndex = 1;
            richTextBox1.Text = "";
            richTextBox1.TextChanged += richTextBox1_TextChanged;
            CustomLogger.SetLogBox(richTextBox1);
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(61, 4);
            // 
            // Log
            // 
            Log.Controls.Add(Clear);
            Log.Controls.Add(richTextBox1);
            Log.Location = new Point(232, 23);
            Log.Name = "Log";
            Log.Size = new Size(536, 415);
            Log.TabIndex = 3;
            Log.TabStop = false;
            Log.Text = "Log";
            Log.Enter += groupBox1_Enter;
            // 
            // Clear
            // 
            Clear.Location = new Point(455, 386);
            Clear.Name = "Clear";
            Clear.Size = new Size(75, 23);
            Clear.TabIndex = 2;
            Clear.Text = "Clear";
            Clear.UseVisualStyleBackColor = true;
            Clear.Click += Clear_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(Log);
            Controls.Add(TestNetwork_DataIntegrity);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            Log.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private ButtonComponent TestNetwork_DataIntegrity;
        private RichTextBox richTextBox1;
        private ContextMenuStrip contextMenuStrip1;
        private GroupBox Log;
        private Button Clear;
    }
}
