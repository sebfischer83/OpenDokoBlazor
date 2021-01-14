using System;
using System.Net.Http;
using OpenDokoBlazor.Shared;
using OpenDokoBlazor.Shared.Services;
using OpenDokoBlazor.Client.Services;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using OpenDokoBlazor.Shared.ViewModels.Chat;
using Stl.Fusion;
using Stl.Fusion.Blazor;
using Stl.Reflection;

#pragma warning disable 8618

namespace OpenDokoBlazor.Client.Components.Chat
{
    [Authorize]
    public partial class ChatList
    {
        [Inject]
        public HttpClient Http { get; set; }
        [Inject]
        public IChatService ChatService { get; set; }
        [Inject]
        public ClientState ClientState { get; set; }
        [Inject]
        public NavigationManager Navigator { get; set; }

        public class LocalsModel
        {
            public string Name { get; set; } = "";
            public string Message { get; set; } = "";
            public Exception Error { get; set; }
        }

        public class Model
        {
            public long UserCount { get; set; } = 0;
            public long ActiveUserCount { get; set; } = 0;
            public ChatPage LastPage { get; set; } = new ChatPage();
        }

        private bool HasError => Locals.Value.Error != null;

        private ChatUser ChatUser
        {
            get => ClientState.ChatUser.Value;
            set => ClientState.ChatUser.Value = value;
        }

        protected override async Task OnInitializedAsync()
        {
            if (ReferenceEquals(this.State, null))
                return;
            if (ChatUser == null)
                await SetNameAsync();
            else
                ResetName();
        }

        protected override async Task<Model> ComputeStateAsync(CancellationToken cancellationToken)
        {
            var userCount = await ChatService.GetUserCountAsync(cancellationToken);
            var activeUserCount = await ChatService.GetActiveUserCountAsync(cancellationToken);
            var lastPage = await ChatService.GetChatTailAsync(30, cancellationToken);
            return new Model()
            {
                UserCount = userCount,
                ActiveUserCount = activeUserCount,
                LastPage = lastPage,
            };
        }

        protected override void OnInitialized()
        {
            StateHasChangedTriggers = StateEventKind.All;
            base.OnInitialized();
        }

        private async Task SetNameAsync()
        {
            ResetError();
            var locals = Locals.Value;
            try
            {
                if (ChatUser != null)
                {
                    try
                    {
                        ChatUser = await ChatService.SetUserNameAsync(ChatUser.Id, locals.Name);
                        return;
                    }
                    catch
                    {
                        // Prob. the user doesn't exist (server restarted), so we should try to recreate it.
                    }
                }
                ChatUser = await ChatService.CreateUserAsync(locals.Name);
            }
            catch (Exception e)
            {
                SetError(e);
            }
            finally
            {
                if (!HasError)
                    ResetName();
            }
        }

        private async Task SendMessageAsync()
        {
            ResetError();
            try
            {
                var locals = Locals.Value;
                if (ChatUser == null)
                    throw new ApplicationException("Please set your name first.");
                await ChatService.AddMessageAsync(ChatUser.Id, locals.Message);
                UpdateLocals(l => l.Message = "");
            }
            catch (Exception e)
            {
                SetError(e);
            }
        }

        private void ResetError()
            => SetError(null);

        private void ResetName()
        {
            var chatUser = ClientState.ChatUser.Value;
            UpdateLocals(l => l.Name = "test" ?? "");
        }

        private void SetError(Exception error)
            => UpdateLocals(l => l.Error = error);

        private void UpdateLocals(Action<LocalsModel> updater)
        {
            var clone = MemberwiseCloner.Invoke(Locals.Value);
            updater.Invoke(clone);
            Locals.Value = clone;
        }
    }
}
