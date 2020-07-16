namespace ARConsistency.Configuration
{
    public class ArcConfiguration
    {
        public ArcConfiguration()
        {
            ResponseOptions = new ResponseOptions();
            ExceptionStatusCodeHandler = new ExceptionStatusCodeHandler();
        }
        
        public ResponseOptions ResponseOptions { get; set; }
        public ExceptionStatusCodeHandler ExceptionStatusCodeHandler { get; set; }
    }
}