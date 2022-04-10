namespace Controller;
using Data;
using Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using ViewModels;

[Route("[controller]")]
[Authorize]
public class DoctorsController:ControllerBase
{
    private readonly AppointmentsDbContext _context;
    public DoctorsController(AppointmentsDbContext context)
    {
        _context = context;
        
    }



    //GET / doctors
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetDoctors(int page, int size, string phone)
    {
        //pagination and filtering
        var query = _context.Doctors
			.Include(d => d.User)
			.Skip(page * size)
			.Take(size);

		if (!string.IsNullOrEmpty(phone))
		{
			query = query.Where(d => d.Phone == phone);
		}

		var doctors = await query
			.OrderBy(d => d.Id)
			.ToListAsync(HttpContext.RequestAborted);

		return Ok(doctors);
        
    }

    //Get /doctor/id
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Doctor(int id)
    {   
        var doctor =await _context.Doctors.Include(d=>d.User).SingleOrDefaultAsync(d=>d.Id==id, HttpContext.RequestAborted);
        if(doctor is null)
        {
            return BadRequest();
        }
        return Ok(doctor);
    }


    //POST /doctor
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> AddDoctor([FromBody]DoctorViewModel doctorViewModel ){

        var doctor = await _context.Doctors
                    .SingleOrDefaultAsync(d=>d.UserId==User.GetId());
        if(doctor is not null)
        {
            return BadRequest("You are already a doctor");
        }

       doctor = new Doctor{
            Phone=doctorViewModel.Phone,
            Bio=doctorViewModel.Bio,
            Certificate=doctorViewModel.Certificate,
            CreatedAt=DateTime.UtcNow,
            Specialty=doctorViewModel.Specialty,
            Picture=doctorViewModel.Picture,
            TicketPrice=doctorViewModel.TicketPrice,
         IsVerified=false,
            UserId=User.GetId()
        };

      await  _context.Doctors.AddAsync(doctor);
      await  _context.SaveChangesAsync(HttpContext.RequestAborted);
        return Created("",doctor);
    }
    

    //GET Doctors/specialties
    [HttpGet("specialties")]
    [AllowAnonymous]
    public async Task<IActionResult> GetSpecialties()
    {
        var specialties =await _context.Doctors
                            .GroupBy(d=>d.Specialty)
                            .Select(g=> new{
                                Specialty=g.Key,
                                Count = g.Count()
                            })
                            .ToListAsync(HttpContext.RequestAborted);
                        
        return Ok(specialties);
    }

}