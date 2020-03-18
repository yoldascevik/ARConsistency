namespace ARConsistency
{
    public class ResponseOptions
    {
        public bool IsDebug { get; set; }
        public bool ShowStatusCode { get; set; } = true;
        public bool ShowApiVersion { get; set; }
        public string ApiVersion { get; set; } = "1.0.0.0";
        public bool IgnoreNullValue { get; set; } = true;
        public bool UseCamelCaseNaming { get; set; } = true;
        public bool EnableExceptionLogging { get; set; } = true;
    }
}
