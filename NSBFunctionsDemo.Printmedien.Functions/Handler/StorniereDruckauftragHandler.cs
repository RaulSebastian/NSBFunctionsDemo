using System.Threading.Tasks;
using NSBFunctionsDemo.Printmedien.Data.Repositories;
using NSBFunctionsDemo.Printmedien.Domain.Model;
using NServiceBus;
using Printmedien.Messages;

namespace NSBFunctionsDemo.Printmedien.Functions.Handler
{
    public class StorniereDruckauftragHandler : IHandleMessages<StorniereDruckauftrag>
    {
        private readonly IPrintmedienRepository _repository;

        public StorniereDruckauftragHandler(IPrintmedienRepository repository)
        {
            _repository = repository;
        }
        
        public async Task Handle(StorniereDruckauftrag message, IMessageHandlerContext context)
        {
            var druckauftrag = await _repository.GetForWerbekampagneId(message.WerbekampagneId);

            if (druckauftrag == null)
            {
                return;
            }
            
            druckauftrag.Status = DruckauftragStatus.Storniert;
            await _repository.Update(druckauftrag);
        }
    }
}