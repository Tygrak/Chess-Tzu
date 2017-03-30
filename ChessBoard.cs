using System;
using System.Collections.Generic;
using SharpCanvas;

namespace ChessAI{
    public class ChessBoard{
        public ChessSquare[,] board;
        public List<Vector2i> checking;
        public Vector2i whiteKingPos;
        public Vector2i blackKingPos;
        public Vector2i lastMoveFrom;
        public Vector2i lastMoveTo;
        public bool whiteTurn = true;
        public bool fullCheckDetection = false;
        public bool whiteCheck = false;
        public bool blackCheck = false;

        public ChessBoard(){
            board = new ChessSquare[8, 8];
            for (int i = 0; i < 8; i++){
                for (int j = 0; j < 8; j++){
                    board[i, j] = new ChessSquare();
                }
            }
            checking = new List<Vector2i>();
            whiteKingPos = new Vector2i(4, 0);
            blackKingPos = new Vector2i(4, 7);
        }

        public ChessBoard(ChessBoard cb){
            this.board = new ChessSquare[8, 8];
            for (int i = 0; i < 8; i++){
                for (int j = 0; j < 8; j++){
                    this.board[i, j] = new ChessSquare();
                }
            }
            this.checking = new List<Vector2i>(cb.checking);
            this.whiteKingPos = cb.whiteKingPos;
            this.blackKingPos = cb.blackKingPos;
            this.whiteCheck = cb.whiteCheck;
            this.blackCheck = cb.blackCheck;
            for (int i = 0; i < 8; i++){
                for (int j = 0; j < 8; j++){
                    if(cb.board[i, j].piece != null){
                        this.board[i, j].piece = cb.board[i, j].piece.Clone();
                    }
                }
            }
        }

        public bool AttackedByBlack(int x, int y){
            for (int i = 0; i < 8; i++){
                for (int j = 0; j < 8; j++){
                    if(!(i == x && j == y) && ChessGame.board[i, j].OccupiedIsBlack){
                        if(ChessGame.board[i, j].piece.GetType() != typeof(King)){
                            Vector2i[] moves = ChessGame.board[i, j].piece.AvailableMoves(this);
                            for (int k = 0; k < moves.Length; k++){
                                if(moves[k].x == x && moves[k].y == y) return true;
                            }
                        } else{
                            if((x == i+1 || x == i-1) && (y == j || y == j+1 || y == j-1) || y == j+1 || y == j-1) return true;
                        }
                    }
                }
            }
            return false;
        }

        public bool AttackedByWhite(int x, int y){
            for (int i = 0; i < 8; i++){
                for (int j = 0; j < 8; j++){
                    if(!(i == x && j == y) && ChessGame.board[i, j].OccupiedIsWhite){
                        if(ChessGame.board[i, j].piece.GetType() != typeof(King)){
                            Vector2i[] moves = ChessGame.board[i, j].piece.AvailableMoves(this);
                            for (int k = 0; k < moves.Length; k++){
                                if(moves[k].x == x && moves[k].y == y) return true;
                            }
                        } else{
                            if((x == i+1 || x == i-1) && (y == j || y == j+1 || y == j-1) || y == j+1 || y == j-1) return true;
                        }
                    }
                }
            }
            return false;
        }

        public void CheckDetect(){
            Piece p = board[lastMoveTo.x, lastMoveTo.y].piece;
            if(p == null){
                Console.WriteLine("Check Detect p is null");
                return;
            }
            if(!fullCheckDetection){
                whiteCheck = false;
                blackCheck = false;
                Vector2i[] moves;
                bool checkingIsBlack = false;
                for (int i = 0; i < checking.Count; i++){
                    if(board[checking[checking.Count-1].x, checking[checking.Count-1].y].piece != null){
                        checkingIsBlack = board[checking[checking.Count-1].x, checking[checking.Count-1].y].piece.isBlack;
                        break;
                    }
                }
                Console.WriteLine("Pieces checking: " + checking.Count + ", black? " + checkingIsBlack);
                for (int i = checking.Count-1; i >= 0; i--){
                    if(board[checking[i].x, checking[i].y].piece != null){
                        int cx = checking[i].x;
                        int cy = checking[i].y;
                        checking.RemoveAt(i);
                        if(checkingIsBlack){
                            moves = board[cx, cy].piece.AvailableMoves(this);
                            for (int k = 0; k < moves.Length; k++){
                                if(moves[k].x == whiteKingPos.x && moves[k].y == whiteKingPos.y){
                                    checking.Add(new Vector2i(cx, cy));
                                    whiteCheck = true;
                                }
                            }
                        } else{
                            moves = board[cx, cy].piece.AvailableMoves(this);
                            for (int k = 0; k < moves.Length; k++){
                                if(moves[k].x == blackKingPos.x && moves[k].y == blackKingPos.y){
                                    checking.Add(new Vector2i(cx, cy));
                                    blackCheck = true;
                                }
                            }
                        }
                    } else{
                        checking.RemoveAt(i);
                    }
                }           
                if(p.GetType() == typeof(Knight)){
                    if(p.isBlack){
                        moves = p.AvailableMoves(this);
                        for (int k = 0; k < moves.Length; k++){
                            if(moves[k].x == whiteKingPos.x && moves[k].y == whiteKingPos.y){
                                whiteCheck = true;
                                checking.Add(new Vector2i(p.x, p.y));
                            }
                        }
                    } else{
                        moves = p.AvailableMoves(this);
                        for (int k = 0; k < moves.Length; k++){
                            if(moves[k].x == blackKingPos.x && moves[k].y == blackKingPos.y){
                                blackCheck = true;
                                checking.Add(new Vector2i(p.x, p.y));
                            }
                        }
                    }
                }              
                Vector2i ray1 = new Vector2i(p.x - whiteKingPos.x, p.y - whiteKingPos.y);
                Vector2i ray2 = new Vector2i(p.x - blackKingPos.x, p.y - blackKingPos.y);
                Vector2i ray3 = new Vector2i(lastMoveFrom.x - whiteKingPos.x, lastMoveFrom.y - whiteKingPos.y);
                Vector2i ray4 = new Vector2i(lastMoveFrom.x - blackKingPos.x, lastMoveFrom.y - blackKingPos.y);
                if(checkRayWhite(ray1)) whiteCheck = true;
                if(checkRayWhite(ray3)) whiteCheck = true;
                if(checkRayBlack(ray2)) blackCheck = true;
                if(checkRayBlack(ray4)) blackCheck = true;
                //Console.WriteLine("1: " + ray1.x +", "+ray1.y + " 2: " + ray2.x +", "+ray2.y+ " 3: " + ray3.x +", "+ray3.y+ " 4: " + ray4.x +", "+ray4.y);
            } else{
                if(p.isBlack){
                    int x = blackKingPos.x;
                    int y = blackKingPos.y;
                    for (int i = 0; i < 8; i++){
                        for (int j = 0; j < 8; j++){
                            if(ChessGame.board[i, j].OccupiedIsWhite){
                                if(ChessGame.board[i, j].piece.GetType() != typeof(King)){
                                    Vector2i[] moves = ChessGame.board[i, j].piece.AvailableMoves(this);
                                    for (int k = 0; k < moves.Length; k++){
                                        if(moves[k].x == x && moves[k].y == y){
                                            checking.Add(new Vector2i(i, j));
                                            blackCheck = true;
                                            break;
                                        }
                                    }
                                } else{
                                    if((x == i+1 || x == i-1) && (y == j || y == j+1 || y == j-1) || y == j+1 || y == j-1){
                                        checking.Add(new Vector2i(i, j));
                                        blackCheck = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                } else{
                    int x = whiteKingPos.x;
                    int y = whiteKingPos.y;
                    for (int i = 0; i < 8; i++){
                        for (int j = 0; j < 8; j++){
                            if(ChessGame.board[i, j].OccupiedIsBlack){
                                if(ChessGame.board[i, j].piece.GetType() != typeof(King)){
                                    Vector2i[] moves = ChessGame.board[i, j].piece.AvailableMoves(this);
                                    for (int k = 0; k < moves.Length; k++){
                                        if(moves[k].x == x && moves[k].y == y){
                                            checking.Add(new Vector2i(i, j));
                                            whiteCheck = true;
                                            break;
                                        }
                                    }
                                } else{
                                    if((x == i+1 || x == i-1) && (y == j || y == j+1 || y == j-1) || y == j+1 || y == j-1){
                                        checking.Add(new Vector2i(i, j));
                                        whiteCheck = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                if(!whiteCheck && !blackCheck) fullCheckDetection = false;
            }
            //if(whiteCheck) Console.WriteLine("Check on white.");
            //else if(blackCheck) Console.WriteLine("Check on black.");
        }

        public bool checkRayWhite(Vector2i ray){
            if(ray.x == ray.y || ray.x == -ray.y || -ray.x == ray.y || -ray.x == -ray.y || ray.x == 0 || ray.y == 0){
                ray.x = Math.Sign(ray.x);
                ray.y = Math.Sign(ray.y);
                int x = whiteKingPos.x + ray.x;
                int y = whiteKingPos.y + ray.y;
                while(x>=0 && y>=0 && x<8 && y<8){
                    Piece rayP = board[x, y].piece;
                    if(rayP != null){
                        Vector2i[] moves = rayP.AvailableMoves(this);
                        //Console.WriteLine("RayP: " + rayP.x + ", " + rayP.y + " : " + rayP.GetType() + " moves: " + moves.Length);
                        for (int k = 0; k < moves.Length; k++){
                            if(moves[k].x == whiteKingPos.x && moves[k].y == whiteKingPos.y){
                                checking.Add(new Vector2i(rayP.x, rayP.y));
                                return true;
                            }
                        }
                        break;
                    }
                    x += ray.x;
                    y += ray.y;
                }
            }
            return false;
        }

        public bool checkRayBlack(Vector2i ray){
            if(ray.x == ray.y || ray.x == -ray.y || -ray.x == ray.y || -ray.x == -ray.y || ray.x == 0 || ray.y == 0){
                ray.x = Math.Sign(ray.x);
                ray.y = Math.Sign(ray.y);
                int x = blackKingPos.x + ray.x;
                int y = blackKingPos.y + ray.y;
                while(x>=0 && y>=0 && x<8 && y<8){
                    Piece rayP = board[x, y].piece;
                    if(rayP != null){
                        Vector2i[] moves = rayP.AvailableMoves(this);
                        for (int k = 0; k < moves.Length; k++){
                            if(moves[k].x == blackKingPos.x && moves[k].y == blackKingPos.y){
                                checking.Add(new Vector2i(rayP.x, rayP.y));
                                return true;
                            }
                        }
                        break;
                    }
                    x += ray.x;
                    y += ray.y;
                }
            }
            return false;
        }

        public bool resultsInCheck(int fromX, int fromY, int toX, int toY){
            ChessBoard clone = new ChessBoard(this);
            clone.board[fromX, fromY].piece.Move(toX, toY, clone);
            clone.lastMoveFrom = new Vector2i(fromX, fromY);
            clone.lastMoveTo = new Vector2i(toX, toY);
            clone.CheckDetect();
            //Console.WriteLine(clone.checking.Count + ", W:" + clone.whiteCheck + ", B:" + clone.blackCheck + ", x: " + toX + ", y: " + toY + " toCheck Black?: " + board[fromX, fromY].piece.isBlack);
            if(board[fromX, fromY].piece.isBlack){
                return clone.blackCheck;
            } else{
                return clone.whiteCheck;
            }
        }

        public bool CheckmateDetect(){
            if(whiteCheck && whiteTurn){
                Console.WriteLine("Checking white.");
                for (int i = 0; i < 8; i++){
                    for (int j = 0; j < 8; j++){
                        if(ChessGame.board[i, j].OccupiedIsWhite){
                            if(ChessGame.board[i, j].piece.NonPseudoAvailableMoves().Length > 0) return false;
                        }
                    }
                }
                Console.WriteLine("Checkmate! Black wins!");
                return true;
            } else if(blackCheck && !whiteTurn){
                for (int i = 0; i < 8; i++){
                    for (int j = 0; j < 8; j++){
                        if(ChessGame.board[i, j].OccupiedIsBlack){
                            if(ChessGame.board[i, j].piece.NonPseudoAvailableMoves().Length > 0) return false;
                        }
                    }
                }
                Console.WriteLine("Checkmate! White wins!");
                return true;
            }
            return false;
        }

        public void EndTurn(){
            ChessGame.current.calculating = true;
            CheckDetect();
            whiteTurn = !whiteTurn;
            if(whiteTurn){
                Console.WriteLine("White turn.");
            } else{
                Console.WriteLine("Black turn.");
            }
            if(CheckmateDetect()){
                Console.WriteLine("The game has ended.");
                ChessGame.current.paused = true;
                return;
            }
            if(whiteCheck) Console.WriteLine("Check on white.");
            else if(blackCheck) Console.WriteLine("Check on black.");
            ChessGame.current.calculating = false;
        }
    }

    public class ChessSquare{
        public Piece piece;

        public ChessSquare(){
            piece = null;
        }

        public ChessSquare(Piece piece){
            this.piece = piece;
        }

        public bool Occupied{
            get{return piece != null;}
        }

        public bool OccupiedIsWhite{
            get{return piece != null && !piece.isBlack;}
        }

        public bool OccupiedIsBlack{
            get{return piece != null && piece.isBlack;}
        }
    }
}