using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ExpressionCalculator
{
    public class Parser
    {
        private Tokenizer _tokenizer;

        // Constructor - just store the tokenizer
        public Parser(Tokenizer tokenizer)
        {
            _tokenizer = tokenizer;
        }

        // Static helper to parse a string
        public static Node Parse(string str)
        {
            return Parse(new Tokenizer(new StringReader(str)));
        }

        // Static helper to parse from a tokenizer
        public static Node Parse(Tokenizer tokenizer)
        {
            var parser = new Parser(tokenizer);
            return parser.ParseExpression();
        }

        // Parse an entire expression and check End Of Function (EOF) was reached
        public Node ParseExpression()
        {
            // Parse the left hand side
            var expr = ParseLeaf();

            // Check everything was consumed
            if (_tokenizer.Token != Token.EOF)
                throw new Exception("Unexpected characters at end of expression");

            return expr;
        }

        // Parse a leaf node
        public Node ParseLeaf()
        {
            if (_tokenizer.Token == Token.Subtract)
            {
                // Skip
                _tokenizer.NextToken();

                // Parse RHS 
                // Note this recurses to self to support negative of a negative
                var rhs = ParseLeaf();

                // Create unary node
                return new NodeUnary(rhs, (a) => -a);
            }

            // Is it a number?
            if (_tokenizer.Token == Token.Number)
            {
                var node = new NodeNumber(_tokenizer.Number);
                _tokenizer.NextToken();
                return node;
            }

            // Parenthesis?
            if (_tokenizer.Token == Token.OpenParens)
            {
                // Skip '('
                _tokenizer.NextToken();

                // Parse a top-level expression
                var node = ParseLeaf();

                // Check and skip ')'
                if (_tokenizer.Token != Token.CloseParens)
                    throw new Exception("Missing close parenthesis");
                _tokenizer.NextToken();

                // Return
                return node;
            }

            // Function call
            if (_tokenizer.Token == Token.Identifier)
            {
                // Capture the name and skip it
                var name = _tokenizer.Identifier;
                _tokenizer.NextToken();

                // Skip parens
                _tokenizer.NextToken();

                // Parse arguments
                var arguments = new List<Node>();
                while (true)
                {
                    // Parse argument and add to list
                    arguments.Add(ParseLeaf());

                    // Is there another argument?
                    if (_tokenizer.Token == Token.Comma)
                    {
                        _tokenizer.NextToken();
                        continue;
                    }

                    // Get out
                    break;
                }

                // Check and skip ')'
                if (_tokenizer.Token != Token.CloseParens)
                    throw new Exception("Missing close parenthesis");
                _tokenizer.NextToken();

                // Create the function call node
                return new NodeFunctionCall(name, arguments.ToArray());
            }

            throw new Exception($"Unexpect token: {_tokenizer.Token}");
        }

    }
}
