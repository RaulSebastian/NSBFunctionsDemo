namespace Printmedien.Messages
{
    public class DruckauftragZuKurzfristig : DruckauftragNichtBestaetigt
    {
        public DruckauftragZuKurzfristig()
        {
            Grund = "Druckauftrag ist zu kurzfristig.";
        }
    }
}