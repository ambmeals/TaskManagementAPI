using Microsoft.AspNetCore.Mvc;
using TaskManagementAPI.Repositories;
using TaskManagementAPI.Models;
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
        public ActionResult<ApiResponse<IEnumerable<Task>>> GetAllTasks()
        {
            return Ok(new ApiResponse<IEnumerable<Task>>
                ("Tasks retrieved successfully", _taskRepository.GetTasks()));
        }

        [HttpGet("{id}")]
        public ActionResult<ApiResponse<Task>> GetTaskById(string id)
        {
            if (_taskRepository.GetTaskById(id) == null)
                return NotFound(new ApiResponse<Task>("Task not found"));

            return Ok(new ApiResponse<Task>("Task retrieved successfully", _taskRepository.GetTaskById(id)));
        }

        [HttpPost]
        public ActionResult<ApiResponse<Task>> CreateTask([FromBody] TaskInput input)
        {
            if (string.IsNullOrWhiteSpace(input.Title))
                return BadRequest(new ApiResponse<Task>("Title is required"));

            var newTask = new Task
            {
                Id = Guid.NewGuid().ToString(),
                Title = input.Title,
                Description = input.Description,
                Priority = input.Priority ?? "MEDIUM",
                Status = input.Status ?? "TODO",
                DueDate = input.DueDate,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _taskRepository.CreateTask(newTask);

            return CreatedAtAction(nameof(GetTaskById), new { id = newTask.Id },
                new ApiResponse<Task>("Task created successfully", newTask));
        }

        [HttpPut("{id}")]
        public ActionResult<ApiResponse<Task>> UpdateTask(string id, [FromBody] TaskInput input)
        {
            var existingTask = _taskRepository.GetTaskById(id);

            if (existingTask == null)
                return NotFound(new ApiResponse<Task>("Task not found"));

            existingTask.Title = input.Title ?? existingTask.Title;
            existingTask.Description = input.Description ?? existingTask.Description;
            existingTask.Priority = input.Priority ?? existingTask.Priority;
            existingTask.Status = input.Status ?? existingTask.Status;
            existingTask.DueDate = input.DueDate ?? existingTask.DueDate;
            existingTask.UpdatedAt = DateTime.UtcNow;

            _taskRepository.UpdateTask(existingTask);

            return Ok(new ApiResponse<Task>("Task updated successfully", existingTask));
        }

        [HttpDelete("{id}")]
        public ActionResult<ApiResponse<string>> DeleteTask(string id)
        {
            var existingTask = _taskRepository.GetTaskById(id);

            if (existingTask == null)
                return NotFound(new ApiResponse<string>("Task not found"));

            _taskRepository.DeleteTask(id);

            return Ok(new ApiResponse<string>("Task deleted successfully"));
        }
    }
}
