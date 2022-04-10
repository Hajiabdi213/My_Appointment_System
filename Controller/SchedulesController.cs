using Data;
using Microsoft.AspNetCore.Mvc;
using Models;
using ViewModels;
using Microsoft.EntityFrameworkCore;
using Helpers;
using Microsoft.AspNetCore.Authorization;

namespace Controller;

[Route("[controller]")]
[ApiController]
[Authorize]
public class SchedulesController:ControllerBase
{
    private readonly AppointmentsDbContext _context;

    public SchedulesController(AppointmentsDbContext context)
    {
        _context=context;
        
    }
    //GET/ schedules
    [HttpGet]
    public async Task<IActionResult> GetMySchedules()
    {
        var schedules= await _context.Schedules
                    .Include(s=>s.TimeSlots)
                    .Where(s=>s.Doctor.UserId==User.GetId())
                    .ToListAsync(HttpContext.RequestAborted);
                    


        return Ok(schedules);
    }

    //PUT /schedule/id
    //TODO: Only the owner doctor of the schedule can update it
    [HttpPut("{id:int}")]
    public IActionResult UpdateSchedule(int id, [FromBody]ModifyScheduleViewModel viewModel)
    {
        var schedule = _context.Schedules.Find(id);
        if(schedule is null)
        {
            return BadRequest();
        }

        schedule.Location=viewModel.Location;
        schedule.IsAvailable=viewModel.IsAvailable;
        schedule.Day=viewModel.Day;
        _context.SaveChanges();
        return NoContent();
        
    }

    //POST /schedule 
    [HttpPost]
    public async Task<IActionResult> AddSchedule(ScheduleViewModel scheduleViewModel)
    {
         var doctor = await _context.Doctors
            .Include(d=>d.Schedules.Where(s=>s.Day == scheduleViewModel.Day))
            .SingleOrDefaultAsync(d=>d.UserId==User.GetId(), HttpContext.RequestAborted);

            if(doctor is null)
            {
                return BadRequest("You are not a doctor");
            }

        if (doctor.Schedules.Any())
        {
            return BadRequest("Schedule has been set already!!");
        }

        if(!doctor.IsVerified)
        {
            return BadRequest("You need to be verified first!!");
        }
        var schedule = new Schedule
        {
            Day=scheduleViewModel.Day,
            Location=scheduleViewModel.Location,
            CreatedAt=DateTime.UtcNow,
            IsAvailable=true,
            DoctorId=doctor.Id
        };
       await _context.Schedules.AddAsync(schedule);
        await _context.SaveChangesAsync(HttpContext.RequestAborted);
        return Created("",schedule);
        
    }

    



//TIMESLOTS
//POST/schedules/{id}/timeslots
[HttpPost("{id}/timeslots")]
	public async Task<IActionResult> AddTimeSlot(int id, [FromBody]TimeSlotViewModel viewModel)
	{
       
		var schedule =await _context.Schedules.FindAsync(id);
		if (schedule is null)
		{
			return BadRequest($"Schedule with id {id} cannot be recognized.");
		}
        // return Ok(viewModel);

		var timeslot = new TimeSlot
		{
			StartTime = viewModel.StartTime,
			EndTime = viewModel.EndTime,
			Description = viewModel.Description,
			MaxAppointments = viewModel.MaxAppointments,
			ScheduleId = schedule.Id,
			CreatedAt = DateTime.UtcNow
		};

		_context.TimeSlots.Add(timeslot);
		_context.SaveChanges();

		return Created("", timeslot);
	}




//PUT/schedules/timeslot/id

[HttpPut("timeslots/{id}")]

public IActionResult UpdateTimeslot(int id, [FromBody]TimeSlotViewModel viewModel)
{
    var timeslot = _context.TimeSlots.Find(id);
    if(timeslot is null)
    {
        return BadRequest();
    }

   

    timeslot.StartTime=viewModel.StartTime;
    timeslot.EndTime=viewModel.EndTime;
    timeslot.MaxAppointments=viewModel.MaxAppointments;
    timeslot.Description=viewModel.Description;


    _context.SaveChanges();

    return NoContent();
}
    



}