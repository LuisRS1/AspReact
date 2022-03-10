#nullable disable

using Microsoft.AspNetCore.Authorization;

namespace AspReactBackEnd.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ContextDB _context;

        public UsersController(ContextDB context)
        {
            _context = context;
        }

        // GET: api/Users/index
        [HttpGet("index")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // GET: api/Users/details/5
        [HttpGet("details/{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);


            if (user == null)
            {
                
                
                return NotFound();
            }
            return user;
        }

        // POST: api/Users/create
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("create")]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(user.Password);
            user.Password = passwordHash;
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        // PUT: api/Users/update/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("update/{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {

            //string passwordHash = BCrypt.Net.BCrypt.HashPassword(user.Password);

            if (id != user.Id)
            {
                return BadRequest();
            }

            var result = await _context.Users
                .FirstOrDefaultAsync(e => e.Id == id);

            if (result.Password != user.Password)
            {
                result.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            }

            if ((result.Name != user.Name ||
                 result.Email != user.Email)
                 && result != null)
            {
                result.Name = user.Name;
                result.Email = user.Email;
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }


            return NoContent();
        }

        // DESACTIVATE USER - SOFT DELETE: api/Users/desactivateUser/{id}
        [HttpPut("desactivateUser/{id}")]
        public async Task<User> DesactivateEmployee(int id)
        {
            var result = await _context.Users
                .FirstOrDefaultAsync(e => e.Id == id);
            if (result != null)
            {

                result.DeletedAt = DateTime.Parse(DateTime.Now.ToString("s"));
                await _context.SaveChangesAsync();
                return result;
            }
            return null;
        }

        // ACTIVATE USER - REMOVE SOFT DELETE api/Users/desactivateUser/{id}
        [HttpPut("activateUser/{id}")]
        public async Task<User> ActivateEmployee(int id)
        {
            var result = await _context.Users
                .FirstOrDefaultAsync(e => e.Id == id);
            if (result != null)
            {

                result.DeletedAt = null;
                await _context.SaveChangesAsync();
                return result;
            }
            return null;
        }

        // PERMANENT DELETE: api/Users/delete/5
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
