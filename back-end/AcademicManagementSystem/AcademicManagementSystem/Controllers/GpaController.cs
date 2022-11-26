using AcademicManagementSystem.Context;
using AcademicManagementSystem.Models.GpaController;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcademicManagementSystem.Controllers;

[ApiController]
public class GpaController : ControllerBase
{
    private readonly AmsContext _context;

    public GpaController(AmsContext context)
    {
        _context = context;
    }

    // get all form
    [HttpGet]
    [Route("api/gpa/forms")]
    [Authorize(Roles = "admin, sro, student")]
    public IActionResult GetForms()
    {
        var forms = _context.Forms
            .Select(f => new FormResponse()
            {
                Id = f.Id, Title = f.Title, Description = f.Description
            }).ToList();
        return Ok(CustomResponse.Ok("Forms retrieved successfully", forms));
    }

    // get all question by form id
    [HttpGet]
    [Route("api/gpa/forms/{formId:int}/questions")]
    [Authorize(Roles = "admin, sro, student")]
    public IActionResult GetQuestionsByFormId(int formId)
    {
        // check if form exists
        var form = _context.Forms.Find(formId);
        if (form == null)
        {
            return NotFound(CustomResponse.NotFound("Form not found with id " + formId));
        }

        var questions = _context.Questions
            .Include(q => q.Forms)
            .Where(q => q.Forms.Any(f => f.Id == formId))
            .Select(q => new QuestionResponse()
            {
                Id = q.Id, Content = q.Content
            }).ToList();
        return Ok(CustomResponse.Ok("Questions retrieved successfully", questions));
    }

    // get all answer by question id
    [HttpGet]
    [Route("api/gpa/forms/{formId:int}/questions/{questionId:int}/answers")]
    [Authorize(Roles = "admin, sro, student")]
    public IActionResult GetAnswersByQuestionId(int formId, int questionId)
    {
        // check if form exists or not
        var form = _context.Forms.Find(formId);
        if (form == null)
        {
            return NotFound(CustomResponse.NotFound("Form not found with id " + formId));
        }

        // check if question in form exists or not
        var question = _context.Questions
            .Include(q => q.Forms)
            .Where(q => q.Forms.Any(f => f.Id == formId))
            .FirstOrDefault(q => q.Id == questionId);
        if (question == null)
        {
            return NotFound(
                CustomResponse.NotFound("Question not found with id " + questionId + " in form id " + formId));
        }

        var answers = _context.Answers
            .Include(a => a.Question)
            .Include(a => a.Question.Forms)
            .Where(a => a.Question.Forms.Any(f => f.Id == formId) && a.Question.Id == questionId)
            .Select(a => new AnswerResponse()
            {
                Id = a.Id, QuestionId = a.QuestionId, AnswerNo = a.AnswerNo, Content = a.Content
            })
            .ToList();
        return Ok(CustomResponse.Ok("Answers retrieved successfully", answers));
    }

    // get all question and answer by form id
    [HttpGet]
    [Route("api/gpa/forms/{formId:int}/questions/answers")]
    [Authorize(Roles = "admin, sro, student")]
    public IActionResult GetQuestionsAndAnswersByFormId(int formId)
    {
        // check if form exists
        var form = _context.Forms.Find(formId);
        if (form == null)
        {
            return NotFound(CustomResponse.NotFound("Form not found with id " + formId));
        }

        var questions = _context.Questions
            .Include(q => q.Forms)
            .Include(q => q.Answers)
            .Where(q => q.Forms.Any(f => f.Id == formId))
            .Select(q => new QuestionResponse()
            {
                Id = q.Id, Content = q.Content, Answer = q.Answers.Select(a => new AnswerResponse()
                {
                    Id = a.Id, QuestionId = a.QuestionId, AnswerNo = a.AnswerNo, Content = a.Content
                }).ToList()
            })
            .ToList();
        foreach (var question in questions)
        {
            var answers = _context.Answers
                .Include(a => a.Question)
                .Include(a => a.Question.Forms)
                .Where(a => a.Question.Forms.Any(f => f.Id == formId) && a.Question.Id == question.Id)
                .Select(a => new AnswerResponse()
                {
                    Id = a.Id, QuestionId = a.QuestionId, AnswerNo = a.AnswerNo, Content = a.Content
                })
                .ToList();
            question.Answer = answers;
        }

        return Ok(CustomResponse.Ok("Questions and answers retrieved successfully", questions));
    }
}