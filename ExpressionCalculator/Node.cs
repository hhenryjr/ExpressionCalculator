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

    // NodeNumber represents a literal number in the expression
    public class NodeNumber : Node
    {
        public NodeNumber(int number)
        {
            _number = number;
        }

        int _number;             

        public override double Eval(IContext ctx)
        {
            // Just return the number.
            return _number;
        }
    }

    public class NodeFunctionCall : Node
    {
        public NodeFunctionCall(string functionName, Node[] arguments)
        {
            _functionName = functionName;
            _arguments = arguments;
        }

        string _functionName;
        Node[] _arguments;

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
