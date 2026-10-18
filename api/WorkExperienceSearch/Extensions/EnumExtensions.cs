using System.ComponentModel;

namespace Work_Experience_Search.Extensions;

public static class EnumExtensions
{
    public static string ToDescriptionString(this Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        if (field is null) return value.ToString();

        var attributes = (DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), false);
        return attributes.Length > 0 ? attributes[0].Description : value.ToString();
    }

    public static T? FromDescriptionString<T>(string description) where T : Enum
    {
        foreach (var field in typeof(T).GetFields())
        {
            var matches = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute
                ? attribute.Description == description
                : field.Name == description;

            if (matches) return field.GetValue(null) is T value ? value : default;
        }

        throw new ArgumentException("Not found.", nameof(description));
    }
}
