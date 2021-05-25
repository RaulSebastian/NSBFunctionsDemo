using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSBFunctionsDemo.Werbekampagne.Data.Repositories;
using NServiceBus;
using Printmedien.Messages;
using WebShop.Messages;
using Werbekampagne.Messages.Statuswechsel;
using WerbekampagneModel = NSBFunctionsDemo.Werbekampagne.Domain.Model.Werbekampagne;

namespace NSBFunctionsDemo.Werbekampagne.API.Controllers
{
    [ApiController]
    [Route("Werbekampagnen")]
    public class StatusController : ControllerBase
    {
        private readonly IMessageSession _messageSession;
        private readonly IWerbekampagnenRepository _repository;

        public StatusController(
            IWerbekampagnenRepository repository,
            IMessageSession messageSession)
        {
            _messageSession = messageSession;
            _repository = repository;
        }

        [HttpPost("{id}/Status/FreigabeAnfordern")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        public async Task<ActionResult> FreigabeAnfordern(Guid id)
        {
            var kampagne = await _repository.GetById(id);
            if (kampagne == null) return NotFound();

            try
            {
                kampagne.FreigabeAnfordern();
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }

            await _messageSession.Send(new FreigabeAnfordern
            {
                WerbekampagneId = id
            });

            return Accepted();
        }

        [HttpPost("{id}/Status/Freigeben")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        public async Task<ActionResult> Freigeben(Guid id)
        {
            var kampagne = await _repository.GetById(id);
            if (kampagne == null) return NotFound();

            try
            {
                kampagne.Freigeben();
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }

            await _messageSession.Send(new Freigeben
            {
                WerbekampagneId = id
            });

            return Accepted();
        }

        [HttpPost("{id}/Status/Stornieren")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        public async Task<ActionResult> Stornieren(Guid id)
        {
            var kampagne = await _repository.GetById(id);
            if (kampagne == null) return NotFound();

            try
            {
                kampagne.Stornieren();
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }

            await _messageSession.Send(new Stornieren
            {
                WerbekampagneId = id
            });

            return Accepted();
        }


        [HttpPost("{id}/Status/Starten")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        public async Task<ActionResult> Starten(Guid id)
        {
            var kampagne = await _repository.GetById(id);
            if (kampagne == null) return NotFound();

            try
            {
                kampagne.Starten();
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }

            await _messageSession.Send(new Starten
            {
                WerbekampagneId = id
            });

            return Accepted();
        }

        [HttpPost("{id}/Status/Abbrechen")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        public async Task<ActionResult> Abbrechen(Guid id)
        {
            var kampagne = await _repository.GetById(id);
            if (kampagne == null) return NotFound();

            try
            {
                kampagne.Abbrechen();
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }

            await _messageSession.Send(new Abbrechen
            {
                WerbekampagneId = id
            });

            return Accepted();
        }

        [HttpPost("{id}/Status/Beenden")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        public async Task<ActionResult> Beenden(Guid id)
        {
            var kampagne = await _repository.GetById(id);
            if (kampagne == null) return NotFound();

            try
            {
                kampagne.Beenden();
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }

            await _messageSession.Send(new Beenden
            {
                WerbekampagneId = id
            });

            return Accepted();
        }
    }
}