using MediatR;
using my_app_backend.Application.QueryRepositories;
using my_app_backend.Domain.AggregateModel.BookAggregate.Events;
using my_app_backend.Models;
using Newtonsoft.Json;

namespace my_app_backend.Application.EventHandlers
{
    public class DeleteDeletedEventHandler : INotificationHandler<BookDeletedEvent>
    {
        private readonly IBookRepository _bookRepository;
        private readonly ILogger<DeleteDeletedEventHandler> _logger;
        public DeleteDeletedEventHandler(IBookRepository bookRepository, ILogger<DeleteDeletedEventHandler> logger)
        {
            _bookRepository = bookRepository;
            _logger = logger;
        }

        public async Task Handle(BookDeletedEvent notification, CancellationToken cancellationToken)
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
                
                var deleteRs = await _bookRepository.Delete(notification.BookId);
                if (!deleteRs.IsSuccessful)
                {
                    throw new Exception(deleteRs.Message);
                }
            }
            catch (Exception ex)
            {
                _logger.Equals($"Exception happened: sync to read repository fail for BookCreatedEvent: {JsonConvert.SerializeObject(notification)}, ex: {ex}");
            }
        }
        
    }
}
