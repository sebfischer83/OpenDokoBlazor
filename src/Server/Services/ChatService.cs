using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using OpenDokoBlazor.Server.Data;
using OpenDokoBlazor.Server.Helper;
using OpenDokoBlazor.Shared.Services;
using OpenDokoBlazor.Shared.ViewModels.Chat;
using Stl.Async;
using Stl.Fusion;
using Stl.Fusion.Authentication;
using Stl.Fusion.Bridge;

namespace OpenDokoBlazor.Server.Services
{
    [ComputeService(typeof(IChatService))]
    public class ChatService : DbServiceBase<InMemoryContext>, IChatService
    {
        private readonly ILogger _log;
        private readonly IPublisher _publisher;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ChatService(
            IPublisher publisher,
            IServiceProvider services,
            IHttpContextAccessor httpContextAccessor,
            ILogger<ChatService> log = null)
            : base(services)
        {
            _log = log ??= NullLogger<ChatService>.Instance;
            _publisher = publisher;
            _httpContextAccessor = httpContextAccessor;
        }

        // Writers

        public async Task<ChatUser> CreateUserAsync(string name, CancellationToken cancellationToken = default)
        {
            await using var dbContext = CreateDbContext();

            var user = new ChatUser() { Name = name };
            await dbContext.ChatUsers.AddAsync(user, cancellationToken).ConfigureAwait(false);
            await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            // Invalidation
            Computed.Invalidate(() => GetUserAsync(user.Id, CancellationToken.None));
            Computed.Invalidate(() => GetUserCountAsync(CancellationToken.None));
            return user;
        }

        public async Task<ChatUser> SetUserNameAsync(long id, string name, CancellationToken cancellationToken = default)
        {
            await using var dbContext = CreateDbContext();

            var user = await dbContext.ChatUsers.AsQueryable()
                .SingleAsync(u => u.Id == id, cancellationToken).ConfigureAwait(false);
            user.Name = name;
            await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            // Invalidation
            Computed.Invalidate(() => GetUserAsync(id, CancellationToken.None));
            return user;
        }

        public async Task<ChatMessage> AddMessageAsync(long userId, string text, CancellationToken cancellationToken = default)
        {
            await using var dbContext = CreateDbContext();

            await GetUserAsync(userId, cancellationToken).ConfigureAwait(false); // Check to ensure the user exists
            var message = new ChatMessage()
            {
                CreatedAt = DateTime.UtcNow,
                UserId = userId,
                Text = text,
            };
            await dbContext.ChatMessages.AddAsync(message, cancellationToken).ConfigureAwait(false);
            await dbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

            // Invalidation
            Computed.Invalidate(EveryChatTail);
            return message;
        }

        // Readers

        public virtual async Task<long> GetUserCountAsync(CancellationToken cancellationToken = default)
        {
            await using var dbContext = CreateDbContext();
            return await dbContext.ChatUsers.AsQueryable().LongCountAsync(cancellationToken).ConfigureAwait(false);
        }

        public virtual Task<long> GetActiveUserCountAsync(CancellationToken cancellationToken = default)
        {
            var channelHub = _publisher.ChannelHub;
            var userCount = (long)channelHub.ChannelCount;
            var c = Computed.GetCurrent();
            Task.Run(async () => {
                do
                {
                    await Task.Delay(1000, default).ConfigureAwait(false);
                } while (userCount == channelHub.ChannelCount);
                c!.Invalidate();
            }, default).Ignore();
            return Task.FromResult(Math.Max(0, userCount));
        }

        public virtual Task<ChatUser> GetUserAsync(long id, CancellationToken cancellationToken = default)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Email).Value;
            return new Task<ChatUser>(() => new ChatUser() {Id = 1, Name = userId});
        }

        public virtual async Task<ChatPage> GetChatTailAsync(int length, CancellationToken cancellationToken = default)
        {
            await EveryChatTail().ConfigureAwait(false);
            await using var dbContext = CreateDbContext();

            // Fetching messages from DB
            var messages = await dbContext.ChatMessages.AsQueryable()
                .OrderByDescending(m => m.Id)
                .Take(length)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
            messages.Reverse();

            // Fetching users via GetUserAsync
            var userIds = messages.Select(m => m.UserId).Distinct().ToArray();
            var userTasks = userIds.Select(id => GetUserAsync(id, cancellationToken));
            var users = await Task.WhenAll(userTasks).ConfigureAwait(false);

            // Composing the end result
            return new ChatPage(messages, users.ToDictionary(u => u.Id));
        }

        public virtual Task<ChatPage> GetChatPageAsync(long minMessageId, long maxMessageId, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        // Helpers

        [ComputeMethod]
        protected virtual Task<Unit> EveryChatTail() => TaskEx.UnitTask;
       
    }
}
