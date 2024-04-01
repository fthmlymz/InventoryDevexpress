using DevExpress.Blazor.Reporting;
using DevExpress.XtraReports.UI;
using InventoryManagement.Frontend.Common;
using InventoryManagement.Frontend.Constants;
using InventoryManagement.Frontend.DTOs.Reports;
using InventoryManagement.Frontend.Services;
using InventoryManagement.Frontend.Services.Authorization;
using Microsoft.AspNetCore.Components;

namespace InventoryManagement.Frontend.Pages
{
    public partial class Index : ComponentBase
    {
        #region Inject
        [Inject] public ApiService? ApiService { get; set; }
        [Inject] IAuthorizationService? AuthorizationService { get; set; }

        #endregion

        #region Dialogs
        private bool ShowGeneralReportVisible { get; set; }
        #endregion

        #region Report
        DxReportViewer? _reportViewer;
        XtraReport _report = XtraReport.FromFile(ApplicationConstants.GenelEnvanterReport, true);
        #endregion

        private PaginatedResult<GeneralReportDto>? generalReportData { get; set; }
        private IEnumerable<AllProductReportItemDto>? genelRapor;
        private IEnumerable<CompanyProductReportItemDto>? genelRaporCompany;

        protected override async Task OnInitializedAsync()
        {
            await GeneralReport();
        }
        private async Task GeneralReport()
        {
            generalReportData = await ApiService!.GetAsync<PaginatedResult<GeneralReportDto>>($"{ApiEndpointConstants.GeneralReport}");
            genelRapor = (IEnumerable<AllProductReportItemDto>?)(generalReportData?.data?.FirstOrDefault()?.AllProductReport);
            genelRaporCompany = (IEnumerable<CompanyProductReportItemDto>?)(generalReportData?.data?.FirstOrDefault()?.CompanyProductReport);
            StateHasChanged();
        }
    }
}
