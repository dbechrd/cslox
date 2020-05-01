using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cslox
{
    interface IVisit<R>
    {
        //R Visit(Expr.Assign expr);
        R Visit(Expr.Binary expr);
        //R Visit(Expr.Call expr);
        //R Visit(Expr.Get expr);
        R Visit(Expr.Grouping expr);
        R Visit(Expr.Literal expr);
        //R Visit(Expr.Logical expr);
        //R Visit(Expr.Set expr);
        //R Visit(Expr.Super expr);
        //R Visit(Expr.This expr);
        R Visit(Expr.Unary expr);
        //R Visit(Expr.Variable expr);
    }

    abstract class Expr
    {
        public abstract R Accept<R>(IVisit<R> visitor);

        public class Binary : Expr
        {
            public Expr left;
            public Token oper;
            public Expr right;

            public Binary(Expr left, Token oper, Expr right)
            {
                this.left = left;
                this.oper = oper;
                this.right = right;
            }

            public override R Accept<R>(IVisit<R> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Grouping : Expr
        {
            public Expr expr;

            public Grouping(Expr expr)
            {
                this.expr = expr;
            }

            public override R Accept<R>(IVisit<R> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Literal : Expr
        {
            public object value;

            public Literal(object value)
            {
                this.value = value;
            }

            public override R Accept<R>(IVisit<R> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Unary : Expr
        {
            public Token oper;
            public Expr right;

            public Unary(Token oper, Expr right)
            {
                this.oper = oper;
                this.right = right;
            }

            public override R Accept<R>(IVisit<R> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}
