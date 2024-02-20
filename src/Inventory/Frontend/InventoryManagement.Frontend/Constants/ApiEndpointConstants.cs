namespace InventoryManagement.Frontend.Constants
{
    public class ApiEndpointConstants
    {
        public static string? InventoryManagementApi { get; private set; }
        public static string? KeycloakEndpoint { get; private set; }
        public static string? Realm { get; private set; }


        public static void Load(IConfiguration configuration)
        {
            InventoryManagementApi = configuration["InventoryManagementApi"];
            KeycloakEndpoint = configuration["Keycloak:KeycloakEndpoint"];
            Realm = configuration["Keycloak:RealmId"];
        }

        #region Keycloak
        public static string KeycloakUserGetTokenEndpoint => $"{KeycloakEndpoint}/realms/{Realm}/protocol/openid-connect/token";
        public static string KeyCloakUserInfoEndpoint => $"{KeycloakEndpoint}/realms/{Realm}/protocol/openid-connect/userinfo";
        public static string KeycloakUserLogoutEndpoint => $"{KeycloakEndpoint}/realms/{Realm}/protocol/openid-connect/logout";
        public static string KeycloakUserSearchEndpoint => $"{KeycloakEndpoint}/admin/realms/{Realm}/users";
        #endregion


        #region Company
        public static string SearchCompany => $"{InventoryManagementApi}/api/company/search";
        public static string AllCompanyList => $"{InventoryManagementApi}/api/company/companyList";
        public static string PostCompany => $"{InventoryManagementApi}/api/company";
        public static string PutCompany => $"{InventoryManagementApi}/api/company";
        public static string DeleteCompany => $"{InventoryManagementApi}/api/company";
        #endregion


        #region Category
        public static string PostCategory => $"{InventoryManagementApi}/api/category";
        public static string GetCategory => $"{InventoryManagementApi}/api/category";
        public static string PutCategory => $"{InventoryManagementApi}/api/category";
        public static string DeleteCategory => $"{InventoryManagementApi}/api/category";
        #endregion

        #region Category sub
        public static string PostCategorySub => $"{InventoryManagementApi}/api/categorySub";
        public static string PutCategorySub => $"{InventoryManagementApi}/api/categorySub";
        public static string DeleteCategorySub => $"{InventoryManagementApi}/api/categorySub";
        #endregion


        #region Brand
        public static string GetBrand => $"{InventoryManagementApi}/api/brand";
        public static string PostBrand => $"{InventoryManagementApi}/api/brand";
        public static string PutBrand => $"{InventoryManagementApi}/api/brand";
        public static string DeleteBrand => $"{InventoryManagementApi}/api/brand";
        #endregion


        #region Model
        public static string PostModel => $"{InventoryManagementApi}/api/model";
        public static string PutModel => $"{InventoryManagementApi}/api/model";
        public static string DeleteModel => $"{InventoryManagementApi}/api/model";
        #endregion




        #region Product
        public static string PostProduct => $"{InventoryManagementApi}/api/product";
        public static string PutProduct => $"{InventoryManagementApi}/api/product";
        public static string GetProduct => $"{InventoryManagementApi}/api/product";
        public static string GetStoreProduct => $"{InventoryManagementApi}/api/product/GetStoreProduct";
        public static string PutProductTransfer => $"{InventoryManagementApi}/api/product/Transfer";
        public static string DeleteProduct => $"{InventoryManagementApi}/api/product";
        public static string GetByIdProductAndDetailsQuery => $"{InventoryManagementApi}/api/product/GetByIdProductAndDetailsQuery";
        public static string SearchProduct => $"{InventoryManagementApi}/api/product/search";
        public static string PostProductFileManagement => $"{InventoryManagementApi}/api/product/fileManagement";
        public static string FileTransferMovement => $"{InventoryManagementApi}/api/product/filemanagement";
        #endregion



        #region FileManager
        public static string SearchFileManager => $"{InventoryManagementApi}/api/FileManager/search"; //{filename}
        public static string UploadFileManager => $"{InventoryManagementApi}/api/FileManager/upload";
        public static string DownloadFileManager => $"{InventoryManagementApi}/api/FileManager/download";
        public static string DeleteFileManager => $"{InventoryManagementApi}/api/FileManager/delete";
        #endregion



        #region AssignedProduct
        public static string AssignedProductCreate => $"{InventoryManagementApi}/api/AssignedProduct/AssignedProductCreate";
        public static string AssignedProductUpdate => $"{InventoryManagementApi}/api/AssignedProduct/AssignedProductUpdate";
        public static string AssignedProductApproveReject => $"{InventoryManagementApi}/api/AssignedProduct/AssignedProductApproveReject";
        #endregion


        #region TransferOfficiers
        public static string PostTransferOfficier => $"{InventoryManagementApi}/api/TransferOfficier";
        public static string PutTransferOfficier => $"{InventoryManagementApi}/api/TransferOfficier";
        public static string DeleteTransferOfficier => $"{InventoryManagementApi}/api/TransferOfficier";
        public static string GetTransferOfficier => $"{InventoryManagementApi}/api/TransferOfficier";
        public static string GetTransferOfficierGetAll => $"{InventoryManagementApi}/api/TransferOfficier/GetAll";
        #endregion


        #region Reports
        public static string GeneralReport => $"{InventoryManagementApi}/api/Report/GeneralReport";
        #endregion
    }
}
