﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MiniApp2.API.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class InvoiceController : ControllerBase
{
    [HttpGet]
    public IActionResult GetInvoices()
    {
        var userName = HttpContext.User.Identity.Name;

        var userIdClaim = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

        //veri tabanında  userId veya userName alanları üzerinden gerekli dataları çek

        // stockId stockQuantity  Category  UserId/UserName

        return Ok($"Invoice işlemleri =>  UserName: {userName}- UserId:{userIdClaim.Value}");
    }
}
