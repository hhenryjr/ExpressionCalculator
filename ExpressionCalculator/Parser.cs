﻿using System;
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
      var expr = ParseAddSubtract();

      // Check everything was consumed
      if (_tokenizer.Token != Token.EOF)
        throw new Exception("Unexpected characters at end of expression");

      return expr;
    }

    // Parse an sequence of add/subtract operators
    Node ParseAddSubtract()
    {
      // Parse the left hand side
      var lhs = ParseMultiplyDivide();

      while (true)
      {
        // Work out the operator
        Func<double, double, double> op = null;
        if (_tokenizer.Token == Token.Add)
        {
          op = (a, b) => a + b;
        }
        else if (_tokenizer.Token == Token.Subtract)
        {
          op = (a, b) => a - b;
        }

        // Binary operator found?
        if (op == null)
          return lhs;             // no

        // Skip the operator
        _tokenizer.NextToken();

        // Parse the right hand side of the expression
        var rhs = ParseMultiplyDivide();

        // Create a binary node and use it as the left-hand side from now on
        lhs = new NodeBinary(lhs, rhs, op);
      }
    }

    // Parse an sequence of add/subtract operators
    Node ParseMultiplyDivide()
    {
      // Parse the left hand side
      var lhs = ParseUnary();

      while (true)
      {
        // Work out the operator
        Func<double, double, double> op = null;
        if (_tokenizer.Token == Token.Multiply)
        {
          op = (a, b) => a * b;
        }
        else if (_tokenizer.Token == Token.Divide)
        {
          op = (a, b) => a / b;
        }

        // Binary operator found?
        if (op == null)
          return lhs;             // no

        // Skip the operator
        _tokenizer.NextToken();

        // Parse the right hand side of the expression
        var rhs = ParseUnary();

        // Create a binary node and use it as the left-hand side from now on
        lhs = new NodeBinary(lhs, rhs, op);
      }
    }


    // Parse a unary operator (eg: negative/positive)
    Node ParseUnary()
    {
      while (true)
      {
        // Positive operator is a no-op so just skip it
        if (_tokenizer.Token == Token.Add)
        {
          // Skip
          _tokenizer.NextToken();
          continue;
        }

        // Negative operator
        if (_tokenizer.Token == Token.Subtract)
        {
          // Skip
          _tokenizer.NextToken();

          // Parse RHS 
          // Note this recurses to self to support negative of a negative
          var rhs = ParseUnary();

          // Create unary node
          return new NodeUnary(rhs, (a) => -a);
        }

        // No positive/negative operator so parse a leaf node
        return ParseLeaf();
      }
    }

    // Parse a leaf node
    public Node ParseLeaf()
    {
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
        var node = ParseAddSubtract();

        // Check and skip ')'
        if (_tokenizer.Token != Token.CloseParens)
          throw new Exception("Missing close parenthesis");
        _tokenizer.NextToken();

        // Return
        return node;
      }

      // Variable
      if (_tokenizer.Token == Token.Identifier)
      {
        // Capture the name and skip it
        var name = _tokenizer.Identifier;
        _tokenizer.NextToken();

        // Parens indicate a function call, otherwise just a variable
        if (_tokenizer.Token != Token.OpenParens)
        {
          // Variable
          return new NodeVariable(name);
        }
        else
        {
          // Function call
          // Skip parens
          _tokenizer.NextToken();

          // Parse arguments
          var arguments = new List<Node>();
          while (true)
          {
            // Parse argument and add to list
            arguments.Add(ParseAddSubtract());

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
      }
      throw new Exception($"Unexpect token: {_tokenizer.Token}");
    }

  }
}
