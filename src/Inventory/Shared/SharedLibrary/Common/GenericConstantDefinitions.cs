using System.ComponentModel;

namespace SharedLibrary.Common
{
    public class GenericConstantDefinitions
    {
        public const string Transfer = "Transfer";
        public const string Accepted = "Accepted";
        public const string Rejected = "Rejected";
        public const string ReturnIt = "ReturnIt";
        public const string InStock = "Depoda";
        public const string Embezzled = "Zimmetlendi";
        public const string Scrap = "Hurda";
    }

    public class GenericConstantDescriptions
    {
        public static Dictionary<string, string> Descriptions = new Dictionary<string, string>
        {
            { GenericConstantDefinitions.Scrap, "Hurda" },
            { GenericConstantDefinitions.Transfer, "Transfer Aşamasında" },
            { GenericConstantDefinitions.Accepted, "Depoda" },
            { GenericConstantDefinitions.Rejected, "Transfer Aşamasında - Red Edildi" },
            { GenericConstantDefinitions.ReturnIt, "Transfer Aşamasında - İade Edildi" }
        };
    }


    #region DataClass
    public enum DataClass
    {
        [Description("Gizli")]
        Confidential,
        [Description("Çok Gizli")]
        TopSecret,
        [Description("Kamuya Açık")]
        Public,
        [Description("Hizmete Özel")]
        ServiceSpecific,
        [Description("Kurum İçi")]
        InHouse
    }
    public static class EnumHelper
    {
        public static string GetEnumDescription(Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
            return attribute == null ? value.ToString() : attribute.Description;
        }

        public static string[] GetEnumDescriptions(Type enumType)
        {
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("Type must be an enum");
            }
            return Enum.GetValues(enumType)
                .Cast<Enum>()
                .Select(GetEnumDescription)
                .ToArray();
        }
    }
    public class GenericConstantDataClassDescriptions
    {
        public static Dictionary<string, string> Descriptions = new Dictionary<string, string>
        {
            { DataClass.Confidential.ToString(), EnumHelper.GetEnumDescription(DataClass.Confidential) },
            { DataClass.TopSecret.ToString(), EnumHelper.GetEnumDescription(DataClass.TopSecret) },
            { DataClass.Public.ToString(), EnumHelper.GetEnumDescription(DataClass.Public) },
            { DataClass.ServiceSpecific.ToString(), EnumHelper.GetEnumDescription(DataClass.ServiceSpecific) },
            { DataClass.InHouse.ToString(), EnumHelper.GetEnumDescription(DataClass.InHouse) }
        };
        public static string[] DescriptionArray => EnumHelper.GetEnumDescriptions(typeof(DataClass));
    }
    #endregion
}
