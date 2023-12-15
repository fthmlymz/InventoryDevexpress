using DevExpress.Blazor;
using InventoryManagement.Frontend.Common;
using InventoryManagement.Frontend.Constants;
using InventoryManagement.Frontend.DTOs.Company;
using InventoryManagement.Frontend.Services;
using InventoryManagement.Frontend.Services.Authorization;
using Microsoft.AspNetCore.Components;
using Radzen;


namespace InventoryManagement.Frontend.Pages.Company
{
    public partial class CompanyPage : ComponentBase
    {
        #region Inject
        [Inject] DialogService? _dialogService { get; set; }
        [Inject] NotificationService? _notificationService { get; set; }
        [Inject] ApiService? _apiService { get; set; }
        [Inject] IAuthorizationService? _authorizationService { get; set; }
        [Inject] NavigationManager? _navigationManager { get; set; }
        [Inject] CommunicationService? _communicationService { get; set; }
        #endregion


        #region Pagination
        private PaginatedResult<CompanyDto>? companyModel;
        private IGrid? GridCompany { get; set; }
        private int PageNumber { get; set; } = 1;
        int PageSize { get; set; } = 3;
        #endregion


        protected override async Task OnInitializedAsync()
        {
            companyModel = await _apiService.GetAsync<PaginatedResult<CompanyDto>>($"{ApiEndpointConstants.CompanySearch}?PageNumber={PageNumber}&PageSize={PageSize}");

            StateHasChanged();
        }

        public async Task DeleteCompany(CompanyDto company)
        {
            var response = await _apiService.DeleteAsync(ApiEndpointConstants.CompanyGetPostPutDelete, company.Id);
            if (response.IsSuccessStatusCode)
            {
                companyModel?.data?.Remove(company);

                companyModel.totalCount--;
                StateHasChanged();
                GridCompany?.Reload();
                _notificationService?.Notify(NotificationSeverity.Success, "Başarılı", $"{company.Name} silme işlemi gerçekleşti.");
            }
            else
            {
                _notificationService?.Notify(NotificationSeverity.Error, "Başarısız", $"{company.Name} silme işlemi gerçekleşti.");
            }
        }



        #region Company
        void GridCompany_CustomizeEditModel(GridCustomizeEditModelEventArgs e)
        {
            if (e.IsNew)
            {
                var company = (CompanyDto)e.EditModel;
                //category.CompanyId = 0; // CompanyId aktif, sadece null gonderiliyor
                company.Name = null;
            }
        }
        async Task GridCompany_EditModelSaving(GridEditModelSavingEventArgs e)
        {
            if (e.IsNew)
            {
                var newCompany = (CompanyDto)e.EditModel;
                var response = await _apiService.PostAsync(ApiEndpointConstants.CompanyGetPostPutDelete, newCompany);
                if (response.IsSuccessStatusCode)
                {
                    var insertedCompany = await response.Content.ReadFromJsonAsync<CompanyDto>();
                    companyModel?.data?.Add(insertedCompany);
                    companyModel.totalCount++;
                    StateHasChanged();
                }
            }
            else
            {
                var updatedCompany = (CompanyDto)e.EditModel;
                var response = await _apiService.PutAsync($"{ApiEndpointConstants.CompanyGetPostPutDelete}", updatedCompany);
                if (response.IsSuccessStatusCode)
                {
                    var index = companyModel.data.FindIndex(c => c.Id == updatedCompany.Id);
                    if (index != -1)
                    {
                        companyModel.data[index] = updatedCompany;
                        StateHasChanged();
                    }
                }
            }
        }
        #endregion


        #region Filtering
        Dictionary<string, string> filterValues = typeof(CompanyDto).GetProperties().ToDictionary(p => p.Name, _ => string.Empty);
        void CompanyFiltering()
        {
            var filtreleme = new FilteringParameters();
            foreach (var property in typeof(FilteringParameters).GetProperties())
            {
                if (filterValues.TryGetValue(property.Name, out var value))
                {
                    property.SetValue(filtreleme, value);
                }
            }
            CompanyFiltering(PageNumber, PageSize, filtreleme);
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
            var url = $"{ApiEndpointConstants.CompanySearch}?{queryString}";

            try
            {
                companyModel = await _apiService.GetAsync<PaginatedResult<CompanyDto>>(url);
            }
            catch (Exception ex)
            {
                _notificationService.Notify(NotificationSeverity.Error, "Bağlantı hatası", $"API sunucusuna bağlanılamıyor {ex.Message}", duration: 6000);
            }

            StateHasChanged();
        }
        #endregion
    }
}

/*void CompanyFiltering()
{
    FilteringParameters filtreleme = new FilteringParameters();
    foreach (var property in typeof(FilteringParameters).GetProperties())
    {
        if (filterValues.ContainsKey(property.Name))
        {
            property.SetValue(filtreleme, filterValues[property.Name]);
        }
    }
    CompanyFiltering(PageNumber, PageSize, filtreleme);
}*/
/*async void CompanyFiltering(int pageNumber, int pageSize, FilteringParameters filtreleme)
        {
            var parameters = new Dictionary<string, string>
        {
        { "PageNumber", pageNumber.ToString() },
        { "PageSize", pageSize.ToString() }
        };
            var propertyInfos = filtreleme.GetType().GetProperties();
            foreach (var propertyInfo in propertyInfos)
            {
                var value = propertyInfo.GetValue(filtreleme)?.ToString();
                if (!string.IsNullOrEmpty(value))
                {
                    parameters.Add(propertyInfo.Name, value);
                }
            }
            var filteredParameters = parameters.Select(p => $"{p.Key}={p.Value}");
            var url = $"{ApiEndpointConstants.CompanySearch}?{string.Join("&", filteredParameters)}";

            var companyModelTask = _apiService.GetAsync<PaginatedResult<CompanyDto>>(url);

            await companyModelTask.ContinueWith(async task =>
            {
                if (task.IsFaulted)
                {
                    var exception = task.Exception?.InnerException;

                    _notificationService.Notify(NotificationSeverity.Error, "Bağlantı hatası", $"API sunucusuna bağlanılamıyor {exception?.Message}", duration: 6000);
                }
                else
                {
                    companyModel = task.Result;
                    TotalCount = companyModel?.totalCount ?? 1;
                    TotalPages = (int)Math.Ceiling((double)TotalCount / PageSize);
                    currentPage = companyModel?.currentPage ?? 1;
                }
                await InvokeAsync(StateHasChanged);
            });
        }*/
