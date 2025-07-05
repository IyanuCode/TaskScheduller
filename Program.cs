using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskScheduller
{
    public class Program
    {
        public class Task
        {
            public string? Name { get; set; }
            public string? Description { get; set; }
            public DateTime CreatedDate { get; set; }
            public DateTime DueDate { get; set; }
            public bool IsCompleted { get; set; }

            public string Status
            {
                get
                {
                    return IsCompleted ? "Completed" : "Pending";
                }
            }
        }


        private static List<Task> tasks = new List<Task>();
        private const string filePath = "task.txt";

        public static void Main(string[] args)
        {
            MainMenu();
        }
/* ----------------------------------------------------Main Menu----------------------------------------------*/
        private static void MainMenu()
        {
            while (true)
            {
                Console.WriteLine("\nWelcome To Task Scheduller App");
                Console.WriteLine("To Add Task, Press 1");
                Console.WriteLine("To View Task, Press 2");
                Console.WriteLine("To Update Task, Press 3");
                Console.WriteLine("To Change Task Status, Press 4");
                Console.WriteLine("To Exit, Press 0");
                Console.Write("Enter Your Choice: ");

                string? input = Console.ReadLine().Trim();

                if (string.IsNullOrEmpty(input))
                {
                    Console.WriteLine("Input cannot be empty.");
                }
                else
                {

                    switch (input)
                    {
                        case "1":
                            AddTask();
                            break;
                        case "2":
                            ViewTask();
                            break;
                        case "3":
                            UpdateTask();
                            break;
                        case "4":
                            ChangeTaskStatus();
                            break;
                        case "0":
                            Console.WriteLine("Exiting the application...");
                            return;
                        default:
                            Console.WriteLine("Invalid input, please try again.");
                            break;
                    }

                }
            }
        }
/* ----------------------------------------------------Main Menu Ends---------------------------------------------*/
       
       
       
/* ----------------------------------------------------Add Task----------------------------------------------*/       
        private static void AddTask()
        {
            Task task = new Task();

            //Task Name
            Console.Write("Enter Task Name:");
            string? inputTaskName = Console.ReadLine().Trim();
            if (!string.IsNullOrEmpty(inputTaskName))
            {
                task.Name = inputTaskName;
            }
            else
            {
                Console.WriteLine("Task name cannot be empty.");
            }

            //Task Description
            Console.Write("Enter Task Description:");
            string? inputDescription = Console.ReadLine().Trim();
            if (!string.IsNullOrEmpty(inputDescription))
            {
                task.Description = inputDescription;
            }
            else
            {
                Console.WriteLine("Task Description cannot be empty.");
            }

            //Task Created Date
            task.CreatedDate = DateTime.Now;

            //Task Due Date
            Console.Write("Enter Due Date in the format (yyyy-mm-dd):");
            if (DateTime.TryParse(Console.ReadLine(), out DateTime dueDate))
            {
                task.DueDate = dueDate;
                task.IsCompleted = false;
                tasks.Add(task);
                Console.WriteLine("Task added successfully!");
            }
            else
            {
                Console.WriteLine("Invalid date format.");
            }

            WriteToFile();
        }

/* -------------------------------------------------Enf of Add Task----------------------------------------------*/       



/* ----------------------------------------------------view Task----------------------------------------------*/       

        private static void ViewTask()
        {
            Console.Clear();
            LoadFromFile();
            Console.WriteLine("=== TASK LIST ===\n");

            if (tasks.Count == 0)
            {
                Console.WriteLine("No tasks available.");
                return;
            }
            else
            {
                Console.WriteLine($"{"NAME",-22}|{"DESCRIPTION",-37}|{"CREATED DATE",-15}|{"DUE DATE",-15}|{"STATUS",-12}");
                Console.WriteLine(new string('-', 107));



            }

            foreach (var task in tasks)
            {
                if (task.Name.StartsWith("NAME", StringComparison.OrdinalIgnoreCase) || task.Name.StartsWith("-")) continue;
                Console.WriteLine($"{task.Name,-22}|{task.Description,-37}|{task.DueDate,-15:yyyy-MM-dd}|{task.DueDate,-15}|{task.Status,-12}");
            }
        }
/* ----------------------------------------------------view Task Ends----------------------------------------------*/       

        private static void WriteToFile()
        {
            try
            {
                using (var writer = new System.IO.StreamWriter(filePath))
                {
                    writer.WriteLine($"{"NAME",-22}|{"DESCRIPTION",-37}|{"CREATED DATE",-15}|{"DUE DATE",-15}|{"COMPLETED",-12}");
                    writer.WriteLine(new string('-', 110));

                    foreach (var task in tasks)
                    {
                        writer.WriteLine($"{task.Name,-22}|{task.Description,-37}|{task.CreatedDate,-15:yyyy-MM-dd}|{task.DueDate:yyyy-MM-dd,-15}|{task.Status,-12}");
                    }
                }
                Console.WriteLine("Task saved to file successfully.");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while writing to the file: {ex.Message}");
            }
        }





/* ----------------------------------------------------Load From File----------------------------------------------*/      
        private static void LoadFromFile()
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine("File doesn't exist.");
                return;
            }

            string[] lines = File.ReadAllLines(filePath);


            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("Name") || line.StartsWith("-"))
                    continue;

                string[] parts = line.Trim().Split('|');

                if (parts.Length >= 5)
                {
                    tasks.Add(new Task
                    {
                        Name = parts[0].Trim(),
                        Description = parts[1].Trim(),
                        CreatedDate = DateTime.TryParse(parts[2], out DateTime created) ? created : DateTime.Now,
                        DueDate = DateTime.TryParse(parts[3], out DateTime due) ? due : DateTime.Now,
                        IsCompleted = parts.Length > 4 && bool.TryParse(parts[4], out bool done) ? done : false,
                        //Status
                    });
                }
                else
                {
                    Console.WriteLine($" Skipping malformed line: {line}");
                }
            }


            Console.WriteLine(" Tasks loaded successfully.");
        }
/* ----------------------------------------------------Load From File Ends----------------------------------------------*/      



/* ----------------------------------------------------Update Task----------------------------------------------*/      
private static void UpdateTask()
{
    Console.Clear();
    Console.Write("Enter the name of Task you want to search for: ");
    string? searchName = Console.ReadLine()?.Trim();

    if (string.IsNullOrWhiteSpace(searchName) || searchName.Length < 1)
    {
        Console.WriteLine("Enter a valid search name");
    }

    var taskFound = tasks.Where(p => p.Name != null && p.Name.Contains(searchName, StringComparison.OrdinalIgnoreCase)).ToList();
    Console.WriteLine($"task found just to see it in updatetask: {taskFound.Count} results");//(needs to be deleted)
    if (taskFound.Count > 0)
    {
        Console.WriteLine("\nSearch Results:\n");

        int counter = 1;
        foreach (var task in taskFound)
        {
            Console.WriteLine($"{counter++}. Name: {task.Name}");
            Console.WriteLine($"   Description: {task.Description}");
            Console.WriteLine($"   Created: {task.CreatedDate:yyyy-MM-dd}");
            Console.WriteLine($"   Due: {task.DueDate:yyyy-MM-dd}");
            Console.WriteLine($"   Status: {(task.IsCompleted ? "[x] Completed" : "[ ] Pending")}\n");
        }

         Console.Write("Enter the number of the task you want to update: ");
         if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 1 && choice <= taskFound.Count)
         {
             var selectedTask = taskFound[choice - 1];

             Console.Write($"\nNew name (or press Enter to keep '{selectedTask.Name}'): ");
             string newName = Console.ReadLine();
             if (!string.IsNullOrWhiteSpace(newName)) selectedTask.Name = newName;

             Console.Write("New description (or press Enter to keep current): ");
             string newDesc = Console.ReadLine();
             if (!string.IsNullOrWhiteSpace(newDesc)) selectedTask.Description = newDesc;

             Console.Write("New due date (yyyy-MM-dd) or press Enter to keep: ");
             string newDue = Console.ReadLine();
             if (DateTime.TryParse(newDue, out DateTime newDate)) selectedTask.DueDate = newDate;

             WriteToFile();
             Console.WriteLine("\n Task updated successfully!");
         }
         else
         {
             Console.WriteLine("Invalid selection.");
         }
    }
    else
    {
        Console.WriteLine("No task found matching that name.");
    }
}

/*---------------------------------------------------Change Task Status ---------------------------------------*/
    private static void ChangeTaskStatus(){
    Console.Clear();
    Console.Write("Enter the name of the Task: ");
    string? searchName = Console.ReadLine()?.Trim();

    if (string.IsNullOrWhiteSpace(searchName) || searchName.Length < 1)
    {
        Console.WriteLine("Enter a valid search name");
    }

        var taskFound = tasks.Where(p => p.Name != null && p.Name.Contains(searchName, StringComparison.OrdinalIgnoreCase)).ToList();
        Console.WriteLine($"the task found list: {taskFound.Count} results");//just to see the list(need to be removed)
        if (taskFound.Count > 0)
        {
            Console.WriteLine("\nSearch Results:\n");

            int counter = 1;
            foreach (var task in taskFound)
            {
                Console.WriteLine($"{counter++}. Name: {task.Name}");
                Console.WriteLine($"   Description: {task.Description}");
                Console.WriteLine($"   Created: {task.CreatedDate:yyyy-MM-dd}");
                Console.WriteLine($"   Due: {task.DueDate:yyyy-MM-dd}");
                Console.WriteLine($"   Status: {(task.IsCompleted ? "[x] Completed" : "[ ] Pending")}\n");
            }

            Console.Write("Enter the serial number of the task you want to Change its status: ");
            if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 1 && choice <= taskFound.Count)
            {
                var selectedTask = taskFound[choice - 1];

                Console.WriteLine("\nPress [Spacebar] to toggle completion status.");
                Console.WriteLine("Or press [Enter] to leave it unchanged.");
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Spacebar)
                {
                    selectedTask.IsCompleted = !selectedTask.IsCompleted;
                    Console.WriteLine($"\nStatus updated: {(selectedTask.IsCompleted ? "[x] Completed" : "[ ] Pending")}");
                }
                WriteToFile();
                Console.WriteLine("Task Status updated Successfully");
            }
            else{
                Console.WriteLine("Task not updated successfully");
            }
        }
        else{
            Console.WriteLine("No Task Found ");
        }



        //    private static string ToggleCheckBox(Task task)
        //     {
        //         ConsoleKeyInfo key = Console.ReadKey(true);
        //         if (key.Key == ConsoleKey.Spacebar)
        //         {
        //             task.IsCompleted = !task.IsCompleted;
        //         }
        //         return task.IsCompleted ? "[x]" : "[ ]";
        //     }
    }
}
}

