using Microsoft.AspNetCore.Mvc;

namespace WebTestJsonConverter;

[ApiController]
[Route("/api")]
public class WebController : ControllerBase
{
    [HttpGet("post")]
    public async Task<ActionResult<ParentModel>> PostForSomething()
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        ParentModel parentModel = new ParentModel()
        {
            ParentId = 143563463467,
            Name = "Name",
            Child = new ChildModel()
            {
                ChildId = 423534645674,
                Description = "Desc",
            }
        };
        return await Task.FromResult(parentModel);
    }
}