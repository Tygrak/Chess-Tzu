using System;
using System.Collections.Generic;
using SharpCanvas;
using OpenTK.Input;
using OpenTK;

namespace ChessAI{
    public class Program{
        public static void Main(string[] args){
            ChessGame game = new ChessGame(1000, 1000, 100, 100); //Still missing rules: >>en passant<<
            game.canvas.NextFrame(game.Initialize);
        }
    }

    public class ChessGame{
        public static ChessGame current;
        public Canvas canvas;
        public static ChessBoard chessBoard;
        public static ChessSquare[,] board;
        public double timer = 1;
        public bool whitePlayer = true;
        public bool blackPlayer = true;
        public bool paused = false;
        public static float squareSize;
        public bool selected;
        public Vector2i lastSelected;
        public Vector2i[] available;
        bool debug = false;

        public ChessGame(int windowWidth, int windowHeight, int width, int height){
            current = this;
            canvas = new Canvas(windowWidth, windowHeight);
            chessBoard = new ChessBoard();
            board = chessBoard.board;
            squareSize = 100.0f / 8;
        }

        public void Initialize(){
            canvas.updateAction = Update;
            canvas.keyboardInterruptAction = KeyboardInterrupt;
            canvas.BackgroundColor = Colorb.DarkGray;
            canvas.UpdateWhenUnfocused = true;
            canvas.ObjectMode();
            canvas.mouseActions.Add(new MouseAction(MouseButton.Left, MouseClick));
            DrawBoard();
            SetupBoard();
            canvas.Draw(new Group(0, 0));
        }

        public void DrawBoard(){
            bool black = true;
            for (int i = 0; i < 8; i++){
                for (int j = 0; j < 8; j++){
                    Square s = new Square(i*squareSize, j*squareSize, i*squareSize+squareSize, j*squareSize+squareSize);
                    if(black){
                        s.color = new Colorb(111, 144, 90);
                    } else{
                        s.color = new Colorb(254, 254, 220);
                    }
                    canvas.Draw(s);
                    black = !black;
                }
                black = !black;
            }
        }

        public void SetupBoard(){
            for (int i = 0; i < 8; i++){
                board[i, 1].piece = new Pawn(i, 1, false);
                board[i, 6].piece = new Pawn(i, 6, true);
            }
            board[0, 0].piece = new Rook(0, 0, false);
            board[0, 7].piece = new Rook(0, 7, true);
            board[7, 0].piece = new Rook(7, 0, false);
            board[7, 7].piece = new Rook(7, 7, true);
            board[1, 0].piece = new Knight(1, 0, false);
            board[1, 7].piece = new Knight(1, 7, true);
            board[6, 0].piece = new Knight(6, 0, false);
            board[6, 7].piece = new Knight(6, 7, true);
            board[2, 0].piece = new Bishop(2, 0, false);
            board[2, 7].piece = new Bishop(2, 7, true);
            board[5, 0].piece = new Bishop(5, 0, false);
            board[5, 7].piece = new Bishop(5, 7, true);
            board[3, 0].piece = new Queen(3, 0, false);
            board[3, 7].piece = new Queen(3, 7, true);
            board[4, 0].piece = new King(4, 0, false);
            board[4, 7].piece = new King(4, 7, true);
        }

        public void KeyboardInterrupt(){
            if((debug || paused) && canvas.LastPressedChar == 'r'){
                chessBoard = new ChessBoard();
                board = chessBoard.board;
                canvas.ClearObjects();
                DrawBoard();
                SetupBoard();
                canvas.Draw(new Group(0, 0));
                paused = false;
            }
            if(canvas.LastPressedChar == ' '){
                paused = !paused;
            }
        }

        public void Update(){
            double delta = canvas.DeltaTime;
            timer -= delta;
            if(timer<0 && !paused){
                timer = 1;
            }
        }

        public void MouseClick(){
            if(paused){
                return;
            }
            Vector2i mousePos = canvas.MousePosition();
            Vector2d pos = canvas.ScreenToWorldPosition(mousePos);
            int x = (int) Math.Floor(pos.X/squareSize);
            int y = (int) Math.Floor(pos.Y/squareSize);
            if(x<0 || x>7 || y<0 || y>7){
                return;
            }
            if(selected == false){
                if(board[x, y].Occupied && ((board[x, y].piece.isBlack && !chessBoard.whiteTurn && blackPlayer) || (!board[x, y].piece.isBlack && chessBoard.whiteTurn && whitePlayer))){
                    selected = true;
                    lastSelected = new Vector2i(x, y);
                    available = board[x, y].piece.NonPseudoAvailableMoves();
                    if(available.Length == 0){
                        selected = false;
                        return;
                    }
                    Group g = new Group(0, 0);
                    //Console.WriteLine(canvas.toDraw.Count + ", " + selected.objects.Count + ", " + available.Length);
                    for (int i = 0; i < available.Length; i++){
                        Circle c = new Circle(available[i].x*squareSize+squareSize/2, available[i].y*squareSize+squareSize/2, 2);
                        c.color = Colorb.Red;
                        g.Add(c);
                    }
                    canvas.toDraw.RemoveAt(canvas.toDraw.Count-1);
                    canvas.Draw(g);
                }
            } else if(available != null && new List<Vector2i>(available).Contains(new Vector2i(x, y))){
                chessBoard.lastMoveFrom = lastSelected;
                chessBoard.lastMoveTo = new Vector2i(x, y);
                board[lastSelected.x, lastSelected.y].piece.Move(x, y);
                selected = false;
                Group g = new Group(0, 0);
                canvas.toDraw.RemoveAt(canvas.toDraw.Count-1);
                canvas.Draw(g);
                if(!debug){
                    chessBoard.EndTurn();
                }
            } else{
                selected = false;
                Group g = new Group(0, 0);
                canvas.toDraw.RemoveAt(canvas.toDraw.Count-1);
                canvas.Draw(g);
            }
        }
    }
}