using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSBFunctionsDemo.Werbekampagne.API.Models;
using NSBFunctionsDemo.Werbekampagne.Data.Repositories;
using NSBFunctionsDemo.Werbekampagne.Domain.Model;
using NServiceBus;
using Werbekampagne.Messages;
using WerbekampagneModel = NSBFunctionsDemo.Werbekampagne.Domain.Model.Werbekampagne;

namespace NSBFunctionsDemo.Werbekampagne.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WerbekampagnenController : ControllerBase
    {
        private readonly ILogger<WerbekampagnenController> _logger;
        private readonly IMessageSession _messageSession;
        private readonly IWerbekampagnenRepository _repository;

        public WerbekampagnenController(
            IWerbekampagnenRepository repository,
            ILogger<WerbekampagnenController> logger, IMessageSession messageSession)
        {
            _logger = logger;
            _messageSession = messageSession;
            _repository = repository;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ISet<WerbekampagneModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var results = await _repository.GetAll();
            return Ok(results);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(WerbekampagneModel), StatusCodes.Status200OK)]
        public async Task<ActionResult> Get(Guid id)
        {
            var results = await _repository.GetById(id);
            return Ok(results);
        }

        [HttpPost]
        [ProducesResponseType(typeof(WerbekampagneModel), StatusCodes.Status200OK)]
        public async Task<ActionResult> Post([FromBody] NeueWerbekampagne neueWerbekampagne)
        {
            var id = Guid.NewGuid();
            var werbemedien = new Werbemedien
            {
                Print = new Werbemedium
                {
                    WirdBeworben = neueWerbekampagne.BeworbenAufPrintmedien
                },
                Webshop = new Werbemedium
                {
                    WirdBeworben = neueWerbekampagne.BeworbenImWebshop
                }
            };
            WerbekampagneModel werbekampagne;
            try
            {
                werbekampagne = new WerbekampagneModel(
                    neueWerbekampagne.Bezeichnung, neueWerbekampagne.Beschreibung, id,
                    neueWerbekampagne.LaufzeitBeginn, neueWerbekampagne.LaufzeitEnde, werbemedien);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            await _repository.Create(werbekampagne);
            await _messageSession.Publish(new WerbekampagneAngelegt
            {
                WerbekampagneId = id
            });
            return Ok(werbekampagne);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Put(Guid id, [FromBody] WerbekampagneRahmendaten kampagneRahmendaten)
        {
            var kampagne = await _repository.GetById(id);
            if (kampagne == null) return NotFound();

            try
            {
                kampagne.RahmendatenAnpassen(kampagneRahmendaten.Bezeichnung, kampagneRahmendaten.Beschreibung,
                    kampagneRahmendaten.LaufzeitBeginn, kampagneRahmendaten.LaufzeitEnde);
            }
            catch (InvalidOperationException e)
            {
                _logger.LogWarning(e, "Failed to update WerbekampagneData");
                return BadRequest(e.Message);
            }

            await _repository.Update(kampagne);
            await _messageSession.Publish(new WerbekampagnenDatenAktualisiert
            {
                WerbekampagneId = id,
                Bezeichnung = kampagneRahmendaten.Bezeichnung,
                Beschreibung = kampagneRahmendaten.Beschreibung,
                LaufzeitBeginn = kampagneRahmendaten.LaufzeitBeginn,
                LaufzeitEnde = kampagneRahmendaten.LaufzeitEnde
            });

            return Ok();
        }

        [HttpPut("{id}/Werbemedien")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateWerbemedien(Guid id,
            [FromBody] WerbemedienFestlegen werbekampagne)
        {
            var kampagne = await _repository.GetById(id);
            if (kampagne == null) return NotFound();

            if (kampagne.Werbemedien.Print.WirdBeworben == werbekampagne.BeworbenAufPrintmedien
                && kampagne.Werbemedien.Webshop.WirdBeworben == werbekampagne.BeworbenImWebshop)
                return Ok();

            if (!kampagne.DarfWerbemedienAnpassen()) return BadRequest();

            kampagne.WerbemedienAnpassen(werbekampagne.BeworbenAufPrintmedien, werbekampagne.BeworbenImWebshop);

            await _repository.Update(kampagne);
            await _messageSession.Publish(new WerbemedienAktualisiert
            {
                WerbekampagneId = id,
                BeworbenImWebshop = werbekampagne.BeworbenImWebshop,
                BeworbenAufPrintmedien = werbekampagne.BeworbenAufPrintmedien
            });

            return Ok();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Delete(Guid id)
        {
            var kampagne = await _repository.GetById(id);
            if (kampagne == null) return NotFound();

            try
            {
                kampagne.Loeschen();
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }

            await _repository.Delete(id);
            await _messageSession.Publish(new WerbekampagneGeloescht
            {
                WerbekampagneId = id
            });

            return Ok();
        }
    }
}