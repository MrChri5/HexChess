namespace HexChess
{
    /// <summary>
    /// A Game of Hexagonal Chess.
    /// </summary>
    public class HexBoard
    {
        /// <summary>
        /// Different possible board layouts used for Hexagonal Chess games and for demonstrating piece moves.
        /// </summary>
        public enum Variant { Empty, Glinski, testPawn, testRook, testKnight, testBishop, testQueenKing, testEnPassant, Decoration };
        /// <summary>
        /// The variant of the current board. Glinski is to standard game layout and rules. Variants begining with 'test' are to demonstrate how pieces move.
        /// </summary>
        public Variant BoardVariant { get; }
        /// <summary>
        /// The length of one side of the hexagonal board.
        /// </summary>
        public int BoardSize { get; }
        /// <summary>
        /// The maximum File and Rank of the board.
        /// </summary>
        public int BoardMax { get; }

        /// <summary>
        /// Represents the players in the game of Hexagonal Chess.
        /// </summary>
        public enum Player { None, Player1, Player2 };
        /// <summary>
        /// The current player who's turn it is to make a move. In the event of checkmate this will be set to the losing player.
        /// </summary>
        public Player CurrentPlayer { get; private set; }
        /// <summary>
        /// The maximum number of players in the game. Usually two, but test boards only have one player.
        /// </summary>
        public int MaxPlayers { get; }

        /// <summary>
        /// The current layout of pieces on the board. Empty cells are represented by a ChessPiece with Type and Player = None.
        /// </summary>
        public ChessPiece[,] BoardLayout { get; private set; }

        /// <summary>
        /// Represents the current status of the game.
        /// </summary>
        public enum Status { InProgress, Check, Checkmate, Draw }
        /// <summary>
        /// The current status of the game. If check/checkmate/draw has occurred.
        /// </summary>
        public Status GameStatus { get; private set; }
        /// <summary>
        /// String outputs the long algebraic notation of the last move.
        /// </summary>
        public string Notation { get; private set; }
        /// <summary>
        /// Specifies which number move has just been made. Increments only after both players have moved.
        /// </summary>
        public int MoveNumber { get; private set; }
        /// <summary>
        /// String contains all the moves made in this game, in long algebraic notation.
        /// </summary>
        public string SaveGame { get; private set; }

        //This will be set to the file of the pawn if the previous player moved their pawn two spaces.
        //If so the opponent may be able to take that pawn via En Passant.
        private int? enPassantFile;

        /// <summary>
        /// Create a new game of Hexagonal Chess using the board layout specified.
        /// </summary>
        /// <param name="variant"></param>
        public HexBoard(Variant variant)
        {
            //Initialize fields common to all variants.
            CurrentPlayer = Player.Player1;
            GameStatus = Status.InProgress;
            BoardVariant = variant;
            Notation = String.Empty;
            SaveGame = variant.ToString();
            MoveNumber = 0;
            enPassantFile = null;


            //Initialize the board sizes for a regular hexagon board.
            BoardSize = 6;
            BoardMax = (2 * BoardSize) - 1;
            BoardLayout = new ChessPiece[BoardMax, BoardMax];

            //Set up board pieces depending on the Variant.
            switch (variant)
            {
                case Variant.Glinski:
                    MaxPlayers = 2;
                    for (int i = 1; i <= 9; i++)
                    {
                        //Add Pawns for both players.
                        BoardLayout[i, 4 - Math.Abs(i - 5)] = new ChessPiece(ChessPiece.Type.Pawn, Player.Player1);
                        BoardLayout[i, 6] = new ChessPiece(ChessPiece.Type.Pawn, Player.Player2);
                        //Add Rooks for both players.
                        if (i == 2 || i == 8)
                        {
                            BoardLayout[i, 0] = new ChessPiece(ChessPiece.Type.Rook, Player.Player1);
                            BoardLayout[i, 7] = new ChessPiece(ChessPiece.Type.Rook, Player.Player2);
                        }
                        //Add Knights for both players.
                        if (i == 3 || i == 7)
                        {
                            BoardLayout[i, 0] = new ChessPiece(ChessPiece.Type.Knight, Player.Player1);
                            BoardLayout[i, 8] = new ChessPiece(ChessPiece.Type.Knight, Player.Player2);
                        }
                        //Add Bishops for both players.
                        if (i == 5)
                        {
                            BoardLayout[i, 0] = new ChessPiece(ChessPiece.Type.Bishop, Player.Player1);
                            BoardLayout[i, 1] = new ChessPiece(ChessPiece.Type.Bishop, Player.Player1);
                            BoardLayout[i, 2] = new ChessPiece(ChessPiece.Type.Bishop, Player.Player1);
                            BoardLayout[i, 8] = new ChessPiece(ChessPiece.Type.Bishop, Player.Player2);
                            BoardLayout[i, 9] = new ChessPiece(ChessPiece.Type.Bishop, Player.Player2);
                            BoardLayout[i, 10] = new ChessPiece(ChessPiece.Type.Bishop, Player.Player2);
                        }
                        //Add a Queen for each player.
                        if (i == 4)
                        {
                            BoardLayout[i, 0] = new ChessPiece(ChessPiece.Type.Queen, Player.Player1);
                            BoardLayout[i, 9] = new ChessPiece(ChessPiece.Type.Queen, Player.Player2);
                        }
                        //Add a King for each player.
                        if (i == 6)
                        {
                            BoardLayout[i, 0] = new ChessPiece(ChessPiece.Type.King, Player.Player1);
                            BoardLayout[i, 9] = new ChessPiece(ChessPiece.Type.King, Player.Player2);
                        }
                    }
                    break;
                case Variant.testPawn:
                    //Add Pawns for one player.
                    MaxPlayers = 1;
                    for (int i = 1; i <= 9; i++)
                        BoardLayout[i, 4 - Math.Abs(i - 5)] = new ChessPiece(ChessPiece.Type.Pawn, Player.Player1);
                    break;
                case Variant.testRook:
                    //Add Rooks for one player.
                    MaxPlayers = 1;
                    BoardLayout[2, 0] = new ChessPiece(ChessPiece.Type.Rook, Player.Player1);
                    BoardLayout[8, 0] = new ChessPiece(ChessPiece.Type.Rook, Player.Player1);
                    break;
                case Variant.testKnight:
                    //Add Knights for one player.
                    MaxPlayers = 1;
                    BoardLayout[3, 0] = new ChessPiece(ChessPiece.Type.Knight, Player.Player1);
                    BoardLayout[7, 0] = new ChessPiece(ChessPiece.Type.Knight, Player.Player1);
                    break;
                case Variant.testBishop:
                    //Add Bishops for one player.
                    MaxPlayers = 1;
                    BoardLayout[5, 0] = new ChessPiece(ChessPiece.Type.Bishop, Player.Player1);
                    BoardLayout[5, 1] = new ChessPiece(ChessPiece.Type.Bishop, Player.Player1);
                    BoardLayout[5, 2] = new ChessPiece(ChessPiece.Type.Bishop, Player.Player1);
                    break;
                case Variant.testQueenKing:
                    //Add a Queen and a King for one player.
                    MaxPlayers = 1;
                    BoardLayout[4, 0] = new ChessPiece(ChessPiece.Type.Queen, Player.Player1);
                    BoardLayout[6, 0] = new ChessPiece(ChessPiece.Type.King, Player.Player1);
                    break;
                case Variant.testEnPassant:
                    //Add certain pawns for both players.
                    MaxPlayers = 2;
                    BoardLayout[1, 4] = new ChessPiece(ChessPiece.Type.Pawn, Player.Player1);
                    BoardLayout[2, 1] = new ChessPiece(ChessPiece.Type.Pawn, Player.Player1);
                    BoardLayout[7, 4] = new ChessPiece(ChessPiece.Type.Pawn, Player.Player1);
                    BoardLayout[8, 1] = new ChessPiece(ChessPiece.Type.Pawn, Player.Player1);
                    BoardLayout[2, 6] = new ChessPiece(ChessPiece.Type.Pawn, Player.Player2);
                    BoardLayout[3, 3] = new ChessPiece(ChessPiece.Type.Pawn, Player.Player2);
                    BoardLayout[6, 6] = new ChessPiece(ChessPiece.Type.Pawn, Player.Player2);
                    BoardLayout[9, 2] = new ChessPiece(ChessPiece.Type.Pawn, Player.Player2);
                    break;
                case Variant.Decoration:
                    //Decorative layout when starting the program.
                    MaxPlayers = 0;
                    for (int i = 0; i < BoardMax; i++)
                        for (int j = 0; j < BoardMax; j++)
                        {                          
                            BoardLayout[i, j] = new ChessPiece((ChessPiece.Type)(1 + (j - Math.Abs(i - BoardSize + 1) + BoardSize) % 6), Player.Player2);
                        }

                    break;
                case Variant.Empty:
                    MaxPlayers = 0;
                    break;
            }
            //Initialize all remaining cells to an empty ChessPiece.
            for (int i = 0; i < BoardLayout.GetLength(0); i++)
                for (int j = 0; j < BoardLayout.GetLength(1); j++)
                {
                    if (BoardLayout[i, j] == null)
                        BoardLayout[i, j] = new ChessPiece();
                }
        }
        /// <summary>
        /// Create a new game of Hexagonal Chess using a save file.
        /// </summary>
        /// <param name="saveFile"></param>
        public HexBoard(string saveFile) : this(ParseVariant(saveFile))
        {
            //Read through the rest of the save file.
            string readLine = String.Empty;
            int lineNumber = 0;
            for (int i = 0; i <= saveFile.Length; i++)
            {
                //Once the end of a line is reached, process the line.
                if (i == saveFile.Length || saveFile[i] == '\n' || saveFile[i] == '\r')
                {
                    //Skip the first line, it contains the board variant.
                    if (!String.IsNullOrEmpty(readLine) && lineNumber != 0)
                    {
                        //First remove the move number.
                        readLine = readLine.Substring(readLine.IndexOf('.') + 1);
                        //Split the line into two parts.
                        //The first player's move, and (optionally) the second player's move. 
                        string[] readLineMoves = readLine.Split(' ');
                        //Parse each move and make it on the board.
                        for (int j = 0; j < 2; j++)
                        {
                            string playerMove = readLineMoves[j].Trim();
                            if (!String.IsNullOrEmpty(playerMove))
                            {
                                //Each move will first have the piece type being moved (or not if moving a pawn).
                                //Then the cell being moved from.
                                //Then possibly 'x' to indicate a capture.
                                //Then the cell being moved to.
                                //Then possibly 'e.p.' for En Passant move. Then possibly the piece type if promoting a pawn.
                                //Then possibly '+' if the move resulted in check.
                                int? fromFile;
                                int? fromRank;
                                int? toFile;
                                int? toRank;
                                ChessPiece.Type? movedPiece;
                                ChessPiece.Type? promoteTo;
                                int charPtr = 0;

                                //Get the piece type being moved. If there is no piece type then a Pawn is being moved, so don't increment charPtr.
                                if ((movedPiece = ChessPiece.GetPieceType(playerMove[charPtr].ToString())) != ChessPiece.Type.None)
                                    charPtr++;
                                else
                                    movedPiece = ChessPiece.Type.Pawn;
                                //Get the File of the piece being moved.
                                fromFile = GetFileIndex(playerMove[charPtr++].ToString());
                                //Get the Rank of the piece being moved. First check if it is a two digit rank.
                                try
                                {
                                    fromRank = GetRankIndex(playerMove.Substring(charPtr, 2));
                                    charPtr += 2;
                                }
                                catch (ArgumentOutOfRangeException)
                                {
                                    fromRank = GetRankIndex(playerMove[charPtr++].ToString());
                                }
                                //Skip 'x' indicating a piece was captured.
                                if (playerMove[charPtr] == 'x')
                                    charPtr++;
                                //Get the File of the cell being moved to.
                                toFile = GetFileIndex(playerMove[charPtr++].ToString());
                                //Get the Rank of the cell being moved to. First check if it is a two digit rank.
                                if ((charPtr + 2) <= playerMove.Length)
                                {
                                    try
                                    {
                                        toRank = GetRankIndex(playerMove.Substring(charPtr, 2));
                                        charPtr += 2;
                                    }
                                    catch (ArgumentOutOfRangeException)
                                    {
                                        toRank = GetRankIndex(playerMove[charPtr++].ToString());
                                    }
                                }
                                else
                                {
                                    toRank = GetRankIndex(playerMove[charPtr++].ToString());
                                }
                                //Skip 'e.p.' to indicate En Passant
                                if ((charPtr + 4) < playerMove.Length && playerMove.Substring(charPtr, 4) == "e.p.")
                                    charPtr += 4;
                                //Check the File and Rank for the starting and ending cells have been found
                                //Also check the piece being moved is of the correct type and player.
                                if (fromFile == null || fromRank == null || toFile == null || toRank == null
                                    || BoardLayout[fromFile.Value, fromRank.Value].PieceType != movedPiece
                                    || BoardLayout[fromFile.Value, fromRank.Value].PieceOwner != CurrentPlayer)
                                    throw new ArgumentException("Invalid Move");

                                if (charPtr < playerMove.Length && (promoteTo = ChessPiece.GetPieceType(playerMove[charPtr].ToString())) != ChessPiece.Type.None)
                                {
                                    MovePiece(fromFile.Value, fromRank.Value, toFile.Value, toRank.Value, promoteTo.Value);
                                }
                                else
                                {
                                    MovePiece(fromFile.Value, fromRank.Value, toFile.Value, toRank.Value);
                                }
                            }
                        }
                    }
                    readLine = String.Empty;
                    lineNumber++;
                }
                else
                {
                    readLine += saveFile[i];
                }
            }
        }

        //Read the first line of the save file to get the variant.
        static private Variant ParseVariant(string variant)
        {
            string readVariant = String.Empty;
            if (variant.Length > 0)
            {
                for (int i = 0; variant[i] != '\n' && variant[i] != '\r'; i++)
                {
                    readVariant += variant[i];
                }
                if (readVariant == Variant.Glinski.ToString())
                {
                    return Variant.Glinski;
                }
            }

            throw new ArgumentException("Invalid variant");
        }

        /// <summary>
        /// Check that the cell passed is a valid cell on the board. Cell should be specified as {zero-indexed file, zero-indexed rank}.
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public bool ValidateCell(int[] cell)
        {
            if (cell == null || cell.Length != 2)
                return false;
            return ValidateCell(cell[0], cell[1]);
        }
        /// <summary>
        /// Check that the cell passed is a valid cell on the board. Cell should be specified as zero-indexed file, zero-indexed rank.
        /// </summary>
        /// <param name="fileIndex"></param>
        /// <param name="rankIndex"></param>
        /// <returns></returns>
        public bool ValidateCell(int fileIndex, int rankIndex)
        {
            if (rankIndex < 0 || fileIndex < 0 || rankIndex >= BoardMax || fileIndex >= BoardMax)
                return false;
            if (fileIndex < BoardSize)
            {
                if (rankIndex >= fileIndex + BoardSize)
                    return false;
            }
            else
            {
                if (rankIndex > (3 * (BoardSize - 1)) - fileIndex)
                    return false;
            }
            return true;
        }

        private static readonly string fileLetters = "ABCDEFGHIKLMNOPQRS";
        /// <summary>
        /// Get the letter of the File to display on the board and in notation.
        /// The letter J is not used as it is too similar to I.
        /// </summary>
        /// <param name="fileIndex"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public string GetFile(int fileIndex)
        {
            if (fileIndex < 0 || fileIndex >= BoardMax || fileIndex >= fileLetters.Length)
                throw new ArgumentOutOfRangeException(nameof(fileIndex), "Invalid File Index");
            return fileLetters[fileIndex].ToString();
        }
        private int GetFileIndex(string file)
        {
            if (file.Length == 1)
            {
                for (int fileIndex = 0; fileIndex < fileLetters.Length; fileIndex++)
                {
                    if (fileLetters[fileIndex] == Char.ToUpper(file[0]) && fileIndex < BoardMax)
                        return fileIndex;
                }
            }
            throw new ArgumentOutOfRangeException(nameof(file), "Invalid File");
        }
        /// <summary>
        /// Get the number of the Rank to display on the board and in notation.
        /// </summary>
        /// <param name="rankIndex"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public string GetRank(int rankIndex)
        {
            if (rankIndex < 0 || rankIndex >= BoardMax)
                throw new ArgumentOutOfRangeException(nameof(rankIndex), "Invalid Rank Index");
            return (rankIndex + 1).ToString();
        }
        private int GetRankIndex(string rank)
        {
            if (int.TryParse(rank, out int rankIndex) && rankIndex > 0 && rankIndex <= BoardMax)
                return rankIndex - 1;
            throw new ArgumentOutOfRangeException(nameof(rank), "Invalid Rank");
        }

        /// <summary>
        /// Return a list of pieces that can be moved by the current player. Pieces are specified by their zero-indexed file and rank.
        /// </summary>
        /// <returns></returns>
        public int[][] GetValidPieces()
        {
            int maxPieces = 18;
            int[][] validPieces = new int[maxPieces][];
            //Return no valid pieces if the game has ended.
            if (GameStatus == Status.Checkmate || GameStatus == Status.Draw)
                return validPieces;

            int addPiece = 0;
            if (CurrentPlayer != Player.None && BoardVariant != Variant.Empty)
            {
                for (int i = 0; i < BoardLayout.GetLength(0); i++)
                    for (int j = 0; j < BoardLayout.GetLength(1); j++)
                    {
                        if (BoardLayout[i, j].PieceOwner == CurrentPlayer && GetValidMoves(i, j, true)[0] != null)
                            validPieces[addPiece++] = [i, j];
                    }
            }
            return validPieces;
        }
        /// <summary>
        /// Return a list of all valid moves for a piece. Pieces and their moves are specified by their zero-indexed file and rank.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="rank"></param>
        /// <returns></returns>
        public int[][] GetValidMoves(int file, int rank)
        {
            return GetValidMoves(file, rank, false);
        }
        //Calculate possible moves for a piece.
        //Set findFirstOnly to true to return once we have determined that the piece has at least one valid move.
        private int[][] GetValidMoves(int file, int rank, bool findFirstOnly)
        {
            //Maximum number of moves is less than the size of the BoardLayout array.            
            int[][] validMoves = new int[BoardLayout.Length][];
            ChessPiece movePiece = BoardLayout[file, rank];
            //Find the valid moves depending on the piece type. If findFirstOnly is set, exit once the first move is found.
            while (!findFirstOnly || validMoves[0] == null)
            {
                switch (movePiece.PieceType)
                {
                    case ChessPiece.Type.Pawn:
                        //Process valid moves for Player 1's pawns.
                        if (movePiece.PieceOwner == Player.Player1)
                        {
                            //Add move for one space in front of pawn if space is empty.
                            //Then, if pawn has not moved from the starting points, also add move for two spaces in front of pawn if empty.
                            if (AddMove(file, rank, Direction.XII, 1, MoveAction.MoveOnly, ref validMoves)
                                && rank == 4 - Math.Abs(file - 5))
                            {
                                AddMove(file, rank, Direction.XII, 2, MoveAction.MoveOnly, ref validMoves);
                            }
                            //Add moves for capturing to the forward side.
                            AddMove(file, rank, Direction.II, 1, MoveAction.CaptureOnly, ref validMoves);
                            AddMove(file, rank, Direction.X, 1, MoveAction.CaptureOnly, ref validMoves);
                            //If the opponent just moved a pawn two spaces, check if it can be captured En Passant.
                            if (enPassantFile != null)
                            {
                                int[] tryMove;
                                try
                                {
                                    tryMove = GetMove(file, rank, Direction.II);
                                    if (tryMove[0] == enPassantFile && tryMove[1] == 5)
                                    {
                                        AddMove(file, rank, Direction.II, 1, MoveAction.MoveOnly, ref validMoves);
                                    }
                                }
                                catch (ArgumentOutOfRangeException)
                                {
                                }
                                try
                                {
                                    tryMove = GetMove(file, rank, Direction.X);
                                    if (tryMove[0] == enPassantFile && tryMove[1] == 5)
                                    {
                                        AddMove(file, rank, Direction.X, 1, MoveAction.MoveOnly, ref validMoves);
                                    }
                                }
                                catch (ArgumentOutOfRangeException)
                                {
                                }
                            }
                        }
                        //Process valid moves for Player 2's pawns.
                        else if (movePiece.PieceOwner == Player.Player2)
                        {
                            //Add move for one space in front of pawn if space is empty.
                            //Then, if pawn has not moved from the starting points, also add move for two spaces in front of pawn if empty.
                            if (AddMove(file, rank, Direction.VI, 1, MoveAction.MoveOnly, ref validMoves)
                                && rank == 6)
                            {
                                AddMove(file, rank, Direction.VI, 2, MoveAction.MoveOnly, ref validMoves);
                            }
                            //Add moves for capturing to the forward side.
                            AddMove(file, rank, Direction.IV, 1, MoveAction.CaptureOnly, ref validMoves);
                            AddMove(file, rank, Direction.VIII, 1, MoveAction.CaptureOnly, ref validMoves);
                            //If the opponent just moved a pawn two spaces, check if it can be captured En Passant.
                            if (enPassantFile != null)
                            {
                                int[] tryMove;
                                try
                                {
                                    tryMove = GetMove(file, rank, Direction.IV);
                                    if (tryMove[0] == enPassantFile && tryMove[1] == 5 - Math.Abs((int)enPassantFile - 5))
                                    {
                                        AddMove(file, rank, Direction.IV, 1, MoveAction.MoveOnly, ref validMoves);
                                    }
                                }
                                catch (ArgumentOutOfRangeException)
                                {
                                }
                                try
                                {
                                    tryMove = GetMove(file, rank, Direction.VIII);
                                    if (tryMove[0] == enPassantFile && tryMove[1] == 5 - Math.Abs((int)enPassantFile - 5))
                                    {
                                        AddMove(file, rank, Direction.VIII, 1, MoveAction.MoveOnly, ref validMoves);
                                    }
                                }
                                catch (ArgumentOutOfRangeException)
                                {
                                }
                            }

                        }
                        break;
                    case ChessPiece.Type.Rook:
                        //Loop through all adjacent directions and add moves of any distance for each one.
                        for (int i = 1; i < directionsMax; i += 2)
                            for (int j = 0; j < BoardMax; j++)
                            {
                                //If moving in this direction meets a piece to capture or the piece cannot move to the cell (cell does not exist or is occupied),
                                //stop looping since the piece cannot move further in this direction.
                                if (AddMove(file, rank, (Direction)i, j, MoveAction.CaptureOnly, ref validMoves)
                                    || !AddMove(file, rank, (Direction)i, j, MoveAction.MoveOnly, ref validMoves))
                                {
                                    break;
                                }
                            }
                        break;
                    case ChessPiece.Type.Knight:
                        //Loop through all knight-moves and add each one.
                        for (int i = 0; i < directionsMax; i++)
                        {
                            AddMove(file, rank, (KnightMove)i, MoveAction.Any, ref validMoves);
                        }
                        break;
                    case ChessPiece.Type.Bishop:
                        //Loop through all diagonal directions and add moves of any distance for each one.
                        for (int i = 0; i < directionsMax; i += 2)
                            for (int j = 0; j < BoardMax; j++)
                            {
                                //If moving in this direction meets a piece to capture or the piece cannot move to the cell (cell does not exist or is occupied),
                                //stop looping since piece cannot move further in this direction.
                                if (AddMove(file, rank, (Direction)i, j, MoveAction.CaptureOnly, ref validMoves)
                                    || !AddMove(file, rank, (Direction)i, j, MoveAction.MoveOnly, ref validMoves))
                                {
                                    break;
                                }
                            }
                        break;
                    case ChessPiece.Type.Queen:
                        //Loop through all adjacent and diagonal directions and add moves of any distance for each one.
                        for (int i = 0; i < directionsMax; i += 1)
                            for (int j = 0; j < BoardMax; j++)
                            {
                                //If moving in this direction meets a piece to capture or the piece cannot move to the cell (cell does not exist or is occupied),
                                //stop looping since the piece cannot move further in this direction.
                                if (AddMove(file, rank, (Direction)i, j, MoveAction.CaptureOnly, ref validMoves)
                                    || !AddMove(file, rank, (Direction)i, j, MoveAction.MoveOnly, ref validMoves))
                                {
                                    break;
                                }
                            }
                        break;
                    case ChessPiece.Type.King:
                        //Loop through all adjacent and diagonal directions and add a move for each one.
                        for (int i = 0; i < directionsMax; i++)
                        {
                            //Check that moving the king won't result in moving to a cell where it can be captured.
                            try
                            {
                                int[] targetCell = GetMove(file, rank, (Direction)i);
                                if (ValidateKingMove(targetCell[0], targetCell[1], movePiece.PieceOwner))
                                    AddMove(file, rank, (Direction)i, 1, MoveAction.Any, ref validMoves);
                            }
                            //If move is invalid, skip to the next move.
                            catch (ArgumentOutOfRangeException)
                            {
                            }
                        }
                        break;
                    default:
                        break;
                }
                //Once all valid moves have been added, break out of the while statement.
                break;
            }
            //Return the list of valid moves.
            return validMoves;
        }
        //Method checks if the King can be moved to the specified cell without being captured.
        private bool ValidateKingMove(int file, int rank, Player player)
        {
            //Check all normal directions for pieces that could capture it.
            for (int i = 0; i < directionsMax; i++)
            {
                int searchFile = file;
                int searchRank = rank;

                for (int j = 0; j < BoardMax; j++)
                {
                    try
                    {
                        //Calculate the next move in this direction.
                        int[] searchCell = GetMove(searchFile, searchRank, (Direction)i);
                        searchFile = searchCell[0];
                        searchRank = searchCell[1];
                        Player searchCellOwner = BoardLayout[searchCell[0], searchCell[1]].PieceOwner;
                        if (searchCellOwner != Player.None && searchCellOwner != player)
                        {
                            switch ((Direction)i)
                            {
                                //Check diagonal directions for Bishop, Queen, and King.
                                case Direction.I:
                                case Direction.III:
                                case Direction.V:
                                case Direction.VII:
                                case Direction.IX:
                                case Direction.XI:
                                    if (BoardLayout[searchCell[0], searchCell[1]].PieceType == ChessPiece.Type.Bishop
                                        || BoardLayout[searchCell[0], searchCell[1]].PieceType == ChessPiece.Type.Queen
                                        || (j == 0 && BoardLayout[searchCell[0], searchCell[1]].PieceType == ChessPiece.Type.King))
                                    {
                                        return false;
                                    }
                                    break;

                                //Check if black pawn, then go on to check Rook, Queen, and King
                                case Direction.II:
                                case Direction.X:
                                    if (searchCellOwner == Player.Player2
                                        && j == 0
                                        && BoardLayout[searchCell[0], searchCell[1]].PieceType == ChessPiece.Type.Pawn)
                                    {
                                        return false;
                                    }
                                    goto case Direction.XII;

                                //Check if white pawn, then go on to check Rook, Queen, and King
                                case Direction.IV:
                                case Direction.VIII:
                                    if (searchCellOwner == Player.Player1
                                        && j == 0
                                        && BoardLayout[searchCell[0], searchCell[1]].PieceType == ChessPiece.Type.Pawn)
                                    {
                                        return false;
                                    }
                                    goto case Direction.XII;

                                //Check adjacent directions for Rook, Queen, and King.
                                case Direction.VI:
                                case Direction.XII:
                                    if (BoardLayout[searchCell[0], searchCell[1]].PieceType == ChessPiece.Type.Rook
                                        || BoardLayout[searchCell[0], searchCell[1]].PieceType == ChessPiece.Type.Queen
                                        || (j == 0 && BoardLayout[searchCell[0], searchCell[1]].PieceType == ChessPiece.Type.King))
                                    {
                                        return false;
                                    }
                                    break;
                            }
                        }
                        //Stop checking in this direction if an enemy piece is reached.
                        if (searchCellOwner != Player.None && searchCellOwner != player)
                        {
                            break;
                        }
                        //Stop searching in this direction if an ally piece is reached.
                        //If it is the current player's King, ignore it. This is the piece that is being verified.
                        if (searchCellOwner == player && (BoardLayout[searchCell[0], searchCell[1]].PieceType != ChessPiece.Type.King))
                        {
                            break;
                        }
                    }
                    //If move is not valid, skip on to the next move.
                    catch (ArgumentOutOfRangeException)
                    {
                        break;
                    }
                }
            }

            //Check all Knight directions.
            for (int i = 0; i < directionsMax; i++)
            {
                try
                {
                    int[] searchCell = GetMove(file, rank, (KnightMove)i);
                    Player searchCellOwner = BoardLayout[searchCell[0], searchCell[1]].PieceOwner;
                    if (searchCellOwner != Player.None && searchCellOwner != player)
                    {
                        if (BoardLayout[searchCell[0], searchCell[1]].PieceType == ChessPiece.Type.Knight)
                        {
                            return false;
                        }
                    }
                }
                //If move is not valid, skip on to the next move.
                catch (ArgumentOutOfRangeException)
                {
                }
            }

            //The king cannot be captured when moving to the selected cell. Return true.
            return true;
        }

        // Represents all possible adjacent and diagonal directions on the hexagonal chess board, as denoted on a clock face.
        // Moves to adjacent cells are II, IV, VI, VIII, X, XII.
        // Moves to diagonal cells are I, III, V, VII, IX, XI.       
        private enum Direction { I, II, III, IV, V, VI, VII, VIII, IX, X, XI, XII }
        private static readonly int directionsMax = Enum.GetNames(typeof(Direction)).Length;
        // Represents all possible knight-moves on the hexagonal chess board, as denoted on a clock face.
        private enum KnightMove { I, II, III, IV, V, VI, VII, VIII, IX, X, XI, XII }
        // Represents allowable actions when checking if a move is valid.
        // MoveOnly if the move is valid only if moving to an empty cell, such as moving forwards with a pawn.
        // CaptureOnly if the move is valid only when capturing a piece, such as moving to the side with a pawn.
        private enum MoveAction { Any, MoveOnly, CaptureOnly }

        // Check if a move is valid, for adjacent or diagonal directions. 
        // The move is specified by the zero-indexed file and rank of the starting cell, the direction, the number to move, and the criteria to check.
        // AddMove returns True if the move is valid and adds the destination of the move to the array of moves passed, otherwise returns false.
        private bool AddMove(int file, int rank, Direction direction, int numberToMove, MoveAction action, ref int[][] moveArray)
        {
            try
            {
                //Get the index of the cell to move to.
                int[] tryMove = GetMove(file, rank, direction);
                for (int i = 1; i < numberToMove; i++)
                {
                    tryMove = GetMove(tryMove[0], tryMove[1], direction);
                }
                //The move is invalid if the cells being moved from and to contain pieces belonging to the same player.
                Player pieceOwner = BoardLayout[file, rank].PieceOwner;
                Player targetOwner = BoardLayout[tryMove[0], tryMove[1]].PieceOwner;
                if (pieceOwner == Player.None || targetOwner == pieceOwner)
                {
                    return false;
                }
                //Check if the target cell contains another piece and if that move is valid based on the MoveAction.
                //Move Only     - target cell must be empty
                //Capture Only  - target cell must be opponent piece
                //Any           - target cell can be empty or opponent piece
                if (action == MoveAction.Any
                    || (action == MoveAction.MoveOnly && targetOwner == Player.None)
                    || (action == MoveAction.CaptureOnly && targetOwner != Player.None))
                {
                    //Find the next empty entry in the array and add the move.
                    for (int i = 0; i < moveArray.Length; i++)
                    {
                        if (moveArray[i] == null)
                        {
                            moveArray[i] = tryMove;
                            return true;
                        }
                    }
                }
            }
            //Otherwise the move is invalid.
            catch (ArgumentOutOfRangeException)
            {
                return false;
            }
            return false;
        }
        // Check if a move is valid, for knight-moves. 
        // The move is specified by the zero-indexed file and rank of the starting cell, the direction, the number to move, and the criteria to check.
        // AddMove returns True if the move is valid and adds the destination of the move to the array of moves passed, otherwise returns false.
        private bool AddMove(int file, int rank, KnightMove direction, MoveAction action, ref int[][] moveArray)
        {
            try
            {
                //Get the index of the cell to move to.
                int[] tryMove = GetMove(file, rank, direction);
                //The move is invalid if the cells being moved from and to contain pieces belonging to the same player.
                Player pieceOwner = BoardLayout[file, rank].PieceOwner;
                Player targetOwner = BoardLayout[tryMove[0], tryMove[1]].PieceOwner;
                if (pieceOwner == Player.None || targetOwner == pieceOwner)
                {
                    return false;
                }
                //Check if the target cell contains another piece and if that move is valid based on the MoveAction.
                //Move Only     - target cell must be empty
                //Capture Only  - target cell must be opponent piece
                //Any           - target cell can be empty or opponent piece
                if (action == MoveAction.Any
                    || (action == MoveAction.MoveOnly && targetOwner == Player.None)
                    || (action == MoveAction.CaptureOnly && targetOwner != Player.None))
                {
                    //Find the next empty entry in the array and add the move.
                    for (int i = 0; i < moveArray.Length; i++)
                    {
                        if (moveArray[i] == null)
                        {
                            moveArray[i] = tryMove;
                            return true;
                        }
                    }
                }
            }
            //Otherwise the move is invalid.
            catch (ArgumentOutOfRangeException)
            {
                return false;
            }
            return false;
        }
        // Move in the direction specified (adjacent or diagonal) from the cell and return the index of the resulting cell.
        private int[] GetMove(int file, int rank, Direction direction)
        {
            //Initialize to an invalid cell, so an error occurs if it is not changed.
            int[] moveTo = [-1, -1];
            //Calculate the resulting cell index depending on the direction.
            //Calculations depend on which half of the board the move takes place and if the centre line is crossed, due to the chavron shaped ranks on the board.
            switch (direction)
            {
                //Adjacent Moves
                case Direction.II:
                    if (file < BoardSize - 1)
                        moveTo = [file + 1, rank + 1];
                    else
                        moveTo = [file + 1, rank];
                    break;
                case Direction.IV:
                    if (file < BoardSize - 1)
                        moveTo = [file + 1, rank];
                    else
                        moveTo = [file + 1, rank - 1];
                    break;
                case Direction.VI:
                    moveTo = [file, rank - 1];
                    break;
                case Direction.VIII:
                    if (file < BoardSize)
                        moveTo = [file - 1, rank - 1];
                    else
                        moveTo = [file - 1, rank];
                    break;
                case Direction.X:
                    if (file < BoardSize)
                        moveTo = [file - 1, rank];
                    else
                        moveTo = [file - 1, rank + 1];
                    break;
                case Direction.XII:
                    moveTo = [file, rank + 1];
                    break;

                //Diagonal Moves
                case Direction.I:
                    if (file < BoardSize - 1)
                        moveTo = [file + 1, rank + 2];
                    else
                        moveTo = [file + 1, rank + 1];
                    break;
                case Direction.III:
                    if (file < BoardSize - 2)
                        moveTo = [file + 2, rank + 1];
                    else if (file == BoardSize - 2)
                        moveTo = [file + 2, rank];
                    else
                        moveTo = [file + 2, rank - 1];
                    break;
                case Direction.V:
                    if (file < BoardSize - 1)
                        moveTo = [file + 1, rank - 1];
                    else
                        moveTo = [file + 1, rank - 2];
                    break;
                case Direction.VII:
                    if (file < BoardSize)
                        moveTo = [file - 1, rank - 2];
                    else
                        moveTo = [file - 1, rank - 1];
                    break;
                case Direction.IX:
                    if (file < BoardSize)
                        moveTo = [file - 2, rank - 1];
                    else if (file == BoardSize)
                        moveTo = [file - 2, rank];
                    else
                        moveTo = [file - 2, rank + 1];
                    break;
                case Direction.XI:
                    if (file < BoardSize)
                        moveTo = [file - 1, rank + 1];
                    else
                        moveTo = [file - 1, rank + 2];
                    break;
            }
            //Only return the cell if it is on the board
            if (ValidateCell(moveTo))
                return moveTo;
            else
                throw new ArgumentOutOfRangeException(nameof(direction), "GetMove Invalid");
        }
        // Move in the direction specified (knight-move) from the cell and return the index of the resulting cell.
        private int[] GetMove(int file, int rank, KnightMove direction)
        {
            //Initialize to an invalid cell, so an error occurs if it is not changed.
            int[] moveTo = [-1, -1];
            //Calculate the resulting cell index depending on the direction.
            //Calculations depend on which half of the board the move takes place and if the centre line is crossed, due to the chavron shaped ranks on the board.
            switch (direction)
            {
                //Knight Moves
                case KnightMove.I:
                    if (file < BoardSize - 1)
                        moveTo = [file + 1, rank + 3];
                    else
                        moveTo = [file + 1, rank + 2];
                    break;
                case KnightMove.II:
                    if (file < BoardSize - 2)
                        moveTo = [file + 2, rank + 3];
                    else if (file == BoardSize - 2)
                        moveTo = [file + 2, rank + 2];
                    else
                        moveTo = [file + 2, rank + 1];
                    break;
                case KnightMove.III:
                    if (file < BoardSize - 3)
                        moveTo = [file + 3, rank + 2];
                    else if (file == BoardSize - 3)
                        moveTo = [file + 3, rank + 1];
                    else if (file == BoardSize - 2)
                        moveTo = [file + 3, rank];
                    else
                        moveTo = [file + 3, rank - 1];
                    break;
                case KnightMove.IV:
                    if (file < BoardSize - 3)
                        moveTo = [file + 3, rank + 1];
                    else if (file == BoardSize - 3)
                        moveTo = [file + 3, rank];
                    else if (file == BoardSize - 2)
                        moveTo = [file + 3, rank - 1];
                    else
                        moveTo = [file + 3, rank - 2];
                    break;
                case KnightMove.V:
                    if (file < BoardSize - 2)
                        moveTo = [file + 2, rank - 1];
                    else if (file == BoardSize - 2)
                        moveTo = [file + 2, rank - 2];
                    else
                        moveTo = [file + 2, rank - 3];
                    break;
                case KnightMove.VI:
                    if (file < BoardSize - 1)
                        moveTo = [file + 1, rank - 2];
                    else
                        moveTo = [file + 1, rank - 3];
                    break;
                case KnightMove.VII:
                    if (file < BoardSize)
                        moveTo = [file - 1, rank - 3];
                    else
                        moveTo = [file - 1, rank - 2];
                    break;
                case KnightMove.VIII:
                    if (file < BoardSize)
                        moveTo = [file - 2, rank - 3];
                    else if (file == BoardSize)
                        moveTo = [file - 2, rank - 2];
                    else
                        moveTo = [file - 2, rank - 1];
                    break;
                case KnightMove.IX:
                    if (file < BoardSize)
                        moveTo = [file - 3, rank - 2];
                    else if (file == BoardSize)
                        moveTo = [file - 3, rank - 1];
                    else if (file == BoardSize + 1)
                        moveTo = [file - 3, rank];
                    else
                        moveTo = [file - 3, rank + 1];
                    break;
                case KnightMove.X:
                    if (file < BoardSize)
                        moveTo = [file - 3, rank - 1];
                    else if (file == BoardSize)
                        moveTo = [file - 3, rank];
                    else if (file == BoardSize + 1)
                        moveTo = [file - 3, rank + 1];
                    else
                        moveTo = [file - 3, rank + 2];
                    break;
                case KnightMove.XI:
                    if (file < BoardSize)
                        moveTo = [file - 2, rank + 1];
                    else if (file == BoardSize)
                        moveTo = [file - 2, rank + 2];
                    else
                        moveTo = [file - 2, rank + 3];
                    break;
                case KnightMove.XII:
                    if (file < BoardSize)
                        moveTo = [file - 1, rank + 2];
                    else
                        moveTo = [file - 1, rank + 3];
                    break;
            }
            //Only return the cell if it is on the board
            if (ValidateCell(moveTo))
                return moveTo;
            else
                throw new ArgumentOutOfRangeException(nameof(direction), "GetMove Invalid");
        }

        /// <summary>
        /// Check if making the specified move will result in pawn promotion.
        /// If so the ChessPiece.Type should be passed to MovePiece to specify the piece to promote to.
        /// </summary>
        /// <param name="fromFile"></param>
        /// <param name="fromRank"></param>
        /// <param name="toFile"></param>
        /// <param name="toRank"></param>
        /// <returns></returns>
        public bool CheckPromotion(int fromFile, int fromRank, int toFile, int toRank)
        {
            //Pawn promotion only occurs when moving a piece to the opposite end of the board.
            return BoardLayout[fromFile, fromRank].PieceType == ChessPiece.Type.Pawn
                && ((BoardLayout[fromFile, fromRank].PieceOwner == Player.Player1 && toRank == 10 - Math.Abs(toFile - 5))
                    || (BoardLayout[fromFile, fromRank].PieceOwner == Player.Player2 && toRank == 0));
        }

        /// <summary>
        /// Move the piece at (fromFile,fromRank) to (toFile,toRank).
        /// </summary>
        /// <param name="fromFile"></param>
        /// <param name="fromRank"></param>
        /// <param name="toFile"></param>
        /// <param name="toRank"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void MovePiece(int fromFile, int fromRank, int toFile, int toRank)
        {
            MovePiece(fromFile, fromRank, toFile, toRank, ChessPiece.Type.None);
        }
        /// <summary>
        /// Move the piece at (fromFile,fromRank) to (toFile,toRank). 
        /// Use pawnPromotion to specify what piece a pawn should be promoted to.
        /// </summary>
        /// <param name="fromFile"></param>
        /// <param name="fromRank"></param>
        /// <param name="toFile"></param>
        /// <param name="toRank"></param>
        /// <param name="pawnPromotion"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void MovePiece(int fromFile, int fromRank, int toFile, int toRank, ChessPiece.Type pawnPromotion)
        {
            //Check cells passed are valid cells on the board.
            if (ValidateCell(fromFile, fromRank) && ValidateCell(toFile, toRank))
            {
                //Check the move passed is a valid move for that piece.
                bool moveValid = false;
                foreach (int[] move in GetValidMoves(fromFile, fromRank))
                {
                    if (move == null)
                        break;
                    if (move[0] == toFile && move[1] == toRank)
                    {
                        moveValid = true;
                        break;
                    }
                }
                if (!moveValid)
                    throw new InvalidOperationException();

                //Check that if a pawn is to be promoted, that a ChessPiece.Type has been passed.
                bool promotion = false;
                if (CheckPromotion(fromFile, fromRank, toFile, toRank))
                {
                    if (pawnPromotion != ChessPiece.Type.None)
                        promotion = true;
                    else
                        throw new InvalidOperationException();
                }

                //If player 1 is making their move, increment the move number and write it to the save game file.
                if (CurrentPlayer == Player.Player1)
                {
                    MoveNumber++;
                    SaveGame += $"\n{MoveNumber}.";
                }

                //Write the long algebraic chess notation
                Notation = $"{BoardLayout[fromFile, fromRank].PieceShorthand}" +
                    $"{GetFile(fromFile).ToLower() + GetRank(fromRank).ToLower()}" +
                    $"{(BoardLayout[toFile, toRank].PieceType == ChessPiece.Type.None ? "" : "x")}" +
                    $"{GetFile(toFile).ToLower() + GetRank(toRank).ToLower()}";

                //If the player is capturing a pawn via En Passant, clear the captured pawn and output special notation.
                if (enPassantFile != null && (
                    (CurrentPlayer == Player.Player1
                        && BoardLayout[fromFile, fromRank].PieceType == ChessPiece.Type.Pawn
                        && toFile == enPassantFile && toRank == 5
                        && BoardLayout[toFile, toRank - 1].PieceType == ChessPiece.Type.Pawn
                        && BoardLayout[toFile, toRank - 1].PieceOwner != Player.Player1
                        )
                    || (CurrentPlayer == Player.Player2
                        && BoardLayout[fromFile, fromRank].PieceType == ChessPiece.Type.Pawn
                        && toFile == enPassantFile && toRank == 5 - Math.Abs(toFile - 5))
                        && BoardLayout[toFile, toRank + 1].PieceType == ChessPiece.Type.Pawn
                        && BoardLayout[toFile, toRank + 1].PieceOwner != Player.Player2
                        ))
                {
                    Notation = $"{BoardLayout[fromFile, fromRank].PieceShorthand}" +
                    $"{GetFile(fromFile).ToLower() + GetRank(fromRank).ToLower()}x" +
                    $"{GetFile(toFile).ToLower() + GetRank(toRank).ToLower()}e.p.";
                    BoardLayout[toFile, toRank + (CurrentPlayer == Player.Player1 ? -1 : 1)] = new ChessPiece();
                }
                enPassantFile = null;
                //If the player moved their pawn two spaces, set the En Passant File.
                //This will be used to determine of the pawn can be taken En Passant.
                if ((CurrentPlayer == Player.Player1
                        && BoardLayout[fromFile, fromRank].PieceType == ChessPiece.Type.Pawn
                        && fromFile == toFile && fromRank == 4 - Math.Abs(fromFile - 5) && toRank == 6 - Math.Abs(toFile - 5))
                    || (CurrentPlayer == Player.Player2
                        && BoardLayout[fromFile, fromRank].PieceType == ChessPiece.Type.Pawn
                        && fromFile == toFile && fromRank == 6 && toRank == 4))
                {
                    enPassantFile = fromFile;
                }

                //Move the piece to the new cell and replace the old cell with an empty piece.
                BoardLayout[toFile, toRank] = BoardLayout[fromFile, fromRank];
                BoardLayout[fromFile, fromRank] = new ChessPiece();
                //If a pawn was promoted, replace it with the new piece and add the piece type to the notation.
                if (promotion)
                {
                    BoardLayout[toFile, toRank] = new ChessPiece(pawnPromotion, CurrentPlayer);
                    Notation += BoardLayout[toFile, toRank].PieceShorthand;
                }

                //Set the current player to the next player.
                CurrentPlayer++;
                if ((int)CurrentPlayer > MaxPlayers)
                    CurrentPlayer = Player.Player1;

                //Update the game status.
                if (BoardVariant == Variant.Glinski)
                {
                    //Calculate which players Kings can be captured in the new board layout and if the next player has any valid moves.
                    bool currentPlayerCheck;
                    bool opponentPlayerCheck;
                    bool currentPlayerCanMove = (GetValidPieces()[0] != null);
                    if (CurrentPlayer == Player.Player1)
                    {
                        currentPlayerCheck = CheckKingCapture(Player.Player1);
                        opponentPlayerCheck = CheckKingCapture(Player.Player2);
                    }
                    else
                    {
                        currentPlayerCheck = CheckKingCapture(Player.Player2);
                        opponentPlayerCheck = CheckKingCapture(Player.Player1);
                    }

                    //If the player who just moved is in Check, then it is Checkmate
                    if (opponentPlayerCheck)
                    {
                        GameStatus = Status.Checkmate;
                    }
                    //Current Player is in check if their piece can be captured and they can still move. Add a '+' to the move notation.
                    else if (currentPlayerCheck && currentPlayerCanMove)
                    {
                        GameStatus = Status.Check;
                        Notation += '+';
                    }
                    //Current Player is in Checkmate if they are in Check and have no valid moves to escape.
                    //Set the current player to the opponent so the checkmate is given correctly.
                    else if (currentPlayerCheck && !currentPlayerCanMove)
                    {
                        if (CurrentPlayer == Player.Player1)
                            CurrentPlayer = Player.Player2;
                        else
                            CurrentPlayer = Player.Player1;
                        GameStatus = Status.Checkmate;
                    }
                    //Game is a draw if the current player has no valid moves.
                    else if (!currentPlayerCanMove)
                    {
                        GameStatus = Status.Draw;
                    }
                    //Otherwise game is in progress with no special status.
                    else
                    {
                        GameStatus = Status.InProgress;
                    }
                    //A game can also end in the following conditions.
                    //But these are not compulsory and so are not determined or enforced here.
                    //  1) Either player chooses to resign.
                    //  2) Both players agree to a draw.
                    //  3) Either player calls a draw in any of the following scenarios:
                    //     a) Not enough pieces for either player to achieve checkmate.
                    //     b) No piece has been captured nor a pawn moved in the past 50 moves.
                    //     c) The same positions occur for the third time in the game.
                }
                else
                    GameStatus = Status.InProgress;

                //Add the move to the save game file
                SaveGame += $"{Notation} ";
            }
            else
                throw new InvalidOperationException();
        }

        //Check if the specified player's king can be captured.
        //If the current player's king can be captured they are in check.
        //If the other player's king can be captured they have lost the game.
        private bool CheckKingCapture(Player player)
        {
            int[]? kingCoord = null;
            //Find the specified player's king.
            for (int i = 0; i < BoardLayout.GetLength(0); i++)
                for (int j = 0; j < BoardLayout.GetLength(1); j++)
                {
                    if (BoardLayout[i, j].PieceOwner == player && BoardLayout[i, j].PieceType == ChessPiece.Type.King)
                        kingCoord = [i, j];
                }
            //If no king was found return true. The game should have ended so no further processing is needed.
            if (kingCoord == null)
                return true;
            //Check all opponent pieces. For each one get it's valid moves and check if any matches the location of the King.
            for (int i = 0; i < BoardLayout.GetLength(0); i++)
                for (int j = 0; j < BoardLayout.GetLength(1); j++)
                {
                    if (BoardLayout[i, j].PieceOwner != Player.None && BoardLayout[i, j].PieceOwner != player)
                    {
                        foreach (int[] move in GetValidMoves(i, j))
                        {
                            if (move != null && move[0] == kingCoord[0] && move[1] == kingCoord[1])
                                return true;
                        }
                    }
                }

            return false;
        }
    }
}