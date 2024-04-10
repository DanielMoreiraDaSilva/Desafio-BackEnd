using System.Diagnostics.CodeAnalysis;

namespace Core.Models.Configurations
{
    [ExcludeFromCodeCoverage]
    public class AwsConfig
    {
        public string HostPath { get; set; }
        public string SQSUrl { get; set; }
        public string BucketName { get; set; }
    }
}
