using FluentValidation;

namespace DotNETworkTool.Common.Config
{
    public class ConfigSetting
    {
        public string PropertyName { get; set; }
        public string PropertyValue { get; set; }
    }

    public class ConfigSettingValidator : AbstractValidator<ConfigSetting>
    {
        public ConfigSettingValidator()
        {
            RuleFor(x => x.PropertyName).Matches(@"^[a-zA-Z_]*$");
            RuleFor(x => x.PropertyValue).Matches(@"^[a-zA-Z0-9_., ]*$");
        }
    }
}
