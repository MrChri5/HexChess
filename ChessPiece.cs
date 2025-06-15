namespace HexChess
{
    /// <summary>
    /// Class to represent a chess piece on the board. Chess boards are defined as a ChessPiece array with empty cells denoted by Type.None and Owner.None.
    /// </summary>
    public class ChessPiece
    {
        /// <summary>
        /// Represents the different types of chess piece.
        /// </summary>
        public enum Type { None, Pawn, Rook, Knight, Bishop, Queen, King };
        /// <summary>
        /// The chess piece type.
        /// </summary>
        public Type PieceType { get; }

        /// <summary>
        /// Owner of the chess piece.
        /// </summary>
        public HexBoard.Player PieceOwner { get; }

        private static readonly string[] TypeShorthand = ["", "", "R", "N", "B", "Q", "K"];
        /// <summary>
        /// Shorthand for the chess piece type to be used in chess notation.
        /// </summary>
        public string PieceShorthand { get; }

        /// <summary>
        /// Create a new chess piece of the Type specified and for the Player specified.
        /// </summary>
        /// <param name="PieceType"></param>
        /// <param name="PieceOwner"></param>
        public ChessPiece(Type pieceType, HexBoard.Player pieceOwner)
        {
            PieceType = pieceType;
            PieceOwner = pieceOwner;
            PieceShorthand = TypeShorthand[(int)pieceType];
        }
        /// <summary>
        /// Create a new chess piece to represent an empty cell on the board.
        /// </summary>
        public ChessPiece() : this(Type.None, HexBoard.Player.None)
        {
        }

        /// <summary>
        /// Check if a passed string matches the shorthand of any piece type.
        /// Type.None will be returned if no match. Pawns do not have a shorthand.
        /// </summary>
        /// <param name="shorthand"></param>
        /// <returns></returns>
        public static ChessPiece.Type GetPieceType(string shorthand)
        {
            //Loop through the piece shorthands and check if any match. Skip the ones for None and Pawn (Enum items 0 and 1).
            for (int i = 2; i < TypeShorthand.Length; i++)
            {
                if (TypeShorthand[i].Equals(shorthand))
                    return (ChessPiece.Type)i;
            }
            //If there was no match, return None.
            return Type.None;
        }
    }
}
