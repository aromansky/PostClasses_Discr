using System.Data;

namespace ANF_Romanskii
{
    public static class MyFuncs
    {
        /// <summary>
        /// Возвращает массив, введённый с клавиатуры.
        /// </summary>
        /// <returns></returns>
        public static int[] ReadArrInt()
        {
            var res = new List<int>();
            Console.Write("Введите eval функции: ");
            var s = Console.ReadLine();
            foreach (var i in s)
                res.Add(int.Parse(i.ToString()));
            return res.ToArray();
        }
    }
    internal class Romanskii
    {
        /// <summary>
        /// Провекра корректности значений функции.
        /// </summary>
        /// <param name="eval"></param>
        /// <returns></returns>
        public static bool EvalCheck(int[] eval)
        {
            for (var i = 0; i < 1000; i++)
                if (eval.Length == Math.Pow(2, i))
                    return true;
            return false;
        }

        /// <summary>
        /// Возвращает количество переменных в функции.
        /// </summary>
        /// <param name="eval"></param>
        /// <returns></returns>
        public static int CountVariables(int[] eval)
        {
            var res = -1;
            if (EvalCheck(eval))
            {
                for (var i = 0; i < 1000; i++)
                    if (eval.Length == Math.Pow(2, i))
                    {
                        res = i;
                        break;
                    }
            }
            return res;
        }

        /// <summary>
        /// Возвращает массив коэффициетов АНФ.
        /// </summary>
        /// <param name="eval"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static bool[] ANF_Coefficients(int[] eval)
        {
            foreach (var i in eval)
                if (!new int[] { 1, 0 }.Contains(i))
                    throw new Exception("В переданном eval некорректные значения.");

            if (!EvalCheck(eval))
                throw new ArgumentException("В переданном eval некорректное число значений.");

            var count = eval.Length;
            var res = new int[count];

            res[0] = eval[0];
            for (var i = 1; i < res.Length; i++)
            {
                var a = new int[eval.Length - 1];
                for (var j = 0; j < a.Length; j++)
                    a[j] = eval[j] ^ eval[j + 1];
                eval = a;
                res[i] = eval[0];
            }

            return res.Select(x => x == 1 ? true : false).ToArray();
        }

        /// <summary>
        /// Возвращает принадлежность функции к классам Поста.
        /// </summary>
        /// <param name="eval"></param>
        /// <returns></returns>
        public static (bool ClassP0, bool ClassP1, bool ClassL, bool ClassS, bool ClassM) PostClasses(int[] eval)
        {
            var res = (ClassP0: false, ClassP1: false, ClassL: true, ClassS: true, ClassM: true);
            var boolEval = eval.Select(x => x == 1 ? true : false).ToArray();

            var coeffs = ANF_Coefficients(eval);
            var countVars = CountVariables(eval);
            var len = eval.Length;

            // Обработка констант
            if (eval.Length == 1)
            {
                if (eval.First() == 1)
                    return (false, true, true, false, true);
                else if (eval.First() == 0)
                    return (true, false, true, false, true);
            }
                


            // Проверка принадлежности к классам P0 и P1
            res.ClassP0 = !boolEval[0];
            res.ClassP1 = boolEval[len - 1];


            // Построение таблицы истинности
            var TrTable = new int[len][];
            for (var i = 0; i < len; i++)
            {
                var st = Convert.ToString(i, 2);
                st = new string('0', countVars - st.Length) + st;
                TrTable[i] = st.Select(x => int.Parse(x.ToString())).ToArray();
            }

            // Проверка принадлежности к классу L
            for (var i = 0; i < coeffs.Length; i++)
            {
                if (TrTable[i].Count(x => x ==  1) <= 1)
                    continue;
                if (coeffs[i])
                {
                    res.ClassL = false;
                    break;
                }
            }

            // Проверка принадлежности к классу S
            for (var i = 0; i < len / 2; i++)
                if (boolEval[i] != !boolEval[len - 1 - i])
                {
                    res.ClassS = false;
                    break;
                }


            // Проверка принадлежности к классу M
            for (var i = 0; i < len - 1; i++)
            {
                if (!res.ClassM)
                    break;
                for (var j = i + 1; j < len; j++)
                {
                    var compRes = true;
                    for (var k = 0; k < countVars; k++)
                    {
                        if (TrTable[i][k] > TrTable[j][k])
                        {
                            compRes = false;
                            break;
                        }
                    }
                    if (compRes && eval[i] > eval[j])
                    {
                        res.ClassM = false;
                        break;
                    }
                }
            }
            return res;
        }

        static void Main()
        {
            Console.WriteLine("Теоретически, программа может принять eval(f) от функции с 1000-ю переменными.\n" +
                "Практически - проведены тесты на функциях от 1 до 5 переменных.\n");
            try
            {
                var PostCl = PostClasses(MyFuncs.ReadArrInt());
                Console.WriteLine("\nФункция принадлежит к классу:\n" +
                    $"P0: {PostCl.ClassP0}\n" +
                    $"P1: {PostCl.ClassP1}\n" +
                    $"L:  {PostCl.ClassL}\n" +
                    $"S:  {PostCl.ClassS}\n" +
                    $"M:  {PostCl.ClassM}\n");
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            Console.WriteLine("Нажмите любую клавишу, чтобы закрыть это окно:");
            Console.ReadLine();
        }
    }
}
