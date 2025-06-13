using BrassLoon.CommonCore;
using BrassLoon.WorkTask.Core;
using BrassLoon.WorkTask.Framework;
using Moq;
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
            _ = workTaskType.SetupGet(wt => wt.WorkTaskTypeId).Returns(workTaskTypeId);
            _ = workTaskType.SetupGet(wt => wt.Statuses).Returns(() =>
            {
                Mock<IWorkTaskStatus> status = new Mock<IWorkTaskStatus>();
                _ = status.SetupGet(s => s.WorkTaskStatusId).Returns(targetStatusId);
                _ = status.SetupGet(s => s.WorkTaskTypeId).Returns(workTaskTypeId);
                return new List<IWorkTaskStatus> { status.Object };
            });
            Mock<IWorkTaskStatus> workTaskStatus = new Mock<IWorkTaskStatus>();
            _ = workTaskStatus.SetupGet(wts => wts.WorkTaskStatusId).Returns(Guid.NewGuid());
            Mock<IWorkTask> workTask = new Mock<IWorkTask>();
            _ = workTask.SetupAllProperties();
            _ = workTask.SetupGet(wt => wt.DomainId).Returns(domainId);
            _ = workTask.SetupGet(wt => wt.WorkTaskType).Returns(workTaskType.Object);
            workTask.Object.WorkTaskStatus = workTaskStatus.Object;

            Mock<IWorkTaskFactory> workTaskFactory = new Mock<IWorkTaskFactory>();
            _ = workTaskFactory.Setup(f => f.Get(It.IsAny<ISettings>(), domainId, workTaskId)).Returns(() => Task.FromResult(workTask.Object));

            WorkTaskPatcher workTaskPatcher = new WorkTaskPatcher(workTaskFactory.Object);
            IEnumerable<IWorkTask> result = await workTaskPatcher.Apply(null, domainId, patchData);
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(targetStatusId, result.First().WorkTaskStatus.WorkTaskStatusId);
#pragma warning restore IDE0028 // Simplify collection initialization
        }
    }
}
