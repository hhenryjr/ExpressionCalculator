using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace ExpressionCalculator
{
  class Program
  {
    static void Main(string[] args)
    {
      while (true)
      {
        Console.WriteLine("Please enter expression.");
        var exp = Console.ReadLine();
        var lib = new CalcFunctions();
        var ctx = new ReflectionContext(lib);
        var node = Parser.Parse(exp);
        var result = node.Eval(ctx);
        Console.WriteLine(result);
      }
    }
  }
}
