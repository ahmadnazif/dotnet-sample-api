using Microsoft.AspNetCore.Mvc;

namespace SampleApi.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
[Route("/")]
[ApiController]
public class HomeController : ControllerBase
{
    [HttpGet]
    public ActionResult RedirectToSwagger() => Redirect("swagger");
}
