using InventoryManagement.Frontend.Models;

namespace InventoryManagement.Frontend.Services
{
    public class CommunicationService
    {
        private CompanyModel? selectedCompany;
        private int selectedProductId;


        public CompanyModel GetSelectedCompany()
        {
            return selectedCompany;
        }

        public int GetSelectedProductId()
        {
            return selectedProductId;
        }


        public async Task SendCompany(CompanyModel company)
        {
            selectedCompany = company;

            if (OnCompanySelected != null)
            {

                foreach (var handler in OnCompanySelected.GetInvocationList())
                {
                    await ((Func<CompanyModel, Task>)handler)(company);
                }
            }
        }
        public event Func<CompanyModel, Task> OnCompanySelected;



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
    }
}