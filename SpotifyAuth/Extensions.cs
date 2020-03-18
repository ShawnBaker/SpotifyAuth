// Copyright © 2020 Shawn Baker using the MIT License.
using System;
using System.ComponentModel;

namespace FrozenNorth.SpotifyAuth
{
	internal static class Extensions
	{
        internal static string GetDescription(this Enum value)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (name != null)
            {
                System.Reflection.FieldInfo field = type.GetField(name);
                if (field != null)
                {
                    DescriptionAttribute attr = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
                    if (attr != null)
                    {
                        return attr.Description;
                    }
                }
            }
            return null;
        }

        internal static string GetFlagsDescription(this Enum value)
        {
            string description = "";

            foreach (Enum flag in Enum.GetValues(value.GetType()))
            {
                if (value.HasFlag(flag))
                {
                    if (description.Length > 0)
                    {
                        description += " ";
                    }
                    description += flag.GetDescription();
                }
            }

            return description;
        }
    }
}
