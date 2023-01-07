namespace QuestRoom.Models
{
    public class Phone
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public int? RoomId { get; set; }
        public virtual Room Room { get; set; } 
    }
}
