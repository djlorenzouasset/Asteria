﻿using System;
using System.Linq;
using System.ComponentModel;
using System.Windows.Markup;

namespace Asteria.Converters;

public static class EnumExtensions
{
    public static string GetDescription(this Enum value)
    {
        return value.GetType().GetField(value.ToString())?.GetCustomAttributes(typeof(DescriptionAttribute), false).SingleOrDefault() is not DescriptionAttribute attribute ? value.ToString() : attribute.Description;
    }
}

public class EnumToItem : MarkupExtension
{
    private readonly Type _type;

    public EnumToItem(Type type)
    {
        _type = type;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        var values = Enum.GetValues(_type).Cast<Enum>();
        return values.Select(x => x.GetDescription()).ToList();
    }
}
