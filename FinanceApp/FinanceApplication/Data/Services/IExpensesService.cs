using FinanceApplication.Models;

namespace FinanceApplication.Data.Services
{
    //To make the application protected and not expose the data, we
    //created an interface IExpenservice add methood task for async functions
    // this Interface will be used by ExpensesServices.cs to make the function whole
    public interface IExpensesService
    { 
        Task<IEnumerable<Expense>> GetAll();
        Task Add(Expense expense);
    }
}
