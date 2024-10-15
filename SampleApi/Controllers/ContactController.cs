using Microsoft.AspNetCore.Mvc;
using SampleApi.Models;
using SampleApi.Services;
using System.Security.AccessControl;

namespace SampleApi.Controllers;

[Route("api/contact")]
[ApiController]
public class ContactController(IDbRepo db) : ControllerBase
{
    private readonly IDbRepo db = db;

    [HttpGet("count-all")]
    public async Task<ActionResult<int>> CountAll() => await db.CountAllContactAsync();

    [HttpGet("get")]
    public async Task<ActionResult<Contact>> Get([FromQuery] int id) => await db.GetContactAsync(id);

    [HttpGet("list-all")]
    public async Task<ActionResult<List<Contact>>> ListAll() => await db.ListAllContactAsync();

    [HttpGet("search")]
    public async Task<ActionResult<List<Contact>>> Search([FromQuery] string? keyword) => await db.SearchContactAsync(keyword);

    [HttpPost("add")]
    public async Task<ActionResult<ResponseBase>> Add([FromBody] ContactBase req) => await db.AddContactAsync(req);

    [HttpPut("edit")]
    public async Task<ActionResult<ResponseBase>> Edit([FromBody] EditContactRequest req)
    {
        ContactBase c = new()
        {
             FirstName = req.FirstName,
             LastName = req.LastName,
             PhoneNumber = req.LastName
        };

        return await db.EditContactAsync(req.Id, c);
    }

    [HttpDelete("delete")]
    public async Task<ActionResult<ResponseBase>> Delete([FromQuery] int id) => await db.DeleteContactAsync(id);
}
