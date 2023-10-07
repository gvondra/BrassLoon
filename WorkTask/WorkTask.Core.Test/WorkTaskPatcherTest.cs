using BrassLoon.CommonCore;
using BrassLoon.WorkTask.Core;
using BrassLoon.WorkTask.Framework;
using Moq;
using Newtonsoft;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorkTask.Core.Test
{
    [TestClass]
    public class WorkTaskPatcherTest
    {
        [TestMethod]
        public async Task ApplyTest()
        {
            Guid domainId = Guid.NewGuid();
            Guid workTaskId = Guid.NewGuid();
            Guid workTaskTypeId = Guid.NewGuid();
            Guid targetStatusId = Guid.NewGuid();

#pragma warning disable IDE0028 // Simplify collection initialization
            List<Dictionary<string, object>> patchData = new List<Dictionary<string, object>>();
            patchData.Add(new Dictionary<string, object>
            {
                { "WorkTaskId", workTaskId.ToString("D") },
                { "WorkTaskStatusId", targetStatusId.ToString("D") }
            });

            Mock<IWorkTaskType> workTaskType = new Mock<IWorkTaskType>();
            workTaskType.SetupGet(wt => wt.WorkTaskTypeId).Returns(workTaskTypeId);
            Mock<IWorkTaskStatus> workTaskStatus = new Mock<IWorkTaskStatus>();
            workTaskStatus.SetupGet(wts => wts.WorkTaskStatusId).Returns(Guid.NewGuid());
            Mock<IWorkTask> workTask = new Mock<IWorkTask>();
            workTask.SetupAllProperties();
            workTask.SetupGet(wt => wt.DomainId).Returns(domainId);
            workTask.SetupGet(wt => wt.WorkTaskType).Returns(workTaskType.Object);
            workTask.Object.WorkTaskStatus = workTaskStatus.Object;

            Mock<IWorkTaskStatusFactory> workTaskStatusFactory = new Mock<IWorkTaskStatusFactory>();
            workTaskStatusFactory.Setup(f => f.GetByWorkTaskTypeId(It.IsAny<ISettings>(), domainId, workTaskTypeId))
                .Returns((ISettings s, Guid dId, Guid id) =>
                {
                    Mock<IWorkTaskStatus> status = new Mock<IWorkTaskStatus>();
                    status.SetupGet(s => s.WorkTaskStatusId).Returns(targetStatusId);
                    status.SetupGet(s => s.WorkTaskTypeId).Returns(id);
                    return Task.FromResult<IEnumerable<IWorkTaskStatus>>(new List<IWorkTaskStatus> { status.Object });
                });

            Mock<IWorkTaskFactory> workTaskFactory = new Mock<IWorkTaskFactory>();
            workTaskFactory.Setup(f => f.Get(It.IsAny<ISettings>(), domainId, workTaskId)).Returns(() => Task.FromResult(workTask.Object));

            WorkTaskPatcher workTaskPatcher = new WorkTaskPatcher(workTaskFactory.Object, workTaskStatusFactory.Object);
            IEnumerable<IWorkTask> result = await workTaskPatcher.Apply(null, domainId, patchData);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(targetStatusId, result.First().WorkTaskStatus.WorkTaskStatusId);
#pragma warning restore IDE0028 // Simplify collection initialization
        }
    }
}
