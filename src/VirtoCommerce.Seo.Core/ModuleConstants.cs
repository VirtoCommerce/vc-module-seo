using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Settings;

namespace VirtoCommerce.Seo.Core;

public static class ModuleConstants
{
    public static class LinkStatus
    {
        public const string Active = "Active";
        public const string Resolved = "Resolved";
        public const string Accepted = "Accepted";
    }

    public static class Security
    {
        public static class Permissions
        {
            public const string Access = "seo:access";
            public const string Create = "seo:create";
            public const string Read = "seo:read";
            public const string Update = "seo:update";
            public const string Delete = "seo:delete";

            public static string[] AllPermissions { get; } =
            [
                Access,
                Create,
                Read,
                Update,
                Delete,
            ];
        }
    }

    public static class Settings
    {
        public static class General
        {
            public static SettingDescriptor SeoEnabled { get; } = new()
            {
                Name = "Seo.Enabled",
                GroupName = "Seo|General",
                ValueType = SettingValueType.Boolean,
                DefaultValue = false,
            };

            public static IEnumerable<SettingDescriptor> AllGeneralSettings
            {
                get
                {
                    yield return SeoEnabled;
                }
            }
        }

        public static IEnumerable<SettingDescriptor> AllSettings
        {
            get
            {
                return General.AllGeneralSettings;
            }
        }
    }
}
