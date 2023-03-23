namespace Books.Api.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
//[Authorize("ClientIdPolicy")]
public class BooksController : ControllerBase
{
    [HttpGet]
    public IEnumerable<string> Get()
    {
        return new[] { "Amat", "Puslu Kýtalar Atlasý", "Efrâsiyâb'ýn Hikayeleri" };
    }
}