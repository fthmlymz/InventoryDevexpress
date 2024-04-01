using Application.Features.Products.Commands.ProductOperations;
using DotNetCore.CAP;
using Mapster;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace Application.Workflows.Product
{
    public class ProductTransferWorkflow : IWorkflow<UpdateProductOperationsCommand>
    {
        private readonly ICapPublisher _capPublisher;

        public ProductTransferWorkflow(ICapPublisher capPublisher)
        {
            _capPublisher = capPublisher;
        }


        public string Id => "ProductTransferWorkflow";
        public int Version => 1;


        public void Build(IWorkflowBuilder<UpdateProductOperationsCommand> builder)
        {
            var productTransfer = builder
               .Then<ProductTransferStep>()
                           .Input(step => step.UserName, data => "ürün transfer bildirimi, transfer")
                           .Then(context =>
                           {
                               var productTransferDto = context.Workflow.Data.Adapt<UpdateProductOperationsCommand>();
                               productTransferDto.TypeOfOperations = "Transfer";
                               _capPublisher.PublishAsync("product.transfer.transaction", productTransferDto);
                           });


            var rejectionNotificationProcess = builder
                .Then<NotifyUserRejection>()
                            .Input(step => step.UserName, data => "Ürün rejection bildirimi")
                             .Then(context =>
                             {
                                 var productTransferDto = context.Workflow.Data.Adapt<UpdateProductOperationsCommand>();
                                 productTransferDto.TypeOfOperations = "Rejected";
                                 _capPublisher.PublishAsync("product.transfer.transaction", productTransferDto);
                             });

            var returnIt = builder
                .Then<ReturnItStep>()
                            .Input(step => step.UserName, data => "Ürün returnIt bildirimi, return it edildi")
                             .Then(context =>
                             {
                                 var productTransferDto = context.Workflow.Data.Adapt<UpdateProductOperationsCommand>();
                                 productTransferDto.TypeOfOperations = "ReturnIt";
                                 _capPublisher.PublishAsync("product.transfer.transaction", productTransferDto);
                             });



            var approvalDecisionProcess = builder
                .WaitFor("Inventory_Approval_Decision", (data, context) => context.Workflow.Id, date => DateTime.Now.ToUniversalTime())
                    .Output(data => data.TypeOfOperations, step => step.EventData);

            builder
                .StartWith(context => ExecutionResult.Next())
                .Then(approvalDecisionProcess)
                .While(data => data.TypeOfOperations == "Transfer")
                    .Do(x => x
                        .Then(productTransfer)
                        .Then(approvalDecisionProcess)
                        )
                .While(data => data.TypeOfOperations == "Rejected")
                    .Do(x => x
                        .Then(rejectionNotificationProcess)
                        .Then(approvalDecisionProcess)
                        )
               .While(data => data.TypeOfOperations == "ReturnIt")
                    .Do(x => x
                        .Then(returnIt)
                        .Then(approvalDecisionProcess)
                        )
                .Then<PrintMessage>()
                    .Input(step => step.Message, data => "Akış tamamlandı, sonraki iş için bekliyorum.");
        }

        public class InitializeStep : StepBody
        {
            public string? UserName { get; set; }

            public override ExecutionResult Run(IStepExecutionContext context)
            {
                Console.WriteLine($"InitializeStep çalıştı: ");
                return ExecutionResult.Next();
            }
        }
        public class ProductTransferStep : StepBody
        {
            public string? UserName { get; set; }
            public override ExecutionResult Run(IStepExecutionContext context)
            {
                Console.WriteLine($"Product transfer step çalıştı: {UserName}");
                return ExecutionResult.Next();
            }
        }
        public class NotifyUserRejection : StepBody
        {
            public string? UserName { get; set; }
            public override ExecutionResult Run(IStepExecutionContext context)
            {
                Console.WriteLine($"NotifyUserRejection step çalıştı: {UserName}");
                return ExecutionResult.Next();
            }
        }
        public class PrintMessage : StepBody
        {
            private static bool hasPrinted = false;
            public string? Message { get; set; }
            public override ExecutionResult Run(IStepExecutionContext context)
            {
                if (!hasPrinted)
                {
                    Console.WriteLine($"PrintMessage step çalıştı: {Message}");
                    hasPrinted = true;
                }
                return ExecutionResult.Next();
            }
        }
        public class ReturnItStep : StepBody
        {
            public string? UserName { get; set; }
            public override ExecutionResult Run(IStepExecutionContext context)
            {
                Console.WriteLine($"ReturnIt step çalıştı: {UserName}");
                return ExecutionResult.Next();
            }
        }
    }
}
