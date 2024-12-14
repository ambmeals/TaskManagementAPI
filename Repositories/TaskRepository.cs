using Task = TaskManagementAPI.Models.Task;

namespace TaskManagementAPI.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly List<Task> _tasks;

        public TaskRepository()
        {
            _tasks =
            [
                new()
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

                new()
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
            ];
        }

        public IEnumerable<Task> GetTasks()
        {
            return _tasks;
        }

        public Task? GetTaskById(string id)
        {
            if (id != null)
                return _tasks.FirstOrDefault(t => t.Id == id);

            return null;
        }

        public void AddTask(Task task)
        {
            if (task != null)
                _tasks.Add(task);
        }

        public void UpdateTask(Task taskToUpdate)
        {
            if (taskToUpdate != null)
            {
                var existingTask = _tasks
                    .FirstOrDefault(t => t.Id == taskToUpdate.Id);

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
            if (id != null)
                _tasks.RemoveAll(t => t.Id == id);
        }
    }

    public interface ITaskRepository
    {
        IEnumerable<Task> GetTasks();
        Task? GetTaskById(string id);
        void AddTask(Task task);
        void UpdateTask(Task taskToUpdate);
        void DeleteTask(string id);
    }
}
