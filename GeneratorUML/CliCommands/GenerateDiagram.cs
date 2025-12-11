using DotMake.CommandLine;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace GeneratorUML.CliCommands
{
    [CliCommand(Description = "Создание ER-диаграммы в PlantUML")]
    public class GenerateDiagram
    {
        List<string> _metadataUml = new List<string>(); // буфер для формирования UML
        HashSet<Type> _entities = new HashSet<Type>(); // список сущностей, которые есть в классе контекста  

        [CliOption(Description = "Путь к сборке проекта")] public string Path { get; set; }

        [CliOption(Description = "Путь сохранения файла")] public string Save { get; set; }

        [CliArgument(Description = "Название файла диаграммы")] public string Umlname { get; set; } = "diagram";

        public void Run()
        {
            Type? contextType = null!;

            Type? entityType = null!;

            Assembly asmbly = Assembly.LoadFrom($"{Path}");

            Type[] typesInApp = asmbly.GetTypes();

            foreach (Type type1 in typesInApp) // поиск класса, который содержит в себе DBContext
            {
                if (type1.IsSubclassOf(typeof(DbContext)))
                {
                    contextType = type1;
                    Console.WriteLine("Класс, реализующий контекст: " + contextType.Name + "\n");
                }
                else
                    continue;
            }

            foreach (PropertyInfo field in contextType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)) //Поиск полей DBSet<>
            {
                if (field.PropertyType.IsGenericType && field.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
                {
                    entityType = field.PropertyType.GetGenericArguments()[0];
                    _entities.Add(entityType); // сохранение в список сущностей
                }
            }

            Console.WriteLine("Список сущностей: ");

            foreach (var i in _entities)
            {
                Console.WriteLine(i.Name);
            }

            Console.WriteLine("\n\n");

            foreach (PropertyInfo field in contextType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)) //Формирование данных для UML
            {
                if (field.PropertyType.IsGenericType && field.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
                {
                    entityType = field.PropertyType.GetGenericArguments()[0];
                    GetUmlClass(entityType);
                }
                GetEntityRelation(entityType);
            }

            CreateUml();
        }
        private void GetEntityRelation(Type type) //Заполнение отношений между сущностями
        {
            foreach (var entity in type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
            {
                foreach (Type t in _entities)
                {
                    if (entity.PropertyType == t && !entity.PropertyType.IsGenericType) // при связи 1 к 1
                    {
                        _metadataUml.Add($"{type.Name} \"1\" --- \"1\" {t.Name}"); // добавить в буфер связь
                    }
                    else if (entity.PropertyType.IsGenericType && entity.PropertyType.GetGenericArguments()[0].Name == t.Name) // при связи 1 к M
                    {
                        _metadataUml.Add($"{type.Name} \"1\" --- \"M\" {t.Name}"); // добавить в буфер связь
                    }
                }
            }
        }

        private void GetUmlClass(Type type) // Заполнение класса UML
        {
            _metadataUml.Add("class " + type.Name + "{");
            foreach (MemberInfo member in type.GetMembers(BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Instance))
            {
                if (member.MemberType == MemberTypes.Property)
                {
                    PropertyInfo info = (PropertyInfo)member;

                    _metadataUml.Add($"+{info.PropertyType.Name} {member.Name}"); // добавить в буфер класс
                }
            }
            _metadataUml.Add("}\n");
        }

        private void CreateUml() //Создание и запись файла в формате .puml
        {
            string path = Save + $"/{Umlname}.puml";

            using (StreamWriter writer = new StreamWriter(path, false))
            {
                writer.WriteLine("@startuml");

                foreach (var s in _metadataUml)
                {
                    writer.WriteLine(s);
                }
                writer.WriteLine("@enduml");
                _metadataUml.Clear(); // очистка буфера после записи
            }

            Console.WriteLine("Файл с UML данными был создан по пути: " + path);
        }
    }
}