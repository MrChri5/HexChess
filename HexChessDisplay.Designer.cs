namespace HexChess
{
    partial class HexChessDisplay
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

        //Panel class with Double Buffer set.
        public class BufferedPanel : System.Windows.Forms.Panel
        {
            public BufferedPanel()
            {
                this.SetStyle(
                    System.Windows.Forms.ControlStyles.UserPaint |
                    System.Windows.Forms.ControlStyles.AllPaintingInWmPaint |
                    System.Windows.Forms.ControlStyles.OptimizedDoubleBuffer,
                    true);
            }
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HexChessDisplay));
            BoardPanel = new BufferedPanel();
            GameStatus = new StatusStrip();
            CurrentPlayerLabel = new ToolStripStatusLabel();
            LastMoveLabel = new ToolStripStatusLabel();
            GameMenu = new MenuStrip();
            newGameToolStripMenuItem = new ToolStripMenuItem();
            RulesToolStripMenuItem = new ToolStripMenuItem();
            PawnMovesToolStripMenuItem = new ToolStripMenuItem();
            RookMovesToolStripMenuItem = new ToolStripMenuItem();
            KnightMovesToolStripMenuItem = new ToolStripMenuItem();
            BishopMovesToolStripMenuItem = new ToolStripMenuItem();
            QueenAndKingMovesToolStripMenuItem = new ToolStripMenuItem();
            EnPassantToolStripMenuItem = new ToolStripMenuItem();
            GlinskisHexagonalChessToolStripMenuItem = new ToolStripMenuItem();
            SaveToolStripMenuItem = new ToolStripMenuItem();
            LoadToolStripMenuItem = new ToolStripMenuItem();
            UndoMoveToolStripMenuItem = new ToolStripMenuItem();
            GameStatus.SuspendLayout();
            GameMenu.SuspendLayout();
            SuspendLayout();
            // 
            // BoardPanel
            // 
            BoardPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            BoardPanel.BackColor = SystemColors.Window;
            BoardPanel.Dock = DockStyle.Fill;
            BoardPanel.Location = new Point(0, 0);
            BoardPanel.Name = "BoardPanel";
            BoardPanel.Size = new Size(1080, 1080);
            BoardPanel.TabIndex = 0;
            BoardPanel.SizeChanged += HexChessDisplay_SizeChanged;
            BoardPanel.Paint += BoardPanel_Paint;
            BoardPanel.MouseClick += BoardPanel_MouseClick;
            // 
            // GameStatus
            // 
            GameStatus.ImageScalingSize = new Size(32, 32);
            GameStatus.Items.AddRange(new ToolStripItem[] { CurrentPlayerLabel, LastMoveLabel });
            GameStatus.Location = new Point(0, 1058);
            GameStatus.Name = "GameStatus";
            GameStatus.RenderMode = ToolStripRenderMode.Professional;
            GameStatus.Size = new Size(1080, 22);
            GameStatus.TabIndex = 1;
            // 
            // CurrentPlayerLabel
            // 
            CurrentPlayerLabel.Name = "CurrentPlayerLabel";
            CurrentPlayerLabel.Size = new Size(0, 12);
            // 
            // LastMoveLabel
            // 
            LastMoveLabel.Name = "LastMoveLabel";
            LastMoveLabel.Size = new Size(0, 12);
            // 
            // GameMenu
            // 
            GameMenu.ImageScalingSize = new Size(32, 32);
            GameMenu.Items.AddRange(new ToolStripItem[] { newGameToolStripMenuItem, SaveToolStripMenuItem, LoadToolStripMenuItem, UndoMoveToolStripMenuItem });
            GameMenu.Location = new Point(0, 0);
            GameMenu.Name = "GameMenu";
            GameMenu.RenderMode = ToolStripRenderMode.Professional;
            GameMenu.Size = new Size(1080, 42);
            GameMenu.TabIndex = 2;
            GameMenu.Text = "menuStrip";
            // 
            // newGameToolStripMenuItem
            // 
            newGameToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { RulesToolStripMenuItem, GlinskisHexagonalChessToolStripMenuItem });
            newGameToolStripMenuItem.Name = "newGameToolStripMenuItem";
            newGameToolStripMenuItem.Size = new Size(151, 38);
            newGameToolStripMenuItem.Text = "New Game";
            // 
            // RulesToolStripMenuItem
            // 
            RulesToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { PawnMovesToolStripMenuItem, RookMovesToolStripMenuItem, KnightMovesToolStripMenuItem, BishopMovesToolStripMenuItem, QueenAndKingMovesToolStripMenuItem, EnPassantToolStripMenuItem });
            RulesToolStripMenuItem.Name = "RulesToolStripMenuItem";
            RulesToolStripMenuItem.Size = new Size(422, 44);
            RulesToolStripMenuItem.Text = "Rules";
            // 
            // PawnMovesToolStripMenuItem
            // 
            PawnMovesToolStripMenuItem.Name = "PawnMovesToolStripMenuItem";
            PawnMovesToolStripMenuItem.Size = new Size(399, 44);
            PawnMovesToolStripMenuItem.Text = "Pawn Moves";
            PawnMovesToolStripMenuItem.Click += PawnMovesToolStripMenuItem_Click;
            // 
            // RookMovesToolStripMenuItem
            // 
            RookMovesToolStripMenuItem.Name = "RookMovesToolStripMenuItem";
            RookMovesToolStripMenuItem.Size = new Size(399, 44);
            RookMovesToolStripMenuItem.Text = "Rook Moves";
            RookMovesToolStripMenuItem.Click += RookMovesToolStripMenuItem_Click;
            // 
            // KnightMovesToolStripMenuItem
            // 
            KnightMovesToolStripMenuItem.Name = "KnightMovesToolStripMenuItem";
            KnightMovesToolStripMenuItem.Size = new Size(399, 44);
            KnightMovesToolStripMenuItem.Text = "Knight Moves";
            KnightMovesToolStripMenuItem.Click += KnightMovesToolStripMenuItem_Click;
            // 
            // BishopMovesToolStripMenuItem
            // 
            BishopMovesToolStripMenuItem.Name = "BishopMovesToolStripMenuItem";
            BishopMovesToolStripMenuItem.Size = new Size(399, 44);
            BishopMovesToolStripMenuItem.Text = "Bishop Moves";
            BishopMovesToolStripMenuItem.Click += BishopMovesToolStripMenuItem_Click;
            // 
            // QueenAndKingMovesToolStripMenuItem
            // 
            QueenAndKingMovesToolStripMenuItem.Name = "QueenAndKingMovesToolStripMenuItem";
            QueenAndKingMovesToolStripMenuItem.Size = new Size(399, 44);
            QueenAndKingMovesToolStripMenuItem.Text = "Queen and King Moves";
            QueenAndKingMovesToolStripMenuItem.Click += QueenAndKingMovesToolStripMenuItem_Click;
            // 
            // EnPassantToolStripMenuItem
            // 
            EnPassantToolStripMenuItem.Name = "EnPassantToolStripMenuItem";
            EnPassantToolStripMenuItem.Size = new Size(399, 44);
            EnPassantToolStripMenuItem.Text = "En Passant";
            EnPassantToolStripMenuItem.Click += EnPassantToolStripMenuItem_Click;
            // 
            // GlinskisHexagonalChessToolStripMenuItem
            // 
            GlinskisHexagonalChessToolStripMenuItem.Name = "GlinskisHexagonalChessToolStripMenuItem";
            GlinskisHexagonalChessToolStripMenuItem.Size = new Size(422, 44);
            GlinskisHexagonalChessToolStripMenuItem.Text = "Glinski's Hexagonal Chess";
            GlinskisHexagonalChessToolStripMenuItem.Click += GlinskisHexagonalChessToolStripMenuItem_Click;
            // 
            // SaveToolStripMenuItem
            // 
            SaveToolStripMenuItem.Name = "SaveToolStripMenuItem";
            SaveToolStripMenuItem.Size = new Size(153, 38);
            SaveToolStripMenuItem.Text = "Save Game";
            SaveToolStripMenuItem.Click += SaveToolStripMenuItem_Click;
            // 
            // LoadToolStripMenuItem
            // 
            LoadToolStripMenuItem.Name = "LoadToolStripMenuItem";
            LoadToolStripMenuItem.Size = new Size(154, 38);
            LoadToolStripMenuItem.Text = "Load Game";
            LoadToolStripMenuItem.Click += LoadToolStripMenuItem_Click;
            // 
            // UndoMoveToolStripMenuItem
            // 
            UndoMoveToolStripMenuItem.Name = "UndoMoveToolStripMenuItem";
            UndoMoveToolStripMenuItem.Size = new Size(160, 38);
            UndoMoveToolStripMenuItem.Text = "Undo Move";
            UndoMoveToolStripMenuItem.Click += UndoMoveToolStripMenuItem_Click;
            // 
            // HexChessDisplay
            // 
            AutoScaleDimensions = new SizeF(13F, 32F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1080, 1080);
            Controls.Add(GameStatus);
            Controls.Add(GameMenu);
            Controls.Add(BoardPanel);
            DoubleBuffered = true;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = GameMenu;
            Name = "HexChessDisplay";
            Text = "Hexagonal Chess";
            FormClosed += HexChessDisplay_FormClosed;
            GameStatus.ResumeLayout(false);
            GameStatus.PerformLayout();
            GameMenu.ResumeLayout(false);
            GameMenu.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private StatusStrip GameStatus;
        private ToolStripStatusLabel CurrentPlayerLabel;
        private ToolStripStatusLabel LastMoveLabel;
        private MenuStrip GameMenu;
        private ToolStripMenuItem newGameToolStripMenuItem;
        private ToolStripMenuItem RulesToolStripMenuItem;
        private ToolStripMenuItem PawnMovesToolStripMenuItem;
        private ToolStripMenuItem GlinskisHexagonalChessToolStripMenuItem;
        private ToolStripMenuItem RookMovesToolStripMenuItem;
        private ToolStripMenuItem KnightMovesToolStripMenuItem;
        private ToolStripMenuItem BishopMovesToolStripMenuItem;
        private ToolStripMenuItem QueenAndKingMovesToolStripMenuItem;
        private ToolStripMenuItem EnPassantToolStripMenuItem;
        private ToolStripMenuItem SaveToolStripMenuItem;
        private ToolStripMenuItem LoadToolStripMenuItem;
        private ToolStripMenuItem UndoMoveToolStripMenuItem;
        private BufferedPanel BoardPanel;
    }
}
