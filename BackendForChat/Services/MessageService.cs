using BackendForChat.Models;
using BackendForChat.Models.DatabaseContext;
using BackendForChat.Models.DTO;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendForChat.Services
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
        public async Task<MessageModel> GetMessageById(int id)
        {
            var message = await _context.Messages.FindAsync(id);
            return message;
        }
        public async Task<List<MessageModel>> GetMessagesPaged(int page, int pageSize)
        {
            var messages = await _context.Messages
               .OrderByDescending(message => message.CreatedAt) // Последние сообщения первыми
               .Skip((page - 1) * pageSize)
               .Take(pageSize)
               .ToListAsync();
            return messages;
        }

        public async Task<ResponseMessageDto> SaveMessageAsync(RequestMessageDto newMessage, Guid userId)
        {

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
                ChatId = newMessage.ChatId,
                Content = newMessage.Content, //Return decrypted
                SenderId = userId,
                CreatedAt = message.CreatedAt
            };
            return responseMessage;
        }
    }
}
