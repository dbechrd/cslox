using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cslox
{
    class AstPrinter : IVisit<string>
    {
        public string Print(Expr expr)
        {
            return expr.Accept(this);
        }

        public string Visit(Expr.Binary expr)
        {
            return Parenthesize(expr.oper.lexeme, expr.left, expr.right);
        }

        public string Visit(Expr.Grouping expr)
        {
            return Parenthesize("group", expr.expr);
        }

        public string Visit(Expr.Literal expr)
        {
            if (expr.value != null)
            {
                return expr.value.ToString();
            }
            return "nil";
        }

        public string Visit(Expr.Unary expr)
        {
            return Parenthesize(expr.oper.lexeme, expr.right);
        }

        private string Parenthesize(string name, params Expr[] exprs)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("(").Append(name);
            foreach (Expr expr in exprs)
            {
                builder.Append(" ");
                builder.Append(expr.Accept(this));
            }
            builder.Append(")");

            return builder.ToString();
        }
    }
}
