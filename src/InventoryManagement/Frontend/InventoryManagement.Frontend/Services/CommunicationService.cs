using InventoryManagement.Frontend.DTOs.Company;

namespace InventoryManagement.Frontend.Services
{
    public class CommunicationService
    {
        private CompanyDto? selectedCompany;
        private int selectedProductId;
        private int selectedProductBarcode;


        public CompanyDto GetSelectedCompany()
        {
            return selectedCompany;
        }

        public int GetSelectedProductId()
        {
            return selectedProductId;
        }

        public int GetSelectedProductBarcode()
        {
            return selectedProductBarcode;
        }



        public async Task SendCompany(CompanyDto company)
        {
            selectedCompany = company;

            if (OnCompanySelected != null)
            {

                foreach (var handler in OnCompanySelected.GetInvocationList())
                {
                    await ((Func<CompanyDto, Task>)handler)(company);
                }
            }
        }
        public event Func<CompanyDto, Task> OnCompanySelected;



        public async Task SendProductId(int productId)
        {
            selectedProductId = productId;

            if (OnProductIdSelected != null)
            {

                foreach (var handler in OnProductIdSelected.GetInvocationList())
                {
                    await ((Func<int, Task>)handler)(productId);
                }
            }
        }
        public event Func<int, Task> OnProductIdSelected;




        public async Task SendProductBarcode(int productBarcode)
        {
            selectedProductBarcode = productBarcode;

            if (OnProductBarcodeSelected != null)
            {

                foreach (var handler in OnProductBarcodeSelected.GetInvocationList())
                {
                    await ((Func<int, Task>)handler)(productBarcode);
                }
            }
        }
        public event Func<int, Task> OnProductBarcodeSelected;
    }
}