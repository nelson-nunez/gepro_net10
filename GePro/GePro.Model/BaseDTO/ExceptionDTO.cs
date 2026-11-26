using System;


namespace GePro.Model.BaseDTO
{
    public class ExceptionDTO
    {
        public string Message { get; set; } = string.Empty;

        public string LogId { get; set; } = string.Empty;

        public string StackTrace { get; set; } = string.Empty;
    }
}