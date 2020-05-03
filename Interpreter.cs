using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace cslox
{
    class Interpreter : IVisit<object>
    {
        public void Interpret(Expr expr)
        {
            try
            {
                object value = Evaluate(expr);
                Console.WriteLine(Stringify(value));
            }
            catch (RuntimeError error)
            {
                Lox.RuntimeError(error);
            }
        }

        public object Visit(Expr.Binary expr)
        {
            object left = Evaluate(expr.left);
            object right = Evaluate(expr.right);

            switch (expr.oper.type) {
                case TokenType.BANG_EQUAL:
                    return !IsEqual(left, right);
                case TokenType.EQUAL_EQUAL:
                    return IsEqual(left, right);
                case TokenType.GREATER:
                    CheckNumberOperands(expr.oper, left, right);
                    return (double)left > (double)right;
                case TokenType.GREATER_EQUAL:
                    CheckNumberOperands(expr.oper, left, right);
                    return (double)left >= (double)right;
                case TokenType.LESS:
                    CheckNumberOperands(expr.oper, left, right);
                    return (double)left < (double)right;
                case TokenType.LESS_EQUAL:
                    CheckNumberOperands(expr.oper, left, right);
                    return (double)left <= (double)right;
                case TokenType.MINUS:
                    CheckNumberOperands(expr.oper, left, right);
                    return (double)left - (double)right;
                case TokenType.PLUS:
                    if (left is double && right is double) {
                        return (double)left + (double)right;
                    }
                    if (left is string && right is string) {
                        return (String)left + (String)right;
                    }
                    throw new RuntimeError(expr.oper, "Operands must be two numbers or two strings.");
                case TokenType.SLASH:
                    CheckNumberOperands(expr.oper, left, right);
                    return (double)left / (double)right;
                case TokenType.STAR:
                    CheckNumberOperands(expr.oper, left, right);
                    return (double)left * (double)right;
            }

            // Unreachable.
            return null;
        }

        public object Visit(Expr.Grouping expr)
        {
            return Evaluate(expr.expr);
        }

        public object Visit(Expr.Literal expr)
        {
            return expr.value;
        }

        public object Visit(Expr.Unary expr)
        {
            object right = Evaluate(expr.right);

            switch (expr.oper.type) {
                case TokenType.BANG:
                    return !IsTruthy(right);
                case TokenType.MINUS:
                    CheckNumberOperand(expr.oper, right);
                    return -(double)right;
            }

            // Unreachable.
            return null;
        }

        private void CheckNumberOperand(Token oper, object operand)
        {
            if (!(operand is double))
            {
                throw new RuntimeError(oper, "Operand must be a number.");
            }
        }

        private void CheckNumberOperands(Token oper, object left, object right)
        {
            if (!(left is double && right is double))
            {
                throw new RuntimeError(oper, "Operands must be numbers.");
            }
        }

        private object Evaluate(Expr expr)
        {
            return expr.Accept(this);
        }

        private bool IsTruthy(object obj)
        {
            if (obj is null) return false;
            if (obj is bool) return (bool)obj;
            return true;
        }

        private bool IsEqual(object a, object b)
        {
            // nil is only equal to nil
            if (a == null && b == null) return true;
            if (a == null) return false;
            return a.Equals(b);
        }

        private string Stringify(object obj)
        {
            if (obj == null) return "nil";
            return obj.ToString();
        }
    }
}
