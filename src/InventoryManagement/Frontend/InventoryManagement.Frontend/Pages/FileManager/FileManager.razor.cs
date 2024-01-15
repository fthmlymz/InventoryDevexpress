using InventoryManagement.Frontend.Constants;
using InventoryManagement.Frontend.DTOs.FileManager;
using InventoryManagement.Frontend.Services;
using InventoryManagement.Frontend.Services.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace InventoryManagement.Frontend.Pages.FileManager
{
    public partial class FileManager : ComponentBase
    {
        #region Inject
        [Inject] public CommunicationService? _communicationService { get; set; }
        [Inject] public ApiService? _apiService { get; set; }
        [Inject] public IAuthorizationService? _authorizationService { get; set; }
        [Inject] public AuthenticationStateProvider? _authenticationStateProvider { get; set; }
        [Inject] public IJSRuntime? JSRuntime { get; set; }
        #endregion

        #region FileManager
        [CascadingParameter(Name = "ShowDeleteButtonForFileManager")]
        public bool ShowDeleteButton { get; set; } = true;
        [CascadingParameter(Name = "ShowUploadAreaForFileManager")]
        public bool ShowUploadArea { get; set; } = true;
        [CascadingParameter(Name = "ExcludeHistoryFilesForFileManager")]
        public bool ExcludeHistoryFiles { get; set; } = false;
        #endregion



        private string selectedFolderType = "Zimmetler";
        private List<string> folderTypes = new List<string> { "Zimmetler", "Tutanaklar", "Diğer" };
        private List<FileItemDto> files;


        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await SearchFiles();
            }
        }

        private async Task OnFolderTypeChanged(ChangeEventArgs e)
        {
            selectedFolderType = e.Value.ToString();
            await SearchFiles();
        }


        #region Upload
        ElementReference fileDropContainer;
        InputFile inputFile;
        IJSObjectReference _filePasteModule;
        IJSObjectReference _filePasteFunctionReference;
        private string HoverClass;
        private const int maxAllowedFiles = 1;
        private string ErrorMessage;
        bool uploadInProgress = false;
        int progressValue = 0;
        void OnDragEnter(DragEventArgs e) => HoverClass = "hover";
        void OnDragLeave(DragEventArgs e) => HoverClass = string.Empty;
        async Task OnChange(InputFileChangeEventArgs e)
        {
            ErrorMessage = string.Empty;

            if (e.FileCount > maxAllowedFiles)
            {
                ErrorMessage = $"Sadece {maxAllowedFiles} dosya yüklenebilir";
                return;
            }

            uploadInProgress = true;
            progressValue = 0;

            foreach (var file in e.GetMultipleFiles(maxAllowedFiles))
            {
                var content = new MultipartFormDataContent();
                var fileContent = new StreamContent(file.OpenReadStream(maxAllowedSize: 15 * 1024 * 1024)); // 15MB maksimum boyut
                string fileExtension = Path.GetExtension(file.Name);
                string newFileName = $"{_communicationService.GetSelectedProduct().Barcode}{fileExtension}";

                content.Add(fileContent, "file", newFileName);
                content.Add(new StringContent(selectedFolderType), "folderType");

                await _apiService.UploadFileAsync($"{ApiEndpointConstants.UploadFileManager}", content);

                progressValue += (int)(100 / e.FileCount); // Basit ilerleme hesaplama
            }

            uploadInProgress = false;
            HoverClass = string.Empty;

            await ProductMovementNotification("upload");
            await SearchFiles();
            await DisposeAsync();
        }
        public async ValueTask DisposeAsync()
        {
            if (_filePasteFunctionReference != null)
            {
                await _filePasteFunctionReference.InvokeVoidAsync("dispose");
                await _filePasteFunctionReference.DisposeAsync();
            }
            if (_filePasteModule != null)
            {
                await _filePasteModule.DisposeAsync();
            }
        }
        #endregion



        #region Download
        private async Task DownloadFile(string folderName, string fileName)
        {
            string extension = Path.GetExtension(fileName);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            string newFileName = fileNameWithoutExtension + extension;

            var downloadUrl = $"{ApiEndpointConstants.DownloadFileManager}/{folderName}/{fileNameWithoutExtension}";
            var fileBytes = await _apiService.DownloadFileAsync(downloadUrl);
            //"http://localhost:4002/api/FileManager/download/Zimmetler/80 - 08.01.2024"
            await JSRuntime.InvokeVoidAsync("downloadFileFromBytes", newFileName, fileBytes);
        }
        #endregion



        #region Delete File
        private async Task DeleteFile(string fileName)
        {
            var confirmed = await JSRuntime.InvokeAsync<bool>("confirm", $"Bu dosyayı silmek istediğinizden emin misiniz: {fileName}?");
            if (confirmed)
            {
                var deleteUrl = $"{ApiEndpointConstants.DeleteFileManager}/{selectedFolderType}/{fileName}";
                var success = await _apiService.DeleteFileAsync(deleteUrl);
                if (success)
                {
                    await SearchFiles();
                    await ProductMovementNotification("delete");
                }
            }
        }
        #endregion


        #region Search
        private async Task SearchFiles()
        {
            files = await _apiService.ListFilesAsync(_communicationService.GetSelectedProduct().Barcode.ToString());
            StateHasChanged();
        }
        #endregion


        private async Task ProductMovementNotification(string type)
        {
            string operationDescription = type switch
            {
                "delete" => "silme",
                "upload" => "dosya yükleme",
                _ => "dosya işleme" // default
            };

            var fileInformation = new FileTransferDto
            {
                CreatedBy = ((KeycloakAuthenticationStateProvider)_authenticationStateProvider).UserInfo.PreferredUsername,
                CreatedUserId = ((KeycloakAuthenticationStateProvider)_authenticationStateProvider).UserInfo.Sub,
                ProductId = _communicationService.GetSelectedProduct().ProductId,
                Description = $"{((KeycloakAuthenticationStateProvider)_authenticationStateProvider)?.UserInfo?.PreferredUsername} tarafından, {_communicationService.GetSelectedProduct().Barcode.ToString()} barkodlu ürün için dosya {operationDescription} işlemi yapıldı"
            };

            #region FileManager Push
            var result = await _apiService.PostAsync(ApiEndpointConstants.FileTransferMovement, fileInformation);
            #endregion
        }

    }


    public class FileTransferDto
    {
        public int? ProductId { get; set; }
        public string? Description { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedUserId { get; set; }
    }
}
