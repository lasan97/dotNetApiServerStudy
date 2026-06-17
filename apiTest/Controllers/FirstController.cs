using apiTest.Services;
using Microsoft.AspNetCore.Mvc;

namespace apiTest.Controllers;

/// <summary>
/// class 이름이 java랑 다르게 파일 이름과 같을 필요가 없음
/// ControllerBase는 API 구성에 필요한 유틸들이 있음
/// </summary>
[ApiController]
[Route("[controller]")]
public class FirstController(FirstService firstService): ControllerBase
{
    [HttpGet]
    public ActionResult<string> Hello()
    {
        return Ok(firstService.getMessage());
    }
}