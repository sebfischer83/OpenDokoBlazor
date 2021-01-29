using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using OpenDokoBlazor.Shared.ViewModels.Chat;
using Stl;
using Stl.DependencyInjection;
using Stl.Fusion;
using Stl.Fusion.Authentication;
using Stl.Fusion.Blazor;

namespace OpenDokoBlazor.Client.Services
{
    [Service(Lifetime = ServiceLifetime.Scoped)]
    public class ClientState : IDisposable
    {
        //protected AuthStateProvider AuthStateProvider { get; }
        protected ISessionResolver SessionResolver { get; }

        // Handy shortcuts
        public Session Session => SessionResolver.Session;
        //public ILiveState<AuthState> AuthState => AuthStateProvider.State;
        // Own properties
        //public ILiveState<User> User { get; }
        public IMutableState<ChatUser?> ChatUser { get; }

        public ClientState(IStateFactory stateFactory, ISessionResolver sessionResolver)
        {
            SessionResolver = sessionResolver;
            ChatUser = stateFactory.NewMutable(Result.Value<ChatUser?>(null));
        }

        void IDisposable.Dispose()
        { 
        }
    }
}
