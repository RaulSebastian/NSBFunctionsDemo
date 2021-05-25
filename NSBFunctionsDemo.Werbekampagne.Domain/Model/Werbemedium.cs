namespace NSBFunctionsDemo.Werbekampagne.Domain.Model
{
    public class Werbemedium
    {
        public bool WirdBeworben { get; set; } = false;
        public Freigabestatus Freigabestatus { get; set; } = Freigabestatus.NichtAngefordert;
    }
}