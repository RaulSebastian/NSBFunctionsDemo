using System;
using System.Net;
using Newtonsoft.Json;
using Stateless;

namespace NSBFunctionsDemo.Werbekampagne.Domain.Model
{
    public class Werbekampagne
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; private set; }
        public string Bezeichnung { get; private set; }
        public string Beschreibung { get; private set; }
        public DateTime? LaufzeitBeginn { get; private set; }
        public DateTime? LaufzeitEnde { get; private set; }
        public Kampagnenstatus Status { get; private set; }
        public Werbemedien Werbemedien { get; }

        private readonly StateMachine<Kampagnenstatus, Statuswechsel> _stateMachine;

        public Werbekampagne(
            string bezeichnung,
            string beschreibung,
            Guid? id = null,
            DateTime? laufzeitBeginn = null,
            DateTime? laufzeitEnde = null,
            Werbemedien werbemedien = null,
            Kampagnenstatus status = Kampagnenstatus.Entwurf)
        {
            Id = id ?? Guid.NewGuid();
            Bezeichnung = bezeichnung;
            Beschreibung = beschreibung;
            UpdateLaufzeit(laufzeitBeginn, laufzeitEnde);
            Werbemedien = werbemedien ?? new Werbemedien();
            Status = status;

            _stateMachine = new StateMachine<Kampagnenstatus, Statuswechsel>(
                () => Status, newStatus => Status = newStatus);

            _stateMachine.Configure(Kampagnenstatus.Entwurf)
                .Permit(Statuswechsel.Loeschen, Kampagnenstatus.Geloescht)
                .PermitIf(Statuswechsel.FreigabeAnfordern, Kampagnenstatus.ErwarteFreigabe, FreigabeAnfordernGuard);

            _stateMachine.Configure(Kampagnenstatus.ErwarteFreigabe)
                .OnEntry(WerbemedienFreigabeAnfordern)
                .Permit(Statuswechsel.FreigabeNichtErteilt, Kampagnenstatus.Entwurf)
                .Permit(Statuswechsel.Stornieren, Kampagnenstatus.Storniert)
                .PermitIf(Statuswechsel.Freigeben, Kampagnenstatus.Geplant, FreigebenGuard);

            _stateMachine.Configure(Kampagnenstatus.Geplant)
                .Permit(Statuswechsel.Stornieren, Kampagnenstatus.Storniert)
                .PermitIf(Statuswechsel.Starten, Kampagnenstatus.Gestartet, StartenGuard);

            _stateMachine.Configure(Kampagnenstatus.Gestartet)
                .Permit(Statuswechsel.Abbrechen, Kampagnenstatus.Abgebrochen)
                .PermitIf(Statuswechsel.Beenden, Kampagnenstatus.Beendet, BeendenGuard);

            _stateMachine.Configure(Kampagnenstatus.Storniert)
                .OnEntry(Storniert)
                .PermitIf(Statuswechsel.Loeschen, Kampagnenstatus.Geloescht, StornoLoeschenGuard);
        }

        public void RahmendatenAnpassen(
            string bezeichnung,
            string beschreibung,
            DateTime? laufzeitBeginn,
            DateTime? laufzeitEnde)
        {
            if (Status != Kampagnenstatus.Entwurf)
            {
                throw new InvalidOperationException("Kampagnendaten dürfen nur im Entwurf geändert werden.");
            }

            UpdateLaufzeit(laufzeitBeginn, laufzeitEnde);
            Bezeichnung = bezeichnung;
            Beschreibung = beschreibung;
        }

        public void WerbemedienAnpassen(bool beworbenAufPrintmedien, bool beworbenImWebshop)
        {
            if (Status != Kampagnenstatus.Entwurf)
            {
                throw new InvalidOperationException("Werbemedien dürfen nur im Entwurf geändert werden.");
            }

            if (Werbemedien.Print.WirdBeworben != beworbenAufPrintmedien)
            {
                Werbemedien.Print.WirdBeworben = beworbenAufPrintmedien;
                Werbemedien.Print.Freigabestatus = beworbenAufPrintmedien
                    ? Freigabestatus.Angefordert
                    : Freigabestatus.NichtAngefordert;
            }

            if (Werbemedien.Webshop.WirdBeworben == beworbenImWebshop)
            {
                return;
            }

            Werbemedien.Webshop.WirdBeworben = beworbenImWebshop;
            Werbemedien.Webshop.Freigabestatus = beworbenImWebshop
                ? Freigabestatus.Angefordert
                : Freigabestatus.NichtAngefordert;
        }

        public bool DarfWerbemedienAnpassen() => Status == Kampagnenstatus.Entwurf;

        public void FreigabeAnfordern()
        {
            _stateMachine.Fire(Statuswechsel.FreigabeAnfordern);
        }

        public void Freigeben()
        {
            _stateMachine.Fire(Statuswechsel.Freigeben);
        }

        public void Starten()
        {
            _stateMachine.Fire(Statuswechsel.Starten);
        }

        public void Beenden()
        {
            _stateMachine.Fire(Statuswechsel.Beenden);
        }

        public void Abbrechen()
        {
            _stateMachine.Fire(Statuswechsel.Abbrechen);
        }

        public void Stornieren()
        {
            _stateMachine.Fire(Statuswechsel.Stornieren);
        }

        public void Loeschen()
        {
            _stateMachine.Fire(Statuswechsel.Loeschen);
        }

        private void UpdateLaufzeit(DateTime? laufzeitBeginn, DateTime? laufzeitEnde)
        {
            if (Status != Kampagnenstatus.Entwurf)
            {
                throw new InvalidOperationException("Die Laufzeit darf nicht mehr geändert werden.");
            }

            if (laufzeitEnde != null && laufzeitBeginn != null && laufzeitBeginn >= laufzeitEnde)
            {
                throw new InvalidOperationException("Laufzeit Beginn darf nicht nach dem Ende liegen.");
            }

            LaufzeitBeginn = laufzeitBeginn;
            LaufzeitEnde = laufzeitEnde;
        }

        private void WerbemedienFreigabeAnfordern()
        {
            WerbemediumFreigabeAnfordern(Werbemedien.Webshop);
            WerbemediumFreigabeAnfordern(Werbemedien.Print);
        }

        private void Storniert()
        {
            if (Werbemedien.Webshop.WirdBeworben)
            {
                Werbemedien.Webshop.Freigabestatus = Freigabestatus.Storniert;
            }
            if (Werbemedien.Print.WirdBeworben)
            {
                Werbemedien.Print.Freigabestatus = Freigabestatus.Storniert;
            }
        }

        private bool FreigabeAnfordernGuard()
            => !string.IsNullOrEmpty(Bezeichnung)
               && !string.IsNullOrEmpty(Beschreibung)
               && LaufzeitBeginn != null
               && LaufzeitEnde != null;

        private bool FreigebenGuard()
            => (!Werbemedien.Webshop.WirdBeworben || Werbemedien.Webshop.Freigabestatus == Freigabestatus.Bestaetigt)
               && (!Werbemedien.Print.WirdBeworben || Werbemedien.Print.Freigabestatus == Freigabestatus.Bestaetigt);

        private bool StartenGuard()
            => LaufzeitBeginn != null
               && LaufzeitEnde != null
               && LaufzeitBeginn <= DateTime.UtcNow;

        private bool BeendenGuard() => LaufzeitEnde <= DateTime.UtcNow;

        private bool StornoLoeschenGuard()
            => (!Werbemedien.Webshop.WirdBeworben || Werbemedien.Webshop.Freigabestatus == Freigabestatus.Storniert)
               && (!Werbemedien.Print.WirdBeworben || Werbemedien.Print.Freigabestatus == Freigabestatus.Storniert);

        private static void WerbemediumFreigabeAnfordern(Werbemedium werbemedium)
        => werbemedium.Freigabestatus = werbemedium.WirdBeworben
            ? Freigabestatus.Angefordert
            : Freigabestatus.NichtAngefordert;
    }
}