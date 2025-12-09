using DotMake.CommandLine;
using GeneratorUML.Database;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

public class Program
{
    static List<string> atributes = new List<string>();

    static void Main(string[] args)
    {
        Type type = typeof(Context);
        Type? s = null;

        foreach (PropertyInfo field in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
        {
            if (field.PropertyType.IsGenericType && field.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
            {
                s = field.PropertyType.GetGenericArguments()[0];
                Console.WriteLine($"Таблица {s.Name}");

                GetUmlClass(s);
            }
            Console.WriteLine("\n\n");
        }
        CreateUml();
    }

    static void GetUmlClass(Type type)
    {
        Console.WriteLine("Поля таблицы:");
        atributes.Add("class " + type.Name + "{");
        foreach (MemberInfo member in type.GetMembers(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance))
        {
            if (member.MemberType == MemberTypes.Property)
            {
                PropertyInfo info = (PropertyInfo)member;
                Console.WriteLine($"{member.Name} {info.PropertyType.Name}");
                atributes.Add($"+{info.PropertyType.Name} {member.Name}"); // добавить атрибуты
            }
        }
        atributes.Add("}\n");
        Console.WriteLine("\n\n");
    }

    static void CreateUml()
    {
        string path = "C:/Users/dd3ni/Desktop/Производственная/CLIGeneratorUML/GeneratorUML/GeneratedUML/ErDiagramm.puml";

        using (StreamWriter writer = new StreamWriter(path, true))
        {
            writer.WriteLine("@startuml");

            foreach (var s in atributes) 
            {
                writer.WriteLine(s);
            }
            writer.WriteLine("@enduml");
            atributes.Clear();
        }
    }
}

public class Person
{
    public string Name { get; }
    public Person(string name) => Name = name;
}

[CliCommand(Description = "Перевод строки в верхний регистр")]
public class CopyString
{
    [CliArgument(Description = "Создать файл uml")]
    public string Create { get; set; }
    public void Run()
    {
        string path = $@"./GeneratedUML/ErDiagramm.puml";
        FileStream? fstream = null;

        if (Create == "create")
        {
            fstream = new FileStream(path, FileMode.Create);
            Console.WriteLine("Файл UML создан!");
        }
        else
            Console.WriteLine("Введена некорректная команда");
    }
}