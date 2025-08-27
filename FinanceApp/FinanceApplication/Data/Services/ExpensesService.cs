using FinanceApplication.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceApplication.Data.Services
{
    //inherits the methods on the interface IExpensesServices
    public class ExpensesService : IExpensesService
    {
      
        private readonly FinanceAppContext _context; 

        //making contructor with financecontext to access it
        public ExpensesService(FinanceAppContext context) {

            _context = context; 
        }


        public async Task Add(Expense expense)
        {  
                _context.Expenses.Add(expense);
                await _context.SaveChangesAsync();     
        }

        public async Task<IEnumerable<Expense>> GetAll()
        {
            var expenses = await _context.Expenses.ToListAsync();
            return expenses; 
        }
    }
}
