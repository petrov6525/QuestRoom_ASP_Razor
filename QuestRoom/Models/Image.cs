namespace QuestRoom.Models
{
    public class Image
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public int? RoomId { get; set; }
        public virtual Room Room { get; set; }
    }
}
