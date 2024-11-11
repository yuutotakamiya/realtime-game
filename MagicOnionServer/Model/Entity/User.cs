namespace MagicOnionServer.Model.Entity
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Token { get; set; }

        public DateTime Created_at { get; set; }
        public DateTime Updated_at { get; set; }
    }
}
