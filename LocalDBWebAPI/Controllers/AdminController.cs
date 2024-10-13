﻿using API_Classes;
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
                return NotFound("No transactions have been found in the database.");
            }
            return Ok(users);
        }
    }
}
