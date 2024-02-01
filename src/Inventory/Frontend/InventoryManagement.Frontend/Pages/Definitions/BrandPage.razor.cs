using DevExpress.Blazor;
using InventoryManagement.Frontend.Common;
using InventoryManagement.Frontend.Constants;
using InventoryManagement.Frontend.DTOs.Brand;
using InventoryManagement.Frontend.Services;
using InventoryManagement.Frontend.Services.Authorization;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace InventoryManagement.Frontend.Pages.Definitions
{
    public partial class BrandPage : ComponentBase
    {
        #region Injection
        [Inject] ApiService? _apiService { get; set; }
        [Inject] IAuthorizationService? AuthorizationService { get; set; }
        [Inject] NotificationService? _notificationService { get; set; }
        [Inject] DialogService? _dialogService { get; set; }
        #endregion


        private IGrid? GridBrand { get; set; }
        private IGrid? GridModel { get; set; }

        private PaginatedResult<BrandDto>? brandModel { get; set; }
        private BrandDto? SelectedBrand { get; set; }


        #region Pagination
        private int PageNumber { get; set; } = 1;
        int PageSize { get; set; } = 250;
        #endregion


        protected override async Task OnInitializedAsync()
        {
            await GetBrandList();
        }

        #region Brand
        async Task HandleBrandPageNumberChanged(int newPageNumber)
        {
            PageNumber = newPageNumber + 1;
            await GetBrandList();
            StateHasChanged();
        }
        async Task GetBrandList()
        {
            brandModel = await _apiService.GetAsync<PaginatedResult<BrandDto>>($"{ApiEndpointConstants.GetBrand}?PageNumber={PageNumber}&PageSize={PageSize}");
            StateHasChanged();
        }
        void GridBrand_CustomizeEditModel(GridCustomizeEditModelEventArgs e)
        {
            if (e.IsNew)
            {
                var brand = (BrandDto)e.EditModel;
                brand.Name = null;
            }
        }
        async Task GridBrand_EditModelSaving(GridEditModelSavingEventArgs e)
        {
            if (e.IsNew)
            {
                var newBrand = (BrandDto)e.EditModel;
                var response = await _apiService.PostAsync(ApiEndpointConstants.PostBrand, newBrand);
                if (response.IsSuccessStatusCode)
                {
                    var insertedBrand = await response.Content.ReadFromJsonAsync<BrandDto>();
                    brandModel?.data?.Add(insertedBrand);
                    brandModel.totalCount++;
                    StateHasChanged();
                }
            }
            else
            {
                var updatedBrand = (BrandDto)e.EditModel;
                var response = await _apiService.PutAsync($"{ApiEndpointConstants.PutBrand}", updatedBrand);
                if (response.IsSuccessStatusCode)
                {
                    var index = brandModel.data.FindIndex(c => c.Id == updatedBrand.Id);
                    if (index != -1)
                    {
                        brandModel.data[index] = updatedBrand;
                        StateHasChanged();
                    }
                }
            }
        }

        public async Task DeleteBrand(BrandDto brand)
        {
            var response = await _apiService.DeleteAsync(ApiEndpointConstants.DeleteBrand, brand.Id);
            if (response.IsSuccessStatusCode)
            {
                brandModel?.data?.Remove(brand);

                foreach (var parentBrand in brandModel?.data)
                {
                    parentBrand?.Models?.RemoveAll(cs => cs.BrandId == brand.Id);
                }

                brandModel.totalCount--;
                StateHasChanged();
                GridBrand?.Reload();
                _notificationService.Notify(NotificationSeverity.Success, "Başarılı", $"{brand.Name} silme işlemi gerçekleşti.");
            }
            else
            {
                _notificationService.Notify(NotificationSeverity.Error, "Başarısız", $"{brand.Name} silme işlemi gerçekleşti.");
            }
        }
        #endregion



        #region Model
        void GridModel_CustomizeEditModel(GridCustomizeEditModelEventArgs e)
        {
            if (e.IsNew)
            {
                var model = (ModelDto)e.EditModel;
                Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(model));
                if (SelectedBrand != null)
                {
                    model.BrandId = SelectedBrand.Id;
                    model.Name = null;
                }
            }
        }
        async Task GridModel_EditModelSaving(GridEditModelSavingEventArgs e)
        {
            var modelSub = (ModelDto)e.EditModel;

            if (e.IsNew)
            {
                var response = await _apiService.PostAsync(ApiEndpointConstants.PostModel, modelSub);
                if (response.IsSuccessStatusCode)
                {
                    var insertedModel = await response.Content.ReadFromJsonAsync<ModelDto>();
                    var parentModel = brandModel?.data?.FirstOrDefault(c => c.Id == modelSub.BrandId);
                    if (parentModel != null && insertedModel != null)
                    {
                        parentModel.Models.Add(insertedModel);
                        StateHasChanged();
                    }
                }
            }
            else
            {
                var response = await _apiService.PutAsync($"{ApiEndpointConstants.PutModel}", modelSub);
                if (response.IsSuccessStatusCode)
                {
                    var parentModel = brandModel?.data?.FirstOrDefault(c => c.Id == modelSub.BrandId);
                    if (parentModel != null)
                    {
                        var index = parentModel.Models.FindIndex(cs => cs.Id == modelSub.Id);
                        if (index != -1)
                        {
                            parentModel.Models[index] = modelSub;
                            StateHasChanged();
                        }
                    }
                }
            }
        }
        public async Task DeleteModel(ModelDto category)
        {
            var modelToDelete = (ModelDto)category;
            var response = await _apiService.DeleteAsync(ApiEndpointConstants.DeleteModel, modelToDelete.Id);
            if (response.IsSuccessStatusCode)
            {
                var parentModel = brandModel?.data?.FirstOrDefault(c => c.Id == modelToDelete.BrandId);
                if (parentModel != null)
                {
                    parentModel?.Models?.Remove(modelToDelete);
                    StateHasChanged();
                }
                _notificationService.Notify(NotificationSeverity.Success, "Başarılı", $"{modelToDelete.Name} silme isteği gerçekleşti.");
            }
            else
            {
                _notificationService.Notify(NotificationSeverity.Error, "Başarısız", $"{modelToDelete.Name} silme isteği gerçekleşti.");
            }
        }
        #endregion
    }
}
