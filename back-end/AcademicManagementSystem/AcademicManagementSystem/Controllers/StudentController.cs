using AcademicManagementSystem.Context;
using AcademicManagementSystem.Models.UserController.StudentController;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcademicManagementSystem.Controllers;

[ApiController]
public class StudentController:ControllerBase
{
    private readonly AmsContext _context;
    
    public StudentController(AmsContext context)
    {
        _context = context;
    }
    
    // // get all students
    // [HttpGet]
    // public IActionResult Get()
    // {
    //     // var students = _context.Users.Include()
    //     //     .Select(s => new StudentResponse()
    //     //     {
    //     //         UserId = s.UserId, Avatar = s.Avatar, Name = s.Name, Email = s.Email, Phone = s.Phone, Address = s.Address, CourseId = s.CourseId, CourseName = s.Course.Name
    //     //     }).ToList();
    //     // return Ok(students);
    // }
}