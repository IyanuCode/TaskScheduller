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
            public DateTime DateCreated { get; set; }
            public DateTime Deadline { get; set; }
            public bool IsCompleted { get; set; }
        }
        private static List<Task> tasks = new List<Task>();
        private const string filePath = "task.txt";

        public static void Main(string[] args)
        {
            LoadFromFile(); //i think calling it here once and available to all other method is better for the memory than calling it in each method

            MainMenu();
        }

        /*------------------- Main Menu Method------------I*/
        private static void MainMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== TASK SCHEDULER ===");
                Console.WriteLine("To Add New Task, Press 1");
                Console.WriteLine("To View Pending Task, Press 2");
                Console.WriteLine("To View Completed Task, Press 3");
                Console.WriteLine("To Change Task Status, Press 4");
                Console.WriteLine("To Update Task, Press 5");
                Console.WriteLine("To Exit, Press 0");
                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddTask();
                        break;
                    case "2":
                        ViewPendingTask();
                        break;
                    case "3":
                        ViewCompletedTask();
                        break;
                    case "4":
                        ChangeTaskStatus();
                        break;
                    case "5":
                        UpdateTask();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Invalid choice, Press Enter to try again");
                        Console.ReadLine();
                        break;
                }
            }
        }
        /*---------------------- Main Menu Method Ends----------------I*/




        /*---------------------- Add Task Method-------------------I*/
        private static void AddTask()
        {
            Console.Clear();
            Console.WriteLine("=== ADD NEW TASK ===");

            //Task Name input with validation untill user enters the right name
            string? nameFromInput;
            do
            {
                Console.Write("Enter task name: ");
                nameFromInput = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(nameFromInput) || nameFromInput.Any(Char.IsDigit))
                {
                    Console.WriteLine("Name cant be empty and please enter a name without number");
                }
            } while (string.IsNullOrWhiteSpace(nameFromInput) || nameFromInput.Any(Char.IsDigit));


            //Description
            string? description;
            do
            {
                Console.Write("Enter task description (min 10 characters): ");
                description = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(description) || description.Length < 10)
                {
                    Console.WriteLine("Description is too short! Try Again!!!");
                }
            } while (string.IsNullOrWhiteSpace(description) || description.Length < 10);


            //Date Deadline input
            DateTime deadline;

            while (true)
            {
                Console.Write("Enter deadline (yyyy-mm-dd): ");
                string? dateInput = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(dateInput) && DateTime.TryParse(dateInput, out deadline) && deadline >= DateTime.Today)
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid or Past Date");
                }
            }

            //storing each task
            Task task = new Task
            {
                Name = nameFromInput,
                Description = description,
                Deadline = deadline,
                DateCreated = DateTime.Now,
                IsCompleted = false
            };
            tasks.Add(task);
            SaveTask(tasks); //passing the full list to save task so it can be consistently saved
            GoBackToMenu();
        }
        /*------------------- Add Task Ends------------I*/





        /*------------------- Save Task Method------------I*/

        private static void SaveTask(List<Task> tasks)
        {
            //Console.WriteLine("Inside SaveTask");
            try
            {
                using (var writer = new StreamWriter("task.txt"))
                {
                    writer.WriteLine($"{"NAME",-22}|{"DESCRIPTION",-37}|{"CREATED DATE",-15}|{"DUE DATE",-15}|{"COMPLETED",-12}");
                    writer.WriteLine(new string('-', 110));

                    foreach (var task in tasks)
                    {
                        string desc = task.Description.Length > 35 ? task.Description.Substring(0, 34) + "..." : task.Description;
                        string completedStatus = task.IsCompleted ? "Yes" : "No";

                        writer.WriteLine($"{task.Name,-22}|{desc,-37}|{task.DateCreated:yyyy-MM-dd, -15}|{task.Deadline:yyyy-MM-dd, -15}|{completedStatus,-12} ");

                    }
                }
                Console.WriteLine("Task saved to file successfully");
            }
            catch (Exception ex)
            {

                Console.WriteLine($"An error occured while writing to the file: {ex.Message}");
            }
        }


        /*------------------- Save Task End------------I*/




        /*------------------- LoadFromFile Method------------I*/
        private static void LoadFromFile()
        {
            tasks.Clear();
            
            if (!File.Exists(filePath))
            {
                Console.WriteLine("Task file not found therefore cannot be load from file.");
                return;
            }
        
            string[] lines = File.ReadAllLines(filePath);


            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                string[] parts = line.Split('|');
                
                if (parts.Length >= 5)
                {
                    tasks.Add(new Task
                    {
                        Name = parts[0].Trim(),
                        Description = parts[1].Trim(),
                        DateCreated = DateTime.TryParse(parts[2], out var created) ? created : DateTime.Now,
                        Deadline = DateTime.TryParse(parts[3], out var deadline) ? deadline : DateTime.Now,
                        IsCompleted = parts[4].Trim().ToLower() == "yes" ? true : false

                    });
                }

            }
            Console.WriteLine($"Tasks loaded: {tasks.Count}");
        }

        /*------------------- UpdateTask Method------------I*/
        public static void UpdateTask()
        {
            //LoadFromFile();
            Console.WriteLine("=== UPDATE TASK ===");
            string? nameToUpdate;
            string? newTaskName;
            string? newDescription;
            string? newDeadline;
            DateTime parsedDeadline;

            //task name to search for
            do
            {
                Console.Write("Enter name of task to update: ");
                nameToUpdate = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(nameToUpdate) || nameToUpdate.Any(Char.IsDigit) || nameToUpdate.Any(Char.IsSymbol))
                {
                    Console.WriteLine("Name cannot be empty and should be pure text");
                }
            }
            while (string.IsNullOrWhiteSpace(nameToUpdate) || nameToUpdate.Any(Char.IsDigit) || nameToUpdate.Any(Char.IsSymbol));



            // new name the user want to set
            do
            {
                Console.Write("Enter the new task name or press Enter to retain the current name: ");
                newTaskName = Console.ReadLine();


                if (string.IsNullOrWhiteSpace(newTaskName)) break;

                if (newTaskName.Any(Char.IsDigit) || newTaskName.Any(Char.IsSymbol))
                {
                    Console.WriteLine("Name should not contain digits or symbols.");
                    newTaskName = null;
                }

            } while (newTaskName == null);


            //description update
            do
            {
                Console.Write("Enter description of task or press Enter to retain the current descripton: ");
                newDescription = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(newDescription)) break;
                if (newDescription.Length < 10 || newDescription.Any(Char.IsDigit) || newDescription.Any(Char.IsSymbol))
                {
                    Console.WriteLine("Min of 10 characters and description should be text");
                }
            } while (newDescription.Length < 10 || newDescription.Any(Char.IsDigit) || newDescription.Any(Char.IsSymbol));

            //deadline update
            do
            {
                Console.Write("Enter new deadline (YYYY-MM-DD) or press Enter to retain the current deadline:: ");
                newDeadline = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(newDeadline)) break;
            } while (!DateTime.TryParse(newDeadline, out parsedDeadline) || parsedDeadline < DateTime.Today);


            //storing the value
            var task = tasks.FirstOrDefault(t => t.Name.Equals(nameToUpdate, StringComparison.OrdinalIgnoreCase));
            if (task != null)
            {
                task.Name = string.IsNullOrEmpty(newTaskName) ? task.Name : newTaskName;
                task.Description = string.IsNullOrEmpty(newDescription) ? task.Description : newDescription;
                if (!string.IsNullOrEmpty(newDeadline) && DateTime.TryParse(newDeadline, out parsedDeadline) && parsedDeadline >= DateTime.Today)
                {
                    task.Deadline = parsedDeadline;
                }

                SaveTask(tasks);
                Console.WriteLine("Task updated successfully");
                Console.WriteLine("The updated record is: ");
                Console.WriteLine($"Task Name: {task.Name},\nDeadline: {task.Deadline:yyyy-MM-dd}, \nCompleted: {(task.IsCompleted ? "Yes" : "No")}");
            }
            else
            {
                Console.WriteLine("Task not found");
            }
            Console.WriteLine("Task updated successfully");
            GoBackToMenu();
        }
        /*------------------- UpdateTask Ends------------I*/






        /*------------------- ViewPending Task Methos------------I*/

        public static void ViewPendingTask()
        {
            Console.WriteLine("=== VIEW PENDING TASK ===");

            var pendingTask = tasks.Where(t => !t.IsCompleted).OrderBy(t => t.Deadline);

            if (!pendingTask.Any())
            {
                Console.WriteLine("No avaialble pending task");
                GoBackToMenu();
                return;
            }
            else
            {
                Console.WriteLine($"{"NAME",-22} | {"DESCRIPTION",-37} | {"CREATED DATE",-15} | {"DUE DATE",-15} | {"COMPLETED",-12}");
                Console.WriteLine(new string('-', 110));
                foreach (var task in pendingTask)
                {
                    Console.WriteLine($"{task.Name,-22} | {task.Description,-37} | {task.DateCreated,-15:yyyy-MM-dd} | {task.Deadline,-15} | {(task.IsCompleted ? "Yes" : "No"),-12}");
                }
            }
            GoBackToMenu();

        }
        /*------------------- UpdateTask Ends------------I*/




        /*------------------- ViewPending Task Methos------------I*/

        public static void ViewCompletedTask()
        {
            Console.WriteLine("=== VIEW COMPLETED TASK ===");

            var completedTask = tasks.Where(t => t.IsCompleted).OrderBy(t => t.Deadline);

            if (!completedTask.Any())
            {
                Console.WriteLine("No completed task");
                GoBackToMenu();
                return;
            }
            else
            {
                Console.WriteLine($"{"NAME",-22} | {"DESCRIPTION",-37} | {"CREATED DATE",-15} | {"DUE DATE",-15} | {"COMPLETED",-12}");
                Console.WriteLine(new string('-', 110));
                foreach (var task in completedTask)
                {
                    Console.WriteLine($"{task.Name,-22} | {task.Description,-37} | {task.DateCreated,-15:yyyy-MM-dd} | {task.Deadline,-15} | {(task.IsCompleted ? "Yes" : "No"),-12}");
                }
            }
            GoBackToMenu();

        }
        /*------------------- UpdateTask Ends------------I*/




        /*------------------- ChangeTaskStatus------------I*/
        public static void ChangeTaskStatus()
        {
            Console.WriteLine("=== MARK TASK AS COMPLETED ===");

            Console.Write("Enter the name of the task to change status : ");
            string? name = Console.ReadLine();

            var task = tasks.FirstOrDefault(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (task != null)
            {
                task.IsCompleted = !task.IsCompleted;

                SaveTask(tasks);

                Console.WriteLine($"Task '{task.Name}' has been {(task.IsCompleted ? "marked as completed" : "marked as pending ")}.");
            }
            else
            {
                Console.WriteLine("Task not found.");
                GoBackToMenu();
            }

            Console.WriteLine("Status changed successfully");
            GoBackToMenu();

        }
        /*------------------- Change Task Status Ends------------I*/


        public static void GoBackToMenu()
        {
            Console.WriteLine("Press enter to return to main menu");
            Console.ReadLine();
        }

    }

  
    
    
}

