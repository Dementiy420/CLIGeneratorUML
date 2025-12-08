using GeneratorUML.Database;
using DotMake.CommandLine;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

public class Program 
{
    static void Main(string[] args)
    {
        Type type = typeof(Context);

        foreach (PropertyInfo field in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)) 
        {
            Type s;
            Console.WriteLine($"{field.PropertyType.Name} || {field.Name}");
            if (field.PropertyType.IsGenericType && field.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
            {
                s = field.PropertyType.GetGenericArguments()[0];
                Console.WriteLine(s.Name);
            }

        }
    }
}

public class Person 
{
    public string Name { get;}
    public Person (string name) => Name = name;
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