using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace CourseTask
{
    class ConsoleUI
    {
        public static string ReadNonEmptyString(string prompt)
        {
            string input;
            do
            {
                Console.Write(prompt);
                input = Console.ReadLine()?.Trim();
                if (string.IsNullOrEmpty(input))
                    Console.WriteLine("Ошибка: поле не может быть пустым. Повторите ввод: ");
            } while (string.IsNullOrEmpty(input));
            return input;
        }
        public static string ReadPhoneNumber(string prompt)
        {
            string input;
            do
            {
                Console.Write(prompt);
                input = Console.ReadLine()?.Trim();
                if (string.IsNullOrEmpty(input))
                {
                    Console.WriteLine("Ошибка: номер не может быть пустым.");
                    continue;
                }
                if (!input.All(char.IsDigit))
                {
                    Console.WriteLine("Ошибка: номер должен содержать только цифры.");
                    continue;
                }
                break;
            } while (true);
            return input;
        }
        public static void ChangingMenu(Abonent abonent)
        {
            if (abonent == null) throw new Exception();
            bool exitKey = false;
            while (!exitKey)
            {
                Console.WriteLine("Выберите поле, которое хотели бы изменить: ");
                Console.WriteLine($"1) Фамилия: {abonent.Surname}");
                Console.WriteLine($"2) Имя: {abonent.Name}");
                Console.WriteLine($"3) Отчество: {abonent.Patronymic}");
                Console.WriteLine($"4) Город проживания: {abonent.City}");
                Console.WriteLine($"5) Завершить редактирование");
                string userInput = Console.ReadLine();
                int menuKey = int.TryParse(userInput, out int parsedMenuKey) ? parsedMenuKey : 0;
                switch (menuKey)
                {

                    case 1:
                        abonent.Surname = ReadNonEmptyString("Введите новую фамилию: ");
                        Console.WriteLine("Поле успешно изменено!");
                        break;
                    case 2:
                        abonent.Name = ReadNonEmptyString("Введите новое имя: ");
                        Console.WriteLine("Поле успешно изменено!");
                        break;
                    case 3:
                        abonent.Patronymic = ReadNonEmptyString("Введите новое отчество (при наличии, иначе '-') ");
                        Console.WriteLine("Поле успешно изменено!");
                        break;
                    case 4:
                        abonent.City = ReadNonEmptyString("Введите новый город проживания: ");
                        Console.WriteLine("Поле успешно изменено!");
                        break;
                    case 5:
                        Console.WriteLine("Вы завершили редактирование! ");
                        exitKey = true;
                        break;
                    default:
                        Console.WriteLine("Номер введен некорректно! Введите повторно. ");
                        break;
                }
            }
        }
       
    }
}
