﻿namespace InventoryManagement.Frontend.Models
{
    public class ProductOperationsModel
    {
        public int Id { get; set; }


        #region Product Transfer
        public string? SenderUserName { get; set; }
        public string? SenderEmail { get; set; }


        public int? RecipientCompanyId { get; set; }
        public string? RecipientUserName { get; set; }
        public string? RecipientFullName { get; set; }
        public string? RecipientEmail { get; set; }
        public string? RecipientCompanyName { get; set; }
        public string? TypeOfOperations { get; set; }
        #endregion




        public string? UpdatedBy { get; set; }
        public string? UpdatedUserId { get; set; }
    }
}
