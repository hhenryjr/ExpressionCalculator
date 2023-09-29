using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionCalculator
{
  // Node - abstract class representing one node in the expression 
  public abstract class Node
  {
    public abstract double Eval(IContext ctx);
  }

  public class NodeUnary : Node
  {
    // Constructor accepts the two nodes to be operated on and function
    // that performs the actual operation
    public NodeUnary(Node rhs, Func<double, double> op)
    {
      _rhs = rhs;
      _op = op;
    }

    // Right hand side of the operation
    Node _rhs;

    // The callback operator
    Func<double, double> _op;

    public override double Eval(IContext ctx)
    {
      // Evaluate RHS
      var rhsVal = _rhs.Eval(ctx);

      // Evaluate and return
      var result = _op(rhsVal);
      return result;
    }
  }

  // NodeBinary for binary operations such as Add, Subtract etc...
  public class NodeBinary : Node
  {
    // Constructor accepts the two nodes to be operated on and function
    // that performs the actual operation
    public NodeBinary(Node lhs, Node rhs, Func<double, double, double> op)
    {
      _lhs = lhs;
      _rhs = rhs;
      _op = op;
    }

    Node _lhs;                              // Left hand side of the operation
    Node _rhs;                              // Right hand side of the operation
    Func<double, double, double> _op;       // The callback operator

    public override double Eval(IContext ctx)
    {
      // Evaluate both sides
      var lhsVal = _lhs.Eval(ctx);
      var rhsVal = _rhs.Eval(ctx);

      // Evaluate and return
      var result = _op(lhsVal, rhsVal);
      return result;
    }
  }

  // NodeNumber represents a literal number in the expression
  public class NodeNumber : Node
  {

    double _number;
    public NodeNumber(double number)
    {
      _number = number;
    }

    public override double Eval(IContext ctx)
    {
      // Just return the number.
      return _number;
    }
  }

  // Represents a variable (or a constant) in an expression.  eg: "2 * pi"
  public class NodeVariable : Node
  {
    public NodeVariable(string variableName)
    {
      _variableName = variableName;
    }

    string _variableName;

    public override double Eval(IContext ctx)
    {
      return ctx.ResolveVariable(_variableName);
    }
  }

  // Represents a function being called
  public class NodeFunctionCall : Node
  {
    string _functionName;
    Node[] _arguments;
    public NodeFunctionCall(string functionName, Node[] arguments)
    {
      _functionName = functionName;
      _arguments = arguments;
    }

    public override double Eval(IContext ctx)
    {
      // Evaluate all arguments
      var argVals = new double[_arguments.Length];
      for (int i = 0; i < _arguments.Length; i++)
      {
        argVals[i] = _arguments[i].Eval(ctx);
      }

      // Call the function
      return ctx.CallFunction(_functionName, argVals);
    }
  }
}
