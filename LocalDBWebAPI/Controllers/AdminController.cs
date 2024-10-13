using API_Classes;
using LocalDBWebAPI.Data;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace LocalDBWebAPI.Controllers
{
    [Route("admin/[controller]")]

    public class AdminController : Controller
    {
        [HttpGet("BySearch/{searchString}")]
        public IActionResult GetUsers(string searchString)
        {
            List<UserDataIntermed> users = DBManager.SearchUsersByName(searchString);
            Debug.WriteLine("Users found: " + users.Count);
            if (users == null || !users.Any())
            {
                DBManager.AddLogEntry("transaction search failed due to not finding any transaction under user " + searchString);
                return NotFound("No transactions have been found in the database.");
            }
            DBManager.AddLogEntry("transaction search successful, returning list of transaction under " + searchString);

            return Ok(users);
        }

        [HttpGet("auditLogs")]
        public IActionResult GetLogs()
        {
            List<LogDataIntermed> logEntries = DBManager.GetAllLogs();
            Debug.WriteLine("Entries found: " + logEntries.Count);
            if (logEntries == null || !logEntries.Any())
            {
                return NotFound("No transactions have been found in the database.");
            }
            return Ok(logEntries);
        }
    }
}
