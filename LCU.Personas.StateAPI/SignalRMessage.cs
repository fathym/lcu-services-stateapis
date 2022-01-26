using Fathym;
using System.Runtime.Serialization;

namespace LCU.Personas.StateAPI
{
    [DataContract]
    public class SignalRMessage : MetadataModel
    {
        [DataMember]
        public virtual object[] Arguments { get; set; }

        [DataMember]
        public virtual string Target { get; set; }
    }
}
