using System;
using System.Threading.Tasks;
using NSBFunctionsDemo.Werbekampagne.Data.Repositories;
using NSBFunctionsDemo.Werbekampagne.Domain.Model;
using NServiceBus;
using Printmedien.Messages;
using WebShop.Messages;
using Werbekampagne.Messages;
using Werbekampagne.Messages.Statuswechsel;

namespace NSBFunctionsDemo.Werbekampagne.API.WerbekampagnenManagement
{
    public class WerbekampagneLifecycleSaga : 
        Saga<WerbekampagneLifecycleSagaData>,
        IAmStartedByMessages<WerbekampagneAngelegt>,
        IHandleMessages<WerbekampagnenDatenAktualisiert>,
        IHandleMessages<WerbemedienAktualisiert>,
        IHandleMessages<FreigabeAnfordern>,
        IHandleMessages<DruckauftragNichtBestaetigt>,
        IHandleMessages<DruckauftragBestaetigt>,
        IHandleMessages<DruckauftragStorniert>,
        IHandleMessages<WerbungBestaetigt>,
        IHandleMessages<Stornieren>,
        IHandleMessages<Freigeben>,
        IHandleMessages<Starten>,
        IHandleMessages<Abbrechen>,
        IHandleMessages<Beenden>,
        IHandleMessages<WerbekampagneGeloescht>
    {
        private readonly IWerbekampagnenRepository _werbekampagnenRepository;

        public WerbekampagneLifecycleSaga(IWerbekampagnenRepository werbekampagnenRepository)
        {
            _werbekampagnenRepository = werbekampagnenRepository;
        }

        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<WerbekampagneLifecycleSagaData> mapper)
        => mapper.MapSaga(saga => saga.WerbekampagneId)
            .ToMessage<WerbekampagneAngelegt>(msg => msg.WerbekampagneId)
            .ToMessage<WerbekampagnenDatenAktualisiert>(msg => msg.WerbekampagneId)
            .ToMessage<WerbemedienAktualisiert>(msg => msg.WerbekampagneId)
            .ToMessage<FreigabeAnfordern>(msg => msg.WerbekampagneId)
            .ToMessage<DruckauftragNichtBestaetigt>(msg => msg.WerbekampagneId)
            .ToMessage<DruckauftragNichtBestaetigt>(msg => msg.WerbekampagneId)
            .ToMessage<DruckauftragBestaetigt>(msg => msg.WerbekampagneId)
            .ToMessage<DruckauftragStorniert>(msg => msg.WerbekampagneId)
            .ToMessage<WerbungBestaetigt>(msg => msg.WerbekampagneId)
            .ToMessage<Stornieren>(msg => msg.WerbekampagneId)
            .ToMessage<Freigeben>(msg => msg.WerbekampagneId)
            .ToMessage<Starten>(msg => msg.WerbekampagneId)
            .ToMessage<Abbrechen>(msg => msg.WerbekampagneId)
            .ToMessage<Beenden>(msg => msg.WerbekampagneId)
            .ToMessage<WerbekampagneGeloescht>(msg => msg.WerbekampagneId);

        public async Task Handle(WerbekampagneAngelegt message, IMessageHandlerContext context)
        {
            var werbeKampagne = await _werbekampagnenRepository.GetById(message.WerbekampagneId);
            Data.Werbekampagne = werbeKampagne;
        }

        public Task Handle(WerbekampagnenDatenAktualisiert message, IMessageHandlerContext context)
        {
            Data.Werbekampagne.RahmendatenAnpassen(message.Bezeichnung, message.Beschreibung,
                message.LaufzeitBeginn, message.LaufzeitEnde);
            return Task.CompletedTask;
        }

        public Task Handle(WerbemedienAktualisiert message, IMessageHandlerContext context)
        {
            Data.Werbekampagne.WerbemedienAnpassen(message.BeworbenAufPrintmedien, message.BeworbenImWebshop);
            return Task.CompletedTask;
        }

        public async Task Handle(FreigabeAnfordern message, IMessageHandlerContext context)
        {
            Data.Werbekampagne.FreigabeAnfordern();
            await _werbekampagnenRepository.Update(Data.Werbekampagne);

            if (Data.Werbekampagne.Werbemedien.Print.WirdBeworben)
            {
                await context.Send(new FordereDruckauftragAn
                {
                    WerbekampagneId = message.WerbekampagneId,
                    Titel = Data.Werbekampagne.Bezeichnung,
                    Inhalt = Data.Werbekampagne.Beschreibung + Environment.NewLine +
                             "LaufzeitBeginn: " + Data.Werbekampagne.LaufzeitBeginn + Environment.NewLine +
                             "LaufzeitEnde: " + Data.Werbekampagne.LaufzeitEnde,
                    Deadline = Data.Werbekampagne.LaufzeitBeginn
                });
            }

            if (Data.Werbekampagne.Werbemedien.Webshop.WirdBeworben)
            {
                await context.Send(new FordereWerbungAn
                {
                    WerbekampagneId = message.WerbekampagneId,
                    Bezeichnung = Data.Werbekampagne.Bezeichnung,
                    Beschreibung = Data.Werbekampagne.Beschreibung,
                    LaufzeitBeginn = Data.Werbekampagne.LaufzeitBeginn,
                    LaufzeitEnde = Data.Werbekampagne.LaufzeitEnde
                });
            }

            await context.Publish(new WerbekampagneFordertFreigabeAn
            {
                WerbekampagneId = message.WerbekampagneId
            });
        }

        public async Task Handle(DruckauftragNichtBestaetigt message, IMessageHandlerContext context)
        {
            Data.Werbekampagne.Werbemedien.Print.Freigabestatus = Freigabestatus.NichtErteilt;
            await _werbekampagnenRepository.Update(Data.Werbekampagne);
        }

        public async Task Handle(DruckauftragBestaetigt message, IMessageHandlerContext context)
        {
            Data.Werbekampagne.Werbemedien.Print.Freigabestatus = Freigabestatus.Bestaetigt;
            await _werbekampagnenRepository.Update(Data.Werbekampagne);
        }

        public async Task Handle(DruckauftragStorniert message, IMessageHandlerContext context)
        {
            Data.Werbekampagne.Werbemedien.Print.Freigabestatus = Freigabestatus.Storniert;
            await _werbekampagnenRepository.Update(Data.Werbekampagne);
        }

        public async Task Handle(WerbungBestaetigt message, IMessageHandlerContext context)
        {
            Data.Werbekampagne.Werbemedien.Webshop.Freigabestatus = Freigabestatus.Bestaetigt;
            await _werbekampagnenRepository.Update(Data.Werbekampagne);
        }

        public async Task Handle(Stornieren message, IMessageHandlerContext context)
        {

            Data.Werbekampagne.Stornieren();
            await _werbekampagnenRepository.Update(Data.Werbekampagne);

            if (Data.Werbekampagne.Werbemedien.Print.WirdBeworben)
            {
                await context.Send(new StorniereDruckauftrag
                {
                    WerbekampagneId = message.WerbekampagneId
                });
            }

            if (Data.Werbekampagne.Werbemedien.Webshop.WirdBeworben)
            {
                await context.Send(new StorniereWerbung
                {
                    WerbekampagneId = message.WerbekampagneId
                });
            }

            await context.Publish(new WerbekampagneStorniert
            {
                WerbekampagneId = message.WerbekampagneId
            });
        }

        public async Task Handle(Freigeben message, IMessageHandlerContext context)
        {
            Data.Werbekampagne.Freigeben();
            await _werbekampagnenRepository.Update(Data.Werbekampagne);

            await context.Publish(new WerbekampagneFreigegeben
            {
                WerbekampagneId = message.WerbekampagneId
            });
        }

        public async Task Handle(Starten message, IMessageHandlerContext context)
        {
            Data.Werbekampagne.Starten();
            await _werbekampagnenRepository.Update(Data.Werbekampagne);

            await context.Publish(new WerbekampagneGestartet
            {
                WerbekampagneId = message.WerbekampagneId
            });
        }

        public async Task Handle(Abbrechen message, IMessageHandlerContext context)
        {
            Data.Werbekampagne.Abbrechen();
            await _werbekampagnenRepository.Update(Data.Werbekampagne);

            await context.Publish(new WerbekampagneAbgebrochen
            {
                WerbekampagneId = message.WerbekampagneId
            });
        }

        public async Task Handle(Beenden message, IMessageHandlerContext context)
        {
            Data.Werbekampagne.Beenden();
            await _werbekampagnenRepository.Update(Data.Werbekampagne);

            await context.Publish(new WerbekampagneBeendet
            {
                WerbekampagneId = message.WerbekampagneId
            });
        }

        public Task Handle(WerbekampagneGeloescht message, IMessageHandlerContext context)
        {
            MarkAsComplete();
            return Task.CompletedTask;
        }
    }
}
