namespace TaskManagementAPI.Repositories
{
    public static class TaskRepository
    {
        public static List<Models.Task> Tasks = new List<TaskManagementAPI.Models.Task>
        {
            new TaskManagementAPI.Models.Task
            {
                Title = "Task 1",
                Description = "This is the first task",
                Priority = "HIGH",
                Status = "TODO",
                DueDate = DateTime.UtcNow.AddDays(7)
            },
            new TaskManagementAPI.Models.Task
            {
                Title = "Task 2",
                Description = "This is the second task",
                Priority = "MEDIUM",
                Status = "IN_PROGRESS",
                DueDate = DateTime.UtcNow.AddDays(5)
            }
        };
    }
}