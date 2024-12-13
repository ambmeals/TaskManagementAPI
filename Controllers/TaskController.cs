using Microsoft.AspNetCore.Mvc;
using TaskManagementAPI.Repositories;
using Task = TaskManagementAPI.Models.Task;

namespace TaskManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAllTasks()
        {
            return Ok(TaskRepository.Tasks);
        }

        [HttpGet("{id}")]
        public IActionResult GetTaskById(string id)
        {
            var task = TaskRepository.Tasks
                .FirstOrDefault(t => t.Id == id);

            if (task != null)
                return Ok(task);

            return NotFound(new { message = "Task not found" });
        }

        [HttpPost]
        public IActionResult CreateTask([FromBody] Task newTask)
        {
            newTask.Id = Guid.NewGuid().ToString();
            newTask.CreatedAt = DateTime.UtcNow;
            newTask.UpdatedAt = DateTime.UtcNow;

            TaskRepository.Tasks.Add(newTask);

            return CreatedAtAction(nameof(GetTaskById), new { id = newTask.Id }, newTask);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateTask(string id, [FromBody] Task updatedTask)
        {
            var task = TaskRepository.Tasks
                .FirstOrDefault(t => t.Id == id);

            if (task != null)
            {
                task.Title = updatedTask.Title;
                task.Description = updatedTask.Description;
                task.Priority = updatedTask.Priority;
                task.Status = updatedTask.Status;
                task.DueDate = updatedTask.DueDate;
                task.UpdatedAt = DateTime.UtcNow;

                return NoContent();
            }

            return NotFound(new { message = "Task not found" });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteTask(string id)
        {
            var task = TaskRepository.Tasks
                .FirstOrDefault(t => t.Id == id);

            if (task != null)
            {
                TaskRepository.Tasks.Remove(task);
                return NoContent();
            }

            return NotFound(new { message = "Task not found" });
        }
    }
}