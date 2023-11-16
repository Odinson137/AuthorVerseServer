﻿using AuthorVerseServer.Data.Enums;
using AuthorVerseServer.DTO;

namespace AuthorVerseServer.Interfaces
{
    public interface IAccount
    {
        Task<UserProfileDTO> GetUserAsync(string userId);
        Task<ICollection<UserSelectedBookDTO>> GetUserSelectedBooksAsync(string userId); // то что читает пользователь
        Task<int> GetCommentsPagesCount(CommentType commentType, int page, string searchComment);
        Task<ICollection<CommentProfileDTO>> GetUserCommentsAsync(CommentType commentType, int page, string searchComment);
        Task<ICollection<FriendDTO>> GetUserFriendsAsync(string userId);
        Task<ICollection<UserBookDTO>> GetUserBooksAsync(string userId); // то что сам пишет

        Task<ICollection<UpdateAccountBook>> CheckUserUpdatesAsync(string userId); // должен выводить последние вышедшие главы, если юзер их ещё не прочитал
    }
}
