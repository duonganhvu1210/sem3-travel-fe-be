using KarnelTravels.API.DTOs;
using KarnelTravels.API.Entities;
using KarnelTravels.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace KarnelTravels.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactController : ControllerBase
{
    private readonly KarnelTravelsDbContext _context;

    public ContactController(KarnelTravelsDbContext context)
    {
        _context = context;
    }

    // F121-F126: Submit contact request
    [HttpPost]
    public async Task<ActionResult<ApiResponse<ContactDto>>> SubmitContact(
        [FromBody] CreateContactRequest request)
    {
        // F124-F126: Server-side validation
        if (string.IsNullOrWhiteSpace(request.FullName))
            return BadRequest(new ApiResponse<ContactDto>
            {
                Success = false,
                Message = "Họ tên là bắt buộc"
            });

        if (string.IsNullOrWhiteSpace(request.Email))
            return BadRequest(new ApiResponse<ContactDto>
            {
                Success = false,
                Message = "Email là bắt buộc"
            });

        if (!new EmailAddressAttribute().IsValid(request.Email))
            return BadRequest(new ApiResponse<ContactDto>
            {
                Success = false,
                Message = "Định dạng email không hợp lệ"
            });

        if (!string.IsNullOrWhiteSpace(request.PhoneNumber) && request.PhoneNumber.Length < 10)
            return BadRequest(new ApiResponse<ContactDto>
            {
                Success = false,
                Message = "Số điện thoại phải có ít nhất 10 chữ số"
            });

        if (string.IsNullOrWhiteSpace(request.MessageContent))
            return BadRequest(new ApiResponse<ContactDto>
            {
                Success = false,
                Message = "Nội dung tin nhắn là bắt buộc"
            });

        // F123: Map request to entity
        var requestType = ContactRequestType.General;
        if (!string.IsNullOrEmpty(request.RequestType))
        {
            Enum.TryParse<ContactRequestType>(request.RequestType, true, out requestType);
        }

        var contact = new Contact
        {
            FullName = request.FullName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            Address = request.Address,
            Subject = request.Subject,
            RequestType = requestType,
            ServiceType = request.ServiceType,
            ExpectedDate = request.ExpectedDate,
            ParticipantCount = request.ParticipantCount,
            MessageContent = request.MessageContent,
            Rating = request.Rating,
            Status = ContactStatus.Unread,
            CreatedAt = DateTime.UtcNow
        };

        _context.Contacts.Add(contact);
        await _context.SaveChangesAsync();

        // F127-F128: Process based on request type
        // In a real app, you would send emails here based on request type

        var result = new ContactDto
        {
            ContactId = contact.Id,
            FullName = contact.FullName,
            Email = contact.Email,
            PhoneNumber = contact.PhoneNumber,
            Address = contact.Address,
            Subject = contact.Subject,
            RequestType = contact.RequestType.ToString(),
            ServiceType = contact.ServiceType,
            ExpectedDate = contact.ExpectedDate,
            ParticipantCount = contact.ParticipantCount,
            MessageContent = contact.MessageContent,
            Rating = contact.Rating,
            Status = contact.Status.ToString(),
            CreatedAt = contact.CreatedAt
        };

        return Ok(new ApiResponse<ContactDto>
        {
            Success = true,
            Message = "Yêu cầu của bạn đã được gửi thành công! Chúng tôi sẽ liên hệ trong thời gian sớm nhất.",
            Data = result
        });
    }

    // Get all contacts (for admin)
    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<ContactDto>>>> GetContacts(
        [FromQuery] ContactRequestType? requestType,
        [FromQuery] ContactStatus? status,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = _context.Contacts.AsQueryable();

        if (requestType.HasValue)
            query = query.Where(c => c.RequestType == requestType.Value);

        if (status.HasValue)
            query = query.Where(c => c.Status == status.Value);

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var result = items.Select(c => new ContactDto
        {
            ContactId = c.Id,
            FullName = c.FullName,
            Email = c.Email,
            PhoneNumber = c.PhoneNumber,
            Address = c.Address,
            Subject = c.Subject,
            RequestType = c.RequestType.ToString(),
            ServiceType = c.ServiceType,
            ExpectedDate = c.ExpectedDate,
            ParticipantCount = c.ParticipantCount,
            MessageContent = c.MessageContent,
            Rating = c.Rating,
            Status = c.Status.ToString(),
            CreatedAt = c.CreatedAt
        }).ToList();

        return Ok(new ApiResponse<List<ContactDto>>
        {
            Success = true,
            Data = result,
            Pagination = new PaginationInfo
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = totalCount
            }
        });
    }

    // Get contact by ID
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ContactDto>>> GetContact(Guid id)
    {
        var contact = await _context.Contacts.FindAsync(id);

        if (contact == null)
            return NotFound(new ApiResponse<ContactDto>
            {
                Success = false,
                Message = "Contact not found"
            });

        return Ok(new ApiResponse<ContactDto>
        {
            Success = true,
            Data = new ContactDto
            {
                ContactId = contact.Id,
                FullName = contact.FullName,
                Email = contact.Email,
                PhoneNumber = contact.PhoneNumber,
                Address = contact.Address,
                Subject = contact.Subject,
                RequestType = contact.RequestType.ToString(),
                ServiceType = contact.ServiceType,
                ExpectedDate = contact.ExpectedDate,
                ParticipantCount = contact.ParticipantCount,
                MessageContent = contact.MessageContent,
                Rating = contact.Rating,
                Status = contact.Status.ToString(),
                CreatedAt = contact.CreatedAt
            }
        });
    }

    // Update contact status (for admin)
    [HttpPut("{id}/status")]
    public async Task<ActionResult<ApiResponse<ContactDto>>> UpdateStatus(
        Guid id,
        [FromBody] UpdateContactStatusRequest request)
    {
        var contact = await _context.Contacts.FindAsync(id);

        if (contact == null)
            return NotFound(new ApiResponse<ContactDto>
            {
                Success = false,
                Message = "Contact not found"
            });

        contact.Status = request.Status;
        if (!string.IsNullOrEmpty(request.ReplyMessage))
        {
            contact.ReplyMessage = request.ReplyMessage;
            contact.RepliedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        return Ok(new ApiResponse<ContactDto>
        {
            Success = true,
            Data = new ContactDto
            {
                ContactId = contact.Id,
                FullName = contact.FullName,
                Email = contact.Email,
                Status = contact.Status.ToString()
            }
        });
    }
}
