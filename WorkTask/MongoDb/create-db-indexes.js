function CreateIndex(collection, keys, options) {
    let allIndexes = collection.getIndexes();
    var create = true;
    for (var index of allIndexes) {    
        if (index["name"] == options["name"]) {
            create = false;
        }
    }
    if (create) {
        print(`Create index ${options["name"]}`);
        collection.createIndex(keys, options);
    }
}
db = db.getSiblingDB("dev-bl-work-task");
// WorkGroup
CreateIndex(db.WorkGroup, { "DomainId": 1 }, { "name": "IX_WorkGroup_DomainId" });
// WorkTask
CreateIndex(db.WorkTask, { "DomainId": 1, "WorkTaskTypeId": 1 }, { "name": "IX_WorkTask_DomainId" });
CreateIndex(db.WorkTask, { "WorkTaskStatusId": 1 }, { "name": "IX_WorkTask_WorkTaskStatusId" });
// WorkTaskComment
CreateIndex(db.WorkTaskComment, { "WorkTaskId": 1 }, { "name": "IX_WorkTaskComment_WorkTaskId" });
// WorkTaskContext
CreateIndex(db.WorkTaskContext, { "DomainId": 1 }, { "name": "IX_WorkTaskContext_DomainId" });
CreateIndex(db.WorkTaskContext, { "WorkTaskId": 1 }, { "name": "IX_WorkTaskContext_WorkTaskId" });
CreateIndex(db.WorkTaskContext, { "DomainId": 1, "ReferenceType": 1, "ReferenceValueHash": 1 }, { "name": "IX_WorkTaskContext_DomainId_ReferenceType_ReferenceValueHash", "sparse": true });
// WorkTaskType
CreateIndex(db.WorkTaskType, { "DomainId": 1 }, { "name": "IX_WorkTaskType_DomainId" });
CreateIndex(db.WorkTaskType, { "DomainId": 1, "Code": 1 }, { "name": "IX_WorkTaskType_DomainId_Code", "unique": true });
// WorkTaskTypeGroup
CreateIndex(db.WorkTaskTypeGroup, { "WorkTaskTypeId": 1 }, { "name": "IX_WorkTaskTypeGroup_WorkTaskTypeId" });
CreateIndex(db.WorkTaskTypeGroup, { "WorkGroupId": 1 }, { "name": "IX_WorkTaskTypeGroup_WorkGroupId" });