using DevExpress.Blazor;
using InventoryManagement.Frontend.Common;
using InventoryManagement.Frontend.Constants;
using InventoryManagement.Frontend.DTOs.Company;
using InventoryManagement.Frontend.Services;
using InventoryManagement.Frontend.Services.Authorization;
using Microsoft.AspNetCore.Components;
using Radzen;


namespace InventoryManagement.Frontend.Pages.Definitions
{
    public partial class CompanyPage : ComponentBase
    {
        #region Inject
        [Inject] DialogService? DialogService { get; set; }
        [Inject] NotificationService? NotificationService { get; set; }
        [Inject] ApiService? ApiService { get; set; }
        [Inject] IAuthorizationService? AuthorizationService { get; set; }
        #endregion


        #region Pagination
        private PaginatedResult<CompanyDto>? companyModel;
        private IGrid? GridCompany { get; set; }
        private int PageNumber { get; set; } = 1;
        int PageSize { get; set; } = 250;
        #endregion


        protected override async Task OnInitializedAsync()
        {
            companyModel = await ApiService!.GetAsync<PaginatedResult<CompanyDto>>($"{ApiEndpointConstants.SearchCompany}?PageNumber={PageNumber}&PageSize={PageSize}");

            StateHasChanged();
        }

        public async Task DeleteCompany(CompanyDto company)
        {
            var response = await ApiService!.DeleteAsync(ApiEndpointConstants.DeleteCompany, company.Id);
            if (response.IsSuccessStatusCode)
            {
                companyModel?.data?.Remove(company);
                companyModel!.totalCount--;

                StateHasChanged();
                GridCompany?.Reload();
                NotificationService?.Notify(NotificationSeverity.Success, "Başarılı", $"{company.Name} isimli şirket ve bağlı ürünler silindi.");
            }
            else
            {
                NotificationService?.Notify(NotificationSeverity.Error, "Başarısız", $"{company.Name} silme işlemi gerçekleşti.");
            }
        }



        #region Company
        static void GridCompany_CustomizeEditModel(GridCustomizeEditModelEventArgs e)
        {
            if (e.IsNew)
            {
                var company = (CompanyDto)e.EditModel;
                company.Name = null;
            }
        }
        async Task GridCompany_EditModelSaving(GridEditModelSavingEventArgs e)
        {
            if (e.IsNew)
            {
                var newCompany = (CompanyDto)e.EditModel;
                var response = await ApiService!.PostAsync(ApiEndpointConstants.PostCompany, newCompany);
                if (response.IsSuccessStatusCode)
                {
                    var insertedCompany = await response.Content.ReadFromJsonAsync<CompanyDto>();
                    companyModel?.data?.Add(insertedCompany!);
                    companyModel!.totalCount++;

                    StateHasChanged();
                }
            }
            else
            {
                var updatedCompany = (CompanyDto)e.EditModel;
                var response = await ApiService!.PutAsync($"{ApiEndpointConstants.PutCompany}", updatedCompany);
                if (response.IsSuccessStatusCode)
                {
                    var index = companyModel!.data!.FindIndex(c => c.Id == updatedCompany.Id);
                    if (index != -1)
                    {
                        companyModel!.data[index] = updatedCompany;
                        StateHasChanged();
                    }
                }
            }
        }
        #endregion


        #region Filtering
        readonly Dictionary<string, string> filterValues = typeof(CompanyDto).GetProperties().ToDictionary(p => p.Name, _ => string.Empty);
        async Task CompanyFiltering()
        {
            var filtreleme = new FilteringParameters();
            foreach (var property in typeof(FilteringParameters).GetProperties())
            {
                if (filterValues.TryGetValue(property.Name, out var value))
                {
                    property.SetValue(filtreleme, value);
                }
            }
            await CompanyFiltering(PageNumber, PageSize, filtreleme);
        }

        async Task CompanyFiltering(int pageNumber, int pageSize, FilteringParameters filtreleme)
        {
            var parameters = new Dictionary<string, string>
            {
                { "PageNumber", pageNumber.ToString() },
                { "PageSize", pageSize.ToString() }
            };

            foreach (var propertyInfo in filtreleme.GetType().GetProperties())
            {
                var value = propertyInfo.GetValue(filtreleme)?.ToString();
                if (!string.IsNullOrEmpty(value))
                {
                    parameters.Add(propertyInfo.Name, value);
                }
            }

            var queryString = string.Join("&", parameters.Select(p => $"{p.Key}={p.Value}"));
            var url = $"{ApiEndpointConstants.SearchCompany}?{queryString}";

            try
            {
                companyModel = await ApiService!.GetAsync<PaginatedResult<CompanyDto>>(url);
            }
            catch (Exception ex)
            {
                NotificationService?.Notify(NotificationSeverity.Error, "Bağlantı hatası", $"API sunucusuna bağlanılamıyor {ex.Message}", duration: 6000);
            }

            StateHasChanged();
        }
        #endregion
    }
}
