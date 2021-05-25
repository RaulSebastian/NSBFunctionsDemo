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
    public class DruckauftragBestaetigenHttpTrigger
    {
        private readonly IFunctionEndpoint _functionEndpoint;
        private readonly IPrintmedienRepository _repository;

        public DruckauftragBestaetigenHttpTrigger(IFunctionEndpoint functionEndpoint, IPrintmedienRepository repository)
        {
            _functionEndpoint = functionEndpoint;
            _repository = repository;
        }

        [FunctionName("DruckauftragBestaetigen")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "Druckauftrag/{id}/Bestaetigen")] 
            HttpRequest request, 
            Guid id,
            ExecutionContext executionContext)
        {
            var druckauftrag = await _repository.GetById(id);

            if (druckauftrag == null)
            {
                return new NotFoundResult();
            }

            if (druckauftrag.Status == DruckauftragStatus.Bestaetigt)
            {
                return new BadRequestObjectResult("Druckauftrag wurde bereits bestätigt.");
            }

            druckauftrag.Status = DruckauftragStatus.Bestaetigt;
            await _repository.Update(druckauftrag);

            var options = new PublishOptions();
            var druckauftragBestaetigt = new DruckauftragBestaetigt
            {
                Id = id,
                WerbekampagneId = druckauftrag.WerbekampagneId
            };
            await _functionEndpoint.Publish(druckauftragBestaetigt, options, executionContext);
            return new OkResult();
        }
    }
}
