namespace InventoryManagement.Frontend.Constants
{
    public class ApiEndpointConstants
    {
        public static string? gatewayAddress { get; private set; }

        public static void Load(IConfiguration configuration)
        {
            gatewayAddress = configuration["GatewayAddress"];
        }

        #region Keycloak
        public static string KeyCloakUserTokenEndpoint => $"{gatewayAddress}/gateway/inventory/user/token";
        public static string KeyCloakUserInfoEndpoint => $"{gatewayAddress}/gateway/inventory/user/info";
        public static string KeyCloakUserLogoutEndpoint => $"{gatewayAddress}/gateway/inventory/user/logout";
        public static string KeyCloakUserSearchEndpoint => $"{gatewayAddress}/gateway/inventory/user/search";
        #endregion


        #region Company
        public static string CompanySearch => $"{gatewayAddress}/gateway/inventory/company/search";
        public static string CompanyGetPostPutDelete => $"{gatewayAddress}/gateway/inventory/company";
        public static string CompanyGetAllList => $"{gatewayAddress}/gateway/inventory/company/companylist";
        #endregion

        
        #region Category
        public static string CategoryGetPostPutDelete => $"{gatewayAddress}/gateway/inventory/category";
        public static string CategorySubGetPostPutDelete => $"{gatewayAddress}/gateway/inventory/categorySub";
        public static string CategoryAllList => $"{gatewayAddress}/gateway/inventory/category/CategoryAllList";
        #endregion


        #region Brand
        public static string BrandGetPostPutDelete => $"{gatewayAddress}/gateway/inventory/brand";
        public static string BrandAllList => $"{gatewayAddress}/gateway/inventory/brand/BrandAllList";
        public static string ModelGetPostPutDelete => $"{gatewayAddress}/gateway/inventory/model";
        #endregion




        #region Product
        public static string ProductGetPostPutDelete => $"{gatewayAddress}/gateway/inventory/product";
        //public static string ProductsAndAssignedProduct => $"{gatewayAddress}/gateway/inventory/product/ProductsAndAssignedProducts";
        public static string GetStoreProduct => $"{gatewayAddress}/gateway/inventory/product/GetStoreProduct";
        public static string GetByIdProductAndDetailsQuery => $"{gatewayAddress}/gateway/inventory/product/GetByIdProductAndDetailsQuery";
        public static string ProductSearch => $"{gatewayAddress}/gateway/inventory/product/search";
        public static string FileTransferMovement => $"{gatewayAddress}/gateway/inventory/product/filemanagement";
        #endregion


        #region AssignedProduct
        public static string AssignedProductCreate => $"{gatewayAddress}/gateway/inventory/AssignedProduct/AssignedProductCreate";
        public static string AssignedProductUpdate => $"{gatewayAddress}/gateway/inventory/AssignedProduct/AssignedProductUpdate";
        public static string AssignedProductApproveReject => $"{gatewayAddress}/gateway/inventory/AssignedProduct/AssignedProductApproveReject";
        #endregion


        #region TransferOfficiers
        public static string TransferOfficierCrud => $"{gatewayAddress}/gateway/inventory/TransferOfficier";
        public static string ProductTransfer => $"{gatewayAddress}/gateway/inventory/product/Transfer";
        #endregion


        #region Reports
        public static string GetProductCountsQuery => $"{gatewayAddress}/gateway/inventory/Report/GetProductCountsQuery";
        #endregion


        #region gRPC - FileTransfer
        public static string FileTransferManager => $"{gatewayAddress}/gateway/filemanager";
        #endregion
    }
}
