using Microsoft.AspNetCore.Mvc;

namespace MachineShopManager.Controllers;

public class StudentController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}