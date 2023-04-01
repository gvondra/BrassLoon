using BrassLoon.CommonCore;
using BrassLoon.WorkTask.Framework;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Core
{
    public class CommentSaver : ICommentSaver
    {
        private readonly Saver _saver;

        public CommentSaver(Saver saver)
        {
            _saver = saver;
        }

        public Task Create(ISettings settings, params IComment[] comments)
        {
            if (comments != null && comments.Length > 0)
            {
                return _saver.Save(new TransactionHandler(settings), async th =>
                {
                    for (int i = 1; i < comments.Length; i += 1)
                    {
                        await comments[i].Create(th);
                    }
                });
            }
            else
            {
                return Task.CompletedTask;
            }
        }
    }
}
