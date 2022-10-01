﻿using System.Text.RegularExpressions;
using AcademicManagementSystem.Context;
using AcademicManagementSystem.Context.AmsModels;
using AcademicManagementSystem.Models.CenterController;
using AcademicManagementSystem.Models.RoomController.RoomModel;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Mvc;

namespace AcademicManagementSystem.Controllers;

[ApiController]
public class CenterController : ControllerBase
{
    private readonly AmsContext _context;
    
    public CenterController(AmsContext context)
    {
        _context = context;
    }
    
    [HttpGet]
    [Route("api/centers")]
    public IActionResult GetCenters()
    {
        var centers = _context.Centers.ToList()
            .Select(c => new CenterResponse()
            {
                Id = c.Id, ProvinceId = c.ProvinceId, DistrictId = c.DistrictId, WardId = c.WardId,
                Name = c.Name, CreatedAt = c.CreatedAt, UpdatedAt = c.UpdatedAt
            });
        if(!centers.Any())
            return BadRequest(CustomResponse.BadRequest("Center not found", "center-error-000001"));
        return Ok(CustomResponse.Ok("Get all centers success", centers));
    }
    
}