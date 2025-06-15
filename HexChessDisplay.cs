using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using FontStyle = System.Drawing.FontStyle;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;
using Point = System.Drawing.Point;
using SaveFileDialog = System.Windows.Forms.SaveFileDialog;

namespace HexChess
{
    public partial class HexChessDisplay : Form
    {
        //drawing tools
        private enum ColourIds { Background, BorderHexagon, TextLabel, CellLight, CellMid, CellDark, Available, Selected };
        Color[] GameColours = [Color.WhiteSmoke, Color.Purple, Color.White, Color.LightGray, Color.Gray, Color.DarkGray, Color.Gold, Color.Purple];
        Color[] PlayerColours = [Color.Empty, Color.White, Color.Black];
        readonly SolidBrush borderHexagon;
        readonly SolidBrush textLabel;
        readonly SolidBrush cellLight;
        readonly SolidBrush cellMid;
        readonly SolidBrush cellDark;
        readonly SolidBrush highlightAvailable;
        readonly SolidBrush highlightSelected;
        readonly StringFormat centreLabel;
        readonly StringFormat leftLabel;
        readonly StringFormat rightLabel;
        readonly StringFormat centredBoth;
        readonly ImageAttributes[] imageAttributesPlayers;
        readonly Image chessPieceIcons;
        Font fileRankLabel = null!;

        //Variables to calculate board positions
        Point[,] cellCentres = null!;
        Point[,][] cellPoints = null!;
        Point[] backgroundHex = null!;
        int cellHeight;
        int cellCircle;

        //The game
        HexBoard hexBoard = null!;
        int[]? cellSelected;
        int[][]? cellsAvailable;
        bool gameModified;

        //Display the screen.
        public HexChessDisplay()
        {
            InitializeComponent();

            //Initialize drawing colours
            borderHexagon = new SolidBrush(GameColours[(int)ColourIds.BorderHexagon]);
            cellDark = new SolidBrush(GameColours[(int)ColourIds.CellDark]);
            cellMid = new SolidBrush(GameColours[(int)ColourIds.CellMid]);
            cellLight = new SolidBrush(GameColours[(int)ColourIds.CellLight]);
            highlightAvailable = new SolidBrush(GameColours[(int)ColourIds.Available]);
            highlightSelected = new SolidBrush(GameColours[(int)ColourIds.Selected]);
            textLabel = new SolidBrush(GameColours[(int)ColourIds.TextLabel]);

            //Initialize label formats
            centreLabel = new StringFormat();
            leftLabel = new StringFormat();
            rightLabel = new StringFormat();
            centredBoth = new StringFormat();
            centreLabel.LineAlignment = StringAlignment.Near;
            centreLabel.Alignment = StringAlignment.Center;
            leftLabel.LineAlignment = StringAlignment.Near;
            leftLabel.Alignment = StringAlignment.Center;
            rightLabel.LineAlignment = StringAlignment.Near;
            rightLabel.Alignment = StringAlignment.Center;
            centredBoth.LineAlignment = StringAlignment.Center;
            centredBoth.Alignment = StringAlignment.Center;

            //Initialize variables for drawing chess pieces.
            //Chess piece icons are all black. So create maps to recolour to each players colour.
            chessPieceIcons = new Bitmap(Properties.Resources.ChessPieces);
            imageAttributesPlayers = new ImageAttributes[3];
            ColorMap[] pieceColorMap = [new ColorMap()];
            pieceColorMap[0].OldColor = Color.Black;
            pieceColorMap[0].NewColor = PlayerColours[(int)HexBoard.Player.Player1];
            imageAttributesPlayers[(int)HexBoard.Player.Player1] = new ImageAttributes();
            imageAttributesPlayers[(int)HexBoard.Player.Player1].SetRemapTable(pieceColorMap);
            pieceColorMap[0].OldColor = Color.Black;
            pieceColorMap[0].NewColor = PlayerColours[(int)HexBoard.Player.Player2];
            imageAttributesPlayers[(int)HexBoard.Player.Player2] = new ImageAttributes();
            imageAttributesPlayers[(int)HexBoard.Player.Player2].SetRemapTable(pieceColorMap);

            //Initialize the labels to space so the strips are built.
            CurrentPlayerLabel.Text = " ";
            LastMoveLabel.Text = " ";

            InitializeBoard(HexBoard.Variant.Decoration);
            CalculateBoardPoints();
        }
        //Redisplay the game area
        private void BoardPanel_Paint(object sender, PaintEventArgs e)
        {
            //Draw the board.
            DrawHexBoard(e.Graphics);

            //If the game has ended in a win or a draw. Display a message
            switch (hexBoard.GameStatus)
            {
                case HexBoard.Status.Checkmate:
                    MessageBox.Show($"{hexBoard.CurrentPlayer} has won.");
                    break;
                case HexBoard.Status.Draw:
                    MessageBox.Show($"Game has ended in a draw.");
                    break;
                default:
                    break;
            }
        }
        //When the screen is resized, recalculate the board and redraw it.
        private void HexChessDisplay_SizeChanged(object sender, EventArgs e)
        {
            CalculateBoardPoints();
            BoardPanel.Invalidate();
        }
        //Clear the form.
        private void HexChessDisplay_FormClosed(object sender, FormClosedEventArgs e)
        {
            cellDark.Dispose();
            cellMid.Dispose();
            cellLight.Dispose();
            textLabel.Dispose();
            highlightAvailable.Dispose();
            highlightSelected.Dispose();
            borderHexagon.Dispose();
            fileRankLabel.Dispose();
            centreLabel.Dispose();
            leftLabel.Dispose();
            rightLabel.Dispose();
            centredBoth.Dispose();
            chessPieceIcons.Dispose();
        }

        // New Game menu items. Create a new game depending on the variant selected from the menu.
        private void GlinskisHexagonalChessToolStripMenuItem_Click(object sender, EventArgs e) => InitializeBoard(HexBoard.Variant.Glinski);
        private void PawnMovesToolStripMenuItem_Click(object sender, EventArgs e) => InitializeBoard(HexBoard.Variant.testPawn);
        private void RookMovesToolStripMenuItem_Click(object sender, EventArgs e) => InitializeBoard(HexBoard.Variant.testRook);
        private void KnightMovesToolStripMenuItem_Click(object sender, EventArgs e) => InitializeBoard(HexBoard.Variant.testKnight);
        private void BishopMovesToolStripMenuItem_Click(object sender, EventArgs e) => InitializeBoard(HexBoard.Variant.testBishop);
        private void QueenAndKingMovesToolStripMenuItem_Click(object sender, EventArgs e) => InitializeBoard(HexBoard.Variant.testQueenKing);
        private void EnPassantToolStripMenuItem_Click(object sender, EventArgs e) => InitializeBoard(HexBoard.Variant.testEnPassant);
        //Save Game menu item.
        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveGame();
        }
        //Load Game menu item.
        private void LoadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Check if the current game should be saved first.
            if (SavePrompt())
                LoadGame();
        }
        //Undo Move menu item.
        private void UndoMoveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Undo the last move
            UndoMove();
        }

        //Check if the user wants to save their current game before continuing. Returns false if the user selected to cancel the current action.
        private bool SavePrompt()
        {
            //Check if a game is already in progress.
            if (!gameModified)
                return true;
            //Only offer to save if the game is of a type that can be saved.
            switch (hexBoard.BoardVariant)
            {
                //These variants can be saved.
                case HexBoard.Variant.Glinski:
                    break;
                //These variants cannot be saved.
                case HexBoard.Variant.Empty:
                case HexBoard.Variant.testPawn:
                case HexBoard.Variant.testRook:
                case HexBoard.Variant.testKnight:
                case HexBoard.Variant.testBishop:
                case HexBoard.Variant.testQueenKing:
                case HexBoard.Variant.testEnPassant:
                default:
                    return true;
            }

            //Display the message to the user.
            MessageBoxResult messageBoxResult = MessageBox.Show(
            "Do you want to save your current game first?",
            "Save Game?",
            MessageBoxButton.YesNoCancel);
            //If user selects Yes, save the game before starting a new game.
            //If No, do not save before starting a new game.
            //If Cancel, do not save and do not start a new game.
            switch (messageBoxResult)
            {
                case MessageBoxResult.Yes:
                    SaveGame();
                    break;
                case MessageBoxResult.No:
                    break;
                default:
                    return false;
            }
            return true;
        }
        //Undo the last move.
        private void UndoMove()
        {
            //Take the current save game file and remove the last move.
            //This will either be Player 1's move which will be preceeded by a \n character, or Player 2's move which will be preceeded by a space.
            string undoGameFile = hexBoard.SaveGame.TrimEnd();
            int subtringIndex = Math.Max(undoGameFile.LastIndexOf(' '), undoGameFile.LastIndexOf('\n'));
            if (subtringIndex == -1)
                gameModified = false;
            undoGameFile = undoGameFile.Substring(0, subtringIndex + 1);

            //Re-load the game using the new save file.
            hexBoard = new HexBoard(undoGameFile);
            //Initialize other variables for the screen.
            gameModified = (hexBoard.Notation.Length > 0);
            cellSelected = null;
            cellsAvailable = hexBoard.GetValidPieces();
            //Redisplay the screen
            DisplayStrips();
            BoardPanel.Invalidate();
        }
        //Save the game to a file.
        private void SaveGame()
        {
            //If the game has not started, give an error.
            if (!gameModified)
            {
                MessageBox.Show("No changes to be saved.");
                return;
            }
            //Give an error if the current game is a variant that cannot be saved (empty board or test scenario).
            switch (hexBoard.BoardVariant)
            {

                //These variants can be saved.
                case HexBoard.Variant.Glinski:
                    break;
                //These variants cannot be saved.
                case HexBoard.Variant.Empty:
                case HexBoard.Variant.testPawn:
                case HexBoard.Variant.testRook:
                case HexBoard.Variant.testKnight:
                case HexBoard.Variant.testBishop:
                case HexBoard.Variant.testQueenKing:
                case HexBoard.Variant.testEnPassant:
                default:
                    MessageBox.Show("Game type cannot be saved.");
                    return;
            }

            //Show the file dialog to choose where to save the game file.
            Stream saveGameWriterStream;
            StreamWriter saveGameWriter;
            SaveFileDialog saveFileDialog = new();
            saveFileDialog.Filter = "Text Documents |*.txt";
            saveFileDialog.RestoreDirectory = true;

            if (saveFileDialog.ShowDialog() == DialogResult.OK
                && (saveGameWriterStream = saveFileDialog.OpenFile()) != null)
            {
                saveGameWriter = new StreamWriter(saveGameWriterStream);
                //Write the save game data and close the file.
                saveGameWriter.Write(hexBoard.SaveGame);
                saveGameWriter.Close();
                saveGameWriterStream.Close();
                gameModified = false;
            }
        }
        //Load the game from a save file.
        private void LoadGame()
        {
            Stream loadGameReaderStream;
            StreamReader loadGameReader;
            OpenFileDialog openFileDialog = new();
            openFileDialog.Filter = "Text Documents |*.txt";
            openFileDialog.RestoreDirectory = true;

            //Show the open file dialog to choose which file to load.
            if (openFileDialog.ShowDialog() == DialogResult.OK
                && (loadGameReaderStream = openFileDialog.OpenFile()) != null)
            {
                //Set the status strip to Loading Game.
                CurrentPlayerLabel.Text = "Loading Game . . .";
                LastMoveLabel.Text = " ";
                GameStatus.Update();

                loadGameReader = new StreamReader(loadGameReaderStream);
                //Read the file and use it to initialize a new game.
                try
                {
                    hexBoard = new HexBoard(loadGameReader.ReadToEnd());
                    loadGameReader.Close();
                    loadGameReaderStream.Close();
                }
                catch (ArgumentException e)
                {
                    //If an error occured, initialize the board to empty.
                    MessageBox.Show($"Invalid save file.\n{e.Message}");
                    InitializeBoard(HexBoard.Variant.Empty);
                    return;
                }
                //Reset the status strip. 
                finally
                {
                    CurrentPlayerLabel.Text = " ";
                    GameStatus.Update();
                }

                //Initialize other variables for the screen.
                gameModified = false;
                cellSelected = null;
                cellsAvailable = hexBoard.GetValidPieces();

                //Redisplay the screen
                CalculateBoardPoints();
                DisplayStrips();
                BoardPanel.Invalidate();
            }
        }

        //Set up a new game of Hexagonal Chess.
        private void InitializeBoard(HexBoard.Variant variant)
        {
            if (!SavePrompt())
                return;

            //Initialize a new chess board
            hexBoard = new HexBoard(variant);
            gameModified = false;
            cellSelected = null;
            cellsAvailable = hexBoard.GetValidPieces();

            //Redisplay the screen
            CalculateBoardPoints();
            DisplayStrips();
            BoardPanel.Invalidate();
        }
        //Process mouse inut to select pieces and move them.
        private void BoardPanel_MouseClick(object sender, MouseEventArgs e)
        {
            //Find which centre point the mouse click was closest to.
            Point mouseClick = e.Location;
            int findCentreX = 0;
            int findCentreY = 0;

            for (int i = 0; i < cellCentres.GetLength(0); i++)
            {
                if (mouseClick.X >= cellCentres[i, 0].X)
                    findCentreX = i;
            }
            for (int j = 0; j < cellCentres.GetLength(1); j++)
            {
                if (mouseClick.Y <= cellCentres[findCentreX, j].Y)
                    findCentreY = j;
            }

            //If in left half of board, the point is between the cells (findFile, findRank), (findFile, findRank+1), and (findFile+1, findRank+1)
            //If in right half of board, the point is between the cells (findFile, findRank), (findFile, findRank+1), and (findFile+1, findRank)
            //Calculate which cell midpoint is closest to the mouse click to determine which cell was clicked.
            int[] clickedScreenCell = [findCentreX, findCentreY];
            double closestDistance = GetDistance(mouseClick, cellCentres[findCentreX, findCentreY]);
            if ((findCentreY + 1) < cellCentres.GetLength(1)
                && GetDistance(mouseClick, cellCentres[findCentreX, findCentreY + 1]) < closestDistance)
            {
                closestDistance = GetDistance(mouseClick, cellCentres[findCentreX, findCentreY + 1]);
                clickedScreenCell = [findCentreX, findCentreY + 1];
            }
            if (findCentreX < hexBoard.BoardSize
                && (findCentreX + 1) < cellCentres.GetLength(0)
                && (findCentreY + 1) < cellCentres.GetLength(1)
                && GetDistance(mouseClick, cellCentres[findCentreX + 1, findCentreY + 1]) < closestDistance)
            {
                clickedScreenCell = [findCentreX + 1, findCentreY + 1];
            }
            else if ((findCentreX + 1) < cellCentres.GetLength(0)
                && GetDistance(mouseClick, cellCentres[findCentreX + 1, findCentreY]) < closestDistance)
            {
                clickedScreenCell = [findCentreX + 1, findCentreY];
            }
            //Check if the clicked cell is a valid cell on the board.
            int[] clickedBoardCell = [clickedScreenCell[0] - 1, clickedScreenCell[1] - 1];
            if (!hexBoard.ValidateCell(clickedBoardCell))
            {
                return;
            }

            //If a cell is already selected and it was clicked, unselect it.
            if (cellSelected != null && clickedBoardCell[0] == cellSelected[0] && clickedBoardCell[1] == cellSelected[1])
            {
                cellSelected = null;
                cellsAvailable = hexBoard.GetValidPieces();
                DisplayStrips();
                //Redraw the screen.
                BoardPanel.Invalidate();
            }
            //Check if the clicked cell is highlighted.
            else if (cellsAvailable != null)
            {
                for (int i = 0; i < cellsAvailable.Length; i++)
                {
                    if (cellsAvailable[i] != null && clickedBoardCell[0] == cellsAvailable[i][0] && clickedBoardCell[1] == cellsAvailable[i][1])
                    {
                        if (cellSelected != null)
                        {
                            //Make the selected move.
                            try
                            {
                                //If the move will result in a pawn promotion, ask the user what they want to promote to.
                                if (hexBoard.CheckPromotion(cellSelected[0], cellSelected[1], clickedBoardCell[0], clickedBoardCell[1]))
                                {
                                    //Show the dialog to allow the player to choose the piece.
                                    PromotionDialog promotionDialog = new();
                                    //Cancel the move if the user closed the screen without choosing.
                                    if (promotionDialog.ShowDialog() != DialogResult.OK
                                        || promotionDialog.PromotionType == ChessPiece.Type.None)
                                    {
                                        return;
                                    }
                                    //Promote the pawn to the selected piece.
                                    hexBoard.MovePiece(cellSelected[0],
                                               cellSelected[1],
                                               clickedBoardCell[0],
                                               clickedBoardCell[1],
                                               promotionDialog.PromotionType);
                                }
                                else
                                {
                                    //Move the piece.
                                    hexBoard.MovePiece(cellSelected[0],
                                               cellSelected[1],
                                               clickedBoardCell[0],
                                               clickedBoardCell[1]);
                                }

                                //Deselect the cell currently selected cell and calculate the new cells to highlight.
                                gameModified = true;
                                cellSelected = null;
                                cellsAvailable = hexBoard.GetValidPieces();
                            }
                            catch (InvalidOperationException)
                            {
                                return;
                            }

                        }
                        else
                        {
                            //Select the clicked cell and calculate the new cells to highlight.
                            cellSelected = clickedBoardCell;
                            cellsAvailable = hexBoard.GetValidMoves(clickedBoardCell[0], clickedBoardCell[1]);
                        }
                        //Redraw the screen.
                        DisplayStrips();
                        BoardPanel.Invalidate();
                        break;
                    }
                }
            }
        }

        //Calculate the geometry of the board.
        private void CalculateBoardPoints()
        {
            //Calculate various constants for drawing the board.
            double hexSide = 1 / Math.Sqrt(3);
            Point boardCentre = new(BoardPanel.Width / 2, BoardPanel.Height / 2);
            cellHeight = (int)Math.Min(
                (BoardPanel.Height - GameMenu.Height - GameStatus.Height) / (2 * hexBoard.BoardSize + 1),
                BoardPanel.Width / ((3 * hexBoard.BoardSize + 1) * hexSide));
            int cellSide = (int)(cellHeight * hexSide);
            cellCircle = (int)(cellHeight * 0.9);
            fileRankLabel = new Font(FontFamily.GenericSansSerif, Math.Max(1, (cellSide / 6)), FontStyle.Bold);
            //Calculate the centre of the origin point, cell A1.
            Point originCentre = new(
                        boardCentre.X - (int)((hexBoard.BoardSize - 1) * 1.5d * cellSide),
                        boardCentre.Y + (int)((hexBoard.BoardSize - 1) * 0.5d * cellHeight));
            //Initialize arrays for cell centres and for cell vertices.
            //Array is 2 larger than the board in order to store data to display the border.
            cellCentres = new Point[hexBoard.BoardMax + 2, hexBoard.BoardMax + 2];
            cellPoints = new Point[hexBoard.BoardMax + 2, hexBoard.BoardMax + 2][];

            //Calculate the centres of each cell and its vertices.
            int file;
            int rank;
            for (int j = 0; j < cellCentres.GetLength(1); j++)
                for (int i = 0; i < cellCentres.GetLength(0); i++)
                {
                    rank = j - 1;
                    file = i - 1;
                    //Calculate the centre point for the cell.
                    cellCentres[i, j] = new(
                    originCentre.X + (int)(cellSide * 1.5d * file),
                    originCentre.Y - (cellHeight * rank) + (int)(cellHeight * 0.5d * (-Math.Abs(file - hexBoard.BoardSize + 1) + hexBoard.BoardSize - 1)));

                    //Only process cells that are on the board or in the 1 cell border.
                    if ((i < hexBoard.BoardSize && j <= i + hexBoard.BoardSize) || (i >= hexBoard.BoardSize && j <= (3 * (hexBoard.BoardSize) - i)))
                    {
                        cellPoints[i, j] = new Point[6];
                        //Use any points from existing adjacent cells to remove gaps due to rounding.
                        //Cell uses existing points from previous cell in same rank.
                        if (i > 0 && cellPoints[i - 1, j] != null)
                        {
                            if (file < hexBoard.BoardSize)
                            {
                                cellPoints[i, j][4] = cellPoints[i - 1, j][2];
                                cellPoints[i, j][5] = cellPoints[i - 1, j][1];
                            }
                            else
                            {
                                cellPoints[i, j][3] = cellPoints[i - 1, j][1];
                                cellPoints[i, j][4] = cellPoints[i - 1, j][0];
                            }
                        }
                        //Cell uses existing points from the cell below it.
                        if (j > 0 && cellPoints[i, j - 1] != null)
                        {
                            cellPoints[i, j][2] = cellPoints[i, j - 1][0];
                            cellPoints[i, j][3] = cellPoints[i, j - 1][5];
                            //Cells in right half of board use existing point below and to the right of it.
                            if (file >= (hexBoard.BoardSize - 1) && (i + 1) < cellPoints.GetLength(0) && cellPoints[i + 1, j - 1] != null)
                                cellPoints[i, j][1] = cellPoints[i + 1, j - 1][5];
                        }
                        //Cells on the top left edge of the board use existing point from cell below and to the left of it.
                        if (i > 0 && j > 0 && (j - i) == hexBoard.BoardSize && cellPoints[i - 1, j - 1] != null)
                            cellPoints[i, j][4] = cellPoints[i - 1, j - 1][0];

                        //Create the hexagon for this cell, passing any existing points.
                        cellPoints[i, j] = CreateHexagon(cellCentres[i, j], cellHeight, cellPoints[i, j]);
                    }
                }

            //Calculate the background hexagon
            backgroundHex =
            [
                cellCentres[hexBoard.BoardSize, hexBoard.BoardMax + 1],
                cellCentres[hexBoard.BoardMax + 1, hexBoard.BoardSize],
                cellCentres[hexBoard.BoardMax + 1, 0],
                cellCentres[hexBoard.BoardSize, 0],
                cellCentres[0, 0],
                cellCentres[0, hexBoard.BoardSize],
            ];
        }
        //Draw the board and chess pieces.
        private void DrawHexBoard(Graphics Panel)
        {
            //Clear the drawing area and draw the background hexagon
            Panel.Clear(GameColours[(int)ColourIds.Background]);
            Panel.FillPolygon(borderHexagon, backgroundHex);

            //Loop through all cells in the display array.
            for (int i = 0; i < cellPoints.GetLength(1); i++)
                for (int j = 0; j < cellPoints.GetLength(0); j++)
                {
                    //Calculate file and rank. The display array has 2 extra columns and rows to calculate the border.
                    int file = i - 1;
                    int rank = j - 1;
                    if (cellPoints[i, j] != null)
                    {
                        //Draw hex board cells.
                        if (hexBoard.ValidateCell(file, rank))
                        {
                            bool cellIsHighlighted = false;

                            if (cellSelected != null && file == cellSelected[0] && rank == cellSelected[1])
                            {
                                cellIsHighlighted = true;
                                Panel.FillPolygon(highlightSelected, cellPoints[i, j]);
                            }
                            if (!cellIsHighlighted && cellsAvailable != null)
                            {
                                foreach (int[] cell in cellsAvailable)
                                {
                                    if (cell != null && file == cell[0] && rank == cell[1])
                                    {
                                        cellIsHighlighted = true;
                                        Panel.FillPolygon(highlightAvailable, cellPoints[i, j]);
                                        break;
                                    }
                                }
                            }
                            //Determine what colour the cell is: Light/Mid/Dark
                            switch ((rank - Math.Abs(file - hexBoard.BoardSize + 1) + hexBoard.BoardSize - 1) % 3)
                            {
                                case 0:
                                    if (cellIsHighlighted)
                                        Panel.FillEllipse(cellMid, cellCentres[i, j].X - (cellCircle / 2), cellCentres[i, j].Y - (cellCircle / 2), cellCircle, cellCircle);
                                    else
                                        Panel.FillPolygon(cellMid, cellPoints[i, j]);
                                    break;
                                case 1:
                                    if (cellIsHighlighted)
                                        Panel.FillEllipse(cellDark, cellCentres[i, j].X - (cellCircle / 2), cellCentres[i, j].Y - (cellCircle / 2), cellCircle, cellCircle);
                                    else
                                        Panel.FillPolygon(cellDark, cellPoints[i, j]);
                                    break;
                                case 2:
                                    if (cellIsHighlighted)
                                        Panel.FillEllipse(cellLight, cellCentres[i, j].X - (cellCircle / 2), cellCentres[i, j].Y - (cellCircle / 2), cellCircle, cellCircle);
                                    else
                                        Panel.FillPolygon(cellLight, cellPoints[i, j]);
                                    break;
                                default:
                                    break;
                            }

                            //Draw the piece
                            Rectangle drawPieceRectangle = new(cellCentres[i, j].X - (cellCircle / 2), cellCentres[i, j].Y - (cellCircle / 2), cellCircle, cellCircle);
                            ChessPiece drawPiece = hexBoard.BoardLayout[file, rank];
                            if (drawPiece.PieceOwner != HexBoard.Player.None && drawPiece.PieceType != ChessPiece.Type.None)
                            {
                                //Image with chess piece icons has pieces left to right in order, each one within a square equal to the height of the image.
                                Panel.DrawImage(chessPieceIcons,
                                                     drawPieceRectangle,
                                                     ((int)(drawPiece.PieceType) - 1) * chessPieceIcons.Height,
                                                     0,
                                                     chessPieceIcons.Height,
                                                     chessPieceIcons.Height,
                                                     GraphicsUnit.Pixel,
                                                     imageAttributesPlayers[(int)drawPiece.PieceOwner]);
                            }
                        }
                    }
                    //Draw the File and Rank labels
                    if (j == 0 && file >= 0 && file < hexBoard.BoardMax)
                        Panel.DrawString(hexBoard.GetFile(file), fileRankLabel, textLabel, cellCentres[i, 0].X, cellPoints[i, 0][0].Y, centreLabel);
                    if (rank >= 0 && (((j - i) == hexBoard.BoardSize && rank < hexBoard.BoardMax) || (i == 0 && rank < hexBoard.BoardSize)))
                        Panel.DrawString(hexBoard.GetRank(rank), fileRankLabel, textLabel, cellPoints[i, j][0].X, cellCentres[i, j].Y, leftLabel);
                    if (rank >= 0 && (((j + i) == 3 * hexBoard.BoardSize && rank < hexBoard.BoardMax) || (i == hexBoard.BoardMax + 1 && rank < hexBoard.BoardSize)))
                        Panel.DrawString(hexBoard.GetRank(rank), fileRankLabel, textLabel, cellPoints[i, j][5].X, cellCentres[i, j].Y, rightLabel);
                }
        }
        //Update menu and label tool strips
        private void DisplayStrips()
        {
            //Build the labels for the current player and the last move to display at the bottom of the screen.
            CurrentPlayerLabel.Text = " ";
            LastMoveLabel.Text = " ";
            if (hexBoard.MaxPlayers > 1)
            {
                CurrentPlayerLabel.Text = $"{hexBoard.CurrentPlayer}'s turn.";
                if (!String.IsNullOrWhiteSpace(hexBoard.Notation))
                    LastMoveLabel.Text = $"Previous Move: {hexBoard.MoveNumber}. {hexBoard.Notation}";
            }
            //Make the menu items unavailable if they cannot be used.
            if (!gameModified)
                UndoMoveToolStripMenuItem.Enabled = false;
            else
                UndoMoveToolStripMenuItem.Enabled = true;
            if (!gameModified)
                SaveToolStripMenuItem.Enabled = false;
            else
                SaveToolStripMenuItem.Enabled = true;
        }
        //Create a regular hexagon given the centre and height. Use any existing points given to prevent gaps in the board.
        private static Point[] CreateHexagon(Point hexCentre, int hexHeight, Point[] existingPoints)
        {
            //A regular hexagon of height H, has sides of length H/Sqrt3 and width 2H/Sqrt3.
            //Points are ordered starting with upper right and proceeding clockwise.
            double hexSide = 1 / Math.Sqrt(3);

            Point[] hexPoints = new Point[6];
            double[,] hexVectors = { { 0.5d, -0.5d }, { 1, 0 }, { 0.5d, 0.5d }, { -0.5d, 0.5d }, { -1, 0 }, { -0.5d, -0.5d } };

            for (int i = 0; i < hexPoints.Length; i++)
            {
                if (existingPoints.Length <= i || existingPoints[i].IsEmpty)
                    hexPoints[i] = new Point(
                        hexCentre.X + (int)(hexHeight * hexSide * hexVectors[i, 0]),
                        hexCentre.Y + (int)(hexHeight * hexVectors[i, 1]));
                else
                    hexPoints[i] = existingPoints[i];
            }
            return hexPoints;
        }
        // Calculate the distance between two points a and b.
        private static double GetDistance(Point a, Point b)
        {
            return Math.Sqrt(((a.X - b.X) * (a.X - b.X)) + ((a.Y - b.Y) * (a.Y - b.Y)));
        }
    }
}

