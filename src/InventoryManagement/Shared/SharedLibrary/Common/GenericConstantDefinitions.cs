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
    }


    
    public class GenericConstantDescriptions
    {
        public static Dictionary<string, string> Descriptions = new Dictionary<string, string>
    {
        { GenericConstantDefinitions.Transfer, "Transfer Aşamasında" },
        { GenericConstantDefinitions.Accepted, "Depoda" },
        { GenericConstantDefinitions.Rejected, "Transfer Aşamasında - Red Edildi" },
        { GenericConstantDefinitions.ReturnIt, "Transfer Aşamasında - İade Edildi" }
    };
    }
}
