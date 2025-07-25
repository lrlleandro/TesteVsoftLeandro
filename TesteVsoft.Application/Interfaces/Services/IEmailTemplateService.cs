using TesteVsoft.Domain.Entities;

namespace TesteVsoft.Application.Interfaces.Services;

public interface IEmailTemplateService
{
    Task<string> GenerateAsync(UserTask userTask);
}