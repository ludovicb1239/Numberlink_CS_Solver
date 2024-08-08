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
            InputImageBox = new PictureBox();
            OutputImageBox = new PictureBox();
            outLabel = new Label();
            ((System.ComponentModel.ISupportInitialize)InputImageBox).BeginInit();
            ((System.ComponentModel.ISupportInitialize)OutputImageBox).BeginInit();
            SuspendLayout();
            // 
            // SolveButton
            // 
            SolveButton.Location = new Point(64, 68);
            SolveButton.Name = "SolveButton";
            SolveButton.Size = new Size(122, 37);
            SolveButton.TabIndex = 0;
            SolveButton.Text = "Play";
            SolveButton.UseVisualStyleBackColor = true;
            SolveButton.Click += SolveButton_Click;
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
            // outLabel
            // 
            outLabel.AutoSize = true;
            outLabel.Location = new Point(75, 141);
            outLabel.Name = "outLabel";
            outLabel.Size = new Size(38, 15);
            outLabel.TabIndex = 7;
            outLabel.Text = "label1";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(617, 626);
            Controls.Add(outLabel);
            Controls.Add(OutputImageBox);
            Controls.Add(InputImageBox);
            Controls.Add(SolveButton);
            Name = "Form1";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)InputImageBox).EndInit();
            ((System.ComponentModel.ISupportInitialize)OutputImageBox).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button SolveButton;
        private PictureBox InputImageBox;
        private PictureBox OutputImageBox;
        private Label outLabel;
    }
}
