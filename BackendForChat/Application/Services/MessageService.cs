using BackendForChat.Application.Common;
using BackendForChat.Models.DatabaseContext;
using BackendForChat.Models.DTO.Requests;
using BackendForChat.Models.DTO.Response;
using BackendForChat.Models.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BackendForChat.Application.Services
{
    public class MessageService
    {
        private readonly ApplicationDbContext _context;
        private readonly EncryptionService _encryptionService;

        public MessageService(ApplicationDbContext context, EncryptionService encryptionService)
        {
            _context = context;
            _encryptionService = encryptionService;
        }

        public async Task<ServiceResult<ResponseMessageDto>> GetMessageById(int id, Guid userId)
        {
            var message = await _context.Messages
                .Include(m => m.Chat)
                .ThenInclude(c => c.ChatUsers)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (message == null)
            {
                return ServiceResult<ResponseMessageDto>.Fail("Message with that id doesn't exist");
            }
            if (!message.Chat.ChatUsers.Any(user => user.User.Id == userId))
            {
                return ServiceResult<ResponseMessageDto>.Fail("User doesn't belong to chat");
            }

            ResponseMessageDto responseMessage = new ResponseMessageDto
            {
                Id = id,
                ChatId = message.ChatId,
                Content = _encryptionService.Decrypt(message.Content),
                SenderId = message.SenderId,
                CreatedAt = message.CreatedAt,
            };
            return ServiceResult<ResponseMessageDto>.Ok(responseMessage);
        }

        public async Task<ServiceResult<List<ResponseMessageDto>>> GetMessagesPaged(int page, int pageSize, Guid userId, Guid chatId)
        {
            bool userInChat = await _context.ChatUsers
                .AnyAsync(uc => uc.ChatId == chatId && uc.UserId == userId);
            if (!userInChat)
            {
                return ServiceResult<List<ResponseMessageDto>>.Fail("User doesn't belong to chat");
            }

            var messages = await _context.Messages
               .Where(message => message.ChatId == chatId)
               .OrderByDescending(message => message.CreatedAt) // Last message first
               .Skip((page - 1) * pageSize)
               .Take(pageSize)
               .ToListAsync();
            if (messages.Count == 0)
                return ServiceResult<List<ResponseMessageDto>>.Fail("Messages with that paged parameters doesn't exist");

            List<ResponseMessageDto> responseMessages = messages.Select(m => new ResponseMessageDto
            {
                Id = m.Id,
                ChatId = m.ChatId,
                Content = _encryptionService.Decrypt(m.Content),
                SenderId = m.SenderId,
                CreatedAt = m.CreatedAt
            }).ToList();

            return ServiceResult<List<ResponseMessageDto>>.Ok(responseMessages);
        }

        public async Task<ServiceResult<ResponseMessageDto>> SaveMessageAsync(RequestMessageDto newMessage, Guid userId)
        {
            bool userInChat = await _context.ChatUsers
                .AnyAsync(uc => uc.ChatId == newMessage.ChatId && uc.UserId == userId);
            if (!userInChat)
            {
                return ServiceResult<ResponseMessageDto>.Fail("User doesn't belong to chat");
            }

            MessageModel message = new MessageModel
            {
                ChatId = newMessage.ChatId,
                Content = _encryptionService.Encrypt(newMessage.Content),
                SenderId = userId,
                CreatedAt = DateTime.UtcNow
            };
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
            ResponseMessageDto responseMessage = new ResponseMessageDto
            {
                Id = message.Id,
                ChatId = message.ChatId,
                Content = _encryptionService.Decrypt(message.Content),
                SenderId = message.SenderId,
                CreatedAt = message.CreatedAt
            };
            return ServiceResult<ResponseMessageDto>.Ok(responseMessage);
        }
    }
}
