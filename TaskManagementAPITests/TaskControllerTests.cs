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
        private TaskController _taskController;
        private Mock<ITaskRepository> _mockRepository;

        [SetUp]
        public void SetUp()
        {
            _mockRepository = new Mock<ITaskRepository>();
            _taskController = new TaskController(_mockRepository.Object);
        }

        private static List<TaskModel> GetDummyTasks()
        {
            return new List<TaskModel>
            {
                new() { Id = "1", Title = "Task 1", DueDate = DateTime.UtcNow.AddDays(1) },
                new() { Id = "2", Title = "Task 2", DueDate = DateTime.UtcNow.AddDays(2) },
                new() { Id = "3", Title = "Task 3" }
            };
        }

        [Test]
        public void get_all_tasks_should_return_tasks_ordered_by_dueDate()
        {
            var tasks = GetDummyTasks();
            _mockRepository.Setup(repo => repo.GetTasks()).Returns(tasks);

            ((_taskController.GetAllTasks() as OkObjectResult).Value as IEnumerable<TaskModel>)
                .Select(t => t.Title).Should()
                .ContainInOrder("Task 1", "Task 2", "Task 3");
        }

        [Test]
        public void get_task_by_id_should_return_task_when_it_exists()
        {
            _mockRepository.Setup(repo => repo.GetTaskById("1"))
                .Returns(new TaskModel { Id = "1", Title = "Task 1" });

            (_taskController.GetTaskById("1") as OkObjectResult).Should().NotBeNull();
            _mockRepository.Verify(repo => repo.GetTaskById("1"), Times.Once);
        }

        [Test]
        public void get_task_by_id_should_return_not_found_when_task_does_not_exist()
        {
            _mockRepository.Setup(repo => repo
                .GetTaskById("non-existent-id")).Returns((TaskModel)null);

            _taskController.GetTaskById("non-existent-id")
                .Should().BeOfType<NotFoundObjectResult>();

            _mockRepository.Verify(repo => repo.GetTaskById("non-existent-id"), Times.Once);
        }

        [Test]
        public void create_should_create_task_if_valid()
        {
            var newTask = new TaskModel
            {
                Title = "New Task",
                Priority = "HIGH",
                Status = "TODO"
            };

            var result = _taskController.CreateTask(newTask) as CreatedAtActionResult;

            result.Should().NotBeNull();
            _mockRepository.Verify(repo => repo.CreateTask(It.IsAny<TaskModel>()), Times.Once);

            result.ActionName.Should().Be(nameof(TaskController.GetTaskById));
            result.RouteValues["id"].Should().NotBeNull();
        }

        [Test]
        public void create_should_return_bad_request_when_title_is_empty()
        {
            var invalidTask = new TaskModel
            {
                Title = "",
                Priority = "LOW",
                Status = "TODO"
            };

            var result = _taskController.CreateTask(invalidTask);

            result.Should().BeOfType<BadRequestObjectResult>();
            _mockRepository.Verify(repo => repo.CreateTask(It.IsAny<TaskModel>()), Times.Never);
        }
        private class TaskResponse
        {
            public Task Message { get; set; }
        }
    }
}
