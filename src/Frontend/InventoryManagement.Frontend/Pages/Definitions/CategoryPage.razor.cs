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
    public partial class CategoryPage: ComponentBase
    {
        #region Inject
        [Inject] ApiService? ApiService { get; set; }
        [Inject] IAuthorizationService? AuthorizationService { get; set; }
        [Inject] NotificationService? NotificationService { get; set; }
        [Inject] DialogService? DialogService { get; set; }
        #endregion



        private IGrid? GridCategory { get; set; }
        private IGrid? GridCategorySub { get; set; }
        private PaginatedResult<CategoryDto>? categoryModel { get; set; }
        private CategoryDto? SelectedCategory { get; set; }



        #region Pagination
        private int PageNumber { get; set; } = 1;
        int PageSize { get; set; } = 250;
        #endregion



        protected override async void OnInitialized()
        {
            await GetCategoryList();
        }



        #region Category List
        async Task HandleCategoryPageNumberChanged(int newPageNumber)
        {
            PageNumber = newPageNumber + 1;
            await GetCategoryList();
            StateHasChanged();
        }
        async Task GetCategoryList()
        {
            categoryModel = await ApiService!.GetAsync<PaginatedResult<CategoryDto>>($"{ApiEndpointConstants.GetCategory}?PageNumber={PageNumber}&PageSize={PageSize}");
            StateHasChanged();
        }
        #endregion


        #region Category
        void GridCategory_CustomizeEditModel(GridCustomizeEditModelEventArgs e)
        {
            if (e.IsNew)
            {
                var category = (CategoryDto)e.EditModel;
                category.Name = null;
            }
        }
        async Task GridCategory_EditModelSaving(GridEditModelSavingEventArgs e)
        {
            if (e.IsNew)
            {
                var newCategory = (CategoryDto)e.EditModel;
                var response = await ApiService!.PostAsync(ApiEndpointConstants.PostCategory, newCategory);
                if (response.IsSuccessStatusCode)
                {
                    var insertedCategoryModel = await response.Content.ReadFromJsonAsync<CategoryDto>();
#pragma warning disable CS8604 // Possible null reference argument.
                    categoryModel?.data?.Add(insertedCategoryModel);
#pragma warning restore CS8604 // Possible null reference argument.
                    categoryModel!.totalCount++;
                    StateHasChanged();
                }
            }
            else
            {
                var updatedCategory = (CategoryDto)e.EditModel;
                var response = await ApiService!.PutAsync($"{ApiEndpointConstants.PutCategory}", updatedCategory);
                if (response.IsSuccessStatusCode)
                {
                    var index = categoryModel!.data!.FindIndex(c => c.Id == updatedCategory.Id);
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
            var response = await ApiService!.DeleteAsync(ApiEndpointConstants.DeleteCategory, category.Id);
            if (response.IsSuccessStatusCode)
            {
                categoryModel?.data?.Remove(category);

#pragma warning disable CS8602 // Dereference of a possibly null reference.
                foreach (var parentCategory in categoryModel?.data)
                {
                    parentCategory?.CategorySubs?.RemoveAll(cs => cs.CategoryId == category.Id);
                }
#pragma warning restore CS8602 // Dereference of a possibly null reference.

                categoryModel.totalCount--;
                StateHasChanged();
                GridCategory?.Reload();
                NotificationService?.Notify(NotificationSeverity.Success, "Başarılı", $"{category.Name} silme işlemi gerçekleşti.");
            }
            else
            {
                NotificationService?.Notify(NotificationSeverity.Error, "Başarısız", $"{category.Name} silme işlemi gerçekleşti.");
            }
        }
        #endregion




        #region Category Sub
        void GridCategorySub_CustomizeEditModel(GridCustomizeEditModelEventArgs e)
        {
            if (e.IsNew)
            {
                var categorySub = (CategorySubDto)e.EditModel;
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                categorySub.CategoryId = SelectedCategory.Id;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                categorySub.Name = null;
            }
        }
        async Task GridCategorySub_EditModelSaving(GridEditModelSavingEventArgs e)
        {
            var categorySub = (CategorySubDto)e.EditModel;

            if (e.IsNew)
            {
                var response = await ApiService!.PostAsync(ApiEndpointConstants.PostCategorySub, categorySub);
                if (response.IsSuccessStatusCode)
                {
                    var insertedCategorySub = await response.Content.ReadFromJsonAsync<CategorySubDto>();
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
                var response = await ApiService!.PutAsync($"{ApiEndpointConstants.PutCategorySub}", categorySub);
                if (response.IsSuccessStatusCode)
                {
                    var parentCategory = categoryModel?.data?.FirstOrDefault(c => c.Id == categorySub.CategoryId);
                    if (parentCategory != null)
                    {
                        var index = parentCategory!.CategorySubs!.FindIndex(cs => cs.Id == categorySub.Id);
                        if (index != -1)
                        {
                            parentCategory.CategorySubs[index] = categorySub;
                            StateHasChanged();
                        }
                    }
                }
            }
        }

        public async Task DeleteCategorySub(CategorySubDto category)
        {
            var categorySubToDelete = (CategorySubDto)category;
            var response = await ApiService!.DeleteAsync(ApiEndpointConstants.DeleteCategorySub, categorySubToDelete.Id);
            if (response.IsSuccessStatusCode)
            {
                var parentCategory = categoryModel?.data?.FirstOrDefault(c => c.Id == categorySubToDelete.CategoryId);
                if (parentCategory != null)
                {
                    parentCategory?.CategorySubs?.Remove(categorySubToDelete);
                    StateHasChanged();
                }

                NotificationService?.Notify(NotificationSeverity.Success, "Başarılı", $"{categorySubToDelete.Name} silme isteği gerçekleşti.");
            }
            else
            {
                NotificationService?.Notify(NotificationSeverity.Error, "Başarısız", $"{categorySubToDelete.Name} silme isteği gerçekleşti.");
            }
        }
        #endregion
    }
}
