using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Nodes;
using AMIProjectView.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace AMIProjectView.Controllers
{
    [Authorize]
    public class BillsController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<BillsController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public BillsController(IHttpClientFactory httpClientFactory, ILogger<BillsController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        private void AddBearer(HttpClient client)
        {
            var token = _httpContextAccessor.HttpContext?.Session?.GetString("ApiToken");
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        // GET /Bills?status=&from=&to=&meterId=&page=&pageSize=
        [HttpGet]
        public async Task<IActionResult> Index(string? status, DateTime? from, DateTime? to, string? meterId, int page = 1, int pageSize = 20)
        {
            page = Math.Max(1, page);
            pageSize = Math.Max(1, pageSize);

            var client = _httpClientFactory.CreateClient("api");
            AddBearer(client);

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            try
            {
                var isConsumer = User.HasClaim(c => string.Equals(c.Type, "UserType", StringComparison.OrdinalIgnoreCase) && c.Value == "Consumer");
                int? consumerId = null;
                if (isConsumer) consumerId = TryGetConsumerIdFromClaims();

                // Try API-side filtering/paging first (if consumerId present we request consumerId)
                var url = $"api/bills?page={page}&pageSize={pageSize}";
                if (!string.IsNullOrWhiteSpace(status)) url += $"&status={Uri.EscapeDataString(status)}";
                if (!string.IsNullOrWhiteSpace(meterId)) url += $"&meterId={Uri.EscapeDataString(meterId)}";
                if (from.HasValue) url += $"&from={from.Value:yyyy-MM-dd}";
                if (to.HasValue) url += $"&to={to.Value:yyyy-MM-dd}";
                if (consumerId.HasValue) url += $"&consumerId={consumerId.Value}";

                var resp = await client.GetAsync(url);
                if (resp.IsSuccessStatusCode)
                {
                    // Try parse paged wrapper
                    try
                    {
                        using var stream = await resp.Content.ReadAsStreamAsync();
                        var wrapper = await JsonSerializer.DeserializeAsync<BillsPagedResponse?>(stream, jsonOptions);
                        if (wrapper != null && wrapper.Items != null)
                        {
                            ViewBag.Total = wrapper.Total;
                            ViewBag.Page = wrapper.Page;
                            ViewBag.PageSize = wrapper.PageSize;
                            ViewBag.TotalPages = Math.Max(1, (int)Math.Ceiling(wrapper.Total / (double)wrapper.PageSize));

                            // forward filters for UI
                            ViewBag.StatusFilter = status ?? "";
                            ViewBag.MeterFilter = meterId ?? "";
                            ViewBag.FromFilter = from?.ToString("yyyy-MM-dd") ?? "";
                            ViewBag.ToFilter = to?.ToString("yyyy-MM-dd") ?? "";

                            // Calculate total pending amount for consumers
                            if (isConsumer && consumerId.HasValue)
                            {
                                // Get all bills for the consumer
                                var allBills = await client.GetFromJsonAsync<List<BillVm>>("api/bills", jsonOptions) ?? new List<BillVm>();
                                var meters = await client.GetFromJsonAsync<List<MeterViewModel>>($"api/meters?consumerId={consumerId.Value}", jsonOptions) ?? new List<MeterViewModel>();
                                var meterIds = new HashSet<string>(meters.Select(m => m.MeterSerialNo));
                                allBills = allBills.Where(b => meterIds.Contains(b.MeterID)).ToList();
                                
                                // Calculate total pending: Sum of (Amount - AmountPaid) for Pending and HalfPaid bills
                                var pendingAndHalfPaidBills = allBills.Where(b => 
                                    string.Equals(b.Status, "Pending", StringComparison.OrdinalIgnoreCase) || 
                                    string.Equals(b.Status, "HalfPaid", StringComparison.OrdinalIgnoreCase)).ToList();
                                
                                var totalPendingAmount = pendingAndHalfPaidBills.Sum(b => b.Amount - b.AmountPaid);
                                
                                ViewBag.TotalPendingAmount = totalPendingAmount;
                                ViewBag.ConsumerBalance = GetConsumerBalance(consumerId.Value);
                            }

                            return View(wrapper.Items);
                        }
                    }
                    catch
                    {
                        // ignore and try array fallback
                    }

                    // Array fallback (non-paged API)
                    try
                    {
                        var arr = await resp.Content.ReadFromJsonAsync<List<BillVm>>(jsonOptions);
                        if (arr != null)
                        {
                            // apply server filters again on client side if necessary
                            if (!string.IsNullOrWhiteSpace(status))
                                arr = arr.Where(b => string.Equals(b.Status, status, StringComparison.OrdinalIgnoreCase)).ToList();

                            if (!string.IsNullOrWhiteSpace(meterId))
                                arr = arr.Where(b => string.Equals(b.MeterID, meterId, StringComparison.OrdinalIgnoreCase)).ToList();

                            if (from.HasValue)
                                arr = arr.Where(b => b.MonthStartDate >= from.Value.Date).ToList();

                            if (to.HasValue)
                                arr = arr.Where(b => b.MonthStartDate <= to.Value.Date).ToList();

                            arr = arr.OrderByDescending(b => b.MonthStartDate).ToList();
                            var total = arr.Count;
                            var items = arr.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                            ViewBag.Total = total;
                            ViewBag.Page = page;
                            ViewBag.PageSize = pageSize;
                            ViewBag.TotalPages = Math.Max(1, (int)Math.Ceiling(total / (double)pageSize));

                            ViewBag.StatusFilter = status ?? "";
                            ViewBag.MeterFilter = meterId ?? "";
                            ViewBag.FromFilter = from?.ToString("yyyy-MM-dd") ?? "";
                            ViewBag.ToFilter = to?.ToString("yyyy-MM-dd") ?? "";

                            // Calculate total pending amount for consumers
                            if (isConsumer && consumerId.HasValue)
                            {
                                // Get all bills for the consumer
                                var allBills = await client.GetFromJsonAsync<List<BillVm>>("api/bills", jsonOptions) ?? new List<BillVm>();
                                var meters = await client.GetFromJsonAsync<List<MeterViewModel>>($"api/meters?consumerId={consumerId.Value}", jsonOptions) ?? new List<MeterViewModel>();
                                var meterIds = new HashSet<string>(meters.Select(m => m.MeterSerialNo));
                                allBills = allBills.Where(b => meterIds.Contains(b.MeterID)).ToList();
                                
                                // Calculate total pending: Sum of (Amount - AmountPaid) for Pending and HalfPaid bills
                                var pendingAndHalfPaidBills = allBills.Where(b => 
                                    string.Equals(b.Status, "Pending", StringComparison.OrdinalIgnoreCase) || 
                                    string.Equals(b.Status, "HalfPaid", StringComparison.OrdinalIgnoreCase)).ToList();
                                
                                var totalPendingAmount = pendingAndHalfPaidBills.Sum(b => b.Amount - b.AmountPaid);
                                
                                ViewBag.TotalPendingAmount = totalPendingAmount;
                                ViewBag.ConsumerBalance = GetConsumerBalance(consumerId.Value);
                            }

                            return View(items);
                        }
                    }
                    catch
                    {
                        // final fallback
                    }
                }

                // fallback: get all (non-paged) and do local filtering
                var all = await client.GetFromJsonAsync<List<BillVm>>("api/bills", jsonOptions) ?? new List<BillVm>();

                // If consumer -> call meters endpoint and filter by meter serial numbers
                if (consumerId.HasValue)
                {
                    var meters = await client.GetFromJsonAsync<List<MeterViewModel>>($"api/meters?consumerId={consumerId.Value}", jsonOptions) ?? new List<MeterViewModel>();
                    var meterIds = new HashSet<string>(meters.Select(m => m.MeterSerialNo));
                    all = all.Where(b => meterIds.Contains(b.MeterID)).ToList();
                }

                // apply filters
                if (!string.IsNullOrWhiteSpace(status))
                    all = all.Where(b => string.Equals(b.Status, status, StringComparison.OrdinalIgnoreCase)).ToList();

                if (!string.IsNullOrWhiteSpace(meterId))
                    all = all.Where(b => string.Equals(b.MeterID, meterId, StringComparison.OrdinalIgnoreCase)).ToList();

                if (from.HasValue)
                    all = all.Where(b => b.MonthStartDate >= from.Value.Date).ToList();

                if (to.HasValue)
                    all = all.Where(b => b.MonthStartDate <= to.Value.Date).ToList();

                // ordering and paging
                all = all.OrderByDescending(b => b.MonthStartDate).ToList();
                var totalCount = all.Count;
                var paged = all.Skip((page - 1) * pageSize).Take(pageSize).ToList();

                ViewBag.Total = totalCount;
                ViewBag.Page = page;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)pageSize));

                ViewBag.StatusFilter = status ?? "";
                ViewBag.MeterFilter = meterId ?? "";
                ViewBag.FromFilter = from?.ToString("yyyy-MM-dd") ?? "";
                ViewBag.ToFilter = to?.ToString("yyyy-MM-dd") ?? "";

                // Calculate total pending amount for consumers
                if (isConsumer && consumerId.HasValue)
                {
                    // Get all bills for the consumer
                    var allBills = await client.GetFromJsonAsync<List<BillVm>>("api/bills", jsonOptions) ?? new List<BillVm>();
                    if (consumerId.HasValue)
                    {
                        var meters = await client.GetFromJsonAsync<List<MeterViewModel>>($"api/meters?consumerId={consumerId.Value}", jsonOptions) ?? new List<MeterViewModel>();
                        var meterIds = new HashSet<string>(meters.Select(m => m.MeterSerialNo));
                        allBills = allBills.Where(b => meterIds.Contains(b.MeterID)).ToList();
                    }
                    
                    // Calculate total pending: Sum of (Amount - AmountPaid) for Pending and HalfPaid bills
                    var pendingAndHalfPaidBills = allBills.Where(b => 
                        string.Equals(b.Status, "Pending", StringComparison.OrdinalIgnoreCase) || 
                        string.Equals(b.Status, "HalfPaid", StringComparison.OrdinalIgnoreCase)).ToList();
                    
                    var totalPendingAmount = pendingAndHalfPaidBills.Sum(b => b.Amount - b.AmountPaid);
                    
                    ViewBag.TotalPendingAmount = totalPendingAmount;
                    ViewBag.ConsumerBalance = GetConsumerBalance(consumerId.Value);
                }

                return View(paged);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load bills");
                TempData["err"] = "Unable to load bills: " + ex.Message;
                return View(new List<BillVm>());
            }
        }

        // GET: /Bills/Details/{id}
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var client = _httpClientFactory.CreateClient("api");
            AddBearer(client);

            try
            {
                var resp = await client.GetAsync($"api/bills/{id}");
                if (resp.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    TempData["err"] = "Bill not found.";
                    return RedirectToAction(nameof(Index));
                }
                if (resp.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    TempData["err"] = "Not authorized to view this bill.";
                    return RedirectToAction(nameof(Index));
                }

                resp.EnsureSuccessStatusCode();
                var bill = await resp.Content.ReadFromJsonAsync<BillVm>(new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (bill == null)
                {
                    TempData["err"] = "Unable to read bill.";
                    return RedirectToAction(nameof(Index));
                }

                return View(bill);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed loading bill details");
                TempData["err"] = "Unable to load bill: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: /Bills/PayBill
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PayBill(int id)
        {
            var client = _httpClientFactory.CreateClient("api");
            AddBearer(client);
            try
            {
                var resp = await client.PostAsync($"api/bills/pay/{id}", null);
                if (resp.IsSuccessStatusCode || resp.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    TempData["msg"] = "Payment successful. Bill marked as Paid.";
                }
                else
                {
                    // try to extract error
                    var body = await resp.Content.ReadAsStringAsync();
                    TempData["err"] = $"Payment failed: {(int)resp.StatusCode} {resp.ReasonPhrase}. {body}";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Payment failed for bill {Id}", id);
                TempData["err"] = "Payment failed: " + ex.Message;
            }

            // Redirect back to Index or referring page
            return RedirectToAction(nameof(Index));
        }

        // POST: /Bills/PayBills
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PayBills(decimal amount)
        {
            // Only allow consumers to use this endpoint
            var isConsumer = User.HasClaim(c => string.Equals(c.Type, "UserType", StringComparison.OrdinalIgnoreCase) && c.Value == "Consumer");
            if (!isConsumer)
            {
                TempData["err"] = "This payment method is only available for consumers.";
                return RedirectToAction(nameof(Index));
            }

            if (amount <= 0)
            {
                TempData["err"] = "Payment amount must be greater than zero.";
                return RedirectToAction(nameof(Index));
            }

            var client = _httpClientFactory.CreateClient("api");
            AddBearer(client);

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            try
            {
                int? consumerId = TryGetConsumerIdFromClaims();
                if (!consumerId.HasValue)
                {
                    TempData["err"] = "Unable to identify consumer.";
                    return RedirectToAction(nameof(Index));
                }

                // Get existing balance and add to payment amount
                decimal existingBalance = GetConsumerBalance(consumerId.Value);
                decimal totalPayment = amount + existingBalance;
                
                // Clear existing balance since we're using it
                if (existingBalance > 0)
                {
                    var session = _httpContextAccessor.HttpContext?.Session;
                    session?.Remove($"ConsumerBalance_{consumerId.Value}");
                }

                decimal remainingPayment = totalPayment;
                int paidCount = 0;
                int halfPaidCount = 0;
                List<string> errors = new List<string>();

                // Keep processing bills until payment is exhausted or no more bills to pay
                bool continueProcessing = true;
                int maxIterations = 10; // Safety limit
                int iteration = 0;

                while (remainingPayment > 0 && continueProcessing && iteration < maxIterations)
                {
                    iteration++;
                    
                    // Get all bills with remaining balance, ordered by MonthStartDate (oldest first)
                    var billsWithBalance = await GetAllPendingBillsForConsumer(client, consumerId, jsonOptions);
                    billsWithBalance = billsWithBalance.OrderBy(b => b.MonthStartDate).ToList();

                    if (!billsWithBalance.Any())
                    {
                        // No more bills to pay - store remaining as balance
                        if (remainingPayment > 0)
                        {
                            await StoreConsumerBalance(client, consumerId.Value, remainingPayment, jsonOptions);
                            _logger.LogInformation("No more bills to pay. Stored remaining balance: {Balance}", remainingPayment);
                        }
                        break;
                    }

                    _logger.LogInformation("Iteration {Iteration}: Processing {Count} bills with remaining balance. Remaining payment: {Payment}", 
                        iteration, billsWithBalance.Count, remainingPayment);

                    bool paymentApplied = false;

                    foreach (var bill in billsWithBalance)
                    {
                        if (remainingPayment <= 0)
                        {
                            break;
                        }

                        // Calculate remaining balance for this bill
                        decimal billRemainingBalance = bill.Amount - bill.AmountPaid;
                        
                        if (billRemainingBalance <= 0)
                        {
                            continue; // Skip fully paid bills
                        }
                        
                        decimal paymentForThisBill = Math.Min(remainingPayment, billRemainingBalance);
                        
                        _logger.LogInformation("Processing bill {BillId}: OriginalAmount={OriginalAmount}, AmountPaid={AmountPaid}, RemainingBalance={RemainingBalance}, Payment={Payment}, RemainingPayment={RemainingPayment}", 
                            bill.BillId, bill.Amount, bill.AmountPaid, billRemainingBalance, paymentForThisBill, remainingPayment);

                        // Use the new PUT endpoint to process payment
                        var paymentDto = new
                        {
                            AmountPaid = paymentForThisBill,
                            PaymentMode = "Online",
                            PaymentDate = DateTime.UtcNow
                        };

                        try
                        {
                            var payResp = await client.PutAsJsonAsync($"api/bills/PayBill/{bill.BillId}", paymentDto);
                            
                            if (payResp.IsSuccessStatusCode)
                            {
                                var responseJson = await payResp.Content.ReadAsStringAsync();
                                var response = JsonSerializer.Deserialize<JsonObject>(responseJson, jsonOptions);
                                var status = response?["status"]?.ToString();
                                
                                if (string.Equals(status, "Paid", StringComparison.OrdinalIgnoreCase))
                                {
                                    paidCount++;
                                }
                                else if (string.Equals(status, "HalfPaid", StringComparison.OrdinalIgnoreCase))
                                {
                                    halfPaidCount++;
                                }
                                
                                remainingPayment -= paymentForThisBill;
                                paymentApplied = true;
                                _logger.LogInformation("Successfully processed payment for bill {BillId}. Status: {Status}, Remaining payment: {Remaining}", 
                                    bill.BillId, status, remainingPayment);
                            }
                            else
                            {
                                var errorBody = await payResp.Content.ReadAsStringAsync();
                                var errorMsg = $"Failed to pay bill {bill.BillId}: {payResp.StatusCode} - {errorBody}";
                                errors.Add(errorMsg);
                                _logger.LogError("Failed to pay bill {BillId}: {Status} - {Error}", bill.BillId, payResp.StatusCode, errorBody);
                            }
                        }
                        catch (Exception ex)
                        {
                            var errorMsg = $"Exception processing bill {bill.BillId}: {ex.Message}";
                            errors.Add(errorMsg);
                            _logger.LogError(ex, "Exception processing payment for bill {BillId}", bill.BillId);
                        }
                    }

                    // If no payment was applied in this iteration, stop to avoid infinite loop
                    if (!paymentApplied)
                    {
                        _logger.LogWarning("No payment was applied in iteration {Iteration}. Stopping.", iteration);
                        break;
                    }
                }

                _logger.LogInformation("Finished processing. Paid: {Paid}, HalfPaid: {HalfPaid}, Remaining: {Remaining}", 
                    paidCount, halfPaidCount, remainingPayment);

                if (paidCount > 0 || halfPaidCount > 0)
                {
                    var msg = $"Payment processed: {paidCount} bill(s) paid in full";
                    if (halfPaidCount > 0)
                        msg += $", {halfPaidCount} bill(s) partially paid";
                    if (remainingPayment > 0)
                    {
                        var remainingBills = await GetAllPendingBillsForConsumer(client, consumerId, jsonOptions);
                        if (!remainingBills.Any())
                        {
                            msg += $". Balance of ₹{remainingPayment:F2} stored for future bills";
                        }
                    }
                    if (errors.Any())
                    {
                        msg += ". Some errors occurred: " + string.Join("; ", errors);
                    }
                    TempData["msg"] = msg;
                }
                else
                {
                    var errorMsg = "Payment could not be processed.";
                    if (errors.Any())
                        errorMsg += " " + string.Join("; ", errors);
                    TempData["err"] = errorMsg;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Payment processing failed");
                TempData["err"] = "Payment failed: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<List<BillVm>> GetAllPendingBillsForConsumer(HttpClient client, int? consumerId, JsonSerializerOptions jsonOptions)
        {
            // Get all bills for the consumer
            var allBills = await client.GetFromJsonAsync<List<BillVm>>("api/bills", jsonOptions) ?? new List<BillVm>();

            // Filter by consumer's meters
            if (consumerId.HasValue)
            {
                var meters = await client.GetFromJsonAsync<List<MeterViewModel>>($"api/meters?consumerId={consumerId.Value}", jsonOptions) ?? new List<MeterViewModel>();
                var meterIds = new HashSet<string>(meters.Select(m => m.MeterSerialNo));
                allBills = allBills.Where(b => meterIds.Contains(b.MeterID)).ToList();
            }

            // Return bills that have remaining balance (Pending or HalfPaid with remaining amount)
            return allBills.Where(b => 
                (string.Equals(b.Status, "Pending", StringComparison.OrdinalIgnoreCase) || 
                 string.Equals(b.Status, "HalfPaid", StringComparison.OrdinalIgnoreCase)) &&
                (b.Amount - b.AmountPaid) > 0).ToList();
        }

        private async Task StoreConsumerBalance(HttpClient client, int consumerId, decimal balance, JsonSerializerOptions jsonOptions)
        {
            // Store balance in session for now (can be moved to database later)
            // For now, we'll store it in session and calculate from HalfPaid bills
            try
            {
                var session = _httpContextAccessor.HttpContext?.Session;
                if (session != null)
                {
                    var currentBalance = session.GetString($"ConsumerBalance_{consumerId}");
                    decimal existingBalance = 0;
                    if (!string.IsNullOrEmpty(currentBalance) && decimal.TryParse(currentBalance, out existingBalance))
                    {
                        balance += existingBalance;
                    }
                    session.SetString($"ConsumerBalance_{consumerId}", balance.ToString("F2"));
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to store consumer balance in session");
            }
        }

        private decimal GetConsumerBalance(int consumerId)
        {
            try
            {
                var session = _httpContextAccessor.HttpContext?.Session;
                if (session != null)
                {
                    var balanceStr = session.GetString($"ConsumerBalance_{consumerId}");
                    if (!string.IsNullOrEmpty(balanceStr) && decimal.TryParse(balanceStr, out var balance))
                    {
                        return balance;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to get consumer balance from session");
            }
            return 0;
        }

        private int? TryGetConsumerIdFromClaims()
        {
            // Try from claims first
            var c = User.Claims.FirstOrDefault(x => string.Equals(x.Type, "ConsumerId", StringComparison.OrdinalIgnoreCase));
            if (c != null && int.TryParse(c.Value, out var cid)) return cid;
            
            // Try from NameIdentifier claim
            var nid = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(nid) && int.TryParse(nid, out var nidInt)) return nidInt;
            
            // Try from session
            var s = _httpContextAccessor.HttpContext?.Session?.GetString("ConsumerId");
            if (!string.IsNullOrEmpty(s) && int.TryParse(s, out var sid)) return sid;
            
            // Fallback: try to extract from JWT token in session
            var token = _httpContextAccessor.HttpContext?.Session?.GetString("ApiToken");
            if (!string.IsNullOrEmpty(token))
            {
                var consumerIdFromToken = ExtractConsumerIdFromToken(token);
                if (consumerIdFromToken.HasValue)
                {
                    // Cache it in session for future use
                    _httpContextAccessor.HttpContext?.Session?.SetString("ConsumerId", consumerIdFromToken.Value.ToString());
                    return consumerIdFromToken;
                }
            }
            
            return null;
        }

        private int? ExtractConsumerIdFromToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return null;

            try
            {
                // JWT tokens have 3 parts separated by dots: header.payload.signature
                var parts = token.Split('.');
                if (parts.Length != 3)
                    return null;

                // Decode the payload (second part)
                var payload = parts[1];
                
                // Add padding if needed (base64url encoding)
                var padding = payload.Length % 4;
                if (padding != 0)
                {
                    payload += new string('=', 4 - padding);
                }
                payload = payload.Replace('-', '+').Replace('_', '/');

                var payloadBytes = Convert.FromBase64String(payload);
                var payloadJson = System.Text.Encoding.UTF8.GetString(payloadBytes);
                
                // Parse JSON to find ConsumerId claim
                var jsonDoc = JsonDocument.Parse(payloadJson);
                if (jsonDoc.RootElement.TryGetProperty("ConsumerId", out var consumerIdElement))
                {
                    if (consumerIdElement.ValueKind == JsonValueKind.Number)
                    {
                        return consumerIdElement.GetInt32();
                    }
                    else if (consumerIdElement.ValueKind == JsonValueKind.String)
                    {
                        if (int.TryParse(consumerIdElement.GetString(), out var cid))
                            return cid;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to extract ConsumerId from token");
            }

            return null;
        }

        private class BillsPagedResponse
        {
            public int Total { get; set; }
            public int Page { get; set; }
            public int PageSize { get; set; }
            public List<BillVm>? Items { get; set; }
        }

        // Minimal meter view model to support consumer-meter filtering
        private class MeterViewModel
        {
            public string MeterSerialNo { get; set; } = "";
            public int MeterId { get; set; }
        }
    }
}
