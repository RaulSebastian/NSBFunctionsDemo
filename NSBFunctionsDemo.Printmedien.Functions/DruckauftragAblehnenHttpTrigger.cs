using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using NSBFunctionsDemo.Printmedien.Data.Repositories;
using NSBFunctionsDemo.Printmedien.Domain.Model;
using NServiceBus;
using Printmedien.Messages;

namespace NSBFunctionsDemo.Printmedien.Functions
{
    public class DruckauftragAblehnenHttpTrigger
    {
        private readonly IFunctionEndpoint _functionEndpoint;
        private readonly IPrintmedienRepository _repository;

        public DruckauftragAblehnenHttpTrigger(IFunctionEndpoint functionEndpoint, IPrintmedienRepository repository)
        {
            _functionEndpoint = functionEndpoint;
            _repository = repository;
        }

        [FunctionName("DruckauftragAblehnen")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "Druckauftrag/{id}/Ablehnen")]
            HttpRequest request,
            Guid id,
            ExecutionContext executionContext)
        {
            var druckauftrag = await _repository.GetById(id);
            if (druckauftrag == null)
            {
                return new NotFoundResult();
            }
            druckauftrag.Status = DruckauftragStatus.Abgelehnt;
            await _repository.Update(druckauftrag);

            var options = new PublishOptions();
            var druckauftragBestaetigt = new DruckauftragNichtBestaetigt
            {
                Id = id,
                WerbekampagneId = druckauftrag.WerbekampagneId
            };
            await _functionEndpoint.Publish(druckauftragBestaetigt, options, executionContext);
            return new OkResult();
        }
    }
}