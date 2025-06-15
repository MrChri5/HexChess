using System.ComponentModel;

namespace HexChess
{
    public partial class PromotionDialog : Form
    {

        readonly Image chessPieceIcons;
        Rectangle iconRec;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ChessPiece.Type PromotionType { get; private set; }

        /// <summary>
        /// Display a Pawn Promotion Dialog to select promoting a Pawn to a Queen, Knight, Bishop, or Rook.
        /// </summary>
        public PromotionDialog()
        {
            InitializeComponent();

            PromotionType = ChessPiece.Type.None;

            chessPieceIcons = new Bitmap(HexChess.Properties.Resources.ChessPieces);
            iconRec = new Rectangle(0, 0, QueenIcon.Width, QueenIcon.Height);
        }
        private void PromotionDialog_Load(object sender, EventArgs e)
        {
            QueenIcon.Refresh();
            BishopIcon.Refresh();
            KnightIcon.Refresh();
            RookIcon.Refresh();
        }
        private void Icon_Paint(object sender, PaintEventArgs e)
        {
            if (sender is Control control)
            {
                Graphics iconPanel = e.Graphics;
                ChessPiece.Type type = ChessPiece.Type.None;
                if (control == QueenIcon)
                    type = ChessPiece.Type.Queen;
                else if (control == BishopIcon)
                    type = ChessPiece.Type.Bishop;
                else if (control == KnightIcon)
                    type = ChessPiece.Type.Knight;
                else if (control == RookIcon)
                    type = ChessPiece.Type.Rook;

                iconPanel.DrawImage(chessPieceIcons,
                                    iconRec,
                                    ((int)type - 1) * chessPieceIcons.Height,
                                    0,
                                    chessPieceIcons.Height,
                                    chessPieceIcons.Height,
                                    GraphicsUnit.Pixel);
            }
        }

        //Process the mouse input to determine which chess piece was selected.
        private void Icon_Click(object sender, EventArgs e)
        {
            if (sender is Control control)
            {                
                if (control == QueenIcon)
                    PromotionType = ChessPiece.Type.Queen;
                else if (control == BishopIcon)
                    PromotionType = ChessPiece.Type.Bishop;
                else if (control == KnightIcon)
                    PromotionType = ChessPiece.Type.Knight;
                else if (control == RookIcon)
                    PromotionType = ChessPiece.Type.Rook;

                DialogResult = DialogResult.OK;
                Close();
            }
        }
    }
}
