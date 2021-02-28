namespace KeyManager.Database
{
    class DatabaseConfig
    {
        public string ConnectionString
        {
            get
            {
                return $"Data Source={Filename}";
            }
        }
        public string Filename { get; set; }
        public string Encryption { get; set; }
    }
}