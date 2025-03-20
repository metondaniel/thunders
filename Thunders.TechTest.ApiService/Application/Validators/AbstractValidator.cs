using FluentValidation;
using Thunders.TechTest.Domain;

namespace Thunders.TechTest.ApiService.Application.Validators
{
    public class TollUsageValidator : AbstractValidator<TollUsage>
    {
        public TollUsageValidator()
        {
            RuleFor(x => x.UsageDateTime)
                .LessThanOrEqualTo(DateTime.UtcNow.AddMinutes(5))
                .WithMessage("Data futura não permitida");

            RuleFor(x => x.PlazaId)
                .NotEmpty().WithMessage("Praça é obrigatória")
                .MaximumLength(10).WithMessage("Máximo 10 caracteres");

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Valor deve ser positivo");
        }
    }
}
