namespace Sztek.Models
{
    public class results
    {
        public int id { get; set; }
        public virtual games game { get; set; }
        public virtual users user { get; set; }
        /*
        public int frag { get; set; }
        public int death { get; set; }
        */
        public int score { get; set; }


    }
}