using System;

namespace BullsAndCows
{
    class Program
    {
        static string prevSuggetion = "None", prevBullsCows = "Быков: None\tКоров: None";
        static long hiddenNumber;

        static void Main()
        {
            RullsOfGame();

            do
            {
                int n = 0;
                GetNumberOfDigits(ref n);
                hiddenNumber = GenerateNumber(n);
                StartGame();

                Console.WriteLine("Нажмите ENTER чтобы продолжить играть или ESC чтобы выйти из игры");
                prevSuggetion = "None";
                prevBullsCows = "Быков: None\tКоров: None";
            } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
            Console.WriteLine("Goodbye!)");
        }

        /// <summary>
        /// Вывод правил игры в консоль.
        /// </summary>
        static void RullsOfGame()
        {
            Console.Clear();
            Console.WriteLine("Игра Быки и Коровы");
            Console.WriteLine("Перед началом игры ознакомьтесь с ее правилами: \n" +
                "1. Я загадываю число (сколько цифр должно быть вы указываете сами, но не больше 10), состоящее из неповторяющихся цифр " +
                "(никакая цифра не может присутствовать в числе дважды, число не может начинаться с нуля)\n" +
                "2. Затем Вы пытаетесь угадать загаданное число. (Вводимое вами число может быть с повтором цифр)\n" +
                "3. Я говорю, сколько цифр (коров) угадано,но не расположено на своих местах; и сколько цифр (быков) угадано и находится на своих местах.\n" +
                "4. Повторяются пункты 2 и 3 пока не угадаете число");
            Console.WriteLine();
            Console.Write("Сложность в игре –- \n" +
                "Кол-во цифр:    1-4 -> Easy\n" +
                "                5-6 -> Medium\n" +
                "                7-8 -> Hard\n" +
                "                9-10 -> Very Hard");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
        }


        /// <summary>
        /// Получение от игрока количество цифр в числе.
        /// </summary>
        /// <param name="n">Количество цифр в числе типа int.</param>
        static void GetNumberOfDigits(ref int n)
        {
            string inputLine, error = "";
            // Проверка на корректный ввод данных в данном случае корректность на ввод кол-во цифр в числе.
            do
            {
                if (error == "")
                {
                    Console.Write("Введите скольки значное число вы хотите отгадать (от 1 до 10): ");
                    inputLine = Console.ReadLine();
                    error = "Ошибка: Неправильный ввод (Нужно ввести целое число от 1 до 10)";
                }
                // Вывод ошибки при некорректном вводе.
                else
                {
                    Console.WriteLine(error);
                    Console.Write("Повторите ввод: ");
                    inputLine = Console.ReadLine();
                }
            } while (!int.TryParse(inputLine, out n) || n < 1 || n > 10);
        }

        /// <summary>
        /// Запуск игры: Повторное получение от игрока числа, проверка этого числа на быков и коров, завершение игры с выводом загаданного числа и количества попыток угадать.
        /// </summary>
        static void StartGame()
        {
            long playerNumber = 0;
            int countOfGuess = 0;
            // Повтор попыток угадать число.
            do
            {
                GetPlayerNumber(ref playerNumber);

                string CheckedBullsCows = CheckBullsCows(playerNumber.ToString());
                Console.Clear();
                countOfGuess++;
                UI(CheckedBullsCows, playerNumber);
            } while (!CheckEndGame(playerNumber));

            Console.WriteLine($"Ты Угадал! Это число {hiddenNumber}. Количество попыток: {countOfGuess}");
            Console.WriteLine();
            Console.WriteLine();
        }

        /// <summary>
        /// Получение числа от игрока.
        /// </summary>
        /// <param name="playerNumber">Переменна в которую записывается полученное число.</param>
        static void GetPlayerNumber(ref long playerNumber)
        {

            string inputLine, error = "";
            // Проверка на корректность данных в данном случае корректность на ввод самого числа с учетом его кол-во цифр.
            // Вводить повторяющиеся Цифры можно чтобы было легче отгадывать.
            do
            {

                long enterFrom = (long)(hiddenNumber.ToString().Length == 1 ? Math.Pow(10, hiddenNumber.ToString().Length - 1) - 1 : Math.Pow(10, hiddenNumber.ToString().Length - 1));
                long enterTo = (long)Math.Pow(10, hiddenNumber.ToString().Length);
                if (error == "")
                {
                    Console.Write($"Введите Ваше предположение (Число должно быть от " +
                        $"{enterFrom} и до " +
                        $"{enterTo} (не включительно)): ");

                    inputLine = Console.ReadLine();
                    error = $"Ошибка: Неправильный ввод (Число должно быть от " +
                        $"{enterFrom} и до " +
                        $"{enterTo} (не включительно))";
                }
                // Вывод ошибки при некорректном вводе.
                else
                {
                    Console.WriteLine(error);
                    Console.Write("Повторите ввод: ");
                    inputLine = Console.ReadLine();
                }
            } while (!long.TryParse(inputLine, out playerNumber)
                        || playerNumber >= Math.Pow(10, hiddenNumber.ToString().Length)
                        || (playerNumber < Math.Pow(10, hiddenNumber.ToString().Length - 1) - 1 && hiddenNumber.ToString().Length == 1)
                        || (playerNumber < Math.Pow(10, hiddenNumber.ToString().Length - 1) && hiddenNumber.ToString().Length != 1)
                        || (inputLine[0] == '0' && inputLine.Length != 1));
        }


        /// <summary>
        /// Проверка введенного числа на количество в нем быков и коров.
        /// </summary>
        /// <param name="playerNumber">Число, которое нужно проверить на быки и коровы типа string.</param>
        /// <returns>Строку вида: "Быки: {Число быков} Коровы: {Число коров}".</returns>
        static string CheckBullsCows(string playerNumber)
        {
            string checkString = hiddenNumber.ToString(), numbers = "0123456789", bullContainer = "";
            int cows = 0, bulls = 0;

            // Подсчет Быков в введеном числе.
            for (int i = 0; i < checkString.Length; i++)
            {
                bulls += checkString[i] == playerNumber[i] ? 1 : 0;

                if (checkString[i] == playerNumber[i])
                {
                    bullContainer += checkString[i];
                    numbers = numbers.Replace(playerNumber[i], '^');
                }
            }
            // Подсчет Коров в введеном числе.
            for (int i = 0; i < checkString.Length; i++)
            {
                cows += !bullContainer.Contains(playerNumber[i]) && checkString.Contains(playerNumber[i]) && numbers.Contains(playerNumber[i]) ? 1 : 0;
                numbers = numbers.Replace(playerNumber[i], '^');
            }
            return $"Быки: {bulls}\tКоровы: {cows}";
        }

        static bool CheckEndGame(long playerNumber)
        {
            return playerNumber == hiddenNumber;
        }


        /// <summary>
        /// Вывод в терминал основной информации: предыдущая попытка (с быками и коровами); введенное на данный момент число; Быки и Коровы введенного числа.
        /// </summary>
        /// <param name="bullsCows">Строка возвращенная из метода CheckBullsCows(string PlayerNumber).</param>
        /// <param name="playerNumber">Введенное число игроком на данный момент.</param>
        static void UI(string bullsCows, long playerNumber)
        {
            Console.WriteLine($"Ваша предыдущая попытка: {prevSuggetion + " (" + prevBullsCows + ")"}");
            Console.WriteLine($"Введенное число: {playerNumber}");
            prevSuggetion = playerNumber.ToString();
            prevBullsCows = bullsCows;
            Console.WriteLine(bullsCows);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
        }

        //Генератор n-значных чисел
        /// <summary>
        /// Генератор чисел с уникальными цифрами (Без одинаковых цифр)
        /// </summary>
        /// <param name="n">Количество цифр в числе</param>
        /// <returns>n-значное число с уникальными цифрами</returns>
        public static long GenerateNumber(int n)
        {
            Random rnd = new();
            string numbers = "0123456789", midRes = "";
            int index;
            for (int i = 10; i > 10 - n; i--)
            {
                if (i == 10 && n != 1)
                {
                    index = rnd.Next(1, 10);
                }
                else
                {
                    index = rnd.Next(i);
                }
                midRes += numbers[index];

                numbers = numbers.Remove(index, 1);
            }
            return long.Parse(midRes);
        }
    }
}
