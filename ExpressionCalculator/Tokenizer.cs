using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace ExpressionCalculator
{
    public enum Token
    {
        EOF,
        Subtract,
        OpenParens,
        CloseParens,
        Comma,
        Identifier,
        Number,
    }

    public class Tokenizer
    {
        private TextReader _reader;
        private char _currentChar;

        public Tokenizer(TextReader reader)
        {
            _reader = reader;
            NextChar();
            NextToken();
        }

        public Token Token { get; private set; }

        public int Number { get; private set; }
        public string Identifier { get; private set; }

        // Read the next character from the input strem
        // and store it in _currentChar, or load '\0' if End Of Function (EOF)
        void NextChar()
        {
            int ch = _reader.Read();
            _currentChar = ch < 0 ? '\0' : (char)ch;
        }

        // Read the next token from the input stream
        public void NextToken()
        {
            // Skip whitespace
            while (char.IsWhiteSpace(_currentChar))
            {
                NextChar();
            }

            // Special characters
            switch (_currentChar)
            {
                case '\0':
                    Token = Token.EOF;
                    return;

                case '-':
                    NextChar();
                    Token = Token.Subtract;
                    return;

                case '(':
                    NextChar();
                    Token = Token.OpenParens;
                    return;

                case ')':
                    NextChar();
                    Token = Token.CloseParens;
                    return;

                case ',':
                    NextChar();
                    Token = Token.Comma;
                    return;
            }

            // Number?
            if (char.IsDigit(_currentChar) || _currentChar == '.')
            {
                var sb = new StringBuilder();
                while (char.IsDigit(_currentChar))
                {
                    sb.Append(_currentChar);
                    NextChar();
                }

                // Parse it
                Number = int.Parse(sb.ToString(), CultureInfo.InvariantCulture);
                Token = Token.Number;
                return;
            }

            // Identifier - starts with letter
            if (char.IsLetter(_currentChar))
            {
                var sb = new StringBuilder();

                // Accept letter or digit
                while (char.IsLetterOrDigit(_currentChar))
                {
                    sb.Append(_currentChar);
                    NextChar();
                }

                // Setup token
                Identifier = sb.ToString();
                Token = Token.Identifier;
                return;
            }
        }
    }
}
