using Task = TaskManagementAPI.Models.Task;

namespace TaskManagementAPI.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private static readonly List<Task> _tasks = new()
        {
            new Task
            {
                Id = Guid.NewGuid().ToString(),
                Title = "Task 1",
                Description = "This is the first task",
                Priority = "HIGH",
                Status = "TODO",
                DueDate = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Task
            {
                Id = Guid.NewGuid().ToString(),
                Title = "Task 2",
                Description = "This is the second task",
                Priority = "MEDIUM",
                Status = "IN_PROGRESS",
                DueDate = DateTime.UtcNow.AddDays(5),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        public IEnumerable<Task> GetTasks()
        {
            return _tasks.OrderBy(t => t.DueDate.HasValue 
                    ? 0 
                    : 1) 
                .ThenBy(t => t.DueDate);
        }

        public Task? GetTaskById(string id) => 
            _tasks.FirstOrDefault(t => t.Id.Equals(id, StringComparison.OrdinalIgnoreCase));

        public void CreateTask(Task task)
        {
            if (task == null || string.IsNullOrWhiteSpace(task.Title))
                throw new ArgumentException("Invalid task");

            _tasks.Add(task);
        }

        public void UpdateTask(Task taskToUpdate)
        {
            if (taskToUpdate != null)
            {
                var existingTask = _tasks.FirstOrDefault(t => t.Id == taskToUpdate.Id);

                if (existingTask != null)
                {
                    existingTask.Title = taskToUpdate.Title;
                    existingTask.Description = taskToUpdate.Description;
                    existingTask.Priority = taskToUpdate.Priority;
                    existingTask.Status = taskToUpdate.Status;
                    existingTask.DueDate = taskToUpdate.DueDate;
                    existingTask.UpdatedAt = DateTime.UtcNow;
                }
            }
        }

        public void DeleteTask(string id)
        {
            _tasks.RemoveAll(t => t.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
        }
    }

    public interface ITaskRepository
    {
        IEnumerable<Task> GetTasks();
        Task? GetTaskById(string id);
        void CreateTask(Task task);
        void UpdateTask(Task taskToUpdate);
        void DeleteTask(string id);
    }
}
