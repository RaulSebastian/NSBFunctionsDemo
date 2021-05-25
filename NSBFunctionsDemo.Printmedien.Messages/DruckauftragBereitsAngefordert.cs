namespace Printmedien.Messages
{
    public class DruckauftragBereitsAngefordert : DruckauftragNichtBestaetigt
    {
        public DruckauftragBereitsAngefordert()
        {
            Grund = "Ein Druckauftrag zu dieser Werbekampagne ist bereits in Bearbeitung.";
        }
    }
}