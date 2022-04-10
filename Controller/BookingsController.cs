using Helpers;
using Microsoft.AspNetCore.Mvc;
namespace Controller;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Models;
using ViewModels;

[Route("[controller]")]
[ApiController]
 [Authorize]
public class BookingsController : ControllerBase
{
    private readonly AppointmentsDbContext _context;

    public BookingsController(AppointmentsDbContext context)
    {
        _context = context;

    }


    //GET /Bookings
  
    [HttpGet]
   
    public async Task<IActionResult> GetAll()
    {      
        
        var bookings = await _context.Booking.ToListAsync();
        return Ok(bookings);

  

    }



    //POST /Bookings

    [HttpPost]
    public async Task<ActionResult> AddBooking([FromBody] BookingViewModel viewModel)
    {
        var timeSlot = await _context.TimeSlots
        .Include(ts => ts.Schedule).ThenInclude(s => s.Doctor)
        .Include(ts => ts.Bookings)
        .SingleOrDefaultAsync(ts => ts.Id == viewModel.TimeSlotId);

        //checking if the selected timeslot exist or not

        if (timeSlot is null)
        {
            return BadRequest("Selected time-slot could not be recognized.");
        }

       

        //checking if hte selected time is past

        if (viewModel.AppointmentTime < DateTime.Today)
        {
            return BadRequest("The selected Appointmet time cannot be in the past! 😁");
        }
        
        //checking if the selected day exist in the schedule

        if (viewModel.AppointmentTime.DayOfWeek != timeSlot.Schedule.Day)
        {
            return BadRequest("Doctor is not available at the selected day!");
        }
        //checking if max number of appointments reached

        if (timeSlot.MaxAppointments <= timeSlot.Bookings.Count)
        {
            return BadRequest("Wuu kaa buuxaa maanta!!");
        }


        //Finding the commission, doctor revenue, doctor ticket price
        var ticketPrice = timeSlot.Schedule.Doctor.TicketPrice;
        var rate=0.02m;
        var commission = ticketPrice * rate;

    //TODO: add real paymet method gateways

    // var userId = User.Claims.First(c=>c.Type=="sub");
    // var userId= User.FindFirst("sub")?.Value;
    // var userId= User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
   
    
    
        var transactionID= new Random().Next(10_000, 999_999);

        var booking = new Booking
        {
            AppointmentTime = viewModel.AppointmentTime,
            IsCompleted = false,
            PaidAmount = ticketPrice,
            Commission=commission,
            DoctorRevenue= ticketPrice-commission,
            PaymentMethod=viewModel.PaymentMethod,
            CreatedAt=DateTime.UtcNow,
            UserId=User.GetId(),
            TransactionId=transactionID,
            TimeSlotId=timeSlot.Id



        };

        _context.Booking.Add(booking);
       await _context.SaveChangesAsync();
        return Created("", booking);
    }
}