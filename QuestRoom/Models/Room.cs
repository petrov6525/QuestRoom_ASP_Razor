namespace QuestRoom.Models
{
    public class Room
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Time { get; set; }
        public int MinPLayers { get; set; }
        public int MaxPLayers { get; set; }
        public int MinAge { get; set; }
        public string Address { get; set; }

        public ICollection<Phone> Phones { get; set; }

        public ICollection<Image> Images { get; set; }

        public string Mail { get; set; }

        public string Company { get; set; }    

        public int Rating { get; set; }
        public int Scary { get; set; }
        public int Complexity { get; set; }

        public string LogoPath { get; set; }
        


        public Room()
        {
            Phones = new List<Phone>();
            Images=new List<Image>();
        }
    }
}
