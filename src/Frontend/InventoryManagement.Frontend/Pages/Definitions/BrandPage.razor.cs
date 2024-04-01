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
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            brandModel = await _apiService.GetAsync<PaginatedResult<BrandDto>>($"{ApiEndpointConstants.GetBrand}?PageNumber={PageNumber}&PageSize={PageSize}");
#pragma warning restore CS8602 // Dereference of a possibly null reference.
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
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                var response = await _apiService.PostAsync(ApiEndpointConstants.PostBrand, newBrand);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                if (response.IsSuccessStatusCode)
                {
                    var insertedBrand = await response.Content.ReadFromJsonAsync<BrandDto>();
#pragma warning disable CS8604 // Possible null reference argument.
                    brandModel?.data?.Add(insertedBrand);
#pragma warning restore CS8604 // Possible null reference argument.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    brandModel.totalCount++;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                    StateHasChanged();
                }
            }
            else
            {
                var updatedBrand = (BrandDto)e.EditModel;
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                var response = await _apiService.PutAsync($"{ApiEndpointConstants.PutBrand}", updatedBrand);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                if (response.IsSuccessStatusCode)
                {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    var index = brandModel.data.FindIndex(c => c.Id == updatedBrand.Id);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
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
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var response = await _apiService.DeleteAsync(ApiEndpointConstants.DeleteBrand, brand.Id);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            if (response.IsSuccessStatusCode)
            {
                brandModel?.data?.Remove(brand);

#pragma warning disable CS8602 // Dereference of a possibly null reference.
                foreach (var parentBrand in brandModel?.data)
                {
                    parentBrand?.Models?.RemoveAll(cs => cs.BrandId == brand.Id);
                }
#pragma warning restore CS8602 // Dereference of a possibly null reference.

                brandModel.totalCount--;
                StateHasChanged();
                GridBrand?.Reload();
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                _notificationService.Notify(NotificationSeverity.Success, "Başarılı", $"{brand.Name} silme işlemi gerçekleşti.");
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            }
            else
            {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                _notificationService.Notify(NotificationSeverity.Error, "Başarısız", $"{brand.Name} silme işlemi gerçekleşti.");
#pragma warning restore CS8602 // Dereference of a possibly null reference.
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
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                var response = await _apiService.PostAsync(ApiEndpointConstants.PostModel, modelSub);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                if (response.IsSuccessStatusCode)
                {
                    var insertedModel = await response.Content.ReadFromJsonAsync<ModelDto>();
                    var parentModel = brandModel?.data?.FirstOrDefault(c => c.Id == modelSub.BrandId);
                    if (parentModel != null && insertedModel != null)
                    {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                        parentModel.Models.Add(insertedModel);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                        StateHasChanged();
                    }
                }
            }
            else
            {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                var response = await _apiService.PutAsync($"{ApiEndpointConstants.PutModel}", modelSub);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                if (response.IsSuccessStatusCode)
                {
                    var parentModel = brandModel?.data?.FirstOrDefault(c => c.Id == modelSub.BrandId);
                    if (parentModel != null)
                    {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                        var index = parentModel.Models.FindIndex(cs => cs.Id == modelSub.Id);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
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
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var response = await _apiService.DeleteAsync(ApiEndpointConstants.DeleteModel, modelToDelete.Id);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            if (response.IsSuccessStatusCode)
            {
                var parentModel = brandModel?.data?.FirstOrDefault(c => c.Id == modelToDelete.BrandId);
                if (parentModel != null)
                {
                    parentModel?.Models?.Remove(modelToDelete);
                    StateHasChanged();
                }
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                _notificationService.Notify(NotificationSeverity.Success, "Başarılı", $"{modelToDelete.Name} silme isteği gerçekleşti.");
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            }
            else
            {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                _notificationService.Notify(NotificationSeverity.Error, "Başarısız", $"{modelToDelete.Name} silme isteği gerçekleşti.");
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            }
        }
        #endregion
    }
}
