using LibrarySystemServer.DTOs.Book;
using LibrarySystemServer.DTOs.Pagination;
using LibrarySystemServer.Model;
using LibrarySystemServer.Repositories.Interfaces;
using LibrarySystemServer.Services.Interfaces;

namespace LibrarySystemServer.Services.Implementations;

public class MemberService (IMemberRepository context): IMemberService
{
    private readonly IMemberRepository  _memberRepository = context;
    
    public Task<PageResult<BookDto>> GetAvailableBooksAsync(int page, int size, CancellationToken cancellationToken)
    {
        var query = _memberRepository.GetAvailableBooksAsync(page, size, cancellationToken);
        
        
    }

    public Task<IEnumerable<BorrowRecord>> GetMyBorrowedBooksAsync(string memberId , bool  onlyActive = true) => _memberRepository.GetBorrowRecordsByMemberAsync(memberId, onlyActive);
    

    public async Task<BorrowRecord> BorrowBookAsync(string memberId, Guid bookId)
    {
        // 1. Check book exists
        var book = await _memberRepository.GetBookByIdAsync(bookId);
        if (book == null)
            throw new Exception("Book not found.");

        // 2. Check if book is available
        if (!book.IsAvailable)
            throw new Exception("Book is not available.");

        // 3. Check if book is already borrowed by someone
        bool alreadyBorrowed = await _memberRepository.IsBookAlreadyBorrowedAsync(bookId);
        if (alreadyBorrowed)
            throw new Exception("This book is already borrowed.");

        // 4. Limit member to 3 active borrows
        var activeBorrows = await _memberRepository.GetBorrowRecordsByMemberAsync(memberId, true);
        if (activeBorrows.Count() >= 3)
            throw new Exception("You have reached the maximum number of borrowed books.");

        //create borrow record 
        var record = new BorrowRecord()
        {
            Id = Guid.NewGuid(),
            BookId = book.Id,
            MemberId = memberId,
            Status = BorrowStatus.Borrowed,
        };
        
        await _memberRepository.AddBorrowRecordAsync(record);
        return record;
    }

    public async Task<BorrowRecord> ReturnBookAsync(string memberId, Guid borrowId)
    {
       var record = await _memberRepository.GetBorrowRecordByIdAsync(borrowId);
       if (record == null)
           throw new Exception("Record not found.");
       
       if(record.MemberId != memberId)
            throw new Exception("Member not found.");
       
       if (record.ReturnDate != null)
           throw new Exception("Book is already returned.");
       
       record.ReturnDate = DateTime.Now;
       
       await _memberRepository.UpdateBorrowRecordAsync(record);
       return record;
    }

    public async Task CancelRequestAsync(string memberId, Guid borrowId)
    {
        var record = await _memberRepository.GetBorrowRecordByIdAsync(borrowId);
        if (record == null)
            throw new Exception("Borrow record not found.");

        if (record.MemberId != memberId)
            throw new Exception("Unauthorized action.");

        if (record.BorrowDate != null)
            throw new Exception("Borrow request already processed.");
        
        record.Status = BorrowStatus.InShelf;
        await _memberRepository.UpdateBorrowRecordAsync(record);
    }
}