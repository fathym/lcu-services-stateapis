using Fathym;
using Fathym.Design;
using Fathym.LCU.Services.StateAPIs.StateServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Fathym.LCU.Services.StateAPIs.StateServices
{
    public interface IStateService
    {
        event EventHandler<HubConnectionStartedEventArgs> Started;

        event EventHandler<StateEventArgs> State;

        string URL { get; }

        Task Start();

        Task Stop();

        Task AttachState(string stateType, string stateKey);

        Task UnattachState(string stateType, string stateKey);
    }

    public class StateService : IStateService
    {
        #region Fields
        protected int retryCount;
        #endregion

        #region Properties
        public virtual HubConnection Hub { get; set; }

        public event EventHandler<HubConnectionStartedEventArgs> Started;

        public event EventHandler<StateEventArgs> State;

        public virtual HttpTransportType Transport { get; protected set; }

        public virtual string URL { get; protected set; }
        #endregion

        #region Constructors
        public StateService(string url, HttpTransportType transport)
        {
            Transport = transport;

            URL = url ?? throw new ArgumentNullException(nameof(url));

            retryCount = 0;
        }
        #endregion

        #region API Methods
        public virtual async Task Start()
        {
            await DesignOutline.Instance.Retry()
                .SetActionAsync(async () =>
                {
                    try
                    {
                        if (Hub == null)
                        {
                            Hub = connect(URL, Transport);

                            Hub.ServerTimeout = TimeSpan.FromMilliseconds(600000);

                            Hub.Closed += onClosed;

                            registerStartHandlers();
                        }

                        await Hub.StartAsync();

                        Started?.Invoke(this, new HubConnectionStartedEventArgs()
                        {
                            Started = DateTimeOffset.Now
                        });

                        return false;
                    }
                    catch (Exception ex)
                    {
                        Hub = null;

                        await handleStartError(ex);

                        return true;
                    }
                })
                .SetThrottle(1000)
                .SetThrottleScale(2)
                .SetCycles(10)
                .Run();
        }

        public virtual Task Stop()
        {
            return Hub?.StopAsync() ?? Task.CompletedTask;
        }

        public virtual Task AttachState(string stateType, string stateKey)
        {
            return Hub.InvokeAsync($"AttachState", stateType, stateKey);
        }

        public virtual Task UnattachState(string stateType, string stateKey)
        {
            return Hub.InvokeAsync($"UnattachState", stateType, stateKey);
        }
        #endregion

        #region Helpers
        protected virtual HubConnection connect(string url, HttpTransportType transport)
        {
            var bldr = createHubBuilder(url, transport);

            return bldr.Build();
        }

        protected virtual IHubConnectionBuilder createHubBuilder(string url, HttpTransportType transport)
        {
            return new HubConnectionBuilder()
                .WithUrl(new Uri(url), o =>
                {
                    o.Transports = transport;

                    //  TODO:  Where to get JWT
                    o.AccessTokenProvider = async () => Environment.GetEnvironmentVariable("TEMP_JWT");
                })
                .WithAutomaticReconnect();
        }

        protected virtual async Task handleStartError(Exception ex)
        { }

        protected virtual async Task onClosed(Exception ex)
        { }

        protected virtual void registerStartHandlers()
        {
            Hub.On<MetadataModel>("state-update", updateState);
        }

        protected virtual void updateState(MetadataModel state)
        {
            State?.Invoke(this, new StateEventArgs()
            {
                State = state
            });
        }
        #endregion
    }
}