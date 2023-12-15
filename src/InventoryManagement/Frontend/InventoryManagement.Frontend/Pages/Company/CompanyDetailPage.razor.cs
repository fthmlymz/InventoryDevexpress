using InventoryManagement.Frontend.Common;
using InventoryManagement.Frontend.Common.Exceptions;
using InventoryManagement.Frontend.Constants;
using InventoryManagement.Frontend.DTOs.Brand;
using InventoryManagement.Frontend.DTOs.Category;
using InventoryManagement.Frontend.DTOs.Company;
using InventoryManagement.Frontend.Services;
using InventoryManagement.Frontend.Services.Authorization;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;


namespace InventoryManagement.Frontend.Pages.Company
{
    public partial class CompanyDetailPage : ComponentBase
    {
        #region Inject
        [Inject] ApiService _apiService { get; set; }
        [Inject] CommunicationService _communicationService { get; set; }
        [Inject] NavigationManager _navigationManager { get; set; }
        [Inject] NotificationService _notificationService { get; set; }
        [Inject] DialogService _dialogService { get; set; }
        [Inject] IAuthorizationService _authorizationService { get; set; }
        #endregion


        private CompanyDto? SelectedCompany { get; set; }

        #region Category-CategorySub
        private PaginatedResult<CategoryDto>? categoryModel { get; set; }
        private PaginatedResult<CategorySubModel>? categorySubModel { get; set; }
        #endregion

        #region Brand-Model
        private PaginatedResult<BrandDto>? brandModel { get; set; }
        private PaginatedResult<ModelDto>? modelModel { get; set; }
        #endregion


        //Master Detail
        IList<CategoryDto> SelectedCategory { get; set; }
        IList<BrandDto> SelectedBrand { get; set; }



        #region CategoryModel
        //Table Edit, Clone
        private Dictionary<int, CategoryDto> originalCategoryDataList = new Dictionary<int, CategoryDto>();//cache
        IQueryable<CategoryDto> categories;
        private CategoryDto categoryToInsert;
        private CategoryDto categoryToUpdate;
        private List<CategoryDto> categoriesToUpdate = new List<CategoryDto>();
        private RadzenDataGrid<CategoryDto> categoryGrid;
        #endregion

        #region CategorySubModel
        //Table Edit, Clone
        private Dictionary<int, CategorySubModel> originalCategorySubDataList = new Dictionary<int, CategorySubModel>();//cache
        IQueryable<CategorySubModel> categoriesSub;
        private CategorySubModel categorySubToInsert;
        private CategorySubModel categorySubToUpdate;
        private List<CategorySubModel> categoriesSubToUpdate = new List<CategorySubModel>();
        private RadzenDataGrid<CategorySubModel> categorySubGrid;
        #endregion


        #region BrandModel
        //Table Edit, Clone
        private Dictionary<int, BrandDto> originalBrandDataList = new Dictionary<int, BrandDto>();//cache
        IQueryable<BrandDto> brands;
        private BrandDto brandToInsert;
        private BrandDto brandToUpdate;
        private List<BrandDto> brandsToUpdate = new List<BrandDto>();
        private RadzenDataGrid<BrandDto> brandGrid;
        #endregion
        #region BrandModelsModel
        //Table Edit, Clone
        private Dictionary<int, ModelDto> originalModelDataList = new Dictionary<int, ModelDto>();//cache
        IQueryable<ModelDto> modelsSub;
        private ModelDto modelToInsert;
        private ModelDto modelToUpdate;
        private List<ModelDto> modelsToUpdate = new List<ModelDto>();
        private RadzenDataGrid<ModelDto> modelGrid;
        #endregion




        protected override async void OnInitialized()
        {
            SelectedCompany = _communicationService.GetSelectedCompany();
            _communicationService.OnCompanySelected += SetSelectedCompany;

            if (SelectedCompany == null)
            {
                _navigationManager.NavigateTo("/company");
                return;
            }

            await GetCategoryList();
            await GetBrandList();
        }

        #region Selected Company Information
        private async Task SetSelectedCompany(CompanyDto selectedCompany)
        {
            SelectedCompany = selectedCompany;

            await Task.Yield();
            StateHasChanged();
        }
        public void Dispose()
        {
            _communicationService.OnCompanySelected -= SetSelectedCompany;
        }
        #endregion

        #region Category List
        async Task GetCategoryList()
        {
            try
            {
                var jsonDocumentTask = _apiService.GetAsync<System.Text.Json.JsonDocument>($"{ApiEndpointConstants.CompanyGetPostPutDelete}/{SelectedCompany.Id}");
                var jsonDocument = await jsonDocumentTask;

                var dataElement = jsonDocument.RootElement.GetProperty("data");
                var categoriesElement = dataElement.GetProperty("categories");
                var categories = System.Text.Json.JsonSerializer.Deserialize<List<CategoryDto>>(categoriesElement);

                categoryModel = new PaginatedResult<CategoryDto> { data = categories };

                StateHasChanged();
            }
            catch (AggregateException ex)
            {
                foreach (var innerException in ex.InnerExceptions)
                {
                    if (innerException is Exception exception)
                    {
                        _notificationService.Notify(NotificationSeverity.Error, "Hata", $"Sorgulama hatası : {exception.Message}", duration: 6000);
                    }
                }
            }
        }
        #endregion
        #region GetBrand List
        async Task GetBrandList()
        {
            Console.WriteLine(SelectedCompany.Id);

            try
            {
                var jsonDocumentTask = _apiService.GetAsync<System.Text.Json.JsonDocument>($"{ApiEndpointConstants.CompanyGetPostPutDelete}/{SelectedCompany.Id}");
                var jsonDocument = await jsonDocumentTask;

                Console.WriteLine(jsonDocument);

                var dataElement = jsonDocument.RootElement.GetProperty("data");
                var brandsElement = dataElement.GetProperty("brands");
                var brands = System.Text.Json.JsonSerializer.Deserialize<List<BrandDto>>(brandsElement);

                brandModel = new PaginatedResult<BrandDto> { data = brands };

                StateHasChanged();
            }
            catch (AggregateException ex)
            {
                foreach (var innerException in ex.InnerExceptions)
                {
                    if (innerException is Exception exception)
                    {
                        _notificationService.Notify(NotificationSeverity.Error, "Hata", $"Sorgulama hatası : {exception.Message}", duration: 6000);
                    }
                }
            }
        }
        #endregion


        #region DataTable Category
        async Task EditRowCategory(CategoryDto category)
        {
            //originalCategoryDataList[category.Id] = category.Clone();
            //categoriesToUpdate.Add(category);
            //category.CompanyId = SelectedCompany.Id;

            //Task editRowTask = categoryGrid.EditRow(category);
            //Task reloadTask = categoryGrid.Reload();

            //await Task.WhenAll(editRowTask, reloadTask);
            //StateHasChanged();
        }
        async void OnUpdateRowCategory(CategoryDto category)
        {
            if (category == categoryToInsert)
            {
                categoryToInsert = null;
            }
            categoryToUpdate = null;
            var updatedCategory = await _apiService.PutAsync(ApiEndpointConstants.CategoryGetPostPutDelete, category);
            if (updatedCategory.IsSuccessStatusCode)
            {
                _notificationService.Notify(NotificationSeverity.Success, "Başarılı", $"{category.Name} isimli kategori güncellendi");
            }
            StateHasChanged();
        }
        async Task SaveRowCategory(CategoryDto category)
        {
            if (categoryGrid != null)
            {
                await categoryGrid.UpdateRow(category);
            }
        }
        void CancelEditCategory(CategoryDto category)
        {
            //if (category == categoryToInsert)
            //{
            //    categoryToInsert = null;
            //}
            //else if (categoriesToUpdate.Contains(category))
            //{
            //    if (originalCategoryDataList.TryGetValue(category.Id, out var originalCompany))
            //    {
            //        category.SetPropertiesFromJson(originalCompany);
            //    }
            //    categoriesToUpdate.Remove(category);
            //}
            //categoryGrid.CancelEditRow(category);
            //categoryGrid.Reload();
            //StateHasChanged();
        }
        async Task DeleteRowCategory(CategoryDto category)
        {
            if (category == categoryToInsert)
            {
                categoryToInsert = null;
            }
            else if (category == categoryToUpdate)
            {
                categoryToUpdate = null;
            }
            if (categoryModel.data.Contains(category))
            {
                categoryModel.data.Remove(category);
                categoryModel.totalCount--;
                //await companyGrid.Reload();
            }

            bool? confirmed = await _dialogService.Confirm($"<b>{category.Name}</b> isimli kategori silinme işlemini onaylıyor musunuz?\n" +
                                                          $"<br><li>Envantere bağlı tüm kayıtlar,\n" +
                                                          $"<br><li> Kategori ve alt kategoriler silinecek",
                                                           "Silme Onayı");
            if (confirmed == true)
            {
                var response = await _apiService.DeleteAsync(ApiEndpointConstants.CategoryGetPostPutDelete, category.Id);
                if (response.IsSuccessStatusCode)
                {
                    await categoryGrid.Reload();
                    _notificationService.Notify(NotificationSeverity.Success, "Başarılı", $"Silme isteği gerçekleşti.");
                }
            }
        }
        async Task InsertRowCategory(CategoryDto category)
        {
            categoryToInsert = category;
            //category.CompanyId = SelectedCompany.Id;
            await categoryGrid.InsertRow(categoryToInsert);
            StateHasChanged();
        }
        async void OnCreateRowCategory(CategoryDto category)
        {
            try
            {
                var insertedCategory = await _apiService.PostAsync(ApiEndpointConstants.CategoryGetPostPutDelete, category);

                if (insertedCategory.IsSuccessStatusCode)
                {
                    var insertedCategoryModel = await insertedCategory.Content.ReadFromJsonAsync<CategoryDto>();
                    categoryModel.data?.Add(insertedCategoryModel);
                    categoryModel.totalCount++;
                }
            }
            catch (ApiException ex)
            {
                _notificationService.Notify(NotificationSeverity.Error, "Hata", $"Kategori oluşturma hatası: {ex.Message}");
            }
            categoryToInsert = null;
            await categoryGrid.Reload();
            StateHasChanged();
        }
        #endregion
        #region DataTable CategorySub
        async Task EditRowCategorySub(CategorySubModel categorySub)
        {
            //originalCategorySubDataList[categorySub.Id] = categorySub.Clone();
            //categoriesSubToUpdate.Add(categorySub);

            //Task editRowTask = categorySubGrid.EditRow(categorySub);
            //Task reloadTask = categorySubGrid.Reload();

            //await Task.WhenAll(editRowTask, reloadTask);
            //StateHasChanged();
        }
        async void OnUpdateRowCategorySub(CategorySubModel categorySub)
        {
            if (categorySub == categorySubToInsert)
            {
                categorySubToInsert = null;
            }
            categorySubToUpdate = null;
            var updatedCategorySub = await _apiService.PutAsync(ApiEndpointConstants.CategorySubGetPostPutDelete, categorySub);
            if (updatedCategorySub.IsSuccessStatusCode)
            {
               _notificationService.Notify(NotificationSeverity.Success, "Başarılı", $"{categorySub.Name} isimli alt kategori güncellendi");
            }
            StateHasChanged();
        }
        async Task SaveRowCategorySub(CategorySubModel categorySub)
        {
            if (categorySubGrid != null)
            {
                await categorySubGrid.UpdateRow(categorySub);
            }
        }
        void CancelEditCategorySub(CategorySubModel categorySub)
        {
            //if (categorySub == categorySubToInsert)
            //{
            //    categorySubToInsert = null;
            //}
            //else if (categoriesSubToUpdate.Contains(categorySub))
            //{
            //    if (originalCategorySubDataList.TryGetValue(categorySub.Id, out var originalCompanySub))
            //    {
            //        categorySub.SetPropertiesFromJson(originalCompanySub);
            //    }
            //    categoriesSubToUpdate.Remove(categorySub);
            //}
            //categorySubGrid.CancelEditRow(categorySub);
            //categorySubGrid.Reload();
            //StateHasChanged();
        }
        async Task DeleteRowCategorySub(CategorySubModel categorySub)
        {
            if (categorySub == categorySubToInsert)
            {
                categorySubToInsert = null;
            }
            else if (categorySub == categorySubToUpdate)
            {
                categorySubToUpdate = null;
            }

            if (categoryModel?.data != null)
            {
                var categoryToRemove = categoryModel.data.FirstOrDefault(c => c.CategorySubs?.Contains(categorySub) == true);
                if (categoryToRemove != null)
                {
                    categoryToRemove.CategorySubs.Remove(categorySub);
                    categoryModel.totalCount--;
                }
            }

            bool? confirmed = await _dialogService.Confirm($"<b>{categorySub.Name}</b> isimli alt kategori silinme işlemini onaylıyor musunuz?\n" + "Silme Onayı");

            if (confirmed == true)
            {
                var response = await _apiService.DeleteAsync(ApiEndpointConstants.CategorySubGetPostPutDelete, categorySub.Id);
                if (response.IsSuccessStatusCode)
                {
                    await categorySubGrid.Reload();
                    _notificationService.Notify(NotificationSeverity.Success, "Başarılı", $"{categorySub.Name} silme isteği gerçekleşti.");
                }
            }
        }
        async Task InsertRowCategorySub(CategorySubModel categorySub)
        {
            categorySubToInsert = categorySub;
            categorySub.CategoryId = SelectedCategory[0].Id;
            await categorySubGrid.InsertRow(categorySubToInsert);
            StateHasChanged();
        }
        async Task OnCreateRowCategorySub(CategorySubModel categorySub)
        {
            categorySub.CategoryId = SelectedCategory.First().Id;

            var response = await _apiService.PostAsync(ApiEndpointConstants.CategorySubGetPostPutDelete, categorySub);

            if (response.IsSuccessStatusCode)
            {
                var createdCategorySub = await response.Content.ReadFromJsonAsync<CategorySubModel>();
                if (createdCategorySub != null)
                {
                    SelectedCategory.First().CategorySubs.Add(createdCategorySub);
                    _notificationService.Notify(NotificationSeverity.Success, "Başarılı", $"Yeni alt kategori oluşturuldu.");
                }
            }
            else
            {
                _notificationService.Notify(NotificationSeverity.Error, "Hata", $"Alt kategori oluşturma hatası: {response.StatusCode}");
            }
            categorySubToInsert = null;
            await categorySubGrid.Reload();
            StateHasChanged();
        }
        #endregion


        #region DataTable Brand
        async Task EditRowBrand(BrandDto brand)
        {
            //originalBrandDataList[brand.Id] = brand.Clone();
            //brandsToUpdate.Add(brand);
            //brand.CompanyId = SelectedCompany.Id;

            //Task editRowTask = brandGrid.EditRow(brand);
            //Task reloadTask = brandGrid.Reload();

            //await Task.WhenAll(editRowTask, reloadTask);
            //StateHasChanged();
        }
        async void OnUpdateRowBrand(BrandDto brand)
        {
            if (brand == brandToInsert)
            {
                brandToInsert = null;
            }
            brandToUpdate = null;
            var updatedBrand = await _apiService.PutAsync(ApiEndpointConstants.BrandGetPostPutDelete, brand);
            if (updatedBrand.IsSuccessStatusCode)
            {
                _notificationService.Notify(NotificationSeverity.Success, "Başarılı", $"{brand.Name} isimli marka güncellendi");
            }
            StateHasChanged();
        }
        async Task SaveRowBrand(BrandDto brand)
        {
            if (brandGrid != null)
            {
                await brandGrid.UpdateRow(brand);
            }
        }
        void CancelEditBrand(BrandDto brand)
        {
            if (brand == brandToInsert)
            {
                brandToInsert = null;
            }
            else if (brandsToUpdate.Contains(brand))
            {
                if (originalBrandDataList.TryGetValue(brand.Id, out var originalCompany))
                {
                    //brand.SetPropertiesFromJson(originalCompany);
                }
                brandsToUpdate.Remove(brand);
            }
            brandGrid.CancelEditRow(brand);
            brandGrid.Reload();
            StateHasChanged();
        }
        async Task DeleteRowBrand(BrandDto brand)
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

            bool? confirmed = await _dialogService.Confirm($"<b>{brand.Name}</b> isimli marka silinme işlemini onaylıyor musunuz?\n" +
                                                          $"<br><li>Envantere bağlı tüm kayıtlar,\n" +
                                                          $"<br><li> Marka ve modeller de silinecek",
                                                           "Silme Onayı");
            if (confirmed == true)
            {
                var response = await _apiService.DeleteAsync(ApiEndpointConstants.BrandGetPostPutDelete, brand.Id);
                if (response.IsSuccessStatusCode)
                {
                    await brandGrid.Reload();
                    _notificationService.Notify(NotificationSeverity.Success, "Başarılı", $"Silme isteği gerçekleşti.");
                }
            }
        }
        async Task InsertRowBrand(BrandDto brand)
        {
            //brandToInsert = brand;
            //brand.CompanyId = SelectedCompany.Id;
            //await brandGrid.InsertRow(brandToInsert);
            //StateHasChanged();
        }
        async void OnCreateRowBrand(BrandDto brand)
        {
            try
            {
                var insertedBrand = await _apiService.PostAsync(ApiEndpointConstants.BrandGetPostPutDelete, brand);

                if (insertedBrand.IsSuccessStatusCode)
                {
                    var insertedBrandModel = await insertedBrand.Content.ReadFromJsonAsync<BrandDto>();
                    brandModel.data?.Add(insertedBrandModel);
                    brandModel.totalCount++;
                }
            }
            catch (ApiException ex)
            {
                _notificationService.Notify(NotificationSeverity.Error, "Hata", $"Marka oluşturma hatası: {ex.Message}");
            }
            brandToInsert = null;
            await brandGrid.Reload();
            StateHasChanged();
        }
        #endregion
        #region DataTable Brand - Model
        async Task EditRowModel(ModelDto model)
        {
            //originalModelDataList[model.Id] = model.Clone();
            //modelsToUpdate.Add(model);

            //Task editRowTask = modelGrid.EditRow(model);
            //Task reloadTask = modelGrid.Reload();

            //await Task.WhenAll(editRowTask, reloadTask);
            //StateHasChanged();
        }
        async void OnUpdateRowModel(ModelDto model)
        {
            if (model == modelToInsert)
            {
                modelToInsert = null;
            }
            modelToUpdate = null;
            var updatedModel = await _apiService.PutAsync(ApiEndpointConstants.ModelGetPostPutDelete, model);
            if (updatedModel.IsSuccessStatusCode)
            {
                _notificationService.Notify(NotificationSeverity.Success, "Başarılı", $"{model.Name} isimli model güncellendi");
            }
            StateHasChanged();
        }
        async Task SaveRowModel(ModelDto model)
        {
            if (modelGrid != null)
            {
                await modelGrid.UpdateRow(model);
            }
        }
        void CancelEditModel(ModelDto model)
        {
            if (model == modelToInsert)
            {
                modelToInsert = null;
            }
            else if (modelsToUpdate.Contains(model))
            {
                if (originalModelDataList.TryGetValue(model.Id, out var originalCompanySub))
                {
                   // model.SetPropertiesFromJson(originalCompanySub);
                }
                modelsToUpdate.Remove(model);
            }
            modelGrid.CancelEditRow(model);
            modelGrid.Reload();
            StateHasChanged();
        }
        async Task DeleteRowModel(ModelDto model)
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

            bool? confirmed = await _dialogService.Confirm($"<b>{model.Name}</b> isimli alt kategori silinme işlemini onaylıyor musunuz?\n" + "Silme Onayı");

            if (confirmed == true)
            {
                var response = await _apiService.DeleteAsync(ApiEndpointConstants.ModelGetPostPutDelete, model.Id);
                if (response.IsSuccessStatusCode)
                {
                    await modelGrid.Reload();
                    _notificationService.Notify(NotificationSeverity.Success, "Başarılı", $"{model.Name} silme isteği gerçekleşti.");
                }
            }
        }
        async Task InsertRowModel(ModelDto model)
        {
            modelToInsert = model;
            model.BrandId = SelectedBrand[0].Id;
            await modelGrid.InsertRow(modelToInsert);
            StateHasChanged();
        }
        async Task OnCreateRowModel(ModelDto model)
        {
            model.BrandId = SelectedBrand.First().Id;

            var response = await _apiService.PostAsync(ApiEndpointConstants.ModelGetPostPutDelete, model);

            if (response.IsSuccessStatusCode)
            {
                var createdModel = await response.Content.ReadFromJsonAsync<ModelDto>();
                if (createdModel != null)
                {
                    SelectedBrand.First().Models.Add(createdModel);
                    _notificationService.Notify(NotificationSeverity.Success, "Başarılı", $"Yeni model oluşturuldu.");
                }
            }
            else
            {
                _notificationService.Notify(NotificationSeverity.Error, "Hata", $"Model oluşturma hatası: {response.StatusCode}");
            }
            modelToInsert = null;
            await modelGrid.Reload();
            StateHasChanged();
        }
        #endregion



    }
}
