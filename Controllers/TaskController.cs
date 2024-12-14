using Microsoft.AspNetCore.Mvc;
using TaskManagementAPI.Repositories;
using Task = TaskManagementAPI.Models.Task;

namespace TaskManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly ITaskRepository _taskRepository;

        public TaskController(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetAllTasks() => Ok(_taskRepository.GetTasks());

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetTaskById(string id)
        {
            var task = _taskRepository.GetTaskById(id);

            if (task == null)
                return NotFound(new { message = "Task not found" });

            return Ok(task);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult CreateTask([FromBody] Task newTask)
        {
            if (string.IsNullOrWhiteSpace(newTask.Title))
                return BadRequest(new { message = "Title is required." });

            newTask.Id = Guid.NewGuid().ToString();
            newTask.CreatedAt = DateTime.UtcNow;
            newTask.UpdatedAt = DateTime.UtcNow;

            _taskRepository.CreateTask(newTask);

            return CreatedAtAction(
                nameof(GetTaskById),
                new { id = newTask.Id },
                new { message = "Task created successfully.", task = newTask }
            );
        }

        [HttpPut("{id}")]
        public IActionResult UpdateTask(string id, [FromBody] Task updatedTask)
        {
            var existingTask = _taskRepository.GetTaskById(id);

            if (existingTask == null)
                return NotFound(new { message = "Task not found" });

            updatedTask.Id = id;
            _taskRepository.UpdateTask(updatedTask);

            return Ok(new { message = "Task updated successfully.", task = updatedTask });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteTask(string id)
        {
            var existingTask = _taskRepository.GetTaskById(id);

            if (existingTask == null)
                return NotFound(new { message = "Task not found" });

            _taskRepository.DeleteTask(id);

            return Ok(new { message = "Task deleted successfully.", taskId = id });
        }
    }
}