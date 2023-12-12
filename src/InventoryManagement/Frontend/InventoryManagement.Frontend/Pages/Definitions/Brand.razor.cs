using InventoryManagement.Frontend.Common;
using InventoryManagement.Frontend.Common.Exceptions;
using InventoryManagement.Frontend.Constants;
using InventoryManagement.Frontend.DTOs.Brand;
using InventoryManagement.Frontend.DTOs.Company;
using InventoryManagement.Frontend.Services;
using InventoryManagement.Frontend.Services.Authorization;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;

namespace InventoryManagement.Frontend.Pages.Definitions
{
    public partial class Brand : ComponentBase
    {
        [Inject] ApiService ApiService { get; set; }
        [Inject] IAuthorizationService AuthorizationService { get; set; }
        [Inject] NotificationService NotificationService { get; set; }
        [Inject] DialogService DialogService { get; set; }


        #region Company Dropdown
        PaginatedResult<CompanyModel>? companies;
        CompanyModel? selectedCompany;
        void OnDropDownValueChanged(object item)
        {
            selectedCompany = (CompanyModel)item;
        }
        #endregion




        #region Brand-Model
        IList<BrandModel>? SelectedBrand { get; set; }
        private PaginatedResult<BrandModel>? brandModel { get; set; }
        private PaginatedResult<Model>? modelModel { get; set; }
        #endregion


        #region CategoryModel Table
        private Dictionary<int, BrandModel> originalBrandDataList = new Dictionary<int, BrandModel>();//cache
        IQueryable<BrandModel>? brands;
        private BrandModel? brandToInsert;
        private BrandModel? brandToUpdate;
        private List<BrandModel> brandsToUpdate = new List<BrandModel>();
        private RadzenDataGrid<BrandModel>? brandGrid;
        #endregion


        #region Model Table
        private Dictionary<int, Model> originalModelDataList = new Dictionary<int, Model>();//cache
        IQueryable<Model>? models;
        private Model? modelToInsert;
        private Model? modelToUpdate;
        private List<Model> modelsToUpdate = new List<Model>();
        private RadzenDataGrid<Model>? modelGrid;
        #endregion


        protected override async void OnInitialized()
        {
            await LoadCompanyData(1, 10000);
            await GetBrandList();
        }

        private async Task LoadCompanyData(int pageNumber, int pageSize)
        {
            try
            {
                var companyModel = await ApiService.GetAsync<PaginatedResult<CompanyModel>>($"{ApiEndpointConstants.CompanySearch}?PageNumber={pageNumber}&PageSize={pageSize}");
                companies = companyModel;
            }
            catch (Exception ex)
            {
                NotificationService.Notify(NotificationSeverity.Error, "Bağlantı hatası", $"İstek gerçekleştirilemedi {ex.Message}", duration: 6000);
            }
            StateHasChanged();
        }


        #region Brand List
        async Task GetBrandList()
        {
            try
            {
                var jsonDocumentTask = ApiService.GetAsync<System.Text.Json.JsonDocument>($"{ApiEndpointConstants.BrandGetPostPutDelete}?PageNumber=1&PageSize=1000"); // bu alan dinamik hale getirilecek
                var jsonDocument = await jsonDocumentTask;

                var dataElement = jsonDocument.RootElement.GetProperty("data");
                var brands = System.Text.Json.JsonSerializer.Deserialize<List<BrandModel>>(dataElement.ToString());

                var paginatedResult = new PaginatedResult<BrandModel> { data = brands };

                brandModel = paginatedResult;

                StateHasChanged();
            }
            catch (AggregateException ex)
            {
                foreach (var innerException in ex.InnerExceptions)
                {
                    if (innerException is Exception exception)
                    {
                        NotificationService.Notify(NotificationSeverity.Error, "Hata", $"Sorgulama hatası: {exception.Message}", duration: 6000);
                    }
                }
            }
        }
        #endregion



        #region DataTable Brand
        async Task EditRowBrand(BrandModel brand)
        {
            if (!originalBrandDataList.ContainsKey(brand.Id))
            {
                originalBrandDataList.Add(brand.Id, brand.Clone());
            }

            brandsToUpdate.Add(brand);

            brand.CompanyId = selectedCompany?.Id ?? 0;

            await Task.WhenAll(brandGrid.EditRow(brand), brandGrid.Reload());

            StateHasChanged();
        }
        async void OnUpdateRowBrand(BrandModel brand)
        {
            if (brand == brandToInsert)
            {
                brandToInsert = null;
            }

            brandToUpdate = null;
            var updateResponse = await ApiService.PutAsync(ApiEndpointConstants.BrandGetPostPutDelete, brand);

            if (updateResponse.IsSuccessStatusCode)
            {
                NotificationService.Notify(NotificationSeverity.Success, "Başarılı", $"{brand.Name} isimli marka güncellendi");
            }
            StateHasChanged();
        }
        async Task SaveRowBrand(BrandModel brand)
        {
            if (brandGrid != null)
            {
                await brandGrid.UpdateRow(brand);
                StateHasChanged();
            }
        }
        void CancelEditBrand(BrandModel brand)
        {
            if (brand == brandToInsert)
            {
                brandToInsert = null;
            }
            else if (brandsToUpdate.Remove(brand))
            {
                if (originalBrandDataList.TryGetValue(brand.Id, out var originalCompany))
                {
                    brand.SetPropertiesFromJson(originalCompany);
                }
            }

            brandGrid?.CancelEditRow(brand);
            brandGrid?.Reload();

            StateHasChanged();
        }



        async Task DeleteRowBrand(BrandModel brand)
        {
            if (brand == brandToInsert)
            {
                brandToInsert = null;
            }
            else if (brand == brandToUpdate)
            {
                brandToUpdate = null;
            }
            if (brandModel.data.Contains(brand))
            {
                brandModel.data.Remove(brand);
                brandModel.totalCount--;
                //await companyGrid.Reload();
            }

            bool? confirmed = await DialogService.Confirm($"<b>{brand.Name}</b> isimli marka silinme işlemini onaylıyor musunuz?\n" +
                                                          $"<br><li>Envantere bağlı tüm kayıtlar,\n" +
                                                          $"<br><li> Marka ve model bilgileri silinecek",
                                                           "Silme Onayı");
            if (confirmed == true)
            {
                var response = await ApiService.DeleteAsync(ApiEndpointConstants.BrandGetPostPutDelete, brand.Id);
                if (response.IsSuccessStatusCode)
                {
                    await brandGrid.Reload();
                    NotificationService.Notify(NotificationSeverity.Success, "Başarılı", $"Silme isteği gerçekleşti.");
                }
            }
        }
        async Task InsertRowBrand(BrandModel brand)
        {
            brandToInsert = brand;
            brand.CompanyId = selectedCompany.Id;
            await brandGrid.InsertRow(brandToInsert);
            StateHasChanged();
        }
        async Task OnCreateRowBrand(BrandModel brand)
        {
            try
            {
                var insertedBrand = await ApiService.PostAsync(ApiEndpointConstants.BrandGetPostPutDelete, brand);

                if (insertedBrand.IsSuccessStatusCode)
                {
                    var insertedBrandModel = await insertedBrand.Content.ReadFromJsonAsync<BrandModel>();
                    brandModel?.data?.Add(insertedBrandModel);
                    brandModel.totalCount++;
                }
            }
            catch (ApiException ex)
            {
                NotificationService.Notify(NotificationSeverity.Error, "Hata", $"Marka oluşturma hatası: {ex.Message}");
            }
            brandToInsert = null;
            await brandGrid.Reload();
            StateHasChanged();
        }
        #endregion


        #region DataTable CategorySub
        async Task EditRowModel(Model model)
        {
            originalModelDataList[model.Id] = model.Clone();
            modelsToUpdate.Add(model);

            Task editRowTask = modelGrid.EditRow(model);
            Task reloadTask = modelGrid.Reload();

            await Task.WhenAll(editRowTask, reloadTask);
            StateHasChanged();
        }
        async void OnUpdateRowModel(Model model)
        {
            if (model == modelToInsert)
            {
                modelToInsert = null;
            }
            modelToUpdate = null;
            var updatedModel = await ApiService.PutAsync(ApiEndpointConstants.ModelGetPostPutDelete, model);
            if (updatedModel.IsSuccessStatusCode)
            {
                NotificationService.Notify(NotificationSeverity.Success, "Başarılı", $"{model.Name} isimli model güncellendi");
            }
            StateHasChanged();
        }
        async Task SaveRowModel(Model model)
        {
            if (modelGrid != null)
            {
                await modelGrid.UpdateRow(model);
            }
        }
        void CancelEditModel(Model model)
        {
            if (model == modelToInsert)
            {
                modelToInsert = null;
            }
            else if (modelsToUpdate.Contains(model))
            {
                if (originalModelDataList.TryGetValue(model.Id, out var originalCompanySub))
                {
                    model.SetPropertiesFromJson(originalCompanySub);
                }
                modelsToUpdate.Remove(model);
            }
            modelGrid.CancelEditRow(model);
            modelGrid.Reload();
            StateHasChanged();
        }
        async Task DeleteRowModel(Model model)
        {
            if (model == modelToInsert)
            {
                modelToInsert = null;
            }
            else if (model == modelToUpdate)
            {
                modelToUpdate = null;
            }

            if (brandModel?.data != null)
            {
                var brandToRemove = brandModel.data.FirstOrDefault(c => c.Models?.Contains(model) == true);
                if (brandToRemove != null)
                {
                    brandToRemove.Models.Remove(model);
                    brandModel.totalCount--;
                }
            }

            bool? confirmed = await DialogService.Confirm($"<b>{model.Name}</b> isimli model silinme işlemini onaylıyor musunuz?\n", "Silme Onayı");

            if (confirmed == true)
            {
                var response = await ApiService.DeleteAsync(ApiEndpointConstants.ModelGetPostPutDelete, model.Id);
                if (response.IsSuccessStatusCode)
                {
                    await modelGrid.Reload();
                    NotificationService.Notify(NotificationSeverity.Success, "Başarılı", $"{model.Name} silme isteği gerçekleşti.");
                }
            }
        }
        async Task InsertRowModel(Model model)
        {
            modelToInsert = model;
            model.BrandId = SelectedBrand[0].Id;
            await modelGrid.InsertRow(modelToInsert);
            StateHasChanged();
        }
        async Task OnCreateRowModel(Model model)
        {
            model.BrandId = SelectedBrand.First().Id;

            var response = await ApiService.PostAsync(ApiEndpointConstants.ModelGetPostPutDelete, model);

            if (response.IsSuccessStatusCode)
            {
                var createdModel = await response.Content.ReadFromJsonAsync<Model>();
                if (createdModel != null)
                {
                    SelectedBrand.First().Models.Add(createdModel);
                    NotificationService.Notify(NotificationSeverity.Success, "Başarılı", $"Yeni model oluşturuldu.");
                }
            }
            else
            {
                NotificationService.Notify(NotificationSeverity.Error, "Hata", $"Model oluşturma hatası: {response.StatusCode}");
            }
            modelToInsert = null;
            await modelGrid.Reload();
            StateHasChanged();
        }
        #endregion
    }
}
