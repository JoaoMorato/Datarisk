using Acornima.Ast;
using Datarisk.Models.Request;
using FluentValidation;
using System.Text.RegularExpressions;

namespace Datarisk.Validators;

public class NewScriptValidator : AbstractValidator<NewScriptRequest>
{
    
    public NewScriptValidator()
    {
        RuleFor(e => e.ScriptName)
            .NotEmpty()
            .WithMessage("ScriptName não pode ser vazio.");
    }

}
