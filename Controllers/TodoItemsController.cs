using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoAPI2.Data;
using TodoAPI2.Models;

namespace TodoAPI2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly TodoContext _context;

        public TodoItemsController(TodoContext context)
        {
            _context = context;
        }

        // GET: api/TodoItems
        // Returns all todo items
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetAllTodos()
        {
            var todos = await _context.TodoItems.ToListAsync();
            return Ok(todos);
        }

        // GET: api/TodoItems/5
        // Returns a single todo by id
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> GetTodoById(int id)
        {
            var todo = await _context.TodoItems.FindAsync(id);

            if (todo == null)
                return NotFound();

            return Ok(todo);
        }

        // POST: api/TodoItems
        // Creates a new todo
        [HttpPost]
        public async Task<ActionResult<TodoItem>> CreateTodo(TodoItem item)
        {
            if (string.IsNullOrWhiteSpace(item.Name))
                return BadRequest("Name is required.");

            _context.TodoItems.Add(item);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTodoById), new { id = item.Id }, item);
        }

        // PUT: api/TodoItems/5
        // Updates an existing todo
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTodo(int id, TodoItem updatedItem)
        {
            if (id != updatedItem.Id)
                return BadRequest("ID mismatch.");

            var existingItem = await _context.TodoItems.FindAsync(id);
            if (existingItem == null)
                return NotFound();

            // Update properties
            existingItem.Name = updatedItem.Name;
            existingItem.IsComplete = updatedItem.IsComplete;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/TodoItems/5
        // Deletes a todo
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodo(int id)
        {
            var todo = await _context.TodoItems.FindAsync(id);
            if (todo == null)
                return NotFound();

            _context.TodoItems.Remove(todo);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
