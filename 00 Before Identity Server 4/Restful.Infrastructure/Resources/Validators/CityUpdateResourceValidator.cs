using FluentValidation;

namespace Restful.Infrastructure.Resources.Validators
{
    public class CityUpdateResourceValidator 
        : CityAddOrUpdateResourceValidator<CityUpdateResource>
    {
        public CityUpdateResourceValidator()
        {
            RuleFor(c => c.Description)
                .NotEmpty().WithName("描述").WithMessage("{PropertyName}是必填项");
        }
    }
}
