using InventoryManagement.Frontend.Common;
using InventoryManagement.Frontend.Constants;
using InventoryManagement.Frontend.DTOs.Company;
using InventoryManagement.Frontend.Services;
using InventoryManagement.Frontend.Services.Authorization;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;

namespace InventoryManagement.Frontend.Pages.Company
{
    public partial class CompanyPage: ComponentBase
    {
        #region Inject
        [Inject] DialogService _dialogService { get; set; }
        [Inject] NotificationService _notificationService { get; set; }
        [Inject] ApiService _apiService { get; set; }
        [Inject] IAuthorizationService _authorizationService { get; set; }
        [Inject] NavigationManager _navigationManager { get; set; }
        [Inject] CommunicationService _communicationService { get; set; }
        #endregion

        private PaginatedResult<CompanyModel>? companyModel;
        private RadzenDataGrid<CompanyModel>? companyGrid;
        private Dictionary<int, CompanyModel> originalCompanyDataList = new Dictionary<int, CompanyModel>(); //DataGrid row cache
        private List<CompanyModel> companiesToUpdate = new List<CompanyModel>();
        private CompanyModel? companyToInsert;
        private CompanyModel? companyToUpdate;
        private int pageNumber = 1;
        private int pageSize = 10;


        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await LoadCompanyData(pageNumber, pageSize);
        }

        void Reset()
        {
            companyToInsert = null;
            companyToUpdate = null;
        }


        #region Filtering Company
        RadzenDataFilter<CompanyModel>? dataFilter;
        IQueryable<CompanyModel>? filteredCompanies;
        IQueryable<CompanyModel>? companies;
        Dictionary<string, string> filterValues = typeof(CompanyModel).GetProperties().ToDictionary(p => p.Name, _ => string.Empty);

        void CompanyFiltering()
        {
            FilteringParameters filtreleme = new FilteringParameters();
            foreach (var property in typeof(FilteringParameters).GetProperties())
            {
                if (filterValues.ContainsKey(property.Name))
                {
                    property.SetValue(filtreleme, filterValues[property.Name]);
                }
            }
            pageNumber = 1;
            pageSize = 100;
            CompanyFiltering(pageNumber, pageSize, filtreleme);
        }

        async void CompanyFiltering(int pageNumber, int pageSize, FilteringParameters filtreleme)
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

            var companyModelTask = _apiService.GetAsync<PaginatedResult<CompanyModel>>(url);
            await companyModelTask.ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    var exception = task.Exception?.InnerException;

                    _notificationService.Notify(NotificationSeverity.Error, "Bağlantı hatası", $"API sunucusuna bağlanılamıyor {exception?.Message}", duration: 6000);
                }
                else
                {
                    companyModel = task.Result;
                }
                StateHasChanged();
            });
        }
        #endregion


        #region Pagination
        string pagingSummaryFormat = "Gösterilen sayfa {0} - {1} (toplam {2} kayıt)";
        IEnumerable<int> pageSizeOptions = new int[] { 5, 10, 20, 30, 50, 100, 500, 1000, 5000 };
        async void ChangePage(PagerEventArgs args)
        {
            pageNumber = args.PageIndex + 1;
            pageSize = companyModel.pageSize;

            await LoadCompanyData(pageNumber, pageSize);
            StateHasChanged();
        }
        void PageSizeOptionsChanged(int pageSize)
        {
            companyModel.pageSize = pageSize;
            ChangePage(new PagerEventArgs() { PageIndex = companyGrid.CurrentPage });
        }
        #endregion

        async void LoadData(LoadDataArgs args)
        {
            pageNumber = args.Skip.Value / args.Top.Value + 1;
            pageSize = args.Top.Value;
            await LoadCompanyData(pageNumber, pageSize);

            companyGrid.CurrentPage = pageNumber - 1;

            StateHasChanged();
        }
        private async Task LoadCompanyData(int pageNumber, int pageSize)
        {
            var companyModelTask = _apiService.GetAsync<PaginatedResult<CompanyModel>>($"{ApiEndpointConstants.CompanySearch}?PageNumber={pageNumber}&PageSize={pageSize}");
            await companyModelTask.ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    var exception = task.Exception?.InnerException;
                    _notificationService.Notify(NotificationSeverity.Error, "Bağlantı hatası", $"İstek gerçekleştirilemedi {exception?.Message}", duration: 6000);
                }
                else
                {
                    companyModel = task.Result;
                }
                //StateHasChanged();
            });
        }



        #region Datatable
        async Task EditRow(CompanyModel company)
        {
            originalCompanyDataList[company.Id] = company.Clone();
            companiesToUpdate.Add(company);

            Task editRowTask = companyGrid.EditRow(company);
            Task reloadTask = companyGrid.Reload();

            await Task.WhenAll(editRowTask, reloadTask);
            StateHasChanged();
        }
        async Task CompanyDetail(CompanyModel company)
        {
            await _communicationService.SendCompany(company);
            _navigationManager.NavigateTo("/company/company-details");
        }
        async void OnUpdateRow(CompanyModel company)
        {
            if (company == companyToInsert)
            {
                companyToInsert = null;
            }
            companyToUpdate = null;
            var updatedcompany = await _apiService.PutAsync(ApiEndpointConstants.CompanyGetPostPutDelete, company);
            if (updatedcompany.IsSuccessStatusCode)
            {
                _notificationService.Notify(NotificationSeverity.Success, "Başarılı", $"{company.Name} isimli şirket güncellendi");
            }
            StateHasChanged();
        }
        async Task SaveRow(CompanyModel company)
        {
            if (companyGrid != null)
            {
                await companyGrid.UpdateRow(company);
            }
        }
        void CancelEdit(CompanyModel company)
        {
            if (company == companyToInsert)
            {
                companyToInsert = null;
            }
            else if (companiesToUpdate.Contains(company))
            {
                if (originalCompanyDataList.TryGetValue(company.Id, out var originalCompany))
                {
                    company.SetPropertiesFromJson(originalCompany);
                }
                companiesToUpdate.Remove(company);
            }
            companyGrid.CancelEditRow(company);
            companyGrid.Reload();
            StateHasChanged();
        }
        async Task DeleteRow(CompanyModel company)
        {
            if (company == companyToInsert)
            {
                companyToInsert = null;
            }
            else if (company == companyToUpdate)
            {
                companyToUpdate = null;
            }
            if (companyModel.data.Contains(company))
            {
                companyModel.data.Remove(company);
                companyModel.totalCount--;
                //await companyGrid.Reload();
            }

            bool? confirmed = await _dialogService.Confirm($"<b>{company.Name}</b> isimli şirketin silinme işlemini onaylıyor musunuz?\n" +
                                                          $"<br><li>Envantere bağlı tüm kayıtlar,\n" +
                                                          $"<br><li> Kategori ve alt kategoriler silinecek",
                                                           "Silme Onayı");
            if (confirmed == true)
            {
                var response = await _apiService.DeleteAsync(ApiEndpointConstants.CompanyGetPostPutDelete, company.Id);
                if (response.IsSuccessStatusCode)
                {
                    await companyGrid.Reload();
                    _notificationService.Notify(NotificationSeverity.Success, "Başarılı", $"Silme isteği gerçekleşti.");
                }
            }
        }


        async Task InsertRow(CompanyModel company)
        {
            companyToInsert = company;
            await companyGrid.InsertRow(companyToInsert);
            StateHasChanged();
        }
        async void OnCreateRow(CompanyModel company)
        {
            var insertedCompany = await _apiService.PostAsync(ApiEndpointConstants.CompanyGetPostPutDelete, company);

            if (insertedCompany.IsSuccessStatusCode)
            {
                var insertedCompanyModel = await insertedCompany.Content.ReadFromJsonAsync<CompanyModel>();
                companyModel.data?.Add(insertedCompanyModel);
                companyModel.totalCount++;
            }

            companyToInsert = null;
            await companyGrid.Reload();
            StateHasChanged();
        }
        #endregion



    }
}
