using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cslox
{
    public static class Lox
    {
        static bool ErrorFlag = false;

        static int Main(string[] args)
        {

            Expr expression = new Expr.Binary(
                new Expr.Unary(
                    new Token(TokenType.MINUS, "-", null, 1, 0),
                    new Expr.Literal(123)),
                new Token(TokenType.STAR, "*", null, 1, 0),
                new Expr.Grouping(
                    new Expr.Literal(45.67)));

            Console.WriteLine(new AstPrinter().Print(expression));

            int exitCode = 0;
            if (args.Length > 1)
            {
                Console.WriteLine("Usage cslox: [script]");
                exitCode = (int)ErrorCode.EX_USAGE;
            }
            else if (args.Length == 1)
            {
                exitCode = RunFile(args[0]);
            }
            else
            {
                exitCode = RunPrompt();
            }
            return exitCode;
        }

        static int RunFile(string script)
        {
            string text = File.ReadAllText(script);
            return Run(text);
        }

        static int RunPrompt()
        {
            while (true)
            {
                Console.Write("> ");
                string text = Console.ReadLine();
                if (String.Equals(text, "quit") || String.Equals(text, "exit"))
                {
                    break;
                }
                Run(text);
            }
            return 0;
        }

        static int Run(string source)
        {
            ErrorFlag = false;

            Scanner scanner = new Scanner(source);
            List<Token> tokens = scanner.ScanTokens();

            // DEBUG: Print tokens
            Console.WriteLine("\nTokens:");
            Console.WriteLine("-------");
            foreach (Token token in tokens)
            {
                Console.WriteLine(token);
            }

            Parser parser = new Parser(tokens);
            Expr expression = parser.Parse();

            // Syntax error
            if (ErrorFlag)
            {
                return (int)ErrorCode.EX_DATAERR;
            }

            // DEBUG: Print AST
            Console.WriteLine("\nAST:");
            Console.WriteLine("-------");
            AstPrinter printer = new AstPrinter();
            Console.WriteLine(printer.Print(expression));
            Console.WriteLine();

            return 0;
        }

        public static void Error(int line, int column, string message)
        {
            ReportError(line, column, "", message);
        }

        public static void Error(Token token, string message)
        {
            if (token.type == TokenType.EOF)
            {
                ReportError(token.line, token.column, " at end", message);
            }
            else
            {
                ReportError(token.line, token.column, $" at '{token.lexeme}'", message);
            }
        }

        private static void ReportError(int line, int column, string where, string message)
        {
            Console.WriteLine($"[line {line}:column {column}] Error {where}: {message}");
            Lox.ErrorFlag = true;
        }

        enum ErrorCode
        {
            SUCCESS = 0,
            EX_USAGE = 64,          // The command was used incorrectly, e.g., with the
                                    // wrong number of arguments, a bad flag, a bad syntax
                                    // in a parameter, or whatever.

            EX_DATAERR = 65,        // The input data was incorrect in some way.  This
                                    // should only be used for user's data and not system
                                    // files.

            EX_NOINPUT = 66,        // An input file (not a system file) did not exist or
                                    // was not readable.  This could also include errors
                                    // like ``No message'' to a mailer (if it cared to
                                    // catch it).

            EX_NOUSER = 67,         // The user specified did not exist.  This might be
                                    // used for mail addresses or remote logins.

            EX_NOHOST = 68,         // The host specified did not exist.  This is used in
                                    // mail addresses or network requests.

            EX_UNAVAILABLE = 69,    // A service is unavailable.  This can occur if a sup­
                                    // port program or file does not exist.  This can also
                                    // be used as a catchall message when something you
                                    // wanted to do doesn't work, but you don't know why.

            EX_SOFTWARE = 70,       // An internal software error has been detected.  This
                                    // should be limited to non-operating system related
                                    // errors as possible.

            EX_OSERR = 71,          // An operating system error has been detected.  This
                                    // is intended to be used for such things as ``cannot
                                    // fork'', ``cannot create pipe'', or the like.  It
                                    // includes things like getuid returning a user that
                                    // does not exist in the passwd file.

            EX_OSFILE = 72,         // Some system file (e.g., /etc/passwd, /var/run/utmp,
                                    // etc.) does not exist, cannot be opened, or has some
                                    // sort of error (e.g., syntax error).

            EX_CANTCREAT = 73,      // A (user specified) output file cannot be created.

            EX_IOERR = 74,          // An error occurred while doing I/O on some file.

            EX_TEMPFAIL = 75,       // Temporary failure, indicating something that is not
                                    // really an error.  In sendmail, this means that a
                                    // mailer (e.g.) could not create a connection, and
                                    // the request should be reattempted later.

            EX_PROTOCOL = 76,       // The remote system returned something that was ``not
                                    // possible'' during a protocol exchange.

            EX_NOPERM = 77,         // You did not have sufficient permission to perform
                                    // the operation.  This is not intended for file sys­
                                    // tem problems, which should use EX_NOINPUT or
                                    // EX_CANTCREAT, but rather for higher level permis­
                                    // sions.

            EX_CONFIG = 78,         // Something was found in an unconfigured or miscon­
                                    // figured state.
        }
    }
}
