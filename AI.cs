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
        public bool isBlack;
        public ChessBoard board;
        public List<Vector2i> checking;

        public AI(bool isBlack = true){
            board = new ChessBoard(ChessGame.chessBoard);
            this.isBlack = isBlack;
        }

        public static int EvaluateBoard(ChessBoard cb){
            int score = 0;
            for (int i = 0; i < 8; i++){
                for (int j = 0; j < 8; j++){
                    if(cb.board[i, j].Occupied){
                        if(!cb.board[i, j].piece.isBlack){
                            score += cb.board[i, j].piece.GetValue();
                            Vector2i[] moves = cb.board[i, j].piece.NonPseudoAvailableMoves(cb);
                            score += moves.Length;
                        } else{
                            score -= cb.board[i, j].piece.GetValue();
                            Vector2i[] moves = cb.board[i, j].piece.NonPseudoAvailableMoves(cb);
                            score -= moves.Length;
                        }
                    }
                }
            }
            return score;
        }

        public int AlphaBetaSearch2Ply(ChessBoard cb, Move m){
            int alpha = int.MinValue;
            int beta = int.MaxValue;
            board = new ChessBoard(cb);
            //Console.WriteLine(board.whiteTurn);
            board.board[m.xFrom, m.yFrom].piece.Move(m.xTo, m.yTo, board);
            board.EndTurn(true);
            //Console.WriteLine("Testing: " + m.xFrom + ", " + m.yFrom + " : " + m.xTo + ", " + m.yTo+ " Turn white?: " + board.whiteTurn);
            int bestId = 0;
            List<Move> availableMoves = new List<Move>();
            for (int i = 0; i < 8; i++){
                for (int j = 0; j < 8; j++){
                    if((board.board[i, j].OccupiedIsWhite && board.whiteTurn) || (board.board[i, j].OccupiedIsBlack && !board.whiteTurn)){
                        //Console.WriteLine((board.board[i, j].OccupiedIsBlack && !board.whiteTurn) + " : " + i + ", " + j);
                        Vector2i[] moves = board.board[i, j].piece.NonPseudoAvailableMoves(board);
                        for (int k = 0; k < moves.Length; k++){
                            availableMoves.Add(new Move(i, j, moves[k].x, moves[k].y));
                        }
                    }
                }
            }
            if(availableMoves.Count > 0){
                if(isBlack){
                    alpha = AlphaBetaMove(board, availableMoves[0], int.MaxValue, int.MinValue).x;
                } else{
                    beta = AlphaBetaMove(board, availableMoves[0], int.MaxValue, int.MinValue).y;
                }
            }
            for (int i = 1; i < availableMoves.Count; i++){
                //Console.WriteLine(m.xFrom + ", " + m.yFrom + " : " + m.xTo + ", " + m.yTo);
                if(isBlack){
                    int al = AlphaBetaMove(board, availableMoves[i], alpha, int.MinValue).x;
                    if(al < alpha && al != int.MaxValue){
                        alpha = al;
                        bestId = i;
                    }
                } else{
                    int be = AlphaBetaMove(board, availableMoves[i], int.MaxValue, beta).y;
                    if(be > beta && be != int.MinValue){
                        beta = be;
                        bestId = i;
                    }
                }
            }
            if(isBlack){
                //Console.WriteLine("Move: " + m.xFrom + ", " + m.yFrom + " : " + m.xTo + ", " + m.yTo + " value is " + alpha);
                return alpha;
            } else{
                //Console.WriteLine("Move: " + m.xFrom + ", " + m.yFrom + " : " + m.xTo + ", " + m.yTo + " value is " + beta);
                return beta;
            }
        }

        public Vector2i AlphaBetaMove(ChessBoard cb, Move m, int alphaCutoff, int betaCutoff){
            int alpha = int.MaxValue;
            int beta = int.MinValue;
            //Console.WriteLine(m.xFrom + ", " + m.yFrom + " : " + m.xTo + ", " + m.yTo + " betaCutoff: " + betaCutoff);
            ChessBoard board = new ChessBoard(cb);
            board.board[m.xFrom, m.yFrom].piece.Move(m.xTo, m.yTo, board);
            board.EndTurn(true);
            List<Move> availableMoves = new List<Move>();
            for (int i = 0; i < 8; i++){
                for (int j = 0; j < 8; j++){
                    if((board.board[i, j].OccupiedIsWhite && board.whiteTurn) || (board.board[i, j].OccupiedIsBlack && !board.whiteTurn)){
                        //Console.WriteLine((board.board[i, j].OccupiedIsBlack && !board.whiteTurn) + " : " + i + ", " + j);
                        Vector2i[] moves = board.board[i, j].piece.NonPseudoAvailableMoves(board);
                        for (int k = 0; k < moves.Length; k++){
                            availableMoves.Add(new Move(i, j, moves[k].x, moves[k].y));
                        }
                    }
                }
            }
            for (int i = 0; i < availableMoves.Count; i++){
                ChessBoard clone = new ChessBoard(board);
                clone.board[availableMoves[i].xFrom, availableMoves[i].yFrom].piece.Move(availableMoves[i].xTo, availableMoves[i].yTo, clone);
                int value = EvaluateBoard(clone);
                Console.WriteLine(availableMoves[i].xFrom + ", " + availableMoves[i].yFrom + " : " + availableMoves[i].xTo + ", " + availableMoves[i].yTo + " val: " + value);
                if(value>alphaCutoff) return new Vector2i(value, beta);
                if(value<betaCutoff) return new Vector2i(alpha, value);
                if(value>beta) beta = value;
                if(value<alpha) alpha = value;
            }
            //Console.WriteLine("alpha: " + alpha + ", beta: " + beta);
            return new Vector2i(alpha, beta);
        }

        public int SimpleMinMaxSearch(ChessBoard cb, Move m){
            int value = 0;
            ChessBoard board = new ChessBoard(cb);
            board.board[m.xFrom, m.yFrom].piece.Move(m.xTo, m.yTo, board);
            board.EndTurn(true);
            //Console.WriteLine("S1: " + value);
            value = GetBoardMinMax(board);
            //Console.WriteLine("S2: " + value);
            return value;
        }

        public int GetBoardMinMax(ChessBoard cb){
            int value = 0;
            ChessBoard board = new ChessBoard(cb);
            List<Move> availableMoves = new List<Move>();
            for (int i = 0; i < 8; i++){
                for (int j = 0; j < 8; j++){
                    if((board.board[i, j].OccupiedIsWhite && board.whiteTurn) || (board.board[i, j].OccupiedIsBlack && !board.whiteTurn)){
                        Console.WriteLine("B1.1: " + value);
                        Vector2i[] moves = board.board[i, j].piece.NonPseudoAvailableMoves(board);
                        Console.WriteLine("B1.2: " + value);
                        for (int k = 0; k < moves.Length; k++){
                            availableMoves.Add(new Move(i, j, moves[k].x, moves[k].y));
                        }
                    }
                }
            }
            for (int i = 0; i < availableMoves.Count; i++){
                //Console.WriteLine("B1.3: " + value + ", " + availableMoves.Count + ", i: " + i);
                ChessBoard clone = new ChessBoard(board);
                clone.board[availableMoves[i].xFrom, availableMoves[i].yFrom].piece.Move(availableMoves[i].xTo, availableMoves[i].yTo, clone);
                if(clone.whiteTurn){
                    value = Math.Max(value, EvaluateBoard(clone));
                } else{
                    value = Math.Min(value, EvaluateBoard(clone));
                }
            }
            return value;
        }

        public Move NextMove(ChessBoard cb){
            int bestValue = isBlack ? int.MaxValue : int.MinValue;
            Move bestMove = new Move(0, 0, 0, 0);
            for (int i = 0; i < 8; i++){
                for (int j = 0; j < 8; j++){
                    if((cb.board[i, j].OccupiedIsWhite && cb.whiteTurn) || (cb.board[i, j].OccupiedIsBlack && !cb.whiteTurn)){
                        Vector2i[] moves = cb.board[i, j].piece.NonPseudoAvailableMoves(cb);
                        for (int k = 0; k < moves.Length; k++){
                            Move m = new Move(i, j, moves[k].x, moves[k].y);
                            int val = SimpleMinMaxSearch(cb, m);
                            //Console.WriteLine("NM: " + val);
                            if((isBlack && val<bestValue) || (!isBlack && val>bestValue)){
                                bestValue = val;
                                bestMove = m;
                            }
                        }
                    }
                }
            }
            Console.WriteLine("Best move: " + bestMove.xFrom + ", " + bestMove.yFrom + " : " + bestMove.xTo + ", " + bestMove.yTo + ", value: " + bestValue);
            return bestMove;
        }

        public Move RandomMove(ChessBoard cb){
            Random r = new Random();
            List<Move> availableMoves = new List<Move>();
            for (int i = 0; i < 8; i++){
                for (int j = 0; j < 8; j++){
                    if((cb.board[i, j].OccupiedIsWhite && cb.whiteTurn) || (cb.board[i, j].OccupiedIsBlack && !cb.whiteTurn)){
                        Vector2i[] moves = cb.board[i, j].piece.NonPseudoAvailableMoves(cb);
                        for (int k = 0; k < moves.Length; k++){
                            availableMoves.Add(new Move(i, j, moves[k].x, moves[k].y));
                        }
                    }
                }
            }
            int rand = r.Next(0, availableMoves.Count);
            Console.WriteLine("M" + cb.movesFromStart + ": " + cb.lastMoveFrom.x + ", " + cb.lastMoveFrom.y + " : " + cb.lastMoveTo.x + ", " + cb.lastMoveTo.y + " val: " + EvaluateBoard(ChessGame.chessBoard) + " : " + availableMoves.Count + ", " + rand);
            if(availableMoves.Count == 0){
                return null;
            }
            return availableMoves[rand];
        }
    }
}