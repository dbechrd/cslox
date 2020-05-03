using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cslox
{
    class Parser
    {
        private class ParseError : Exception { }

        /*
        expression     → equality ;
        equality       → comparison ( ( "!=" | "==" ) comparison )* ;
        comparison     → addition ( ( ">" | ">=" | "<" | "<=" ) addition )* ;
        addition       → multiplication ( ( "-" | "+" ) multiplication )* ;
        multiplication → unary ( ( "/" | "*" ) unary )* ;
        unary          → ( "!" | "-" ) unary
                       | primary ;
        primary        → NUMBER | STRING | "false" | "true" | "nil"
                       | "(" expression ")" ;
        */

        List<Token> tokens;
        int current = 0;

        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
        }

        public Expr Parse()
        {
            try
            {
                return Expression();
            }
            catch (ParseError error)
            {
                error.GetType();
                // TODO: Handle this better (statements?)
                return null;
            }
        }

        Expr Expression()
        {
            return Equality();
        }

        Expr Equality()
        {
            Expr expr = Comparison();
            while (Match(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL))
            {
                Token oper = Previous();
                Expr right = Comparison();
                expr = new Expr.Binary(expr, oper, right);
            }
            return expr;
        }

        Expr Comparison()
        {
            Expr expr = Addition();
            while (Match(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL))
            {
                Token oper = Previous();
                Expr right = Addition();
                expr = new Expr.Binary(expr, oper, right);
            }
            return expr;
        }

        Expr Addition()
        {
            Expr expr = Multiplication();
            while (Match(TokenType.MINUS, TokenType.PLUS))
            {
                Token oper = Previous();
                Expr right = Multiplication();
                expr = new Expr.Binary(expr, oper, right);
            }
            return expr;
        }

        Expr Multiplication()
        {
            Expr expr = Unary();
            while (Match(TokenType.SLASH, TokenType.STAR))
            {
                Token oper = Previous();
                Expr right = Unary();
                expr = new Expr.Binary(expr, oper, right);
            }
            return expr;
        }

        Expr Unary()
        {
            Expr expr;
            if (Match(TokenType.BANG, TokenType.MINUS))
            {
                Token oper = Previous();
                Expr right = Unary();
                expr = new Expr.Unary(oper, right);
            }
            else
            {
                expr = Primary();
            }
            return expr;
        }

        Expr Primary()
        {
            Expr expr;
            if (Match(TokenType.FALSE))
            {
                expr = new Expr.Literal(false);
            }
            else if (Match(TokenType.TRUE))
            {
                expr = new Expr.Literal(true);
            }
            else if (Match(TokenType.NIL))
            {
                expr = new Expr.Literal(null);
            }
            else if (Match(TokenType.NUMBER, TokenType.STRING))
            {
                object literal = Previous().literal;
                expr = new Expr.Literal(literal);
            }
            else if (Match(TokenType.LEFT_PAREN))
            {
                Expr inner_expr = Expression();
                Consume(TokenType.RIGHT_PAREN, "Expect ')' after expression.");
                expr = new Expr.Grouping(inner_expr);
            }
            else
            {
                // TODO: Report error
                Token next = Peek();
                throw Error(next, "Expect expression.");
            }
            return expr;
        }

        bool Match(params TokenType[] types)
        {
            foreach (TokenType type in types)
            {
                if (Check(type))
                {
                    Advance();
                    return true;
                }
            }
            return false;
        }

        Token Consume(TokenType type, string message)
        {
            Token token;

            if (!Check(type))
            {
                token = Peek();
                throw Error(token, message);
            }

            token = Advance();
            return token;
        }

        bool Check(TokenType type)
        {
            if (IsAtEnd()) return false;

            Token peek = Peek();
            return peek.type == type;
        }

        Token Advance()
        {
            if (!IsAtEnd()) current++;

            return Previous();
        }

        bool IsAtEnd()
        {
            Token peek = Peek();
            return peek.type == TokenType.EOF;
        }

        Token Peek()
        {
            return tokens[current];
        }

        Token Previous()
        {
            return tokens[current - 1];
        }

        // http://craftinginterpreters.com/parsing-expressions.html
        // > Another way to handle common syntax errors is with error productions. You augment the grammar with a rule
        // > that matches the erroneous syntax. The parser safely parses it but then reports it as an error instead of
        // > producing a syntax tree.
        ParseError Error(Token token, string message)
        {
            Lox.Error(token, message);
            return new ParseError();
        }

        void Synchronize()
        {
            Advance();

            while (!IsAtEnd())
            {
                Token prev = Previous();

                // If previous token is a semi-colon, assume we're at the end of a statement
                // NOTE: not necessarily true (e.g. for loops separators), but we're already in a soft panic
                if (prev.type == TokenType.SEMICOLON)
                    return;

                // If next token is one of these keywords, we're at the beginning of a statement
                Token peek = Peek();
                switch (peek.type)
                {
                    case TokenType.CLASS:
                    case TokenType.FN:
                    case TokenType.LET:
                    case TokenType.FOR:
                    case TokenType.IF:
                    case TokenType.WHILE:
                    case TokenType.PRINT:
                    case TokenType.RETURN:
                        return;
                }

                Advance();
            }
        }
    }
}
