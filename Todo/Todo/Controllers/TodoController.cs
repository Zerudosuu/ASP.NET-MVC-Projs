using Microsoft.AspNetCore.Mvc;
using Todo.Data.Services;
using Todo.Models;

namespace Todo.Controllers
{
    public class TodoController : Controller
    {
        private readonly ITodoService _todoService;

        public TodoController(ITodoService todoService)
        {
            _todoService = todoService;
        }

        public async Task<IActionResult> Index()
        {
            var tasks = await _todoService.GetAllTasksAsync();
            return View(tasks);
        }

        [HttpPost]
        public async Task<IActionResult> Create(TodoTask task)
        {
            if (ModelState.IsValid)
            {
                await _todoService.AddTaskAsync(task);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["Error"] = "Invalid data. Please correct the errors and try again.";
            }

            return View(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var task = _todoService.GetTaskByIdAsync(id).Result;
            if (task == null)
            {
                return NotFound();
            }
            return PartialView("_EditTaskModal", task);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, TodoTask task)
        {
            if (id != task.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                await _todoService.UpdateTaskAsync(task);

                return RedirectToAction(nameof(Index));
            }
            return View(task);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var task = await _todoService.GetTaskByIdAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            await _todoService.DeleteTaskAsync(id);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var task = await _todoService.GetTaskByIdAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            return PartialView("_taskModal", task);
        }
    }
}
