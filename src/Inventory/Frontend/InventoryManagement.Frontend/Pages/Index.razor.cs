using DevExpress.Blazor.Reporting;
using DevExpress.XtraReports.UI;
using InventoryManagement.Frontend.Constants;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace InventoryManagement.Frontend.Pages
{
    public partial class Index : ComponentBase
    {
        [Inject] public DialogService? _dialogService { get; set; }


        #region Report
        DxReportViewer? _reportViewer;
        XtraReport _report = XtraReport.FromFile(ApplicationConstants.GenelEnvanterReport, true);
        #endregion
    }
}
