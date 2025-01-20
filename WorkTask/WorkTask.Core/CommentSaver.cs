using BrassLoon.CommonCore;
using BrassLoon.WorkTask.Framework;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Core
{
    public class CommentSaver : ICommentSaver
    {
        public Task Create(ISettings settings, params IComment[] comments)
        {
            if (comments != null && comments.Length > 0)
            {
                return Saver.Save(
                    new SaveSettings(settings),
                    async ss =>
                    {
                        for (int i = 0; i < comments.Length; i += 1)
                        {
                            await comments[i].Create(ss);
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
