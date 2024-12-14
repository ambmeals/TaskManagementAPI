using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using TaskManagementAPI.Controllers;
using TaskManagementAPI.Repositories;
using TaskModel = TaskManagementAPI.Models.Task;

namespace TaskManagementAPI.TaskManagementAPITests
{
    [TestFixture]
    public class TaskControllerTests
    {
        private Mock<ITaskRepository> _mockRepository;
        private TaskController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockRepository = new Mock<ITaskRepository>();
            _mockRepository.Setup(repo => repo.GetTasks())
                .Returns(new List<TaskModel>
                {
                    new() { Id = "1", Title = "Task 1", DueDate = DateTime.UtcNow.AddDays(1) },
                    new() { Id = "2", Title = "Task 2", DueDate = DateTime.UtcNow.AddDays(2) }
                });
            _mockRepository.Setup(repo => repo.GetTaskById("1"))
                .Returns(new TaskModel { Id = "1", Title = "Task 1" });

            _controller = new TaskController(_mockRepository.Object);
        }

        [Test]
        public void GetAllTasks_ShouldReturnAllTasks()
        {
            var result = _controller.GetAllTasks() as OkObjectResult;

            result.Should().NotBeNull();

            var tasks = result!.Value as IEnumerable<TaskModel>;

            tasks.Should()
                .NotBeNull()
                .And
                .HaveCount(2);
        }

        [Test]
        public void GetTaskById_ShouldReturnTask_WhenTaskExists()
        {
            var result = _controller.GetTaskById("1") as OkObjectResult;

            result.Should().NotBeNull();

            var task = result!.Value as TaskModel;

            task.Should()
                .NotBeNull()
                .And
                .Match<TaskModel>(t => t.Id == "1");

        }

        [Test]
        public void GetTaskById_ShouldReturnNotFound_WhenTaskDoesNotExist()
        {
            var result = _controller.GetTaskById("non-existent-id");

            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Test]
        public void CreateTask_ShouldAddTask_WhenValid()
        {
            var newTask = new TaskModel
            {
                Title = "New Task", 
                Priority = "HIGH", 
                Status = "TODO"
            };

            var result = _controller.CreateTask(newTask) as CreatedAtActionResult;

            result.Should().NotBeNull();

            var createdTask = result!.Value as TaskModel;

            createdTask.Should().NotBeNull();
            createdTask!.Title.Should().Be("New Task");
        }
    }
}
