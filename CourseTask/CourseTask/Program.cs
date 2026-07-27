using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseTask
{
    class Program
    {
        static void Main()
        {
            AbonentStorage abonentStorage = new AbonentStorage();
            Task Loading = abonentStorage.ReadBinaryAsync();
            bool exitKey = false;
            while (!exitKey)
            {
                Console.WriteLine("Выберите действие: ");
                Console.WriteLine("1) Отфильтровать абонентов по первым четырем цифрам номера телефона");
                Console.WriteLine("2) Добавить запись о новом абоненте");
                Console.WriteLine("3) Изменить запись об абоненте");
                Console.WriteLine("4) Удалить запись об абоненте");
                Console.WriteLine("5) Завершить работу");
                string userInput = Console.ReadLine();
                int menuKey = int.TryParse(userInput, out int parsedMenuKey) ? parsedMenuKey : 0;
                switch (menuKey)
                {

                    case 1:
                        if (!Loading.IsCompleted) { Console.WriteLine($"Загрузка файла еще не завершена, функция не доступна! Загружено: {abonentStorage.progress}%. Выход в меню.."); break; }
                       
                        try
                        {
                            abonentStorage.PrintByPhoneNumber();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        break;
                    case 2:
                        abonentStorage.AddAbonent();
                        break;
                    case 3:
                        abonentStorage.ChangeAbonent();
                        break;
                    case 4:
                        abonentStorage.DeleteAbonent();
                        break;
                    case 5:
                        if (!Loading.IsCompleted) { Console.WriteLine($"Загрузка файла еще не завершена! Загружено: {abonentStorage.progress}%. Выход в меню.."); break; }
                        Console.WriteLine("Сохранение изменений и завершение работы...");
                        try
                        {
                            abonentStorage.WriteToBinary();
                        }
                        catch (Exception)
                        {
                            Console.WriteLine("Ошибка! Попробуйте снова!");
                            break;
                        }
                        Console.WriteLine("Успешно сохранено! Выход...");
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
        
