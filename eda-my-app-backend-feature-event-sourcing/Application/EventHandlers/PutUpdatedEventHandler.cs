using MediatR;
using my_app_backend.Application.QueryRepositories;
using my_app_backend.Domain.AggregateModel.BookAggregate.Events;
using my_app_backend.Models;
using Newtonsoft.Json;

namespace my_app_backend.Application.EventHandlers
{
    public class PutUpdatedEventHandler : INotificationHandler<BookUpdatedEvent>
    {
        private readonly IBookRepository _bookRepository;
        private readonly ILogger<PutUpdatedEventHandler> _logger;
        public PutUpdatedEventHandler(IBookRepository bookRepository, ILogger<PutUpdatedEventHandler> logger)
        {
            _bookRepository = bookRepository;
            _logger = logger;
        }

        public async Task Handle(BookUpdatedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                var rs = await _bookRepository.GetById(notification.BookId);
                if (!rs.IsSuccessful)
                {
                    throw new Exception(rs.Message);
                }
                var book = rs.Data;
                book.Id = notification.BookId;
                book.Author = notification.Author;
                book.Type = notification.Type;
                book.Name = notification.Name;
                book.Locked = notification.Locked;
                book.Quantity = rs.Data.Quantity;
                book.InventoryHistories = new List<BookInventoryHistoryDto>();
                
                var updateRs = await _bookRepository.Update(book);
                if (!updateRs.IsSuccessful)
                {
                    throw new Exception(updateRs.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.Equals($"Exception happened: sync to read repository fail for BookCreatedEvent: {JsonConvert.SerializeObject(notification)}, ex: {ex}");
            }
        }
        
    }
}
