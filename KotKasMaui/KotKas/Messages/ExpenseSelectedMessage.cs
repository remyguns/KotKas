using CommunityToolkit.Mvvm.Messaging.Messages;
using KotKas.Models;

namespace KotKas.Messages;

// ExpensesViewModel -> ReviewExpenseViewModel: bestaande uitgave aanpassen
public class ExpenseSelectedMessage : ValueChangedMessage<Expense>
{
    public ExpenseSelectedMessage(Expense value) : base(value)
    {
    }
}
