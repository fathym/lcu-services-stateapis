using Microsoft.Azure.WebJobs.Host.Bindings;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LCU.Personas.StateAPI.StateServices
{
    public class StateServiceValueBinder : IValueBinder
    {
        private object value;

        public StateServiceValueBinder(object value)
        {
            this.value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public Type Type => typeof(string);

        public Task<object> GetValueAsync()
        {
            return Task.FromResult(value);
        }

        public Task SetValueAsync(object value, CancellationToken cancellationToken)
        {
            this.value = value;

            return Task.CompletedTask;
        }

        public string ToInvokeString()
        {
            return value?.ToString();
        }
    }
}