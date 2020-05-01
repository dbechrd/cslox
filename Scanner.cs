using System;
using System.Collections.Generic;

namespace cslox
{
    class Scanner
    {
        string source;
        List<Token> tokens = new List<Token>();
        int start = 0;
        int current = 0;
        int line = 1;
        int column = 0;

        static Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>()
        {
            { "and",    TokenType.AND },
            { "class",  TokenType.CLASS },
            { "else",   TokenType.ELSE },
            { "false",  TokenType.FALSE },
            { "for",    TokenType.FOR },
            { "fn",     TokenType.FN },
            { "if",     TokenType.IF },
            { "nil",    TokenType.NIL },
            { "or",     TokenType.OR },
            { "print",  TokenType.PRINT },
            { "return", TokenType.RETURN },
            { "super",  TokenType.SUPER },
            { "this",   TokenType.THIS },
            { "true",   TokenType.TRUE },
            { "let",    TokenType.LET },
            { "while",  TokenType.WHILE },
        };

        public Scanner(string source)
        {
            this.source = source;
        }

        public List<Token> ScanTokens()
        {
            while (!IsAtEnd())
            {
                start = current;
                ScanToken();
            }

            tokens.Add(new Token(TokenType.EOF, "", null, line, 0));
            return tokens;
        }

        private bool IsAtEnd()
        {
            return current >= source.Length;
        }

        private void ScanToken()
        {
            char c = Advance();
            switch (c)
            {
                case '(': AddToken(TokenType.LEFT_PAREN); break;
                case ')': AddToken(TokenType.RIGHT_PAREN); break;
                case '{': AddToken(TokenType.LEFT_BRACE); break;
                case '}': AddToken(TokenType.RIGHT_BRACE); break;
                case ',': AddToken(TokenType.COMMA); break;
                case '.': AddToken(TokenType.DOT); break;
                case '-': AddToken(TokenType.MINUS); break;
                case '+': AddToken(TokenType.PLUS); break;
                case ';': AddToken(TokenType.SEMICOLON); break;
                case '*': AddToken(TokenType.STAR); break;
                case '!': AddToken(Match('=') ? TokenType.BANG_EQUAL : TokenType.BANG); break;
                case '=': AddToken(Match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL); break;
                case '<': AddToken(Match('=') ? TokenType.LESS_EQUAL : TokenType.LESS); break;
                case '>': AddToken(Match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER); break;
                case '/':
                    if (Match('/'))
                    {
                        // Eat comment
                        while (Peek() != '\n' && !IsAtEnd())
                        {
                            Advance();
                        }
                    }
                    else
                    {
                        AddToken(TokenType.SLASH);
                    }
                    break;
                case ' ':
                case '\r':
                case '\t':
                    // ignore whitespace
                    break;
                case '\n':
                    line++;
                    column = 0;
                    break;
                case '"':
                    ScanString();
                    break;
                default:
                    if (IsDigit(c))
                    {
                        ScanNumber();
                    }
                    else if (IsAlpha(c))
                    {
                        ScanIdentifier();
                    }
                    else
                    {
                        Lox.Error(line, column, $"Unexpected character '{c}'.");
                    }
                    break;
            }
        }

        private void ScanString()
        {
            while (Peek() != '"' && !IsAtEnd())
            {
                if (Peek() == '\n') line++;
                Advance();
            }

            if (IsAtEnd())
            {
                Lox.Error(line, column, "Unterminated string.");
                return;
            }

            // TODO(dlb): If we wanted to support escapes sequences in strings (e.g. '\n', etc.), we would need to
            // unescape them here.
            string value = source.Substring(start + 1, current - start - 1);

            // eat closing '"'
            Advance();

            AddToken(TokenType.STRING, value);
        }

        private void ScanNumber()
        {
            while (IsDigit(Peek()))
            {
                Advance();
            }

            if (Peek() == '.' && IsDigit(PeekNext()))
            {
                // eat the '.'
                Advance();

                while (IsDigit(Peek()))
                {
                    Advance();
                }
            }

            string text = source.Substring(start, current - start);
            double value;
            if (!Double.TryParse(text, out value))
            {
                // TODO(dlb): Catch exception and print more specific error message?
                Lox.Error(line, column, $"Failed to parse number '{text}'.");
                return;
            }

            AddToken(TokenType.NUMBER, value);
        }

        private void ScanIdentifier()
        {
            while (IsAlphaNumeric(Peek()))
            {
                Advance();
            }

            string text = source.Substring(start, current - start);
            TokenType type;
            if (!keywords.TryGetValue(text, out type))
            {
                type = TokenType.IDENTIFIER;
            }

            AddToken(type);
        }

        private bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        private bool IsAlpha(char c)
        {
            return (c >= 'a' && c <= 'z') ||
                (c >= 'A' && c <= 'Z') ||
                c == '_';
        }

        private bool IsAlphaNumeric(char c)
        {
            return IsAlpha(c) || IsDigit(c);
        }

        private bool Match(char expected)
        {
            if (IsAtEnd())
                return false;
            if (source[current] != expected)
                return false;

            current++;
            column++;
            return true;
        }

        private char Peek()
        {
            if (IsAtEnd())
                return '\0';

            return source[current];
        }

        private char PeekNext()
        {
            if (current + 1 >= source.Length)
                return '\0';

            return source[current + 1];
        }

        private char Advance()
        {
            current++;
            column++;
            return source[current - 1];
        }

        private void AddToken(TokenType type, object literal = null)
        {
            string text = source.Substring(start, current - start);
            tokens.Add(new Token(type, text, literal, line, column));
        }
    }
}