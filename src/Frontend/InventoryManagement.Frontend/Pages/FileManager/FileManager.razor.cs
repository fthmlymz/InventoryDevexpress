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


        private string selectedFolderType = ApplicationConstants.Folders.Zimmetler!;
        public static List<string> FolderTypes = ApplicationConstants.Folders.AllFolders;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private List<FileItemDto> files;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.


        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await SearchFiles();
                base.OnInitialized();
            }
        }



        private async Task OnFolderTypeChanged(ChangeEventArgs e)
        {
#pragma warning disable CS8601 // Possible null reference assignment.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            selectedFolderType = e.Value.ToString();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore CS8601 // Possible null reference assignment.
            await SearchFiles();
        }


        #region Upload
        ElementReference fileDropContainer;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        InputFile inputFile;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS0649 // Field 'FileManager._filePasteModule' is never assigned to, and will always have its default value null
        IJSObjectReference _filePasteModule;
#pragma warning restore CS0649 // Field 'FileManager._filePasteModule' is never assigned to, and will always have its default value null
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS0649 // Field 'FileManager._filePasteFunctionReference' is never assigned to, and will always have its default value null
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        IJSObjectReference _filePasteFunctionReference;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning restore CS0649 // Field 'FileManager._filePasteFunctionReference' is never assigned to, and will always have its default value null
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private string HoverClass;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private const int maxAllowedFiles = 1;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private string ErrorMessage;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
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
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                string newFileName = $"{_communicationService.GetSelectedProduct().Barcode}{fileExtension}";
#pragma warning restore CS8602 // Dereference of a possibly null reference.

                content.Add(fileContent, "file", newFileName);
                content.Add(new StringContent(selectedFolderType), "folderType");

#pragma warning disable CS8602 // Dereference of a possibly null reference.
                await _apiService.UploadFileAsync($"{ApiEndpointConstants.UploadFileManager}", content);
#pragma warning restore CS8602 // Dereference of a possibly null reference.

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
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var fileBytes = await _apiService.DownloadFileAsync(downloadUrl);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8604 // Possible null reference argument.
            await JSRuntime.InvokeVoidAsync("downloadFileFromBytes", newFileName, fileBytes);
#pragma warning restore CS8604 // Possible null reference argument.
        }
        #endregion


        #region Delete File
        private async Task DeleteFile(string fileName)
        {
#pragma warning disable CS8604 // Possible null reference argument.
            var confirmed = await JSRuntime.InvokeAsync<bool>("confirm", $"Bu dosyayı silmek istediğinizden emin misiniz: {fileName}?");
#pragma warning restore CS8604 // Possible null reference argument.
            if (confirmed)
            {
                var deleteUrl = $"{ApiEndpointConstants.DeleteFileManager}/{selectedFolderType}/{fileName}";
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                var success = await _apiService.DeleteFileAsync(deleteUrl);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                if (success)
                {
                    await SearchFiles();
                    await ProductMovementNotification("delete");
                }
            }
        }
        #endregion



        #region Search
        public async Task SearchFiles()
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8604 // Possible null reference argument.
            files = await _apiService.ListFilesAsync(_communicationService.GetSelectedProduct().Barcode.ToString());
#pragma warning restore CS8604 // Possible null reference argument.
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            await InvokeAsync(StateHasChanged);
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

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var fileInformation = new FileTransferDto
            {
                CreatedBy = ((KeycloakAuthenticationStateProvider)_authenticationStateProvider).UserInfo.PreferredUsername,
                CreatedUserId = ((KeycloakAuthenticationStateProvider)_authenticationStateProvider).UserInfo.Sub,
                ProductId = _communicationService.GetSelectedProduct().ProductId,
                Description = $"{((KeycloakAuthenticationStateProvider)_authenticationStateProvider)?.UserInfo?.PreferredUsername} tarafından, {_communicationService.GetSelectedProduct().Barcode.ToString()} barkodlu ürün için dosya {operationDescription} işlemi yapıldı"
            };
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

            #region FileManager Push
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            var result = await _apiService.PostAsync(ApiEndpointConstants.FileTransferMovement, fileInformation);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
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
