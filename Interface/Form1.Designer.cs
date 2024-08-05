namespace Interface
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
            SolveButton = new Button();
            XCountInput = new NumericUpDown();
            YCountInput = new NumericUpDown();
            label1 = new Label();
            label2 = new Label();
            InputImageBox = new PictureBox();
            OutputImageBox = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)XCountInput).BeginInit();
            ((System.ComponentModel.ISupportInitialize)YCountInput).BeginInit();
            ((System.ComponentModel.ISupportInitialize)InputImageBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)OutputImageBox).BeginInit();
            SuspendLayout();
            // 
            // SolveButton
            // 
            SolveButton.Location = new Point(57, 231);
            SolveButton.Name = "SolveButton";
            SolveButton.Size = new Size(122, 37);
            SolveButton.TabIndex = 0;
            SolveButton.Text = "Play";
            SolveButton.UseVisualStyleBackColor = true;
            SolveButton.Click += SolveButton_Click;
            // 
            // XCountInput
            // 
            XCountInput.Location = new Point(90, 110);
            XCountInput.Maximum = new decimal(new int[] { 36, 0, 0, 0 });
            XCountInput.Minimum = new decimal(new int[] { 5, 0, 0, 0 });
            XCountInput.Name = "XCountInput";
            XCountInput.Size = new Size(120, 23);
            XCountInput.TabIndex = 1;
            XCountInput.Value = new decimal(new int[] { 9, 0, 0, 0 });
            XCountInput.ValueChanged += XCountInput_ValueChanged;
            // 
            // YCountInput
            // 
            YCountInput.Location = new Point(90, 154);
            YCountInput.Maximum = new decimal(new int[] { 36, 0, 0, 0 });
            YCountInput.Minimum = new decimal(new int[] { 5, 0, 0, 0 });
            YCountInput.Name = "YCountInput";
            YCountInput.Size = new Size(120, 23);
            YCountInput.TabIndex = 2;
            YCountInput.Value = new decimal(new int[] { 9, 0, 0, 0 });
            YCountInput.ValueChanged += YCountInput_ValueChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(35, 114);
            label1.Name = "label1";
            label1.Size = new Size(50, 15);
            label1.TabIndex = 3;
            label1.Text = "X Count";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(35, 158);
            label2.Name = "label2";
            label2.Size = new Size(50, 15);
            label2.TabIndex = 4;
            label2.Text = "Y Count";
            // 
            // InputImageBox
            // 
            InputImageBox.Location = new Point(305, 12);
            InputImageBox.Name = "InputImageBox";
            InputImageBox.Size = new Size(300, 300);
            InputImageBox.SizeMode = PictureBoxSizeMode.StretchImage;
            InputImageBox.TabIndex = 5;
            InputImageBox.TabStop = false;
            // 
            // OutputImageBox
            // 
            OutputImageBox.Location = new Point(305, 318);
            OutputImageBox.Name = "OutputImageBox";
            OutputImageBox.Size = new Size(300, 300);
            OutputImageBox.SizeMode = PictureBoxSizeMode.StretchImage;
            OutputImageBox.TabIndex = 6;
            OutputImageBox.TabStop = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(617, 626);
            Controls.Add(OutputImageBox);
            Controls.Add(InputImageBox);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(YCountInput);
            Controls.Add(XCountInput);
            Controls.Add(SolveButton);
            Name = "Form1";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)XCountInput).EndInit();
            ((System.ComponentModel.ISupportInitialize)YCountInput).EndInit();
            ((System.ComponentModel.ISupportInitialize)InputImageBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)OutputImageBox).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button SolveButton;
        private NumericUpDown XCountInput;
        private NumericUpDown YCountInput;
        private Label label1;
        private Label label2;
        private PictureBox InputImageBox;
        private PictureBox OutputImageBox;
    }
}
