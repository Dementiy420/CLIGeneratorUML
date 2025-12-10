using DotMake.CommandLine;
using GeneratorUML.Database;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

public class Program
{
    static List<string> atributes = new List<string>();
    static HashSet<Type> typesEntities = new HashSet<Type>();
    static Assembly asmbly = Assembly.LoadFrom("GeneratorUML.dll");

    static void Main(string[] args)
    {
        Type? type = null;
        Type? s = null;

        Console.WriteLine(asmbly.FullName);

        Type[] types = asmbly.GetTypes();

        foreach (Type type1 in types) 
        {
            if (type1.IsSubclassOf(typeof(DbContext)))
            {
                type = type1;
                Console.WriteLine("В проекте есть контекст с БД");
            }
            else
                continue;
        }
        foreach (PropertyInfo field in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
        {
            if (field.PropertyType.IsGenericType && field.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
            {
                s = field.PropertyType.GetGenericArguments()[0];                
                typesEntities.Add(s);
            }
        }

        foreach (var i in typesEntities) 
        {
            Console.WriteLine(i.Name);
        }


        foreach (PropertyInfo field in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
        {
            if (field.PropertyType.IsGenericType && field.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
            {
                s = field.PropertyType.GetGenericArguments()[0];
                //Console.WriteLine($"Таблица {s.Name}");
                typesEntities.Add(s);
                GetUmlClass(s);
            }

            GetEntityRelation(s);
            Console.WriteLine("\n\n");
        }

        CreateUml();
    }

    static void GetEntityRelation(Type type) 
    {

        foreach (var entity in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)) 
        {

            foreach (var t in typesEntities)
            {
                if (entity.PropertyType == t && !entity.PropertyType.IsGenericType)
                    atributes.Add($"{type.Name} \"1\" --- \"1\" {t.Name}");

                else if (entity.PropertyType.IsGenericType)
                {
                    if (entity.PropertyType.GetGenericArguments()[0].Name == t.Name)
                        atributes.Add($"{type.Name} \"1\" --- \"M\" {t.Name}");
                    else
                        continue;
                }
            }
        }
    }

    static void GetUmlClass(Type type)
    {
        //Console.WriteLine("Поля таблицы:");
        atributes.Add("class " + type.Name + "{");
        foreach (MemberInfo member in type.GetMembers(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance))
        {
            if (member.MemberType == MemberTypes.Property)
            {
                PropertyInfo info = (PropertyInfo)member;
                
                //Console.WriteLine($"{member.Name} {info.PropertyType.Name}");
                atributes.Add($"+{info.PropertyType.Name} {member.Name}"); // добавить атрибуты
            }
        }
        atributes.Add("}\n");
        Console.WriteLine("\n\n");
    }

    static void CreateUml()
    {
        string path = "C:/Users/dd3ni/Desktop/Производственная/CLIGeneratorUML/GeneratorUML/GeneratedUML/ErDiagramm.puml";

        using (StreamWriter writer = new StreamWriter(path, false))
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


//[CliCommand(Description = "Перевод строки в верхний регистр")]
//public class CopyString
//{
//    [CliArgument(Description = "Создать файл uml")]
//    public string Create { get; set; }
//    public void Run()
//    {
//        string path = $@"./GeneratedUML/ErDiagramm.puml";
//        FileStream? fstream = null;

//        if (Create == "create")
//        {
//            fstream = new FileStream(path, FileMode.Create);
//            Console.WriteLine("Файл UML создан!");
//        }
//        else
//            Console.WriteLine("Введена некорректная команда");
//    }
//}