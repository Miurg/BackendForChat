using BackendForChat.Application.Common;
using BackendForChat.Models.DatabaseContext;
using BackendForChat.Models.DTO.Response;
using BackendForChat.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BackendForChat.Application.Services
{
    public class ChatService
    {
        private readonly ApplicationDbContext _context;

        public ChatService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<ServiceResult<ResponseChatCreateDto>> FindPrivateChatByUsersAsync (Guid firtsUserId, Guid secondUserId)
        {
            var chatType = await _context.ChatTypes
                .FirstOrDefaultAsync(ct => ct.Type == "Private");

            if (chatType == null)
            {
                throw new Exception("Chat type not found");
            }
            ChatModel chat = await _context.Chats
                .Where(chat => chat.ChatType.Id == chatType.Id)
                .Where(chat => chat.ChatUsers.Any(uc => uc.UserId == firtsUserId) &&
                      chat.ChatUsers.Any(uc => uc.UserId == secondUserId))
                .FirstOrDefaultAsync();
            if (chat == null)
                return ServiceResult<ResponseChatCreateDto>.Fail("Chat with this user does not exist");
            ResponseChatCreateDto chatResponse = new ResponseChatCreateDto { ChatId = chat.Id };
            return ServiceResult<ResponseChatCreateDto>.Ok(chatResponse);
        }

        public async Task<ServiceResult<ResponseChatCreateDto>> CreatePrivateChatAsync(Guid firtsUserId, Guid secondUserId)
        {
            var chatType = await _context.ChatTypes
                .FirstOrDefaultAsync(ct => ct.Type == "Private"); 

            if (chatType == null)
            {
                throw new Exception("Chat type not found"); 
            }

            var chat = new ChatModel
            {
                ChatTypeId = chatType.Id, 
                ChatType = chatType 
            };

            var chatUser1 = new ChatUserModel
            {
                UserId = firtsUserId,
                Chat = chat
            };

            var chatUser2 = new ChatUserModel
            {
                UserId = secondUserId,
                Chat = chat
            };

            chat.ChatUsers = new List<ChatUserModel> { chatUser1, chatUser2 };

            _context.Chats.Add(chat);
            await _context.SaveChangesAsync();
            ResponseChatCreateDto chatResponse = new ResponseChatCreateDto { ChatId = chat.Id };
            return ServiceResult<ResponseChatCreateDto>.Ok(chatResponse);
        }
        

        //public async Task<ResponseChatCreateDto> CreateGroupChatAsync(List<Guid> userIds)
        //{
        //    var chatType = await _context.ChatTypes
        //        .FirstOrDefaultAsync(ct => ct.Type == "Group");

        //    if (chatType == null)
        //    {
        //        throw new Exception("Chat type not found");
        //    }

        //    var chat = new ChatModel
        //    {
        //        ChatTypeId = chatType.Id,
        //        ChatType = chatType
        //    };

        //    var chatUsers = userIds.Select(userId => new ChatUserModel
        //    {
        //        UserId = userId,
        //        Chat = chat
        //    }).ToList();

        //    chat.ChatUsers = chatUsers;

        //    _context.Chats.Add(chat);
        //    await _context.SaveChangesAsync();
        //    ResponseChatCreateDto chatResponse = new ResponseChatCreateDto { ChatId = chat.Id };
        //    return chatResponse;
        //}

        public async Task<ServiceResult<ResponseChatCreateDto>> GetChatById(Guid id)
        {
            var chat = await _context.Chats.FindAsync(id);
            if (chat == null)
            {
                return ServiceResult<ResponseChatCreateDto>.Fail("Chat with that id does not exist");
            }
            ResponseChatCreateDto chatResponse = new ResponseChatCreateDto { ChatId = chat.Id };
            return ServiceResult<ResponseChatCreateDto>.Ok(chatResponse);
        }

    }
}
