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
        private TaskController? _taskController;
        private Mock<ITaskRepository>? _mockRepository;

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

            _mockRepository.Setup(repo => repo.GetTaskById("non-existent-id"))
                .Returns((TaskModel)null);

            _mockRepository.Setup(repo => repo.CreateTask(It.IsAny<TaskModel>()))
                .Verifiable();

            _taskController = new TaskController(_mockRepository.Object);
        }

        [Test]
        public void get_all_tasks_should_return_tasks()
        {
            var result = _taskController?.GetAllTasks() as OkObjectResult;

            result.Should().NotBeNull();
            _mockRepository?.Verify(repo => repo.GetTasks(), Times.Once);

            var tasks = result!.Value as IEnumerable<TaskModel>;

            tasks.Should().NotBeNull();
            tasks!.Count().Should().Be(2);
        }

        [Test]
        public void get_task_by_id_should_return_task_when_it_exists()
        {
            var result = _taskController?.GetTaskById("1") as OkObjectResult;

            result.Should().NotBeNull();
            _mockRepository?.Verify(repo => repo.GetTaskById("1"), Times.Once);

            var task = result!.Value as TaskModel;

            task.Should().NotBeNull();
            task!.Id.Should().Be("1");
            task.Title.Should().Be("Task 1");
        }

        [Test]
        public void get_task_by_id_should_return_not_found_when_task_does_not_exist()
        {
            var result = _taskController?.GetTaskById("non-existent-id");

            result.Should().BeOfType<NotFoundObjectResult>();

            _mockRepository?.Verify(repo => repo.GetTaskById("non-existent-id"), Times.Once);
        }

        [Test]
        public void create_should_create_task_if_valid()
        {
            var newTask = new TaskModel
            {
                Id = "3",
                Title = "New Task",
                Priority = "HIGH",
                Status = "TODO"
            };

            var result = _taskController?.CreateTask(newTask) as CreatedAtActionResult;

            result.Should().NotBeNull();
            _mockRepository?.Verify(repo => repo.CreateTask(newTask), Times.Once);

            result!.ActionName.Should().Be(nameof(TaskController.GetTaskById));
            result.RouteValues.Should().ContainKey("id").WhoseValue.Should().Be(newTask.Id);
        }

        [Test]
        public void create_should_return_bad_request_when_title_is_empty()
        {
            var newTask = new TaskModel
            {
                Id = "4",
                Title = "",
                Priority = "LOW",
                Status = "TODO"
            };

            var result = _taskController?.CreateTask(newTask);

            result.Should().BeOfType<BadRequestObjectResult>();

            _mockRepository?.Verify(repo => repo.CreateTask(It.IsAny<TaskModel>()), Times.Never);
        }
    }
}