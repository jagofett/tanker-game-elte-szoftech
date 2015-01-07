namespace Sztek.Models
{
    public class results
    {
        public int id { get; set; }
        public virtual games games { get; set; }
        public virtual users users { get; set; }
        /*
        public int frag { get; set; }
        public int death { get; set; }
        */
        public int score { get; set; }


    }
}