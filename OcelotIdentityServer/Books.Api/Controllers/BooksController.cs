namespace Books.Api.Controllers;

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class BooksController : ControllerBase
{
    [HttpGet]
    public IEnumerable<string> Get()
    {
        return new[] { "Amat", "Puslu Kıtalar Atlası", "Efrâsiyâb'ın Hikayeleri" };
    }
}