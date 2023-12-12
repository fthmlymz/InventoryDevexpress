using DevExpress.Blazor;
using InventoryManagement.Frontend.Common;
using InventoryManagement.Frontend.Constants;
using InventoryManagement.Frontend.DTOs.Category;
using InventoryManagement.Frontend.Services;
using InventoryManagement.Frontend.Services.Authorization;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace InventoryManagement.Frontend.Pages.Definitions
{
    public partial class Category : ComponentBase
    {
        #region Inject
        [Inject] ApiService? _apiService { get; set; }
        [Inject] IAuthorizationService? _authorizationService { get; set; }
        [Inject] NotificationService? _notificationService { get; set; }
        [Inject] DialogService? _dialogService { get; set; }
        #endregion



        private IGrid? GridCategory { get; set; }
        private IGrid? GridCategorySub { get; set; }
        private PaginatedResult<CategoryDto>? categoryModel { get; set; }
        private CategoryDto? SelectedCategory { get; set; }



        protected override async void OnInitialized()
        {
            await GetCategoryList();
        }



        #region Category List
        async Task GetCategoryList()
        {
            categoryModel = await _apiService.GetAsync<PaginatedResult<CategoryDto>>($"{ApiEndpointConstants.CategoryGetPostPutDelete}?PageNumber=1&PageSize=1000");
            StateHasChanged();
        }
        #endregion


        #region Category
        //void Grid_CustomizeDataRowEditor(GridCustomizeDataRowEditorEventArgs e)
        //{
        //    ((ITextEditSettings)e.EditSettings).ShowValidationIcon = true;
        //}
        void GridCategory_CustomizeEditModel(GridCustomizeEditModelEventArgs e)
        {
            if (e.IsNew)
            {
                var category = (CategoryDto)e.EditModel;
                //category.CompanyId = 0; // CompanyId aktif, sadece null gonderiliyor
                category.Name = null;
            }
        }
        async Task GridCategory_EditModelSaving(GridEditModelSavingEventArgs e)
        {
            if (e.IsNew)
            {
                var newCategory = (CategoryDto)e.EditModel;
                var response = await _apiService.PostAsync(ApiEndpointConstants.CategoryGetPostPutDelete, newCategory);
                if (response.IsSuccessStatusCode)
                {
                    var insertedCategoryModel = await response.Content.ReadFromJsonAsync<CategoryDto>();
                    categoryModel?.data?.Add(insertedCategoryModel);
                    categoryModel.totalCount++;
                    StateHasChanged();
                }
            }
            else
            {
                var updatedCategory = (CategoryDto)e.EditModel;
                var response = await _apiService.PutAsync($"{ApiEndpointConstants.CategoryGetPostPutDelete}", updatedCategory);
                if (response.IsSuccessStatusCode)
                {
                    var index = categoryModel.data.FindIndex(c => c.Id == updatedCategory.Id);
                    if (index != -1)
                    {
                        categoryModel.data[index] = updatedCategory;
                        StateHasChanged();
                    }
                }
            }
        }

        public async Task DeleteCategory(CategoryDto category)
        {
            var response = await _apiService.DeleteAsync(ApiEndpointConstants.CategoryGetPostPutDelete, category.Id);
            if (response.IsSuccessStatusCode)
            {
                categoryModel?.data?.Remove(category);

                foreach (var parentCategory in categoryModel?.data)
                {
                    parentCategory?.CategorySubs?.RemoveAll(cs => cs.CategoryId == category.Id);
                }

                categoryModel.totalCount--;
                StateHasChanged();
                GridCategory?.Reload();
                _notificationService?.Notify(NotificationSeverity.Success, "Başarılı", $"{category.Name} silme işlemi gerçekleşti.");
            }
            else
            {
                _notificationService?.Notify(NotificationSeverity.Error, "Başarısız", $"{category.Name} silme işlemi gerçekleşti.");
            }
        }
        #endregion


        #region Category Sub
        void GridCategorySub_CustomizeEditModel(GridCustomizeEditModelEventArgs e)
        {
            if (e.IsNew)
            {
                var categorySub = (CategorySubModel)e.EditModel;
                //category.CompanyId = 1; // CompanyId aktif, sadece blazor uzerinden null gonderiliyor
                categorySub.CategoryId = SelectedCategory.Id;
                categorySub.Name = null;
            }
        }
        async Task GridCategorySub_EditModelSaving(GridEditModelSavingEventArgs e)
        {
            var categorySub = (CategorySubModel)e.EditModel;

            if (e.IsNew)
            {
                var response = await _apiService.PostAsync(ApiEndpointConstants.CategorySubGetPostPutDelete, categorySub);
                if (response.IsSuccessStatusCode)
                {
                    var insertedCategorySub = await response.Content.ReadFromJsonAsync<CategorySubModel>();
                    var parentCategory = categoryModel?.data?.FirstOrDefault(c => c.Id == categorySub.CategoryId);
                    if (parentCategory != null && insertedCategorySub != null)
                    {
                        parentCategory?.CategorySubs?.Add(insertedCategorySub);
                        StateHasChanged();
                    }
                }
            }
            else
            {
                var response = await _apiService.PutAsync($"{ApiEndpointConstants.CategorySubGetPostPutDelete}", categorySub);
                if (response.IsSuccessStatusCode)
                {
                    var parentCategory = categoryModel?.data?.FirstOrDefault(c => c.Id == categorySub.CategoryId);
                    if (parentCategory != null)
                    {
                        var index = parentCategory.CategorySubs.FindIndex(cs => cs.Id == categorySub.Id);
                        if (index != -1)
                        {
                            parentCategory.CategorySubs[index] = categorySub;
                            StateHasChanged();
                        }
                    }
                }
            }
        }

        public async Task DeleteCategorySub(CategorySubModel category)
        {
            var categorySubToDelete = (CategorySubModel)category;
            var response = await _apiService.DeleteAsync(ApiEndpointConstants.CategorySubGetPostPutDelete, categorySubToDelete.Id);
            if (response.IsSuccessStatusCode)
            {
                var parentCategory = categoryModel?.data?.FirstOrDefault(c => c.Id == categorySubToDelete.CategoryId);
                if (parentCategory != null)
                {
                    parentCategory?.CategorySubs?.Remove(categorySubToDelete);
                    StateHasChanged();
                }

                _notificationService?.Notify(NotificationSeverity.Success, "Başarılı", $"{categorySubToDelete.Name} silme isteği gerçekleşti.");
            }
            else
            {
                _notificationService?.Notify(NotificationSeverity.Error, "Başarısız", $"{categorySubToDelete.Name} silme isteği gerçekleşti.");
            }
        }
        #endregion
    }
}
