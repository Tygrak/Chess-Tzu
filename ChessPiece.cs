using System;
using SharpCanvas;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;

namespace ChessAI{
    public class Piece{
        public Group group;
        public int x;
        public int y;
        public bool isBlack;
 
        public virtual int GetValue(){
            return 1;
        }
        
        public virtual void Move(int x, int y){
            ChessGame.board[this.x, this.y].piece = null;
            group.MoveTo(ChessGame.squareSize*x, ChessGame.squareSize*y);
            ChessGame.chessBoard.lastMoveFrom = new Vector2i(this.x, this.y);
            this.x = x;
            this.y = y;
            if(ChessGame.board[x, y].piece != null){
                ChessGame.chessBoard.movesFromCapture = -1;
                ChessGame.current.canvas.RemoveDrawable(ChessGame.board[x, y].piece.group);
            }
            ChessGame.chessBoard.lastMoveTo = new Vector2i(x, y);
            ChessGame.board[x, y].piece = this;
        }

        public virtual void Move(int x, int y, ChessBoard cb){
            cb.board[this.x, this.y].piece = null;
            cb.lastMoveFrom = new Vector2i(this.x, this.y);
            this.x = x;
            this.y = y;
            if(ChessGame.board[x, y].piece != null){
                ChessGame.chessBoard.movesFromCapture = -1;
            }
            cb.lastMoveTo = new Vector2i(x, y);
            cb.board[x, y].piece = this;
        }

        public virtual Piece Clone(){
            return (Piece) this.MemberwiseClone();
        }

        public virtual Vector2i[] AvailableMoves(ChessBoard cb = null){
            return new Vector2i[0];
        }

        public virtual Vector2i[] NonPseudoAvailableMoves(ChessBoard cb = null){ //FIXME: This is ineffective AF!
            if(cb == null) cb = ChessGame.chessBoard;
            List<Vector2i> moves = new List<Vector2i>(AvailableMoves(cb));
            for (int i = moves.Count-1; i >= 0; i--){
                if(cb.resultsInCheck(x, y, moves[i].x, moves[i].y)) moves.RemoveAt(i);
            }
            return moves.ToArray();
        }
    }
    public class Pawn : Piece{
        public Pawn(int x, int y, bool isBlack){
            this.x = x;
            this.y = y;
            this.isBlack = isBlack;
            this.group = new Group(0, 0);
            Square s = new Square(3, 1.5f, 9.5f, 3);
            s.color = isBlack ? Colorb.Black : Colorb.White;
            group.Add(s);
            s = new Square(3, 1.5f, 9.5f, 3);
            s.setFillMode(PolygonMode.Line);
            s.color = !isBlack ? Colorb.Black : Colorb.White;
            group.Add(s);
            s = new Square(4.25f, 3, 8.25f, 6);
            s.color = isBlack ? Colorb.Black : Colorb.White;
            group.Add(s);
            s = new Square(4.25f, 3, 8.25f, 6);
            s.setFillMode(PolygonMode.Line);
            s.color = !isBlack ? Colorb.Black : Colorb.White;
            group.Add(s);
            Circle c = new Circle(6.25f, 8f, 1.75f);
            c.color = isBlack ? Colorb.Black : Colorb.White;
            group.Add(c);
            CircleUnfilled cu = new CircleUnfilled(6.25f, 8f, 1.75f);
            cu.color = !isBlack ? Colorb.Black : Colorb.White;
            group.Add(cu);
            s = new Square(3.5f, 6, 9f, 6.5f);
            s.color = isBlack ? Colorb.Black : Colorb.White;
            group.Add(s);
            s = new Square(3.5f, 6, 9f, 6.5f);
            s.setFillMode(PolygonMode.Line);
            s.color = !isBlack ? Colorb.Black : Colorb.White;
            group.Add(s);
            group.Move(ChessGame.squareSize*x, ChessGame.squareSize*y);
            ChessGame.current.canvas.Draw(group);
        }

        public override void Move(int x, int y){
            base.Move(x, y);
            if(isBlack && y == 0){
                ChessGame.current.canvas.RemoveDrawable(ChessGame.board[x, y].piece.group);
                ChessGame.board[x, y].piece = new Queen(x, y, true);
            } else if(!isBlack && y == 7){
                ChessGame.current.canvas.RemoveDrawable(ChessGame.board[x, y].piece.group);
                ChessGame.board[x, y].piece = new Queen(x, y, false);
            }
        }

        public override void Move(int x, int y, ChessBoard cb){
            base.Move(x, y, cb);
            if(isBlack && y == 0){
                cb.board[x, y].piece = new Queen(x, y, true, false);
            } else if(!isBlack && y == 7){
                cb.board[x, y].piece = new Queen(x, y, false, false);
            }
        }

        public override int GetValue(){
            return 1;
        }

        public override Vector2i[] AvailableMoves(ChessBoard cb = null){
            List<Vector2i> moves = new List<Vector2i>();
            if(cb == null) cb = ChessGame.chessBoard;
            if(isBlack){
                if(y-1>=0){
                    if(y == 6 && !cb.board[x, y-1].Occupied && !cb.board[x, y-2].Occupied) moves.Add(new Vector2i(x, y-2));
                    if(!cb.board[x, y-1].Occupied) moves.Add(new Vector2i(x, y-1));
                    if(x+1<8 && cb.board[x+1, y-1].OccupiedIsWhite) moves.Add(new Vector2i(x+1, y-1));
                    if(x-1>=0 && cb.board[x-1, y-1].OccupiedIsWhite) moves.Add(new Vector2i(x-1, y-1));
                }
            } else{
                if(y+1<8){
                    if(y == 1 && !cb.board[x, y+1].Occupied && !cb.board[x, y+2].Occupied) moves.Add(new Vector2i(x, y+2));
                    if(!cb.board[x, y+1].Occupied) moves.Add(new Vector2i(x, y+1));
                    if(x+1<8 && cb.board[x+1, y+1].OccupiedIsBlack) moves.Add(new Vector2i(x+1, y+1));
                    if(x-1>=0 && cb.board[x-1, y+1].OccupiedIsBlack) moves.Add(new Vector2i(x-1, y+1));
                }
            }
            return moves.ToArray();
        }
    }

    public class Rook : Piece{
        public bool moved = false;
        public Rook(int x, int y, bool isBlack){
            this.x = x;
            this.y = y;
            this.isBlack = isBlack;
            this.group = new Group(0, 0);
            Square s = new Square(3, 1.5f, 9.5f, 3);
            s.color = isBlack ? Colorb.Black : Colorb.White;
            group.Add(s);
            s = new Square(3, 1.5f, 9.5f, 3);
            s.setFillMode(PolygonMode.Line);
            s.color = !isBlack ? Colorb.Black : Colorb.White;
            group.Add(s);
            s = new Square(3.5f, 3f, 9f, 4.5f);
            s.color = isBlack ? Colorb.Black : Colorb.White;
            group.Add(s);
            s = new Square(3.5f, 3f, 9f, 4.5f);
            s.setFillMode(PolygonMode.Line);
            s.color = !isBlack ? Colorb.Black : Colorb.White;
            group.Add(s);
            s = new Square(4.25f, 4.5f, 8.25f, 7.5f);
            s.color = isBlack ? Colorb.Black : Colorb.White;
            group.Add(s);
            s = new Square(4.25f, 4.5f, 8.25f, 7.5f);
            s.setFillMode(PolygonMode.Line);
            s.color = !isBlack ? Colorb.Black : Colorb.White;
            group.Add(s);
            s = new Square(3.5f, 7.5f, 9f, 8.25f);
            s.color = isBlack ? Colorb.Black : Colorb.White;
            group.Add(s);
            s = new Square(3.5f, 7.5f, 9f, 8.25f);
            s.setFillMode(PolygonMode.Line);
            s.color = !isBlack ? Colorb.Black : Colorb.White;
            group.Add(s);
            s = new Square(3.5f, 8.25f, 4.6f, 9f);
            s.color = isBlack ? Colorb.Black : Colorb.White;
            group.Add(s);
            s = new Square(3.5f, 8.25f, 4.6f, 9f);
            s.setFillMode(PolygonMode.Line);
            s.color = !isBlack ? Colorb.Black : Colorb.White;
            group.Add(s);
            s = new Square(5.7f, 8.25f, 6.8f, 9f);
            s.color = isBlack ? Colorb.Black : Colorb.White;
            group.Add(s);
            s = new Square(5.7f, 8.25f, 6.8f, 9f);
            s.setFillMode(PolygonMode.Line);
            s.color = !isBlack ? Colorb.Black : Colorb.White;
            group.Add(s);
            s = new Square(7.9f, 8.25f, 9.0f, 9f);
            s.color = isBlack ? Colorb.Black : Colorb.White;
            group.Add(s);
            s = new Square(7.9f, 8.25f, 9.0f, 9f);
            s.setFillMode(PolygonMode.Line);
            s.color = !isBlack ? Colorb.Black : Colorb.White;
            group.Add(s);
            group.Move(ChessGame.squareSize*x, ChessGame.squareSize*y);
            ChessGame.current.canvas.Draw(group);
        }

        public override void Move(int x, int y){
            base.Move(x, y);
            moved = true;
        }

        public override void Move(int x, int y, ChessBoard cb){
            base.Move(x, y, cb);
            moved = true;
        }

        public override int GetValue(){
            return 5;
        }

        public override Vector2i[] AvailableMoves(ChessBoard cb = null){
            List<Vector2i> moves = new List<Vector2i>();
            if(cb == null) cb = ChessGame.chessBoard;
            for(int i = x+1; i<8; i++){
                if(!cb.board[i, y].Occupied){moves.Add(new Vector2i(i, y));}
                else if((cb.board[i, y].OccupiedIsBlack && !isBlack) || (cb.board[i, y].OccupiedIsWhite && isBlack)){moves.Add(new Vector2i(i, y)); break;}
                else {break;}
            }
            for(int i = x-1; i>=0; i--){
                if(!cb.board[i, y].Occupied){moves.Add(new Vector2i(i, y));}
                else if((cb.board[i, y].OccupiedIsBlack && !isBlack) || (cb.board[i, y].OccupiedIsWhite && isBlack)){moves.Add(new Vector2i(i, y)); break;}
                else {break;}
            }
            for(int i = y+1; i<8; i++){
                if(!cb.board[x, i].Occupied){moves.Add(new Vector2i(x, i));}
                else if((cb.board[x, i].OccupiedIsBlack && !isBlack) || (cb.board[x, i].OccupiedIsWhite && isBlack)){moves.Add(new Vector2i(x, i)); break;}
                else {break;}
            }
            for(int i = y-1; i>=0; i--){
                if(!cb.board[x, i].Occupied){moves.Add(new Vector2i(x, i));}
                else if((cb.board[x, i].OccupiedIsBlack && !isBlack) || (cb.board[x, i].OccupiedIsWhite && isBlack)){moves.Add(new Vector2i(x, i)); break;}
                else {break;}
            }
            return moves.ToArray();
        }
    }

    public class Knight : Piece{
        public Knight(int x, int y, bool isBlack){
            this.x = x;
            this.y = y;
            this.isBlack = isBlack;
            this.group = new Group(0, 0);
            PolygonConvex p = new PolygonConvex(new float[]{8, 9, 5.25f, 10, 2.0f, 7, 3.25f, 5, 5, 6, 4, 3, 9, 3});
            p.color = isBlack ? Colorb.Black : Colorb.White;
            group.Add(p);
            p = new PolygonConvex(new float[]{8, 9, 5.25f, 10, 2.0f, 7, 3.25f, 5, 5, 6, 4, 3, 9, 3});
            p.setFillMode(PolygonMode.Line);
            p.color = !isBlack ? Colorb.Black : Colorb.White;
            group.Add(p);
            /*PolygonUnfilled pu = new PolygonUnfilled(new float[]{8, 9, 5.25f, 10, 2.0f, 7, 3.25f, 5, 5, 6, 4, 3, 9, 3});
            pu.color = !isBlack ? Colorb.Black : Colorb.White;
            group.Add(pu);*/
            Square s = new Square(3f, 1.5f, 10f, 3);
            s.color = isBlack ? Colorb.Black : Colorb.White;
            group.Add(s);
            s = new Square(3f, 1.5f, 10f, 3);
            s.setFillMode(PolygonMode.Line);
            s.color = !isBlack ? Colorb.Black : Colorb.White;
            group.Add(s);
            group.Move(ChessGame.squareSize*x, ChessGame.squareSize*y);
            ChessGame.current.canvas.Draw(group);
        }

        public override int GetValue(){
            return 3;
        }

        public override Vector2i[] AvailableMoves(ChessBoard cb = null){
            List<Vector2i> moves = new List<Vector2i>();
            if(cb == null) cb = ChessGame.chessBoard;
            if(isBlack){
                if(x+1<8 && y+2<8 && !cb.board[x+1, y+2].OccupiedIsBlack) moves.Add(new Vector2i(x+1, y+2));
                if(x-1>=0 && y+2<8 && !cb.board[x-1, y+2].OccupiedIsBlack) moves.Add(new Vector2i(x-1, y+2));
                if(x+1<8 && y-2>=0 && !cb.board[x+1, y-2].OccupiedIsBlack) moves.Add(new Vector2i(x+1, y-2));
                if(x-1>=0 && y-2>=0 && !cb.board[x-1, y-2].OccupiedIsBlack) moves.Add(new Vector2i(x-1, y-2));
                if(y+1<8 && x+2<8 && !cb.board[x+2, y+1].OccupiedIsBlack) moves.Add(new Vector2i(x+2, y+1));
                if(y-1>=0 && x+2<8 && !cb.board[x+2, y-1].OccupiedIsBlack) moves.Add(new Vector2i(x+2, y-1));
                if(y+1<8 && x-2>=0 && !cb.board[x-2, y+1].OccupiedIsBlack) moves.Add(new Vector2i(x-2, y+1));
                if(y-1>0 && x-2>=0 && !cb.board[x-2, y-1].OccupiedIsBlack) moves.Add(new Vector2i(x-2, y-1));
            } else{
                if(x+1<8 && y+2<8 && !cb.board[x+1, y+2].OccupiedIsWhite) moves.Add(new Vector2i(x+1, y+2));
                if(x-1>=0 && y+2<8 && !cb.board[x-1, y+2].OccupiedIsWhite) moves.Add(new Vector2i(x-1, y+2));
                if(x+1<8 && y-2>=0 && !cb.board[x+1, y-2].OccupiedIsWhite) moves.Add(new Vector2i(x+1, y-2));
                if(x-1>=0 && y-2>=0 && !cb.board[x-1, y-2].OccupiedIsWhite) moves.Add(new Vector2i(x-1, y-2));
                if(y+1<8 && x+2<8 && !cb.board[x+2, y+1].OccupiedIsWhite) moves.Add(new Vector2i(x+2, y+1));
                if(y-1>=0 && x+2<8 && !cb.board[x+2, y-1].OccupiedIsWhite) moves.Add(new Vector2i(x+2, y-1));
                if(y+1<8 && x-2>=0 && !cb.board[x-2, y+1].OccupiedIsWhite) moves.Add(new Vector2i(x-2, y+1));
                if(y-1>=0 && x-2>=0 && !cb.board[x-2, y-1].OccupiedIsWhite) moves.Add(new Vector2i(x-2, y-1));
            }
            return moves.ToArray();
        }
    }

    public class Bishop : Piece{
        public Bishop(int x, int y, bool isBlack){
            this.x = x;
            this.y = y;
            this.isBlack = isBlack;
            this.group = new Group(0, 0);
            Square s = new Square(3, 1.5f, 9.5f, 3.25f);
            s.color = isBlack ? Colorb.Black : Colorb.White;
            group.Add(s);
            s = new Square(3, 1.5f, 9.5f, 3.25f);
            s.setFillMode(PolygonMode.Line);
            s.color = !isBlack ? Colorb.Black : Colorb.White;
            group.Add(s);
            s = new Square(4.25f, 3.25f, 8.25f, 4.5f);
            s.color = isBlack ? Colorb.Black : Colorb.White;
            group.Add(s);
            s = new Square(4.25f, 3.25f, 8.25f, 4.5f);
            s.setFillMode(PolygonMode.Line);
            s.color = !isBlack ? Colorb.Black : Colorb.White;
            group.Add(s);
            s = new Square(4.5f, 4.5f, 8.0f, 8f);
            s.color = isBlack ? Colorb.Black : Colorb.White;
            group.Add(s);
            s = new Square(4.5f, 4.5f, 8.0f, 8f);
            s.setFillMode(PolygonMode.Line);
            s.color = !isBlack ? Colorb.Black : Colorb.White;
            group.Add(s);
            Circle c = new Circle(6.25f, 7.5f, 2.25f);
            c.color = isBlack ? Colorb.Black : Colorb.White;
            group.Add(c);
            CircleUnfilled cu = new CircleUnfilled(6.25f, 7.5f, 2.25f);
            cu.color = !isBlack ? Colorb.Black : Colorb.White;
            group.Add(cu);
            c = new Circle(6.25f, 10.25f, 0.5f);
            c.color = isBlack ? Colorb.Black : Colorb.White;
            group.Add(c);
            cu = new CircleUnfilled(6.25f, 10.25f, 0.5f);
            cu.color = !isBlack ? Colorb.Black : Colorb.White;
            group.Add(cu);
            s = new Square(6.0f, 6.5f, 6.5f, 8.5f);
            s.color = !isBlack ? Colorb.Black : Colorb.White;
            group.Add(s);
            s = new Square(5.25f, 7.25f, 7.25f, 7.75f);
            s.color = !isBlack ? Colorb.Black : Colorb.White;
            group.Add(s);
            group.Move(ChessGame.squareSize*x, ChessGame.squareSize*y);
            ChessGame.current.canvas.Draw(group);
        }

        public override int GetValue(){
            return 3;
        }

        public override Vector2i[] AvailableMoves(ChessBoard cb = null){
            List<Vector2i> moves = new List<Vector2i>();
            if(cb == null) cb = ChessGame.chessBoard;
            int j = y+1;
            for(int i = x+1; i<8 && j<8; i++){
                if(!cb.board[i, j].Occupied){moves.Add(new Vector2i(i, j));}
                else if((cb.board[i, j].OccupiedIsBlack && !isBlack) || (cb.board[i, j].OccupiedIsWhite && isBlack)){moves.Add(new Vector2i(i, j)); break;}
                else {break;}
                j++;
            }
            j = y-1;
            for(int i = x+1; i<8 && j>=0; i++){
                if(!cb.board[i, j].Occupied){moves.Add(new Vector2i(i, j));}
                else if((cb.board[i, j].OccupiedIsBlack && !isBlack) || (cb.board[i, j].OccupiedIsWhite && isBlack)){moves.Add(new Vector2i(i, j)); break;}
                else {break;}
                j--;
            }
            j = y+1;
            for(int i = x-1; i>=0 && j<8; i--){
                if(!cb.board[i, j].Occupied){moves.Add(new Vector2i(i, j));}
                else if((cb.board[i, j].OccupiedIsBlack && !isBlack) || (cb.board[i, j].OccupiedIsWhite && isBlack)){moves.Add(new Vector2i(i, j)); break;}
                else {break;}
                j++;
            }
            j = y-1;
            for(int i = x-1; i>=0 && j>=0; i--){
                if(!cb.board[i, j].Occupied){moves.Add(new Vector2i(i, j));}
                else if((cb.board[i, j].OccupiedIsBlack && !isBlack) || (cb.board[i, j].OccupiedIsWhite && isBlack)){moves.Add(new Vector2i(i, j)); break;}
                else {break;}
                j--;
            }
            return moves.ToArray();
        }
    }

    public class Queen : Piece{
        public Queen(int x, int y, bool isBlack){
            this.x = x;
            this.y = y;
            this.isBlack = isBlack;
            this.group = new Group(0, 0);
            Square s = new Square(3, 1.5f, 9.5f, 2.75f);
            s.color = isBlack ? Colorb.Black : Colorb.White;
            group.Add(s);
            s = new Square(3, 1.5f, 9.5f, 2.75f);
            s.color = !isBlack ? Colorb.Black : Colorb.White;
            s.setFillMode(PolygonMode.Line);
            group.Add(s);
            s = new Square(3.5f, 2.75f, 9.0f, 3.5f);
            s.color = isBlack ? Colorb.Black : Colorb.White;
            group.Add(s);
            s = new Square(3.5f, 2.75f, 9.0f, 3.5f);
            s.setFillMode(PolygonMode.Line);
            s.color = !isBlack ? Colorb.Black : Colorb.White;
            group.Add(s);
            s = new Square(3.5f, 3.5f, 9.0f, 4.25f);
            s.color = isBlack ? Colorb.Black : Colorb.White;
            group.Add(s);
            s = new Square(3.5f, 3.5f, 9.0f, 4.25f);
            s.setFillMode(PolygonMode.Line);
            s.color = !isBlack ? Colorb.Black : Colorb.White;
            group.Add(s);
            s = new Square(3, 4.25f, 9.5f, 5.25f);
            s.color = isBlack ? Colorb.Black : Colorb.White;
            group.Add(s);
            s = new Square(3, 4.25f, 9.5f, 5.25f);
            s.color = !isBlack ? Colorb.Black : Colorb.White;
            s.setFillMode(PolygonMode.Line);
            group.Add(s);
            Triangle t = new Triangle(3f, 5.25f, 2.0f, 8.5f, 4.3f, 5.25f);
            t.color = isBlack ? Colorb.Black : Colorb.White;
            group.Add(t);
            t = new Triangle(3f, 5.25f, 2.0f, 8.5f, 4.3f, 5.25f);
            t.color = !isBlack ? Colorb.Black : Colorb.White;
            t.setFillMode(PolygonMode.Line);
            group.Add(t);
            t = new Triangle(4.3f, 5.25f, 4.15f, 9.25f, 5.6f, 5.25f);
            t.color = isBlack ? Colorb.Black : Colorb.White;
            group.Add(t);
            t = new Triangle(4.3f, 5.25f, 4.15f, 9.25f, 5.6f, 5.25f);
            t.color = !isBlack ? Colorb.Black : Colorb.White;
            t.setFillMode(PolygonMode.Line);
            group.Add(t);
            t = new Triangle(5.6f, 5.25f, 6.25f, 9.75f, 6.9f, 5.25f);
            t.color = isBlack ? Colorb.Black : Colorb.White;
            group.Add(t);
            t = new Triangle(5.6f, 5.25f, 6.25f, 9.75f, 6.9f, 5.25f);
            t.color = !isBlack ? Colorb.Black : Colorb.White;
            t.setFillMode(PolygonMode.Line);
            group.Add(t);
            t = new Triangle(6.9f, 5.25f, 8.35f, 9.25f, 8.2f, 5.25f);
            t.color = isBlack ? Colorb.Black : Colorb.White;
            group.Add(t);
            t = new Triangle(6.9f, 5.25f, 8.35f, 9.25f, 8.2f, 5.25f);
            t.color = !isBlack ? Colorb.Black : Colorb.White;
            t.setFillMode(PolygonMode.Line);
            group.Add(t);
            t = new Triangle(8.2f, 5.25f, 10.25f, 8.5f, 9.5f, 5.25f);
            t.color = isBlack ? Colorb.Black : Colorb.White;
            group.Add(t);
            t = new Triangle(8.2f, 5.25f, 10.25f, 8.5f, 9.5f, 5.25f);
            t.color = !isBlack ? Colorb.Black : Colorb.White;
            t.setFillMode(PolygonMode.Line);
            group.Add(t);
            group.Move(ChessGame.squareSize*x, ChessGame.squareSize*y);
            ChessGame.current.canvas.Draw(group);
        }

        public Queen(int x, int y, bool isBlack, bool draw){
            if(draw){
                Queen q = new Queen(x, y, isBlack);
                this.group = q.group;
            }
            this.x = x;
            this.y = y;
            this.isBlack = isBlack;
        }

        public override int GetValue(){
            return 9;
        }

        public override Vector2i[] AvailableMoves(ChessBoard cb = null){
            List<Vector2i> moves = new List<Vector2i>();
            if(cb == null) cb = ChessGame.chessBoard;
            for(int i = x+1; i<8; i++){
                if(!cb.board[i, y].Occupied){moves.Add(new Vector2i(i, y));}
                else if((cb.board[i, y].OccupiedIsBlack && !isBlack) || (cb.board[i, y].OccupiedIsWhite && isBlack)){moves.Add(new Vector2i(i, y)); break;}
                else {break;}
            }
            for(int i = x-1; i>=0; i--){
                if(!cb.board[i, y].Occupied){moves.Add(new Vector2i(i, y));}
                else if((cb.board[i, y].OccupiedIsBlack && !isBlack) || (cb.board[i, y].OccupiedIsWhite && isBlack)){moves.Add(new Vector2i(i, y)); break;}
                else {break;}
            }
            for(int i = y+1; i<8; i++){
                if(!cb.board[x, i].Occupied){moves.Add(new Vector2i(x, i));}
                else if((cb.board[x, i].OccupiedIsBlack && !isBlack) || (cb.board[x, i].OccupiedIsWhite && isBlack)){moves.Add(new Vector2i(x, i)); break;}
                else {break;}
            }
            for(int i = y-1; i>=0; i--){
                if(!cb.board[x, i].Occupied){moves.Add(new Vector2i(x, i));}
                else if((cb.board[x, i].OccupiedIsBlack && !isBlack) || (cb.board[x, i].OccupiedIsWhite && isBlack)){moves.Add(new Vector2i(x, i)); break;}
                else {break;}
            }
            int j = y+1;
            for(int i = x+1; i<8 && j<8; i++){
                if(!cb.board[i, j].Occupied){moves.Add(new Vector2i(i, j));}
                else if((cb.board[i, j].OccupiedIsBlack && !isBlack) || (cb.board[i, j].OccupiedIsWhite && isBlack)){moves.Add(new Vector2i(i, j)); break;}
                else {break;}
                j++;
            }
            j = y-1;
            for(int i = x+1; i<8 && j>=0; i++){
                if(!cb.board[i, j].Occupied){moves.Add(new Vector2i(i, j));}
                else if((cb.board[i, j].OccupiedIsBlack && !isBlack) || (cb.board[i, j].OccupiedIsWhite && isBlack)){moves.Add(new Vector2i(i, j)); break;}
                else {break;}
                j--;
            }
            j = y+1;
            for(int i = x-1; i>=0 && j<8; i--){
                if(!cb.board[i, j].Occupied){moves.Add(new Vector2i(i, j));}
                else if((cb.board[i, j].OccupiedIsBlack && !isBlack) || (cb.board[i, j].OccupiedIsWhite && isBlack)){moves.Add(new Vector2i(i, j)); break;}
                else {break;}
                j++;
            }
            j = y-1;
            for(int i = x-1; i>=0 && j>=0; i--){
                if(!cb.board[i, j].Occupied){moves.Add(new Vector2i(i, j));}
                else if((cb.board[i, j].OccupiedIsBlack && !isBlack) || (cb.board[i, j].OccupiedIsWhite && isBlack)){moves.Add(new Vector2i(i, j)); break;}
                else {break;}
                j--;
            }
            return moves.ToArray();
        }
    }

    public class King : Piece{
        public bool moved = false;
        public King(int x, int y, bool isBlack){
            this.x = x;
            this.y = y;
            this.isBlack = isBlack;
            this.group = new Group(0, 0);
            
            Square s = new Square(3, 1.5f, 9.5f, 2.75f);
            s.color = isBlack ? Colorb.Black : Colorb.White;
            group.Add(s);
            s = new Square(3, 1.5f, 9.5f, 2.75f);
            s.color = !isBlack ? Colorb.Black : Colorb.White;
            s.setFillMode(PolygonMode.Line);
            group.Add(s);
            s = new Square(6.0f, 6.25f, 6.5f, 9.5f);
            s.color = Colorb.Black;
            group.Add(s);
            s = new Square(5.25f, 8.0f, 7.25f, 8.5f);
            s.color = Colorb.Black;
            group.Add(s);
            Circle c = new Circle(4.5f, 5.8f, 1.8f);
            c.color = isBlack ? Colorb.Black : Colorb.White;
            group.Add(c);
            CircleUnfilled cu = new CircleUnfilled(4.5f, 5.8f, 1.8f);
            cu.color = !isBlack ? Colorb.Black : Colorb.White;
            group.Add(cu);
            c = new Circle(8.0f, 5.8f, 1.8f);
            c.color = isBlack ? Colorb.Black : Colorb.White;
            group.Add(c);
            cu = new CircleUnfilled(8.0f, 5.8f, 1.8f);
            cu.color = !isBlack ? Colorb.Black : Colorb.White;
            group.Add(cu);
            s = new Square(3.5f, 2.75f, 9.0f, 3.5f);
            s.color = isBlack ? Colorb.Black : Colorb.White;
            group.Add(s);
            s = new Square(3.5f, 2.75f, 9.0f, 3.5f);
            s.setFillMode(PolygonMode.Line);
            s.color = !isBlack ? Colorb.Black : Colorb.White;
            group.Add(s);
            s = new Square(3.5f, 3.5f, 9.0f, 4.25f);
            s.color = isBlack ? Colorb.Black : Colorb.White;
            group.Add(s);
            s = new Square(3.5f, 3.5f, 9.0f, 4.25f);
            s.setFillMode(PolygonMode.Line);
            s.color = !isBlack ? Colorb.Black : Colorb.White;
            group.Add(s);
            s = new Square(3, 4.25f, 9.5f, 5.25f);
            s.color = isBlack ? Colorb.Black : Colorb.White;
            group.Add(s);
            s = new Square(3, 4.25f, 9.5f, 5.25f);
            s.color = !isBlack ? Colorb.Black : Colorb.White;
            s.setFillMode(PolygonMode.Line);
            group.Add(s);
            group.Move(ChessGame.squareSize*x, ChessGame.squareSize*y);
            ChessGame.current.canvas.Draw(group);
        }

        public override void Move(int x, int y){
            if(!moved && x == 2 && y == 7){
                ChessGame.board[0, 7].piece.Move(3, 7);
            }
            if(!moved && x == 6 && y == 7){
                ChessGame.board[7, 7].piece.Move(5, 7);
            }
            if(!moved && x == 2 && y == 0){
                ChessGame.board[0, 0].piece.Move(3, 0);
            }
            if(!moved && x == 6 && y == 0){
                ChessGame.board[7, 0].piece.Move(5, 0);
            }
            if(isBlack){
                ChessGame.chessBoard.blackKingPos.x = x;
                ChessGame.chessBoard.blackKingPos.y = y;
            } else{
                ChessGame.chessBoard.whiteKingPos.x = x;
                ChessGame.chessBoard.whiteKingPos.y = y;
            }
            ChessGame.chessBoard.fullCheckDetection = true;
            moved = true;
            base.Move(x, y);
        }

        public override void Move(int x, int y, ChessBoard cb){
            if(!moved && x == 2 && y == 7){
                cb.board[0, 7].piece.Move(3, 7, cb);
            }
            if(!moved && x == 6 && y == 7){
                cb.board[7, 7].piece.Move(5, 7, cb);
            }
            if(!moved && x == 2 && y == 0){
                cb.board[0, 0].piece.Move(3, 0, cb);
            }
            if(!moved && x == 6 && y == 0){
                cb.board[7, 0].piece.Move(5, 0, cb);
            }
            if(isBlack){
                cb.blackKingPos.x = x;
                cb.blackKingPos.y = y;
            } else{
                cb.whiteKingPos.x = x;
                cb.whiteKingPos.y = y;
            }
            cb.fullCheckDetection = true;
            moved = true;
            base.Move(x, y, cb);
        }

        public override Vector2i[] AvailableMoves(ChessBoard cb = null){
            List<Vector2i> moves = new List<Vector2i>();
            if(cb == null) cb = ChessGame.chessBoard;
            if(isBlack){
                if(!moved && !cb.AttackedByWhite(x, y)){
                    if(cb.board[0, 7].piece != null && cb.board[0, 7].piece.GetType() == typeof(Rook) && !((Rook) cb.board[0, 7].piece).moved){
                        if(!cb.AttackedByWhite(3,7) && !cb.board[3,7].Occupied && !cb.AttackedByWhite(2,7) && !cb.board[2,7].Occupied && !cb.board[1,7].Occupied){
                            moves.Add(new Vector2i(2, 7));
                        }
                    }
                    if(cb.board[7, 7].piece != null && cb.board[7, 7].piece.GetType() == typeof(Rook) && !((Rook) cb.board[7, 7].piece).moved){
                        if(!cb.AttackedByWhite(5,7) && !cb.board[5,7].Occupied && !cb.AttackedByWhite(6,7) && !cb.board[6,7].Occupied){
                            moves.Add(new Vector2i(6, 7));
                        }
                    }
                }
                if(x+1<8 && y+1<8 && !cb.board[x+1, y+1].OccupiedIsBlack) moves.Add(new Vector2i(x+1, y+1));
                if(x-1>=0 && y+1<8 && !cb.board[x-1, y+1].OccupiedIsBlack) moves.Add(new Vector2i(x-1, y+1));
                if(y+1<8 && !cb.board[x, y+1].OccupiedIsBlack) moves.Add(new Vector2i(x, y+1));
                if(x+1<8 && y-1>=0 && !cb.board[x+1, y-1].OccupiedIsBlack) moves.Add(new Vector2i(x+1, y-1));
                if(x-1>=0 && y-1>=0 && !cb.board[x-1, y-1].OccupiedIsBlack) moves.Add(new Vector2i(x-1, y-1));
                if(y-1>=0 && !cb.board[x, y-1].OccupiedIsBlack) moves.Add(new Vector2i(x, y-1));
                if(x-1>=0 && !cb.board[x-1, y].OccupiedIsBlack) moves.Add(new Vector2i(x-1, y));
                if(x+1<8 && !cb.board[x+1, y].OccupiedIsBlack) moves.Add(new Vector2i(x+1, y));
            } else{
                if(!moved && !cb.AttackedByBlack(x, y)){
                    if(cb.board[0, 0].piece != null && cb.board[0, 0].piece.GetType() == typeof(Rook) && !((Rook) cb.board[0, 0].piece).moved){
                        if(!cb.AttackedByBlack(3,0) && !cb.board[3,0].Occupied && !cb.AttackedByBlack(2,0) && !cb.board[2,0].Occupied && !cb.board[1,0].Occupied){
                            moves.Add(new Vector2i(2, 0));
                        }
                    }
                    if(cb.board[7, 0].piece != null && cb.board[7, 0].piece.GetType() == typeof(Rook) && !((Rook) cb.board[7, 0].piece).moved){
                        if(!cb.AttackedByBlack(5,0) && !cb.board[5,0].Occupied && !cb.AttackedByBlack(6,0) && !cb.board[6,0].Occupied){
                            moves.Add(new Vector2i(6, 0));
                        }
                    }
                }
                if(x+1<8 && y+1<8 && !cb.board[x+1, y+1].OccupiedIsWhite) moves.Add(new Vector2i(x+1, y+1));
                if(x-1>=0 && y+1<8 && !cb.board[x-1, y+1].OccupiedIsWhite) moves.Add(new Vector2i(x-1, y+1));
                if(y+1<8 && !cb.board[x, y+1].OccupiedIsWhite) moves.Add(new Vector2i(x, y+1));
                if(x+1<8 && y-1>=0 && !cb.board[x+1, y-1].OccupiedIsWhite) moves.Add(new Vector2i(x+1, y-1));
                if(x-1>=0 && y-1>=0 && !cb.board[x-1, y-1].OccupiedIsWhite) moves.Add(new Vector2i(x-1, y-1));
                if(y-1>=0 && !cb.board[x, y-1].OccupiedIsWhite) moves.Add(new Vector2i(x, y-1));
                if(x-1>=0 && !cb.board[x-1, y].OccupiedIsWhite) moves.Add(new Vector2i(x-1, y));
                if(x+1<8 && !cb.board[x+1, y].OccupiedIsWhite) moves.Add(new Vector2i(x+1, y));
            }
            return moves.ToArray();
        }

        public override int GetValue(){
            return 30;
        }
    }
}