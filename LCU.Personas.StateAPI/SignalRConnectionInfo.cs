using Fathym;
using System.Runtime.Serialization;

namespace LCU.Personas.StateAPI
{
    [DataContract]
    public class SignalRConnectionInfo : MetadataModel
    {
        [DataMember]
        public virtual string Url { get; set; }

        [DataMember]
        public virtual string AccessToken { get; set; }
    }
}
