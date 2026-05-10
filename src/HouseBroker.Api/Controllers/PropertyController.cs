using HouseBroker.App.Auth.Dtos;
using HouseBroker.App.Properties.Dtos;
using HouseBroker.App.Properties.Interfaces;
using HouseBroker.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace HouseBroker.API.Controllers;

[ApiController]
[Route("api/properties")]
public class PropertiesController : ControllerBase
{
    private readonly IPropertyService _service;

    public PropertiesController(IPropertyService service)
    {
        _service = service;
    }

    [HttpPost]
    [Authorize(Roles = UserRoles.Broker)]
    public async Task<IActionResult> Create([FromBody] CreatePropertyDto dto)
    {
        var brokerId = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? string.Empty;
        return Ok(await _service.CreateAsync(dto, brokerId));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = UserRoles.Broker)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePropertyDto dto)
    {
        var brokerId = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? string.Empty;
        return Ok(await _service.UpdateAsync(id, dto, brokerId));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = UserRoles.Broker)]
    public async Task<IActionResult> Delete(int id)
    {
        var brokerId = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? string.Empty;
        await _service.DeleteAsync(id, brokerId);
        return NoContent();
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        // anonymous calls -> currentUserId is null, no commission shown
        var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        var result = await _service.GetByIdAsync(id, userId);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] PropertySearchDto filter)
    {
        var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
        var result = await _service.SearchAsync(filter, userId);
        return Ok(result);
    }
}