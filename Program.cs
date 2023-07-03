using System;
using System.Collections.Generic;

interface ITaskUnit
{

    string Name { get; set; }
    string Description { get; set; }
    DateTime DueDate { get; set; }
}

class TaskUnit : ITaskUnit
{
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime DueDate { get; set; }
}

delegate void Notify(ITaskUnit task);

static class Notifier
{
    public static void SendSms(ITaskUnit task)
    {
        Console.WriteLine($"Vazifa uchun SMS xabar yuborilmoqda: {task.Name}");
    }

    public static void SendEmail(ITaskUnit task)
    {
        Console.WriteLine($"Vazifa uchun elektron pochta xabari yuborilmoqda: {task.Name}");
    }
}

class TaskTracker<T> where T : ITaskUnit
{
    private List<T> tasks;

    public TaskTracker()
    {
        tasks = new List<T>();
    }

    public void Add(T task)
    {
        tasks.Add(task);
    }

    public void Remove(T task)
    {
        tasks.Remove(task);
    }

    public IEnumerable<T> LatestTasks()
    {
        return tasks.OrderBy(t => t.DueDate);
    }

    public int TasksNumber()
    {
        return tasks.Count;
    }

    public void PrintClosestDueTasks()
    {
        var closestTasks = tasks.OrderBy(t => t.DueDate).Take(5);

        foreach (var task in closestTasks)
        {
            Console.WriteLine($"{task.Name} - Muddati: {task.DueDate}");
        }
    }
}

class Program
{

    static void RunCommand(TaskTracker<TaskUnit> tracker, string[] command)
    {
        if (command[0] == "Add" && command.Length == 5)
        {
            string name = command[1];
            string description = command[2];
            DateTime dueDate;
            if (DateTime.TryParse(command[3], out dueDate))
            {
                tracker.Add(new TaskUnit { Name = name, Description = description, DueDate = dueDate });
                Console.WriteLine("Vazifa muvaffaqiyatli qo'shildi.");
            }
            else
            {
                Console.WriteLine("Noto'g'ri muddat formati.");
            }
        }
        else if (command[0] == "Remove" && command.Length == 2)
        {
            string name = command[1];
            var task = tracker.LatestTasks().FirstOrDefault(t => t.Name == name);
            if (task != null)
            {
                tracker.Remove(task);
                Console.WriteLine("Vazifa muvaffaqiyatli olib tashlandi.");
            }
            else
            {
                Console.WriteLine("Bunday nomli vazifa topilmadi.");
            }
        }
        else if (command[0] == "PrintClosestDueTasks" && command.Length == 1)
        {
            tracker.PrintClosestDueTasks();
        }
        else
        {
            Console.WriteLine("Noto'g'ri buyruq.");
        }
    }

    static void Main(string[] args)
    {
        TaskTracker<TaskUnit> tracker = new TaskTracker<TaskUnit>();

        // Test ma'lumotlar
        tracker.Add(new TaskUnit { Name = "Vazifa 1", Description = "Tavsif 1", DueDate = DateTime.Now.AddDays(2) });
        tracker.Add(new TaskUnit { Name = "Vazifa 2", Description = "Tavsif 2", DueDate = DateTime.Now.AddDays(3) });
        tracker.Add(new TaskUnit { Name = "Vazifa 3", Description = "Tavsif 3", DueDate = DateTime.Now.AddDays(1) });
        tracker.Add(new TaskUnit { Name = "Vazifa 4", Description = "Tavsif 4", DueDate = DateTime.Now.AddDays(5) });
        tracker.Add(new TaskUnit { Name = "Vazifa 5", Description = "Tavsif 5", DueDate = DateTime.Now.AddDays(4) });

        tracker.PrintClosestDueTasks();

        string input;
        do
        {
            Console.WriteLine("Buyruq kiriting ('Exit' deb yozib chiqish uchun):");
            input = Console.ReadLine();
            var command = input.Split(' ');
            RunCommand(tracker, command);
            Console.WriteLine();
        } while (input != "Exit");

    }
}
