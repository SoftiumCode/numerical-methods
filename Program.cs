// (c) Ярослав Юрьев , ПР-22.102 х Егор Шатровой, ПР-22.103к

// Импорты
using NCalc;

//
namespace NumericalMethods
{
    class Program
    {
        private static string expression; 
        private static double x, a, b, e, m;

        static void Main()
        {
            Console.WriteLine("Добро пожаловать в программу для метода касательных (с) Ярослав Юрьев х Егор Шатровой, 2024\n");
            (x, a, b, e) = ArgumentsInput();
            m = GetSecondDerivativeIntervalMin(expression, a, b, e);

            double result;
            uint iteration;
            (result, iteration) = GetResult(expression, x, e, m);
            
            Console.WriteLine($"Найденное значение: {result}");
            Console.WriteLine($"Номер итерации: {iteration}");

            // Ожидаем ввод пользователя, чтобы программа не закрылась сразу.
            Console.ReadKey();
        }

        // Ввод аргументов
        static (double, double, double, double) ArgumentsInput()
        {
            Console.WriteLine("Необходимо указать промежуток [a; b].");
            Console.WriteLine("Введите значение a:");
            double a = Convert.ToDouble(Console.ReadLine());
            
            Console.WriteLine("Введите значение b:");
            double b = Convert.ToDouble(Console.ReadLine());
            
            Console.WriteLine("Необходимо указать приближение e:\nПримечание: цифры в дробной части числа должны указываться после ЗАПЯТОЙ перед целой частью\nПример: 0,0005");
            Console.WriteLine("Введите значение e:");
            double e = Convert.ToDouble(Console.ReadLine());

            bool check = false;
            while (true)
            {
                foreach (var i in e.ToString())
                {

                    if (i == ',')
                    {
                        check = true;
                    }
                }

                if (check) break;
                else
                {
                    Console.WriteLine("Введите корректное значение e:");
                    e = Convert.ToDouble(Console.ReadLine());
                }
            }
            
            Console.WriteLine("Введите значение аргумента x:");
            Console.WriteLine("Примечание: аргумент должен удовлетворять условию f(x0) * F\"(x0) > 0");

            double x;
            
            while (true)
            {
                x = Convert.ToDouble(Console.ReadLine());

                // ввод функции
                expression = ExpressionInput();
                
                double fx = GetFunctionValue(expression, x);
                double fx1 = Math.Round(GetFirstDerivative(expression, x, e));
                double fx2 = Math.Round(GetSecondDerivative(expression, x, e));

                Console.WriteLine($"Значение f(x) = {fx}");
                Console.WriteLine($"Значение f'(x) = {fx1}");
                Console.WriteLine($"Значение f\"(x) = {fx2}");

                if (fx * fx2 > 0) break;
                else Console.WriteLine("Некорректный аргумент x, введите ещё раз:");
            }
            
            return (x, a, b, e);
        }
        
        // Ввод функции f(x)
        static string ExpressionInput()
        {
            Console.WriteLine("Введите функцию.\nПримечание: в функции может присутствовать только одна переменная x\nВ ином случае будет выведена ошибка и результат будет равен нулю.");
            Console.WriteLine("Доступные математические функции: pow(число, степень) — возведение в корень; sqrt(число) — квадратный корень");
            
            
            return Console.ReadLine().ToLower();
        }

        // Получение значения
        static double GetFunctionValue(string expressionString, double x)
        {
            // Вводим выражение, полученное ранее
            Expression expr = new Expression(expressionString);
            
            expr.Parameters["x"] = x;

            expr.EvaluateFunction += (string name, FunctionArgs args) =>
            {
                switch (name)
                {
                    case "pow":
                        args.Result = Math.Pow(Convert.ToDouble(args.Parameters[0].Evaluate()),
                            Convert.ToDouble(args.Parameters[1].Evaluate()));
                        break;
                    case "sqrt":
                        args.Result = Math.Sqrt(Convert.ToDouble(args.Parameters[0].Evaluate()));
                        break;
                }
            };

            try
            {
                return Convert.ToDouble(expr.Evaluate());
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Произошла ошибка: {e}");
                return 0;
            }
            
        }

        // Нахождение производных
        static double GetFirstDerivative(string expression, double x, double accuracy)
        {
            return (GetFunctionValue(expression, x + accuracy) - GetFunctionValue(expression, x)) / accuracy;
        }

        static double GetSecondDerivative(string expression, double x, double accuracy)
        {
            return (GetFirstDerivative(expression, x+accuracy, accuracy) - GetFirstDerivative(expression, x, accuracy)) / accuracy;
        }
        
        // Нахождение переменной m
        static double GetSecondDerivativeIntervalMin(string expression, double a, double b, double accuracy)
        {
            double m, fx2a, fx2b; // fx2a и fx2b - вторые производные a и b соответственно

            fx2a = Math.Abs(GetFirstDerivative(expression, a, accuracy));
            fx2b = Math.Abs(GetFirstDerivative(expression, b, accuracy));

            if (fx2a < fx2b) m = fx2a;
            else m = fx2b;

            m = Math.Round(m, 5);

            Console.WriteLine($"Значение переменной m = {m}");
            
            return m;
        }

        static (double, uint) GetResult(string expression, double x, double accuracy, double m)
        {
            uint i = 1;
            while (true)
            {
                double fx = Math.Round(GetFunctionValue(expression, x), 5);
                double fx1 = Math.Round(GetFirstDerivative(expression, x, e));
                
                x = Math.Round(x - (fx / fx1), 5);
                
                Console.WriteLine($"x({i}) = {x} - ({fx})/({fx1}) = {x}");
                fx = Math.Round(GetFunctionValue(expression, x), 5);

                double check = Math.Abs(fx / m);
                Console.WriteLine($"|F({x})| = {expression.Replace("x", x.ToString())}/{m} = {Math.Abs(fx / 2)}");

                if (check <= accuracy)
                {
                    Console.WriteLine("< E");
                    break;
                }
                
                Console.WriteLine("> E\n");
                i++;
            }

            return (x, i);
        }
    }
}