using System;

namespace NSBFunctionsDemo.Werbekampagne.API.Models
{
    [ApiModel]
    public class WerbemedienFestlegen
    {
        public bool BeworbenAufPrintmedien { get; set; }
        public bool BeworbenImWebshop { get; set; }
    }
}