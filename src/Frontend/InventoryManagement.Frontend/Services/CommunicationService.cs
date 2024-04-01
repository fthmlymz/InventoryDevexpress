using InventoryManagement.Frontend.DTOs.Company;
using InventoryManagement.Frontend.DTOs.Keycloak;
using InventoryManagement.Frontend.DTOs.Product;

namespace InventoryManagement.Frontend.Services
{
    public class CommunicationService
    {
        private CompanyDto? selectedCompany;
        private ProductDto? selectedProduct;
        private KeycloakUsersDto? selectedUser;


        public CompanyDto GetSelectedCompany()
        {
#pragma warning disable CS8603 // Possible null reference return.
            return selectedCompany;
#pragma warning restore CS8603 // Possible null reference return.
        }

        public ProductDto GetSelectedProduct()
        {
#pragma warning disable CS8603 // Possible null reference return.
            return selectedProduct;
#pragma warning restore CS8603 // Possible null reference return.
        }

        public KeycloakUsersDto GetSelectedUser()
        {
#pragma warning disable CS8603 // Possible null reference return.
            return selectedUser;
#pragma warning restore CS8603 // Possible null reference return.
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
        public event Func<CompanyDto, Task> ?OnCompanySelected;


        public async Task SendProduct(ProductDto product)
        {
            selectedProduct = product;

            if (OnProductSelected != null)
            {
                foreach (var handler in OnProductSelected.GetInvocationList())
                {
                    await ((Func<ProductDto, Task>)handler)(product);
                }
            }
        }
        public event Func<ProductDto, Task> ? OnProductSelected;





        public async Task SendSelectedUser(KeycloakUsersDto user)
        {
            selectedUser = user;

            if (OnSelectedUser != null)
            {
                foreach (var handler in OnSelectedUser.GetInvocationList())
                {
                    await ((Func<KeycloakUsersDto, Task>)handler)(user);
                }
            }
        }
        public event Func<KeycloakUsersDto, Task>? OnSelectedUser;
    }
}
