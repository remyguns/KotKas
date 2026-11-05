using KotKas.Api.DTOs;
using KotKas.Core.Entities;
using KotKas.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KotKas.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ExpensesController : ControllerBase
{
    private readonly IExpenseRepository _expenseRepository;
    private readonly IRoommateRepository _roommateRepository;

    public ExpensesController(IExpenseRepository expenseRepository, IRoommateRepository roommateRepository)
    {
        _expenseRepository = expenseRepository;
        _roommateRepository = roommateRepository;
    }

    // GET: api/expenses (nieuwste eerst)
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ExpenseDto>>> GetExpensesAsync()
    {
        var expenses = await _expenseRepository.GetAllWithRoommateAsync();
        return Ok(expenses.Select(MapToDto));
    }

    // GET: api/expenses/5
    [HttpGet("{id}")]
    public async Task<ActionResult<ExpenseDto>> GetExpenseAsync(int id)
    {
        var expense = await _expenseRepository.GetByIdWithRoommateAsync(id);
        if (expense == null)
            return NotFound();

        return Ok(MapToDto(expense));
    }

    // POST: api/expenses
    [HttpPost]
    public async Task<ActionResult<ExpenseDto>> PostExpenseAsync(ExpenseCreateUpdateDto dto)
    {
        var error = await ValidateAsync(dto);
        if (error != null)
            return BadRequest(error);

        var expense = new Expense
        {
            StoreName = dto.StoreName.Trim(),
            PurchaseDate = dto.PurchaseDate.Date,
            TotalAmount = dto.TotalAmount,
            RoommateId = dto.RoommateId
        };
        await _expenseRepository.AddAsync(expense);

        var created = await _expenseRepository.GetByIdWithRoommateAsync(expense.Id);
        return Ok(MapToDto(created!));
    }

    // PUT: api/expenses/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutExpenseAsync(int id, ExpenseCreateUpdateDto dto)
    {
        var expense = await _expenseRepository.GetByIdAsync(id);
        if (expense == null)
            return NotFound();

        var error = await ValidateAsync(dto);
        if (error != null)
            return BadRequest(error);

        expense.StoreName = dto.StoreName.Trim();
        expense.PurchaseDate = dto.PurchaseDate.Date;
        expense.TotalAmount = dto.TotalAmount;
        expense.RoommateId = dto.RoommateId;
        await _expenseRepository.UpdateAsync(expense);

        return NoContent();
    }

    // DELETE: api/expenses/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteExpenseAsync(int id)
    {
        var expense = await _expenseRepository.GetByIdAsync(id);
        if (expense == null)
            return NotFound();

        await _expenseRepository.DeleteAsync(expense);
        return NoContent();
    }

    private async Task<string?> ValidateAsync(ExpenseCreateUpdateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.StoreName))
            return "Winkel is verplicht.";
        if (dto.TotalAmount <= 0)
            return "Het bedrag moet groter zijn dan 0.";
        if (await _roommateRepository.GetByIdAsync(dto.RoommateId) == null)
            return "Kotgenoot bestaat niet.";
        return null;
    }

    internal static ExpenseDto MapToDto(Expense expense)
    {
        return new ExpenseDto
        {
            Id = expense.Id,
            StoreName = expense.StoreName,
            PurchaseDate = expense.PurchaseDate,
            TotalAmount = expense.TotalAmount,
            RoommateId = expense.RoommateId,
            RoommateName = expense.Roommate?.Name ?? string.Empty
        };
    }
}
