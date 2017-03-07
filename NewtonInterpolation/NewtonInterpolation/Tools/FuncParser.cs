using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewtonInterpolation.Tools
{
    class FuncParser
    {
        public string Postfix
        {
            get
            {
                var res = new StringBuilder();
                for (int i = 0; i < _Postfix.Count; i++)
                    res.AppendFormat("{0} ", _Postfix[i]);
                return res.ToString();
            }
        }

        public FuncParser()
        {
            _Postfix = new List<string>();

            int i = 0;
            _Operations = new Dictionary<string, int>();
            _Operations.Add("(", i++);
            _Operations.Add(")", i++);
            _Operations.Add("+", i++);
            _Operations.Add("-", i++);
            _Operations.Add("*", i++);
            _Operations.Add("/", i++);
            _Operations.Add("%", i++);
            _Operations.Add("^", i++);
            _Operations.Add("Cos", i++);
            _Operations.Add("Sin", i++);

            _Consts = new Dictionary<char, double>();
            _Consts.Add('P', Math.PI);
            _Consts.Add('E', Math.E);
            _Consts.Add('X', 0);
        }

        public FuncParser(string sourceStr) : this()
        {
            ParseString(sourceStr);
        }

        public void ParseString(string sourceStr)
        {
            if (sourceStr == null)
                throw new ArgumentNullException(nameof(sourceStr));

            if (sourceStr.Length == 0)
                throw new ArgumentException("Пустая строка");

            sourceStr += ";";
            var tmp = new StringBuilder("");
            var stackOp = new Stack<string>();
            stackOp.Push("(");
            char c = ' ';
            char _c = ' ';


            for (int i = 0; i < sourceStr.Length; i++)
            {
                c = sourceStr[i];
                if (Char.IsDigit(c) || c == ',')
                {
                    tmp.Append(c);
                }
                else if (_Consts.ContainsKey(c))
                {
                    if (tmp.Length > 0)
                        throw new MissingOperationException(sourceStr, i);
                    tmp.Append(c);
                    _Postfix.Add(tmp.ToString());
                    tmp.Clear();
                }
                else
                {
                    if (Char.IsDigit(_c))
                    {
                        _Postfix.Add(tmp.ToString());
                        //Console.Write(tmp + " ");
                        tmp.Clear();
                    }

                    tmp.Append(c);
                    if (_Operations.ContainsKey(tmp.ToString()))
                    {
                        if (c == '(')
                        {
                            stackOp.Push(tmp.ToString());
                            tmp.Clear();
                        }
                        else if (c == ')')
                        {
                            while (stackOp.Peek() != "(")
                            {
                                //Console.Write(stackOp.Peek() + " ");
                                _Postfix.Add(stackOp.Pop());
                            }
                            stackOp.Pop();
                            tmp.Clear();
                        }
                        else
                        {
                            while (_Operations[stackOp.Peek()] >= _Operations[tmp.ToString()])
                            {
                                //Console.Write(stackOp.Peek() + " ");
                                _Postfix.Add(stackOp.Pop());
                            }

                            stackOp.Push(tmp.ToString());
                            tmp.Clear();
                        }
                    }
                }

                _c = c;
            }

            while (_Operations[stackOp.Peek()] > 0)// < _Operations["("])
            {
                //Console.Write(stackOp.Peek() + " ");
                _Postfix.Add(stackOp.Pop());
            }

            if (stackOp.Count > 1)
                throw new StackNotEmptyException(stackOp.ToString());
            //Console.WriteLine("\nОшибка! В стеке остались элементы: ", stackOp.ToString());


        }

        public double Calculation(double x)
        {
            var stackNum = new Stack<double>();
            _Consts['X'] = x;

            for (int i = 0; i < _Postfix.Count; i++)
            {
                if (_Operations.ContainsKey(_Postfix[i]))
                {
                    if (_Operations[_Postfix[i]] <= _BINAR)
                    {
                        double arg2 = stackNum.Pop();
                        double arg1 = stackNum.Pop();
                        stackNum.Push(Calc(arg1, arg2, _Postfix[i]));
                    }
                    else
                    {
                        double arg = stackNum.Pop();
                        stackNum.Push(Calc(arg, _Postfix[i]));
                    }
                }
                else if (_Consts.ContainsKey(_Postfix[i][0]))
                {
                    stackNum.Push(_Consts[_Postfix[i][0]]);
                }
                else
                {
                    double num;
                    if (!Double.TryParse(_Postfix[i], out num))
                        throw new Exception("Некорректное число: " + _Postfix[i]);
                    stackNum.Push(num);
                }

            }
            if (stackNum.Count > 1)
                throw new StackNotEmptyException(stackNum.ToString());

            return stackNum.Pop();
        }




        private Dictionary<String, int> _Operations;
        private List<String> _Postfix;
        private Dictionary<char, double> _Consts;
        private const int _BINAR = 7;


        private double Calc(double arg, string oper)
        {
            double res = 0;
            switch (oper)
            {
                case "Cos":
                    res = Math.Cos(arg);
                    break;
                case "Sin":
                    res = Math.Sin(arg);
                    break;
                default:
                    throw new Exception("Неверный оператор");
                    break;
            }
            return res;
        }

        private double Calc(double arg1, double arg2, string oper)
        {
            double res = 0;
            switch (oper)
            {
                case "+":
                    res = arg1 + arg2;
                    break;
                case "-":
                    res = arg1 - arg2;
                    break;
                case "*":
                    res = arg1 * arg2;
                    break;
                case "/":
                    res = arg1 / arg2;
                    break;
                case "%":
                    res = arg1 % arg2;
                    break;
                case "^":
                    res = Math.Pow(arg1, arg2);
                    break;
                default:
                    throw new Exception("Неверный оператор");
                    break;
            }
            return res;
        }
    }

    public class MissingOperationException : Exception
    {
        public MissingOperationException() :
            base("Пропущена опрерация!")
        { }

        public MissingOperationException(string str, int pos) :
            base("Пропущена опрерация '" + str + "' в позиции: " + pos)
        { }

        public MissingOperationException(string msg) :
            base(msg)
        { }
    }

    public class StackNotEmptyException : Exception
    {
        public StackNotEmptyException(string stack) :
            base("В стеке остались операции:" + stack)
        { }
    }

}

