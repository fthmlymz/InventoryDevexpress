namespace InventoryManagement.Frontend.Constants
{
    public class ApplicationConstants
    {

        private static string ZimmetLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reports/ZimmetFormu.repx");
        private static string GenelEnvanterRaporuLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reports/GenelEnvanterRaporu.repx");



        public static string ZimmetFormuReport => ZimmetLocation;
        public static string GenelEnvanterReport => GenelEnvanterRaporuLocation;



        public static class Folders
        {
            public static string Zimmetler => "Zimmetler";
            public static string Tutanaklar => "Tutanaklar";
            public static string Diger => "Diğer";
            public static List<string> AllFolders => new List<string> { Zimmetler, Tutanaklar, Diger };
        }
    }
}
