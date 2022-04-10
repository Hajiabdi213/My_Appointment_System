using Microsoft.AspNetCore.Mvc;
using Data;
namespace Controller;
using Models;

[Route("[controller]")]
public class UsersController : ControllerBase
{

    private readonly AppointmentsDbContext _context;
    public UsersController(AppointmentsDbContext context)
    {
        _context = context;
    }


    //GET users
    [HttpGet]
    public IActionResult Get()
    {
        var allUsers = _context.Users.ToList();
        return Ok(allUsers);
    }


    //GET /users/id

    [HttpGet("{id}")]
    public IActionResult GetUser(int id)
    {
        var user = _context.Users.FirstOrDefault(u => u.Id == id);
        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);

    }



    //POST /user
    [HttpPost]
    public IActionResult Add([FromBody] User user)

    {
        _context.Users.Add(user);
        _context.SaveChanges();

        return Created("", user);

    }

    //PUT /users

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] User user)
    {
        var targetUser = _context.Users.FirstOrDefault(u => u.Id == id);
        if (targetUser is null)
        {
            return BadRequest();

        }

        targetUser.Id = id;
        targetUser.FullName = user.FullName;
        targetUser.Address = user.Address;
        targetUser.Email = user.Email;
        

        _context.SaveChanges();

        return NoContent();

    }


    //DELETE /users/id
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var user = _context.Users.Find(id);
        if (user is null)
        {
            return BadRequest();
        }

        _context.Users.Remove(user);
        _context.SaveChanges();
        return NoContent();
    }
}
