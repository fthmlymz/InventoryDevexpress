namespace InventoryManagement.Frontend.Constants
{
    public class ApplicationConstants
    {

        private static string ZimmetLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reports/ZimmetFormu.repx");
        private static string GenelEnvanterRaporuLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reports/GenelEnvanterRaporu.repx");



        public static string ZimmetFormuReport => ZimmetLocation;
        public static string GenelEnvanterReport => GenelEnvanterRaporuLocation;
    }
}
