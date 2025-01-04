db = db.getSiblingDB("dev-bl-log");
var collections = db.getCollectionNames();
print("collections")
printjson(collections);
print("trace")
var trace = db.Trace.find({ "CreateTimestamp": { $gt: ISODate('2025-01-04T13:27:53.905Z') } }).sort({ "CreateTimestamp": -1 });
printjson(trace);