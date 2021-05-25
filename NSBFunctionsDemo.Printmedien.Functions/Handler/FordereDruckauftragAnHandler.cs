using System;
using System.Threading.Tasks;
using NSBFunctionsDemo.Printmedien.Data.Repositories;
using NSBFunctionsDemo.Printmedien.Domain.Model;
using NServiceBus;
using Printmedien.Messages;

namespace NSBFunctionsDemo.Printmedien.Functions.Handler
{
    public class FordereDruckauftragAnHandler : IHandleMessages<FordereDruckauftragAn>
    {
        private readonly IPrintmedienRepository _repository;

        public FordereDruckauftragAnHandler(IPrintmedienRepository repository)
        {
            _repository = repository;
        }

        public async Task Handle(FordereDruckauftragAn message, IMessageHandlerContext context)
        {
            var existingAuftrag = await _repository.GetForWerbekampagneId(message.WerbekampagneId);

            if (existingAuftrag?.Status == DruckauftragStatus.Angefordert)
            {
                await context.Publish(new DruckauftragBereitsAngefordert
                {
                    Id = existingAuftrag.Id,
                    WerbekampagneId = message.WerbekampagneId
                });
                return;
            }
            
            var auftragId = Guid.NewGuid();
            if (existingAuftrag != null)
            {
                auftragId = existingAuftrag.Id;
                existingAuftrag.Titel = message.Titel;
                existingAuftrag.Inhalt = message.Inhalt;
                existingAuftrag.Deadline = message.Deadline;
                await _repository.Update(existingAuftrag);
            }
            else
            {
                await _repository.Create(new Druckauftrag
                {
                    Id = auftragId,
                    WerbekampagneId = message.WerbekampagneId,
                    Titel = message.Titel,
                    Inhalt = message.Inhalt,
                    Deadline = message.Deadline
                });
            }

            await context.Publish(
                new DruckauftragAngefordert
                    {Id = auftragId, WerbekampagneId = message.WerbekampagneId});
        }
    }
}