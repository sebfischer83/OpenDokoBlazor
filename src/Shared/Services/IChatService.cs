using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenDokoBlazor.Shared.ViewModels.Chat;
using Stl.Async;
using Stl.Fusion;

namespace OpenDokoBlazor.Shared.Services
{
    public interface IChatService
    {
        // Writers
        Task<ChatUser> CreateUserAsync(string name, CancellationToken cancellationToken = default);
        Task<ChatUser> SetUserNameAsync(long id, string name, CancellationToken cancellationToken = default);
        Task<ChatMessage> AddMessageAsync(long userId, string text, CancellationToken cancellationToken = default);

        // Readers
        [ComputeMethod(KeepAliveTime = 10)]
        Task<long> GetUserCountAsync(CancellationToken cancellationToken = default);
        [ComputeMethod(KeepAliveTime = 10)]
        Task<long> GetActiveUserCountAsync(CancellationToken cancellationToken = default);
        [ComputeMethod(KeepAliveTime = 1)]
        Task<ChatUser> GetUserAsync(long id, CancellationToken cancellationToken = default);
        [ComputeMethod(KeepAliveTime = 10)]
        Task<ChatPage> GetChatTailAsync(int length, CancellationToken cancellationToken = default);
        [ComputeMethod(KeepAliveTime = 1)]
        Task<ChatPage> GetChatPageAsync(long minMessageId, long maxMessageId, CancellationToken cancellationToken = default);
    }
}
