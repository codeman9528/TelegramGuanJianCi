public class MyCollector : IPeerCollector
{
    private readonly IDictionary<long, User> _users;
    private readonly Services.CollectorPeer _inner;

    public MyCollector(IDictionary<long, User> users, IDictionary<long, ChatBase> chats)
    {
        _users = users;
        _inner = new Services.CollectorPeer(users, chats);
    }

    public bool HasUser(long id) => _inner.HasUser(id);

    public bool HasChat(long id) => _inner.HasChat(id);

    public void Collect(IEnumerable<TL.User> users)
    {
        lock (_users)
            foreach (var user in users)
                if (user != null)
                    if (!user.flags.HasFlag(User.Flags.min) || !_users.TryGetValue(user.id, out var prevUser) || prevUser.flags.HasFlag(User.Flags.min))
                        _users[user.id] = user;
                    else
                    {
                        const User.Flags updated_flags = (User.Flags)0x5DAFE000;
                        const User.Flags2 updated_flags2 = (User.Flags2)0x711;
                        prevUser.flags = (prevUser.flags & ~updated_flags) | (user.flags & updated_flags);
                        prevUser.flags2 = (prevUser.flags2 & ~updated_flags2) | (user.flags2 & updated_flags2);
                        prevUser.first_name ??= user.first_name;
                        prevUser.last_name ??= user.last_name; prevUser.phone ??= user.phone;
                        if (prevUser.flags.HasFlag(User.Flags.apply_min_photo) && user.photo != null)
                        {
                            prevUser.photo = user.photo; prevUser.flags |= User.Flags.has_photo;
                        }
                        prevUser.bot_info_version = user.bot_info_version;
                        prevUser.restriction_reason = user.restriction_reason;
                        prevUser.bot_inline_placeholder = user.bot_inline_placeholder;

                        if (user.lang_code != null)
                            prevUser.lang_code = user.lang_code; prevUser.emoji_status = user.emoji_status;
                        if (user.username != null)
                            prevUser.username = user.username;

                        if (user.usernames != null)
                            prevUser.usernames = user.usernames;
                        if (user.stories_max_id > 0)
                            prevUser.stories_max_id = user.stories_max_id; prevUser.color = user.color;
                        prevUser.profile_color = user.profile_color; _users[user.id] = prevUser;
                    }
    }

    public void Collect(IEnumerable<ChatBase> chats)
    {
        _inner.Collect(chats);
    }
}