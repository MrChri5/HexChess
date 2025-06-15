namespace HexChess
{
    partial class PromotionDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PromotionDialog));
            QueenIcon = new PictureBox();
            BishopIcon = new PictureBox();
            KnightIcon = new PictureBox();
            RookIcon = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)QueenIcon).BeginInit();
            ((System.ComponentModel.ISupportInitialize)BishopIcon).BeginInit();
            ((System.ComponentModel.ISupportInitialize)KnightIcon).BeginInit();
            ((System.ComponentModel.ISupportInitialize)RookIcon).BeginInit();
            SuspendLayout();
            // 
            // QueenIcon
            // 
            QueenIcon.Location = new Point(12, 12);
            QueenIcon.Name = "QueenIcon";
            QueenIcon.Size = new Size(128, 128);
            QueenIcon.TabIndex = 1;
            QueenIcon.TabStop = false;
            QueenIcon.Click += Icon_Click;
            QueenIcon.Paint += Icon_Paint;
            // 
            // BishopIcon
            // 
            BishopIcon.Location = new Point(146, 12);
            BishopIcon.Name = "BishopIcon";
            BishopIcon.Size = new Size(128, 128);
            BishopIcon.SizeMode = PictureBoxSizeMode.AutoSize;
            BishopIcon.TabIndex = 2;
            BishopIcon.TabStop = false;
            BishopIcon.Click += Icon_Click;
            BishopIcon.Paint += Icon_Paint;
            // 
            // KnightIcon
            // 
            KnightIcon.Location = new Point(12, 146);
            KnightIcon.Name = "KnightIcon";
            KnightIcon.Size = new Size(128, 128);
            KnightIcon.SizeMode = PictureBoxSizeMode.AutoSize;
            KnightIcon.TabIndex = 3;
            KnightIcon.TabStop = false;
            KnightIcon.Click += Icon_Click;
            KnightIcon.Paint += Icon_Paint;
            // 
            // RookIcon
            // 
            RookIcon.Location = new Point(146, 146);
            RookIcon.Name = "RookIcon";
            RookIcon.Size = new Size(128, 128);
            RookIcon.SizeMode = PictureBoxSizeMode.AutoSize;
            RookIcon.TabIndex = 4;
            RookIcon.TabStop = false;
            RookIcon.Click += Icon_Click;
            RookIcon.Paint += Icon_Paint;
            // 
            // PromotionDialog
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(284, 283);
            Controls.Add(RookIcon);
            Controls.Add(KnightIcon);
            Controls.Add(BishopIcon);
            Controls.Add(QueenIcon);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "PromotionDialog";
            ShowInTaskbar = false;
            Text = "Promotion";
            Load += PromotionDialog_Load;
            ((System.ComponentModel.ISupportInitialize)QueenIcon).EndInit();
            ((System.ComponentModel.ISupportInitialize)BishopIcon).EndInit();
            ((System.ComponentModel.ISupportInitialize)KnightIcon).EndInit();
            ((System.ComponentModel.ISupportInitialize)RookIcon).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox QueenIcon;
        private PictureBox BishopIcon;
        private PictureBox KnightIcon;
        private PictureBox RookIcon;
    }
}