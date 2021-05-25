namespace NSBFunctionsDemo.Werbekampagne.Domain.Model
{
    public class Werbemedien
    {
        public Werbemedium Print { get; set; }

        public Werbemedium Webshop { get; set; }

        public Werbemedien()
        {
            Print = new Werbemedium();
            Webshop = new Werbemedium();
        }
    }
}