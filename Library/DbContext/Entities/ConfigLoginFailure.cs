namespace Library.DbContext.Entities
{
    // ConfigLoginFailure
    
    public partial class ConfigLoginFailure
    {
        public int Id { get; set; } // Id (Primary key)
        public byte MaximumLoginFailure { get; set; } // MaximumLoginFailure
        public short LockDuration { get; set; } // LockDuration
        public byte LoginFailureInterval { get; set; } // LoginFailureInterval

        public ConfigLoginFailure()
        {
            InitializePartial();
        }

        partial void InitializePartial();
    }

}
