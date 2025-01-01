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
db = db.getSiblingDB("dev-bl-log");
// EventId
CreateIndex(db.EventId, { "DomainId": 1, "Id": 1, "Name": 1 }, { "name": "IX_EventId_DomainId", "unique": true });
// Exception
CreateIndex(db.Exception, { "ParentExceptionId": 1 }, { "name": "IX_Exception_ParentExceptionId" });
CreateIndex(db.Exception, { "DomainId": 1, "Id": 1, "CreateTimestamp": -1 }, { "name": "IX_Exception_DomainId" });
CreateIndex(db.Exception, { "EventId": 1 }, { "name": "IX_Exception_EventId" });
// Metric
CreateIndex(db.Metric, { "DomainId": 1, "EventCode": 1, "CreateTimestamp": -1 }, { "name": "IX_Metric_DomainId" });
CreateIndex(db.Metric, { "EventId": 1 }, { "name": "IX_Metric_EventId" });
// Trace
CreateIndex(db.Trace, { "DomainId": 1, "EventCode": 1, "CreateTimestamp": -1 }, { "name": "IX_Trace_DomainId" });
CreateIndex(db.Trace, { "EventId": 1 }, { "name": "IX_Trace_EventId" });
