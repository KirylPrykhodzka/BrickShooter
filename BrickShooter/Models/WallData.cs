namespace BrickShooter.Models
{
    public record WallData
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width {  get; set; }
        public int Height { get; set; }
        public float Rotation { get; set; }
        public string TextureName { get; set; }
    }
}
