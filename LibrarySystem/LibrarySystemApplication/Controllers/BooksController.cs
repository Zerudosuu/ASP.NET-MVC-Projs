public class BooksController : Controller
{
    private readonly ILibraryServices _libraryServices;

    public BooksController(ILibraryServices libraryServices)
    {
        _libraryServices = libraryServices;
    }

    // GET: Books
    public async Task<IActionResult> Index()
    {
        var books = await _libraryServices.GetAllAsync();
        return View(books);
    }

    // GET: Books/Details/5
    public async Task<IActionResult> Details(string id)
    {
        if (id == null) return NotFound();

        var book = await _libraryServices.GetByIdAsync(id);
        if (book == null) return NotFound();

        return View(book);
    }

    // GET: Books/Create
    public IActionResult Create() => View();

    // POST: Books/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("BookId,Title,Description,Author,Category,PublishedDate,CoverImageUrl")] Book book)
    {
        if (ModelState.IsValid)
        {
            await _libraryServices.AddAsync(book);
            return RedirectToAction(nameof(Index));
        }
        return View(book);
    }

    // GET: Books/Edit/5
    public async Task<IActionResult> Edit(string id)
    {
        var book = await _libraryServices.GetByIdAsync(id);
        if (book == null) return NotFound();

        return View(book);
    }

    // POST: Books/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, Book book)
    {
        if (id != book.BookId) return NotFound();

        if (ModelState.IsValid)
        {
            await _libraryServices.UpdateAsync(book);
            return RedirectToAction(nameof(Index));
        }
        return View(book);
    }

    // GET: Books/Delete/5
    public async Task<IActionResult> Delete(string id)
    {
        var book = await _libraryServices.GetByIdAsync(id);
        if (book == null) return NotFound();

        return View(book);
    }

    // POST: Books/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        await _libraryServices.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
