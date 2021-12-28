namespace Webbsäkerhet_upg2.Models
{
    public class SavedFile
    {
        public Guid Id { get; set; }    
        public string UntrustedName { get; set; }   
        public DateTime TimeStamp { get; set; }
        public long Size { get; set; }
        public byte[] Content { get; set; }

    }
}
