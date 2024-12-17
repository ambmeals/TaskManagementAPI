using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using TaskManagementAPI.Controllers;
using TaskManagementAPI.Models;
using TaskManagementAPI.Repositories;

namespace TaskManagementAPI.TaskManagementAPITests
{
    [TestFixture]
    public class TaskControllerTests
    {
        private TaskController _taskController;
        private Mock<ITaskRepository> _mockRepository;

        [SetUp]
        public void SetUp()
        {
            _mockRepository = new Mock<ITaskRepository>();
            _taskController = new TaskController(_mockRepository.Object);
        }

        private static List<Models.Task> GetDummyTasks() =>
            new()
            {
                new() { Id = "1", Title = "Task 1", DueDate = DateTime.UtcNow.AddDays(1) },
                new() { Id = "2", Title = "Task 2", DueDate = DateTime.UtcNow.AddDays(2) },
                new() { Id = "3", Title = "Task 3" }
            };

        [Test]
        public void get_all_tasks_should_return_tasks_ordered_by_dueDate()
        {
            var tasks = GetDummyTasks();
            _mockRepository.Setup(repo => repo.GetTasks()).Returns(tasks);

            var result = _taskController.GetAllTasks().Result as OkObjectResult;

            var response = result!.Value as ApiResponse<IEnumerable<Models.Task>>;

            response!.Message.Should().Be("Tasks retrieved successfully");
            response.Data.Select(t => t.Title).Should().ContainInOrder("Task 1", "Task 2", "Task 3");
        }

        [Test]
        public void get_task_by_id_should_return_task_when_it_exists()
        {
            var task = new Models.Task { Id = "1", Title = "Task 1" };
            _mockRepository.Setup(repo => repo.GetTaskById("1")).Returns(task);

            var result = _taskController.GetTaskById("1").Result as OkObjectResult;

            var response = result!.Value as ApiResponse<Models.Task>;

            response!.Message.Should().Be("Task retrieved successfully");
            response.Data.Should().BeEquivalentTo(task);
        }

        [Test]
        public void get_task_by_id_should_return_not_found_when_task_does_not_exist()
        {
            _mockRepository.Setup(repo => repo.GetTaskById("non-existent-id")).Returns((Models.Task)null);

            var result = _taskController.GetTaskById("non-existent-id").Result;

            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Test]
        public void create_should_create_task_if_valid()
        {
            var input = new TaskInput
            {
                Title = "New Task",
                Description = "Description",
                Priority = "HIGH",
                Status = "TODO",
                DueDate = DateTime.UtcNow.AddDays(3)
            };

            var result = _taskController.CreateTask(input).Result as CreatedAtActionResult;

            var response = result!.Value as ApiResponse<Models.Task>;

            response!.Message.Should().Be("Task created successfully");
            _mockRepository.Verify(repo => repo.CreateTask(It.IsAny<Models.Task>()), Times.Once);
        }

        [Test]
        public void create_should_return_bad_request_when_title_is_empty()
        {
            var input = new TaskInput
            {
                Title = "",
                Description = "Invalid task",
                Priority = "LOW",
                Status = "TODO"
            };

            var result = _taskController.CreateTask(input).Result as BadRequestObjectResult;

            var response = result!.Value as ApiResponse<Models.Task>;

            response!.Message.Should().Be("Title is required");
            _mockRepository.Verify(repo => repo.CreateTask(It.IsAny<Models.Task>()), Times.Never);
        }
    }
}
