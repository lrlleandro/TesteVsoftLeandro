namespace TesteVsoft.Domain.Enums;

using System.ComponentModel.DataAnnotations;

public enum UserTaskStatusTypes
{
    [Display(Name = "Pendente")]
    Pending,

    [Display(Name = "Em progresso")]
    InProgress,

    [Display(Name = "Finalizada")]
    Completed
}
