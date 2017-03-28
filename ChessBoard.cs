using System;
using SharpCanvas;

namespace ChessAI{
    public class ChessBoard{
        public ChessSquare[,] board;
        public Vector2i whiteKingPos;
        public Vector2i blackKingPos;
        public Vector2i lastMoveFrom;
        public Vector2i lastMoveTo;
        public bool fullCheckDetection = false;
        public bool whiteCheck = false;
        public bool blackCheck = false;

        public ChessBoard(){
            board = new ChessSquare[8, 8];
            whiteKingPos = new Vector2i(4, 0);
            blackKingPos = new Vector2i(4, 7);
        }

        public bool AttackedByBlack(int x, int y){
            for (int i = 0; i < 8; i++){
                for (int j = 0; j < 8; j++){
                    if(!(i == x && j == y) && ChessGame.board[i, j].OccupiedIsBlack){
                        if(ChessGame.board[i, j].piece.GetType() != typeof(King)){
                            Vector2i[] moves = ChessGame.board[i, j].piece.AvailableMoves();
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
                            Vector2i[] moves = ChessGame.board[i, j].piece.AvailableMoves();
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
            if(!fullCheckDetection){
                whiteCheck = false;
                blackCheck = false;
                Vector2i[] moves;
                if(p.isBlack){
                    moves = p.AvailableMoves();
                    for (int k = 0; k < moves.Length; k++){
                        if(moves[k].x == whiteKingPos.x && moves[k].y == whiteKingPos.y) whiteCheck = true;
                    }
                } else{
                    moves = p.AvailableMoves();
                    for (int k = 0; k < moves.Length; k++){
                        if(moves[k].x == blackKingPos.x && moves[k].y == blackKingPos.y) blackCheck = true;
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
                Console.WriteLine("1: " + ray1.x +", "+ray1.y + " 2: " + ray2.x +", "+ray2.y+ " 3: " + ray3.x +", "+ray3.y+ " 4: " + ray4.x +", "+ray4.y);
            } else{
                whiteCheck = AttackedByBlack(whiteKingPos.x, whiteKingPos.y);
                blackCheck = AttackedByWhite(blackKingPos.x, blackKingPos.y);
            }
            if(whiteCheck) Console.WriteLine("Check on white.");
            else if(blackCheck) Console.WriteLine("Check on black.");
            fullCheckDetection = false;
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
                        Vector2i[] moves = rayP.AvailableMoves();
                        for (int k = 0; k < moves.Length; k++){
                            if(moves[k].x == whiteKingPos.x && moves[k].y == whiteKingPos.y){
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
                        Vector2i[] moves = rayP.AvailableMoves();
                        for (int k = 0; k < moves.Length; k++){
                            if(moves[k].x == blackKingPos.x && moves[k].y == blackKingPos.y){
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