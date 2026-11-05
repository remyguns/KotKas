using KotKas.Api.DTOs;
using KotKas.Core.Entities;
using KotKas.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KotKas.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RoommatesController : ControllerBase
{
    private readonly IRoommateRepository _roommateRepository;
    private readonly IExpenseRepository _expenseRepository;
    private readonly IBalanceService _balanceService;

    public RoommatesController(IRoommateRepository roommateRepository,
                               IExpenseRepository expenseRepository,
                               IBalanceService balanceService)
    {
        _roommateRepository = roommateRepository;
        _expenseRepository = expenseRepository;
        _balanceService = balanceService;
    }

    // GET: api/roommates
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RoommateDto>>> GetRoommatesAsync()
    {
        var roommates = await _roommateRepository.GetAllWithExpensesAsync();
        var balances = await _balanceService.CalculateBalancesAsync();

        return Ok(roommates.Select(r => MapToDto(r, balances)));
    }

    // GET: api/roommates/5
    [HttpGet("{id}")]
    public async Task<ActionResult<RoommateDto>> GetRoommateAsync(int id)
    {
        var roommate = await _roommateRepository.GetByIdWithExpensesAsync(id);
        if (roommate == null)
            return NotFound();

        var balances = await _balanceService.CalculateBalancesAsync();
        return Ok(MapToDto(roommate, balances));
    }

    // GET: api/roommates/5/expenses
    [HttpGet("{id}/expenses")]
    public async Task<ActionResult<IEnumerable<ExpenseDto>>> GetRoommateExpensesAsync(int id)
    {
        var expenses = await _expenseRepository.GetByRoommateIdAsync(id);
        return Ok(expenses.Select(ExpensesController.MapToDto));
    }

    // POST: api/roommates
    [HttpPost]
    public async Task<ActionResult<RoommateDto>> PostRoommateAsync(RoommateCreateUpdateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest("Naam is verplicht.");

        var roommate = new Roommate { Name = dto.Name.Trim(), Email = dto.Email.Trim() };
        await _roommateRepository.AddAsync(roommate);

        var balances = await _balanceService.CalculateBalancesAsync();
        return Ok(MapToDto(roommate, balances));
    }

    // PUT: api/roommates/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutRoommateAsync(int id, RoommateCreateUpdateDto dto)
    {
        var roommate = await _roommateRepository.GetByIdAsync(id);
        if (roommate == null)
            return NotFound();

        roommate.Name = dto.Name.Trim();
        roommate.Email = dto.Email.Trim();
        await _roommateRepository.UpdateAsync(roommate);

        return NoContent();
    }

    // DELETE: api/roommates/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRoommateAsync(int id)
    {
        var roommate = await _roommateRepository.GetByIdAsync(id);
        if (roommate == null)
            return NotFound();

        await _roommateRepository.DeleteAsync(roommate);
        return NoContent();
    }

    private RoommateDto MapToDto(Roommate roommate, Dictionary<int, decimal> balances)
    {
        return new RoommateDto
        {
            Id = roommate.Id,
            Name = roommate.Name,
            Email = roommate.Email,
            TotalPaid = _balanceService.CalculateTotalPaid(roommate),
            Balance = balances.TryGetValue(roommate.Id, out var balance) ? balance : 0
        };
    }
}
