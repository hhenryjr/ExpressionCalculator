using System;
using System.Collections.Generic;
using System.Text;

namespace ExpressionCalculator
{
    public interface IContext
    {
        double CallFunction(string name, double[] arguments);
    }
}
