using FluentValidation;

namespace Restful.Infrastructure.Resources.Validators
{
    public class CountryAddResourceValidator: AbstractValidator<CountryAddResource>
    {
        public CountryAddResourceValidator()
        {
            RuleFor(c => c.EnglishName)
                .NotEmpty().WithName("英文名").WithMessage("{PropertyName}是必填项")
                .MaximumLength(100).WithMessage("{PropertyName}的长度不可超过{MaxLength}");

            RuleFor(c => c.ChineseName)
                .NotEmpty().WithName("中文名").WithMessage("{PropertyName}是必填项")
                .MaximumLength(100).WithMessage("{PropertyName}的长度不可超过{MaxLength}");

            RuleFor(c => c.Abbreviation)
                .NotEmpty().WithName("缩写").WithMessage("{PropertyName}是必填项")
                .MaximumLength(5).WithMessage("{PropertyName}的长度不可超过{MaxLength}");
        }
    }
}
