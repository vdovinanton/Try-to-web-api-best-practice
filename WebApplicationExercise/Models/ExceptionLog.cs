
namespace WebApplicationExercise.Models
{
    public class ExceptionLog
    {
        public class Request
        {
            public string Uri { get; set; }
            public string Methood { get; set; }
        }

        public string Type { get; set; }

        public Request RequestInfo { get; set; }

        public string Message { get; set; }

        public string CallStack { get; set; }

    }
}