using System.Diagnostics;
using System.Reflection.Metadata;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace wps.core
{
    public interface IActivity
    {
        Task ExecuteAsync();
        Task RollbackAsync();
    }
}

