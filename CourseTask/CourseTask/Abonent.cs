using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace CourseTask
{
    public interface IOutput
    {
        void PrintInfo(string message);
    }
    public class OutputToFile: IOutput
    {
        public readonly string _path;
        public OutputToFile(string path)
        {
            _path = path;
        }
        public void PrintInfo(string message)
        {
            StreamWriter filterStream = File.AppendText(_path);
            filterStream.WriteLine(message);
            filterStream.Close();
        }
    }
    public class OutputToConsole: IOutput
    {
        public void PrintInfo(string message) => Console.WriteLine(message);
    } 
    public class Abonent
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Patronymic { get; set; }
        public string PhoneNumber { get; }
        public string City { get; set; }
        public Abonent(string name, string surname, string patronymic, string phoneNumber, string city) {
            Name = name;
            Surname = surname;
            Patronymic = patronymic;
            PhoneNumber = phoneNumber;
            City = city;
        }
        public void GetInfo(IOutput output)
        {
            output.PrintInfo($"ФИО: {Surname} {Name} {Patronymic}; Номер телефона: {PhoneNumber}; Город проживания: {City}");
        }
    }
}
