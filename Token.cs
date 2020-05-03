namespace cslox
{
    public enum TokenType
    {
        UNKNOWN,

        // 1 character tokens
        LEFT_PAREN, RIGHT_PAREN, LEFT_BRACE, RIGHT_BRACE,
        COMMA, DOT, MINUS, PLUS, SEMICOLON, SLASH, STAR,

        // 1 or 2 character tokens
        BANG, BANG_EQUAL,
        EQUAL, EQUAL_EQUAL,
        GREATER, GREATER_EQUAL,
        LESS, LESS_EQUAL,

        // literals
        IDENTIFIER, STRING, NUMBER,

        // keywords
        AND, CLASS, ELSE, FALSE, FN, FOR, IF, NIL, OR,
        PRINT, RETURN, SUPER, THIS, TRUE, LET, WHILE,

        EOF
    }

    public class Token
    {
        public TokenType type;
        public string lexeme;
        public object literal;
        public int line;
        public int column;

        public Token(TokenType type, string lexeme, object literal, int line, int column)
        {
            this.type = type;
            this.lexeme = lexeme;
            this.literal = literal;
            this.line = line;
            this.column = column;
        }

        public override string ToString()
        {
            return $"{type} {lexeme} {literal}";
        }
    }
}