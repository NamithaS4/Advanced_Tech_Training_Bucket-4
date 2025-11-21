using AMIProjectAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AMIProjectAPI.Controllers.Secured
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BillsController : ControllerBase
    {
        private readonly AmiprojectContext _ctx;
        private readonly ILogger<BillsController> _logger;
        public BillsController(AmiprojectContext ctx, ILogger<BillsController> logger)
        {
            _ctx = ctx;
            _logger = logger;
        }

        private bool TryGetConsumerId(out int consumerId)
        {
            consumerId = 0;
            var c = User.Claims.FirstOrDefault(x => string.Equals(x.Type, "ConsumerId", StringComparison.OrdinalIgnoreCase));
            if (c == null) return false;
            return int.TryParse(c.Value, out consumerId);
        }

        // GET: api/bills
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? meterId, [FromQuery] string? status,
                                               [FromQuery] DateTime? from, [FromQuery] DateTime? to,
                                               [FromQuery] int? page, [FromQuery] int? pageSize,
                                               [FromQuery] int? consumerId) // optional consumerId to assist client
        {
            try
            {
                var q = _ctx.Bills
                    .AsNoTracking()
                    .Include(b => b.Meter)
                    .AsQueryable();

                // If caller supplied consumerId explicitly use it; otherwise check claims
                if (consumerId.HasValue)
                {
                    q = q.Where(b => b.Meter != null && b.Meter.ConsumerId == consumerId.Value);
                }
                else
                {
                    var isConsumer = User.HasClaim(c => string.Equals(c.Type, "UserType", StringComparison.OrdinalIgnoreCase)
                                                        && c.Value == "Consumer");
                    if (isConsumer)
                    {
                        if (!TryGetConsumerId(out var cid))
                        {
                            _logger.LogWarning("ConsumerId claim missing or invalid.");
                            return Forbid("ConsumerId claim missing or invalid.");
                        }
                        q = q.Where(b => b.Meter != null && b.Meter.ConsumerId == cid);
                    }
                }

                if (!string.IsNullOrWhiteSpace(meterId))
                    q = q.Where(b => b.MeterId == meterId);

                if (!string.IsNullOrWhiteSpace(status))
                    q = q.Where(b => b.Status == status);

                // The MVC Index uses MonthStartDate for month filtering in some places.
                // Keep supporting GeneratedAt filters (from/to) for backwards compatibility.
                if (from.HasValue)
                    q = q.Where(b => b.GeneratedAt >= from.Value);

                if (to.HasValue)
                    q = q.Where(b => b.GeneratedAt <= to.Value);

                q = q.OrderByDescending(b => b.GeneratedAt);

                // paging
                if (page.HasValue && pageSize.HasValue && page > 0 && pageSize > 0)
                {
                    var p = Math.Max(1, page.Value);
                    var ps = Math.Max(1, pageSize.Value);
                    var total = await q.CountAsync();
                    var items = await q.Skip((p - 1) * ps).Take(ps).ToListAsync();

                    var projected = items.Select(b => new
                    {
                        b.BillId,
                        MeterID = b.MeterId,
                        MonthStartDate = b.MonthStartDate,
                        MonthlyConsumptionkWh = b.MonthlyConsumptionkWh,
                        Category = b.Category ?? "",
                        BaseRate = b.BaseRate,
                        SlabRate = b.SlabRate,
                        TaxRate = b.TaxRate,
                        b.Amount,
                        AmountPaid = b.AmountPaid,
                        b.Status,
                        GeneratedAt = b.GeneratedAt
                    });

                    return Ok(new
                    {
                        total,
                        page = p,
                        pageSize = ps,
                        items = projected
                    });
                }
                else
                {
                    var list = await q.ToListAsync();
                    var projected = list.Select(b => new
                    {
                        b.BillId,
                        MeterID = b.MeterId,
                        MonthStartDate = b.MonthStartDate,
                        MonthlyConsumptionkWh = b.MonthlyConsumptionkWh,
                        Category = b.Category ?? "",
                        BaseRate = b.BaseRate,
                        SlabRate = b.SlabRate,
                        TaxRate = b.TaxRate,
                        b.Amount,
                        AmountPaid = b.AmountPaid,
                        b.Status,
                        GeneratedAt = b.GeneratedAt
                    });
                    return Ok(projected);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed loading bills");
                return StatusCode(500, new { error = "Server error loading bills." });
            }
        }

        // GET: api/bills/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var b = await _ctx.Bills
                .AsNoTracking()
                .Include(x => x.Meter)
                .FirstOrDefaultAsync(x => x.BillId == id);
            if (b == null) return NotFound(new { error = "Bill not found." });

            // consumer check
            var isConsumer = User.HasClaim(c => string.Equals(c.Type, "UserType", StringComparison.OrdinalIgnoreCase) && c.Value == "Consumer");
            if (isConsumer)
            {
                if (!TryGetConsumerId(out var cid)) return Forbid("ConsumerId claim missing or invalid.");
                if (b.Meter?.ConsumerId != cid) return Forbid("Not allowed to access this bill.");
            }

            return Ok(new
            {
                b.BillId,
                MeterID = b.MeterId,
                MonthStartDate = b.MonthStartDate,
                b.MonthlyConsumptionkWh,
                Category = b.Category ?? "",
                BaseRate = b.BaseRate,
                SlabRate = b.SlabRate,
                TaxRate = b.TaxRate,
                b.Amount,
                AmountPaid = b.AmountPaid,
                b.Status,
                b.GeneratedAt
            });
        }

        // PUT: api/bills/PayBill/{id} -> process payment for a bill
        [HttpPut("PayBill/{id:int}")]
        public async Task<IActionResult> PayBill(int id, [FromBody] BillPaymentDto payment)
        {
            try
            {
                var bill = await _ctx.Bills
                    .Include(b => b.Meter)
                    .FirstOrDefaultAsync(b => b.BillId == id);

                if (bill == null) return NotFound(new { error = "Bill not found." });

                var isConsumer = User.HasClaim(c => string.Equals(c.Type, "UserType", StringComparison.OrdinalIgnoreCase) && c.Value == "Consumer");
                if (isConsumer)
                {
                    if (!TryGetConsumerId(out var cid)) return Forbid("ConsumerId claim missing or invalid.");
                    if (bill.Meter?.ConsumerId != cid) return Forbid("Not allowed to pay this bill.");
                }

                // Allow payment for Pending and HalfPaid bills
                if (string.Equals(bill.Status, "Paid", StringComparison.OrdinalIgnoreCase))
                {
                    // Check if it's actually fully paid
                    if (bill.AmountPaid >= bill.Amount)
                        return BadRequest(new { error = "Bill is already fully paid." });
                    // If AmountPaid < Amount, allow payment to continue
                }

                if (payment.AmountPaid <= 0)
                    return BadRequest(new { error = "Invalid payment amount. Amount must be greater than zero." });

                // Calculate remaining balance
                // Amount is the original bill amount (never changes)
                // AmountPaid tracks how much has been paid
                decimal originalAmount = bill.Amount;
                decimal currentPaid = bill.AmountPaid;
                decimal newPaid = currentPaid + payment.AmountPaid;
                
                // Don't allow overpayment
                if (newPaid > originalAmount)
                {
                    return BadRequest(new { error = $"Payment amount exceeds bill amount. Maximum payment allowed: ₹{originalAmount - currentPaid:F2}" });
                }
                
                // Update AmountPaid (not Amount - Amount stays as original)
                bill.AmountPaid = newPaid;
                
                // Calculate remaining balance
                decimal remainingBalance = originalAmount - newPaid;

                // Update status based on remaining balance
                if (remainingBalance <= 0)
                {
                    bill.Status = "Paid";
                }
                else
                {
                    bill.Status = "HalfPaid";
                }

                await _ctx.SaveChangesAsync();

                return Ok(new
                {
                    message = "Payment processed successfully.",
                    remainingBalance = Math.Max(0, remainingBalance),
                    status = bill.Status,
                    amountPaid = bill.AmountPaid,
                    originalAmount = bill.Amount
                });
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "DB error processing payment for bill {Id}", id);
                return StatusCode(500, new { error = "Database error while processing payment." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment for bill {Id}", id);
                return StatusCode(500, new { error = "Server error while processing payment." });
            }
        }

        // POST: api/bills/pay/{id} -> mark bill as paid (legacy endpoint, kept for backward compatibility)
        [HttpPost("pay/{id:int}")]
        public async Task<IActionResult> Pay(int id)
        {
            try
            {
                var bill = await _ctx.Bills
                    .Include(b => b.Meter)
                    .FirstOrDefaultAsync(b => b.BillId == id);

                if (bill == null) return NotFound(new { error = "Bill not found." });

                var isConsumer = User.HasClaim(c => string.Equals(c.Type, "UserType", StringComparison.OrdinalIgnoreCase) && c.Value == "Consumer");
                if (isConsumer)
                {
                    if (!TryGetConsumerId(out var cid)) return Forbid("ConsumerId claim missing or invalid.");
                    if (bill.Meter?.ConsumerId != cid) return Forbid("Not allowed to pay this bill.");
                }

                // If already paid, reject. Allow Pending or HalfPaid bills to be paid
                if (string.Equals(bill.Status, "Paid", StringComparison.OrdinalIgnoreCase))
                    return BadRequest(new { error = $"Bill is already paid." });
                
                if (!string.Equals(bill.Status, "Pending", StringComparison.OrdinalIgnoreCase) 
                    && !string.Equals(bill.Status, "HalfPaid", StringComparison.OrdinalIgnoreCase))
                    return BadRequest(new { error = $"Bill status is '{bill.Status}' and cannot be paid." });

                bill.Status = "Paid";
                // Optionally record payment time — preserve GeneratedAt as bill generation time; you may add PaidAt field if needed

                await _ctx.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "DB error paying bill {Id}", id);
                return StatusCode(500, new { error = "Database error while updating bill status." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error paying bill {Id}", id);
                return StatusCode(500, new { error = "Server error while processing payment." });
            }
        }

        // PATCH: api/bills/{id} -> update bill status and/or amount (for partial payments)
        [HttpPatch("{id:int}")]
        public async Task<IActionResult> UpdateBill(int id, [FromBody] BillUpdateDto updateDto)
        {
            try
            {
                var bill = await _ctx.Bills
                    .Include(b => b.Meter)
                    .FirstOrDefaultAsync(b => b.BillId == id);

                if (bill == null) return NotFound(new { error = "Bill not found." });

                var isConsumer = User.HasClaim(c => string.Equals(c.Type, "UserType", StringComparison.OrdinalIgnoreCase) && c.Value == "Consumer");
                if (isConsumer)
                {
                    if (!TryGetConsumerId(out var cid)) return Forbid("ConsumerId claim missing or invalid.");
                    if (bill.Meter?.ConsumerId != cid) return Forbid("Not allowed to update this bill.");
                }

                // Validate status transitions
                if (!string.IsNullOrWhiteSpace(updateDto.Status))
                {
                    var newStatus = updateDto.Status.Trim();
                    var currentStatus = bill.Status;

                    // Allow: Pending -> HalfPaid, Pending -> Paid, HalfPaid -> Paid
                    if (string.Equals(newStatus, "HalfPaid", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!string.Equals(currentStatus, "Pending", StringComparison.OrdinalIgnoreCase))
                            return BadRequest(new { error = $"Cannot change status from '{currentStatus}' to 'HalfPaid'. Only 'Pending' bills can be partially paid." });
                    }
                    else if (string.Equals(newStatus, "Paid", StringComparison.OrdinalIgnoreCase))
                    {
                        if (!string.Equals(currentStatus, "Pending", StringComparison.OrdinalIgnoreCase) 
                            && !string.Equals(currentStatus, "HalfPaid", StringComparison.OrdinalIgnoreCase))
                            return BadRequest(new { error = $"Cannot change status from '{currentStatus}' to 'Paid'. Only 'Pending' or 'HalfPaid' bills can be marked as paid." });
                    }
                    else if (!string.Equals(newStatus, "Pending", StringComparison.OrdinalIgnoreCase))
                    {
                        return BadRequest(new { error = $"Invalid status '{newStatus}'. Allowed values: 'Pending', 'HalfPaid', 'Paid'." });
                    }

                    bill.Status = newStatus;
                }

                // Update amount if provided (for partial payments)
                if (updateDto.Amount.HasValue)
                {
                    if (updateDto.Amount.Value < 0)
                        return BadRequest(new { error = "Amount cannot be negative." });
                    bill.Amount = updateDto.Amount.Value;
                }

                await _ctx.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "DB error updating bill {Id}", id);
                return StatusCode(500, new { error = "Database error while updating bill." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating bill {Id}", id);
                return StatusCode(500, new { error = "Server error while updating bill." });
            }
        }

        // DTO for bill updates
        public class BillUpdateDto
        {
            public string? Status { get; set; }
            public decimal? Amount { get; set; }
        }

        // DTO for bill payment
        public class BillPaymentDto
        {
            public decimal AmountPaid { get; set; }
            public string PaymentMode { get; set; } = string.Empty;
            public DateTime PaymentDate { get; set; }
        }
    }
}
