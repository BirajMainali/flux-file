using System;

namespace FluxFile.Exceptions;

public class FluxFileUploadException : Exception
{
    public FluxFileUploadException(string message) : base(message)
    {
    }
}