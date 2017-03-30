using System.Collections.Generic;
using System;
using SharpCanvas;

namespace ChessAI{
    public class Move{
        public int xFrom;
        public int yFrom;
        public int xTo;
        public int yTo;
        public int value = 0;
        public Move(int xFrom, int yFrom, int xTo, int yTo){
            this.xFrom = xFrom;
            this.yFrom = yFrom;
            this.xTo = xTo;
            this.yTo = yTo;
        }
    }

    public class AI{
        public ChessBoard board;
        public List<Vector2i> checking;

        public AI(){
        }

        public Move RandomMove(ChessBoard cb){
            Random r = new Random();
            List<Move> availableMoves = new List<Move>();
            for (int i = 0; i < 8; i++){
                for (int j = 0; j < 8; j++){
                    if((cb.board[i, j].OccupiedIsWhite && cb.whiteTurn) || (cb.board[i, j].OccupiedIsBlack && !cb.whiteTurn)){
                        if(cb.board[i, j].piece.GetType() != typeof(King)){
                            Vector2i[] moves = cb.board[i, j].piece.NonPseudoAvailableMoves(cb);
                            Console.WriteLine("moves ai: " + moves.Length);
                            for (int k = 0; k < moves.Length; k++){
                                availableMoves.Add(new Move(i, j, moves[k].x, moves[k].y));
                            }
                        }
                    }
                }
            }
            int rand = r.Next(0, availableMoves.Count);
            Console.WriteLine(availableMoves.Count + ", " + rand);
            if(availableMoves.Count == 0){
                return null;
            }
            return availableMoves[rand];
        }
    }
}