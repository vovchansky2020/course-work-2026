using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CourseTask
{
    class AbonentStorage
    {
        public int _errorCounter = 0;
        public int progress = 0;
        SortedSet<string> AbonentNumbers = new SortedSet<string>();
        Dictionary<string, Abonent> Abonents = new Dictionary<string, Abonent>();
        OutputToConsole console = new OutputToConsole();
        OutputToFile file = new OutputToFile("Filtered.txt");
        private readonly object _lock = new object();
        private readonly object _errorLock = new object();

        private void ErrorProtocol(Abonent abonent1, Abonent abonent2)
        {
            _errorCounter += 1;
            StreamWriter errorStream = File.AppendText("error.txt");
            errorStream.WriteLine($"Конфликт номер {_errorCounter}");
            errorStream.WriteLine($"Конфликт номеров: {abonent1.PhoneNumber}");
            errorStream.WriteLine($"   Существующий: {abonent1.Surname} {abonent1.Name} {abonent1.Patronymic}, {abonent1.City}");
            errorStream.WriteLine($"   Дубликат:     {abonent2.Surname} {abonent2.Name} {abonent2.Patronymic}, {abonent2.City}");
            errorStream.WriteLine();
            errorStream.Close();
        }
        public Abonent FindByPhoneNumber(string phoneNumber)
        {
            lock (_lock)
            {
                return Abonents.ContainsKey(phoneNumber) ? Abonents[phoneNumber] : null;
            }
        } 
        public void PrintByPhoneNumber()
        {
            Console.Write("Введите первые 4 цифры номера, по которым вы хотите отсортировать абонентов: ");
            string filter = Console.ReadLine();
            if (filter.Length == 4)
            {
                int amount = 0;
                lock (_lock)
                {
                    List<string> FilteredNumbers = AbonentNumbers.GetViewBetween(filter + "0000000", filter + "9999999").ToList();
                    foreach (var num in FilteredNumbers)
                    {
                        Abonents[num].GetInfo(file);
                        amount += 1;
                    }
                }
                Console.WriteLine($"Процесс завершен! Найдено: {amount} абонентов.");
                Console.WriteLine();
            }
            else
            {
                throw new Exception("Введено количество цифр, не равное 4м. Выход в меню! ");
            }
        }
        public void ReadBinary()
        {
            
            FileStream fs = new FileStream("data.bin", FileMode.OpenOrCreate);
            BinaryReader binaryReader = new BinaryReader(fs);
            string name, surname, patronymic, city, phoneNumber;
            while (binaryReader.BaseStream.Position < binaryReader.BaseStream.Length)
            {
                Thread.Sleep(3000);
                name = binaryReader.ReadString();
                surname = binaryReader.ReadString();
                patronymic = binaryReader.ReadString();
                phoneNumber = binaryReader.ReadString();
                city = binaryReader.ReadString();
                lock (_lock)
                {
                    if (!AbonentNumbers.Contains(phoneNumber))
                    {
                        AbonentNumbers.Add(phoneNumber);
                        Abonents.Add(phoneNumber, new Abonent(name, surname, patronymic, phoneNumber, city));
                    }
                    else
                    {
                        lock (_errorLock)
                        {
                            ErrorProtocol(Abonents[phoneNumber], new Abonent(name, surname, patronymic, phoneNumber, city));
                        }
                    }
                }
                progress = (int)(binaryReader.BaseStream.Position * 100 / binaryReader.BaseStream.Length) ;
            }
            fs.Close();
        }
        public async Task ReadBinaryAsync()
        {
            await Task.Run(() => ReadBinary());
        }
        public void WriteToBinary()
        {
            FileStream fs = new FileStream("data.bin.tmp", FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);
            foreach (string phone in AbonentNumbers)
            {
                Abonent _abonent = Abonents[phone];
                bw.Write(_abonent.Name);
                bw.Write(_abonent.Surname);
                bw.Write(_abonent.Patronymic);
                bw.Write(_abonent.PhoneNumber);
                bw.Write(_abonent.City);

            }
            fs.Close();
            File.Replace("data.bin.tmp", "data.bin", null, false);
        }
        public void AddAbonent()
        {
            Console.WriteLine("Вы создаете нового абонента: ");
            string surname = ConsoleUI.ReadNonEmptyString("Введите фамилию: ");
            string name = ConsoleUI.ReadNonEmptyString("Введите имя: ");
            string patronymic = ConsoleUI.ReadNonEmptyString("Введите отчество при наличии, иначе '-' : ");
            string phoneNumber = ConsoleUI.ReadPhoneNumber("Введите номер телефона: ");
            string city = ConsoleUI.ReadNonEmptyString("Введите город, в котором проживает абонент: ");
            try 
            {   
                lock (_lock)
                {
                    Abonents.Add(phoneNumber, new Abonent(name, surname, patronymic, phoneNumber, city));
                    AbonentNumbers.Add(phoneNumber);
                }
            }
            catch (ArgumentException)
            {
                lock (_errorLock)
                {
                    ErrorProtocol(Abonents[phoneNumber], new Abonent(name, surname, patronymic, phoneNumber, city));
                }
                Console.WriteLine("Номер уже занят другим абонентом! Ошибка зафиксирована! Выход в главное меню!");
                Console.WriteLine("");
                return;
            }
            catch (Exception)
            {
                Console.WriteLine("Неизвестная ошибка! Выход в главное меню");
                Console.WriteLine("");
                return;
            }
            Console.WriteLine("Абонент был успешно создан!");
            Console.WriteLine("");
        }
        public void ChangeAbonent()
        {
            string number = ConsoleUI.ReadPhoneNumber("Введите номер телефона абонента, информацию о котором вы хотели бы изменить: ");
            lock (_lock)
            {
                if (Abonents.ContainsKey(number))
                {
                    try
                    {
                        ConsoleUI.ChangingMenu(FindByPhoneNumber(number));
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Не вышло! Неизвестная ошибка!");
                        return;
                    }
                    Console.WriteLine("Абонент успешно отредактирован!");
                    Console.WriteLine("");
                }
                else
                {
                    Console.WriteLine($"Запрашиваемый на изменение абонент не найден! Загружено: {progress}% ");
                }
            }
        }
        public void DeleteAbonent()
        {
            string number = ConsoleUI.ReadPhoneNumber("Введите номер телефона абонента, информацию о котором вы хотели бы удалить: ");
            lock (_lock)
            {
                if (Abonents.ContainsKey(number))
                {
                    try
                    {
                        lock (_lock)
                        {
                            Abonents.Remove(number);
                            AbonentNumbers.Remove(number);
                        }
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Не вышло! Неизвестная ошибка!");
                        return;
                    }
                    Console.WriteLine("Абонент успешно удален!");
                    Console.WriteLine("");
                }
                else
                {
                    Console.WriteLine($"Запрашиваемый на удаление абонент не найден! Загружено: {progress}% ");
                }
            }
        }
    }
}
