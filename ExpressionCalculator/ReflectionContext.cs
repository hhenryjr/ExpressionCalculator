using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ExpressionCalculator
{

    public class ReflectionContext : IContext
    {

        private object _targetObject;

        public ReflectionContext(object targetObject)
        {
            _targetObject = targetObject;
        }

        public double CallFunction(string name, double[] arguments)
        {
            // Find method
            var mi = _targetObject.GetType().GetMethod(name);
            if (mi == null)
                throw new InvalidDataException($"Unknown function: '{name}'");

            // Convert double array to object array
            var argObjs = arguments.Select(x => (object)x).ToArray();

            // Call the method
            return (double)mi.Invoke(_targetObject, argObjs);
        }
    }
}
