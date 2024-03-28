using Amazon.DynamoDBv2.DataModel;
using Microsoft.AspNetCore.Mvc;
using ServerlessAPI.Entities;

namespace ServerlessAPI.Controllers;

[Route("api/[controller]")]
[Produces("application/json")]
public class BooksController : ControllerBase
{
    private readonly ILogger<BooksController> logger;
    private List<Book> _listBooks;

    public BooksController(ILogger<BooksController> logger)
    {
        this.logger = logger;
        _listBooks = new List<Book>
        {
            new() {Id = new Guid()  , Authors = "Firmino", Title = "Como ajudar um amigo com lambda",},
            new() {Id = new Guid(),  Authors = "Gil", Title = "Como ser cototo em lambda"},
        };
    }

    // GET api/books
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Book>>> Get([FromQuery] int limit = 10)
    {
        if (limit <= 0 || limit > 100) return BadRequest("The limit should been between [1-100]"); 
        return Ok(_listBooks);
    }

    // GET api/books/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Book>> Get(Guid id)
    {
        return Ok(_listBooks.Find(x => x.Id == id));
    }

    // POST api/books
    [HttpPost]
    public async Task<ActionResult<Book>> Post([FromBody] Book book)
    {
        if (book == null) return ValidationProblem("Invalid input! Book not informed");

        try
        {
            _listBooks.Add(book);
            return CreatedAtAction(
                nameof(Get),
                new { id = book.Id },
                book);
        }
        catch (Exception e)
        {
            return BadRequest("Fail to persist");

        }
    }

    // PUT api/books/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(Guid id, [FromBody] Book book)
    {
        if (id == Guid.Empty || book == null) return ValidationProblem("Invalid request payload");

        // Retrieve the book.
        var bookRetrieved = _listBooks.Find(x => x.Id == id);

        if (bookRetrieved == null)
        {
            var errorMsg = $"Invalid input! No book found with id:{id}";
            return NotFound(errorMsg);
        }

        book.Id = bookRetrieved.Id;

        _listBooks.Remove(bookRetrieved);
        _listBooks.Add(book);
        return Ok();
    }

    // DELETE api/books/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        if (id == Guid.Empty) return ValidationProblem("Invalid request payload");

        var bookRetrieved = _listBooks.Find(x => x.Id == id);

        if (bookRetrieved == null)
        {
            var errorMsg = $"Invalid input! No book found with id:{id}";
            return NotFound(errorMsg);
        }

        _listBooks.Remove(bookRetrieved);
        return Ok();
    }
}
