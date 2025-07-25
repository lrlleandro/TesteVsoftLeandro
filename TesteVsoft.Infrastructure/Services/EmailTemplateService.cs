using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Reflection;
using System.Text;
using TesteVsoft.Application.Interfaces.Services;
using TesteVsoft.Domain.Entities;
using TesteVsoft.Infrastructure.Common.Attributes;
using static System.Net.Mime.MediaTypeNames;

namespace TesteVsoft.Infrastructure.Services;

[Scoped]
public class EmailTemplateService(IIcsCalendarGenerator icsCalendarGenerator) : IEmailTemplateService
{
    public async Task<string> GenerateAsync(UserTask userTask)
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), "..", "TesteVsoft.Infrastructure", "EmailsTemplating", "TaskAssignedTemplate.html");
        var htmlTemplate = await File.ReadAllTextAsync(path);

        var status = userTask.Status.GetType()
                        .GetMember(userTask.Status.ToString())[0]
                        .GetCustomAttribute<DisplayAttribute>()?
                        .Name ?? userTask.Status.ToString();
        
        return htmlTemplate
            .Replace("{Name}", userTask.AssignedUser!.Name)
            .Replace("{Title}", userTask.Title)
            .Replace("{Description}", userTask.Description)
            .Replace("{DueDate}", userTask.DueDate.ToString("dd/MM/yyyy"))
            .Replace("{Status}", status)
            .Replace("{Year}", DateTime.Now.Year.ToString())
            .Replace("{Id}", userTask.Id.ToString())
            .Replace("{Ics}", GenerateIcsUrl(userTask));
    }

    private string GenerateIcsUrl(UserTask userTask)
    {
        var sb = new StringBuilder();

        sb.Append("https://calendar.google.com/calendar/render?action=TEMPLATE");
        sb.Append($"&text={Uri.EscapeDataString(userTask.Title)}");
        sb.Append($"&dates={DateTime.UtcNow:yyyyMMddTHHmmssZ}/{userTask.DueDate:yyyyMMddTHHmmssZ}");
        sb.Append($"&details={userTask.Description}");
        sb.Append($"&status=CONFIRMED");

        return sb.ToString();
    }
}
